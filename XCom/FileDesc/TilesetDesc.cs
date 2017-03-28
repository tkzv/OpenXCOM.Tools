using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class TilesetDesc
		:
			FileDesc
	{
		private readonly Dictionary<string, ITileset> _tilesets;
		public Dictionary<string, ITileset> Tilesets
		{
			get { return _tilesets; }
		}

//		private double version;


/*		public TilesetDesc()
			:
				base(String.Empty)
		{
			_tilesets = new Dictionary<string, ITileset>();
		} */
		internal TilesetDesc(string inFile, Varidia vars)
			:
				base(inFile)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[4]TilesetDesc cTor file= " + inFile);
			_tilesets = new Dictionary<string, ITileset>();
			//LogFile.WriteLine(". [4]instantiate _tilesets Dictionary");

			using (var sr = new StreamReader(File.OpenRead(inFile)))
			{
				var vars1 = new Varidia(sr, vars);

				int pos;
				string key, val, line;

				//LogFile.WriteLine(". [4]start stream iteration");
				while ((line = vars1.ReadLine(sr)) != null) // will not return lines that start '$' (or whitespace lines)
				{
					pos = line.IndexOf(':');
					key = line.Substring(0, pos);
					val = line.Substring(pos + 1);

					//LogFile.WriteLine("");
					//LogFile.WriteLine(". . [4]TilesetDesc cTor line= " + line);
					//LogFile.WriteLine(". . [4]pos= " + pos);
					//LogFile.WriteLine(". . [4]key= " + key);
					//LogFile.WriteLine(". . [4]val= " + val);

					switch (key.ToUpperInvariant())
					{
						case "TILESET":
							line = Varidia.ReadLine(sr, vars1);
							pos  = line.IndexOf(':');
							key  = line.Substring(0, pos).ToUpperInvariant();

							//LogFile.WriteLine(". . . [4]case TILESET");
							//LogFile.WriteLine(". . . [4]line= " + line);
							//LogFile.WriteLine(". . . [4]pos= " + pos);
							//LogFile.WriteLine(". . . [4]key= " + key);

							switch (key)
							{
								case "TYPE":
									//LogFile.WriteLine(". . . . [4]subcase TYPE val= " + int.Parse(line.Substring(pos + 1), System.Globalization.CultureInfo.InvariantCulture));
									switch (int.Parse(line.Substring(pos + 1), System.Globalization.CultureInfo.InvariantCulture))
									{
//										case 0:
//											_tilesets[name] = new Type0Tileset(name, sr, new Varidia(vars1));
//											break;
										case 1:
											//LogFile.WriteLine(". . . . . [4]instantiate XCTileset _tilesets[" + val + "]");
											_tilesets[val] = new XCTileset(val, sr, new Varidia(vars1));
											break;
									}
									break;

								default:
									//LogFile.WriteLine(". . . . [4]subcase default Type Not Found");
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
							//LogFile.WriteLine(". . . [4]case default UNKNOWN line= " + line);
							Console.WriteLine(string.Format(
														System.Globalization.CultureInfo.CurrentCulture,
														"Unknown line: {0}",
														line));
							break;
					}
				}
			}
		}

		public ITileset AddTileset(
								string name,
								string pathMaps,
								string pathRoutes,
								string pathBlanks)
		{
			var tileset = new XCTileset(name);

			tileset.MapPath   = pathMaps;
			tileset.RoutePath = pathRoutes;
			tileset.BlankPath = pathBlanks;

			return (_tilesets[name] = tileset);
		}

		/// <summary>
		/// WARNING: This doesn't appear to be used but has to be here to
		/// satisfy inheritance from FileDesc.Save() which is abstract.
		/// </summary>
		/// <param name="outFile"></param>
		public override void Save(string outFile)
		{
			var vars = new Varidia("Path"); // iterate thru each tileset, call save on them

			foreach (string key in _tilesets.Keys)
			{
				var tileset = (IXCTileset)_tilesets[key];
				if (tileset != null)
				{
					vars.AddKeyvalPair("rootPath",  tileset.MapPath);
					vars.AddKeyvalPair("rmpPath",   tileset.RoutePath);
					vars.AddKeyvalPair("blankPath", tileset.BlankPath);
				}
			}

			using (var sw = new StreamWriter(outFile))
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
						((IXCTileset)_tilesets[key]).Save(sw, vars);
			}
		}
	}
}
