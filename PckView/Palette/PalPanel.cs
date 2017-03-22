using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	public delegate void PaletteClickDelegate(int selectedIndex);

	public enum SelectMode
	{
		Bar,
		Single
	};


	public class PalPanel
		:
		Panel
	{
		private Palette _palette;
//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private const int _pad = 0;

		private int _width  = 15;
		private int _height = 10;

		private int _id;
		private int _clickX, _clickY;

		private SelectMode mode;

		public const int _across = 16;

		public event PaletteClickDelegate PaletteIndexChanged;


		public PalPanel()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			_palette = null;
			this.MouseDown += new MouseEventHandler(mouseDown);
//			Width  = (width  + 2 * space) * NumAcross;
//			Height = (height + 2 * space) * NumAcross;
			_clickX = -100;
			_clickY = -100;
			_id = -1;
			mode = SelectMode.Single;
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_width  = (Width  / _across) - 2 * _pad;
			_height = (Height / _across) - 2 * _pad;

			switch (mode)
			{
				case SelectMode.Single:
					_clickX = (_id % _across) * (_width + 2 * _pad);
					break;

				case SelectMode.Bar:
					_clickX = 0;
					break;
			}
			_clickY = (_id / _across) * (_height + 2 * _pad);

			Refresh();
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			switch (mode)
			{
				case SelectMode.Single:
					_clickX = (e.X / (_width + 2 * _pad)) * (_width + 2 * _pad);
					_id = (e.X / (_width + 2 * _pad)) + (e.Y / (_height + 2 * _pad)) * _across;
					break;

				case SelectMode.Bar:
					_clickX = 0;
					_id = (e.Y / (_height + 2 * _pad)) * _across;
					break;
			}

			_clickY = (e.Y / (_height + 2 * _pad)) * (_height + 2 * _pad);

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
			get { return mode; }
			set { mode = value; }
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
						i = 0, y = _pad;
						i < _across;
						i++, y += _height + 2 * _pad)
					for (int
							j = 0, x = _pad;
							j < _across;
							j++, x += _width + 2 * _pad)
					{
						g.FillRectangle(new SolidBrush(
													_palette[i * _across + j]),
													x, y,
													_width, _height);
					}

				switch(mode)
				{
					case SelectMode.Single:
						g.DrawRectangle(
									Pens.Red, // _brush
									_clickX, _clickY,
									_width + 2 * _pad - 1, _height + 2 * _pad - 1);
						break;

					case SelectMode.Bar:
						g.DrawRectangle(
									Pens.Red, // _brush
									_clickX, _clickY,
									(_width + 2 * _pad) * _across - 1, _height + 2 * _pad - 1);
						break;
				}
			}
		}
	}
}
