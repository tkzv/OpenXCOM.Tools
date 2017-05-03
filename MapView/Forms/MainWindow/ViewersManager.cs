using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers;

using XCom;


namespace MapView.Forms.MainWindow
{
	internal sealed class ViewersManager
	{
		private readonly Dictionary<string, Form> _viewersDictionary = new Dictionary<string, Form>();

		private readonly SettingsManager    _settingsManager;
		private readonly ConsoleSharedSpace _consoleShare;


		internal ViewersManager(
				SettingsManager settingsManager,
				ConsoleSharedSpace consoleShare)
		{
			_settingsManager = settingsManager;
			_consoleShare = consoleShare;
		}


		internal void ManageViewers()
		{
			//LogFile.WriteLine("ManageViewers");
			ViewerFormsManager.TopRouteView.ControlTop.Settings   = ViewerFormsManager.TopView.Control.Settings;
			ViewerFormsManager.TopRouteView.ControlRoute.Settings = ViewerFormsManager.RouteView.Control.Settings;

			ViewerFormsManager.TopRouteView.ControlTop.LoadControl0Settings();
			ViewerFormsManager.TopRouteView.ControlRoute.LoadControl0Settings();


			AddViewer("Top View",    ViewerFormsManager.TopView);
			SetAsObserver("TopView", ViewerFormsManager.TopView);

			AddViewer("Route View",    ViewerFormsManager.RouteView);
			SetAsObserver("RouteView", ViewerFormsManager.RouteView);

			AddViewer("Tile View",    ViewerFormsManager.TileView);
			SetAsObserver("TileView", ViewerFormsManager.TileView);

			AddViewer("TopRoute View", ViewerFormsManager.TopRouteView);

			AddViewer("Console", _consoleShare.GetConsole());

			AddViewer("Help",  ViewerFormsManager.HelpScreen);
			AddViewer("About", ViewerFormsManager.AboutScreen);


//			SetAsObserver(MainWindowsManager.TopView,      "Top View",      "TopView");
//			SetAsObserver(MainWindowsManager.RouteView,    "Route View",    "RouteView");
//			SetAsObserver(MainWindowsManager.TopRouteView, "TopRoute View", "TopRouteView");
//			SetAsObserver(MainWindowsManager.TileView,     "Tile View",     "TileView");
//			SetAsObserver(_consoleShare.GetNewConsole(),   "Console",       "Console");
//			SetAsObserver(MainWindowsManager.HelpScreen,   "Help");
//			SetAsObserver(MainWindowsManager.AboutWindow,  "About");

//			MainWindowsManager.TopRouteView.ControlTop.RegistryInfo   = MainWindowsManager.TopView.Control.RegistryInfo;
//			MainWindowsManager.TopRouteView.ControlRoute.RegistryInfo = MainWindowsManager.RouteView.Control.RegistryInfo;
		}

		private void AddViewer(string caption, Form f)
		{
			//LogFile.WriteLine("AddViewer caption= " + caption);
			_viewersDictionary.Add(caption, f);

//			f.Text = caption;
//			f.ShowInTaskbar = false;
//			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
		}

		private void SetAsObserver(string regkey, Form f)
		{
			//LogFile.WriteLine("SetAsObserver regkey= " + regkey);
			var fobserver = f as IMapObserverProvider; // TopViewForm, RouteViewForm, TileViewForm only.
			if (fobserver != null)
			{
				//LogFile.WriteLine(". is IMapObserverProvider");
				var fcontrol = fobserver.ObserverControl0; // ie. TopView, RouteView, TileView.
				fcontrol.LoadControl0Settings();

				var regInfo = new DSShared.Windows.RegistryInfo(f, regkey); // subscribe to Load and Closing events.

				_settingsManager.Add(regkey, fcontrol.Settings);
			}
		}
/*		private void SetAsObserver(Form f, string caption, string regkey = null)
		{
			f.Text = caption;
			var fObserver = f as IMapObserverProvider;
			if (fObserver != null)
			{
				var observerType0 = fObserver.MapObserver;
				observerType0.LoadControl0Settings();
//				observerType0.RegistryInfo = new DSShared.Windows.RegistryInfo(f, regkey); // subscribe to Load and Closing events.
				var regInfo = new DSShared.Windows.RegistryInfo(f, regkey); // subscribe to Load and Closing events.
				_settingsManager.Add(regkey, observerType0.Settings);
			}
//			f.ShowInTaskbar = false;
//			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			_viewersDictionary.Add(caption, f);
		} */

		/// <summary>
		/// Closes the following viewers: TopView, RouteView, TopRouteView,
		/// TileView, Console, Help, About.
		/// </summary>
		internal void CloseSubsidiaryViewers()
		{
			foreach (string key in _viewersDictionary.Keys)
			{
				var f = _viewersDictionary[key];
				f.WindowState = FormWindowState.Normal;

				f.Close();
			}
		}
	}
}
