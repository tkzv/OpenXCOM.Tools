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
		private readonly string name;

		private ColorPalette _colorPalette;

		private static Hashtable _palHash = new Hashtable();

		private static readonly string EmbedPath = "XCom._Embedded.";

		/// <summary>
		/// The UFO Palette embedded in this assembly.
		/// </summary>
		public static Palette UFOBattle
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

		public static Palette UFOGeo
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

		public static Palette UFOGraph
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

		public static Palette UFOResearch
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
		public static Palette TFTDBattle
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

		public static Palette TFTDGeo
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

		public static Palette TFTDGraph
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

		public static Palette TFTDResearch
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
			_colorPalette = b.Palette;
			b.Dispose();
			this.name = name;
		}

		public Palette(Stream str)
		{
			var input = new StreamReader(str);
			string[] line = new string[0];
			name = input.ReadLine();

			var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			_colorPalette = b.Palette;

			for (byte i = 0; i < 0xFF; i++)
			{
				string allLine = input.ReadLine().Trim();
				if (allLine[0] == '#')
				{
					i--;
					continue;
				}
				line = allLine.Split(',');
				_colorPalette.Entries[i] = Color.FromArgb(
											int.Parse(line[0]),
											int.Parse(line[1]),
											int.Parse(line[2]));
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
			get { return _colorPalette.Entries[Bmp.DefaultTransparentIndex]; }
		}

		public Palette Grayscale
		{
			get
			{
				if (_palHash[name + "#gray"] == null)
				{
					var palette = new Palette(name + "#gray");
					_palHash[palette.name] = palette;
					for (int i = 0; i < _colorPalette.Entries.Length; i++)
					{
						int st = (int)(this[i].R * 0.1 + this[i].G * 0.5 + this[i].B * 0.25);
						palette[i] = Color.FromArgb(st, st, st);
					}
				}
				return (Palette)_palHash[name + "#gray"];
			}
		}

		public void SetTransparent(bool val)
		{
			int transparent = Bmp.DefaultTransparentIndex;
			var color = _colorPalette.Entries[transparent];
			_colorPalette.Entries[transparent] = (val) ? Color.FromArgb(  0, color)
													   : Color.FromArgb(255, color);
		}

		public ColorPalette Colors
		{
			get { return _colorPalette; }
		}

		/// <summary>
		/// This palette's name
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Indexes colors on number
		/// </summary>
		public Color this[int i]
		{
			get { return _colorPalette.Entries[i]; }
			set { _colorPalette.Entries[i] = value; }
		}

		/// <summary>
		/// tests for palette equality
		/// </summary>
		/// <param name="obj">another palette</param>
		/// <returns>true if the palette names are the same</returns>
		public override bool Equals(Object obj)
		{
			var palette = obj as Palette;
			return (palette != null && _colorPalette.Equals(palette._colorPalette));
		}

		public override int GetHashCode()
		{
			return _colorPalette.GetHashCode(); // FIX: "Non-readonly field referenced in GetHashCode()."
		}

		public static Palette GetPalette(string name)
		{
			if (_palHash[name] == null)
			{
				Assembly thisAssembly = Assembly.GetExecutingAssembly();
				try
				{
					_palHash[name] = new Palette(thisAssembly.GetManifestResourceStream(EmbedPath + name + ".pal"));
				}
				catch
				{
					_palHash[name] = null;
				}
			}
			return (Palette)_palHash[name];
		}

		public override string ToString()
		{
			return name;
		}
	}
}
