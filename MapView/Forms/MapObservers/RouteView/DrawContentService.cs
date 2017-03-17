using System.Drawing;
using System.Drawing.Drawing2D;

using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// Draws floor- and wall- and content- blobs for TopView and RMP View.
	/// </summary>
	public class DrawContentService // kL_note: should be called DrawStuffService().
	{
		public int HWidth  = 8; // NOTE: 'H' means half.
		public int HHeight = 4;

		private readonly GraphicsPath _floor;
		private readonly GraphicsPath _content;


		/// <summary>
		/// cTor. Draws floor- and wall- and content- blobs for TopView and RMP View.
		/// </summary>
		public DrawContentService()
		{
			_floor   = new GraphicsPath();
			_content = new GraphicsPath();
		}


		/// <summary>
		/// Draws floor-blobs for TopView.
		/// </summary>
		public void DrawFloor(
				Graphics g,
				SolidBrush brush,
				int x, int y)
		{
			g.FillPath(brush, GetFloorPath(x, y));
		}

		private const int _pad = 4;

		/// <summary>
		/// Draws wall- and content- blobs for TopView and RMP View.
		/// </summary>
		public void DrawContent(
				Graphics g,
				SolidPenBrush color,
				int x, int y,
				TileBase content)
		{
			var ptTop	= new Point(
								x,
								y + _pad);
			var ptBot	= new Point(
								x,
								y + (HHeight * 2) - _pad);
			var ptLeft	= new Point(
								x - HWidth + (_pad * 2),
								y + HHeight);
			var ptRight	= new Point(
								x + HWidth - (_pad * 2),
								y + HHeight);

			switch (ContentTypeService.GetContentType(content))
			{
				case ContentType.Content:
					SetGroundPath(x, y);
					g.FillPath(
							color.Brush,
							_content);
					break;

				case ContentType.Ground:
					SetGroundPath(x, y);
					g.FillPath(
							color.LightBrush,
							_content);
					break;

				case ContentType.NorthFence:
					g.DrawLine(
							color.LightPen,
							ptTop,
							ptRight);
					break;

				case ContentType.NorthWall:
					g.DrawLine(
							color.Pen,
							ptTop,
							ptRight);

					if (ContentTypeService.IsDoor(content))
						g.DrawLine(
								color.Pen,
								ptTop,
								Point.Add(ptRight, new Size(-10, 4)));
					break;

				case ContentType.WestFence:
					g.DrawLine(
							color.LightPen,
							ptTop,
							ptLeft);
					break;

				case ContentType.WestWall:
					g.DrawLine(
							color.Pen,
							ptTop,
							ptLeft);

					if (ContentTypeService.IsDoor(content))
						g.DrawLine(
								color.Pen,
								Point.Add(ptTop, new Size(6, 8)),
								ptLeft);
					break;

				case ContentType.NorthWallWindow:
					DrawWindow(
							g,
							color,
							ptTop,
							ptRight);
					break;

				case ContentType.WestWallWindow:
					DrawWindow(
							g,
							color,
							ptTop,
							ptLeft);
					break;

				case ContentType.SouthWall:
					g.DrawLine(
							color.Pen,
							ptLeft,
							ptBot);
					break;

				case ContentType.EastWall:
					g.DrawLine(
							color.Pen,
							ptBot,
							ptRight);
					break;

				case ContentType.NorthwestSoutheast:
					g.DrawLine(
							color.Pen,
							ptTop,
							ptBot);
					break;

				case ContentType.NortheastSouthwest:
					g.DrawLine(
							color.Pen,
							ptLeft,
							ptRight);
					break;

				case ContentType.NorthwestCorner:
					g.DrawLine(
							color.Pen,
							Point.Add(ptTop, new Size(-4, 0)),
							Point.Add(ptTop, new Size( 4, 0)));
					break;

				case ContentType.NortheastCorner:
					g.DrawLine(
							color.Pen,
							Point.Add(ptRight, new Size(0, -4)),
							Point.Add(ptRight, new Size(0,  4)));
					break;

				case ContentType.SoutheastCorner:
					g.DrawLine(
							color.Pen,
							Point.Add(ptBot, new Size(-4, 0)),
							Point.Add(ptBot, new Size( 4, 0)));
					break;

				case ContentType.SouthwestCorner:
					g.DrawLine(
							color.Pen,
							Point.Add(ptLeft, new Size(0, -4)),
							Point.Add(ptLeft, new Size(0,  4)));
					break;
			}
		}

		private void SetGroundPath(int x, int y)
		{
			var w = HWidth  / 2;
			var h = HHeight / 2;

			y += h;

			_content.Reset();
			_content.AddLine(
							x, y,
							x + w, y + h);
			_content.AddLine(
							x + w, y + h,
							x, y + h * 2);
			_content.AddLine(
							x, y + h * 2,
							x - w, y + h);
			_content.CloseFigure();
		}

		private static void DrawWindow(
				Graphics g,
				SolidPenBrush color,
				Point start, Point end)
		{
			var pt	= Point.Subtract(end, new Size(start));
			var xy	= new Size(pt.X / 3, pt.Y / 3);
			pt		= Point.Add(start, Size.Add(xy, xy));

			g.DrawLine(
					color.Pen,
					start,
					Point.Add(start, xy));
			g.DrawLine(
					color.LightPen,
					Point.Add(start, xy),
					pt);
			g.DrawLine(
					color.Pen,
					pt,
					end);
		}

		private GraphicsPath GetFloorPath(int x, int y)
		{
			_floor.Reset();
			_floor.AddLine(
						x, y,
						x + HWidth, y + HHeight);
			_floor.AddLine(
						x + HWidth, y + HHeight,
						x, y + HHeight * 2);
			_floor.AddLine(
						x, y + HHeight * 2,
						x - HWidth, y + HHeight);
			_floor.CloseFigure();

			return _floor;
		}
	}
}
