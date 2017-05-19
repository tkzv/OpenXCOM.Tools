using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class GroupHerder
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, TileGroupBase> _tilegroups = new Dictionary<string, TileGroupBase>();
		public Dictionary<string, TileGroupBase> TileGroups
		{
			get { return _tilegroups; }
		}
		#endregion


		#region cTor
		internal GroupHerder(TilesetManager tilesetManager)
		{
			_path = tilesetManager.FullPath; // TODO: not right. not needed.

			foreach (string gruop in tilesetManager.Groups)
				_tilegroups[gruop] = new TileGroupChild(gruop, tilesetManager.Tilesets);
		}


/*		internal GroupHerder(string pfe, Varidia vars) // path to MapEdit.cfg
		{
			_path = pfe;

			_tilegroups = new Dictionary<string, TileGroupBase>();


			using (var sr = new StreamReader(File.OpenRead(pfe)))
			{
				var vars1 = new Varidia(sr, vars);

				int pos;
				string key, val, line;

				while ((line = vars1.ReadLine(sr)) != null) // will not return lines that start '$' (or whitespace lines)
				{
					pos = line.IndexOf(':');
					key = line.Substring(0, pos).ToUpperInvariant();
					val = line.Substring(pos + 1);

					switch (key)
					{
						case "TILESET":
							line = Varidia.ReadLine(sr, vars1);
							pos  = line.IndexOf(':');
							key  = line.Substring(0, pos).ToUpperInvariant();

							switch (key)
							{
								case "TYPE":
									switch (Int32.Parse(line.Substring(pos + 1), System.Globalization.CultureInfo.InvariantCulture))
									{
//										case 0:
//											_tilesets[name] = new Type0Tileset(name, sr, new Varidia(vars1));
//											break;
										case 1:
											_tilegroups[val] = new TileGroupChild(val, sr, new Varidia(vars1));
											break;
									}
									break;

								default:
									Console.WriteLine(string.Format(
																System.Globalization.CultureInfo.CurrentCulture,
																"Type line not found: {0}",
																line));
									break;
							}
							break;

//						case "VERSION":
//							version = double.Parse(name);
//							break;
	
						default:
							Console.WriteLine(string.Format(
														System.Globalization.CultureInfo.CurrentCulture,
														"Unknown line: {0}",
														line));
							break;
					}
				}
			}
		} */
		#endregion


		#region Methods
		public TileGroupBase AddTileGroup(
				string label,
				string pathMaps,
				string pathRoutes,
				string pathOccults)
		{
			var tilegroup = new TileGroupChild(label);

			tilegroup.MapDirectory    = pathMaps;
			tilegroup.RouteDirectory  = pathRoutes;
			tilegroup.OccultDirectory = pathOccults;

			return (_tilegroups[label] = tilegroup);
		}
		#endregion
	}
}

/*		/// <summary>
		/// WARNING: This doesn't appear to be used but has to be here to
		/// satisfy inheritance from FileDesc.Save() which is abstract.
		/// </summary>
		/// <param name="pfe"></param>
		public override void Save(string pfe)
		{
			var vars = new Varidia("Path"); // iterate thru each tileset, call save on them

			foreach (string key in _tilesets.Keys)
			{
				var tileset = (TileGroup)_tilesets[key];
				if (tileset != null)
				{
					vars.AddKeyvalPair("rootPath",  tileset.MapPath);
					vars.AddKeyvalPair("rmpPath",   tileset.RoutePath);
					vars.AddKeyvalPair("blankPath", tileset.OccultPath);
				}
			}

			using (var sw = new StreamWriter(pfe))
			{
				foreach (string key in vars.Variables)
				{
					var val = (Variable)vars.Vars[key];
					sw.WriteLine(string.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"{0}:{1}",
											val.Name, val.Value));
				}

				foreach (string key in _tilesets.Keys)
					if (_tilesets[key] != null)
						((TileGroup)_tilesets[key]).Save(sw, vars);
			}
		} */
