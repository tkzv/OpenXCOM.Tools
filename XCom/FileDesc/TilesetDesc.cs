using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public class TilesetDesc
		:
		FileDesc
	{
		private readonly Dictionary<string, ITileset> _tilesets;

//		private double version;


		public TilesetDesc()
			:
			base(String.Empty)
		{
			_tilesets = new Dictionary<string, ITileset>();
		}

		public TilesetDesc(string inFile, VarCollection vars)
			:
			base(inFile)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[4]TilesetDesc cTor file= " + inFile);
			_tilesets = new Dictionary<string, ITileset>();
			//LogFile.WriteLine(". [4]instantiate _tilesets Dictionary");

			using (var sr = new StreamReader(File.OpenRead(inFile)))
			{
				var vars1 = new VarCollection(sr, vars);

				int pos;
				string key, val, line;

				//LogFile.WriteLine(". [4]start stream iteration");
				while ((line = vars1.ReadLine(sr)) != null) // will not return lines that start '$' (or whitespace lines)
				{
					//LogFile.WriteLine("");
					//LogFile.WriteLine(". . [4]TilesetDesc cTor line= " + line);
					pos = line.IndexOf(':');
					//LogFile.WriteLine(". . [4]pos= " + pos);
					key = line.Substring(0, pos);
					//LogFile.WriteLine(". . [4]key= " + key);
					val = line.Substring(pos + 1);
					//LogFile.WriteLine(". . [4]val= " + val);

					switch (key.ToUpperInvariant())
					{
						case "TILESET":
							//LogFile.WriteLine(". . . [4]case TILESET");
							line = VarCollection.ReadLine(sr, vars1);
							//LogFile.WriteLine(". . . [4]line= " + line);
							pos = line.IndexOf(':');
							//LogFile.WriteLine(". . . [4]pos= " + pos);
							key = line.Substring(0, pos).ToUpperInvariant();
							//LogFile.WriteLine(". . . [4]key= " + key);

							switch (key)
							{
								case "TYPE":
									//LogFile.WriteLine(". . . . [4]subcase TYPE val= " + int.Parse(line.Substring(pos + 1), System.Globalization.CultureInfo.InvariantCulture));
									switch (int.Parse(line.Substring(pos + 1), System.Globalization.CultureInfo.InvariantCulture))
									{
//										case 0:
//											_tilesets[name] = new Type0Tileset(name, sr, new VarCollection(vars1));
//											break;
										case 1:
											//LogFile.WriteLine(". . . . . [4]instantiate XCTileset _tilesets[" + val + "]");
											_tilesets[val] = new XCTileset(val, sr, new VarCollection(vars1));
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
								string mapPath,
								string rmpPath,
								string blankPath)
		{
			var tileset = new XCTileset(name);

			tileset.MapPath   = mapPath;
			tileset.RmpPath   = rmpPath;
			tileset.BlankPath = blankPath;

			return (_tilesets[name] = tileset);
		}

		public Dictionary<string, ITileset> Tilesets
		{
			get { return _tilesets; }
		}

		/// <summary>
		/// WARNING: This isn't used but has to be here to satisfy inheritance
		/// from FileDesc.Save() which is abstract.
		/// </summary>
		/// <param name="outFile"></param>
		public override void Save(string outFile)
		{
			var vars = new VarCollection("Path"); // iterate thru each tileset, call save on them

			foreach (string st in _tilesets.Keys)
			{
				var tileset = (IXCTileset)_tilesets[st];
				if (tileset != null)
				{
					vars.AddKeyVal("rootPath",  tileset.MapPath);
					vars.AddKeyVal("rmpPath",   tileset.RmpPath);
					vars.AddKeyVal("blankPath", tileset.BlankPath);
				}
			}

			var sw = new StreamWriter(outFile);

			foreach (string key in vars.Variables)
			{
				var val = (Variable)vars.Vars[key];
				sw.WriteLine(string.Format("{0}:{1}", val.Name, val.Value));
			}

			foreach (string st in _tilesets.Keys)
				if (_tilesets[st] != null)
					((IXCTileset)_tilesets[st]).Save(sw, vars);

			sw.Close();
		}
	}
}
