using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.RepresentationModel;


namespace XCom
{
	/// <summary>
	/// A TilesetManager reads, stores, and manages all the tileset-data taken
	/// from the user-file MapTilesets.yml. It's the user-configuration for all
	/// the Maps.
	/// NOTE: Tilesets are converted into Descriptors and Tilesets are no longer
	/// used after loading is finished.
	/// </summary>
	public sealed class TilesetManager
	{
		#region Fields & Properties
		private List<Tileset> _tilesets = new List<Tileset>();
		internal List<Tileset> Tilesets
		{
			get { return _tilesets; }
		}

//		private readonly List<string> _types      = new List<string>();
//		private readonly List<string> _categories = new List<string>();

		private List<string> _terrains = new List<string>();
		internal List<string> Terrains
		{
			get { return _terrains; }
		}

		private readonly List<string> _groups = new List<string>();
		internal List<string> Groups
		{
			get { return _groups; }
		}

		internal string FullPath // TODO: might not be needed.
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="fullpath">path+file+extension of MapTilesets.yml</param>
		public TilesetManager(string fullpath)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("TilesetManager cTor");

			FullPath = fullpath;


			var progress = ProgressBarForm.Instance;
			progress.SetInfo("Parsing MapTilesets ...");

			var typeCount = 0; // TODO: optimize the reading (here & below) into a buffer.
			using (var reader = File.OpenText(fullpath))
			{
				string line = String.Empty;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Contains("- type"))
						++typeCount;
				}
			}
			progress.SetTotal(typeCount);


			// TODO: if exists(pfe)
			// else error out.

			bool isUfoConfigured  = !String.IsNullOrEmpty(SharedSpace.Instance.GetShare(SharedSpace.ResourceDirectoryUfo));
			bool isTftdConfigured = !String.IsNullOrEmpty(SharedSpace.Instance.GetShare(SharedSpace.ResourceDirectoryTftd));

			using (var sr = new StreamReader(File.OpenRead(fullpath)))
			{
				var str = new YamlStream();
				str.Load(sr);

				var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
//				foreach (var node in nodeRoot.Children) // iterate over all the tilesets
//				{
				//LogFile.WriteLine(". node.Key(ScalarNode)= " + (YamlScalarNode)node.Key); // "tilesets"

				var nodeTilesets = nodeRoot.Children[new YamlScalarNode("tilesets")] as YamlSequenceNode;
				foreach (YamlMappingNode nodeTileset in nodeTilesets)
				{
					//LogFile.WriteLine(". . tileset= " + tileset); // lists all data in the tileset

					// IMPORTANT: ensure that tileset-labels (ie, type) and terrain-labels
					// (ie, terrains) are stored and used only as UpperCASE strings.


					string nodeGroup = nodeTileset.Children[new YamlScalarNode("group")].ToString();
					//LogFile.WriteLine(". . group= " + nodeGroup); // eg. "ufoShips"

					if (   (!isUfoConfigured  && nodeGroup.StartsWith("ufo",  StringComparison.OrdinalIgnoreCase))
						|| (!isTftdConfigured && nodeGroup.StartsWith("tftd", StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}

					if (!Groups.Contains(nodeGroup))
						Groups.Add(nodeGroup);


					string nodeCategory = nodeTileset.Children[new YamlScalarNode("category")].ToString();
					//LogFile.WriteLine(". . category= " + nodeCategory); // eg. "Ufo"
//					if (!_categories.Contains(nodeCategory))
//						_categories.Add(nodeCategory);


					string nodeLabel = nodeTileset.Children[new YamlScalarNode("type")].ToString();
					nodeLabel = nodeLabel.ToUpperInvariant();
					//LogFile.WriteLine(". . type= " + nodeLabel); // eg. "UFO_110"
//					if (!_types.Contains(nodeLabel))
//						_types.Add(nodeLabel);


					var terrainList = new List<string>();

					var nodeTerrains = nodeTileset.Children[new YamlScalarNode("terrains")] as YamlSequenceNode;
					foreach (YamlScalarNode nodeTerrain in nodeTerrains)
					{
						//LogFile.WriteLine(". . . terrain= " + nodeTerrain); // eg. "U_EXT02" etc.

						string terrain = nodeTerrain.ToString();
						terrain = terrain.ToUpperInvariant();

						terrainList.Add(terrain);

						if (!Terrains.Contains(terrain)) // TODO: this is probly irrelevant since YAML etc.
						{
							//LogFile.WriteLine(". . . . adding terrain");
							Terrains.Add(terrain);
						}
					}


					string nodeBasepath = String.Empty;
					var basepath = new YamlScalarNode("basepath");
					if (nodeTileset.Children.ContainsKey(basepath))
					{
						nodeBasepath = nodeTileset.Children[basepath].ToString();
						//LogFile.WriteLine(". . basepath= " + nodeBasepath);
//						if (!Groups.Contains(nodeBasepath))
//							Groups.Add(nodeBasepath);
					}
					//else LogFile.WriteLine(". . basepath not found.");


					var tileset = new Tileset(
											nodeLabel,
											nodeGroup,
											nodeCategory,
											terrainList,
											nodeBasepath);
					Tilesets.Add(tileset);

					progress.UpdateProgress();
				}
			}
			progress.Hide();
		}
		#endregion
	}
}
