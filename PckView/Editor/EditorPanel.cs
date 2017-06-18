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
		#region Fields
		private readonly StatusBar _statusBar = new StatusBar();
		private StatusBarPanel _sbpEyeDropper = new StatusBarPanel();
		#endregion


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


			_sbpEyeDropper.AutoSize = StatusBarPanelAutoSize.Spring;

			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.SizingGrip = false;
			_statusBar.ShowPanels = true;
			_statusBar.Panels.Add(_sbpEyeDropper);

			Controls.Add(_statusBar);

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


		#region EventCalls (override)
		/// <summary>
		/// Changes a clicked pixel's palette-id (color) to whatever the current
		/// 'PaletteId' is in PalettePanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			int pixelX = e.X / _scale;
			int pixelY = e.Y / _scale;

			int bindataId = pixelY * (Sprite.Bindata.Length / XCImageFile.SpriteHeight) + pixelX;

			if (bindataId > -1 && bindataId < Sprite.Bindata.Length)
			{
				switch (EditorForm.Mode)
				{
					case EditorForm.EditMode.ModeEnabled: // paint ->
					{
						int palId = PalettePanel.Instance.PaletteId;
						if (palId > -1 && palId < PckImage.SpriteTransparencyByte)	// NOTE: 0xFE and 0xFF are reserved for special
						{															// stuff when reading/writing the .PCK file.
//							var color = PckViewForm.Pal[palId];

							Sprite.Bindata[bindataId] = (byte)palId;
							Sprite.Image = BitmapService.MakeBitmapTrue(
																	XCImageFile.SpriteWidth,
																	XCImageFile.SpriteHeight,
																	Sprite.Bindata,
																	PckViewForm.Pal.ColorTable);
							Refresh();
							PckViewPanel.Instance.Refresh();
						}
						else
						{
							switch (palId)
							{
								case PckImage.SpriteTransparencyByte:	// #254
								case PckImage.SpriteStopByte:			// #255
									MessageBox.Show(
												this,
												"The colortable indices #254 and #255 are reserved"
													+ " for reading and writing the .PCK file."
													+ Environment.NewLine + Environment.NewLine
													+ "#254 is used for RLE encoding"
													+ Environment.NewLine
													+ "#255 is the end-of-sprite marker",
												"Error",
												MessageBoxButtons.OK,
												MessageBoxIcon.Error,
												MessageBoxDefaultButton.Button1,
												0);
									break;
							}
						}
						break;
					}

					case EditorForm.EditMode.ModeLocked: // eye-dropper ->
						PalettePanel.Instance.SelectPaletteId((int)Sprite.Bindata[bindataId]);
						break;
				}
			}
		}

		/// <summary>
		/// Displays the color of any mouseovered paletteId.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

			int pixelX = e.X / _scale;
			int pixelY = e.Y / _scale;

			int bindataId = pixelY * (Sprite.Bindata.Length / XCImageFile.SpriteHeight) + pixelX;

			if (bindataId > -1 && bindataId < Sprite.Bindata.Length)
			{
				int palId = Sprite.Bindata[bindataId];

				// TODO: what follows is lifted from PaletteForm.OnPaletteIdChanged()
				string text = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"id:{0} (0x{0:X2})",
										palId);

				var color = PckViewForm.Pal[palId];
				text += String.Format(
									System.Globalization.CultureInfo.CurrentCulture,
									" r:{0} g:{1} b:{2} a:{3}",
									color.R,
									color.G,
									color.B,
									color.A);

				switch (palId)
				{
					case 0:
						text += " [transparent]";
						break;

					// the following values cannot be palette-ids. They have special meaning in the .PCK file.
					case 254: // transparency marker
					case 255: // end of file marker
						text += " [invalid]";
						break;
				}

				_sbpEyeDropper.Text = text;
			}
			else
				_sbpEyeDropper.Text = String.Empty;
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
		#endregion


		#region Methods
		internal void ClearSprite()
		{
			Sprite = null;
			Refresh();
		}

		internal int GetStatusBarHeight()
		{
			return _statusBar.Height;
		}
		#endregion
	}
}
