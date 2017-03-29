using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	/// <summary>
	/// Summary description for EditorPane.
	/// </summary>
	internal sealed class EditorPane
		:
			Panel
	{
		private XCImage _image;
		public XCImage Image
		{
//			get { return _image; }
			set
			{
				_image = value;
				Refresh();
			}
		}

		private Palette _palette;
		public Palette Palette
		{
//			get { return _palette; }
			set
			{
				_palette = value;
				if (_image!=null)
				{
					_image.Image.Palette = _palette.Colors;
					Refresh();
				}
			}
		}

		private bool _lines;
		public bool Lines
		{
//			get { return _lines; }
			set
			{
				_lines = value;
				Refresh();
			}
		}

		private const int Square = 1;

//		private int _width;
//		private int _height;

		private double _scale = 1.0;
		public double ScaleVal
		{
			get { return _scale; }
			set
			{
				_scale = value;
				Refresh();
			}
		}


		public EditorPane(XCImage image)
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

			_image = image;
//			pal = null;
//			lines = false;

//			_width  = PckImage.Width  * Square;
//			_height = PckImage.Height * Square;
		}


		public static int PreferredWidth
		{
			get { return PckImage.Width * 10; }
		}

		public static int PreferredHeight
		{
			get { return PckImage.Height * 10; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int width  = _image.Image.Width;
			int height = _image.Image.Height;

			for (int y = 0; y != height; ++y)
				for (int x = 0; x != width; ++x)
					g.FillRectangle(
								new SolidBrush(_image.Image.GetPixel(x, y)),
								x * (int)(Square * _scale),
								y * (int)(Square * _scale),
								(int)(Square * _scale),
								(int)(Square * _scale));

			if (_lines)
			{
				for (int x = 0; x != width + 1; ++x)
					g.DrawLine(
							Pens.Black,
							x * (int)(Square * _scale),
							0,
							x * (int)(Square * _scale),
							height * (int)(Square * _scale));

				for (int y = 0; y != height + 1; ++y)
					g.DrawLine(
							Pens.Black,
							0,
							y * (int)(Square * _scale),
							width * (int)(Square * _scale),
							y * (int)(Square * _scale));
			}
		}

		public void SelectColor(int index)
		{}
	}
}
