using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	internal delegate void PaletteIndexChangedEventHandler(int selectedId);


	internal sealed class PalettePanel
		:
			Panel
	{
//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private Palette _palette;

		private const int Pad = 0; // well, you know ...

		private int _width  = 15;
		private int _height = 10;

		private int _id;
		private int _clickX;
		private int _clickY;

		internal const int Across = 16;

		internal event PaletteIndexChangedEventHandler PaletteIndexChangedEvent;


		internal PalettePanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_palette = null;

			MouseDown += OnMouseDown;

			_clickX = -100;
			_clickY = -100;

			_id = -1;
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_width  = (Width  / Across) - 2 * Pad;
			_height = (Height / Across) - 2 * Pad;

			_clickX = (_id % Across) * (_width  + 2 * Pad);
			_clickY = (_id / Across) * (_height + 2 * Pad);

			Refresh();
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			_clickX = (e.X / (_width  + 2 * Pad)) * (_width  + 2 * Pad);
			_clickY = (e.Y / (_height + 2 * Pad)) * (_height + 2 * Pad);

			_id = (e.X / (_width + 2 * Pad)) + (e.Y / (_height + 2 * Pad)) * Across;

			if (PaletteIndexChangedEvent != null && _id < 256)
			{
				PaletteIndexChangedEvent(_id);
				Refresh();
			}
		}

		[DefaultValue(null)]
		[Browsable(false)]
		internal Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_palette != null)
			{
				Graphics g = e.Graphics;

				for (int
						i = 0, y = Pad;
						i < Across;
						++i, y += _height + Pad * 2)
					for (int
							j = 0, x = Pad;
							j < Across;
							++j, x += _width + Pad * 2)
					{
						g.FillRectangle(new SolidBrush(
													_palette[i * Across + j]),
													x, y,
													_width, _height);
					}

				g.DrawRectangle(
							Pens.Red,
							_clickX, _clickY,
							_width + Pad * 2 - 1, _height + Pad * 2 - 1);
			}
		}
	}
}
