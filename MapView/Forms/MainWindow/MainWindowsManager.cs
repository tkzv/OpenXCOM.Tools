using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainWindowsManager
	{
		internal static MainShowAllManager ShowAllManager;
		internal static EditButtonsFactory EditFactory;


		private static TopViewForm _topView;
		internal static TopViewForm TopView
		{
			get { return _topView ?? (_topView = new TopViewForm()); }
		}

		private static RouteViewForm _routeView;
		internal static RouteViewForm RouteView
		{
			get { return _routeView ?? (_routeView = new RouteViewForm()); }
		}

		private static TopRouteViewForm _topRouteView;
		internal static TopRouteViewForm TopRouteView
		{
			get { return _topRouteView ?? (_topRouteView = new TopRouteViewForm()); }
		}

		private static TileViewForm _tileView;
		internal static TileViewForm TileView
		{
			get { return _tileView ?? (_tileView = new TileViewForm()); }
		}

		private static Help _helpScreen;
		internal static Help HelpScreen
		{
			get { return _helpScreen ?? (_helpScreen = new Help()); }
		}

		private static About _aboutWindow;
		internal static About AboutScreen
		{
			get { return _aboutWindow ?? (_aboutWindow = new About()); }
		}


		internal static void Initialize()
		{
			TopRouteView.ControlTop.InitializeEditStrip(EditFactory);

			TopView.Control.InitializeEditStrip(EditFactory);

			TileView.Control.Initialize(ShowAllManager);
			TileView.Control.SelectedTileTypeChangedObserver += OnSelectedTileTypeChanged;
		}

		internal void SetObservers(IMapBase baseMap)
		{
			var observers = new IMapObserver[]
			{
				TopRouteView.ControlTop,
				TopRouteView.ControlRoute,
				TileView.Control,
				RouteView.Control,
				TopView.Control
			};

			foreach (var f in observers)
				if (f != null)
					SetObserver(baseMap, f);

			MainViewPanel.Instance.MainView.Refresh();
		}

		private void SetObserver(IMapBase baseMap, IMapObserver observer)
		{
			if (observer.BaseMap != null)
			{
				observer.BaseMap.HeightChanged -= observer.OnHeightChanged;
				observer.BaseMap.SelectedTileChanged -= observer.OnSelectedTileChanged;
			}

			if ((observer.BaseMap = baseMap) != null)
			{
				baseMap.HeightChanged += observer.OnHeightChanged;
				baseMap.SelectedTileChanged += observer.OnSelectedTileChanged;
			}

			foreach (string key in observer.MoreObservers.Keys) // ie. TopViewPanel and QuadrantsPanel
				SetObserver(baseMap, observer.MoreObservers[key]);
		}

		/// <summary>
		/// Changes the selected quadrant in TopView when a tilepart is selected
		/// in TileView.
		/// </summary>
		/// <param name="tile"></param>
		private static void OnSelectedTileTypeChanged(TileBase tile)
		{
			if (tile != null && tile.Record != null)
				TopView.Control.SelectQuadrant(tile.Record.TileType);
		}
	}
}
