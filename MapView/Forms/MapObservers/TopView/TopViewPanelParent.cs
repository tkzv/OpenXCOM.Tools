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

		private int _mouseRow = -1;
		private int _mouseCol = -1;

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
		public override IMapBase MapBase
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
			DrawSelectionLozenge();
		}

		internal protected void OnMouseDrag()
		{
			DrawSelectionLozenge();
		}

		/// <summary>
		/// Draws a lozenge-border around all tiles that are selected or
		/// being selected.
		/// </summary>
		private void DrawSelectionLozenge()
		{
			if (MainViewUnderlay.Instance.MainView.FirstClick)
			{
				var start = GetDragStart();
				var end   = GetDragEnd();
	
				int hWidth  = _blobService.HalfWidth;
				int hHeight = _blobService.HalfHeight;
	
				var sel1 = new Point(
								_offX + (start.X - start.Y) * hWidth,
								_offY + (start.X + start.Y) * hHeight);
				var sel2 = new Point(
								_offX + (end.X - start.Y) * hWidth + hWidth,
								_offY + (end.X + start.Y) * hHeight + hHeight);
				var sel3 = new Point(
								_offX + (end.X - end.Y) * hWidth,
								_offY + (end.X + end.Y) * hHeight + hHeight + hHeight);
				var sel4 = new Point(
								_offX + (start.X - end.Y) * hWidth - hWidth,
								_offY + (start.X + end.Y) * hHeight + hHeight);
	
				_lozSelected.Reset();
				_lozSelected.AddLine(sel1, sel2);
				_lozSelected.AddLine(sel2, sel3);
				_lozSelected.AddLine(sel3, sel4);
				_lozSelected.CloseFigure();
	
				Refresh();
			}
		}

		private static Point GetDragStart()
		{
			var start = new Point(0, 0);
			start.X = Math.Min(
							MainViewUnderlay.Instance.MainView.DragStart.X,
							MainViewUnderlay.Instance.MainView.DragEnd.X);
			start.Y = Math.Min(
							MainViewUnderlay.Instance.MainView.DragStart.Y,
							MainViewUnderlay.Instance.MainView.DragEnd.Y);
			return start;
		}

		private static Point GetDragEnd()
		{
			var end = new Point(0, 0);
			end.X = Math.Max(
						MainViewUnderlay.Instance.MainView.DragStart.X,
						MainViewUnderlay.Instance.MainView.DragEnd.X);
			end.Y = Math.Max(
						MainViewUnderlay.Instance.MainView.DragStart.Y,
						MainViewUnderlay.Instance.MainView.DragEnd.Y);
			return end;
		}


		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, SolidBrush> TopBrushes // question: why can TopView access this
		{ get; set; }

		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, Pen> TopPens // question: why can TopView access this
		{ get; set; }


/*		public override void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			var pt = e.MapPosition;
//			Text = "c: " + pt.Col + " r: " + pt.Row; // I don't think this actually prints anywhere.

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

				if (MainViewUnderlay.Instance.MainView.FirstClick)
					backBuffer.DrawPath(TopPens[TopView.SelectedColor], _lozSelected);

				if (   _mouseCol > -1
					&& _mouseRow > -1
					&& _mouseCol < MapBase.MapSize.Cols
					&& _mouseRow < MapBase.MapSize.Rows)
				{
					int x = (_mouseCol - _mouseRow) * hWidth  + _offX;
					int y = (_mouseCol + _mouseRow) * hHeight + _offY;

					SetSelectorPath(x, y);
					backBuffer.DrawPath(TopPens[TopView.SelectorColor], _lozSelector);
				}
			}
		}

//		/// <summary>
//		/// Forwards the call to TopViewPanel.DrawTileBlobs().
//		/// </summary>
//		/// <param name="tile"></param>
//		/// <param name="g"></param>
//		/// <param name="x"></param>
//		/// <param name="y"></param>
//		internal protected virtual void DrawTileBlobs(
//				MapTileBase tile,
//				Graphics g,
//				int x, int y)
//		{}

		/// <summary>
		/// Gets the GraphicsPath to draw for ... what.
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
					MainViewUnderlay.Instance.MainView.FirstClick = true;

					MapBase.SelectedTile = new MapLocation(
														pt.Y,
														pt.X,
														MapBase.CurrentHeight);

					_isMouseDrag = true;
					MainViewUnderlay.Instance.MainView.SetDrag(pt, pt);
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
			MainViewUnderlay.Instance.MainView.Refresh();

			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var pt = ConvertCoordsDiamond(
										e.X - _offX,
										e.Y - _offY);
			if (pt.X != _mouseCol || pt.Y != _mouseRow)
			{
				_mouseCol = pt.X;
				_mouseRow = pt.Y;

				if (_isMouseDrag)
					MainViewUnderlay.Instance.MainView.SetDrag(
															MainViewUnderlay.Instance.MainView.DragStart,
															pt);

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
