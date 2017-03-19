using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MainWindow
{
	public class WindowMenuManager
	{
		private readonly MenuItem _show;
		private readonly MenuItem _help;

		private readonly List<MenuItem>	_allItems = new List<MenuItem>();
		private readonly List<Form>		_allForms = new List<Form>();

		private Settings _settings;

		private bool _disposed;


		public WindowMenuManager(MenuItem show, MenuItem help)
		{
			_show = show;
			_help = help;
		}


		public void SetMenus(ConsoleForm consoleWindow, Settings settings) // NOTE: has nothing to do with the Registry.
		{
			_settings = settings;

			RegisterForm(MainWindowsManager.TileView,		"Tile View",		_show);//, "TileView");
			_show.MenuItems.Add(new MenuItem("-"));

			RegisterForm(MainWindowsManager.TopView,		"Top View",			_show);//, "TopView");
			RegisterForm(MainWindowsManager.RouteView,		"Route View",		_show);//, "RmpView");
			RegisterForm(MainWindowsManager.TopRouteView,	"TopRoute View",	_show);
			_show.MenuItems.Add(new MenuItem("-"));

			RegisterForm(consoleWindow,						"Console",			_show);


			RegisterForm(MainWindowsManager.HelpScreen,		"Quick Help",		_help);
			RegisterForm(MainWindowsManager.AboutWindow,	"About",			_help);

			RegisterWindowMenuItemValue();
		}

		public void LoadState()
		{
			foreach (MenuItem item in _show.MenuItems)
			{
				var label = GetWindowSettingName(item);
				if (_settings[label].ValueBool)
				{
					item.PerformClick();
				}
				else
				{
					item.PerformClick();
					item.PerformClick();
				}
			}
/*			foreach (MenuItem item in _show.MenuItems)	// NOTE: Don't do this. Go figure.
			{											// All the View-Panels will load ...
				item.PerformClick();					// regardless of their saved settings.

				var label = GetWindowSettingName(item);
				if (!(_settings[label].ValueBool))
					item.PerformClick();
			} */
			_show.Enabled = true;
		}

		public IMainWindowsShowAllManager CreateShowAll()
		{
			return new MainWindowsShowAllManager(_allForms, _allItems);
		}

		private void RegisterWindowMenuItemValue() // NOTE: has nothing to do with the Registry.
		{
			foreach (MenuItem item in _show.MenuItems)
			{
				var label = GetWindowSettingName(item);

				_settings.AddSetting(
								label,
								!(item.Tag is TopViewForm) && !(item.Tag is RouteViewForm),
								"Default display window - " + item.Text,
								"Windows",
								null,
								false,
								null);

				var f = item.Tag as Form;
				if (f != null)
				{
					f.VisibleChanged += (sender, a) =>
					{
						if (_disposed)
							return;

						var senderForm = sender as Form;
						if (senderForm == null)
							return;

						_settings[label].Value = senderForm.Visible;
					};
				}
			}
		}

		private void RegisterForm( // NOTE: has nothing to do with the Registry.
				Form f,
				string caption,
				Menu parent)
//				string registryKey = null)
		{
			f.Text = caption;

			var item = new MenuItem(caption);
			item.Tag = f;

			parent.MenuItems.Add(item);

			item.Click += FormItemClick;

			f.Closing += (sender, e) =>
			{
				e.Cancel = true;
				item.Checked = false;
				f.Hide();
			};

			_allItems.Add(item);
			_allForms.Add(f);
		}

		private static void FormItemClick(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;

			if (!item.Checked)
			{
				((Form)item.Tag).Show();
				((Form)item.Tag).WindowState = FormWindowState.Normal;

				item.Checked = true;
			}
			else
			{
				((Form)item.Tag).Close();

				item.Checked = false;
			}
		}

		private static string GetWindowSettingName(MenuItem item)
		{
			return ("Window-" + item.Text);
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}
