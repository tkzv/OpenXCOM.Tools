using System;
using System.Drawing;
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

		#endregion


		#region cTor

		internal EditorPanel(XCImage image)
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

			_image = image;
		}
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

		#endregion


		#region Static

		internal static int PreferredWidth
		{
			get { return PckImage.Width * 10; }
		}

		internal static int PreferredHeight
		{
			get { return PckImage.Height * 10; }
		}

		#endregion


		#region Events

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
			var g = e.Graphics;

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

		#endregion
	}
}
