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
			_tilesets = new Dictionary<string, ITileset>();

			var sr = new StreamReader(File.OpenRead(inFile));
			var vars1 = new VarCollection(sr, vars);

			string line, name, keyword;
			int idx;

			while ((line = vars1.ReadLine(sr)) != null)
			{
				idx = line.IndexOf(':');
				name = line.Substring(idx + 1);
				keyword = line.Substring(0, idx);

				switch (keyword.ToLower())
				{
					case "tileset":
						line = VarCollection.ReadLine(sr, vars1);
						idx = line.IndexOf(':');
						keyword = line.Substring(0, idx).ToLower();

						switch (keyword)
						{
							case "type":
								switch (int.Parse(line.Substring(idx + 1)))
								{
//									case 0:
//										_tilesets[name] = new Type0Tileset(name, sr, new VarCollection(vars1));
//										break;
									case 1:
										_tilesets[name] = new XCTileset(name, sr, new VarCollection(vars1));
										break;
								}
								break;

							default:
								Console.WriteLine(string.Format("Type line not found: {0}", line));
								break;
						}
						break;

//					case "version":
//						version = double.Parse(name);
//						break;

					default:
						Console.WriteLine(string.Format("Unknown line: {0}", line));
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

		public override void Save(string outFile)
		{
			var vars = new VarCollection("Path"); // iterate thru each tileset, call save on them

			foreach (string st in _tilesets.Keys)
			{
				var tileset = (IXCTileset)_tilesets[st];
				if (tileset != null)
				{
					vars.AddVar("rootPath",  tileset.MapPath);
					vars.AddVar("rmpPath",   tileset.RmpPath);
					vars.AddVar("blankPath", tileset.BlankPath);
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
