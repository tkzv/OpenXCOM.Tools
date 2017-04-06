using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;


namespace MapView // NOTE: namespace conflict w/ .NET itself
{
	public delegate void MouseDragEventHandler();


	internal sealed class MainView
		:
			Panel
	{
		public event MouseDragEventHandler MouseDragEvent;


		private IMapBase _baseMap;
		public IMapBase Map
		{
			get { return _baseMap; }
			set
			{
				if (_baseMap != null)
				{
					_baseMap.HeightChanged -= OnHeightChanged;
					_baseMap.SelectedTileChanged -= OnTileChange;
				}

				if ((_baseMap = value) != null)
				{
					_baseMap.HeightChanged += OnHeightChanged;
					_baseMap.SelectedTileChanged += OnTileChange;

					SetupMapSize();

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

		private GraphicsPath _gridUnder;
		private Brush _brushTrans;

		private Color _colorGrid;
		public Color GridColor
		{
			get { return _colorGrid; }
			set
			{
				_colorGrid = value;
				_brushTrans = new SolidBrush(value);
				Refresh();
			}
		}

		private Pen _penDash;
		public Color GridLineColor
		{
			get { return _penDash.Color; }
			set
			{
				_penDash.Color = value;
				Refresh();
			}
		}
		public int GridLineWidth
		{
			get { return (int)_penDash.Width; }
			set
			{
				_penDash.Width = value;
				Refresh();
			}
		}


		private bool _graySelection = true;
		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
		"CA1811:AvoidUncalledPrivateCode",
		Justification = "Because the setter is called dynamically w/ Reflection or other: it is used.")]
		public bool GraySelection
		{
			get { return _graySelection; }
			set
			{
				_graySelection = value;
				Refresh();
			}
		}

		private bool _showGrid = true;
		public bool ShowGrid
		{
			get { return _showGrid; }
			set
			{
				_showGrid = value;
				Refresh();
			}
		}

		/// <summary>
		/// If true draws a translucent red box around selected tiles ->
		/// superceded by using GraySelection (false) property.
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


		public MainView()
		{
//			_baseMap = null;

			SetStyle(
					ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,
					true);

			_colorGrid  = Color.FromArgb(175, 69, 100, 129);
			_brushTrans = new SolidBrush(_colorGrid);
			_penDash    = new Pen(Brushes.Black, 1);
		}


		public void ClearSelection()
		{
			if (_baseMap != null)
			{
				_baseMap.MapChanged = true;

				var start = GetDragStart();
				var end   = GetDragEnd();
	
				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						_baseMap[r, c] = XCMapTile.BlankTile;

//				Refresh(); // handled for all viewers by EditButtonsFactory.
			}
		}

		private MapTileBase[,] _copied;

		public void Copy()
		{
			if (_baseMap != null)
			{
				var start = GetDragStart();
				var end   = GetDragEnd();

				_copied = new MapTileBase[end.Y - start.Y + 1, end.X - start.X + 1];

				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						_copied[r - start.Y, c - start.X] = _baseMap[r, c];
			}
		}

		public void Paste()
		{
			if (_baseMap != null && _copied != null)
			{
				_baseMap.MapChanged = true;

				for (int
						r = _dragStart.Y;
						r != _baseMap.MapSize.Rows && (r - _dragStart.Y) < _copied.GetLength(0);
						++r)
					for (int
							c = _dragStart.X;
							c != _baseMap.MapSize.Cols && (c - _dragStart.X) < _copied.GetLength(1);
							++c)
					{
						var tile = _baseMap[r, c] as XCMapTile;
						if (tile != null)
						{
							var copyTile = _copied[r - _dragStart.Y, c - _dragStart.X] as XCMapTile;
							if (copyTile != null)
							{
								tile.Ground  = copyTile.Ground;
								tile.Content = copyTile.Content;
								tile.West    = copyTile.West;
								tile.North   = copyTile.North;
							}
						}
					}

//				Refresh(); // handled for all viewers by EditButtonsFactory.
			}
		}

		public void Fill()
		{
			if (_baseMap != null)
			{
				_baseMap.MapChanged = true;

				var quadType = MainWindowsManager.TopView.Control.QuadrantsPanel.SelectedQuadrant;

				var start = new Point(0, 0);
				var end   = new Point(0, 0);

				start.X = Math.Min(DragStart.X, DragEnd.X);
				start.Y = Math.Min(DragStart.Y, DragEnd.Y);
	
				end.X = Math.Max(DragStart.X, DragEnd.X);
				end.Y = Math.Max(DragStart.Y, DragEnd.Y);

				var tileView = MainWindowsManager.TileView.Control;
				for (int c = start.X; c <= end.X; ++c)
					for (int r = start.Y; r <= end.Y; ++r)
						((XCMapTile)_baseMap[r, c])[quadType] = tileView.SelectedTile;

//				MainViewPanel.Instance.Refresh();
//				Refresh(); // handled for all viewers by EditButtonsFactory.
			}
		}

		internal void SetCursor(CursorSprite cursor)
		{
			_cursor = cursor;
			Refresh();
		}

		/// <summary>
		/// Flag that tells TopViewPanelBase.DrawSelectedLozenge that it's okay
		/// to draw a lozenge for a selected tile; ie, that an initial tile has
		/// actually been selected. This prevents an off-border lozenge from
		/// being drawn right after TopView initially appears.
		/// </summary>
		internal bool _isSelectedTileValid;
		internal bool IsSelectedTileValid
		{
			get { return _isSelectedTileValid; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_baseMap != null)
			{
				_isSelectedTileValid = true; // klugdge.

				var dragStart = ConvertCoordsDiamond(
												e.X, e.Y,
												_baseMap.CurrentHeight);
				var dragEnd   = ConvertCoordsDiamond(
												e.X, e.Y,
												_baseMap.CurrentHeight);
				SetDrag(dragStart, dragEnd);

				_baseMap.SelectedTile = new MapLocation(
													_dragEnd.Y, _dragEnd.X,
													_baseMap.CurrentHeight);

				Select();
				Refresh();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Refresh(); // is this used for anything like, at all.
		}

		/// <summary>
		/// Scrolls the z-axis for MapView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) _baseMap.Up();
			else if (e.Delta > 0) _baseMap.Down();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_baseMap != null)
			{
				var current = ConvertCoordsDiamond(
												e.X, e.Y,
												_baseMap.CurrentHeight);

				if (current.X != _dragEnd.X || current.Y != _dragEnd.Y)
				{
					if (e.Button != MouseButtons.None)
						SetDrag(_dragStart, current);

					Refresh(); // mouseover refresh for MapView.
				}
			}
		}

		public Point DragStart
		{
			get { return _dragStart; }
			private set
			{
				_dragStart = value;

				if      (_dragStart.Y < 0) _dragStart.Y = 0;
				else if (_dragStart.Y >= _baseMap.MapSize.Rows) _dragStart.Y = _baseMap.MapSize.Rows - 1;

				if      (_dragStart.X < 0) _dragStart.X = 0;
				else if (_dragStart.X >= _baseMap.MapSize.Cols) _dragStart.X = _baseMap.MapSize.Cols - 1;
			}
		}

		public Point DragEnd
		{
			get { return _dragEnd; }
			private set
			{
				_dragEnd = value;

				if      (_dragEnd.Y < 0) _dragEnd.Y = 0;
				else if (_dragEnd.Y >= _baseMap.MapSize.Rows) _dragEnd.Y = _baseMap.MapSize.Rows - 1;

				if      (_dragEnd.X < 0) _dragEnd.X = 0;
				else if (_dragEnd.X >= _baseMap.MapSize.Cols) _dragEnd.X = _baseMap.MapSize.Cols - 1;
			}
		}

		public void SetDrag(Point dragStart, Point dragEnd)
		{
			if (_dragStart != dragStart || _dragEnd != dragEnd)
			{
				DragStart = dragStart;
				DragEnd   = dragEnd;
	
				if (MouseDragEvent != null)
					MouseDragEvent();

				Refresh();
			}
		}

		private Point GetDragStart()
		{
			var start = new Point();
			start.X = Math.Max(Math.Min(_dragStart.X, _dragEnd.X), 0); // TODO: these bounds should have been taken care of
			start.Y = Math.Max(Math.Min(_dragStart.Y, _dragEnd.Y), 0); // unless drag is being gotten right after instantiation ....
			return start;
		}

		private Point GetDragEnd()
		{
			var end = new Point();
			end.X = Math.Max(_dragStart.X, _dragEnd.X); // wft: is dragend not dragend ...
			end.Y = Math.Max(_dragStart.Y, _dragEnd.Y);
			return end;
		}

		public void SetupMapSize()
		{
			if (_baseMap != null)
			{
				var size = GetMapSize(Globals.PckImageScale);
				Width    = size.Width;
				Height   = size.Height;
			}
		}

		public Size GetMapSize(double pckImageScale)
		{
			if (_baseMap != null)
			{
				int halfWidth  = (int)(HalfWidth  * pckImageScale);
				int halfHeight = (int)(HalfHeight * pckImageScale);

				_origin = new Point((_baseMap.MapSize.Rows - 1) * halfWidth, 0);

				int width  = (_baseMap.MapSize.Rows + _baseMap.MapSize.Cols) * halfWidth;
				int height =  _baseMap.MapSize.Height * halfHeight * 3
						   + (_baseMap.MapSize.Rows + _baseMap.MapSize.Cols) * halfHeight;

				return new Size(width, height);
			}
			return Size.Empty;
		}

		private void OnTileChange(IMapBase baseMap, SelectedTileChangedEventArgs e)
		{
			var loc = e.MapPosition;
			var start = new Point(loc.Col, loc.Row);
			SetDrag(start, _dragEnd);

			XCMainWindow.Instance.StatusBarPrintPosition(loc.Col, loc.Row);
		}

		private void OnHeightChanged(IMapBase baseMap, HeightChangedEventArgs e)
		{
			Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_baseMap != null)
			{
				var g = e.Graphics;

				var dragMin = new Point(
									Math.Min(_dragStart.X, _dragEnd.X),
									Math.Min(_dragStart.Y, _dragEnd.Y));
				var dragMax = new Point(
									Math.Max(_dragStart.X, _dragEnd.X),
									Math.Max(_dragStart.Y, _dragEnd.Y));

				var dragRect = new Rectangle(dragMin, new Size(Point.Subtract(dragMax, new Size(dragMin))));
				dragRect.Width  += 1;
				dragRect.Height += 1;

				int halfWidth  = (int)(HalfWidth  * Globals.PckImageScale);
				int halfHeight = (int)(HalfHeight * Globals.PckImageScale);

				for (int h = _baseMap.MapSize.Height - 1; h > -1; --h)
				{
					if (_baseMap.CurrentHeight <= h)
					{
						DrawGrid(h, g);

						for (int
								r = 0, startY = _origin.Y + (halfHeight * h * 3), startX = _origin.X;
								r != _baseMap.MapSize.Rows;
								++r, startY += halfHeight, startX -= halfWidth)
						{
							for (int
									c = 0, x = startX, y = startY;
									c != _baseMap.MapSize.Cols;
									++c, x += halfWidth, y += halfHeight)
							{
								var tileRect = new Rectangle(c, r, 1, 1);

								bool isClicked = (r == _dragStart.Y && c == _dragStart.X)
											  || (r == _dragEnd.Y   && c == _dragEnd.X);

								if (isClicked)
									DrawCursor(g, x, y, h);

								if (_baseMap.CurrentHeight == h || _baseMap[r, c, h].DrawAbove)
								{
									var tile = (XCMapTile)_baseMap[r, c, h];
									if (_graySelection
										&& (isClicked || dragRect.IntersectsWith(tileRect)))
									{
										DrawTileGray(g, tile, x, y);
									}
									else
										DrawTile(g, tile, x, y);
								}

								if (isClicked && _cursor != null)
									_cursor.DrawLow(
												g,
												x, y,
												MainViewPanel.Current,
												false,
												_baseMap.CurrentHeight == h);
							}
						}
					}
				}

//				if (_drawSelectionBox) // always false.
				if (!_graySelection)
					DrawLozengeSelected(g, _baseMap.CurrentHeight, dragRect);
			}
		}

		private void DrawCursor(Graphics g, int x, int y, int h)
		{
			if (_cursor != null)
				_cursor.DrawHigh(
							g,
							x, y,
							false,
							_baseMap.CurrentHeight == h);
		}

		private void DrawGrid(int h, Graphics g)
		{
			if (_showGrid && _baseMap.CurrentHeight == h)
			{
				var hWidth  = (int)(HalfWidth  * Globals.PckImageScale);
				var hHeight = (int)(HalfHeight * Globals.PckImageScale);

				var x = hWidth + _origin.X;
				var y = (_baseMap.CurrentHeight + 1) * (hHeight * 3) + _origin.Y;

				var xMax = _baseMap.MapSize.Rows * hWidth;
				var yMax = _baseMap.MapSize.Rows * hHeight;

				_gridUnder = new GraphicsPath();

				var pt0 = new Point(x, y);
				var pt1 = new Point(
								x + _baseMap.MapSize.Cols * hWidth,
								y + _baseMap.MapSize.Cols * hHeight);
				var pt2 = new Point(
								x + (_baseMap.MapSize.Cols - _baseMap.MapSize.Rows) * hWidth,
								y + (_baseMap.MapSize.Rows + _baseMap.MapSize.Cols) * hHeight);
				var pt3 = new Point(x - xMax, yMax + y);

				_gridUnder.AddLine(pt0, pt1);
				_gridUnder.AddLine(pt1, pt2);
				_gridUnder.AddLine(pt2, pt3);
				_gridUnder.CloseFigure();

				g.FillPath(_brushTrans, _gridUnder);

				for (var i = 0; i <= _baseMap.MapSize.Rows; ++i)
					g.DrawLine(
							_penDash,
							x - hWidth  * i,
							y + hHeight * i,
							x + (_baseMap.MapSize.Cols - i) * hWidth,
							y + (_baseMap.MapSize.Cols + i) * hHeight);

				for (int i = 0; i <= _baseMap.MapSize.Cols; ++i)
					g.DrawLine(
							_penDash,
							x + hWidth  * i,
							y + hHeight * i,
							hWidth  * i - xMax + x,
							hHeight * i + yMax + y);
			}
		}

		private static void DrawTile(Graphics g, XCMapTile tile, int x, int y)
		{
			var topView = MainWindowsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Image);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Image);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Image);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Image);
			}
		}

		private static void DrawTileGray(Graphics g, XCMapTile tile, int x, int y)
		{
			var topView = MainWindowsManager.TopView.Control;

			if (topView.GroundVisible)
			{
				var baseTile = tile.Ground;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Gray);
			}

			if (topView.WestVisible)
			{
				var baseTile = tile.West;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Gray);
			}

			if (topView.NorthVisible)
			{
				var baseTile = tile.North;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Gray);
			}

			if (topView.ContentVisible)
			{
				var baseTile = tile.Content;
				if (baseTile != null)
					DrawTileImage(g, x, y, baseTile, baseTile[MainViewPanel.Current].Gray);
			}
		}

		private static void DrawTileImage(Graphics g, int x, int y, TileBase tile, Image image)
		{
			g.DrawImage(
					image,
					x,
					(int)(y - tile.Info.TileOffset * Globals.PckImageScale),
					(int)(image.Width  * Globals.PckImageScale),
					(int)(image.Height * Globals.PckImageScale));
		}

		private void DrawLozengeSelected(Graphics g, int h, Rectangle dragRect)
		{
			var hWidth = (int)(HalfWidth * Globals.PckImageScale);

			var top    = ConvertCoordsRect(new Point(dragRect.X,     dragRect.Y), h + 1);
			var right  = ConvertCoordsRect(new Point(dragRect.Right, dragRect.Y), h + 1);
			var bottom = ConvertCoordsRect(new Point(dragRect.Right, dragRect.Bottom), h + 1);
			var left   = ConvertCoordsRect(new Point(dragRect.Left,  dragRect.Bottom), h + 1);

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

		private Point ConvertCoordsRect(Point pt, int height)
		{
			int hWidth  = (int)(HalfWidth  * Globals.PckImageScale);
			int hHeight = (int)(HalfHeight * Globals.PckImageScale);
			int hOffset = hHeight * height * 3;
			return new Point(
						_origin.X + (pt.X - pt.Y) * hWidth,
						_origin.Y + (pt.X + pt.Y) * hHeight + hOffset);
		}
	}
}
