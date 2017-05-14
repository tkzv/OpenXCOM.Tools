using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed class RoutePanel
		:
			RoutePanelParent
	{
		#region Fields & Properties
		private Point _pos = new Point(-1, -1); // TODO: use 'null' instead of (-1,-1) here and elsewhere.
		/// <summary>
		/// Tracks the screen-position of the mouse cursor.
		/// </summary>
		public Point CursorPosition
		{
			get { return _pos; }
			set { _pos = value; }
		}

		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over cursor lozenge
		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // clicked tile lozenge

		private readonly DrawBlobService _blobService = new DrawBlobService();

		private readonly Font _fontOverlay = new Font("Verdana", 7F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
		private readonly Font _fontRose    = new Font("Courier New", 22, FontStyle.Bold);

		private ColorTools _toolWall;
		private ColorTools _toolContent;

		private Graphics     _graphics;
		private GraphicsPath _nodeFill = new GraphicsPath();

		private RouteNode _nodeSelected;
		#endregion


		/// <summary>
		/// You know the drill ... Paint it, Black
		/// black as night
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			_graphics = e.Graphics;
			_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
//			_graphics.SmoothingMode = SmoothingMode.HighQuality;

			ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);


//			try // TODO: i get the impression that many of the try/catch blocks can and should be replaced w/ standard code.
//			{
			if (MapFile != null)
			{
				_blobService.HalfWidth  = DrawAreaWidth;
				_blobService.HalfHeight = DrawAreaHeight;

				DrawBlobs();

				if (ClickPoint.X > -1 && ClickPoint.Y > -1)
				{
					_nodeSelected = ((XCMapTile)MapFile[ClickPoint.Y, ClickPoint.X]).Node;
				}
				else
					_nodeSelected = null;

				DrawLinks();

				if (_nodeSelected != null)
					DrawLinkLines(
							Origin.X + (ClickPoint.X - ClickPoint.Y)     * DrawAreaWidth,
							Origin.Y + (ClickPoint.X + ClickPoint.Y + 1) * DrawAreaHeight,
							_nodeSelected,
							RoutePens[RouteView.SelectedLinkColor]);

				DrawNodes();

				DrawGridLines();

				if (_overCol != -1) // draw the selector lozenge
				{
					PathSelectorLozenge(
									Origin.X + (_overCol - _overRow) * DrawAreaWidth,
									Origin.Y + (_overCol + _overRow) * DrawAreaHeight);
					_graphics.DrawPath(
									new Pen(Color.Black, 2), // TODO: use TopPens[TopView.SelectorColor]
									_lozSelector);
				}

				if (_selectedCol != -1)
				{
					PathSelectedLozenge(); // TODO: cache that.
					_graphics.DrawPath(
									new Pen(Color.RoyalBlue, 2), // TODO: use TopPens[TopView.SelectorColor] or other.
									_lozSelected);
				}

				if (ShowPriorityBars)
					DrawNodeImportanceMeters();

				DrawRose();

				if (ShowOverlay)
					DrawInfoOverlay();
			}
//			}
//			catch (Exception ex)
//			{
//				g.FillRectangle(new SolidBrush(Color.Black), g.ClipBounds);
//				g.DrawString(
//							ex.Message,
//							Font,
//							new SolidBrush(Color.White),
//							8, 8);
//				throw;
//			}
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

			var p0 = new Point(x,             y);
			var p1 = new Point(x + halfWidth, y + halfHeight);
			var p2 = new Point(x,             y + halfHeight * 2);
			var p3 = new Point(x - halfWidth, y + halfHeight);

			_lozSelector.Reset();
			_lozSelector.AddLine(p0, p1);
			_lozSelector.AddLine(p1, p2);
			_lozSelector.AddLine(p2, p3);
			_lozSelector.CloseFigure();
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around the tile at the
		/// current 'ClickPoint'.
		/// </summary>
		internal void PathSelectedLozenge()
		{
			int halfWidth  = _blobService.HalfWidth;
			int halfHeight = _blobService.HalfHeight;

			var p0 = new Point(
							Origin.X + (_selectedCol - _selectedRow) * halfWidth,
							Origin.Y + (_selectedCol + _selectedRow) * halfHeight);
			var p1 = new Point(
							Origin.X + (_selectedCol - _selectedRow) * halfWidth  + halfWidth,
							Origin.Y + (_selectedCol + _selectedRow) * halfHeight + halfHeight);
			var p2 = new Point(
							Origin.X + (_selectedCol - _selectedRow) * halfWidth,
							Origin.Y + (_selectedCol + _selectedRow) * halfHeight + halfHeight * 2);
			var p3 = new Point(
							Origin.X + (_selectedCol - _selectedRow) * halfWidth  - halfWidth,
							Origin.Y + (_selectedCol + _selectedRow) * halfHeight + halfHeight);

			_lozSelected.Reset();
			_lozSelected.AddLine(p0, p1);
			_lozSelected.AddLine(p1, p2);
			_lozSelected.AddLine(p2, p3);
			_lozSelected.CloseFigure();
		}

		internal void SetSelectedTile(int x, int y)
		{
			_selectedCol = x;
			_selectedRow = y;

			if (x != -1)
				PathSelectedLozenge();
		}


		#region Draw Methods
		/// <summary>
		/// Draws any wall and/or content indicators.
		/// </summary>
		private void DrawBlobs()
		{
			if (_toolWall == null)
				_toolWall = new ColorTools(RoutePens[RouteView.WallColor]);

			if (_toolContent == null)
				_toolContent = new ColorTools(RouteBrushes[RouteView.ContentColor], _toolWall.Pen.Width);


			XCMapTile tile = null;
			for (int
					r = 0,
						startX = Origin.X,
						startY = Origin.Y;
					r != MapFile.MapSize.Rows;
					++r,
						startX -= DrawAreaWidth,
						startY += DrawAreaHeight)
			{
				for (int
						c = 0,
							x = startX,
							y = startY;
						c != MapFile.MapSize.Cols;
						++c,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					if (MapFile[r, c] != null)
					{
						tile = MapFile[r, c] as XCMapTile;

						if (tile.Content != null)
							_blobService.DrawContent(_graphics, _toolContent, x, y, tile.Content);

						if (tile.West != null)
							_blobService.DrawContent(_graphics, _toolWall, x, y, tile.West);

						if (tile.North != null)
							_blobService.DrawContent(_graphics, _toolWall, x, y, tile.North);
					}
				}
			}
		}

		/// <summary>
		/// Draws unselected link-lines.
		/// </summary>
		private void DrawLinks()
		{
			var pen = RoutePens[RouteView.UnselectedLinkColor];

			RouteNode node = null;
			for (int
					rSrc = 0,
						x = Origin.X,
						y = Origin.Y;
					rSrc != MapFile.MapSize.Rows;
					++rSrc,
						x -= DrawAreaWidth,
						y += DrawAreaHeight)
			{
				for (int
						cSrc = 0,
							xSrc = x,
							ySrc = y;
						cSrc != MapFile.MapSize.Cols;
						++cSrc,
							xSrc += DrawAreaWidth,
							ySrc += DrawAreaHeight)
				{
					if (MapFile[rSrc, cSrc] != null
						&& (node = ((XCMapTile)MapFile[rSrc, cSrc]).Node) != null
						&& (_nodeSelected == null || !_nodeSelected.Equals(node)))
					{
						DrawLinkLines(xSrc, ySrc, node, pen, false);
					}
				}
			}
		}

		/// <summary>
		/// Draws link-lines for a given node.
		/// </summary>
		/// <param name="xSrc"></param>
		/// <param name="ySrc"></param>
		/// <param name="node"></param>
		/// <param name="pen"></param>
		/// <param name="selected">(default true)</param>
		private void DrawLinkLines(
				int xSrc,
				int ySrc,
				RouteNode node,
				Pen pen,
				bool selected = true)
		{
			int xDst;
			int yDst;
			int yOffset = (!selected) ? DrawAreaHeight
									  : 0;

			RouteNode nodeDst = null;

			for (int i = 0; i != RouteNode.LinkSlots; ++i)
			{
				xDst = -1;
				yDst =  0;

				var link = node[i] as Link;
				switch (link.Destination)
				{
					case Link.NotUsed:
						break;

					case Link.ExitWest:
						xDst = OffsetX + 1;
						yDst = OffsetY + 1;
						break;

					case Link.ExitNorth:
						xDst = Width - OffsetX * 2;
						yDst = OffsetY + 1;
						break;

					case Link.ExitEast:
						xDst = Width  - OffsetX * 2;
						yDst = Height - OffsetY * 2;
						break;

					case Link.ExitSouth:
						xDst = OffsetX + 1;
						yDst = Height - OffsetY * 2;
						break;

					default:
						if ((nodeDst = MapFile.RouteFile[link.Destination]) != null
							&& nodeDst.Lev == MapFile.Level
							&& (_nodeSelected == null || !_nodeSelected.Equals(nodeDst)))
						{
							xDst = Origin.X + (nodeDst.Col - nodeDst.Row)     * DrawAreaWidth;
							yDst = Origin.Y + (nodeDst.Col + nodeDst.Row + 1) * DrawAreaHeight;
						}
						break;
				}

				if (xDst != -1)
					_graphics.DrawLine(
									pen,
									xSrc, ySrc + yOffset, // unselected nodes need an offset
									xDst, yDst);
			}
		}


		private bool _brushes;
		private SolidBrush _brushSelected;
		private SolidBrush _brushUnselected;
		private SolidBrush _brushSpawn;

		/// <summary>
		/// Draws the nodes.
		/// </summary>
		private void DrawNodes()
		{
			if (!_brushes)
			{
				_brushes = true;

				_brushSelected   = RouteBrushes[RouteView.SelectedNodeColor];
				_brushUnselected = RouteBrushes[RouteView.UnselectedNodeColor];
				_brushSpawn      = RouteBrushes[RouteView.SpawnNodeColor];
			}

			_brushSelected.Color   = Color.FromArgb(Opacity, _brushSelected.Color); // NOTE: these colors change depending on Options.
			_brushUnselected.Color = Color.FromArgb(Opacity, _brushUnselected.Color);
			_brushSpawn.Color      = Color.FromArgb(Opacity, _brushSpawn.Color);


			int startX = Origin.X;
			int startY = Origin.Y;

			for (int row = 0; row != MapFile.MapSize.Rows; ++row)
			{
				for (int
						col = 0,
							x = startX,
							y = startY;
						col != MapFile.MapSize.Cols;
						++col,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					var tile = MapFile[row, col] as XCMapTile;	// NOTE: XCMapBase has the current level stored and uses
					if (tile != null)							// it to return only tiles on the correct level here.
					{
						var node = tile.Node;
						if (node != null)
						{
							_nodeFill.Reset();
							_nodeFill.AddLine(
											x,                 y,
											x + DrawAreaWidth, y + DrawAreaHeight);
							_nodeFill.AddLine(
											x + DrawAreaWidth, y + DrawAreaHeight,
											x,                 y + DrawAreaHeight * 2);
							_nodeFill.AddLine(
											x,                 y + DrawAreaHeight * 2,
											x - DrawAreaWidth, y + DrawAreaHeight);
							_nodeFill.CloseFigure();


							if (row == ClickPoint.Y && col == ClickPoint.X)
							{
								_graphics.FillPath(_brushSelected, _nodeFill);
							}
							else if (node.SpawnWeight != SpawnUsage.NoSpawn)
							{
								_graphics.FillPath(_brushSpawn, _nodeFill);
							}
							else
								_graphics.FillPath(_brushUnselected, _nodeFill);


							for (int i = 0; i != RouteNode.LinkSlots; ++i) // check for and if applicable draw the up/down indicators.
							{
								var link = node[i] as Link;
								switch (link.Destination)
								{
									case Link.NotUsed:
									case Link.ExitEast:
									case Link.ExitNorth:
									case Link.ExitSouth:
									case Link.ExitWest:
										break;

									default:
										if (MapFile.RouteFile[link.Destination] != null)
										{
											int levelDestination = MapFile.RouteFile[link.Destination].Lev;
											if (levelDestination != MapFile.Level)
											{
												if (levelDestination < MapFile.Level) // draw arrow up.
												{
													_graphics.DrawLine( // start w/ a vertical line in the tile-lozenge
																	RoutePens[RouteView.UnselectedLinkColor],
																	x, y,
																	x, y + DrawAreaHeight * 2);
													_graphics.DrawLine(
																	RoutePens[RouteView.UnselectedLinkColor],
																	x + 2,                 y,
																	x + 2 - DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine(
																	RoutePens[RouteView.UnselectedLinkColor],
																	x - 2,                 y,
																	x - 2 + DrawAreaWidth, y + DrawAreaHeight);
												}
												else //if (levelDestination > MapFile.Level) // draw arrow down.
												{
													_graphics.DrawLine( // start w/ a horizontal line in the tile-lozenge
																	RoutePens[RouteView.UnselectedLinkColor],
																	x - DrawAreaWidth, y + DrawAreaHeight,
																	x + DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine(
																	RoutePens[RouteView.UnselectedLinkColor],
																	x + 2,                 y + DrawAreaHeight * 2,
																	x + 2 - DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine(
																	RoutePens[RouteView.UnselectedLinkColor],
																	x - 2,                 y + DrawAreaHeight * 2,
																	x - 2 + DrawAreaWidth, y + DrawAreaHeight);
												}
											}
										}
										break;
								}
							}
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		/// <summary>
		/// Draws the grid-lines.
		/// </summary>
		private void DrawGridLines()
		{
			var map = MapFile;

			for (int i = 0; i <= map.MapSize.Rows; ++i)
				_graphics.DrawLine(
								RoutePens[RouteView.GridLineColor],
								Origin.X - i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
								Origin.X + ((map.MapSize.Cols - i) * DrawAreaWidth),
								Origin.Y + ((map.MapSize.Cols + i) * DrawAreaHeight));

			for (int i = 0; i <= map.MapSize.Cols; ++i)
				_graphics.DrawLine(
								RoutePens[RouteView.GridLineColor],
								Origin.X + i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
							   (Origin.X + i * DrawAreaWidth)  - map.MapSize.Rows * DrawAreaWidth,
							   (Origin.Y + i * DrawAreaHeight) + map.MapSize.Rows * DrawAreaHeight);
		}


		private const int NodeValMax = 12;

		/// <summary>
		/// Draws the node importance bars.
		/// </summary>
		private void DrawNodeImportanceMeters()
		{
			int startX = Origin.X;
			int startY = Origin.Y;

			for (int r = 0; r != MapFile.MapSize.Rows; ++r)
			{
				for (int
						c = 0,
							x = startX,
							y = startY;
						c != MapFile.MapSize.Cols;
						++c,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					var tile = MapFile[r, c] as XCMapTile;
					if (tile != null)
					{
						var node = tile.Node;
						if (node != null)
						{
//								if (DrawAreaHeight >= NodeValMax)
//								{
								int infoboxX = x - DrawAreaWidth / 2 - 2;			// -2 to prevent drawing over the link-going-up
								int infoboxY = y + DrawAreaHeight - NodeValMax / 2;	//    vertical line indicator when panel is small sized.

								int nodePatrolPriority = (int)node.Priority;
								DrawImportanceMeter(
												infoboxX,
												infoboxY,
												nodePatrolPriority,
												Brushes.DeepSkyBlue); //CornflowerBlue

								int nodeSpawnWeight = (int)node.SpawnWeight;
								DrawImportanceMeter(
												infoboxX + 3,
												infoboxY,
												nodeSpawnWeight,
												Brushes.LightCoral); //IndianRed
//								}
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		/// <summary>
		/// Helper for DrawNodeImportanceMeters().
		/// </summary>
		/// <param name="infoboxX"></param>
		/// <param name="infoboxY"></param>
		/// <param name="value"></param>
		/// <param name="color"></param>
		private void DrawImportanceMeter(
				int infoboxX,
				int infoboxY,
				int value,
				Brush color)
		{
			var p0 = new Point(
							infoboxX - 1, // ...
							infoboxY);
			var p1 = new Point(
							infoboxX + 3,
							infoboxY);
			var p2 = new Point(
							infoboxX + 3,
							infoboxY + NodeValMax - 1);
			var p3 = new Point(
							infoboxX,
							infoboxY + NodeValMax - 1);
			var p4 = new Point(
							infoboxX,
							infoboxY);

			var path = new GraphicsPath();

			path.AddLine(p0, p1);
			path.AddLine(p1, p2);
			path.AddLine(p2, p3);
			path.AddLine(p3, p4);

			_graphics.FillPath(Brushes.WhiteSmoke, path); // fill background.

			if (value > 0)
				_graphics.FillRectangle(
									color,
									infoboxX, infoboxY + NodeValMax - value - 2,
									2, value);

			_graphics.DrawPath(Pens.Black, path); // draw borders.
		}


		private const int RoseMarginX = 25;
		private const int RoseMarginY = 5;

		/// <summary>
		/// Draws the compass-rose.
		/// </summary>
		private void DrawRose()
		{
			_graphics.DrawString(
							"W",
							_fontRose,
							Brushes.Black,
							RoseMarginX,
							RoseMarginY);
			_graphics.DrawString(
							"N",
							_fontRose,
							Brushes.Black,
//							Width - TextRenderer.MeasureText("N", _fontRose).Width - RoseMarginX,
							Width - (int)_graphics.MeasureString("N", _fontRose).Width - RoseMarginX,
							RoseMarginY);
			_graphics.DrawString(
							"S",
							_fontRose,
							Brushes.Black,
							RoseMarginX,
							Height - _fontRose.Height - RoseMarginY);
			_graphics.DrawString(
							"E",
							_fontRose,
							Brushes.Black,
//							Width  - TextRenderer.MeasureText("E", _fontRose).Width - RoseMarginX,
							Width  - (int)_graphics.MeasureString("E", _fontRose).Width - RoseMarginX,
							Height - _fontRose.Height - RoseMarginY);
		}

		/// <summary>
		/// Draws tile/node information in the overlay.
		/// </summary>
		private void DrawInfoOverlay()
		{
			var tile = GetTile(CursorPosition.X, CursorPosition.Y);
			if (tile != null)
			{
				var pt = GetTileLocation(CursorPosition.X, CursorPosition.Y);
				const string textTile1 = ""; // "position" or "location" or ... "pos" or "loc" ...
				string textTile2 = "c " + pt.X + "  r " + pt.Y + "  L " + (MapFile.MapSize.Levs - MapFile.Level);

//				int textWidth1 = TextRenderer.MeasureText(textTile1, font).Width;
				int textWidth1 = (int)_graphics.MeasureString(textTile1, _fontOverlay).Width;
//				int textWidth2 = TextRenderer.MeasureText(textTile2, font).Width;
				int textWidth2 = (int)_graphics.MeasureString(textTile2, _fontOverlay).Width;

				string textOver1     = String.Empty;
				string textPriority1 = String.Empty;
				string textSpawn1    = String.Empty;
				string textWeight1   = String.Empty;

				string textOver2     = String.Empty;
				string textPriority2 = String.Empty;
				string textSpawn2    = String.Empty;
				string textWeight2   = String.Empty;

				if (tile.Node != null)
				{
					textOver1     = "over";
					textPriority1 = "priority";
					textSpawn1    = "spawn";
					textWeight1   = "weight";

					textOver2     = (tile.Node.Index).ToString(System.Globalization.CultureInfo.CurrentCulture);
					textPriority2 = (tile.Node.Priority).ToString();
					textSpawn2    = (RouteNodeCollection.UnitRankUfo[tile.Node.SpawnRank]).ToString();
					textWeight2   = (tile.Node.SpawnWeight).ToString();

					int width;
//					width = TextRenderer.MeasureText(textOver1, font).Width;
					width = (int)_graphics.MeasureString(textOver1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textPriority1, font).Width;
					width = (int)_graphics.MeasureString(textPriority1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textSpawn1, font).Width;
					width = (int)_graphics.MeasureString(textSpawn1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textWeight1, font).Width;
					width = (int)_graphics.MeasureString(textWeight1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;

//					width = TextRenderer.MeasureText(textOver2, font).Width;
					width = (int)_graphics.MeasureString(textOver2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textPriority2, font).Width;
					width = (int)_graphics.MeasureString(textPriority2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textSpawn2, font).Width;
					width = (int)_graphics.MeasureString(textSpawn2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textWeight2, font).Width;
					width = (int)_graphics.MeasureString(textWeight2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					// time to move to a higher .NET framework.
				}

				const int Separator = 0;

//				int textHeight = TextRenderer.MeasureText("X", font).Height;
				int textHeight = (int)_graphics.MeasureString("X", _fontOverlay).Height;
				var overlay = new Rectangle(
										CursorPosition.X + 18, CursorPosition.Y,
										textWidth1 + Separator + textWidth2 + 8, textHeight + 8);

				if (tile.Node != null)
					overlay.Height += textHeight * 4;

				if (overlay.X + overlay.Width > ClientRectangle.Width)
					overlay.X = CursorPosition.X - overlay.Width - 8;

				if (overlay.Y + overlay.Height > ClientRectangle.Height)
					overlay.Y = CursorPosition.Y - overlay.Height;

				_graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, Color.DarkBlue)), overlay);
				_graphics.FillRectangle(
									new SolidBrush(Color.FromArgb(120, Color.Linen)),
									overlay.X + 2,
									overlay.Y + 2,
									overlay.Width  - 4,
									overlay.Height - 4);

				int textLeft = overlay.X + 4;
				int textTop  = overlay.Y + 3;

				_graphics.DrawString(
								textTile1,
								_fontOverlay,
								Brushes.Yellow,
								textLeft,
								textTop);
				_graphics.DrawString(
								textTile2,
								_fontOverlay,
								Brushes.Yellow,
								textLeft + textWidth1 + Separator,
								textTop);

				if (tile.Node != null)
				{
					_graphics.DrawString(
									textOver1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight);
					_graphics.DrawString(
									textOver2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Separator,
									textTop  + textHeight);

					_graphics.DrawString(
									textPriority1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 2);
					_graphics.DrawString(
									textPriority2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Separator,
									textTop  + textHeight * 2);

					_graphics.DrawString(
									textSpawn1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 3);
					_graphics.DrawString(
									textSpawn2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Separator,
									textTop  + textHeight * 3);

					_graphics.DrawString(
									textWeight1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 4);
					_graphics.DrawString(
									textWeight2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Separator,
									textTop  + textHeight * 4);
				}
			}
		}
		#endregion
	}
}
