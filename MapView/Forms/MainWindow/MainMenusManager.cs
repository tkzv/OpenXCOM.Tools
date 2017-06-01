using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainMenusManager
	{
		private readonly MenuItem _viewsMenu;
		private readonly MenuItem _helpMenu;

		private readonly List<MenuItem> _allItems = new List<MenuItem>();
		private readonly List<Form>     _allForms = new List<Form>();

		private Options _options;

		private bool _quitting;

		private const string Divider = "-";


		internal MainMenusManager(MenuItem show, MenuItem help)
		{
			_viewsMenu = show; // why are these MenuItems
			_helpMenu  = help;
		}


		/// <summary>
		/// Adds menuitems to MapView's dropdown list.
		/// </summary>
		/// <param name="console">pointer to the console-form</param>
		/// <param name="options">pointer to MV_OptionsFile options</param>
		internal void PopulateMenus(Form console, Options options)
		{
			_options = options;

			CreateMenuItem(ViewerFormsManager.TileView,     "Tile View",     _viewsMenu);

			_viewsMenu.MenuItems.Add(new MenuItem(Divider));

			CreateMenuItem(ViewerFormsManager.TopView,      "Top View",      _viewsMenu);
			CreateMenuItem(ViewerFormsManager.RouteView,    "Route View",    _viewsMenu);
			CreateMenuItem(ViewerFormsManager.TopRouteView, "TopRoute View", _viewsMenu);

			_viewsMenu.MenuItems.Add(new MenuItem(Divider));

			CreateMenuItem(console,                         "Console",       _viewsMenu);


			CreateMenuItem(ViewerFormsManager.HelpScreen,   "Help",          _helpMenu);
			CreateMenuItem(ViewerFormsManager.AboutScreen,  "About",         _helpMenu);

			AddMenuItemOptions();
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer. See PopulateMenus() above^
		/// </summary>
		/// <param name="f"></param>
		/// <param name="text"></param>
		/// <param name="parent"></param>
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

		// Handles clicking on a MenuItem to open/close a viewer.
		private static void OnMenuItemClick(object sender, EventArgs e)
		{
			var it = (MenuItem)sender;

			if (!it.Checked)
			{
				it.Checked = true;
				((Form)it.Tag).Show();
				((Form)it.Tag).WindowState = FormWindowState.Normal;

				if (it.Tag is Help) // update colors that user might have set in TileView's Option-settings.
					ViewerFormsManager.HelpScreen.UpdateColors();
			}
			else
			{
				it.Checked = false;
				((Form)it.Tag).Close();
			}
		}

		private void AddMenuItemOptions()
		{
			foreach (MenuItem it in _viewsMenu.MenuItems)
			{
				string key = GetViewerKey(it);
				if (!String.IsNullOrEmpty(key))
				{
					_options.AddOption(
									key,
//									!(it.Tag is XCom.ConsoleForm)
//									!(it.Tag is MapView.Forms.MapObservers.TileViews.TopRouteViewForm),	// q. why is TopRouteViewForm under 'TileViews'
																										// a. why not.
									(       it.Tag is TopViewForm) // true to have the viewer open on 1st run.
										|| (it.Tag is RouteViewForm)
										|| (it.Tag is TileViewForm),
									"Viewer - " + it.Text,	// appears as a tip at the bottom of the Options screen.
									"Windows");				// this identifies what Option category the setting appears under.
															// NOTE: the Console is not technically a viewer
					var f = it.Tag as Form;					// but it appears under Options like the real viewers.
					if (f != null)
					{
						f.VisibleChanged += (sender, e) =>
						{
							if (!_quitting)
							{
								var fsender = sender as Form;
								if (fsender != null)
									_options[key].Value = fsender.Visible;
							}
//							if (_quitting) return;
//							var fsender = sender as Form;
//							if (fsender == null) return;
//							_options[key].Value = fsender.Visible;
						};
					}
				}
			}
		}

		/// <summary>
		/// Opens the subsidiary viewers. All viewers will open and those that
		/// the user has flagged closed (in Options) will be re-closed.
		/// </summary>
		internal void StartAllViewers()
		{
			foreach (MenuItem it in _viewsMenu.MenuItems)
			{
				string key = GetViewerKey(it);
				if (key != null)
				{
					if (_options[key].IsTrue)
					{
						it.PerformClick();
					}
					else					// NOTE: does not run unless a viewer's key was
					{						// set "false" in AddMenuItemOptions() above^
						it.PerformClick();	// not sure that this double-click gimmick
						it.PerformClick();	// is necessary even then ...
					}
				}
			}
//			foreach (MenuItem it in _show.MenuItems) // NOTE: Don't do this. Go figure.
//			{
//				it.PerformClick();
//				var key = GetViewerKey(it);
//				if (!(_options[key].IsTrue))
//					it.PerformClick();
//			}

			_viewsMenu.Enabled = true;
		}

		private static string GetViewerKey(MenuItem it)
		{
			string suffix = it.Text;
			return (suffix != Divider) ? "Window" + Divider + suffix
									   : null;
		}

		internal ShowHideManager CreateShowHideManager()
		{
			return new ShowHideManager(_allForms, _allItems);
		}

		/// <summary>
		/// Effectively disables the VisibleChanged event for all subsidiary
		/// viewers.
		/// </summary>
		internal void IsQuitting()
		{
			_quitting = true;
		}
	}
}
