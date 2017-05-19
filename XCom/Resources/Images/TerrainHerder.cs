using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public sealed class TerrainHerder
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, Terrain> _terrainsDictionary = new Dictionary<string, Terrain>();
		public Terrain this[string label]
		{
			get
			{
				string labelUc = label.ToUpperInvariant();
				return (_terrainsDictionary.ContainsKey(labelUc)) ? _terrainsDictionary[labelUc]
																  : null;
			}
			set { _terrainsDictionary[label.ToUpperInvariant()] = value; }
		}

		public TerrainAccessor Terrains
		{
			get { return new TerrainAccessor(_terrainsDictionary); }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Loads from YAML.
		/// </summary>
		internal TerrainHerder(TilesetManager tilesetManager)
		{
			_path = tilesetManager.FullPath; // TODO: not right. not needed.

			foreach (string terrain in tilesetManager.Terrains)
			{
				string terrainUc = terrain.ToUpperInvariant();

				if (!_terrainsDictionary.ContainsKey(terrainUc))
					_terrainsDictionary[terrainUc] = new Terrain(terrainUc);
			}
		}



		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="pfe"></param>
		/// <param name="vars"></param>
		internal TerrainHerder(string pfe, Varidia vars)
		{
			_path = pfe;

			using (var sr = new StreamReader(File.OpenRead(pfe)))
			{
				vars = new Varidia(sr, vars);

				KeyvalPair keyval;
				while ((keyval = vars.ReadLine()) != null)
				{
					var terrain = new Terrain(keyval.Keyword.ToUpperInvariant(), keyval.Value);
					_terrainsDictionary[keyval.Keyword.ToUpperInvariant()] = terrain;
				}
			}
		}
		#endregion


		#region Methods
		public void Save(string pfe)
		{
			using (var sw = new StreamWriter(pfe))
			{
				var keys = new List<string>(_terrainsDictionary.Keys);
				keys.Sort();
				var vars = new Dictionary<string, Variable>();

				foreach (string key in keys)
				{
					if (_terrainsDictionary[key] != null)
					{
						var terrain = _terrainsDictionary[key];
						if (!vars.ContainsKey(terrain.PathDirectory))
							vars[terrain.PathDirectory] = new Variable(terrain.Label + ":", terrain.PathDirectory);
						else
							vars[terrain.PathDirectory].Add(terrain.Label + ":");
					}
				}

				foreach (string path in vars.Keys)
					vars[path].Write(sw);
			}
		}
		#endregion



		/// <summary>
		/// Ensures terrains are accessed with uppercase keys.
		/// good lord ....
		/// </summary>
		public sealed class TerrainAccessor
		{
			#region Fields
			private readonly Dictionary<string, Terrain> _terrainsDictionary;
			#endregion


			#region Properties
			public Terrain this[string terrain]
			{
				get { return _terrainsDictionary[terrain.ToUpperInvariant()]; }
			}

			public IEnumerable<string> Keys
			{
				get { return _terrainsDictionary.Keys; }
			}

//			public IEnumerable<TerrainDescriptor> TerrainDescriptors
//			{
//				get { return _terrainsDictionary.Values; }
//			}
			#endregion


			#region cTor
			/// <summary>
			/// cTor.
			/// </summary>
			/// <param name="terrainsDictionary"></param>
			internal TerrainAccessor(Dictionary<string, Terrain> terrainsDictionary)
			{
				_terrainsDictionary = terrainsDictionary;
			}
			#endregion


			#region Methods
			public void Remove(string key)
			{
				_terrainsDictionary.Remove(key.ToUpperInvariant());
			}
			#endregion
		}
	}
}
