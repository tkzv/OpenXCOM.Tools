using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class IXCTileset // TODO: cTor has inheritors and calls a virtual function.
		:
		ITileset
	{
		protected Palette myPal;
		protected string rootPath, rmpPath, blankPath;
		protected string[] groundMaps;
		protected bool underwater, baseStyle;
		protected int mapDepth;
		protected string scanFile, loftFile;
		protected MapSize mapSize;


		protected IXCTileset(string name)
			:
			base(name)
		{
			myPal = GameInfo.DefaultPalette;
			mapSize = new MapSize(60, 60, 4);
			mapDepth = 0;
			underwater = true;
			baseStyle = false;
		}

		protected IXCTileset(string name, StreamReader sr, VarCollection vars)
			:
			this(name)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("[7]IXCTileset cTor");
			while (sr.Peek() != -1)
			{
				string line = VarCollection.ReadLine(sr, vars);
				LogFile.WriteLine(". [7]line= " + line);

				if (line.ToUpperInvariant() == "END")
				{
					LogFile.WriteLine(". . [7]Exit.");
					return;
				}

				int pos         = line.IndexOf(':');
				LogFile.WriteLine(". [7]pos= " + pos);
				string key      = line.Substring(0, pos);
				LogFile.WriteLine(". [7]key= " + key);
				string keyUpper = key.ToUpperInvariant();
				LogFile.WriteLine(". [7]keyUpper= " + keyUpper);
				string val      = line.Substring(pos + 1);
				LogFile.WriteLine(". [7]val= " + val);

				switch (keyUpper)
				{
					case "PALETTE":
						switch (val.ToUpperInvariant())
						{
							case "UFO":  myPal = Palette.UFOBattle;       break;
							case "TFTD": myPal = Palette.TFTDBattle;      break;
							default:     myPal = Palette.GetPalette(val); break;
						}
						break;

					case "DLL":
						string dll = val.Substring(val.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						Console.WriteLine(name + " is in dll " + dll);
						break;

					case "ROOTPATH":
						rootPath = val;
						break;

					case "RMPPATH":
						rmpPath = val;
						break;

					case "BASESTYLE":
						baseStyle = true;
						break;

					case "GROUND":
						groundMaps = val.Split(' ');
						break;

					case "SIZE":
						string[] dim = val.Split(',');
						int rows   = int.Parse(dim[0], System.Globalization.CultureInfo.InvariantCulture);
						int cols   = int.Parse(dim[1], System.Globalization.CultureInfo.InvariantCulture);
						int height = int.Parse(dim[2], System.Globalization.CultureInfo.InvariantCulture);

						mapSize = new MapSize(rows, cols, height);
						break;

					case "LANDMAP":
						underwater = false;
						break;

					case "DEPTH":
						mapDepth = int.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
						break;

					case "BLANKPATH":
						blankPath = val;
						break;

					case "SCANG":
						scanFile = val;
						break;

					case "LOFTEMP":
						loftFile = val;
						break;

					default:
						// user-defined keyword
						ParseLine(key, val, sr, vars); // FIX: "Virtual member call in a constructor."
						break;
				}
			}
		}


		public MapSize Size
		{
			get { return mapSize; }
		}

		public virtual void Save(StreamWriter sw, VarCollection vars)
		{}

		public bool Underwater
		{
			get { return underwater; }
		}

		public string MapPath
		{
			get { return rootPath; }
			set { rootPath = value; }
		}

		public string RmpPath
		{
			get { return rmpPath; }
			set { rmpPath = value; }
		}

		public string BlankPath
		{
			get { return blankPath; }
			set { blankPath = value; }
		}

		public Palette Palette
		{
			get { return myPal; }
			set { myPal = value; }
		}

		public int Depth
		{
			get { return mapDepth; }
		}

		public string[] Ground
		{
			get { return groundMaps; }
		}

		public bool BaseStyle
		{
			get { return baseStyle; }
		}

		public virtual void ParseLine(
				string keyword,
				string line,
				StreamReader sr,
				VarCollection vars)
		{}

		public virtual void AddMap(string name, string subset)
		{}

		public virtual void AddMap(XCMapDesc imd, string subset)
		{}

		public virtual XCMapDesc RemoveMap(string name, string subset)
		{
			return null;
		}
	}
}
