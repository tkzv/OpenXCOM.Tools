using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

					// clear the selected tile on TopView
//					ViewerFormsManager.TopView.Control.TopViewPanel.PathSelectedLozenge();
//					ViewerFormsManager.TopView.Control.TopViewPanel.Refresh();

					// clear the selected tile for RouteView also
					ViewerFormsManager.RouteView.Control.RoutePanel.SetSelectedTile(-1, -1); // NOTE: does not understand 'dragStart/dragEnd' like TopView does.
//					ViewerFormsManager.RouteView.Control.RoutePanel.PathSelectedLozenge();
				}
			}
		}

		private int _col; // these are used only to print the clicked location info.
		private int _row;
		private int _lev;


		internal CursorSprite Cuboid
		{ private get; set; }


		private GraphicsPath _layerFill = new GraphicsPath();

		private Graphics _graphics;
		private ImageAttributes _spriteAttributes = new ImageAttributes();

		private Brush _brushLayer;

		private Color _colorLayer = Color.MediumVioletRed;							// initial color for the grid-layer Option
		public Color GridLayerColor													// <- public for Reflection.
		{
			get { return _colorLayer; }
			set
			{
				_colorLayer = value;
				_brushLayer = new SolidBrush(Color.FromArgb(GridLayerOpacity, _colorLayer));
				Refresh();
			}
		}

		private int _opacity = 200;													// initial opacity for the grid-layer Option
		public int GridLayerOpacity													// <- public for Reflection.
		{
			get { return _opacity; }
			set
			{
				_opacity = value.Clamp(0, 255);
				_brushLayer = new SolidBrush(Color.FromArgb(_opacity, ((SolidBrush)_brushLayer).Color));
				Refresh();
			}
		}

		private Pen _penGrid = new Pen(Brushes.Black, 1);							// initial pen for grid-lines Option
		public Color GridLineColor													// <- public for Reflection.
		{
			get { return _penGrid.Color; }
			set
			{
				_penGrid.Color = value;
				Refresh();
			}
		}
		public int GridLineWidth													// <- public for Reflection.
		{
			get { return (int)_penGrid.Width; }
			set
			{
				_penGrid.Width = value;
				Refresh();
			}
		}

		private bool _showGrid = true;												// initial val for show-grid Option
		public bool ShowGrid														// <- public for Reflection.
		{
			get { return _showGrid; }
			set
			{
				_showGrid = value;
				Refresh();
			}
		}

		private bool _graySelection = true;											// initial val for gray-selection Option
		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
		"CA1811:AvoidUncalledPrivateCode",
		Justification = "Because the setter is called dynamically w/ Reflection" +
		"or other: not only is it used it needs to be public.")]
		public bool GraySelection													// <- public for Reflection.
		{
			get { return _graySelection; }
			set
			{
				_graySelection = value;
				Refresh();
			}
		}


		private bool _spriteShadeEnabled = true;

		// NOTE: Options don't like floats afaict, hence this workaround w/
		// 'SpriteDarkness' and 'SpriteDarknessLocal' ->
		private int _spriteDarkness;												// 0 = initial val for sprite darkness Option
		public int SpriteDarkness													// <- public for Reflection.
		{
			get { return _spriteDarkness; }
			set
			{
				_spriteDarkness = value;

				if (_spriteDarkness > 9 && _spriteDarkness < 101)
				{
					_spriteShadeEnabled = true;
					SpriteDarknessLocal = _spriteDarkness * 0.03f;
				}
				else
					_spriteShadeEnabled = false;

				Refresh();
			}
		}
		private float _spriteDarknessLocal = 1.0f;									// initial val for local sprite darkness
		private float SpriteDarknessLocal
		{
			get { return _spriteDarknessLocal; }
			set { _spriteDarknessLocal = value; }
		}

		// NOTE: Options don't like enums afaict, hence this workaround w/
		// 'Interpolation' and 'InterpolationLocal' ->
		private int _interpolation;													// 0 = initial val for interpolation Option
		public int Interpolation													// <- public for Reflection.
		{
			get { return _interpolation; }
			set
			{
				_interpolation = value.Clamp(0, 7);
				_interpolationLocal = (InterpolationMode)_interpolation;
				Refresh();
			}
		}
		private InterpolationMode _interpolationLocal = InterpolationMode.Default;	// initial val for local interpolation
		private InterpolationMode InterpolationLocal
		{
			get { return _interpolationLocal; }
			set { _interpolationLocal = value; }
		}

		/// <summary>
		/// If true draws a translucent red box around selected tiles
		/// -> superceded by using GraySelection(false) property.
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


		#region Editor
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
				{
					var node = ((XCMapTile)MapBase[row, col]).Node; // leave any node(s) that might be on the tile(s)
					MapBase[row, col] = XCMapTile.VacantTile;
					((XCMapTile)MapBase[row, col]).Node = node;
				}

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

				_copied = new MapTileBase[end.Y - start.Y + 1,
										  end.X - start.X + 1];

				for (int col = start.X; col <= end.X; ++col)
				for (int row = start.Y; row <= end.Y; ++row)
				{
					_copied[row - start.Y,
							col - start.X] = MapBase[row, col];
				}
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
							if ((tileCopy = _copied[row - DragStart.Y,
													col - DragStart.X] as XCMapTile) != null)
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
		#endregion


		#region Mouse & Drag-points
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
					_isMouseDrag = true;
					DragSelect(dragStart, dragStart);

					MapBase.Location = new MapLocation(
													dragStart.Y,
													dragStart.X,
													MapBase.Level);
					Refresh();

					// update the selected tile for RouteView (no drags allowed on RouteView however)
					// NOTE: TopView understands drags and updates auto.
					var routePanel = ViewerFormsManager.RouteView.Control.RoutePanel;
					routePanel.SetSelectedTile(dragStart.X, dragStart.Y);
					routePanel.Refresh();
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
				var location = GetTileLocation(e.X, e.Y);
				if (location.X != DragEnd.X || location.Y != DragEnd.Y)
				{
					if (_isMouseDrag)
					{
						DragSelect(DragStart, location);

						// clear the selected lozenge on RouteView (no drags for RouteView)
						var routePanel = ViewerFormsManager.RouteView.Control.RoutePanel;
						routePanel.SetSelectedTile(-1, -1);
						routePanel.Refresh();
					}

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
		internal void DragSelect(Point dragStart, Point dragEnd)
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
		#endregion


		/// <summary>
		/// Fires when a location is selected in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLocationSelected_Main(LocationSelectedEventArgs args)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("MainViewOverlay.OnLocationSelected_Main");

			FirstClick = true;

			_col = args.Location.Col;
			_row = args.Location.Row;
			_lev = args.Location.Lev;

			DragSelect(new Point(_col, _row), DragEnd);

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


		#region Draw
		/// <summary>
		/// Draws the Map in MainView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (MapBase != null)
			{
				_graphics = e.Graphics;
				_graphics.InterpolationMode = InterpolationLocal;
				_graphics.PixelOffsetMode   = PixelOffsetMode.HighQuality;
//				_graphics.SmoothingMode     = SmoothingMode.HighQuality;

//				if (!_isMouseDrag
//					|| (DragStart.X == DragEnd.X && DragStart.Y == DragEnd.Y))
				if (_spriteShadeEnabled)
					_spriteAttributes.SetGamma(SpriteDarknessLocal, ColorAdjustType.Bitmap); // TODO: laggy ....

				// Image Processing using C# - https://www.codeproject.com/Articles/33838/Image-Processing-using-C
				// ColorMatrix Guide - https://docs.rainmeter.net/tips/colormatrix-guide/

				ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);


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
						DrawGrid();

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

							if (FirstClick && isClicked && Cuboid != null)
								Cuboid.DrawCursorBack(
													_graphics,
													x + 1, y - 1,
//													false,
													lev == MapBase.Level,
													HalfWidth,
													HalfHeight);

							if (lev == MapBase.Level || MapBase[row, col, lev].DrawAbove) // TODO: investigate DrawAbove.
							{
								var tile = (XCMapTile)MapBase[row, col, lev];

								bool isGray = FirstClick && lev == MapBase.Level && _graySelection
										   && (isClicked || dragRect.IntersectsWith(tileRect));

//								if (System.Windows.Forms.Control.MouseButtons != System.Windows.Forms.MouseButtons.Left) // TEST.
//								if (!_isMouseDrag)
									DrawTile(tile, x, y, isGray);
							}

							if (FirstClick && isClicked && Cuboid != null)
								Cuboid.DrawCursorFront(
													_graphics,
													x + 1, y - 1,
//													MainViewUnderlay.AniStep,
//													false,
													lev == MapBase.Level,
													HalfWidth,
													HalfHeight);
						}
					}
				}

//				if (_drawSelectionBox) // always false.
				if (FirstClick && !_graySelection)
					DrawSelectedLozenge(dragRect);
			}
		}

		/// <summary>
		/// Draws the grid-lines and the grid-sheet.
		/// </summary>
		private void DrawGrid()
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

			_graphics.FillPath(_brushLayer, _layerFill); // the grid-sheet

			// draw the grid-lines ->
			for (int i = 0; i <= MapBase.MapSize.Rows; ++i)
				_graphics.DrawLine(
								_penGrid,
								x - HalfWidth  * i,
								y + HalfHeight * i,
								x + (MapBase.MapSize.Cols - i) * HalfWidth,
								y + (MapBase.MapSize.Cols + i) * HalfHeight);

			for (int i = 0; i <= MapBase.MapSize.Cols; ++i)
				_graphics.DrawLine(
								_penGrid,
								x + HalfWidth  * i,
								y + HalfHeight * i,
								x - x1 + HalfWidth  * i,
								y + y1 + HalfHeight * i);
		}


		private const int SpriteWidth  = 32;
		private const int SpriteHeight = 40;

		/// <summary>
		/// Draws the tileparts in the Tile.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="isGray"></param>
		private void DrawTile(
				XCMapTile tile,
				int x, int y,
				bool isGray)
		{
			// NOTE: The width and height args are based on a sprite that's 32x40.
			// Going back to a universal sprite-size would do this:
			//   (int)(sprite.Width  * Globals.Scale)
			//   (int)(sprite.Height * Globals.Scale)
			// with its attendent consequences.

			TilepartBase part = null;

			var topView = ViewerFormsManager.TopView.Control;
			if (topView.GroundVisible)
			{
				if ((part = tile.Ground) != null)
				{
					var sprite = (isGray) ? part[MainViewUnderlay.AniStep].SpriteGray
										  : part[MainViewUnderlay.AniStep].Image;
					DrawSprite(
							sprite,
							new Rectangle(
									x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst,
									HalfWidth * 2, HalfHeight * 5));
				}
			}

			if (topView.WestVisible)
			{
				if ((part = tile.West) != null)
				{
					var sprite = (isGray) ? part[MainViewUnderlay.AniStep].SpriteGray
										  : part[MainViewUnderlay.AniStep].Image;
					DrawSprite(
							sprite,
							new Rectangle(
									x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst,
									HalfWidth * 2, HalfHeight * 5));
				}
			}

			if (topView.NorthVisible)
			{
				if ((part = tile.North) != null)
				{
					var sprite = (isGray) ? part[MainViewUnderlay.AniStep].SpriteGray
										  : part[MainViewUnderlay.AniStep].Image;
					DrawSprite(
							sprite,
							new Rectangle(
									x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst,
									HalfWidth * 2, HalfHeight * 5));
				}
			}

			if (topView.ContentVisible)
			{
				if ((part = tile.Content) != null)
				{
					var sprite = (isGray) ? part[MainViewUnderlay.AniStep].SpriteGray
										  : part[MainViewUnderlay.AniStep].Image;
					DrawSprite(
							sprite,
							new Rectangle(
									x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst,
									HalfWidth * 2, HalfHeight * 5));
				}
			}
		}

		/// <summary>
		/// Draws a tilepart's sprite.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="rect"></param>
		private void DrawSprite(Image sprite, Rectangle rect)
		{
			if (_spriteShadeEnabled)
				_graphics.DrawImage(
								sprite,
								rect,
								0, 0, SpriteWidth, SpriteHeight,
								GraphicsUnit.Pixel,
								_spriteAttributes);
			else
				_graphics.DrawImage(sprite, rect);
		}

		/// <summary>
		/// Draws a red lozenge around any selected Tiles if the option to draw
		/// selected tiles in grayscale is FALSE.
		/// </summary>
		/// <param name="dragRect"></param>
		private void DrawSelectedLozenge(Rectangle dragRect)
		{
			var top    = GetScreenCoordinates(new Point(dragRect.X,     dragRect.Y));
			var right  = GetScreenCoordinates(new Point(dragRect.Right, dragRect.Y));
			var bottom = GetScreenCoordinates(new Point(dragRect.Right, dragRect.Bottom));
			var left   = GetScreenCoordinates(new Point(dragRect.Left,  dragRect.Bottom));

			top.X    += HalfWidth;
			right.X  += HalfWidth;
			bottom.X += HalfWidth;
			left.X   += HalfWidth;

			float penWidth = Globals.Scale < 1.5 ? 2
												 : 3;
			var pen = new Pen(Color.FromArgb(60, Color.Red), penWidth);

			_graphics.DrawLine(pen, top,    right);
			_graphics.DrawLine(pen, right,  bottom);
			_graphics.DrawLine(pen, bottom, left);
			_graphics.DrawLine(pen, left,   top);
		}
		#endregion


		#region Coordinate conversion
		/// <summary>
		/// Converts a point from tile-location to screen-coordinates.
		/// </summary>
		/// <param name="point">the x/y-position of a tile</param>
		/// <returns></returns>
		private Point GetScreenCoordinates(Point point)
		{
			int verticalOffset = HalfHeight * (MapBase.Level + 1) * 3;
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
			double halfWidth  = HalfWidth;
			double halfHeight = HalfHeight;

			double verticalOffset = (MapBase.Level + 1) * 3;

			double xd = Math.Floor(x - Origin.X - halfWidth);					// x=0 is the axis from the top to the bottom of the map-lozenge.
			double yd = Math.Floor(y - Origin.Y - halfHeight * verticalOffset);	// y=0 is measured from the top of the map-lozenge downward.

			double x1 = xd / (halfWidth  * 2)
					  + yd / (halfHeight * 2);
			double y1 = (yd * 2 - xd) / (halfWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}
		#endregion
	}
}
