using System.Drawing;
using System.Drawing.Drawing2D;

using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// Draws floor- and wall- and content- blobs for RouteView and TopView.
	/// </summary>
	internal sealed class DrawContentService	// Warning CA1001: Implement IDisposable on 'DrawContentService' because
	{											// it creates members of the following IDisposable types: 'GraphicsPath'.
		#region Fields & Properties
		private readonly GraphicsPath _floor   = new GraphicsPath();
		private readonly GraphicsPath _content = new GraphicsPath();

		private int _halfWidth = 8;
		internal int HalfWidth
		{
			get { return _halfWidth; }
			set { _halfWidth = value; }
		}
		private int _halfHeight = 4;
		internal int HalfHeight
		{
			get { return _halfHeight; }
			set { _halfHeight = value; }
		}
		#endregion


		#region Methods
		/// <summary>
		/// Draws floor-blobs for TopView.
		/// </summary>
		internal void DrawFloor(
				Graphics g,
				Brush brush,
				int x, int y)
		{
			_floor.Reset();
			_floor.AddLine(
						x,             y,
						x + HalfWidth, y + HalfHeight);
			_floor.AddLine(
						x + HalfWidth, y + HalfHeight,
						x,             y + HalfHeight * 2);
			_floor.AddLine(
						x,             y + HalfHeight * 2,
						x - HalfWidth, y + HalfHeight);
			_floor.CloseFigure();

			g.FillPath(brush, _floor);
		}


		private const int Margin = 4;

		/// <summary>
		/// Draws wall- and content- blobs for RouteView and TopView.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="tile"></param>
		internal void DrawContent(
				Graphics g,
				ColorTools tool,
				int x, int y,
				TileBase tile)
		{
			var ptTop   = new Point(
								x,
								(y > int.MaxValue - Margin) ? int.MaxValue : y + Margin); // <- FxCop ...
			var ptBot   = new Point(
								x,
								y + (HalfHeight * 2) - Margin);
			var ptLeft  = new Point(
								x - HalfWidth + (Margin * 2),
								y + HalfHeight);
			var ptRight = new Point(
								x + HalfWidth - (Margin * 2),
								y + HalfHeight);

			switch (ContentTypeService.GetContentType(tile))
			{
				case ContentType.Content:
					SetContentPath(x, y);
					g.FillPath(
							tool.Brush,
							_content);
					break;

				case ContentType.Ground:
					SetContentPath(x, y);
					g.FillPath(
							tool.LightBrush,
							_content);
					break;

				case ContentType.NorthFence:
					g.DrawLine(
							tool.LightPen,
							ptTop,
							ptRight);
					break;

				case ContentType.NorthWall:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptRight);

					if (ContentTypeService.IsDoor(tile))
						g.DrawLine(
								tool.Pen,
								ptTop,
								Point.Add(ptRight, new Size(-10, 4)));
					break;

				case ContentType.WestFence:
					g.DrawLine(
							tool.LightPen,
							ptTop,
							ptLeft);
					break;

				case ContentType.WestWall:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptLeft);

					if (ContentTypeService.IsDoor(tile))
						g.DrawLine(
								tool.Pen,
								Point.Add(ptTop, new Size(6, 8)),
								ptLeft);
					break;

				case ContentType.NorthWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptRight);
					break;

				case ContentType.WestWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptLeft);
					break;

				case ContentType.SouthWall:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptBot);
					break;

				case ContentType.EastWall:
					g.DrawLine(
							tool.Pen,
							ptBot,
							ptRight);
					break;

				case ContentType.NorthwestSoutheast:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptBot);
					break;

				case ContentType.NortheastSouthwest:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptRight);
					break;

				case ContentType.NorthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptTop, new Size(-4, 0)),
							Point.Add(ptTop, new Size( 4, 0)));
					break;

				case ContentType.NortheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptRight, new Size(0, -4)),
							Point.Add(ptRight, new Size(0,  4)));
					break;

				case ContentType.SoutheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptBot, new Size(-4, 0)),
							Point.Add(ptBot, new Size( 4, 0)));
					break;

				case ContentType.SouthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptLeft, new Size(0, -4)),
							Point.Add(ptLeft, new Size(0,  4)));
					break;
			}
		}

		private void SetContentPath(int x, int y)
		{
			var w = HalfWidth  / 2;
			var h = HalfHeight / 2;

			y += h;

			_content.Reset();
			_content.AddLine(
							x,     y,
							x + w, y + h);
			_content.AddLine(
							x + w, y + h,
							x,     y + h * 2);
			_content.AddLine(
							x,     y + h * 2,
							x - w, y + h);
			_content.CloseFigure();
		}

		private static void DrawWindow(
				Graphics g,
				ColorTools tool,
				Point start, Point end)
		{
			var pt = Point.Subtract(end, new Size(start));
			var xy = new Size(pt.X / 3, pt.Y / 3);
			pt     = Point.Add(start, Size.Add(xy, xy));

			g.DrawLine(
					tool.Pen,
					start,
					Point.Add(start, xy));
			g.DrawLine(
					tool.LightPen,
					Point.Add(start, xy),
					pt);
			g.DrawLine(
					tool.Pen,
					pt,
					end);
		}
		#endregion
	}
}
