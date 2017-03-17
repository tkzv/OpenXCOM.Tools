using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers;

using XCom;


namespace MapView.Forms.MainWindow
{
	public interface IMainWindowWindowsManager
	{
		void Register();
		void CloseAll();
	}

	public class MainWindowWindowsManager
		:
		IMainWindowWindowsManager
	{
		private readonly Dictionary<string, Form> _registeredForms;
		private readonly SettingsManager _settingsManager;
		private readonly ConsoleSharedSpace _consoleShare;


		public MainWindowWindowsManager(
				SettingsManager settingsManager,
				ConsoleSharedSpace consoleShare)
		{
			_settingsManager = settingsManager;
			_consoleShare = consoleShare;
			_registeredForms = new Dictionary<string, Form>();
		}


		public void Register()
		{
			MainWindowsManager.TopRouteView.TopViewControl.Settings =
				MainWindowsManager.TopView.Control.Settings;

			MainWindowsManager.TopRouteView.RouteViewControl.Settings =
				MainWindowsManager.RouteView.RouteViewControl.Settings;

			MainWindowsManager.TopRouteView.TopViewControl.LoadDefaultSettings();
			MainWindowsManager.TopRouteView.RouteViewControl.LoadDefaultSettings();

			RegisterForm(MainWindowsManager.TopView,	"Top View",			"TopView");
			RegisterForm(MainWindowsManager.RouteView,	"Route View",		"RmpView");
			RegisterForm(MainWindowsManager.TopRouteView,	"Top-Route View");
			RegisterForm(MainWindowsManager.TileView,	"Tile View",		"TileView");

			RegisterForm(_consoleShare.GetNewConsole(), "Console");

			RegisterForm(MainWindowsManager.HelpScreen, "Quick Help");
			RegisterForm(MainWindowsManager.AboutWindow, "About");

			MainWindowsManager.TopRouteView.TopViewControl.RegistryInfo = // TODO: check if this should really be registered.
				MainWindowsManager.TopView.Control.RegistryInfo;

			MainWindowsManager.TopRouteView.RouteViewControl.RegistryInfo = // TODO: check if this should really be registered.
				MainWindowsManager.RouteView.RouteViewControl.RegistryInfo;
		}

		private void RegisterForm(Form f, string title, string regkey = null)
		{
			f.Text = title;

			var observerForm = f as IMapObserverFormProvider;
			if (observerForm != null)
			{
				var observer = observerForm.MapObserver;
				observer.LoadDefaultSettings();
				observer.RegistryInfo = new DSShared.Windows.RegistryInfo(f, regkey);
				_settingsManager.Add(regkey, observer.Settings);
			}

			f.ShowInTaskbar = false;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			_registeredForms.Add(title, f);
		}

		public void CloseAll()
		{
			foreach (string key in _registeredForms.Keys)
			{
				var f = _registeredForms[key];
				f.WindowState = FormWindowState.Normal;

				f.Close();
			}
		}
	}
}
