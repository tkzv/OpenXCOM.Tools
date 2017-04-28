using System.Drawing;
using System.Drawing.Drawing2D;

using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;



using System.Collections.Generic;

using XCom;
//using XCom.Interfaces.Base;
//namespace MapView.Forms.MapObservers.TopViews


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// The various wall- and content-types that will be used to determine how
	/// to draw the wall- and content-blobs in TopView and RouteView.
	/// </summary>
	internal enum ContentType
	{
		Content,
		EastWall,
		SouthWall,
		NorthWall,
		WestWall,
		NorthwestSoutheast,
		NortheastSouthwest,
		NorthWallWindow,
		WestWallWindow,
		Ground,
		NorthFence,
		WestFence,
		NorthwestCorner,
		NortheastCorner,
		SouthwestCorner,
		SoutheastCorner
	}


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


	/// <summary>
	/// A class that determines how walls and objects are drawn for TopView and
	/// RouteView.
	/// </summary>
	internal static class ContentTypeService
	{
		#region Fields
		private static List<byte> _loftList;
		#endregion


		#region Methods
		/// <summary>
		/// Gets the ContentType of a given tile for drawing its blob in TopView
		/// and/or RouteView.
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		internal static ContentType GetContentType(TileBase tile)
		{
			var record = tile.Record;
			if (record != null)
			{
				_loftList = record.GetLoftList();

				if (IsGround())
					return ContentType.Ground;

				if (HasAllInNecessary(new[]{ 24, 26 }))
					return ContentType.EastWall;

				if (HasAllInNecessary(new[]{ 23, 25 }))
					return ContentType.SouthWall;

				if (HasAllInNecessary(new[]{ 8, 10, 12, 14, 38 })
					&& HasAnyInContingent(new[]{ 38 }))
				{
					return ContentType.NorthWallWindow;
				}

				if (HasAllInNecessary(new[]{ 0, 8, 10, 12, 14, 38, 39, 77 })
					&& HasAnyInContingent(new[]{ 0 }))
				{
					return ContentType.NorthFence;
				}

				if (HasAllInNecessary(new[]{ 8, 10, 12, 14, 16, 18, 20, 21 }))
					return ContentType.NorthWall;

				if (HasAllInNecessary(new[]{ 7, 9, 11, 13, 37 })
					&& HasAnyInContingent(new[]{ 37 }))
				{
					return ContentType.WestWallWindow;
				}

				if (HasAllInNecessary(new[]{ 0, 7, 9, 11, 13, 37, 39, 76 })
					&& HasAnyInContingent(new[]{ 0 }))
				{
					return ContentType.WestFence;
				}

				if (HasAllInNecessary(new[]{ 7, 9, 11, 13, 15, 17, 19, 22 }))
					return ContentType.WestWall;

				if (HasAllInNecessary(new[]{ 35 }))
					return ContentType.NorthwestSoutheast;

				if (HasAllInNecessary(new[]{ 36 }))
					return ContentType.NortheastSouthwest;

				if (HasAllInNecessary(new[]{ 39, 40, 41, 103 }))
					return ContentType.NorthwestCorner;

				if (HasAllInNecessary(new[]{ 100 }))
					return ContentType.NortheastCorner;

				if (HasAllInNecessary(new[]{ 106 }))
					return ContentType.SouthwestCorner;

				if (HasAllInNecessary(new[]{ 109 }))
					return ContentType.SoutheastCorner;
			}
			return ContentType.Content;
		}

		/// <summary>
		/// Checks if the tilepart is purely Floor-type.
		/// </summary>
		/// <returns></returns>
		private static bool IsGround()
		{
			int length = _loftList.Count;
			for (int id = 0; id != length; ++id)
			{
				switch (id)
				{
					case 0:
						if (_loftList[id] == 0)
							return false;
						break;

					default:
						if (_loftList[id] != 0)
							return false;
						break;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' are in 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool HasAllInNecessary(int[] necessary)
		{
			foreach (var loft in _loftList)
			{
				var valid = false;
				foreach (var gottfried in necessary)
					if (gottfried == loft)
					{
						valid = true;
						break;
					}

				if (!valid)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if any entry in '_loftList' is also in 'contingent'.
		/// </summary>
		/// <param name="contingent"></param>
		/// <returns></returns>
		private static bool HasAnyInContingent(int[] contingent)
		{
			foreach (var loft in _loftList)
				foreach (var gottfried in contingent)
					if (gottfried == loft)
						return true;

			return false;
		}

		/// <summary>
		/// Checks if a tilepart is either a standard door or a ufo door.
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		internal static bool IsDoor(TileBase tile)
		{
			var record = tile.Record;
			if (record != null
				&& (record.HumanDoor || record.UfoDoor))
			{
				return true;
			}
			return false;
		}
		#endregion
	}
}
