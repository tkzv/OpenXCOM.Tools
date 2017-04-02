using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;


namespace XCom
{
	/// <summary>
	/// A class defining a color array of 256 values
	/// </summary>
	//see http://support.microsoft.com/default.aspx?scid=kb%3Ben-us%3B319061
	public class Palette
	{
		public const byte TransparentId = 0xFE;

		private readonly string _label;
		/// <summary>
		/// This palette's label.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		private ColorPalette _colors;
		public ColorPalette Colors
		{
			get { return _colors; }
		}
		/// <summary>
		/// Gets/Sets indices for colors.
		/// </summary>
		public Color this[int i]
		{
			get { return _colors.Entries[i]; }
			set { _colors.Entries[i] = value; }
		}

		private static Hashtable _palHash = new Hashtable();

		private const string EmbedPath = "XCom._Embedded.";

		/// <summary>
		/// The UFO Palette embedded in this assembly.
		/// </summary>
		public static Palette UfoBattle
		{
			get
			{
				if (_palHash["ufo-battle"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["ufo-battle"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "ufo-battle.pal"));
				}
				return (Palette)_palHash["ufo-battle"];
			}
		}

		public static Palette UfoGeo
		{
			get
			{
				if (_palHash["ufo-geo"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["ufo-geo"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "ufo-geo.pal"));
				}
				return (Palette)_palHash["ufo-geo"];
			}
		}

		public static Palette UfoGraph
		{
			get
			{
				if (_palHash["ufo-graph"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["ufo-graph"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "ufo-graph.pal"));
				}
				return (Palette)_palHash["ufo-graph"];
			}
		}

		public static Palette UfoResearch
		{
			get
			{
				if (_palHash["ufo-research"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["ufo-research"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "ufo-research.pal"));
				}
				return (Palette)_palHash["ufo-research"];
			}
		}

		/// <summary>
		/// The TFTD Palette embedded in this assembly.
		/// </summary>
		public static Palette TftdBattle
		{
			get
			{
				if (_palHash["tftd-battle"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["tftd-battle"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "tftd-battle.pal"));
				}
				return (Palette)_palHash["tftd-battle"];
			}
		}

		public static Palette TftdGeo
		{
			get
			{
				if (_palHash["tftd-geo"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["tftd-geo"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "tftd-geo.pal"));
				}
				return (Palette)_palHash["tftd-geo"];
			}
		}

		public static Palette TftdGraph
		{
			get
			{
				if (_palHash["tftd-graph"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["tftd-graph"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "tftd-graph.pal"));
				}
				return (Palette)_palHash["tftd-graph"];
			}
		}

		public static Palette TftdResearch
		{
			get
			{
				if( _palHash["tftd-research"] == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					_palHash["tftd-research"] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + "tftd-research.pal"));
				}
				return (Palette)_palHash["tftd-research"];
			}
		}

		public Palette(string name)
		{
			var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			_colors = b.Palette;
			b.Dispose();
			this._label = name;
		}

		public Palette(Stream str)
		{
			var input = new StreamReader(str);
			var line = new string[0];
			_label = input.ReadLine();

			var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			_colors = b.Palette;

			for (byte i = 0; i < 0xFF; i++)
			{
				string allLine = input.ReadLine().Trim();
				if (allLine[0] == '#')
				{
					i--;
					continue;
				}
				line = allLine.Split(',');
				_colors.Entries[i] = Color.FromArgb(
												int.Parse(line[0], System.Globalization.CultureInfo.InvariantCulture),
												int.Parse(line[1], System.Globalization.CultureInfo.InvariantCulture),
												int.Parse(line[2], System.Globalization.CultureInfo.InvariantCulture));
			}
			b.Dispose();

//			checkPalette();
		}

/*		private void checkPalette()
		{
			Bitmap b = new Bitmap(1,1,PixelFormat.Format8bppIndexed);
			ColorPalette colors = b.Palette;
			b.Dispose();

			ArrayList cpList = new ArrayList(_colorPalette.Entries);
			ArrayList colorList = new ArrayList();

			for(int i=0;i<cpList.Count;i++)
			{
				if(!colorList.Contains(cpList[i]))
				{
					colorList.Add(cpList[i]);
					colors.Entries[i]=(Color)cpList[i];
				}
				else
				{
					Color c = (Color)cpList[i];
					int rc=c.R;
					int gc=c.G;
					int bc=c.B;

					if(rc==0)
						rc++;
					else
						rc--;

					if(gc==0)
						gc++;
					else
						gc--;

					if(bc==0)
						bc++;
					else
						bc--;

					colorList.Add(Color.FromArgb(rc,gc,bc));
					colors.Entries[i]=Color.FromArgb(rc,gc,bc);
				}
			}
		} */

		public Color Transparent
		{
			get { return _colors.Entries[TransparentId]; }
		}

		public Palette Grayscale
		{
			get
			{
				if (_palHash[_label + "#gray"] == null)
				{
					var palette = new Palette(_label + "#gray");
					_palHash[palette._label] = palette;
					for (int i = 0; i != _colors.Entries.Length; ++i)
					{
						int st = (int)(this[i].R * 0.1 + this[i].G * 0.5 + this[i].B * 0.25);
						palette[i] = Color.FromArgb(st, st, st);
					}
				}
				return (Palette)_palHash[_label + "#gray"];
			}
		}

		/// <summary>
		/// Sets the palette-id that will be used for transparency.
		/// </summary>
		/// <param name="zero">true to use #0 palette id; else use #255</param>
		public void SetTransparent(bool zero)
		{
			_colors.Entries[TransparentId] = (zero) ? Color.FromArgb(  0, _colors.Entries[TransparentId])
													: Color.FromArgb(255, _colors.Entries[TransparentId]);
		}

		/// <summary>
		/// tests for palette equality
		/// </summary>
		/// <param name="obj">another palette</param>
		/// <returns>true if the palette names are the same</returns>
		public override bool Equals(Object obj)
		{
			var palette = obj as Palette;
			return (palette != null && _colors.Equals(palette._colors));
		}

		public override int GetHashCode()
		{
			return _colors.GetHashCode(); // FIX: "Non-readonly field referenced in GetHashCode()."
		}

		public static Palette GetPalette(string label)
		{
			if (_palHash[label] == null)
			{
				var ass = Assembly.GetExecutingAssembly();
				try
				{
					_palHash[label] = new Palette(ass.GetManifestResourceStream(EmbedPath + label + ".pal"));
				}
				catch
				{
					_palHash[label] = null;
					throw;
				}
			}
			return (Palette)_palHash[label];
		}

		public override string ToString()
		{
			return _label;
		}
	}
}
