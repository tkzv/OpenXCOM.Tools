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
		#region Properties
		private XCImage _sprite;
		internal XCImage Sprite
		{
			private get { return _sprite; }
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

				if (Sprite != null)
				{
					Sprite.Image.Palette = _palette.Colors;
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
		/// <summary>
		/// cTor.
		/// </summary>
		internal EditorPanel()
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

			int width  = Sprite.Image.Width;
			int height = Sprite.Image.Height;

			for (int y = 0; y != height; ++y)
			for (int x = 0; x != width;  ++x)
				graphics.FillRectangle(
									new SolidBrush(Sprite.Image.GetPixel(x, y)),
									x * _scale,
									y * _scale,
										_scale,
										_scale);

			if (_grid)
			{
				for (int x = 0; x != width + 1; ++x)
					graphics.DrawLine(
									Pens.Black,
									x      * _scale,
									0,
									x      * _scale,
									height * _scale);

				for (int y = 0; y != height + 1; ++y)
					graphics.DrawLine(
									Pens.Black,
									0,
									y     * _scale,
									width * _scale,
									y     * _scale);
			}
		}
		#endregion
	}
}
