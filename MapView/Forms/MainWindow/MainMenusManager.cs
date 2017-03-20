using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	public class MainMenusManager
	{
		private readonly MenuItem _show;
		private readonly MenuItem _help;

		private readonly List<MenuItem>	_allItems = new List<MenuItem>();
		private readonly List<Form>		_allForms = new List<Form>();

		private Settings _settings;

		private bool _disposed;
		
		private const string Separator = "-";


		public MainMenusManager(MenuItem show, MenuItem help)
		{
			_show = show;
			_help = help;
		}


		public void PopulateMenus(Form fConsole, Settings settings) // NOTE: this is done w/ MV_Settings options.
		{
			_settings = settings;

			LinkMenuItemToForm(MainWindowsManager.TileView,		"Tile View",		_show);//, "TileView");
			_show.MenuItems.Add(new MenuItem(Separator));

			LinkMenuItemToForm(MainWindowsManager.TopView,		"Top View",			_show);//, "TopView");
			LinkMenuItemToForm(MainWindowsManager.RouteView,	"Route View",		_show);//, "RmpView");
			LinkMenuItemToForm(MainWindowsManager.TopRouteView,	"TopRoute View",	_show);
			_show.MenuItems.Add(new MenuItem(Separator));

			LinkMenuItemToForm(fConsole,						"Console",			_show);


			LinkMenuItemToForm(MainWindowsManager.HelpScreen,	"Quick Help",		_help);
			LinkMenuItemToForm(MainWindowsManager.AboutWindow,	"About",			_help);

			AddMenuItemSettings();
		}

		private void LinkMenuItemToForm( // NOTE: has nothing to do with the Registry.
				Form f,
				string caption,
				Menu parent)
//				string registryKey = null)
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
			foreach (MenuItem item in _show.MenuItems)
			{
				var label = GetWindowSettingLabel(item);
				if (label != null)
				{
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

		private static string GetWindowSettingLabel(MenuItem item)
		{
			string st = item.Text;
			return (st != Separator) ? "Window-" + st
							   : null;
		}

		public IMainWindowsShowAllManager CreateShowAll()
		{
			return new MainWindowsShowAllManager(_allForms, _allItems);
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}
