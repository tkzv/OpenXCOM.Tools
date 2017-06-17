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
		#region Properties (static)
		internal static EditorPanel Instance
		{ get; set; }
		#endregion


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
			Instance = this;

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

//			MouseClick += OnEditMouseClick;

			PckViewForm.PaletteChangedEvent += OnPaletteChanged; // NOTE: lives the life of the app, so no leak.
		}
		#endregion


//		For the form resize Events (onResizeBegin & on ResizeEnd) use the following code:
//
//		protected override void OnResizeBegin(EventArgs e) 
//		{
//			SuspendLayout();
//			base.OnResizeBegin(e);
//		}
//		protected override void OnResizeEnd(EventArgs e) 
//		{
//			ResumeLayout();
//			base.OnResizeEnd(e);
//		}


		#region EventCalls
		private void OnPaletteChanged(Palette pal)
		{
			Refresh();
//			if (Sprite != null)
//			{
//				Sprite.Image.Palette = pal.Colors;
//				Refresh();
//			}
		}

//		private void OnEditMouseClick(object sender, EventArgs e)
//		{
//			if (EditorForm.Mode == EditorForm.EditMode.ModeEnabled)
//			{
//				// change a clicked pixel's palette-id (color) to whatever the
//				// current 'PaletteId' is in PalettePanel.
//				
//			}
//		}
		#endregion


		#region EventCalls (override)
		/// <summary>
		/// Changes a clicked pixel's palette-id (color) to whatever the current
		/// 'PaletteId' is in PalettePanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (EditorForm.Mode == EditorForm.EditMode.ModeEnabled)
			{
				int palId = PalettePanel.Instance.PaletteId;
				if (palId > -1 && palId < 256)
				{
					int x = e.X / _scale;
					int y = e.Y / _scale;

					var color = PckViewForm.Pal[palId];

					int id = y * (Sprite.Bindata.Length / 40) + x;
					Sprite.Bindata[id] = (byte)palId;
					Sprite.Image = BitmapService.MakeBitmapTrue(
															XCImageFile.SpriteWidth,
															XCImageFile.SpriteHeight,
															Sprite.Bindata,
															PckViewForm.Pal.ColorTable);
					Refresh();
					PckViewPanel.Instance.Refresh();
				}
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			if (Sprite != null)
			{
				for (int y = 0; y != XCImageFile.SpriteHeight; ++y)
				for (int x = 0; x != XCImageFile.SpriteWidth;  ++x)
					graphics.FillRectangle(
										new SolidBrush(Sprite.Image.GetPixel(x, y)),
										x * _scale,
										y * _scale,
											_scale,
											_scale);
			}


//			var p0 = new Point(0,     1); // draw a 1px border around the image ->
//			var p1 = new Point(Width, 1);
//			var p2 = new Point(Width, Height);
//			var p3 = new Point(1,     Height);
//			var p4 = new Point(1,     1);
			var p0 = new Point( // draw a 1px border around the image ->
							0,
							1);
			var p1 = new Point(
							XCImageFile.SpriteWidth  * _scale,
							1);
			var p2 = new Point(
							XCImageFile.SpriteWidth  * _scale,
							XCImageFile.SpriteHeight * _scale);
			var p3 = new Point(
							1,
							XCImageFile.SpriteHeight * _scale);
			var p4 = new Point(
							1,
							1);

			var path = new GraphicsPath();

			path.AddLine(p0, p1);
			path.AddLine(p1, p2);
			path.AddLine(p2, p3);
			path.AddLine(p3, p4);

			graphics.DrawPath(Pens.Black, path);


			if (_grid)
			{
//				for (int x = 0; x != XCImageFile.SpriteWidth + 1; ++x)
				for (int x = 0; x != XCImageFile.SpriteWidth; ++x)
					graphics.DrawLine(
									Pens.Black,
									x * _scale,
									0,
									x * _scale,
									XCImageFile.SpriteHeight * _scale);

//				for (int y = 0; y != XCImageFile.SpriteHeight + 1; ++y)
				for (int y = 0; y != XCImageFile.SpriteHeight; ++y)
					graphics.DrawLine(
									Pens.Black,
									0,
									y * _scale,
									XCImageFile.SpriteWidth * _scale,
									y * _scale);
			}
		}
		#endregion


		#region Methods
		internal void ClearSprite()
		{
			Sprite = null;
			Refresh();
		}
		#endregion
	}
}
