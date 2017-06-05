using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	internal sealed class MainMenusManager
	{
		#region Fields
		private readonly MenuItem _menuViewers;
		private readonly MenuItem _menuHelp;

		private readonly List<MenuItem> _allItems = new List<MenuItem>();
		private readonly List<Form>     _allForms = new List<Form>();

		private Options _options;

		private bool _quitting;

		private const string Divider = "-";
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="show"></param>
		/// <param name="help"></param>
		internal MainMenusManager(MenuItem show, MenuItem help)
		{
			_menuViewers = show; // why are these MenuItems
			_menuHelp    = help;
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// Handles clicking on a MenuItem to open/close a window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		#endregion


		#region Methods
		/// <summary>
		/// Adds menuitems to MapView's dropdown list.
		/// </summary>
		/// <param name="fconsole">pointer to the console-form</param>
		/// <param name="options">pointer to MV_OptionsFile options</param>
		internal void PopulateMenus(Form fconsole, Options options)
		{
			_options = options;

			// Viewers menuitems ->
			CreateMenuItem(ViewerFormsManager.TileView,     RegistryInfo.TileView,     _menuViewers);

			_menuViewers.MenuItems.Add(new MenuItem(Divider));

			CreateMenuItem(ViewerFormsManager.TopView,      RegistryInfo.TopView,      _menuViewers);
			CreateMenuItem(ViewerFormsManager.RouteView,    RegistryInfo.RouteView,    _menuViewers);
			CreateMenuItem(ViewerFormsManager.TopRouteView, RegistryInfo.TopRouteView, _menuViewers);

			_menuViewers.MenuItems.Add(new MenuItem(Divider));

			CreateMenuItem(fconsole,                        RegistryInfo.Console,      _menuViewers);

			// Help menuitems ->
			CreateMenuItem(ViewerFormsManager.HelpScreen,  "Help",  _menuHelp);
			CreateMenuItem(ViewerFormsManager.AboutScreen, "About", _menuHelp);


			AddViewersOptions();
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer. See PopulateMenus() above^
		/// </summary>
		/// <param name="f"></param>
		/// <param name="caption"></param>
		/// <param name="parent"></param>
		private void CreateMenuItem(
				Form f,
				string caption,
				Menu parent)
		{
			f.Text = caption;

			var it = new MenuItem(caption);
			it.Tag = f;

			parent.MenuItems.Add(it);

			it.Click += OnMenuItemClick;

			f.FormClosing += (sender, e) =>
			{
				e.Cancel = true;
				it.Checked = false;
				f.Hide();
			};

			_allItems.Add(it);
			_allForms.Add(f);
		}

		/// <summary>
		/// Adds the user Options for each viewer.
		/// </summary>
		private void AddViewersOptions()
		{
			foreach (MenuItem it in _menuViewers.MenuItems)
			{
//				string key = GetViewerKey(it);
//				if (!String.IsNullOrEmpty(key))

				string key = it.Text;
				if (!key.Equals(Divider, StringComparison.Ordinal))
				{
					_options.AddOption(
									key,
//									!(it.Tag is XCom.ConsoleForm)
//									!(it.Tag is MapView.Forms.MapObservers.TileViews.TopRouteViewForm),	// q. why is TopRouteViewForm under 'TileViews'
																										// a. why not.
									(       it.Tag is TopViewForm)	// true to have the viewer open on 1st run.
										|| (it.Tag is RouteViewForm)
										|| (it.Tag is TileViewForm),
									"Open on load - " + key,		// appears as a tip at the bottom of the Options screen.
									"Windows");						// this identifies what Option category the setting appears under.
																	// NOTE: the Console is not technically a viewer
					var f = it.Tag as Form;							// but it appears under Options like the real viewers.
					if (f != null)
					{
						f.VisibleChanged += (sender, e) => {
							if (!_quitting)
							{
								var fsender = sender as Form;
								if (fsender != null)
									_options[key].Value = fsender.Visible;
							}
						};
					}
				}
			}
		}

		/// <summary>
		/// Opens the subsidiary viewers. All viewers will open and those that
		/// the user has flagged false in Options will be closed.
		/// </summary>
		internal void StartAllViewers()
		{
			foreach (MenuItem it in _menuViewers.MenuItems)
			{
//				string key = GetViewerKey(it);
//				if (key != null)

				string key = it.Text;
				if (!key.Equals(Divider, StringComparison.Ordinal))
				{
					if (_options[key].IsTrue)
						it.PerformClick();

//					else					// NOTE: does not run unless a viewer's key was
//					{						// set "false" in AddMenuItemOptions() above^
//						it.PerformClick();	// not sure that this double-click gimmick
//						it.PerformClick();	// is necessary even then ...
//					}
				}
			}
//			foreach (MenuItem it in _show.MenuItems) // NOTE: Don't do this. Go figure.
//			{
//				it.PerformClick();
//				var key = GetViewerKey(it);
//				if (!(_options[key].IsTrue))
//					it.PerformClick();
//			}

			_menuViewers.Enabled = true;
		}

//		private static string GetViewerKey(MenuItem it)
//		{
//			string suffix = it.Text;
//			return (suffix != Divider) ? "Window" + Divider + suffix
//									   : null;
//		}

		/// <summary>
		/// Effectively disables the VisibleChanged event for all subsidiary
		/// viewers.
		/// </summary>
		internal void IsQuitting()
		{
			_quitting = true;
		}

		/// <summary>
		/// Creates a device that minimizes and restores all subsidiary viewers
		/// when PckView is opened in TileView.
		/// </summary>
		/// <returns></returns>
		internal ShowHideManager CreateShowHideManager()
		{
			return new ShowHideManager(_allForms, _allItems);
		}
		#endregion
	}
}
