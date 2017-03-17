using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	public delegate void MapPanelClickDelegate(object sender, MapPanelClickEventArgs e);


	public class MapPanel
		:
		UserControl
	{
		private XCMapFile _mapFile;

		public Point ClickPoint;
		protected Point Origin;

		protected int DrawAreaWidth  = 8;
		protected int DrawAreaHeight = 4;

		public event MapPanelClickDelegate MapPanelClicked;

		public Dictionary<string, Pen> MapPens;
		public Dictionary<string, SolidBrush> MapBrushes;


		public MapPanel()
		{
			MapPens    = new Dictionary<string, Pen>();
			MapBrushes = new Dictionary<string, SolidBrush>();

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

		public void ClearSelected()
		{
			ClickPoint = new Point(-1, -1);
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
							ClickPoint = pt;

							_mapFile.SelectedTile = new MapLocation(
															ClickPoint.Y,
															ClickPoint.X,
															_mapFile.CurrentHeight);

							MapViewPanel.Instance.MapView.SetDrag(pt, pt);

							var args = new MapPanelClickEventArgs();
							args.ClickTile = tile;
							args.MouseEventArgs = e;
							args.ClickLocation = new MapLocation(
															ClickPoint.Y,
															ClickPoint.X,
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
					DrawAreaWidth = Width / (_mapFile.MapSize.Rows + _mapFile.MapSize.Cols + 1);

					if (DrawAreaWidth % 2 != 0)
						DrawAreaWidth--;

					DrawAreaHeight = DrawAreaWidth / 2;
				}
				else // use height
				{
					DrawAreaHeight = Height / (_mapFile.MapSize.Rows + _mapFile.MapSize.Cols);
					DrawAreaWidth  = DrawAreaHeight * 2;
				}

				Origin = new Point(_mapFile.MapSize.Rows * DrawAreaWidth, 0);
				Refresh();
			}
		}

		private Point ConvertCoordsDiamond(int ptX, int ptY)
		{
			int x = ptX - Origin.X;
			int y = ptY - Origin.Y;

			double x1 = ((double)x / (DrawAreaWidth  * 2))
					  + ((double)y / (DrawAreaHeight * 2));
			double x2 = -((double)x - (double)y * 2) / (DrawAreaWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(x2));
		}
	}
}
