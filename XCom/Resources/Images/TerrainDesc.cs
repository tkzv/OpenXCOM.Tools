using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public sealed class TerrainDesc
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, TerrainDescriptor> _terrainsDictionary;
		public TerrainDescriptor this[string label]
		{
			get
			{
				string key = label.ToUpperInvariant();
				return (_terrainsDictionary.ContainsKey(key)) ? _terrainsDictionary[key]
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
		/// cTor.
		/// </summary>
		/// <param name="pfe"></param>
		/// <param name="vars"></param>
		internal TerrainDesc(string pfe, Varidia vars)
		{
			_path = pfe;
			_terrainsDictionary = new Dictionary<string, TerrainDescriptor>();

			Load(pfe, vars);
		}
		#endregion


		#region Methods
		private void Load(string pfe, Varidia vars)
		{
			using (var sr = new StreamReader(File.OpenRead(pfe)))
			{
				vars = new Varidia(sr, vars);

				KeyvalPair keyval;
				while ((keyval = vars.ReadLine()) != null)
				{
					var terrain = new TerrainDescriptor(keyval.Keyword.ToUpperInvariant(), keyval.Value);
					_terrainsDictionary[keyval.Keyword.ToUpperInvariant()] = terrain;
				}
			}
		}

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
						if (!vars.ContainsKey(terrain.Path))
							vars[terrain.Path] = new Variable(terrain.Label + ":", terrain.Path);
						else
							vars[terrain.Path].Add(terrain.Label + ":");
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
			private readonly Dictionary<string, TerrainDescriptor> _terrainsDictionary;
			#endregion


			#region Properties
			public TerrainDescriptor this[string terrain]
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
			internal TerrainAccessor(Dictionary<string, TerrainDescriptor> terrainsDictionary)
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
