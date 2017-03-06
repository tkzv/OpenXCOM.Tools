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
	public class BottomPanel
		:
		Map_Observer_Control
	{
		private XCMapTile _mapTile;
		private MapLocation _lastLoc;

		private readonly BottomPanelDrawService _drawService;
		private XCMapTile.MapQuadrant _selectedQuadrant;

//		public event EventHandler PanelClicked;


		public BottomPanel()
		{
			_mapTile = null;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
//			SelectedQuadrant = XCMapTile.MapQuadrant.Ground; // is done in the designer.

			Globals.LoadExtras();

			_drawService = new BottomPanelDrawService();
			_drawService.Brush = new SolidBrush(Color.FromArgb(204, 204, 255));
			_drawService.Font = new Font("Verdana", 7);
		}


		[Browsable(false)]
		public Dictionary<string, SolidBrush> Brushes
		{
			get { return _drawService.Brushes; }
			set { _drawService.Brushes = value; }
		}

		[Browsable(false)]
		public Dictionary<string, Pen> Pens
		{
			get { return _drawService.Pens; }
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

		public XCMapTile.MapQuadrant SelectedQuadrant
		{
			get { return _selectedQuadrant; }
			set
			{
				_selectedQuadrant = value;
				Refresh();
			}
		}

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
								var tileView = MainWindowsManager.TileView.TileViewControl;
								tileView.SelectedTile = _mapTile[SelectedQuadrant];
								break;
						}
						break;

					case MouseButtons.Right:
					{
						switch (clicks)
						{
							case 1:
								var tileView = MainWindowsManager.TileView.TileViewControl;
								_mapTile[SelectedQuadrant] = tileView.SelectedTile;
								break;

							case 2:
								_mapTile[SelectedQuadrant] = null;
								break;
						}
						break;
					}
				}
			}

			map.MapChanged = true;
			Refresh();
		}

		public override void HeightChanged(IMap_Base sender, HeightChangedEventArgs e)
		{
			_lastLoc.Height = e.NewHeight;
			_mapTile = map[_lastLoc.Row, _lastLoc.Col] as XCMapTile;
			Refresh();
		}

		public override void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
		{
			_mapTile = (XCMapTile)e.SelectedTile;
			_lastLoc = e.MapPosition;
			Refresh();
		}

//		private void visChanged(object sender, EventArgs e)
//		{
//			Refresh();
//		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			var quad = (e.X - BottomPanelDrawService.startX) / BottomPanelDrawService.TOTAL_QUADRANT_SPACE;
			if (quad > -1 && quad < 4)
			{
				SelectedQuadrant = (XCMapTile.MapQuadrant)quad;
				SetSelected(e.Button, e.Clicks);

//				if (PanelClicked != null)
//					PanelClicked(this, new EventArgs());

				Refresh();
				// TODO: Refresh *all* views (Top,Map,Rmp) if right-click.
				// See SetSelected() above.
			}
		}

		protected override void Render(Graphics g)
		{
			_drawService.Draw(g, _mapTile, SelectedQuadrant);
		}
	}
}
