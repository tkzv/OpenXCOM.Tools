using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public class TilesetDesc:FileDesc
	{
		private readonly Dictionary<string, ITileset> tilesets;
//		private double version;


		public TilesetDesc()
			:
			base("")
		{
			tilesets = new Dictionary<string, ITileset>();
		}

		public TilesetDesc(string inFile, VarCollection v)
			:
			base(inFile)
		{
			tilesets = new Dictionary<string, ITileset>();
			var sr = new StreamReader(File.OpenRead(inFile));
			var vars = new VarCollection(sr, v);

			string line, name, keyword;
			int idx;

			while ((line = vars.ReadLine(sr)) != null)
			{
				idx = line.IndexOf(':');
				name = line.Substring(idx + 1);
				keyword = line.Substring(0, idx);

				switch (keyword.ToLower())
				{
					case "tileset":
						line = VarCollection.ReadLine(sr, vars);
						idx = line.IndexOf(':');
						keyword = line.Substring(0, idx).ToLower();

						switch (keyword)
						{
							case "type":
								switch (int.Parse(line.Substring(idx + 1)))
								{
//									case 0:
//										tilesets[name] = new Type0Tileset(name, sr, new VarCollection(vars));
//										break;
									case 1:
										tilesets[name] = new XCTileset(name, sr, new VarCollection(vars));
										break;
								}
								break;

							default:
								Console.WriteLine("Type line not found: " + line);
								break;
						}
						break;

//					case "version":
//						version = double.Parse(name);
//						break;

					default:
						Console.WriteLine("Unknown line: " + line);
						break;
				}
			}
			sr.Close();
		}

		public ITileset AddTileset(
								string name,
								string mapPath,
								string rmpPath,
								string blankPath)
		{
			IXCTileset tSet = new XCTileset(name);
			tSet.MapPath = mapPath;
			tSet.RmpPath = rmpPath;
			tSet.BlankPath = blankPath;
			tilesets[name] = tSet;
			return tSet;
		}

		public Dictionary<string, ITileset> Tilesets
		{
			get { return tilesets; }
		}

		public override void Save(string outFile)
		{
			var vc = new VarCollection("Path"); // iterate thru each tileset, call save on them
			var sw = new StreamWriter(outFile);

			foreach (string s in tilesets.Keys)
			{
				var ts = (IXCTileset)tilesets[s];
				if (ts != null)
				{
					vc.AddVar("rootPath", ts.MapPath);
					vc.AddVar("rmpPath", ts.RmpPath);
					vc.AddVar("blankPath", ts.BlankPath);
				}
			}

			foreach (string v in vc.Variables)
			{
				var var = (Variable)vc.Vars[v];
				sw.WriteLine(var.Name + ":" + var.Value);
			}

			foreach (string s in tilesets.Keys)
				if (tilesets[s] != null)
					((IXCTileset)tilesets[s]).Save(sw,vc);

			sw.Close();
		}
	}
}
