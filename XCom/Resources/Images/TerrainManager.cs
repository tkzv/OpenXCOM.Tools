using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public sealed class TerrainManager
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, Terrain> _terrains = new Dictionary<string, Terrain>();
		public Terrain this[string label]
		{
			get
			{
				string labelUC = label.ToUpperInvariant();
				return (_terrains.ContainsKey(labelUC)) ? _terrains[labelUC]
														: null;
			}
			set { _terrains[label.ToUpperInvariant()] = value; }
		}

		public TerrainAccessor Terrains
		{
			get { return new TerrainAccessor(_terrains); }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Loads from YAML.
		/// </summary>
		internal TerrainManager(TilesetManager tilesetManager)
		{
			_path = tilesetManager.FullPath; // TODO: not right. not needed.

			foreach (string terrain in tilesetManager.Terrains)
			{
				string terrainUC = terrain.ToUpperInvariant();

				if (!_terrains.ContainsKey(terrainUC))
					_terrains[terrainUC] = new Terrain(terrainUC);
			}
		}
		#endregion


		#region Methods
		public void SaveTerrain(string pfe)
		{
//			using (var sw = new StreamWriter(pfe)) // TODO: update to exploit YAML.
//			{
//				var keys = new List<string>(_terrains.Keys);
//				keys.Sort();
//				var vars = new Dictionary<string, Variable>();
//
//				foreach (string key in keys)
//				{
//					if (_terrains[key] != null)
//					{
//						var terrain = _terrains[key];
//						if (!vars.ContainsKey(terrain.PathDir))
//							vars[terrain.PathDir] = new Variable(terrain.Label + ":", terrain.PathDir);
//						else
//							vars[terrain.PathDir].Add(terrain.Label + ":");
//					}
//				}
//
//				foreach (string path in vars.Keys)
//					vars[path].Write(sw);
//			}
		}
		#endregion



		/// <summary>
		/// Ensures terrains are accessed with uppercase keys.
		/// good lord ....
		/// </summary>
		public sealed class TerrainAccessor
		{
			#region Fields
			private readonly Dictionary<string, Terrain> _terrains;
			#endregion


			#region Properties
			public Terrain this[string terrain]
			{
				get { return _terrains[terrain.ToUpperInvariant()]; }
			}

			public IEnumerable<string> Keys
			{
				get { return _terrains.Keys; }
			}

//			public IEnumerable<TerrainDescriptor> TerrainDescriptors
//			{
//				get { return _terrains.Values; }
//			}
			#endregion


			#region cTor
			/// <summary>
			/// cTor.
			/// </summary>
			/// <param name="terrainsDictionary"></param>
			internal TerrainAccessor(Dictionary<string, Terrain> terrainsDictionary)
			{
				_terrains = terrainsDictionary;
			}
			#endregion


			#region Methods
			public void Remove(string key)
			{
				_terrains.Remove(key.ToUpperInvariant());
			}
			#endregion
		}
	}
}
