using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal class RoutePanelBase
		:
			UserControl
	{
		public event EventHandler<RoutePanelClickedEventArgs> RoutePanelClickedEvent;


		private XCMapFile _mapFile;
		public XCMapFile MapFile
		{
			get { return _mapFile; }
			set
			{
				_mapFile = value;
				OnResize(null);
			}
		}

		protected Point ClickPoint
		{ get; set; }

		protected Point Origin
		{ get; set; }

		private int _drawAreaWidth = 8;
		protected int DrawAreaWidth
		{
			get { return _drawAreaWidth; }
		}
		private int _drawAreaHeight = 4;
		protected int DrawAreaHeight
		{
			get { return _drawAreaHeight; }
		}

		private readonly Dictionary<string, Pen> _mapPens;
		public Dictionary<string, Pen> MapPens
		{
			get { return _mapPens; }
		}
		private readonly Dictionary<string, SolidBrush> _mapBrushes;
		public Dictionary<string, SolidBrush> MapBrushes
		{
			get { return _mapBrushes; }
		}


		public RoutePanelBase()
		{
			_mapPens    = new Dictionary<string, Pen>();
			_mapBrushes = new Dictionary<string, SolidBrush>();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
		}


		/// <summary>
		/// Get the tile contained at (x,y) in local screen coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>null if (x,y) is an invalid location for a tile</returns>
		public XCMapTile GetTile(int x, int y)
		{
			if (_mapFile != null)
			{
				Point p = ConvertCoordsDiamond(x, y);
				if (   p.Y >= 0 && p.Y < _mapFile.MapSize.Rows
					&& p.X >= 0 && p.X < _mapFile.MapSize.Cols)
				{
					return (XCMapTile)_mapFile[p.Y, p.X];
				}
			}
			return null;
		}

		public Point GetTileCoordinates(int x, int y)
		{
			Point pt = ConvertCoordsDiamond(x, y);
			if (   pt.Y >= 0 && pt.Y < _mapFile.MapSize.Rows
				&& pt.X >= 0 && pt.X < _mapFile.MapSize.Cols)
			{
				return pt;
			}
			return new Point(-1, -1);
		}

		public void DeselectLocation()
		{
			ClickPoint = new Point(-1, -1);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_mapFile != null && RoutePanelClickedEvent != null)
			{
				var pt = ConvertCoordsDiamond(e.X, e.Y);
				if (   pt.Y >= 0 && pt.Y < _mapFile.MapSize.Rows
					&& pt.X >= 0 && pt.X < _mapFile.MapSize.Cols)
				{
					var tile = _mapFile[pt.Y, pt.X];
					if (tile != null)
					{
						ClickPoint = pt;

						_mapFile.SelectedTile = new MapLocation(
															ClickPoint.Y,
															ClickPoint.X,
															_mapFile.CurrentHeight);

						MainViewUnderlay.Instance.MainView.SetDrag(pt, pt);

						var args = new RoutePanelClickedEventArgs();
						args.ClickTile = tile;
						args.MouseEventArgs = e;
						args.ClickLocation = new MapLocation(
														ClickPoint.Y,
														ClickPoint.X,
														_mapFile.CurrentHeight);
						RoutePanelClickedEvent(this, args);

						Refresh();
					}
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			if (_mapFile != null)
			{
				if (Height > Width / 2) // use width
				{
					_drawAreaWidth = Width / (_mapFile.MapSize.Rows + _mapFile.MapSize.Cols + 1);

					if (_drawAreaWidth % 2 != 0)
						--_drawAreaWidth;

					_drawAreaHeight = _drawAreaWidth / 2;
				}
				else // use height
				{
					_drawAreaHeight = Height / (_mapFile.MapSize.Rows + _mapFile.MapSize.Cols);
					_drawAreaWidth  = _drawAreaHeight * 2;
				}

				Origin = new Point(_mapFile.MapSize.Rows * _drawAreaWidth, 0);
				Refresh();
			}
		}

		private Point ConvertCoordsDiamond(int ptX, int ptY)
		{
			int x = ptX - Origin.X;
			int y = ptY - Origin.Y;

			double x1 = ((double)x / (_drawAreaWidth  * 2))
					  + ((double)y / (_drawAreaHeight * 2));
			double x2 = -((double)x - (double)y * 2) / (_drawAreaWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(x2));
		}
	}
}
