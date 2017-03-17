using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	public class RoutePanel
		:
		MapPanel
	{
		private Point _pos = new Point(-1, -1);

		public Point Pos
		{
			get { return _pos; }
			set { _pos = value; }
		}

		private readonly Font _font = new Font("Courier New", 22, FontStyle.Bold);

		private readonly DrawContentService _drawContentService = new DrawContentService();

		private SolidPenBrush _wallColor;

/*		public void Calculate()
		{
			OnResize(null);
		} */

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;

			try // TODO: why do i get the impression that many of the try/catch blocks can and should be replaced w/ standard code.
			{
				if (MapFile != null)
				{
					var upper = new GraphicsPath();

					DrawWallsAndContent(graphics);
					DrawUnselectedLink(upper, graphics);
					DrawSelectedLink(graphics);
					DrawNodes(upper, graphics);
					DrawGridLines(graphics);
					DrawRose(graphics);
					DrawInformation(graphics);
				}
			}
			catch (Exception ex)
			{
				graphics.FillRectangle(new SolidBrush(Color.Black), graphics.ClipBounds);
				graphics.DrawString(
								ex.Message,
								Font,
								new SolidBrush(Color.White),
								8, 8);
				throw;
			}
		}

		#region Draw Methods

		private void DrawInformation(Graphics g)
		{
			var tile = GetTile(Pos.X, Pos.Y);
			if (tile != null)
			{
				var textHeight = (int)g.MeasureString("X", Font).Height;
				var overlayPos = new Rectangle(
											Pos.X + 18, Pos.Y,
											200, textHeight + 10);

				if (tile.Node != null)
					overlayPos.Height += textHeight * 4;

				if (overlayPos.X + overlayPos.Width > ClientRectangle.Width)
					overlayPos.X = Pos.X - overlayPos.Width - 8;

				if (overlayPos.Y + overlayPos.Height > ClientRectangle.Height)
					overlayPos.Y = Pos.X - overlayPos.Height;

				g.FillRectangle(new SolidBrush(Color.FromArgb(192, 0, 0, 0)), overlayPos);
				g.FillRectangle(
							new SolidBrush(Color.FromArgb(192, 255, 255, 255)),
							overlayPos.X + 3,
							overlayPos.Y + 3,
							overlayPos.Width - 6,
							overlayPos.Height - 6);

				var textLeft = overlayPos.X + 5;
				var textTop  = overlayPos.Y + 5;

				var pt = GetTileCoordinates(Pos.X, Pos.Y);
				g.DrawString(
							"Tile (c:" + pt.X + " r:" + pt.Y + ")",
							Font,
							Brushes.Black,
							textLeft,
							textTop);

				if (tile.Node != null)
				{
					g.DrawString(
								"Over: " + tile.Node.Index,
								Font,
								Brushes.Black,
								textLeft,
								textTop + textHeight);
					g.DrawString(
								"Priority: " + tile.Node.Priority,
								Font,
								Brushes.Black,
								textLeft,
								textTop + textHeight * 2);
					g.DrawString(
								"Spawns: " + RouteFile.UnitRankUFO[tile.Node.UsableRank],
								Font,
								Brushes.Black,
								textLeft,
								textTop + textHeight * 3);
					g.DrawString(
								"Weight: " + tile.Node.Spawn,
								Font,
								Brushes.Black,
								textLeft,
								textTop + textHeight * 4);
				}
			}
		}

		private void DrawRose(Graphics g)
		{
			const int PAD_HORI = 25;
			const int PAD_VERT =  5;
			
			g.DrawString(
						"W",
						_font,
						Brushes.Black,
						PAD_HORI,
						PAD_VERT);
			g.DrawString(
						"N",
						_font,
						Brushes.Black,
						Width - (int)g.MeasureString("N", _font).Width - PAD_HORI,
						PAD_VERT);
			g.DrawString(
						"S",
						_font,
						Brushes.Black,
						PAD_HORI,
						Height - _font.Height - PAD_VERT);
			g.DrawString(
						"E",
						_font,
						Brushes.Black,
						Width - (int)g.MeasureString("E", _font).Width - PAD_HORI,
						Height - _font.Height - PAD_VERT);
		}

		private void DrawGridLines(Graphics g)
		{
			var map = MapFile;

			for (int i = 0; i <= map.MapSize.Rows; i++)
				g.DrawLine(
						MapPens["GridLineColor"],
						Origin.X - i * DrawAreaWidth,
						Origin.Y + i * DrawAreaHeight,
						Origin.X + ((map.MapSize.Cols - i) * DrawAreaWidth),
						Origin.Y + ((map.MapSize.Cols + i) * DrawAreaHeight));

			for (int i = 0; i <= map.MapSize.Cols; i++)
				g.DrawLine(
						MapPens["GridLineColor"],
						Origin.X + i * DrawAreaWidth,
						Origin.Y + i * DrawAreaHeight,
					   (Origin.X + i * DrawAreaWidth)  - map.MapSize.Rows * DrawAreaWidth,
					   (Origin.Y + i * DrawAreaHeight) + map.MapSize.Rows * DrawAreaHeight);
		}

		private const int NODE_VAL_MAX = 12;

		private void DrawNodes(GraphicsPath upper, Graphics g)
		{
			var brushSelected   = MapBrushes["SelectedNodeColor"];
			var brushUnselected = MapBrushes["UnselectedNodeColor"];
			var brushSpawn      = MapBrushes["SpawnNodeColor"];

			brushSelected.Color   = Color.FromArgb(200, brushSelected.Color);
			brushUnselected.Color = Color.FromArgb(200, brushUnselected.Color);
			brushSpawn.Color      = Color.FromArgb(200, brushSpawn.Color);

			var startX = Origin.X;
			var startY = Origin.Y;

			for (int r = 0; r < MapFile.MapSize.Rows; ++r)
			{
				for (int
						c = 0, x = startX, y = startY;
						c < MapFile.MapSize.Cols;
						++c, x += DrawAreaWidth, y += DrawAreaHeight)
				{
					var tile = MapFile[r, c] as XCMapTile;
					if (tile != null)
					{
						var node = tile.Node;
						if (node != null)
						{
							upper.Reset();
							upper.AddLine(
										x, y,
										x + DrawAreaWidth, y + DrawAreaHeight);
							upper.AddLine(
										x + DrawAreaWidth, y + DrawAreaHeight,
										x, y + 2 * DrawAreaHeight);
							upper.AddLine(
										x, y + 2 * DrawAreaHeight,
										x - DrawAreaWidth, y + DrawAreaHeight);
							upper.CloseFigure();


							if (r == ClickPoint.Y && c == ClickPoint.X)
							{
								g.FillPath(brushSelected, upper);
							}
							else if (node.Spawn != SpawnUsage.NoSpawn)
							{
								g.FillPath(brushSpawn, upper);
							}
							else
								g.FillPath(brushUnselected, upper);


							for (int i = 0; i < RouteNode.LinkSlots; i++)
							{
								var link = node[i] as Link;
								switch (link.Destination)
								{
									case Link.NOT_USED:
									case Link.EXIT_EAST:
									case Link.EXIT_NORTH:
									case Link.EXIT_SOUTH:
									case Link.EXIT_WEST:
										break;

									default:
										if (   MapFile.RouteFile[link.Destination] != null
											&& MapFile.RouteFile[link.Destination].Height < MapFile.CurrentHeight)
										{
											g.DrawLine(
													MapPens["UnselectedLinkColor"],
													x, y,
													x, y + DrawAreaHeight * 2);
										}
										else if (MapFile.RouteFile[link.Destination] != null
											&&   MapFile.RouteFile[link.Destination].Height > MapFile.CurrentHeight)
										{
											g.DrawLine(
													MapPens["UnselectedLinkColor"],
													x - DrawAreaWidth, y + DrawAreaHeight,
													x + DrawAreaWidth, y + DrawAreaHeight);
										}
										break;
								}
							}

//							if (DrawAreaHeight >= NODE_VAL_MAX)
//							{
							var boxX = x - DrawAreaWidth / 2;
							var boxY = y + DrawAreaHeight - NODE_VAL_MAX / 2;

							var nodePatrolPriority = (int)node.Priority;
							DrawBox(
									g,
									boxX,
									boxY,
									nodePatrolPriority,
									NODE_VAL_MAX,
									Brushes.CornflowerBlue);

							var nodeSpawnWeight = (int)node.Spawn;
							DrawBox(
									g,
									boxX + 3,
									boxY,
									nodeSpawnWeight,
									NODE_VAL_MAX,
									Brushes.Firebrick);
//							}
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		private static void DrawBox(
				Graphics g,
				int boxX,
				int boxY,
				int value,
				int max,
				Brush color)
		{
			g.FillRectangle(
						Brushes.Gray,
						boxX, boxY,
						3, NODE_VAL_MAX);
			g.DrawRectangle(
						Pens.Black,
						boxX, boxY,
						3, NODE_VAL_MAX);

			if (value > 0)
			{
				value = (value > max) ? NODE_VAL_MAX
									  : (int)(Math.Ceiling((double)value / max * NODE_VAL_MAX));

				g.FillRectangle(
							color,
							boxX + 1,
							boxY + (NODE_VAL_MAX - value) - 1,
							2,
							value + 1);
			}
		}

		private void DrawWallsAndContent(Graphics g)
		{
			if (_wallColor == null)
				_wallColor = new SolidPenBrush(MapPens["WallColor"]);

			_drawContentService.HWidth  = DrawAreaWidth;
			_drawContentService.HHeight = DrawAreaHeight;

			var map = MapFile;
			for (int
					r = 0, startX = Origin.X, startY = Origin.Y;
					r < map.MapSize.Rows;
					++r, startX -= DrawAreaWidth, startY += DrawAreaHeight)
			{
				for (int
						c = 0, x = startX, y = startY;
						c < map.MapSize.Cols;
						++c, x += DrawAreaWidth, y += DrawAreaHeight)
				{
					if (map[r, c] != null)
					{
						var tile = (XCMapTile)map[r, c];

						if (tile.North != null)
							_drawContentService.DrawContent(g, _wallColor, x, y, tile.North);

						if (tile.West != null)
							_drawContentService.DrawContent(g, _wallColor, x, y, tile.West);

						if (tile.Content != null)
							_drawContentService.DrawContent(g, _wallColor, x, y, tile.Content);
					}
				}
			}
		}

		private void DrawUnselectedLink(GraphicsPath upper, Graphics g)
		{
			var pen = MapPens["UnselectedLinkColor"];

			for (int
					r = 0, startX = Origin.X, startY = Origin.Y;
					r < MapFile.MapSize.Rows;
					++r, startX -= DrawAreaWidth, startY += DrawAreaHeight)
			{
				for (int
						c = 0, x = startX, y = startY;
						c < MapFile.MapSize.Cols;
						++c, x += DrawAreaWidth, y += DrawAreaHeight)
				{
					if (MapFile[r, c] != null)
					{
						RouteNode entry = ((XCMapTile)MapFile[r, c]).Node;
						if (entry != null)
						{
							upper.Reset();
							upper.AddLine(
										x, y,
										x + DrawAreaWidth, y + DrawAreaHeight);
							upper.AddLine(
										x + DrawAreaWidth, y + DrawAreaHeight,
										x, y + DrawAreaHeight * 2);
							upper.AddLine(
										x, y + DrawAreaHeight * 2,
										x - DrawAreaWidth, y + DrawAreaHeight);
							upper.CloseFigure();


							int xEnd, yEnd;

							for (int i = 0; i != RouteNode.LinkSlots; ++i)
							{
								xEnd = -1;
								yEnd =  0; // pacify the compiler.

								var link = entry[i] as Link;
								switch (link.Destination)
								{
									case Link.NOT_USED:
										break;

									case Link.EXIT_NORTH:
										xEnd = Width;
										yEnd = 0;
										break;

									case Link.EXIT_EAST:
										xEnd = Width;
										yEnd = Height;
										break;

									case Link.EXIT_SOUTH:
										xEnd = 0;
										yEnd = Height;
										break;

									case Link.EXIT_WEST:
										xEnd = 0;
										yEnd = 0;
										break;

									default:
										if (   MapFile.RouteFile[link.Destination] != null
											&& MapFile.RouteFile[link.Destination].Height == MapFile.CurrentHeight)
										{
											int rEnd = MapFile.RouteFile[link.Destination].Row;
											int cEnd = MapFile.RouteFile[link.Destination].Col;

											xEnd = Origin.X + (cEnd - rEnd)     * DrawAreaWidth;
											yEnd = Origin.Y + (cEnd + rEnd + 1) * DrawAreaHeight;
										}
										break;
								}

								if (xEnd != -1)
								{
									g.DrawLine(
											pen,
											x, y + DrawAreaHeight,
											xEnd, yEnd);
								}
							}
						}
					}
				}
			}
		}

		private void DrawSelectedLink(Graphics g)
		{
			int c = ClickPoint.X;
			int r = ClickPoint.Y;

			if (c > -1 && r > -1)
			{
				var entry = ((XCMapTile)MapFile[r, c]).Node;
				if (entry != null)
				{
					int xLoc = Origin.X + (c - r)     * DrawAreaWidth;
					int yLoc = Origin.Y + (c + r + 1) * DrawAreaHeight;

					var pen = MapPens["SelectedLinkColor"];

					int xEnd, yEnd;

					for (int i = 0; i != RouteNode.LinkSlots; ++i)
					{
						xEnd = -1;
						yEnd =  0; // pacify the compiler.

						var link = entry[i] as Link;
						switch (link.Destination)
						{
							case Link.NOT_USED:
								break;

							case Link.EXIT_NORTH:
								xEnd = Width;
								break;

							case Link.EXIT_EAST:
								xEnd = Width;
								yEnd = Height;
								break;

							case Link.EXIT_SOUTH:
								xEnd = 0;
								yEnd = Height;
								break;

							case Link.EXIT_WEST:
								xEnd = 0;
								break;

							default:
								if (   MapFile.RouteFile[link.Destination] != null
									&& MapFile.RouteFile[link.Destination].Height == MapFile.CurrentHeight)
								{
									int cEnd = MapFile.RouteFile[link.Destination].Col;
									int rEnd = MapFile.RouteFile[link.Destination].Row;

									xEnd = Origin.X + (cEnd - rEnd)     * DrawAreaWidth;
									yEnd = Origin.Y + (cEnd + rEnd + 1) * DrawAreaHeight;
								}
								break;
						}

						if (xEnd != -1)
						{
							g.DrawLine(
									pen,
									xLoc, yLoc,
									xEnd, yEnd);
						}
					}
				}
			}
		}

		#endregion
	}
}
