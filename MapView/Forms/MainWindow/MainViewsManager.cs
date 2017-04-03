using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers;

using XCom;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainViewsManager
	{
		private readonly SettingsManager    _settingsManager;
		private readonly ConsoleSharedSpace _consoleShare;

		private readonly Dictionary<string, Form> _dictForms;


		public MainViewsManager(
				SettingsManager settingsManager,
				ConsoleSharedSpace consoleShare)
		{
			_settingsManager = settingsManager;
			_consoleShare = consoleShare;
			_dictForms = new Dictionary<string, Form>();
		}


		public void ManageViews()
		{
			MainWindowsManager.TopRouteView.TopViewControl.Settings =
				MainWindowsManager.TopView.Control.Settings;

			MainWindowsManager.TopRouteView.RouteViewControl.Settings =
				MainWindowsManager.RouteView.Control.Settings;

			MainWindowsManager.TopRouteView.TopViewControl.LoadDefaultSettings();
			MainWindowsManager.TopRouteView.RouteViewControl.LoadDefaultSettings();

			SetViewToObserver(MainWindowsManager.TopView,      "Top View",      "TopView");
			SetViewToObserver(MainWindowsManager.RouteView,    "Route View",    "RouteView");
			SetViewToObserver(MainWindowsManager.TopRouteView, "TopRoute View", "TopRouteView");
			SetViewToObserver(MainWindowsManager.TileView,     "Tile View",     "TileView");

			SetViewToObserver(_consoleShare.GetNewConsole(), "Console", "Console");

			SetViewToObserver(MainWindowsManager.HelpScreen,  "Quick Help");
			SetViewToObserver(MainWindowsManager.AboutWindow, "About");

			MainWindowsManager.TopRouteView.TopViewControl.RegistryInfo =
				MainWindowsManager.TopView.Control.RegistryInfo;

			MainWindowsManager.TopRouteView.RouteViewControl.RegistryInfo =
				MainWindowsManager.RouteView.Control.RegistryInfo;
		}

		private void SetViewToObserver(Form f, string caption, string regkey = null)
		{
			f.Text = caption;

			var fObserver = f as IMapObserverFormProvider;
			if (fObserver != null)
			{
				var observerType0 = fObserver.MapObserver;
				observerType0.LoadDefaultSettings();

				observerType0.RegistryInfo = new DSShared.Windows.RegistryInfo(f, regkey);

				_settingsManager.Add(regkey, observerType0.Settings);
			}

//			f.ShowInTaskbar = false;
//			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			_dictForms.Add(caption, f);
		}

		public void CloseAllViews()
		{
			foreach (string key in _dictForms.Keys)
			{
				var f = _dictForms[key];
//				f.WindowState = FormWindowState.Normal;

				f.Close();
			}
		}
	}
}
