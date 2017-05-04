using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// A base class for RoutePanel.
	/// </summary>
	internal class RoutePanelParent
		:
			UserControl
	{
		public event EventHandler<RoutePanelClickedEventArgs> RoutePanelClickedEvent;


		private XCMapFile _mapFile;
		internal XCMapFile MapFile
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

		/// <summary>
		/// The top-left point of the panel.
		/// </summary>
		protected Point Origin
		{ get; set; }

		private int _drawAreaWidth = 8;
		/// <summary>
		/// Half the horizontal width of a tile-lozenge.
		/// </summary>
		protected int DrawAreaWidth
		{
			get { return _drawAreaWidth; }
		}
		private int _drawAreaHeight = 4;
		/// <summary>
		/// Half the vertical height of a tile-lozenge.
		/// </summary>
		protected int DrawAreaHeight
		{
			get { return _drawAreaHeight; }
		}

		private readonly Dictionary<string, Pen> _pens = new Dictionary<string, Pen>();
		internal Dictionary<string, Pen> RoutePens
		{
			get { return _pens; }
		}
		private readonly Dictionary<string, SolidBrush> _brushes = new Dictionary<string, SolidBrush>();
		internal Dictionary<string, SolidBrush> RouteBrushes
		{
			get { return _brushes; }
		}

		private int _opacity = 255; // cf. RouteView.LoadControl0Settings()
		internal int Opacity
		{
			get { return _opacity; }
			set { _opacity = value.Clamp(0, 255); }
		}

		private bool _showOverlay = true; // cf. RouteView.LoadControl0Settings()
		internal bool ShowOverlay
		{
			get { return _showOverlay; }
			set { _showOverlay = value; }
		}

		private bool _showPriorityBars = true; // cf. RouteView.LoadControl0Settings()
		internal bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of RoutePanel.
		/// </summary>
		internal protected RoutePanelParent()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
		}
		#endregion


		/// <summary>
		/// Gets the tile contained at (x,y) in local screen coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>null if (x,y) is an invalid location for a tile</returns>
		internal protected XCMapTile GetTile(int x, int y)	// question: why can RouteView access this
		{													// it's 'protected'
			if (_mapFile != null)
			{
				Point pt = ConvertCoordsDiamond(x, y);
				if (   pt.Y >= 0 && pt.Y < _mapFile.MapSize.Rows
					&& pt.X >= 0 && pt.X < _mapFile.MapSize.Cols)
				{
					return (XCMapTile)_mapFile[pt.Y, pt.X];
				}
			}
			return null;
		}

		internal protected Point GetTileCoordinates(int x, int y)
		{
			Point pt = ConvertCoordsDiamond(x, y);
			if (   pt.Y >= 0 && pt.Y < _mapFile.MapSize.Rows
				&& pt.X >= 0 && pt.X < _mapFile.MapSize.Cols)
			{
				return pt;
			}
			return new Point(-1, -1);
		}

		internal protected void ClearClickPoint()
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

						_mapFile.Location = new MapLocation(
														ClickPoint.Y,
														ClickPoint.X,
														_mapFile.Level);

						MainViewUnderlay.Instance.MainViewOverlay.SetDrag(pt, pt);

						var args = new RoutePanelClickedEventArgs();
						args.MouseEventArgs = e;
						args.ClickTile = tile;
						args.ClickLocation = new MapLocation(
														ClickPoint.Y,
														ClickPoint.X,
														_mapFile.Level);
						RoutePanelClickedEvent(this, args);

						Refresh();
					}
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			MainViewUnderlay.Instance.MainViewOverlay.Refresh();	// For some whack reason, this is needed in order to refresh
		}															// MainView's selector iff a Map has just been loaded *and*
																	// RouteView is clicked at location (0,0).

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
