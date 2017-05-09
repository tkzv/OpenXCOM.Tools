using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	internal sealed class ViewerFormsManager
	{
		internal static ShowHideManager HideViewersManager;
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

			TileView.Control.SetShowHideManager(HideViewersManager);
			TileView.Control.TileSelectedEvent_Observer0 += OnTileSelected_Observer0;
		}

		/// <summary>
		/// Changes the selected quadrant in the QuadrantPanel when a tilepart
		/// is selected in TileView.
		/// </summary>
		/// <param name="tile"></param>
		private static void OnTileSelected_Observer0(TileBase tile)
		{
			if (tile != null && tile.Record != null)
				TopView.Control.SelectQuadrant(tile.Record.TileType);
		}

		internal void SetObservers(XCMapBase mapBase)
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
					SetObserver(mapBase, f);

			MainViewUnderlay.Instance.MainViewOverlay.Refresh();
		}

		private void SetObserver(XCMapBase mapBase, IMapObserver observer)
		{
			if (observer.MapBase != null)
			{
				observer.MapBase.LocationSelectedEvent -= observer.OnLocationSelected_Observer;
				observer.MapBase.LevelChangedEvent     -= observer.OnLevelChanged_Observer;
			}

			if ((observer.MapBase = mapBase) != null)
			{
				mapBase.LocationSelectedEvent += observer.OnLocationSelected_Observer;
				mapBase.LevelChangedEvent     += observer.OnLevelChanged_Observer;
			}

			foreach (string key in observer.MoreObservers.Keys) // ie. TopViewPanel and QuadrantsPanel
				SetObserver(mapBase, observer.MoreObservers[key]);
		}
	}
}
