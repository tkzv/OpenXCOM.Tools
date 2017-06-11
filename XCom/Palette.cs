using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;


namespace XCom
{
	/// <summary>
	/// A class defining a color array of 256 values.
	/// </summary>
	//see http://support.microsoft.com/default.aspx?scid=kb%3Ben-us%3B319061
	public class Palette
	{
		#region Fields (static)
		internal const byte TransparentId = 0xFE;

		private static readonly Hashtable _palettes = new Hashtable();

		private const string Embedded     = "XCom._Embedded.";

		private const string ufobattle    = "ufo-battle";
		private const string ufogeo       = "ufo-geo";
		private const string ufograph     = "ufo-graph";
		private const string uforesearch  = "ufo-research";

		private const string tftdbattle   = "tftd-battle";
		private const string tftdgeo      = "tftd-geo";
		private const string tftdgraph    = "tftd-graph";
		private const string tftdresearch = "tftd-research";

		private const string PalExt       = ".pal";
		#endregion


		#region Properties (static)
		/// <summary>
		/// The UFO Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette UfoBattle
		{
			get
			{
				if (_palettes[ufobattle] == null)
					_palettes[ufobattle] = new Palette(Assembly.GetExecutingAssembly()
											  .GetManifestResourceStream(Embedded + ufobattle + PalExt));

				return _palettes[ufobattle] as Palette;
			}
		}

		public static Palette UfoGeo
		{
			get
			{
				if (_palettes[ufogeo] == null)
					_palettes[ufogeo] = new Palette(Assembly.GetExecutingAssembly()
										   .GetManifestResourceStream(Embedded + ufogeo + PalExt));

				return _palettes[ufogeo] as Palette;
			}
		}

		public static Palette UfoGraph
		{
			get
			{
				if (_palettes[ufograph] == null)
					_palettes[ufograph] = new Palette(Assembly.GetExecutingAssembly()
											 .GetManifestResourceStream(Embedded + ufograph + PalExt));

				return _palettes[ufograph] as Palette;
			}
		}

		public static Palette UfoResearch
		{
			get
			{
				if (_palettes[uforesearch] == null)
					_palettes[uforesearch] = new Palette(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream(Embedded + uforesearch + PalExt));

				return _palettes[uforesearch] as Palette;
			}
		}

		/// <summary>
		/// The TFTD Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette TftdBattle
		{
			get
			{
				if (_palettes[tftdbattle] == null)
					_palettes[tftdbattle] = new Palette(Assembly.GetExecutingAssembly()
											   .GetManifestResourceStream(Embedded + tftdbattle + PalExt));

				return _palettes[tftdbattle] as Palette;
			}
		}

		public static Palette TftdGeo
		{
			get
			{
				if (_palettes[tftdgeo] == null)
					_palettes[tftdgeo] = new Palette(Assembly.GetExecutingAssembly()
											.GetManifestResourceStream(Embedded + tftdgeo + PalExt));

				return _palettes[tftdgeo] as Palette;
			}
		}

		public static Palette TftdGraph
		{
			get
			{
				if (_palettes[tftdgraph] == null)
					_palettes[tftdgraph] = new Palette(Assembly.GetExecutingAssembly()
											  .GetManifestResourceStream(Embedded + tftdgraph + PalExt));

				return _palettes[tftdgraph] as Palette;
			}
		}

		public static Palette TftdResearch
		{
			get
			{
				if( _palettes[tftdresearch] == null)
					_palettes[tftdresearch] = new Palette(Assembly.GetExecutingAssembly()
												 .GetManifestResourceStream(Embedded + tftdresearch + PalExt));

				return _palettes[tftdresearch] as Palette;
			}
		}
		#endregion


		#region Properties
		private readonly string _label;
		/// <summary>
		/// This palette's label.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		public ColorPalette Colors
		{ get; private set; }

		/// <summary>
		/// Gets/Sets indices for colors.
		/// </summary>
		public Color this[int id]
		{
			get { return Colors.Entries[id]; }
			set { Colors.Entries[id] = value; }
		}

		public Color Transparent
		{
			get { return Colors.Entries[TransparentId]; }
		}

		public Palette Grayscale
		{
			get
			{
				if (_palettes[_label + "#gray"] == null)
				{
					var pal = new Palette(_label + "#gray");

					_palettes[pal._label] = pal;

					for (int id = 0; id != Colors.Entries.Length; ++id)
					{
						int val = (int)(this[id].R * 0.1 + this[id].G * 0.5 + this[id].B * 0.25);
						pal[id] = Color.FromArgb(val, val, val);
					}
				}
				return _palettes[_label + "#gray"] as Palette;
			}
		}
		#endregion


		#region cTors
		private Palette(string label)
		{
			using (var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				Colors = b.Palette;

			_label = label;
		}
		private Palette(Stream fs)
		{
			using (var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				Colors = b.Palette;

			using (var input = new StreamReader(fs))
			{
				_label = input.ReadLine(); // 1st line is the label.

				var line = new string[3];

				for (byte id = 0; id != 0xFF; )
				{
					string rgb = input.ReadLine().Trim();
					if (rgb[0] != '#')
					{
						line = rgb.Split(',');
						Colors.Entries[id++] = Color.FromArgb(
														Int32.Parse(line[0], System.Globalization.CultureInfo.InvariantCulture),
														Int32.Parse(line[1], System.Globalization.CultureInfo.InvariantCulture),
														Int32.Parse(line[2], System.Globalization.CultureInfo.InvariantCulture));
					}
				}
			}
//			checkPalette();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Enables or disables transparency on the 'TransparentId'
		/// palette-index.
		/// </summary>
		/// <param name="transparent">true to enable transparency</param>
		public void SetTransparent(bool transparent)
		{
			Colors.Entries[TransparentId] = Color.FromArgb(
														transparent ? 0 : 255,
														Colors.Entries[TransparentId]);
		}

		public override string ToString()
		{
			return _label;
		}

		/// <summary>
		/// Checks for palette equality.
		/// </summary>
		/// <param name="obj">another palette</param>
		/// <returns>true if the palette names are the same</returns>
		public override bool Equals(Object obj)
		{
			var palette = obj as Palette;
			return (palette != null && Colors.Equals(palette.Colors));
		}

		public override int GetHashCode()
		{
			return Colors.GetHashCode(); // FIX: "Non-readonly field referenced in GetHashCode()."
		}

//		internal static Palette GetPalette(string label)
//		{
//			if (_palHash[label] == null)
//			{
//				var ass = Assembly.GetExecutingAssembly();
//				try
//				{
//					_palHash[label] = new Palette(ass.GetManifestResourceStream(EmbedPath + label + ".pal"));
//				}
//				catch
//				{
//					_palHash[label] = null;
//					throw;
//				}
//			}
//			return (Palette)_palHash[label];
//		}

//		private void checkPalette()
//		{
//			Bitmap b = new Bitmap(1,1,PixelFormat.Format8bppIndexed);
//			ColorPalette colors = b.Palette;
//			b.Dispose();
//
//			ArrayList cpList = new ArrayList(_colorPalette.Entries);
//			ArrayList colorList = new ArrayList();
//
//			for(int i=0;i<cpList.Count;i++)
//			{
//				if(!colorList.Contains(cpList[i]))
//				{
//					colorList.Add(cpList[i]);
//					colors.Entries[i]=(Color)cpList[i];
//				}
//				else
//				{
//					Color c = (Color)cpList[i];
//					int rc=c.R;
//					int gc=c.G;
//					int bc=c.B;
//
//					if(rc==0)
//						rc++;
//					else
//						rc--;
//
//					if(gc==0)
//						gc++;
//					else
//						gc--;
//
//					if(bc==0)
//						bc++;
//					else
//						bc--;
//
//					colorList.Add(Color.FromArgb(rc,gc,bc));
//					colors.Entries[i]=Color.FromArgb(rc,gc,bc);
//				}
//			}
//		}
		#endregion
	}
}
