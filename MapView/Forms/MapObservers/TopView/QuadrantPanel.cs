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
	/// These are not actually "quadrants"; they are tile-part types.
	/// </summary>
	internal sealed class QuadrantPanel
		:
			MapObserverControl1
	{
		#region Fields & Properties
		private readonly QuadrantPanelDrawService _drawService;

		private XCMapTile _mapTile;
		private MapLocation _mapLoc;

		private QuadrantType _selType;
		public QuadrantType SelectedQuadrant
		{
			get { return _selType; }
			set
			{
				_selType = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public Dictionary<string, SolidBrush> Brushes
		{
			set { _drawService.Brushes = value; }
		}

		[Browsable(false)]
		public Dictionary<string, Pen> Pens
		{
			set { _drawService.Pens = value; }
		}

		[Browsable(false)]
		public SolidBrush SelectColor
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
		public QuadrantPanel()
		{
			_mapTile = null;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Globals.LoadExtras();

			_drawService = new QuadrantPanelDrawService();
		}
		#endregion


		#region EventCalls
		public override void OnLocationChanged(IMapBase sender, LocationChangedEventArgs e)
		{
			_mapTile = e.SelectedTile as XCMapTile;
			_mapLoc  = e.Location;
			Refresh();
		}

		public override void OnLevelChanged(IMapBase sender, LevelChangedEventArgs e)
		{
			if (_mapLoc != null)
			{
				_mapTile = MapBase[_mapLoc.Row, _mapLoc.Col] as XCMapTile;
				_mapLoc.Lev = e.Level;
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

					if (e.Button == MouseButtons.Right) // see SetSelected() above^
					{
						MainViewUnderlay.Instance.MainView.Refresh();
						ViewerFormsManager.TopView.Refresh();
						ViewerFormsManager.RouteView.Refresh();
						ViewerFormsManager.TopRouteView.Refresh();
					}
					Refresh();
					break;
			}
		}

		protected override void RenderGraphics(Graphics backBuffer)
		{
			_drawService.Draw(backBuffer, _mapTile, SelectedQuadrant);
		}
		#endregion


		#region Methods
		public void SetSelected(MouseButtons btn, int clicks)
		{
			if (_mapTile != null)
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
								tileView.SelectedTile = _mapTile[SelectedQuadrant];
								break;
						}
						break;

					case MouseButtons.Right:
					{
						switch (clicks)
						{
							case 1:
								var tileView = ViewerFormsManager.TileView.Control;
								_mapTile[SelectedQuadrant] = tileView.SelectedTile;
								break;

							case 2:
								_mapTile[SelectedQuadrant] = null;
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
