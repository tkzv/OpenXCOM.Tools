using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainMenusManager
	{
		private readonly MenuItem _show;
		private readonly MenuItem _help;

		private readonly List<MenuItem> _allItems = new List<MenuItem>();
		private readonly List<Form>     _allForms = new List<Form>();

		private Settings _settings;

		private bool _disposed;

		private const string Separator = "-";


		public MainMenusManager(MenuItem show, MenuItem help)
		{
			_show = show;
			_help = help;
		}


		/// <summary>
		/// Adds menuitems to MapView's dropdown list.
		/// </summary>
		/// <param name="console">pointer to the console-form</param>
		/// <param name="settings">pointer to MV_SettingsFile options</param>
		public void PopulateMenus(Form console, Settings settings)
		{
			_settings = settings;

			LinkMenuItemToForm(MainWindowsManager.TileView,     "Tile View",     _show);

			_show.MenuItems.Add(new MenuItem(Separator));

			LinkMenuItemToForm(MainWindowsManager.TopView,      "Top View",      _show);
			LinkMenuItemToForm(MainWindowsManager.RouteView,    "Route View",    _show);
			LinkMenuItemToForm(MainWindowsManager.TopRouteView, "TopRoute View", _show);

			_show.MenuItems.Add(new MenuItem(Separator));

			LinkMenuItemToForm(console,                         "Console",       _show);


			LinkMenuItemToForm(MainWindowsManager.HelpScreen,   "Quick Help",    _help);
			LinkMenuItemToForm(MainWindowsManager.AboutWindow,  "About",         _help);

			AddMenuItemSettings();
		}

		private void LinkMenuItemToForm(
				Form f,
				string caption,
				Menu parent)
		{
			f.Text = caption;

			var it = new MenuItem(caption);
			it.Tag = f;

			parent.MenuItems.Add(it);

			it.Click += FormItemClick;

			f.Closing += (sender, e) =>
			{
				e.Cancel = true;
				it.Checked = false;
				f.Hide();
			};

			_allItems.Add(it);
			_allForms.Add(f);
		}

		private static void FormItemClick(object sender, EventArgs e)
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
			foreach (MenuItem it in _show.MenuItems)
			{
				var label = GetWindowSettingLabel(it);
				if (label != null)
				{
					_settings.AddSetting(
									label,
									!(it.Tag is TopViewForm) && !(it.Tag is RouteViewForm),
									"Default display window - " + it.Text,
									"Windows",
									null,
									false,
									null);

					var f = it.Tag as Form;
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
		}

		public void LoadState()
		{
			foreach (MenuItem it in _show.MenuItems)
			{
				var label = GetWindowSettingLabel(it);
				if (label != null)
				{
					if (_settings[label].IsBoolean)
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
			{											// All the View-Panels will load ...
				it.PerformClick();						// regardless of their saved settings.

				var label = GetWindowSettingName(it);
				if (!(_settings[label].ValueBool))
					it.PerformClick();
			} */
			_show.Enabled = true;
		}

		private static string GetWindowSettingLabel(MenuItem item)
		{
			string st = item.Text;
			return (st != Separator) ? "Window-" + st
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
