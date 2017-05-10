using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// These are not actually "quadrants"; they are tile-part types. But that's
	/// the way this rolls.
	/// </summary>
	internal sealed class QuadrantPanel
		:
			MapObserverControl1
	{
		#region Fields & Properties
		private readonly QuadrantPanelDrawService _drawService = new QuadrantPanelDrawService();

		private XCMapTile _tile;
		private MapLocation _location;

		private QuadrantType _quadrant;
		internal QuadrantType SelectedQuadrant
		{
			get { return _quadrant; }
			set
			{
				_quadrant = value;
				Refresh();
			}
		}

		[Browsable(false)]
		internal Dictionary<string, SolidBrush> Brushes
		{
			set { _drawService.Brushes = value; }
		}

		[Browsable(false)]
		internal Dictionary<string, Pen> Pens
		{
			set { _drawService.Pens = value; }
		}

		[Browsable(false)]
		internal SolidBrush SelectColor
		{
			get { return _drawService.Brush; }
			set
			{
				_drawService.Brush = value;
				Refresh();
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal QuadrantPanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Globals.LoadExtras();
		}
		#endregion


		#region EventCalls
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLocationSelected_Observer(LocationSelectedEventArgs args)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("QuadrantPanel.OnLocationSelected_Observer");

			_tile = args.SelectedTile as XCMapTile;
			_location = args.Location;
			Refresh();
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLevelChanged_Observer(LevelChangedEventArgs args)
		{
			if (_location != null)
			{
				_tile = MapBase[_location.Row, _location.Col] as XCMapTile;
				_location.Lev = args.Level;
			}
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			var quad = (QuadrantType)((e.X - QuadrantPanelDrawService.StartX) / QuadrantPanelDrawService.QuadWidthTotal);
			switch (quad)
			{
				case QuadrantType.Ground:
				case QuadrantType.West:
				case QuadrantType.North:
				case QuadrantType.Content:
					SelectedQuadrant = quad;

					SetSelected(e.Button, e.Clicks);

					if (e.Button == MouseButtons.Right) // see SetSelected()
					{
						MainViewUnderlay.Instance.MainViewOverlay.Refresh();
						ViewerFormsManager.TopView.Refresh();
						ViewerFormsManager.RouteView.Refresh();
						ViewerFormsManager.TopRouteView.Refresh();
					}
					Refresh();
					break;
			}
		}

		/// <summary>
		/// Passes the draw-function on to QuadrantPanelDrawService.
		/// </summary>
		/// <param name="graphics"></param>
		protected override void RenderGraphics(Graphics graphics)
		{
			_drawService.Draw(graphics, _tile, SelectedQuadrant);
		}
		#endregion


		#region Methods
		internal void SetSelected(MouseButtons btn, int clicks)
		{
			if (_tile != null)
			{
				switch (btn)
				{
					case MouseButtons.Left:
						switch (clicks)
						{
							case 1:
								break;

							case 2:
								var tileView = ViewerFormsManager.TileView.Control;
								tileView.SelectedTile = _tile[SelectedQuadrant];
								break;
						}
						break;

					case MouseButtons.Right:
					{
						switch (clicks)
						{
							case 1:
								var tileView = ViewerFormsManager.TileView.Control;
								_tile[SelectedQuadrant] = tileView.SelectedTile;
								break;

							case 2:
								_tile[SelectedQuadrant] = null;
								break;
						}

						MapBase.MapChanged = true;
						Refresh();

						break;
					}
				}
			}
		}
		#endregion
	}
}
