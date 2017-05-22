using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.RepresentationModel;


namespace XCom
{
	/// <summary>
	/// A TilesetManager reads, stores, and manages all the tileset-data taken
	/// from the user-file MapConfig.yml. It's the user-configuration for all
	/// the Maps.
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

		internal string FullPath
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor. 
		/// </summary>
		/// <param name="fullpath">path+file+extension of MapConfig.yml</param>
		public TilesetManager(string fullpath)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TilesetManager cTor");

			FullPath = fullpath;


			var progress = ProgressBarForm.Instance;
			progress.SetInfo("Parsing MapConfig ...");

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

					string nodeLabel = nodeTileset.Children[new YamlScalarNode("type")].ToString();
					LogFile.WriteLine(". . type= " + nodeLabel); // "UFO_110"
//					if (!_types.Contains(nodeLabel))
//						_types.Add(nodeLabel);

					string nodeCategory = nodeTileset.Children[new YamlScalarNode("category")].ToString();
					LogFile.WriteLine(". . category= " + nodeCategory); // "Ufo"
//					if (!_categories.Contains(nodeCategory))
//						_categories.Add(nodeCategory);

					string nodeGroup = nodeTileset.Children[new YamlScalarNode("group")].ToString();
					LogFile.WriteLine(". . group= " + nodeGroup); // "ufoShips"
					if (!Groups.Contains(nodeGroup))
						Groups.Add(nodeGroup);


					var terrainList = new List<string>();

					var nodeTerrains = nodeTileset.Children[new YamlScalarNode("terrains")] as YamlSequenceNode;
					foreach (YamlScalarNode nodeTerrain in nodeTerrains)
					{
						LogFile.WriteLine(". . . terrain= " + nodeTerrain); // "U_EXT02" etc.

						string terrain = nodeTerrain.ToString();

						terrainList.Add(terrain);

						if (!Terrains.Contains(terrain))
						{
							LogFile.WriteLine(". . . . adding terrain");
							Terrains.Add(terrain);
						}
					}

					string nodeBasepath = String.Empty;
					var basepath = new YamlScalarNode("basepath");
					if (nodeTileset.Children.ContainsKey(basepath))
					{
						nodeBasepath = nodeTileset.Children[basepath].ToString();
						LogFile.WriteLine(". . basepath= " + nodeBasepath);
//						if (!Groups.Contains(nodeBasepath))
//							Groups.Add(nodeBasepath);
					}
					else LogFile.WriteLine(". . basepath not found.");


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
