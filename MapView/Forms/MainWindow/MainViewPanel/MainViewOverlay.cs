using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
{
	internal delegate void MouseDragEventHandler();


	internal sealed class MainViewOverlay
		:
			Panel
	{
		internal event MouseDragEventHandler MouseDragEvent;


		private XCMapBase _mapBase;
		internal XCMapBase MapBase
		{
			get { return _mapBase; }
			set
			{
				if (_mapBase != null)
				{
					_mapBase.LevelChangedEvent     -= OnLevelChanged_Main;
					_mapBase.LocationSelectedEvent -= OnLocationSelected_Main;
				}

				if ((_mapBase = value) != null)
				{
					_mapBase.LevelChangedEvent     += OnLevelChanged_Main;
					_mapBase.LocationSelectedEvent += OnLocationSelected_Main;

					SetPanelSize();

//					DragStart = _dragStart;	// this might be how to give drags their legitimate values
//					DragEnd   = _dragEnd;	// after initialization to Point(-1/-1).
				}
			}
		}

		private Point _origin = new Point(100, 0);

		private CursorSprite _cursor;

		private const int HalfWidth  = 16;
		private const int HalfHeight =  8;

		private Point _dragStart = new Point(-1, -1);
		private Point _dragEnd   = new Point(-1, -1);

		private bool _firstClick;
		/// <summary>
		/// Flag that tells the viewers, including Main, that it's okay to draw
		/// a lozenge for a selected tile; ie, that an initial tile has actually
		/// been selected.
		/// Can also happen on MainView when GraySelected is false.
		/// </summary>
		internal bool FirstClick
		{
			get { return _firstClick; }
			set
			{
				_firstClick = value;

				if (!_firstClick)
				{
					_dragStart = new Point(-1, -1);
					_dragEnd   = new Point(-1, -1);

					ViewerFormsManager.TopView.Control.TopViewPanel.SetSelectedBorder();
				}
			}
		}


		private GraphicsPath _layerFill = new GraphicsPath();

		private Brush _brushLayer;

		private Color _colorLayer = Color.MediumVioletRed; // initial color for the grid-layer
		public Color GridLayerColor // public for Reflection.
		{
			get { return _colorLayer; }
			set
			{
				_colorLayer = value;
				_brushLayer = new SolidBrush(Color.FromArgb(GridLayerOpacity, _colorLayer));
				Refresh();
			}
		}

		private int _opacity = 200; // initial opacity for the grid-layer
		public int GridLayerOpacity // public for Reflection.
		{
			get { return _opacity; }
			set
			{
				_opacity = value.Clamp(0, 255);
				_brushLayer = new SolidBrush(Color.FromArgb(_opacity, ((SolidBrush)_brushLayer).Color));
				Refresh();
			}
		}

		private Pen _penGrid = new Pen(Brushes.Black, 1); // initial pen for grid-lines
		public Color GridLineColor // public for Reflection.
		{
			get { return _penGrid.Color; }
			set
			{
				_penGrid.Color = value;
				Refresh();
			}
		}
		public int GridLineWidth // public for Reflection.
		{
			get { return (int)_penGrid.Width; }
			set
			{
				_penGrid.Width = value;
				Refresh();
			}
		}

		private bool _showGrid = true; // initial val for show-grid
		public bool ShowGrid // public for Reflection.
		{
			get { return _showGrid; }
			set
			{
				_showGrid = value;
				Refresh();
			}
		}

		private bool _graySelection = true; // initial val for gray-selection
		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
		"CA1811:AvoidUncalledPrivateCode",
		Justification = "Because the setter is called dynamically w/ Reflection" +
		"or other: not only is it used it needs to be public.")]
		public bool GraySelection // public for Reflection.
		{
			get { return _graySelection; }
			set
			{
				_graySelection = value;
				Refresh();
			}
		}

		/// <summary>
		/// If true draws a translucent red box around selected tiles ->
		/// superceded by using GraySelection(false) property.
		/// </summary>
//		private bool _drawSelectionBox;
//		public bool DrawSelectionBox
//		{
//			get { return _drawSelectionBox; }
//			set
//			{
//				_drawSelectionBox = value;
//				Refresh();
//			}
//		}


		#region cTor
		internal MainViewOverlay()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_brushLayer = new SolidBrush(Color.FromArgb(GridLayerOpacity, GridLayerColor));

			KeyDown += OnEditKeyDown;
		}
		#endregion


		private void OnEditKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.X:
						Copy();
						ClearSelection();
						break;

					case Keys.C:
						Copy();
						break;

					case Keys.V:
						Paste();
						break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					case Keys.Delete:
						ClearSelection();
						break;
				}
			}
		}

		internal void ClearSelection()
		{
			if (_mapBase != null)
			{
				_mapBase.MapChanged = true;

				var start = GetDragStart();
				var end   = GetDragEnd();
	
				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						_mapBase[r, c] = XCMapTile.BlankTile;

				RefreshViewers();
			}
		}

		private MapTileBase[,] _copied;

		internal void Copy()
		{
			if (_mapBase != null)
			{
				var start = GetDragStart();
				var end   = GetDragEnd();

				_copied = new MapTileBase[end.Y - start.Y + 1, end.X - start.X + 1];

				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						_copied[r - start.Y, c - start.X] = _mapBase[r, c];
			}
		}

		internal void Paste()
		{
			if (_mapBase != null && _copied != null)
			{
				_mapBase.MapChanged = true;

				for (int
						r = DragStart.Y;
						r != _mapBase.MapSize.Rows && (r - DragStart.Y) < _copied.GetLength(0);
						++r)
					for (int
							c = DragStart.X;
							c != _mapBase.MapSize.Cols && (c - DragStart.X) < _copied.GetLength(1);
							++c)
					{
						var tile = _mapBase[r, c] as XCMapTile;
						if (tile != null)
						{
							var copyTile = _copied[r - DragStart.Y, c - DragStart.X] as XCMapTile;
							if (copyTile != null)
							{
								tile.Ground  = copyTile.Ground;
								tile.Content = copyTile.Content;
								tile.West    = copyTile.West;
								tile.North   = copyTile.North;
							}
						}
					}

				RefreshViewers();
			}
		}

		internal void Fill()
		{
			if (_mapBase != null)
			{
				_mapBase.MapChanged = true;

				var quadType = ViewerFormsManager.TopView.Control.QuadrantsPanel.SelectedQuadrant;

				var start = new Point(0, 0);
				var end   = new Point(0, 0);

				start.X = Math.Min(DragStart.X, DragEnd.X);
				start.Y = Math.Min(DragStart.Y, DragEnd.Y);
	
				end.X = Math.Max(DragStart.X, DragEnd.X);
				end.Y = Math.Max(DragStart.Y, DragEnd.Y);

				var tileView = ViewerFormsManager.TileView.Control;
				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						((XCMapTile)_mapBase[r, c])[quadType] = tileView.SelectedTile;

				RefreshViewers();
			}
		}

		private void RefreshViewers()
		{
			Refresh();

			ViewerFormsManager.TopView.Refresh();
			ViewerFormsManager.RouteView.Refresh();

			// TODO: refresh TopRouteView (both Top & Route panels) also.
		}

		internal void SetCursor(CursorSprite cursor)
		{
			_cursor = cursor;
			Refresh();
		}

		/// <summary>
		/// Scrolls the z-axis for MainView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) _mapBase.Up();
			else if (e.Delta > 0) _mapBase.Down();
		}

		private bool _isMouseDrag;

		/// <summary>
		/// Selects a tile and/or starts a drag-select procedure.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_mapBase != null)
			{
				var dragStart = ConvertCoordsDiamond(
												e.X, e.Y,
												_mapBase.Level);
				if (   dragStart.X > -1 && dragStart.X < MapBase.MapSize.Cols
					&& dragStart.Y > -1 && dragStart.Y < MapBase.MapSize.Rows)
				{
					FirstClick = true;

					_isMouseDrag = true;
					var dragEnd = ConvertCoordsDiamond(
													e.X, e.Y,
													_mapBase.Level);
					SetDrag(dragStart, dragEnd);

					_mapBase.Location = new MapLocation(
													DragStart.Y,
													DragStart.X,
													_mapBase.Level);
					Select();
					Refresh();
				}
			}
		}

		/// <summary>
		/// uh.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
			Refresh(); // is this used for anything like, at all.
		}

		/// <summary>
		/// Updates the drag-selection process.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_mapBase != null)
			{
				var pt = ConvertCoordsDiamond(
											e.X, e.Y,
											_mapBase.Level);

				if (pt.X != DragEnd.X || pt.Y != DragEnd.Y)
				{
					if (_isMouseDrag)
						SetDrag(DragStart, pt);

					Refresh(); // mouseover refresh for MainView.
				}
			}
		}

		/// <summary>
		/// Gets/Sets the drag-start point. See also 'GetDragStart()'.
		/// </summary>
		internal Point DragStart
		{
			get { return _dragStart; }
			private set
			{
				_dragStart = value;

				if      (_dragStart.Y < 0) _dragStart.Y = 0;
				else if (_dragStart.Y >= _mapBase.MapSize.Rows) _dragStart.Y = _mapBase.MapSize.Rows - 1;

				if      (_dragStart.X < 0) _dragStart.X = 0;
				else if (_dragStart.X >= _mapBase.MapSize.Cols) _dragStart.X = _mapBase.MapSize.Cols - 1;
			}
		}

		/// <summary>
		/// Gets/Sets the drag-end point. See also 'GetDragEnd()'.
		/// </summary>
		internal Point DragEnd
		{
			get { return _dragEnd; }
			private set
			{
				_dragEnd = value;

				if      (_dragEnd.Y < 0) _dragEnd.Y = 0;
				else if (_dragEnd.Y >= _mapBase.MapSize.Rows) _dragEnd.Y = _mapBase.MapSize.Rows - 1;

				if      (_dragEnd.X < 0) _dragEnd.X = 0;
				else if (_dragEnd.X >= _mapBase.MapSize.Cols) _dragEnd.X = _mapBase.MapSize.Cols - 1;
			}
		}

		/// <summary>
		/// Fires the drag-select event handler.
		/// </summary>
		/// <param name="dragStart"></param>
		/// <param name="dragEnd"></param>
		internal void SetDrag(Point dragStart, Point dragEnd)
		{
			if (DragStart != dragStart || DragEnd != dragEnd)
			{
				DragStart = dragStart;
				DragEnd   = dragEnd;
	
				if (MouseDragEvent != null)
					MouseDragEvent();

				// refreshes MainView in realtime iff a drag-select is happening in TopView.
				// But if the drag-select is done on MainView itself this is not needed to update the selection in realtime.
				Refresh();	// this refreshes MainView for a click on RouteView
			}				// unless the click is at location (0,0) *and* the
		}					// map has just been loaded ... NOTE: a click on
							// TopView will refresh the MainView selected-tile
							// by some other other way ... -> TopViewPanelParent.OnMouseUp().

		/// <summary>
		/// Gets the drag-start point. See also 'DragStart'.
		/// </summary>
		/// <returns></returns>
		private Point GetDragStart()
		{
			return new Point(
						Math.Max(Math.Min(DragStart.X, DragEnd.X), 0),	// TODO: these bounds should have been taken care of
						Math.Max(Math.Min(DragStart.Y, DragEnd.Y), 0));	// unless drag is being gotten right after instantiation ....
		}

		/// <summary>
		/// Gets the drag-end point. See also 'DragEnd'.
		/// </summary>
		/// <returns></returns>
		private Point GetDragEnd()
		{
			return new Point(
						Math.Max(DragStart.X, DragEnd.X), // wft: is dragend not dragend ...
						Math.Max(DragStart.Y, DragEnd.Y));
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetPanelSize()
		{
			if (_mapBase != null)
			{
				var size = GetPanelSizeRequired(Globals.PckImageScale);
				Width  = size.Width;
				Height = size.Height;
			}
		}

		/// <summary>
		/// Gets the required x/y size in pixels for the current MapBase as a
		/// lozenge.
		/// </summary>
		/// <param name="pckImageScale"></param>
		/// <returns></returns>
		internal Size GetPanelSizeRequired(double pckImageScale)
		{
			if (_mapBase != null)
			{
				int halfWidth  = (int)(HalfWidth  * pckImageScale);
				int halfHeight = (int)(HalfHeight * pckImageScale);

				_origin = new Point((_mapBase.MapSize.Rows - 1) * halfWidth, 0);

				int width  = (_mapBase.MapSize.Rows + _mapBase.MapSize.Cols) * halfWidth;
				int height =  _mapBase.MapSize.Levs * halfHeight * 3
						   + (_mapBase.MapSize.Rows + _mapBase.MapSize.Cols) * halfHeight;

				return new Size(width, height);
			}
			return Size.Empty;
		}

		/// <summary>
		/// Fires when a location is selected in MainView.
		/// </summary>
		/// <param name="e"></param>
		private void OnLocationSelected_Main(LocationSelectedEventArgs e)
		{
			var loc = e.Location;
			var dragStart = new Point(loc.Col, loc.Row);
			SetDrag(dragStart, DragEnd);

			XCMainWindow.Instance.StatusBarPrintPosition(loc.Col, loc.Row);
		}

		/// <summary>
		/// Fires when the map level changes in MainView.
		/// </summary>
		/// <param name="mapBase"></param>
		/// <param name="e"></param>
		private void OnLevelChanged_Main(XCMapBase mapBase, LevelChangedEventArgs e)
		{
			Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_mapBase != null)
			{
				var g = e.Graphics;

				var dragRect = new Rectangle(new Point(0, 0), new Size(0, 0));
				if (FirstClick)
				{
					var dragMin = new Point(
										Math.Min(DragStart.X, DragEnd.X),
										Math.Min(DragStart.Y, DragEnd.Y));
					var dragMax = new Point(
										Math.Max(DragStart.X, DragEnd.X),
										Math.Max(DragStart.Y, DragEnd.Y));

					dragRect = new Rectangle(dragMin, new Size(Point.Subtract(dragMax, new Size(dragMin))));
					dragRect.Width  += 1;
					dragRect.Height += 1;
				}

				int halfWidth  = (int)(HalfWidth  * Globals.PckImageScale);
				int halfHeight = (int)(HalfHeight * Globals.PckImageScale);

				for (int l = _mapBase.MapSize.Levs - 1; l != -1; --l)
				{
					if (_mapBase.Level <= l)
					{
						DrawGrid(l, g);

						for (int
								r = 0,
									startY = _origin.Y + (halfHeight * l * 3),
									startX = _origin.X;
								r != _mapBase.MapSize.Rows;
								++r,
									startY += halfHeight,
									startX -= halfWidth)
						{
							for (int
									c = 0,
										x = startX,
										y = startY;
									c != _mapBase.MapSize.Cols;
									++c,
										x += halfWidth,
										y += halfHeight)
							{
								var tileRect = new Rectangle(c, r, 1, 1);

								bool isClicked = (c == DragStart.X && r == DragStart.Y)
											  || (c == DragEnd.X   && r == DragEnd.Y);

								if (FirstClick && isClicked && _cursor != null)
									_cursor.DrawCursorBack(
														g,
														x, y,
//														false,
														_mapBase.Level == l);

								if (_mapBase.Level == l || _mapBase[r, c, l].DrawAbove)
								{
									var tile = (XCMapTile)_mapBase[r, c, l];
									if (_graySelection && FirstClick
										&& (isClicked || dragRect.IntersectsWith(tileRect)))
									{
										DrawTileGray(g, tile, x, y);
									}
									else
										DrawTile(g, tile, x, y);
								}

								if (FirstClick && isClicked && _cursor != null)
									_cursor.DrawCursorFront(
														g,
														x, y,
//														MainViewUnderlay.AniStep,
//														false,
														_mapBase.Level == l);
							}
						}
					}
				}

//				if (_drawSelectionBox) // always false.
				if (FirstClick && !_graySelection)
					DrawLozengeSelected(g, _mapBase.Level, dragRect);
			}
		}

		private void DrawGrid(int l, Graphics g)
		{
			if (_showGrid && _mapBase.Level == l)
			{
				int hWidth  = (int)(HalfWidth  * Globals.PckImageScale);
				int hHeight = (int)(HalfHeight * Globals.PckImageScale);

				int x = hWidth + _origin.X;
				int y = (_mapBase.Level + 1) * (hHeight * 3) + _origin.Y;

				int xMax = _mapBase.MapSize.Rows * hWidth;
				int yMax = _mapBase.MapSize.Rows * hHeight;

				var pt0 = new Point(x, y);
				var pt1 = new Point(
								x + _mapBase.MapSize.Cols * hWidth,
								y + _mapBase.MapSize.Cols * hHeight);
				var pt2 = new Point(
								x + (_mapBase.MapSize.Cols - _mapBase.MapSize.Rows) * hWidth,
								y + (_mapBase.MapSize.Rows + _mapBase.MapSize.Cols) * hHeight);
				var pt3 = new Point(x - xMax, yMax + y);

				_layerFill.Reset();
				_layerFill.AddLine(pt0, pt1);
				_layerFill.AddLine(pt1, pt2);
				_layerFill.AddLine(pt2, pt3);
				_layerFill.CloseFigure();

				g.FillPath(_brushLayer, _layerFill); // the grid-layer

				for (int i = 0; i <= _mapBase.MapSize.Rows; ++i)
					g.DrawLine(
							_penGrid,
							x - hWidth  * i,
							y + hHeight * i,
							x + (_mapBase.MapSize.Cols - i) * hWidth,
							y + (_mapBase.MapSize.Cols + i) * hHeight);

				for (int i = 0; i <= _mapBase.MapSize.Cols; ++i)
					g.DrawLine(
							_penGrid,
							x + hWidth  * i,
							y + hHeight * i,
							hWidth  * i - xMax + x,
							hHeight * i + yMax + y);
			}
		}

		private static void DrawTile(Graphics g, XCMapTile tile, int x, int y)
		{
			var topView = ViewerFormsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}
		}

		private static void DrawTileGray(Graphics g, XCMapTile tile, int x, int y)
		{
			var topView = ViewerFormsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}
		}

		private static void DrawTileImage(Graphics g, int x, int y, TileBase tile, Image image)
		{
			g.DrawImage(
					image,
					x,
					(int)(y - tile.Record.TileOffset * Globals.PckImageScale),
					(int)(image.Width  * Globals.PckImageScale),
					(int)(image.Height * Globals.PckImageScale));
		}

		private void DrawLozengeSelected(Graphics g, int l, Rectangle dragRect)
		{
			var hWidth = (int)(HalfWidth * Globals.PckImageScale);

			var top    = ConvertCoordsRect(new Point(dragRect.X,     dragRect.Y), l + 1);
			var right  = ConvertCoordsRect(new Point(dragRect.Right, dragRect.Y), l + 1);
			var bottom = ConvertCoordsRect(new Point(dragRect.Right, dragRect.Bottom), l + 1);
			var left   = ConvertCoordsRect(new Point(dragRect.Left,  dragRect.Bottom), l + 1);

			top.X    += hWidth;
			right.X  += hWidth;
			bottom.X += hWidth;
			left.X   += hWidth;

			var pen = new Pen(Color.FromArgb(70, Color.Red));
			pen.Width = 3;

			g.DrawLine(pen, top,    right);
			g.DrawLine(pen, right,  bottom);
			g.DrawLine(pen, bottom, left);
			g.DrawLine(pen, left,   top);
		}

		/// <summary>
		/// Converts a point from screen coordinates to tile coordinates.
		/// </summary>
		/// <param name="ptX"></param>
		/// <param name="ptY"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		private Point ConvertCoordsDiamond(int ptX, int ptY, int level)
		{
			// 16 is half the width of the diamond
			// 24 is the distance from the top of the diamond to the very top of the image

			double halfWidth  = HalfWidth  * Globals.PckImageScale;
			double halfHeight = HalfHeight * Globals.PckImageScale;

			double x = ptX - _origin.X - halfWidth;
			double y = ptY - _origin.Y - halfHeight * 3 * (level + 1);

			double x1 = x / (halfWidth  * 2)
					  + y / (halfHeight * 2);
			double y1 = (y * 2 - x) / (halfWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}

		private Point ConvertCoordsRect(Point pt, int level)
		{
			int hWidth  = (int)(HalfWidth  * Globals.PckImageScale);
			int hHeight = (int)(HalfHeight * Globals.PckImageScale);
			int hOffset = hHeight * level * 3;
			return new Point(
						_origin.X + (pt.X - pt.Y) * hWidth,
						_origin.Y + (pt.X + pt.Y) * hHeight + hOffset);
		}
	}
}
