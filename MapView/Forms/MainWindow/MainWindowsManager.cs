using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	public class MainWindowsManager
	{
		public static IMainWindowsShowAllManager MainWindowsShowAllManager;
		public static MainToolStripButtonsFactory MainToolStripButtonsFactory;

		private static TopViewForm		_topView;
		private static TileViewForm		_tileView;
		private static RouteViewForm	_routeView;
		private static TopRouteViewForm	_topRouteView;
		private static HelpScreen		_helpScreen;
		private static AboutWindow		_aboutWindow;

		public static TopRouteViewForm TopRouteView
		{
			get { return _topRouteView ?? (_topRouteView = new TopRouteViewForm()); }
		}

		public static RouteViewForm RouteView
		{
			get { return _routeView ?? (_routeView = new RouteViewForm()); }
		}

		public static TopViewForm TopView
		{
			get { return _topView ?? (_topView = new TopViewForm()); }
		}

		public static TileViewForm TileView
		{
			get { return _tileView ?? (_tileView = new TileViewForm()); }
		}

		public static HelpScreen HelpScreen
		{
			get { return _helpScreen ?? (_helpScreen = new HelpScreen()); }
		}

		public static AboutWindow AboutWindow
		{
			get { return _aboutWindow ?? (_aboutWindow = new AboutWindow()); }
		}

		public static void Initialize()
		{
			TopRouteView.TopViewControl.Initialize(MainToolStripButtonsFactory);
			TopView.Control.Initialize(MainToolStripButtonsFactory);
			TileView.TileViewControl.Initialize(MainWindowsShowAllManager);
			TileView.TileViewControl.SelectedTileTypeChanged_view += _tileView_SelectedTileTypeChanged;
		}

		public void SetMap(IMap_Base map)
		{
			var maps = new IMap_Observer[]
			{
				TopRouteView.TopViewControl,
				TopRouteView.RouteViewControl,
				TileView.TileViewControl,
				RouteView.RouteViewControl,
				TopView.Control
			};

			foreach (var f in maps) // iterate all Forms/Views/Controls (take your pick.).
				if (f != null)
					SetMap(map, f);

			MapViewPanel.Instance.MapView.Refresh();

//			TopView.Refresh(); // TODO: fix TopView selector when loading or changing Maps.
//			TopRmpView.Refresh();
//			RmpView.Refresh();
		}

		private static void _tileView_SelectedTileTypeChanged(TileBase newTile)
		{
			if (newTile != null && newTile.Info != null)
				TopView.Control.SelectQuadrant(newTile.Info.TileType);
		}

		private void SetMap(IMap_Base newMap, IMap_Observer observer)
		{
			if (observer.Map != null)
			{
				observer.Map.HeightChanged -= observer.HeightChanged;
				observer.Map.SelectedTileChanged -= observer.SelectedTileChanged;
			}

			if ((observer.Map = newMap) != null)
			{
				newMap.HeightChanged += observer.HeightChanged;
				newMap.SelectedTileChanged += observer.SelectedTileChanged;
			}

			foreach (string key in observer.MoreObservers.Keys)
				SetMap(newMap, observer.MoreObservers[key]);
		}
	}
}
