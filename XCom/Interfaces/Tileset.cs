using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class Tileset	// psst. This isn't an interface.
		:					// TODO: cTor has inheritors and calls a virtual function.
			TilesetBase
	{
		public Palette Palette
		{ get; set; }

		public string MapPath
		{ get; set; }

		public string RoutePath
		{ get; set; }

		public string BlankPath
		{ get; set; }

//		private string[] _ground;
//		public string[] Ground // TODO: return a collection or make it a method.
//		{
//			get { return _ground; }
//		}

//		private bool _underwater;
//		public bool Underwater
//		{
//			get { return _underwater; }
//		}

//		private bool _baseStyle;
//		public bool BaseStyle
//		{
//			get { return _baseStyle; }
//		}

//		private int _mapDepth;
//		public int Depth
//		{
//			get { return _mapDepth; }
//		}

/*		private MapSize _mapSize;
		public MapSize Size
		{
			get { return _mapSize; }
		} */

//		private string scanFile;
//		private string loftFile;



		protected Tileset(string name)
			:
				base(name)
		{
			Palette = GameInfo.DefaultPalette;
//			_mapSize = new MapSize(60, 60, 4);

//			_mapDepth = 0;
//			_underwater = true;
//			_baseStyle = false;
		}

		protected Tileset(string name, StreamReader sr, Varidia vars)
			:
				this(name)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[7]Tileset cTor");
			while (sr.Peek() != -1)
			{
				string line = Varidia.ReadLine(sr, vars);
				//LogFile.WriteLine(". [7]line= " + line);

				if (line.ToUpperInvariant() == "END")
				{
					//LogFile.WriteLine(". . [7]Exit.");
					return;
				}

				int pos    = line.IndexOf(':');
				string key = line.Substring(0, pos);
				string val = line.Substring(pos + 1);

				//LogFile.WriteLine(". [7]pos= " + pos);
				//LogFile.WriteLine(". [7]key= " + key);
				//LogFile.WriteLine(". [7]val= " + val);

				switch (key.ToUpperInvariant())
				{
					case "PALETTE":
						switch (val.ToUpperInvariant())
						{
							case "UFO":  Palette = Palette.UfoBattle;       break;
							case "TFTD": Palette = Palette.TftdBattle;      break;
							default:     Palette = Palette.GetPalette(val); break;
						}
						break;

					case "DLL": // not used. Might want to throw it at default:ParseLine()
//						string dll = val.Substring(val.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
//						Console.WriteLine(name + " is in dll " + dll);
						break;

					case "ROOTPATH":
						MapPath = val;
						break;

					case "RMPPATH":
						RoutePath = val;
						break;

					case "BLANKPATH":
						BlankPath = val;
						break;

					case "BASESTYLE": // not used. Might want to throw it at default:ParseLine()
//						_baseStyle = true;
						break;

					case "GROUND": // not used. Might want to throw it at default:ParseLine()
//						_ground = val.Split(' ');
						break;

					case "SIZE": // not used. Might want to throw it at default:ParseLine()
//						string[] dim = val.Split(',');
//						int rows   = int.Parse(dim[0], System.Globalization.CultureInfo.InvariantCulture);
//						int cols   = int.Parse(dim[1], System.Globalization.CultureInfo.InvariantCulture);
//						int height = int.Parse(dim[2], System.Globalization.CultureInfo.InvariantCulture);
//
//						_mapSize = new MapSize(rows, cols, height);
						break;

					case "LANDMAP": // not used. Might want to throw it at default:ParseLine()
//						_underwater = false;
						break;

					case "DEPTH": // not used. Might want to throw it at default:ParseLine()
//						_mapDepth = int.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
						break;

					case "SCANG": // not used. Might want to throw it at default:ParseLine()
//						scanFile = val;
						break;

					case "LOFTEMP": // not used. Might want to throw it at default:ParseLine()
//						loftFile = val;
						break;

					default:
						// user-defined keyword
						ParseLine(key, val, sr, vars); // FIX: "Virtual member call in a constructor."
						break;
				}
			}
		}


		public virtual void Save(StreamWriter sw, Varidia vars)
		{}

		public virtual void ParseLine(
				string key,
				string line,
				StreamReader sr,
				Varidia vars)
		{}

		public virtual void AddMap(string name, string subset)
		{}

		public virtual void AddMap(XCMapDesc desc, string subset)
		{}

		public virtual XCMapDesc RemoveMap(string name, string subset)
		{
			return null;
		}
	}
}
