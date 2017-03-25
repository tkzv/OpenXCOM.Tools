using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
//	public delegate void MapPanelClickDelegate(object sender, MapPanelClickEventArgs e);


	public class MapPanel
		:
		UserControl
	{
		public event EventHandler<MapPanelClickEventArgs> MapPanelClicked;
//		public event MapPanelClickDelegate MapPanelClicked;


		private XCMapFile _mapFile;

		private Point _clickPoint;
		protected Point ClickPoint
		{
			get { return _clickPoint; }
			set { _clickPoint = value; }
		}

		private Point _origin;
		protected Point Origin
		{
			get { return _origin; }
			set { _origin = value; }
		}

		private int _drawAreaWidth = 8;
		protected int DrawAreaWidth
		{
			get { return _drawAreaWidth; }
			set { _drawAreaWidth = value; }
		}
		private int _drawAreaHeight = 4;
		protected int DrawAreaHeight
		{
			get { return _drawAreaHeight; }
			set { _drawAreaHeight = value; }
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


		public MapPanel()
		{
			_mapPens    = new Dictionary<string, Pen>();
			_mapBrushes = new Dictionary<string, SolidBrush>();

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
		}


		public XCMapFile MapFile
		{
			get { return _mapFile; }
			set
			{
				_mapFile = value;
				OnResize(null);
			}
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
			_clickPoint = new Point(-1, -1);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_mapFile != null)
			{
				if (MapPanelClicked != null)
				{
					var pt = ConvertCoordsDiamond(e.X, e.Y);
					if (   pt.Y >= 0 && pt.Y < _mapFile.MapSize.Rows
						&& pt.X >= 0 && pt.X < _mapFile.MapSize.Cols)
					{
						var tile = _mapFile[pt.Y, pt.X];
						if (tile != null)
						{
							_clickPoint = pt;

							_mapFile.SelectedTile = new MapLocation(
															_clickPoint.Y,
															_clickPoint.X,
															_mapFile.CurrentHeight);

							MapViewPanel.Instance.MapView.SetDrag(pt, pt);

							var args = new MapPanelClickEventArgs();
							args.ClickTile = tile;
							args.MouseEventArgs = e;
							args.ClickLocation = new MapLocation(
															_clickPoint.Y,
															_clickPoint.X,
															_mapFile.CurrentHeight);
							MapPanelClicked(this, args);

							Refresh();
						}
//						else return;
					}
				}
//				Refresh(); // moved up^
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
						_drawAreaWidth--;

					_drawAreaHeight = _drawAreaWidth / 2;
				}
				else // use height
				{
					_drawAreaHeight = Height / (_mapFile.MapSize.Rows + _mapFile.MapSize.Cols);
					_drawAreaWidth  = _drawAreaHeight * 2;
				}

				_origin = new Point(_mapFile.MapSize.Rows * _drawAreaWidth, 0);
				Refresh();
			}
		}

		private Point ConvertCoordsDiamond(int ptX, int ptY)
		{
			int x = ptX - _origin.X;
			int y = ptY - _origin.Y;

			double x1 = ((double)x / (_drawAreaWidth  * 2))
					  + ((double)y / (_drawAreaHeight * 2));
			double x2 = -((double)x - (double)y * 2) / (_drawAreaWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(x2));
		}
	}
}
