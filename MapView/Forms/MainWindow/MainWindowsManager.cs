using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainWindowsManager
	{
		public static IMainShowAllManager MainShowAllManager;
		public static EditButtonsFactory EditButtonsFactory;


		private static TopViewForm      _topView;
		private static TileViewForm     _tileView;
		private static RouteViewForm    _routeView;
		private static TopRouteViewForm _topRouteView;

		private static HelpScreen       _helpScreen;
		private static AboutWindow      _aboutWindow;


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
			TopRouteView.TopViewControl.Initialize(EditButtonsFactory);
			TopView.Control.Initialize(EditButtonsFactory);
			TileView.TileViewControl.Initialize(MainShowAllManager);
			TileView.TileViewControl.SelectedTileTypeChangedObserver += SelectedTileTypeChanged;
		}

		public void SetMap(IMap_Base baseMap)
		{
			var observers = new IMap_Observer[]
			{
				TopRouteView.TopViewControl,
				TopRouteView.RouteViewControl,
				TileView.TileViewControl,
				RouteView.RouteViewControl,
				TopView.Control
			};

			foreach (var f in observers) // iterate all Forms/Views/Controls (take your pick.).
				if (f != null)
					SetMap(baseMap, f);

			MapViewPanel.Instance.MapView.Refresh();

//			TopView.Refresh(); // TODO: fix TopView selector when loading or changing Maps.
//			TopRmpView.Refresh();
//			RmpView.Refresh();
		}

		private void SetMap(IMap_Base baseMap, IMap_Observer observer)
		{
			if (observer.Map != null)
			{
				observer.Map.HeightChanged -= observer.HeightChanged;
				observer.Map.SelectedTileChanged -= observer.SelectedTileChanged;
			}

			if ((observer.Map = baseMap) != null)
			{
				baseMap.HeightChanged += observer.HeightChanged;
				baseMap.SelectedTileChanged += observer.SelectedTileChanged;
			}

			foreach (string key in observer.MoreObservers.Keys)
				SetMap(baseMap, observer.MoreObservers[key]);
		}

		/// <summary>
		/// Changes the selected quadrant in TopView when a tilepart is selected
		/// in TileView.
		/// </summary>
		/// <param name="tile"></param>
		private static void SelectedTileTypeChanged(TileBase tile)
		{
			if (tile != null && tile.Info != null)
				TopView.Control.SelectQuadrant(tile.Info.TileType);
		}
	}
}
