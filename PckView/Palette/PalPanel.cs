using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	internal delegate void PaletteClickDelegate(int selectedIndex);

	internal enum SelectMode
	{
		Bar,
		Single
	};


	internal sealed class PalPanel
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

		private SelectMode _mode;

		public const int Across = 16;

		public event PaletteClickDelegate PaletteIndexChanged;


		public PalPanel()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			_palette = null;
			this.MouseDown += mouseDown;
//			Width  = (width  + 2 * space) * NumAcross;
//			Height = (height + 2 * space) * NumAcross;
			_clickX = -100;
			_clickY = -100;
			_id = -1;
			_mode = SelectMode.Single;
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_width  = (Width  / Across) - 2 * Pad;
			_height = (Height / Across) - 2 * Pad;

			switch (_mode)
			{
				case SelectMode.Single:
					_clickX = (_id % Across) * (_width + 2 * Pad);
					break;

				case SelectMode.Bar:
					_clickX = 0;
					break;
			}
			_clickY = (_id / Across) * (_height + 2 * Pad);

			Refresh();
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			switch (_mode)
			{
				case SelectMode.Single:
					_clickX = (e.X / (_width + 2 * Pad)) * (_width + 2 * Pad);
					_id = (e.X / (_width + 2 * Pad)) + (e.Y / (_height + 2 * Pad)) * Across;
					break;

				case SelectMode.Bar:
					_clickX = 0;
					_id = (e.Y / (_height + 2 * Pad)) * Across;
					break;
			}

			_clickY = (e.Y / (_height + 2 * Pad)) * (_height + 2 * Pad);

			if (PaletteIndexChanged != null && _id < 256)
			{
				PaletteIndexChanged(_id);
				Refresh();
			}
		}

		[DefaultValue(SelectMode.Single)]
		[Category("Behavior")]
		public SelectMode Mode
		{
			get { return _mode; }
//			set { mode = value; }
		}

		[DefaultValue(null)]
		[Browsable(false)]
		public Palette Palette
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
						i++, y += _height + 2 * Pad)
					for (int
							j = 0, x = Pad;
							j < Across;
							j++, x += _width + 2 * Pad)
					{
						g.FillRectangle(new SolidBrush(
													_palette[i * Across + j]),
													x, y,
													_width, _height);
					}

				switch (_mode)
				{
					case SelectMode.Single:
						g.DrawRectangle(
									Pens.Red, // _brush
									_clickX, _clickY,
									_width + 2 * Pad - 1, _height + 2 * Pad - 1);
						break;

					case SelectMode.Bar:
						g.DrawRectangle(
									Pens.Red, // _brush
									_clickX, _clickY,
									(_width + 2 * Pad) * Across - 1, _height + 2 * Pad - 1);
						break;
				}
			}
		}
	}
}
