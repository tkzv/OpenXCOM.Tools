using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// A base class for TopViewPanel.
	/// </summary>
	internal class TopViewPanelParent
		:
			MapObserverControl1
	{
		#region Fields & Properties
		private int _offX;
		private int _offY;

		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over cursor lozenge
		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // selected tile or tiles being drag-selected
//		private readonly GraphicsPath _lozSel      = new GraphicsPath();

		private int _cursorCol = -1;
		private int _cursorRow = -1;
		
		private DrawBlobService _blobService = new DrawBlobService();
		internal protected DrawBlobService BlobService
		{
			get { return _blobService; }
		}

//		private int _lozHeightMin = 4;
//		internal protected int TileLozengeHeight	// question: why can TopView access this
//		{											// it's 'protected'
//			get { return _lozHeightMin; }
//			set
//			{
//				_lozHeightMin = value;
//				ResizeObserver(Width, Height);
//			}
//		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of TopViewPanel.
		/// </summary>
		internal protected TopViewPanelParent()
		{}
		#endregion


		[Browsable(false), DefaultValue(null)]
		public override XCMapBase MapBase
		{
			set
			{
				base.MapBase = value;
//				_blobService.HalfWidth = 7; // TODO: 7 ... inits to 8 in DrawContentService.
				_blobService.HalfWidth = 8;
				ResizeObserver(Parent.Width, Parent.Height);

				Refresh();
			}
		}

		/// <summary>
		/// Called by the main panel's resize event. See TopView. Also fired by
		/// TileLozengeHeight set change, or by a straight MapBase set change.
		/// </summary>
		/// <param name="width">the width to resize to</param>
		/// <param name="height">the height to resize to</param>
		internal protected void ResizeObserver(int width, int height)
		{
			if (MapBase != null)
			{
				int hWidth  = _blobService.HalfWidth;
				int hHeight = _blobService.HalfHeight;
				
				int curWidth = hWidth;

				width  -= hWidth; // don't clip the bottom or right tips of the big-lozenge.
				height -= hHeight;


				if (MapBase.MapSize.Rows > 0 || MapBase.MapSize.Cols > 0) // safety vs. div-by-0
				{
					if (height > width / 2) // use width
					{
						hWidth = width / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);

						if (hWidth % 2 != 0)
							--hWidth;

						hHeight = hWidth / 2;
					}
					else // use height
					{
						hHeight = height / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);
						hWidth  = hHeight * 2;
					}
				}

//				if (hHeight < _lozHeightMin)
//				{
//					hWidth  = _lozHeightMin * 2;
//					hHeight = _lozHeightMin;
//				}
				if (hHeight < 1)
				{
					hHeight = 1;
					hWidth  = 2;
				}

				_blobService.HalfWidth  = hWidth;
				_blobService.HalfHeight = hHeight;

				_offX = 4 + MapBase.MapSize.Rows * hWidth;
				_offY = 4;

				if (curWidth != hWidth)
				{
					Width  = 8 + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * hWidth;
					Height = 8 + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * hHeight;

					Refresh();
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetSelectedBorder();
		}

		internal protected void OnMouseDrag()
		{
			SetSelectedBorder();
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around all tiles that
		/// are selected or being selected.
		/// </summary>
		internal void SetSelectedBorder()
		{
			var dragStart = GetDragStart();
			var dragEnd   = GetDragEnd();

			int hWidth  = _blobService.HalfWidth;
			int hHeight = _blobService.HalfHeight;

			var p1 = new Point(
							_offX + (dragStart.X - dragStart.Y) * hWidth,
							_offY + (dragStart.X + dragStart.Y) * hHeight);
			var p2 = new Point(
							_offX + (dragEnd.X - dragStart.Y) * hWidth + hWidth,
							_offY + (dragEnd.X + dragStart.Y) * hHeight + hHeight);
			var p3 = new Point(
							_offX + (dragEnd.X - dragEnd.Y) * hWidth,
							_offY + (dragEnd.X + dragEnd.Y) * hHeight + hHeight + hHeight);
			var p4 = new Point(
							_offX + (dragStart.X - dragEnd.Y) * hWidth - hWidth,
							_offY + (dragStart.X + dragEnd.Y) * hHeight + hHeight);

			_lozSelected.Reset();
			_lozSelected.AddLine(p1, p2);
			_lozSelected.AddLine(p2, p3);
			_lozSelected.AddLine(p3, p4);
			_lozSelected.CloseFigure();

			Refresh();
		}

		/// <summary>
		/// Gets the drag-start point. Note that 'MainViewOverlay' will bound the
		/// point between 0,0 and the map's maximum dimensions (-1) inclusively.
		/// </summary>
		/// <returns></returns>
		private static Point GetDragStart()
		{
			return new Point(
						Math.Min(
								MainViewUnderlay.Instance.MainViewOverlay.DragStart.X,
								MainViewUnderlay.Instance.MainViewOverlay.DragEnd.X),
						Math.Min(
								MainViewUnderlay.Instance.MainViewOverlay.DragStart.Y,
								MainViewUnderlay.Instance.MainViewOverlay.DragEnd.Y));
		}

		/// <summary>
		/// Gets the drag-end point. Note that 'MainViewOverlay' will bound the
		/// point between 0,0 and the map's maximum dimensions (-1) inclusively.
		/// </summary>
		/// <returns></returns>
		private static Point GetDragEnd()
		{
			return new Point(
						Math.Max(
								MainViewUnderlay.Instance.MainViewOverlay.DragStart.X,
								MainViewUnderlay.Instance.MainViewOverlay.DragEnd.X),
						Math.Max(
								MainViewUnderlay.Instance.MainViewOverlay.DragStart.Y,
								MainViewUnderlay.Instance.MainViewOverlay.DragEnd.Y));
		}


		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, SolidBrush> TopBrushes // question: why can TopView access this
		{ get; set; }

		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, Pen> TopPens // question: why can TopView access this
		{ get; set; }


/*		/// <summary>
		/// Inherits from IMapObserver.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnLocationSelected_Observer(XCMapBase sender, LocationSelectedEventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TopViewPanelParent.OnLocationSelected_Observer");

			var pt = e.MapLocation;
//			Text = "c " + pt.Col + "  r " + pt.Row; // I don't think this actually prints anywhere.

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			int xc = (pt.Col - pt.Row) * hWidth;
			int yc = (pt.Col + pt.Row) * hHeight;

			_lozSel.Reset();
			_lozSel.AddLine(
					xc, yc,
					xc + hWidth, yc + hHeight);
			_lozSel.AddLine(
					xc + hWidth, yc + hHeight,
					xc, yc + 2 * hHeight);
			_lozSel.AddLine(
					xc, yc + 2 * hHeight,
					xc - hWidth, yc + hHeight);
			_lozSel.CloseFigure();

			OnMouseDrag();

			Refresh(); // I don't think this is needed.
		} */

		// NOTE: there is no OnLevelChanged_Observer for TopView.


		protected override void RenderGraphics(Graphics backBuffer)
		{
			backBuffer.FillRectangle(SystemBrushes.Control, ClientRectangle);

			if (MapBase != null)
			{
				int hWidth  = _blobService.HalfWidth;
				int hHeight = _blobService.HalfHeight;

				for (int
						r = 0,
							startX = _offX,
							startY = _offY;
						r != MapBase.MapSize.Rows;
						++r,
							startX -= hWidth,
							startY += hHeight)
				{
					for (int
							c = 0,
								x = startX,
								y = startY;
							c != MapBase.MapSize.Cols;
							++c,
								x += hWidth,
								y += hHeight)
					{
						var mapTile = MapBase[r, c] as MapTileBase;
						if (mapTile != null)
							((TopViewPanel)this).DrawTileBlobs(mapTile, backBuffer, x, y);
					}
				}

				for (int i = 0; i <= MapBase.MapSize.Rows; ++i)
					backBuffer.DrawLine(
									TopPens[TopView.GridColor],
									_offX - i * hWidth,
									_offY + i * hHeight,
									(MapBase.MapSize.Cols - i) * hWidth  + _offX,
									(MapBase.MapSize.Cols + i) * hHeight + _offY);

				for (int i = 0; i <= MapBase.MapSize.Cols; ++i)
					backBuffer.DrawLine(
									TopPens[TopView.GridColor],
									_offX + i * hWidth,
									_offY + i * hHeight,
									i * hWidth  - MapBase.MapSize.Rows * hWidth  + _offX,
									i * hHeight + MapBase.MapSize.Rows * hHeight + _offY);

				if (MainViewUnderlay.Instance.MainViewOverlay.FirstClick)
					backBuffer.DrawPath(TopPens[TopView.SelectedColor], _lozSelected);

				if (   _cursorCol > -1
					&& _cursorRow > -1
					&& _cursorCol < MapBase.MapSize.Cols
					&& _cursorRow < MapBase.MapSize.Rows)
				{
					int x = (_cursorCol - _cursorRow) * hWidth  + _offX;
					int y = (_cursorCol + _cursorRow) * hHeight + _offY;

					SetSelectorPath(x, y);
					backBuffer.DrawPath(TopPens[TopView.SelectorColor], _lozSelector);
				}
			}
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around the tile that
		/// is currently mouse-overed.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private void SetSelectorPath(int x, int y)
		{
			int hWidth  = _blobService.HalfWidth;
			int hHeight = _blobService.HalfHeight;

			_lozSelector.Reset();
			_lozSelector.AddLine(
						x, y,
						x + hWidth, y + hHeight);
			_lozSelector.AddLine(
						x + hWidth, y + hHeight,
						x, y + 2 * hHeight);
			_lozSelector.AddLine(
						x, y + 2 * hHeight,
						x - hWidth, y + hHeight);
			_lozSelector.CloseFigure();
		}

		private Point ConvertCoordsDiamond(int x, int y)
		{
			// 16 is half the width of the diamond
			// 24 is the distance from the top of the diamond to the very top of the image

			double hWidth  = (double)_blobService.HalfWidth;
			double hHeight = (double)_blobService.HalfHeight;

			double x1 =  (x          / (hWidth * 2)) + (y / (hHeight * 2));
			double x2 = -(x - y * 2) / (hWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(x2));
		}


		private bool _isMouseDrag;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (MapBase != null)
			{
				var pt = ConvertCoordsDiamond(
											e.X - _offX,
											e.Y - _offY);
				if (   pt.Y >= 0 && pt.Y < MainViewUnderlay.Instance.MapBase.MapSize.Rows
					&& pt.X >= 0 && pt.X < MainViewUnderlay.Instance.MapBase.MapSize.Cols)
				{
					MainViewUnderlay.Instance.MainViewOverlay.FirstClick = true;

					MapBase.Location = new MapLocation(
													pt.Y,
													pt.X,
													MapBase.Level);

					_isMouseDrag = true;
					MainViewUnderlay.Instance.MainViewOverlay.SetDrag(pt, pt);
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
			MainViewUnderlay.Instance.MainViewOverlay.Refresh();

			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var ptCursor = ConvertCoordsDiamond(
											e.X - _offX,
											e.Y - _offY);
			if (ptCursor.X != _cursorCol || ptCursor.Y != _cursorRow)
			{
				_cursorCol = ptCursor.X;
				_cursorRow = ptCursor.Y;

				if (_isMouseDrag)
				{
					var overlay = MainViewUnderlay.Instance.MainViewOverlay;
					overlay.SetDrag(overlay.DragStart, ptCursor);
				}

				Refresh(); // mouseover refresh for TopView.
			}
		}

/*		/// <summary>
		/// Scrolls the z-axis for TopRouteView. Sort of .... no, well no it doesn't.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if		(e.Delta < 0) base.Map.Up();
			else if	(e.Delta > 0) base.Map.Down();
		} */
	}
}
