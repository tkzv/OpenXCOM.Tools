using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class EditorPanel
		:
			Panel
	{
		#region Fields (static)
		private const int Square = 1;
		#endregion


		#region Properties
		private XCImage _sprite;
		internal XCImage Sprite
		{
			get { return _sprite; }
			set
			{
				_sprite = value;
				Refresh();
			}
		}

		private Palette _palette;
		internal Palette Pal
		{
			set
			{
				_palette = value;
				if (_sprite != null)
				{
					_sprite.Image.Palette = _palette.Colors;
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

		private int _scale = 10;
		internal int ScaleFactor
		{
			set
			{
				_scale = value;
				Refresh();
			}
		}
		#endregion


		#region cTor
		internal EditorPanel(XCImage sprite)
		{
			// form level code to fix flicker
//			protected override CreateParams CreateParams
//			{
//				get
//				{
//					CreateParams cp = base.CreateParams;
//					cp.ExStyle |= 0x02000000;  // Turn on 'WS_EX_COMPOSITED'
//					return cp;
//				}
//			}

			// user control level code to fix flicker when there's a background image
//			protected override CreateParams CreateParams
//			{
//				get
//				{
//					var parms = base.CreateParams;
//					parms.Style &= ~0x02000000;  // Turn off 'WS_CLIPCHILDREN'
//					return parms;
//				}
//			}

//			DoubleBuffered = true;
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
//			UpdateStyles();

			_sprite = sprite;
		}
		#endregion


//		For the form resize Events (onResizeBegin & on ResizeEnd) use the following code:
//
//		protected override void OnResizeBegin(EventArgs e) 
//		{
//			SuspendLayout();
//			base.OnResizeBegin(e);
//		}
//
//		protected override void OnResizeEnd(EventArgs e) 
//		{
//			ResumeLayout();
//			base.OnResizeEnd(e);
//		}


		#region EventCalls
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			int width  = _sprite.Image.Width;
			int height = _sprite.Image.Height;

			int factor = Square * _scale;

			for (int y = 0; y != height; ++y)
			for (int x = 0; x != width;  ++x)
				graphics.FillRectangle(
									new SolidBrush(_sprite.Image.GetPixel(x, y)),
									x * factor,
									y * factor,
										factor,
										factor);

			if (_grid)
			{
				for (int x = 0; x != width + 1; ++x)
					graphics.DrawLine(
									Pens.Black,
									x      * factor,
									0,
									x      * factor,
									height * factor);

				for (int y = 0; y != height + 1; ++y)
					graphics.DrawLine(
									Pens.Black,
									0,
									y     * factor,
									width * factor,
									y     * factor);
			}
		}
		#endregion
	}
}
