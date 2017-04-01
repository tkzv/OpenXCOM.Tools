using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainMenusManager
	{
		private readonly MenuItem _viewsMenu;
		private readonly MenuItem _helpMenu;

		private readonly List<MenuItem> _allItems = new List<MenuItem>();
		private readonly List<Form>     _allForms = new List<Form>();

		private Settings _settings;

		private bool _disposed;

		private const string Separator = "-";


		public MainMenusManager(MenuItem show, MenuItem help)
		{
			_viewsMenu = show;
			_helpMenu = help;
		}


		/// <summary>
		/// Adds menuitems to MapView's dropdown list.
		/// </summary>
		/// <param name="console">pointer to the console-form</param>
		/// <param name="settings">pointer to MV_SettingsFile options</param>
		public void PopulateMenus(Form console, Settings settings)
		{
			_settings = settings;

			CreateMenuItem(MainWindowsManager.TileView,     "Tile View",     _viewsMenu);

			_viewsMenu.MenuItems.Add(new MenuItem(Separator));

			CreateMenuItem(MainWindowsManager.TopView,      "Top View",      _viewsMenu);
			CreateMenuItem(MainWindowsManager.RouteView,    "Route View",    _viewsMenu);
			CreateMenuItem(MainWindowsManager.TopRouteView, "TopRoute View", _viewsMenu);

			_viewsMenu.MenuItems.Add(new MenuItem(Separator));

			CreateMenuItem(console,                         "Console",       _viewsMenu);


			CreateMenuItem(MainWindowsManager.HelpScreen,   "Quick Help",    _helpMenu);
			CreateMenuItem(MainWindowsManager.AboutWindow,  "About",         _helpMenu);

			AddMenuItemSettings();
		}

		private void CreateMenuItem(
				Form f,
				string text,
				Menu parent)
		{
			f.Text = text;

			var it = new MenuItem(text);
			it.Tag = f;

			parent.MenuItems.Add(it);

			it.Click += OnMenuItemClick;

			f.Closing += (sender, e) =>
			{
				e.Cancel = true;
				it.Checked = false;
				f.Hide();
			};

			_allItems.Add(it);
			_allForms.Add(f);
		}

		private static void OnMenuItemClick(object sender, EventArgs e)
		{
			var it = (MenuItem)sender;

			if (!it.Checked)
			{
				((Form)it.Tag).Show();
				((Form)it.Tag).WindowState = FormWindowState.Normal;
				it.Checked = true;
			}
			else
			{
				((Form)it.Tag).Close();
				it.Checked = false;
			}
		}

		private void AddMenuItemSettings()
		{
			foreach (MenuItem it in _viewsMenu.MenuItems)
			{
				var key = GetWindowSettingKey(it);
				if (key != null)
				{
					_settings.AddSetting(
									key,
									!(it.Tag is TopViewForm) && !(it.Tag is RouteViewForm),
									"Default display window - " + it.Text,
									"Windows",
									null,
									false,
									null);

					var f = it.Tag as Form;
					if (f != null)
					{
						f.VisibleChanged += (sender, e) =>
						{
							if (_disposed)
								return;

							var senderForm = sender as Form;
							if (senderForm == null)
								return;

							_settings[key].Value = senderForm.Visible;
						};
					}
				}
			}
		}

		public void LoadState()
		{
			foreach (MenuItem it in _viewsMenu.MenuItems)
			{
				var key = GetWindowSettingKey(it);
				if (key != null)
				{
					if (_settings[key].IsBoolean)
					{
						it.PerformClick();
					}
					else
					{
						it.PerformClick();
						it.PerformClick();
					}
				}
			}
/*			foreach (MenuItem it in _show.MenuItems)	// NOTE: Don't do this. Go figure.
			{											// All the Views will load ...
				it.PerformClick();						// regardless of their saved settings.

				var label = GetWindowSettingName(it);
				if (!(_settings[label].ValueBool))
					it.PerformClick();
			} */
			_viewsMenu.Enabled = true;
		}

		private static string GetWindowSettingKey(MenuItem it)
		{
			string suffix = it.Text;
			return (suffix != Separator) ? "Window" + Separator + suffix
										 : null;
		}

		public IMainShowAllManager CreateShowAllManager()
		{
			return new MainShowAllManager(_allForms, _allItems);
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}
