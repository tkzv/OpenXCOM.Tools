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

		private const string Embedded = "XCom._Embedded.";
		private static readonly Hashtable _palettes = new Hashtable();
		#endregion


		#region Properties (static)
		/// <summary>
		/// The UFO Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette UfoBattle
		{
			get
			{
				if (_palettes["ufo-battle"] == null)
					_palettes["ufo-battle"] = new Palette(Assembly.GetExecutingAssembly()
											  .GetManifestResourceStream(Embedded + "ufo-battle.pal"));

				return _palettes["ufo-battle"] as Palette;
			}
		}

		public static Palette UfoGeo
		{
			get
			{
				if (_palettes["ufo-geo"] == null)
					_palettes["ufo-geo"] = new Palette(Assembly.GetExecutingAssembly()
										   .GetManifestResourceStream(Embedded + "ufo-geo.pal"));

				return _palettes["ufo-geo"] as Palette;
			}
		}

		public static Palette UfoGraph
		{
			get
			{
				if (_palettes["ufo-graph"] == null)
					_palettes["ufo-graph"] = new Palette(Assembly.GetExecutingAssembly()
											 .GetManifestResourceStream(Embedded + "ufo-graph.pal"));

				return _palettes["ufo-graph"] as Palette;
			}
		}

		public static Palette UfoResearch
		{
			get
			{
				if (_palettes["ufo-research"] == null)
					_palettes["ufo-research"] = new Palette(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream(Embedded + "ufo-research.pal"));

				return _palettes["ufo-research"] as Palette;
			}
		}

		/// <summary>
		/// The TFTD Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette TftdBattle
		{
			get
			{
				if (_palettes["tftd-battle"] == null)
					_palettes["tftd-battle"] = new Palette(Assembly.GetExecutingAssembly()
											   .GetManifestResourceStream(Embedded + "tftd-battle.pal"));

				return _palettes["tftd-battle"] as Palette;
			}
		}

		public static Palette TftdGeo
		{
			get
			{
				if (_palettes["tftd-geo"] == null)
					_palettes["tftd-geo"] = new Palette(Assembly.GetExecutingAssembly()
											.GetManifestResourceStream(Embedded + "tftd-geo.pal"));

				return _palettes["tftd-geo"] as Palette;
			}
		}

		public static Palette TftdGraph
		{
			get
			{
				if (_palettes["tftd-graph"] == null)
					_palettes["tftd-graph"] = new Palette(Assembly.GetExecutingAssembly()
											  .GetManifestResourceStream(Embedded + "tftd-graph.pal"));

				return _palettes["tftd-graph"] as Palette;
			}
		}

		public static Palette TftdResearch
		{
			get
			{
				if( _palettes["tftd-research"] == null)
					_palettes["tftd-research"] = new Palette(Assembly.GetExecutingAssembly()
												 .GetManifestResourceStream(Embedded + "tftd-research.pal"));

				return _palettes["tftd-research"] as Palette;
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
		public void SetAlpha(bool transparent)
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
