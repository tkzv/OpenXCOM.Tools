using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers;

using XCom;


namespace MapView.Forms.MainWindow
{
	internal interface IMainViewsManager
	{
		void ManageViews();
		void CloseAllViews();
	}


	internal sealed class MainViewsManager
		:
			IMainViewsManager
	{
		private readonly SettingsManager _settingsManager;
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
				MainWindowsManager.RouteView.RouteViewControl.Settings;

			MainWindowsManager.TopRouteView.TopViewControl.LoadDefaultSettings();
			MainWindowsManager.TopRouteView.RouteViewControl.LoadDefaultSettings();

			SetViewToObserver(MainWindowsManager.TopView,      "Top View",   "TopView");
			SetViewToObserver(MainWindowsManager.RouteView,    "Route View", "RouteView");
			SetViewToObserver(MainWindowsManager.TopRouteView, "TopRoute View");
			SetViewToObserver(MainWindowsManager.TileView,     "Tile View",  "TileView");

			SetViewToObserver(_consoleShare.GetNewConsole(), "Console");

			SetViewToObserver(MainWindowsManager.HelpScreen,  "Quick Help");
			SetViewToObserver(MainWindowsManager.AboutWindow, "About");

			MainWindowsManager.TopRouteView.TopViewControl.RegistryInfo = // TODO: check if this should really be registered.
				MainWindowsManager.TopView.Control.RegistryInfo;

			MainWindowsManager.TopRouteView.RouteViewControl.RegistryInfo = // TODO: check if this should really be registered.
				MainWindowsManager.RouteView.RouteViewControl.RegistryInfo;
		}

		private void SetViewToObserver(Form f, string title, string regkey = null)
		{
			f.Text = title;

			var fObserver = f as IMapObserverFormProvider;
			if (fObserver != null)
			{
				var observerType0 = fObserver.MapObserver;
				observerType0.LoadDefaultSettings();

				observerType0.RegistryInfo = new DSShared.Windows.RegistryInfo(f, regkey);

				_settingsManager.Add(regkey, observerType0.Settings);
			}

//			f.ShowInTaskbar = false;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			_dictForms.Add(title, f);
		}

		public void CloseAllViews()
		{
			foreach (string key in _dictForms.Keys)
			{
				var f = _dictForms[key];
				f.WindowState = FormWindowState.Normal;

				f.Close();
			}
		}
	}
}
