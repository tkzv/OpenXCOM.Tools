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
		private readonly Dictionary<string, Tileset> _tilesetDictionary = new Dictionary<string, Tileset>();
		internal Dictionary<string, Tileset> Tilesets
		{
			get { return _tilesetDictionary; }
		}

		private readonly List<string> _types      = new List<string>();
		private readonly List<string> _categories = new List<string>();

		private readonly List<string> _terrains   = new List<string>();
		internal List<string> Terrains
		{
			get { return _terrains; }
		}

		private readonly List<string> _groups     = new List<string>();
		internal List<string> Groups
		{
			get { return _groups; }
		}

		private readonly string _fullpath;
		internal string FullPath
		{
			get { return _fullpath; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. 
		/// </summary>
		/// <param name="pfe">path+file+extension of MapConfig.yml</param>
		public TilesetManager(string pfe)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TilesetManager cTor");

			_fullpath = pfe;

			// TODO: if exists(pfe)
			// else error out.

			using (var sr = new StreamReader(File.OpenRead(pfe)))
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

					string nodeType = nodeTileset.Children[new YamlScalarNode("type")].ToString();
					LogFile.WriteLine(". . type= " + nodeType); // "UFO_110"
					if (!_types.Contains(nodeType)) // safety. There shall be only 1 tileset of any type in YAML.
						_types.Add(nodeType);

					string nodeGroup = nodeTileset.Children[new YamlScalarNode("group")].ToString();
					LogFile.WriteLine(". . group= " + nodeGroup); // "ufoShips"
					if (!_groups.Contains(nodeGroup))
						_groups.Add(nodeGroup);

					string nodeCategory = nodeTileset.Children[new YamlScalarNode("category")].ToString();
					LogFile.WriteLine(". . category= " + nodeCategory); // "Ufo"
					if (!_categories.Contains(nodeCategory))
						_categories.Add(nodeCategory);


					var terrainList = new List<string>();

					var nodeTerrains = nodeTileset.Children[new YamlScalarNode("terrains")] as YamlSequenceNode;
					foreach (YamlScalarNode nodeTerrain in nodeTerrains)
					{
						LogFile.WriteLine(". . . terrain= " + nodeTerrain); // "U_EXT02" etc.

						string st = nodeTerrain.ToString();

						terrainList.Add(st);

						if (!_terrains.Contains(st))
						{
							LogFile.WriteLine(". . . . adding terrain");
							_terrains.Add(st);
						}
					}

					var tileset = new Tileset(
											nodeType,
											nodeGroup,
											nodeCategory,
											terrainList);
					_tilesetDictionary.Add(nodeType, tileset);
				}
//				}
			}
		}
		#endregion
	}
}
