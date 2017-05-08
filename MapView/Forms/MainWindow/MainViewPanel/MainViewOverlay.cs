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
			Panel // god I hate these double-panels!!!! cf. MainViewUnderlay
	{
		internal event MouseDragEventHandler MouseDragEvent;


		#region Fields & Properties
		private MainViewUnderlay _mainViewUnderlay;
		internal void SetMainViewUnderlay(MainViewUnderlay underlay)
		{
			_mainViewUnderlay = underlay;
		}

		internal XCMapBase MapBase
		{ get; set; }

		private Point _origin = new Point(0, 0);
		internal Point Origin
		{
			get { return _origin; }
			set { _origin = value; }
		}

		internal const int HalfWidthConst  = 16;
		internal const int HalfHeightConst =  8;

		internal int HalfWidth
		{ private get; set; }

		internal int HalfHeight
		{ private get; set; }


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

					ViewerFormsManager.TopView.Control.TopViewPanel.PathSelectedLozenge();
				}
			}
		}

		private int _col; // these are used only to print the clicked location info.
		private int _row;
		private int _lev;


		private CursorSprite _cursor;

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
		#endregion


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


		internal void SetCursor(CursorSprite cursor)
		{
			_cursor = cursor;
			Refresh();
		}

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
			if (MapBase != null)
			{
				MapBase.MapChanged = true;

				var start = GetDragStart();
				var end   = GetDragEnd();
	
				for (int col = start.X; col <= end.X; ++col)
					for (int row = start.Y; row <= end.Y; ++row)
						MapBase[row, col] = XCMapTile.BlankTile;

				RefreshViewers();
			}
		}


		private MapTileBase[,] _copied;

		internal void Copy()
		{
			if (MapBase != null)
			{
				var start = GetDragStart();
				var end   = GetDragEnd();

				_copied = new MapTileBase[end.Y - start.Y + 1, end.X - start.X + 1];

				for (int col = start.X; col <= end.X; ++col)
					for (int row = start.Y; row <= end.Y; ++row)
						_copied[row - start.Y, col - start.X] = MapBase[row, col];
			}
		}

		internal void Paste()
		{
			if (MapBase != null && _copied != null)
			{
				MapBase.MapChanged = true;

				XCMapTile tile     = null;
				XCMapTile tileCopy = null;
				for (int
						row = DragStart.Y;
						row != MapBase.MapSize.Rows && (row - DragStart.Y) < _copied.GetLength(0);
						++row)
					for (int
							col = DragStart.X;
							col != MapBase.MapSize.Cols && (col - DragStart.X) < _copied.GetLength(1);
							++col)
					{
						if ((tile = MapBase[row, col] as XCMapTile) != null)
						{
							if ((tileCopy = _copied[row - DragStart.Y, col - DragStart.X] as XCMapTile) != null)
							{
								tile.Ground  = tileCopy.Ground;
								tile.Content = tileCopy.Content;
								tile.West    = tileCopy.West;
								tile.North   = tileCopy.North;
							}
						}
					}

				RefreshViewers();
			}
		}

		internal void FillSelectedTiles()
		{
			if (MapBase != null)
			{
				MapBase.MapChanged = true;

				var quadType = ViewerFormsManager.TopView.Control.QuadrantsPanel.SelectedQuadrant;

				var start = new Point(0, 0);
				var end   = new Point(0, 0);

				start.X = Math.Min(DragStart.X, DragEnd.X);
				start.Y = Math.Min(DragStart.Y, DragEnd.Y);
	
				end.X = Math.Max(DragStart.X, DragEnd.X);
				end.Y = Math.Max(DragStart.Y, DragEnd.Y);

				var tileView = ViewerFormsManager.TileView.Control;
				for (int col = start.X; col <= end.X; ++col)
				for (int row = start.Y; row <= end.Y; ++row)
				{
					((XCMapTile)MapBase[row, col])[quadType] = tileView.SelectedTile;
				}

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

		/// <summary>
		/// Scrolls the z-axis for MainView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) MapBase.LevelUp();
			else if (e.Delta > 0) MapBase.LevelDown();
		}


		private bool _isMouseDrag;

		/// <summary>
		/// Selects a tile and/or starts a drag-select procedure.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (MapBase != null)
			{
				var dragStart = GetTileLocation(e.X, e.Y);
				if (   dragStart.X > -1 && dragStart.X < MapBase.MapSize.Cols
					&& dragStart.Y > -1 && dragStart.Y < MapBase.MapSize.Rows)
				{
					FirstClick = true;

					_isMouseDrag = true;
					FireMouseDrag(dragStart, dragStart);

					MapBase.Location = new MapLocation(
													dragStart.Y,
													dragStart.X,
													MapBase.Level);
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
		}

		/// <summary>
		/// Updates the drag-selection process.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (MapBase != null)
			{
				var loc = GetTileLocation(e.X, e.Y);
				if (loc.X != DragEnd.X || loc.Y != DragEnd.Y)
				{
					if (_isMouseDrag)
						FireMouseDrag(DragStart, loc);

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
				else if (_dragStart.Y >= MapBase.MapSize.Rows) _dragStart.Y = MapBase.MapSize.Rows - 1;

				if      (_dragStart.X < 0) _dragStart.X = 0;
				else if (_dragStart.X >= MapBase.MapSize.Cols) _dragStart.X = MapBase.MapSize.Cols - 1;
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
				else if (_dragEnd.Y >= MapBase.MapSize.Rows) _dragEnd.Y = MapBase.MapSize.Rows - 1;

				if      (_dragEnd.X < 0) _dragEnd.X = 0;
				else if (_dragEnd.X >= MapBase.MapSize.Cols) _dragEnd.X = MapBase.MapSize.Cols - 1;
			}
		}

		/// <summary>
		/// Fires the drag-select event handler.
		/// </summary>
		/// <param name="dragStart"></param>
		/// <param name="dragEnd"></param>
		internal void FireMouseDrag(Point dragStart, Point dragEnd)
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
		/// Fires when a location is selected in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLocationSelected_Main(LocationSelectedEventArgs args)
		{
			_col = args.Location.Col;
			_row = args.Location.Row;
			_lev = args.Location.Lev;

			FireMouseDrag(new Point(_col, _row), DragEnd);

			XCMainWindow.Instance.StatusBarPrintPosition(
													_col,
													_row,
													MapBase.MapSize.Levs - _lev);
		}

		/// <summary>
		/// Fires when the map level changes in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLevelChanged_Main(LevelChangedEventArgs args)
		{
			_lev = args.Level;
			XCMainWindow.Instance.StatusBarPrintPosition(
													_col,
													_row,
													MapBase.MapSize.Levs - _lev);
			Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// indicate the reserved width for the scrollbars.
			var pen = new Pen(SystemColors.ControlLight, 1);
			int xBar = _mainViewUnderlay.GetVBarWidth();
			int yBar = _mainViewUnderlay.GetHBarHeight();
			e.Graphics.DrawLine(
							pen,
							Right - xBar, 0,
							Right - xBar, ClientSize.Height - yBar);
			e.Graphics.DrawLine(
							pen,
							0,                       Bottom - yBar,
							ClientSize.Width - xBar, Bottom - yBar);


			if (MapBase != null)
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

				for (int
					lev = MapBase.MapSize.Levs - 1;
					lev >= MapBase.Level && lev != -1;
					--lev)
				{
					if (_showGrid && lev == MapBase.Level)
						DrawGrid(lev, g);

					for (int
							row = 0,
								startY = Origin.Y + (HalfHeight * lev * 3),
								startX = Origin.X;
							row != MapBase.MapSize.Rows;
							++row,
								startY += HalfHeight,
								startX -= HalfWidth)
					{
						for (int
								col = 0,
									x = startX,
									y = startY;
								col != MapBase.MapSize.Cols;
								++col,
									x += HalfWidth,
									y += HalfHeight)
						{
							var tileRect = new Rectangle(col, row, 1, 1);

							bool isClicked = (col == DragStart.X && row == DragStart.Y)
										  || (col == DragEnd.X   && row == DragEnd.Y);

							if (FirstClick && isClicked && _cursor != null)
								_cursor.DrawCursorBack(
													g,
													x + 1, y - 3,
//													false,
													lev == MapBase.Level);

							if (lev == MapBase.Level || MapBase[row, col, lev].DrawAbove)
							{
								var tile = (XCMapTile)MapBase[row, col, lev];
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
													x + 1, y - 3,
//													MainViewUnderlay.AniStep,
//													false,
													lev == MapBase.Level);
						}
					}
				}

//				if (_drawSelectionBox) // always false.
				if (FirstClick && !_graySelection)
					DrawSelectedLozenge(g, MapBase.Level, dragRect);
			}
		}

		private void DrawGrid(int lev, Graphics g)
		{
			int x = Origin.X + HalfWidth;
			int y = Origin.Y + HalfHeight * (MapBase.Level + 1) * 3;

			int x1 = MapBase.MapSize.Rows * HalfWidth;
			int y1 = MapBase.MapSize.Rows * HalfHeight;

			var pt0 = new Point(x, y);
			var pt1 = new Point(
							x + MapBase.MapSize.Cols * HalfWidth,
							y + MapBase.MapSize.Cols * HalfHeight);
			var pt2 = new Point(
							x + (MapBase.MapSize.Cols - MapBase.MapSize.Rows) * HalfWidth,
							y + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * HalfHeight);
			var pt3 = new Point(x - x1, y + y1);

			_layerFill.Reset();
			_layerFill.AddLine(pt0, pt1);
			_layerFill.AddLine(pt1, pt2);
			_layerFill.AddLine(pt2, pt3);
			_layerFill.CloseFigure();

			g.FillPath(_brushLayer, _layerFill); // the grid-sheet

			// draw the grid-lines ->
			for (int i = 0; i <= MapBase.MapSize.Rows; ++i)
				g.DrawLine(
						_penGrid,
						x - HalfWidth  * i,
						y + HalfHeight * i,
						x + (MapBase.MapSize.Cols - i) * HalfWidth,
						y + (MapBase.MapSize.Cols + i) * HalfHeight);

			for (int i = 0; i <= MapBase.MapSize.Cols; ++i)
				g.DrawLine(
						_penGrid,
						x + HalfWidth  * i,
						y + HalfHeight * i,
						x - x1 + HalfWidth  * i,
						y + y1 + HalfHeight * i);
		}

		private static void DrawTile(
				Graphics g,
				XCMapTile tile,
				int x, int y)
		{
			var topView = ViewerFormsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Image);
			}
		}

		private static void DrawTileGray(
				Graphics g,
				XCMapTile tile,
				int x, int y)
		{
			var topView = ViewerFormsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileSprite(g, x, y, baseTile, baseTile[MainViewUnderlay.AniStep].Gray);
			}
		}

		private static void DrawTileSprite(
				Graphics g,
				int x, int y,
				TileBase tile,
				Image sprite)
		{
			g.DrawImage(
					sprite,
					x,
					(int)(y - tile.Record.TileOffset * Globals.Scale),
					(int)(sprite.Width  * Globals.Scale),
					(int)(sprite.Height * Globals.Scale));
		}

		private void DrawSelectedLozenge(
				Graphics g,
				int level,
				Rectangle dragRect)
		{
			var top    = GetScreenCoordinates(new Point(dragRect.X,     dragRect.Y),      level);
			var right  = GetScreenCoordinates(new Point(dragRect.Right, dragRect.Y),      level);
			var bottom = GetScreenCoordinates(new Point(dragRect.Right, dragRect.Bottom), level);
			var left   = GetScreenCoordinates(new Point(dragRect.Left,  dragRect.Bottom), level);

			top.X    += HalfWidth;
			right.X  += HalfWidth;
			bottom.X += HalfWidth;
			left.X   += HalfWidth;

			float penWidth = Globals.Scale < 1.5 ? 2
												 : 3;
			var pen = new Pen(Color.FromArgb(60, Color.Red), penWidth);

			g.DrawLine(pen, top,    right);
			g.DrawLine(pen, right,  bottom);
			g.DrawLine(pen, bottom, left);
			g.DrawLine(pen, left,   top);
		}

		/// <summary>
		/// Converts a point from tile-location to screen-coordinates.
		/// </summary>
		/// <param name="point">the x/y-position of a tile</param>
		/// <param name="level">the current level</param>
		/// <returns></returns>
		private Point GetScreenCoordinates(Point point, int level)
		{
			int verticalOffset = HalfHeight * (level + 1) * 3;
			return new Point(
							Origin.X + (point.X - point.Y) * HalfWidth,
							Origin.Y + (point.X + point.Y) * HalfHeight + verticalOffset);
		}

		/// <summary>
		/// Converts a position from screen-coordinates to tile-location.
		/// </summary>
		/// <param name="x">the x-position of the mouse cursor</param>
		/// <param name="y">the y-position of the mouse cursor</param>
		/// <returns></returns>
		private Point GetTileLocation(int x, int y)
		{
			double halfWidth  = (double)HalfWidth;
			double halfHeight = (double)HalfHeight;

			double verticalOffset = (MapBase.Level + 1) * 3;

			double xd = Math.Floor((double)x - (double)Origin.X - halfWidth);					// x=0 is the axis from the top to the bottom of the map-lozenge.
			double yd = Math.Floor((double)y - (double)Origin.Y - halfHeight * verticalOffset);	// y=0 is measured from the top of the map-lozenge downward.

			double x1 = xd / (halfWidth  * 2)
					  + yd / (halfHeight * 2);
			double y1 = (yd * 2 - xd) / (halfWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}
	}
}
