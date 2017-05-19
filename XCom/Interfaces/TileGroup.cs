using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class TileGroup // TODO: cTor has inheritors and calls a virtual function.
		:
			TileGroupBase
	{
		#region Fields & Properties
		public Palette Palette
		{ get; set; }

		public string MapDirectory
		{ get; set; }

		public string RouteDirectory
		{ get; set; }

		public string OccultDirectory
		{ get; set; }
		#endregion


		#region cTors
		/// <summary>
		/// cTor. Load from YAML.
		/// </summary>
		internal TileGroup(string label)
			:
				base(label)
		{
			MapDirectory    = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "MAPS"); // TODO: TFTD ....
			RouteDirectory  = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "ROUTES");
			OccultDirectory = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.SettingsDirectory), @"OccultTileData\UFO");

			Palette = ResourceInfo.Palette;
//			Palette = Palette.UfoBattle;
			// TODO: TFTD Palette = Palette.TftdBattle
			// custom Palette = Palette.GetPalette(val)
		}



/*		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		internal protected TileGroup(string label)
			:
				base(label)
		{
			Palette = ResourceInfo.Palette;

//			_mapSize = new MapSize(60, 60, 4);
//			_mapDepth = 0;
//			_underwater = true;
//			_baseStyle = false;
		} */
/*		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="sr"></param>
		/// <param name="vars"></param>
		internal protected TileGroup(
				string label,
				StreamReader sr,
				Varidia vars)
			:
				this(label)
		{
			while (sr.Peek() != -1)
			{
				string line = Varidia.ReadLine(sr, vars);
				if (line.ToUpperInvariant() == "END")
					return;

				int pos    = line.IndexOf(':');
				string key = line.Substring(0, pos);
				string val = line.Substring(pos + 1);

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

//					case "DLL": // not used. Might want to throw it at default:ParseLine()
//						string dll = val.Substring(val.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
//						Console.WriteLine(name + " is in dll " + dll);
//						break;

					case "ROOTPATH":
						MapDirectory = val;
						break;

					case "RMPPATH":
						RouteDirectory = val;
						break;

					case "BLANKPATH":
						OccultDirectory = val;
						break;

//					case "BASESTYLE": // not used. Might want to throw it at default:ParseLine()
//						_baseStyle = true;
//						break;

//					case "GROUND": // not used. Might want to throw it at default:ParseLine()
//						_ground = val.Split(' ');
//						break;

//					case "SIZE": // not used. Might want to throw it at default:ParseLine()
//						string[] dim = val.Split(',');
//						int rows   = int.Parse(dim[0], System.Globalization.CultureInfo.InvariantCulture);
//						int cols   = int.Parse(dim[1], System.Globalization.CultureInfo.InvariantCulture);
//						int height = int.Parse(dim[2], System.Globalization.CultureInfo.InvariantCulture);
//
//						_mapSize = new MapSize(rows, cols, height);
//						break;

//					case "LANDMAP": // not used. Might want to throw it at default:ParseLine()
//						_underwater = false;
//						break;

//					case "DEPTH": // not used. Might want to throw it at default:ParseLine()
//						_mapDepth = int.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
//						break;

//					case "SCANG": // not used. Might want to throw it at default:ParseLine()
//						scanFile = val;
//						break;

//					case "LOFTEMP": // not used. Might want to throw it at default:ParseLine()
//						loftFile = val;
//						break;

					default: // user-defined keyword
						ParseLine(key, val, sr, vars); // FIX: "Virtual member call in a constructor."
						break;
				}
			}
		} */
		#endregion


		#region Methods (virtual)
		public virtual void Save(StreamWriter sw, Varidia vars)
		{}

		public virtual void ParseLine(
				string key,
				string category,
				StreamReader sr,
				Varidia vars)
		{}

		public virtual void AddMap(string tileset, string category)
		{}

		public virtual void AddMap(Descriptor descriptor, string category)
		{}

//		public virtual Descriptor RemoveTileset(string tileset, string category)
//		{
//			return null;
//		}
		#endregion
	}
}

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
