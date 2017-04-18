using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	/// <summary>
	/// EditorPane class.
	/// </summary>
	internal sealed class EditorPane
		:
			Panel
	{
		private XCImage _image;
		internal XCImage Image
		{
			set
			{
				_image = value;
				Refresh();
			}
		}

		private Palette _palette;
		internal Palette Palette
		{
			set
			{
				_palette = value;
				if (_image != null)
				{
					_image.Image.Palette = _palette.Colors;
					Refresh();
				}
			}
		}

		private bool _grid;
		internal bool Grid
		{
			set
			{
				_grid = value;
				Refresh();
			}
		}


		internal EditorPane(XCImage image)
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

			_image = image;
		}


		internal static int PreferredWidth
		{
			get { return PckImage.Width * 10; }
		}

		internal static int PreferredHeight
		{
			get { return PckImage.Height * 10; }
		}

		private const int Square = 1;

		private int _scale = 10;
		internal int ScaleFactor
		{
			set
			{
				_scale = value;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int width  = _image.Image.Width;
			int height = _image.Image.Height;

			int factor = Square * _scale;

			for (int y = 0; y != height; ++y)
				for (int x = 0; x != width; ++x)
					g.FillRectangle(
								new SolidBrush(_image.Image.GetPixel(x, y)),
								x * factor,
								y * factor,
								    factor,
								    factor);

			if (_grid)
			{
				for (int x = 0; x != width + 1; ++x)
					g.DrawLine(
							Pens.Black,
							x      * factor,
							0,
							x      * factor,
							height * factor);

				for (int y = 0; y != height + 1; ++y)
					g.DrawLine(
							Pens.Black,
							0,
							y     * factor,
							width * factor,
							y     * factor);
			}
		}
	}
}
