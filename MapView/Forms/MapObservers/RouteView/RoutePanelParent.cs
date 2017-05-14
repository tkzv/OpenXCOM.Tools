using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// The base class for RoutePanel.
	/// </summary>
	internal class RoutePanelParent
		:
			UserControl
	{
		public event EventHandler<RoutePanelClickedEventArgs> RoutePanelClickedEvent;


		#region Fields & Properties
		private XCMapFile _mapFile;
		internal protected XCMapFile MapFile
		{
			get { return _mapFile; }
			set
			{
				_mapFile = value;
				OnResize(null);
			}
		}

		internal protected Point ClickPoint
		{ get; set; }

		/// <summary>
		/// The top-left point of the panel.
		/// </summary>
		internal protected Point Origin
		{ get; set; }

		private int _drawAreaWidth = 8;
		/// <summary>
		/// Half the horizontal width of a tile-lozenge.
		/// </summary>
		internal protected int DrawAreaWidth
		{
			get { return _drawAreaWidth; }
		}
		private int _drawAreaHeight = 4;
		/// <summary>
		/// Half the vertical height of a tile-lozenge.
		/// </summary>
		internal protected int DrawAreaHeight
		{
			get { return _drawAreaHeight; }
		}

		internal protected const int OffsetX = 2; // these track the offset between the panel border
		internal protected const int OffsetY = 2; // and the lozenge-tip.

		internal protected int _overCol = -1; // these track the location of the mouse-cursor
		internal protected int _overRow = -1; // NOTE: could be subsumed into 'RoutePanel.CursorPosition'

		internal protected int _selectedCol = -1;	// these track the currently clicked/selected location
		internal protected int _selectedRow = -1;	// NOTE: could be subsumed into 'ClickPoint' except that
													// these need to persist while the ClickPoint gets (-1,-1)
													// to clear the info-overlay.

		private readonly Dictionary<string, Pen> _pens = new Dictionary<string, Pen>();
		internal protected Dictionary<string, Pen> RoutePens
		{
			get { return _pens; }
		}
		private readonly Dictionary<string, SolidBrush> _brushes = new Dictionary<string, SolidBrush>();
		internal protected Dictionary<string, SolidBrush> RouteBrushes
		{
			get { return _brushes; }
		}

		private int _opacity = 255; // cf. RouteView.LoadControl0Settings()
		internal protected int Opacity
		{
			get { return _opacity; }
			set { _opacity = value.Clamp(0, 255); }
		}

		private bool _showOverlay = true; // cf. RouteView.LoadControl0Settings()
		internal protected bool ShowOverlay
		{
			get { return _showOverlay; }
			set { _showOverlay = value; }
		}

		private bool _showPriorityBars = true; // cf. RouteView.LoadControl0Settings()
		internal protected bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}
		#endregion


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


		#region EventCalls
		protected override void OnResize(EventArgs e)
		{
			if (MapFile != null)
			{
				int width  = Width  - OffsetX * 2;
				int height = Height - OffsetY * 2;

				if (height > width / 2) // use width
				{
					_drawAreaWidth = width / (MapFile.MapSize.Rows + MapFile.MapSize.Cols);

					if (_drawAreaWidth % 2 != 0)
						--_drawAreaWidth;

					_drawAreaHeight = _drawAreaWidth / 2;
				}
				else // use height
				{
					_drawAreaHeight = height / (MapFile.MapSize.Rows + MapFile.MapSize.Cols);
					_drawAreaWidth  = _drawAreaHeight * 2;
				}

				Origin = new Point( // offset the left and top edges to account for the 3d panel border
								OffsetX + MapFile.MapSize.Rows * _drawAreaWidth,
								OffsetY);
				Refresh();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (MapFile != null && RoutePanelClickedEvent != null)
			{
				var location = GetTileLocation(e.X, e.Y);
				if (location.X != -1)
				{
//					var tile = MapFile[loc.Y, loc.X];
//					if (tile != null) // this had better not be null ...
//					{
					_selectedCol = location.X;
					_selectedRow = location.Y;


					ClickPoint = location;

					MapFile.Location = new MapLocation(
													location.Y,
													location.X,
													MapFile.Level);

					MainViewUnderlay.Instance.MainViewOverlay.DragSelect(location, location);

					var args = new RoutePanelClickedEventArgs();
					args.MouseEventArgs  = e; // used only to get the Button by RouteView.OnRoutePanelClicked()

					args.ClickedTile     = MapFile[location.Y, location.X];
//					args.ClickedTile     = tile;

					args.ClickedLocation = MapFile.Location; // WARNING: keep an eye on that. Ie, don't let 'args' change 'MapFile.Location' wantonly.
//					args.ClickedLocation = new MapLocation(
//														loc.Y,
//														loc.X,
//														MapFile.Level);
					RoutePanelClickedEvent(this, args);

					Refresh();
				}
//				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{															// For some whack reason, this is needed in order to refresh
			MainViewUnderlay.Instance.MainViewOverlay.Refresh();	// MainView's selector iff a Map has just been loaded *and*
		}															// RouteView is clicked at location (0,0).

		/// <summary>
		/// Tracks x/y location for the mouseover lozenge.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e); // required to fire RouteView.OnRoutePanelMouseMove()

			var location = GetTileLocation(e.X, e.Y);
			if (location.X != _overCol || location.Y != _overRow)
			{
				_overCol = location.X;
				_overRow = location.Y;

				Refresh(); // 3nd mouseover refresh for RouteView. See RouteView.OnRoutePanelMouseMove(), RouteView.OnRoutePanelMouseLeave()
			}
		}
		#endregion


		#region Methods
		internal protected void ClearClickPoint()
		{
			ClickPoint = new Point(-1, -1);
		}

		/// <summary>
		/// Gets the tile contained at (x,y) in local screen coordinates.
		/// </summary>
		/// <param name="x">the x-position of the mouse-cursor</param>
		/// <param name="y">the y-position of the mouse-cursor</param>
		/// <returns>null if (x,y) is an invalid location for a tile</returns>
		internal protected XCMapTile GetTile(int x, int y)
		{
			var loc = GetTileLocation(x, y);
			return (loc.X != -1) ? MapFile[loc.Y, loc.X] as XCMapTile
								 : null;
		}

		/// <summary>
		/// Converts a position from screen-coordinates to tile-location.
		/// </summary>
		/// <param name="x">the x-position of the mouse-cursor</param>
		/// <param name="y">the y-position of the mouse-cursor</param>
		/// <returns></returns>
		internal protected Point GetTileLocation(int x, int y)
		{
			if (MapFile != null)
			{
				x -= Origin.X;
				y -= Origin.Y;

				double xd = (double)x / (_drawAreaWidth  * 2)
						  + (double)y / (_drawAreaHeight * 2);
				double yd = ((double)y * 2 - x) / (_drawAreaWidth * 2);

				var point = new Point(
									(int)Math.Floor(xd),
									(int)Math.Floor(yd));

				if (   point.Y > -1 && point.Y < MapFile.MapSize.Rows
					&& point.X > -1 && point.X < MapFile.MapSize.Cols)
				{
					return point;
				}
			}
			return new Point(-1, -1);
		}
		#endregion
	}
}
