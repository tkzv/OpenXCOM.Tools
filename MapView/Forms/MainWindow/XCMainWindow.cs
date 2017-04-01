using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainWindow;
using MapView.Forms.XCError.WarningConsole;
using MapView.SettingServices;

using Microsoft.Win32;

using XCom;
using XCom.GameFiles.Map;
using XCom.GameFiles.Map.RouteData;
using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace MapView
{
//	public delegate void MapChangedDelegate(object sender, SetMapEventArgs e);
//	public delegate void StringDelegate(object sender, string args);


	internal sealed partial class XCMainWindow
		:
			Form
	{
		private readonly SettingsManager    _settingsManager;
		private readonly MapViewPanel       _mapViewPanel;
		private readonly LoadingForm        _loadingProgress;
		private readonly IWarningHandler    _warningHandler;
		private readonly IMainViewsManager  _mainViewsManager;
		private readonly MainWindowsManager _mainWindowsManager;
		private readonly MainMenusManager   _mainMenusManager;


		public XCMainWindow()
		{
			_instance = this;

			LogFile.CreateLogFile();
			LogFile.WriteLine("Starting MAIN MapView window ...");

			InitializeComponent();
			LogFile.WriteLine("MapView window created");


			// jijack: These two events keep getting deleted in my designer:
			tvMaps.BeforeSelect += OnMapSelect;
			tvMaps.AfterSelect  += OnMapSelected;
			// welcome to your new home.


			// WORKAROUND: The size of the form in the designer keeps increasing
			// (for whatever reason) based on the
			// 'SystemInformation.CaptionButtonSize.Height' value (the titlebar
			// height - note that 'SystemInformation.CaptionHeight' seems to be
			// 1 pixel larger, which (for whatever reason) is not the
			// 'SystemInformation.BorderSize.Height'). To prevent all that, in
			// the designer cap the form's Size by setting its MaximumSize to
			// the Size - but now run this code that sets the MaximumSize to
			// "0,0" (unlimited, as wanted). And, as a safety, do the same thing
			// with MinimumSize ... if desired.
			//
			// - observed & tested in SharpDevelop 5.1
			//
			// NOTE: This code (the constructor of a Form) shouldn't run when
			// opening the designer; it appears to run only when actually
			// running the application:

			var size = new Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			_mapViewPanel = MapViewPanel.Instance;
			LogFile.WriteLine("MapView panel created");

			_settingsManager = new SettingsManager();
			_mainMenusManager = new MainMenusManager(showMenu, miHelp);
			LogFile.WriteLine("Quick Help and About created");

			LoadDefaults();
			LogFile.WriteLine("Default settings loaded");

			Palette.UfoBattle.SetTransparent(true);
			Palette.TftdBattle.SetTransparent(true);
			Palette.UfoBattle.Grayscale.SetTransparent(true);
			Palette.TftdBattle.Grayscale.SetTransparent(true);
			LogFile.WriteLine("Palette transparencies set");

			#region Setup SharedSpace information and paths

			var share = SharedSpace.Instance;
			var consoleShare = new ConsoleSharedSpace(share);
			_warningHandler  = new ConsoleWarningHandler(consoleShare);

			MainWindowsManager.EditButtonsFactory = new EditButtonsFactory(_mapViewPanel);

			_mainWindowsManager = new MainWindowsManager();
			_mainViewsManager   = new MainViewsManager(_settingsManager, consoleShare);

			_mainMenusManager.PopulateMenus(consoleShare.GetNewConsole(), Settings);

			MainWindowsManager.MainShowAllManager = _mainMenusManager.CreateShowAllManager();
			MainWindowsManager.Initialize();
			LogFile.WriteLine("MainWindowsManager initialized");

			share.AllocateObject("MapView", this);
			share.AllocateObject(SharedSpace.AppDir,      Environment.CurrentDirectory);
			share.AllocateObject(SharedSpace.SettingsDir, Environment.CurrentDirectory + @"\settings");

			// I think this is needed only for PckView. so I'll assume '_PckView' can handle it.
//			share.AllocateObject(SharedSpace.CustomDir, Environment.CurrentDirectory + @"\custom");

			LogFile.WriteLine("Environment set");

			var dir = SharedSpace.Instance.GetString(SharedSpace.SettingsDir);
			var infoSettings = new PathInfo(dir, "MVSettings", "cfg");
			var infoPaths    = new PathInfo(dir, "Paths",      "cfg");
			var infoMapEdit  = new PathInfo(dir, "MapEdit",    "cfg");
			var infoImages   = new PathInfo(dir, "Images",     "cfg");

			share.AllocateObject(SettingsService.SettingsFile, infoSettings);
			share.AllocateObject(PathInfo.PathsFile,           infoPaths);
			share.AllocateObject(PathInfo.MapEditFile,         infoMapEdit);
			share.AllocateObject(PathInfo.ImagesFile,          infoImages);
			LogFile.WriteLine("Paths set");
			#endregion


			if (!infoPaths.FileExists()) // check if Paths.cfg exists yet
			{
				using (var install = new InstallWindow())
					if (install.ShowDialog(this) != DialogResult.OK)
						Environment.Exit(-1); // wtf -1
			}
			LogFile.WriteLine("Installation checked");



			GameInfo.ParseLine += ParseLine; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			LogFile.WriteLine("Line parsed");

			InitGameInfo(infoPaths);
			LogFile.WriteLine("GameInfo initialized");

			_mainViewsManager.ManageViews();

			MainWindowsManager.TileView.TileViewControl.MapChangedEventHandler += OnMapChanged;

			MapViewPanel.ImageUpdateEvent += OnImageUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_mapViewPanel.Dock = DockStyle.Fill;

			tvMaps.TreeViewNodeSorter = StringComparer.OrdinalIgnoreCase;

			toolStripContainer1.ContentPanel.Controls.Add(_mapViewPanel);
			MainWindowsManager.EditButtonsFactory.MakeToolStrip(tsEdit);
			tsEdit.Enabled = false;
			tsEdit.Items.Add(new ToolStripSeparator());


			try
			{
				PckSpriteCollection cursor = GameInfo.CachePckPack(
															SharedSpace.Instance.GetString(SharedSpace.CursorFile),
															String.Empty,
															2,
															Palette.UfoBattle);
				_mapViewPanel.MapView.SetCursor(new CursorSprite(cursor));
			}
			catch
			{
				try
				{
					PckSpriteCollection cursor = GameInfo.CachePckPack(
																SharedSpace.Instance.GetString(SharedSpace.CursorFile),
																String.Empty,
																4,
																Palette.TftdBattle);
					_mapViewPanel.MapView.SetCursor(new CursorSprite(cursor));
				}
				catch
				{
					_mapViewPanel.Cursor = null;
					throw; // TODO: there's got to be a better way to do that ....
				}
				throw;
			}
			LogFile.WriteLine("Cursor loaded");

			InitList();
			LogFile.WriteLine("Tilesets created and loaded to Main window's tree panel");

			if (infoSettings.FileExists())
			{
				_settingsManager.Load(infoSettings.FullPath);
				LogFile.WriteLine("User settings loaded");
			}
			else
				LogFile.WriteLine("User settings NOT loaded - no settings file to load");


			OnResize(null);
			Closing += OnCloseSaveRegistry;

			_loadingProgress = new LoadingForm();
			Bmp.LoadingEvent += _loadingProgress.HandleProgress;

			// I should rewrite the hq2x wrapper for .NET sometime -- not the code it's pretty insane
//			if (!File.Exists("hq2xa.dll"))
			miHq.Visible = false;

//			LogFile.WriteLine("Loading user-made plugins");

			/****************************************/
			// Copied from PckView
//			loadedTypes = new LoadOfType<IMapDesc>();
//			sharedSpace["MapMods"] = loadedTypes.AllLoaded;

			// There are no currently loadable maps in this assembly so this is more for future use
//			loadedTypes.LoadFrom(Assembly.GetAssembly(typeof(XCom.Interfaces.Base.IMapDesc)));

//			if (Directory.Exists(sharedSpace[SharedSpace.CustomDir].ToString()))
//			{
//				xConsole.AddLine("Custom directory exists: " + sharedSpace[SharedSpace.CustomDir].ToString());
//				foreach (string s in Directory.GetFiles(sharedSpace[SharedSpace.CustomDir].ToString()))
//					if (s.EndsWith(".dll"))
//					{
//						xConsole.AddLine("Loading dll: " + s);
//						loadedTypes.LoadFrom(Assembly.LoadFrom(s));
//					}
//			}
			/****************************************/

			LogFile.WriteLine("About to show Main window");
			Show();
		}


		private static void InitGameInfo(PathInfo filePaths)
		{
			GameInfo.Init(Palette.UfoBattle, filePaths);
		}

		private static XCMainWindow _instance;
		public static XCMainWindow Instance
		{
			get { return _instance; }
		}

		private void ParseLine(KeyvalPair line, Varidia vars)
		{
			switch (line.Keyword.ToUpperInvariant())
			{
				case "CURSOR":
					if (line.Value.EndsWith(@"\", StringComparison.Ordinal))
						SharedSpace.Instance.AllocateObject(
														SharedSpace.CursorFile,
														line.Value + SharedSpace.Cursor);
					else
						SharedSpace.Instance.AllocateObject(
														SharedSpace.CursorFile,
														line.Value + @"\" + SharedSpace.Cursor);
					break;

//				case "LOGFILE":
//					try
//					{
//						LogFile.DebugOn = bool.Parse(line.Rest);
//					}
//					catch
//					{
//						Console.WriteLine("Could not parse logfile line.");
//					}
//					break;
			}
		}

		private void ChangeSetting(object sender, string key, object val)
		{
			Settings[key].Value = val;
			switch (key)
			{
				case "Animation":
					onItem.Checked = (bool)val;
					offItem.Checked = !onItem.Checked;

					if (onItem.Checked)
						MapViewPanel.Start();
					else
						MapViewPanel.Stop();
					break;

				case "Doors":
					if (MapViewPanel.Instance.BaseMap != null)
					{
						if ((bool)val)
						{
							foreach (XCTile tile in MapViewPanel.Instance.BaseMap.Tiles)
								if (tile.Info.UfoDoor || tile.Info.HumanDoor)
									tile.MakeAnimate();
						}
						else
						{
							foreach (XCTile tile in MapViewPanel.Instance.BaseMap.Tiles)
								if (tile.Info.UfoDoor || tile.Info.HumanDoor)
									tile.StopAnimate();
						}
					}
					break;

				case "SaveWindowPositions":
					PathsEditor.SaveRegistry = (bool)val;
					break;

				case "UseGrid":
					MapViewPanel.Instance.MapView.UseGrid = (bool)val;
					break;

				case "GridColor":
					MapViewPanel.Instance.MapView.GridColor = (Color)val;
					break;

				case "GridLineColor":
					MapViewPanel.Instance.MapView.GridLineColor = (Color)val;
					break;

				case "GridLineWidth":
					MapViewPanel.Instance.MapView.GridLineWidth = (int)val;
					break;
			}
		}

		private sealed class SortableTreeNode
			:
				TreeNode,
				IComparable
		{
			public SortableTreeNode(string text)
				:
					base(text)
			{}

			public int CompareTo(object other)
			{
				var node = other as SortableTreeNode;
				return (node != null) ? String.CompareOrdinal(Text, node.Text)
									  : -1;
			}
		}

		private static void addMaps(TreeNode tn, IDictionary<string, IMapDesc> maps)
		{
			foreach (string key in maps.Keys)
			{
				var node = new SortableTreeNode(key);
				node.Tag = maps[key];
				tn.Nodes.Add(node);
			}
		}

		private void AddTileset(ITileset tileset)
		{
			var node = new SortableTreeNode(tileset.Name);
			node.Tag = tileset;
			tvMaps.Nodes.Add(node);

			foreach (string key in tileset.Subsets.Keys)
			{
				var group = new SortableTreeNode(key);
				group.Tag = tileset.Subsets[key];
				node.Nodes.Add(group);

				addMaps(group, tileset.Subsets[key]);
			}
		}

		private void InitList()
		{
			tvMaps.Nodes.Clear();

			foreach (string key in GameInfo.TilesetInfo.Tilesets.Keys)
				AddTileset(GameInfo.TilesetInfo.Tilesets[key]);
		}

		private void OnCloseSaveRegistry(object sender, CancelEventArgs args)
		{
			if (NotifySave() == DialogResult.Cancel)
			{
				args.Cancel = true;
			}
			else
			{
				_mainMenusManager.Dispose();

				if (PathsEditor.SaveRegistry)
				{
					var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.Software);
					var keyMapView  = keySoftware.CreateSubKey("MapView");
					var keyMainView = keyMapView.CreateSubKey("MainView");

					_mainViewsManager.CloseAllViews();

					WindowState = FormWindowState.Normal;

					keyMainView.SetValue("Left",   Left);
					keyMainView.SetValue("Top",    Top);
					keyMainView.SetValue("Width",  Width);
					keyMainView.SetValue("Height", Height - SystemInformation.CaptionButtonSize.Height);

					keyMainView.Close();
					keyMapView.Close();
					keySoftware.Close();
				}

				_settingsManager.Save(); // save MV_SettingsFile // TODO: Save settings when closing the Options form(s).
			}
		}

		private void LoadDefaults()
		{
			var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.Software);
			var keyMapView  = keySoftware.CreateSubKey("MapView");
			var keyMainView = keyMapView.CreateSubKey("MainView");

			Left   = (int)keyMainView.GetValue("Left",   Left);
			Top    = (int)keyMainView.GetValue("Top",    Top);
			Width  = (int)keyMainView.GetValue("Width",  Width);
			Height = (int)keyMainView.GetValue("Height", Height);

			keyMainView.Close();
			keyMapView.Close();
			keySoftware.Close();

			var settings = new Settings();

//			Color.FromArgb(175, 69, 100, 129)

			var change = new ValueChangedEventHandler(ChangeSetting);

			settings.AddSetting(
							"Animation",
							MapViewPanel.IsAnimated,
							"If true the map will animate itself.",
							"Main",
							change, false, null);
			settings.AddSetting(
							"Doors",
							false,
							"If true the door tiles will animate themselves.",
							"Main",
							change, false, null);
			settings.AddSetting(
							"SaveWindowPositions",
							PathsEditor.SaveRegistry,
							"If true the window positions and sizes will be saved in the windows registry.",
							"Main",
							change, false, null);
			settings.AddSetting(
							"UseGrid",
							MapViewPanel.Instance.MapView.UseGrid,
							"If true a grid will show up at the current level of editing.",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridColor",
							MapViewPanel.Instance.MapView.GridColor,
							"Color of the grid in (a,r,g,b) format.",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridLineColor",
							MapViewPanel.Instance.MapView.GridLineColor,
							"Color of the lines that make up the grid.",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridLineWidth",
							MapViewPanel.Instance.MapView.GridLineWidth,
							"Width of the grid lines in pixels.",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"SelectGrayscale",
							MapViewPanel.Instance.MapView.SelectGrayscale,
							"If true the selection area will show up in gray.",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
//			settings.AddSetting(
//							"SaveOnExit",
//							true,
//							"If true these settings will be saved on program exit.",
//							"Main",
//							null, false, null);

			Settings = settings;
		}

		private void OnImageUpdate(object sender, EventArgs e)
		{
			MainWindowsManager.TopView.Control.BottomPanel.Refresh();
		}

/*		private static void myQuit(object sender, string command)
		{
			if (command == "OK")
				Environment.Exit(0);
		} */

		private void onItem_Click(object sender, EventArgs e)
		{
			ChangeSetting(this, "Animation", true);
		}

		private void offItem_Click(object sender, EventArgs e)
		{
			ChangeSetting(this, "Animation", false);
		}

		private void saveItem_Click(object sender, EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				_mapViewPanel.BaseMap.Save();
				XConsole.AdZerg("Saved: " + _mapViewPanel.BaseMap.Name);
			}
		}

		private void miQuit_Click(object sender, EventArgs e)
		{
			OnCloseSaveRegistry(null, new CancelEventArgs(true));
			Environment.Exit(0);
		}

		private void miPaths_Click(object sender, EventArgs e)
		{
			var share = SharedSpace.Instance[PathInfo.PathsFile];

			using (var paths = new PathsEditor(share.ToString()))
				paths.ShowDialog();

			var pathInfo = (PathInfo)share;
			InitGameInfo(pathInfo);
			InitList();
		}

		/// <summary>
		/// De-colorizes the background-field of a previously selected label
		/// in the left-panel's MapBlocks' tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapSelect(object sender, CancelEventArgs e)
		{
			if (NotifySave() == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
			else if (tvMaps.SelectedNode != null)
			{
				tvMaps.SelectedNode.BackColor = SystemColors.Control;
			}
		}

		/// <summary>
		/// Colorizes the background-field of the newly selected label in the
		/// left-panel's MapBlocks' tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapSelected(object sender, TreeViewEventArgs e)
		{
			tvMaps.SelectedNode.BackColor = Color.Gold;
			LoadSelectedMap();
		}

		private void OnMapChanged()
		{
			LoadSelectedMap();
		}

		private void LoadSelectedMap()
		{
			var desc = tvMaps.SelectedNode.Tag as IMapDesc;
			if (desc != null)
			{
				miExport.Enabled = true;

				var tileFactory = new XCTileFactory();
				tileFactory.HandleWarning += _warningHandler.HandleWarning;

				var fileService = new XCMapFileService(tileFactory);

				var baseMap = fileService.Load(desc as XCMapDesc);
				_mapViewPanel.SetMap(baseMap);

				tsEdit.Enabled = true;

				RouteService.CheckNodeBounds(baseMap);

				tsslMap.Text = desc.Label;

				tsslDimensions.Text = (baseMap != null) ? baseMap.MapSize.ToString()
											   : "size: n/a";

				if (miDoors.Checked) // turn off door animations
				{
					miDoors.Checked = false;
					miDoors_Click(null, null);
				}

				if (!showMenu.Enabled) // open all the forms in the show menu once
					_mainMenusManager.LoadState();

				_mainWindowsManager.SetMap(baseMap); // reset all observer events
			}
			else
				miExport.Enabled = false;
		}

		private DialogResult NotifySave()
		{
			if (_mapViewPanel.BaseMap != null && _mapViewPanel.BaseMap.MapChanged)
			{
				switch (MessageBox.Show(
									this,
									"Do you want to save changes?",
									"Map Changed",
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0))
				{
					case DialogResult.No:		// don't save
						break;

					case DialogResult.Yes:		// save
						_mapViewPanel.BaseMap.Save();
						break;

					case DialogResult.Cancel:	// do nothing
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		private void miOptions_Click(object sender, EventArgs e)
		{
			var f = new OptionsForm("MainViewOptions", Settings);
			f.Text = "Main View Options";
			f.Show();
		}

		private void miSaveImage_Click(object sender, EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				sfdSaveDialog.FileName = _mapViewPanel.BaseMap.Name;
				if (sfdSaveDialog.ShowDialog() == DialogResult.OK)
				{
					_loadingProgress.Show();

					try
					{
						_mapViewPanel.BaseMap.SaveGif(sfdSaveDialog.FileName);
					}
					finally
					{
						_loadingProgress.Hide();
					}
				}
			}
		}

		private void miHq_Click(object sender, EventArgs e)
		{
			var map = _mapViewPanel.BaseMap as XCMapFile;
			if (map != null)
			{
				map.HQ2X();
				_mapViewPanel.OnResize();
			}
		}

		private void miDoors_Click(object sender, EventArgs e)
		{
			miDoors.Checked = !miDoors.Checked;

			foreach (XCTile tile in _mapViewPanel.BaseMap.Tiles)
				if (tile.Info.UfoDoor || tile.Info.HumanDoor)
				{
					if (miDoors.Checked)
						tile.MakeAnimate();
					else
						tile.StopAnimate();
				}
		}

		private void miResize_Click(object sender, EventArgs e)
		{
			if (_mapViewPanel.MapView.Map != null)
			{
				using (var f = new ChangeMapSizeForm())
				{
					f.Map = _mapViewPanel.MapView.Map;
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						f.Map.ResizeTo(
									f.NewRows,
									f.NewCols,
									f.NewHeight,
									f.AddHeightToCeiling);
						_mapViewPanel.ForceResize();
					}
				}
			}
		}

		private bool _windowFlag = false;

		private void MainWindow_Activated(object sender, EventArgs e)
		{
			if (!_windowFlag)
			{
				_windowFlag = true;

				foreach (MenuItem it in showMenu.MenuItems)
					if (it.Checked)
						((Form)it.Tag).BringToFront();

				Focus();
				BringToFront();

				_windowFlag = false;
			}
		}

		private void miInfo_Click(object sender, EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				var f = new MapInfoForm();
				f.Show();
				f.Analyze(_mapViewPanel.BaseMap);
			}
		}

		private void miExport_Click(object sender, EventArgs e)
		{
//			if (mapList.SelectedNode.Parent == null) // top level node - bad
//				throw new Exception("miExport_Click: Should not be here");
//
//			ExportForm ef = new ExportForm();
//			List<string> maps = new List<string>();
//
//			if (mapList.SelectedNode.Parent.Parent == null)//tileset
//			{
//				foreach (TreeNode tn in mapList.SelectedNode.Nodes)
//					maps.Add(tn.Text);
//			}
//			else // map
//				maps.Add(mapList.SelectedNode.Text);
//
//			ef.Maps = maps;
//			ef.ShowDialog();
		}

		private void miOpen_Click(object sender, EventArgs e)
		{}

		private Settings Settings
		{
			get { return _settingsManager["MainWindow"]; }
			set { _settingsManager["MainWindow"] = value; }
		}

		private void drawSelectionBoxButton_Click(object sender, EventArgs e)
		{
			_mapViewPanel.MapView.DrawSelectionBox = !_mapViewPanel.MapView.DrawSelectionBox;
			tsbSelectionBox.Checked = !tsbSelectionBox.Checked;
		}

		private void ZoomInButton_Click(object sender, EventArgs e)
		{
			if (Globals.PckImageScale < Globals.MaxPckImageScale)
			{
				Globals.PckImageScale += 0.125;
				Globals.AutoPckImageScale = false;
				tsbAutoZoom.Checked = false;

				_mapViewPanel.SetupMapSize();

				Refresh();

				_mapViewPanel.OnResize();
			}
		}

		private void ZoomOutButton_Click(object sender, EventArgs e)
		{
			if (Globals.PckImageScale > Globals.MinPckImageScale)
			{
				Globals.PckImageScale -= 0.125;
				Globals.AutoPckImageScale = false;
				tsbAutoZoom.Checked = false;

				_mapViewPanel.SetupMapSize();

				Refresh();

				_mapViewPanel.OnResize();
			}
		}

		private void AutoZoomButton_Click(object sender, EventArgs e)
		{
			Globals.AutoPckImageScale = !Globals.AutoPckImageScale;

			if (!Globals.AutoPckImageScale)
				Globals.PckImageScale = 1;

			tsbAutoZoom.Checked = !tsbAutoZoom.Checked;

			_mapViewPanel.SetupMapSize();

			Refresh();

			_mapViewPanel.OnResize();
		}

		public void StatusBarPrintPosition(int col, int row)
		{
			tsslPosition.Text = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"c:{0} r:{1}",
											col, row);
		}
	}
}
