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
		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over cursor lozenge
		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // selected tile or tiles being drag-selected
//		private readonly GraphicsPath _lozSel      = new GraphicsPath();

		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, Pen> TopPens // question: why can TopView access this
		{ get; set; }

		[Browsable(false), DefaultValue(null)]
		internal protected Dictionary<string, SolidBrush> TopBrushes // question: why can TopView access this
		{ get; set; }


		private int _col = -1; // these track the location of the mouse-cursor
		private int _row = -1;
		
		private int _xOffset;
		private int _yOffset;


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


//		#region cTor
//		/// <summary>
//		/// cTor. Instantiated only as the parent of TopViewPanel.
//		/// </summary>
//		internal protected TopViewPanelParent()
//		{}
//		#endregion


		[Browsable(false), DefaultValue(null)]
		public override XCMapBase MapBase
		{
			set
			{
				base.MapBase = value;

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
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;
				
				int curWidth = halfWidth;

				width  -= halfWidth; // don't clip the bottom or right tips of the big-lozenge.
				height -= halfHeight;


				if (MapBase.MapSize.Rows > 0 || MapBase.MapSize.Cols > 0) // safety vs. div-by-0
				{
					if (height > width / 2) // use width
					{
						halfWidth = width / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);

						if (halfWidth % 2 != 0)
							--halfWidth;

						halfHeight = halfWidth / 2;
					}
					else // use height
					{
						halfHeight = height / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);
						halfWidth  = halfHeight * 2;
					}
				}

//				if (halfHeight < _lozHeightMin)
//				{
//					halfWidth  = _lozHeightMin * 2;
//					halfHeight = _lozHeightMin;
//				}
				if (halfHeight < 1)
				{
					halfHeight = 1;
					halfWidth  = 2;
				}

				_blobService.HalfWidth  = halfWidth;
				_blobService.HalfHeight = halfHeight;

				_xOffset = 4 + MapBase.MapSize.Rows * halfWidth;
				_yOffset = 4;

				if (curWidth != halfWidth)
				{
					Width  = 8 + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfWidth;
					Height = 8 + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfHeight;

					Refresh();
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			PathSelectedLozenge();
		}

		internal protected void OnMouseDrag()
		{
			PathSelectedLozenge();
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around all tiles that
		/// are selected or being selected.
		/// </summary>
		internal void PathSelectedLozenge()
		{
			var dragStart = GetDragStart();
			var dragEnd   = GetDragEnd();

			int halfWidth  = _blobService.HalfWidth;
			int halfHeight = _blobService.HalfHeight;

			var p1 = new Point(
							_xOffset + (dragStart.X - dragStart.Y) * halfWidth,
							_yOffset + (dragStart.X + dragStart.Y) * halfHeight);
			var p2 = new Point(
							_xOffset + (dragEnd.X - dragStart.Y) * halfWidth  + halfWidth,
							_yOffset + (dragEnd.X + dragStart.Y) * halfHeight + halfHeight);
			var p3 = new Point(
							_xOffset + (dragEnd.X - dragEnd.Y) * halfWidth,
							_yOffset + (dragEnd.X + dragEnd.Y) * halfHeight + halfHeight * 2);
			var p4 = new Point(
							_xOffset + (dragStart.X - dragEnd.Y) * halfWidth  - halfWidth,
							_yOffset + (dragStart.X + dragEnd.Y) * halfHeight + halfHeight);

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


/*		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLocationSelected_Observer(LocationSelectedEventArgs args)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TopViewPanelParent.OnLocationSelected_Observer");

			var pt = e.MapLocation;
//			Text = "c " + pt.Col + "  r " + pt.Row; // I don't think this actually prints anywhere.

			var halfWidth  = _drawService.HalfWidth;
			var halfHeight = _drawService.HalfHeight;

			int xc = (pt.Col - pt.Row) * halfWidth;
			int yc = (pt.Col + pt.Row) * halfHeight;

			_lozSel.Reset();
			_lozSel.AddLine(
					xc, yc,
					xc + halfWidth, yc + halfHeight);
			_lozSel.AddLine(
					xc + halfWidth, yc + halfHeight,
					xc, yc + 2 * halfHeight);
			_lozSel.AddLine(
					xc, yc + 2 * halfHeight,
					xc - halfWidth, yc + halfHeight);
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
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;

				for (int
						r = 0,
							startX = _xOffset,
							startY = _yOffset;
						r != MapBase.MapSize.Rows;
						++r,
							startX -= halfWidth,
							startY += halfHeight)
				{
					for (int
							c = 0,
								x = startX,
								y = startY;
							c != MapBase.MapSize.Cols;
							++c,
								x += halfWidth,
								y += halfHeight)
					{
						var mapTile = MapBase[r, c] as MapTileBase;
						if (mapTile != null)
							((TopViewPanel)this).DrawTileBlobs(mapTile, backBuffer, x, y);
					}
				}

				for (int i = 0; i <= MapBase.MapSize.Rows; ++i)
					backBuffer.DrawLine(
									TopPens[TopView.GridColor],
									_xOffset - i * halfWidth,
									_yOffset + i * halfHeight,
									(MapBase.MapSize.Cols - i) * halfWidth  + _xOffset,
									(MapBase.MapSize.Cols + i) * halfHeight + _yOffset);

				for (int i = 0; i <= MapBase.MapSize.Cols; ++i)
					backBuffer.DrawLine(
									TopPens[TopView.GridColor],
									_xOffset + i * halfWidth,
									_yOffset + i * halfHeight,
									i * halfWidth  - MapBase.MapSize.Rows * halfWidth  + _xOffset,
									i * halfHeight + MapBase.MapSize.Rows * halfHeight + _yOffset);

				if (MainViewUnderlay.Instance.MainViewOverlay.FirstClick)
					backBuffer.DrawPath(TopPens[TopView.SelectedColor], _lozSelected);

				if (   _col > -1 && _col < MapBase.MapSize.Cols
					&& _row > -1 && _row < MapBase.MapSize.Rows)
				{
					PathSelectorLozenge(
									(_col - _row) * halfWidth  + _xOffset,
									(_col + _row) * halfHeight + _yOffset);
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
		private void PathSelectorLozenge(int x, int y)
		{
			int halfWidth  = _blobService.HalfWidth;
			int halfHeight = _blobService.HalfHeight;

			_lozSelector.Reset();
			_lozSelector.AddLine(
						x,             y,
						x + halfWidth, y + halfHeight);
			_lozSelector.AddLine(
						x + halfWidth, y + halfHeight,
						x,             y + halfHeight * 2);
			_lozSelector.AddLine(
						x,             y + halfHeight * 2,
						x - halfWidth, y + halfHeight);
			_lozSelector.CloseFigure();
		}

		private Point ConvertCoordsDiamond(int x, int y)
		{
			// 16 is half the width of the diamond
			// 24 is the distance from the top of the diamond to the very top of the image

			double halfWidth  = (double)_blobService.HalfWidth;
			double halfHeight = (double)_blobService.HalfHeight;

			double x1 =  (x          / (halfWidth * 2)) + (y / (halfHeight * 2));
			double x2 = -(x - y * 2) / (halfWidth * 2);

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
											e.X - _xOffset,
											e.Y - _yOffset);
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
			var pt = ConvertCoordsDiamond(
										e.X - _xOffset,
										e.Y - _yOffset);
			if (pt.X != _col || pt.Y != _row)
			{
				_col = pt.X;
				_row = pt.Y;

				if (_isMouseDrag)
				{
					var overlay = MainViewUnderlay.Instance.MainViewOverlay;
					overlay.SetDrag(overlay.DragStart, pt);
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
