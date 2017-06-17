using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	internal delegate void PaletteIdChangedEventHandler(int selectedId);


	internal sealed class PalettePanel
		:
			Panel
	{
		#region Events
		internal event PaletteIdChangedEventHandler PaletteIdChangedEvent;
		#endregion


		#region Fields (static)
		internal const int Across = 16; // 16 swatches across the panel.
		#endregion


		#region Fields
		private int _tilesX;
		private int _tilesY;

		private int _clickX = -1;
		private int _clickY = -1;
		#endregion


		#region Properties (static)
		internal static PalettePanel Instance
		{ get; set; }

		private int _id = -1;
		internal int PaletteId
		{
			get { return _id; }
			set { _id = value; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PalettePanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			PckViewForm.PaletteChangedEvent += OnPaletteChanged; // NOTE: lives the life of the app, so no leak.

			Instance = this;
		}
		#endregion


		#region Eventcalls (override)
		protected override void OnResize(EventArgs eventargs)
		{
			_tilesX = Width  / Across;
			_tilesY = Height / Across;

			if (PaletteId != -1)
			{
				_clickX = PaletteId % Across * _tilesX + 1;
				_clickY = PaletteId / Across * _tilesY + 1;
			}
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			int tileX = e.X / _tilesX;
			int tileY = e.Y / _tilesY;

			_clickX = tileX * _tilesX + 1;
			_clickY = tileY * _tilesY + 1;

			PaletteId = tileY * Across + tileX;

			UpdateStatusPaletteId();
		}

		/// <summary>
		/// Draws the palette viewer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			//LogFile.WriteLine("##");
			for (int
					i = 0,
						y = 0;
					i != Across;
					++i,
						y += _tilesY)
			{
				for (int
						j = 0,
							x = 0;
						j != Across;
						++j,
							x += _tilesX)
				{
					//LogFile.WriteLine((i+j) + ": " + PckViewForm.Pal[i * Across + j]);
					graphics.FillRectangle(
										new SolidBrush(PckViewForm.Pal[i * Across + j]),
										x,       y,
										_tilesX, _tilesY);
				}
			}
			//LogFile.WriteLine("##");

			if (PaletteId != -1)
			{
				graphics.DrawRectangle(
									Pens.Red,
									_clickX, _clickY,
									_tilesX - 1, _tilesY - 1);
				graphics.FillRectangle( // -> fill the darn hole that .NET leaves in the top-left corner.
									Brushes.Red,
									_clickX - 1, _clickY - 1,
									1, 1);
			}
		}
		#endregion


		#region Eventcalls
		internal void UpdateStatusPaletteId()
		{
			if (PaletteId > -1 && PaletteId < 256
				&& PaletteIdChangedEvent != null)
			{
				PaletteIdChangedEvent(PaletteId);
				Refresh();
			}
		}

		private void OnPaletteChanged(Palette pal)
		{
			Refresh();
		}
		#endregion
	}
}
