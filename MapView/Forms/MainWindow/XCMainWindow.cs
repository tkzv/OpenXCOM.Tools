using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
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

using YamlDotNet.RepresentationModel;


namespace MapView
{
//	public delegate void MapChangedDelegate(object sender, SetMapEventArgs e);
//	public delegate void StringDelegate(object sender, string args);


	internal sealed partial class XCMainWindow
		:
			Form
	{
		private readonly SettingsManager       _settingsManager;
		private readonly MapViewPanel          _mainViewPanel;
		private readonly LoadingForm           _loadingProgress;
		private readonly ConsoleWarningHandler _warningHandler;
		private readonly MainViewsManager      _mainViewsManager;
		private readonly MainWindowsManager    _mainWindowsManager;
		private readonly MainMenusManager      _mainMenusManager;


		public XCMainWindow()
		{
			// TODO: further optimize this loading sequence ....

			LogFile.CreateLogFile();
			LogFile.WriteLine("Starting MAIN MapView window ...");


			var share = SharedSpace.Instance;

			share.AllocateObject("MapView", this);
			share.AllocateObject(
							SharedSpace.ApplicationDirectory,
							Environment.CurrentDirectory);

			string dir = share.AllocateObject(
										SharedSpace.SettingsDirectory,
										Environment.CurrentDirectory + @"\settings")
									.ToString();

			// I think this is needed only for PckView. so I'll assume 'PckViewForm' can handle it.
//			share.AllocateObject(SharedSpace.CustomDirectory, Environment.CurrentDirectory + @"\custom");

			LogFile.WriteLine("Environment cached.");

//			string dir = SharedSpace.Instance.GetString(SharedSpace.SettingsDirectory);
			var infoViewers  = new PathInfo(dir, "MapViewers", "yml");

			var infoSettings = new PathInfo(dir, "MVSettings", "cfg");
			var infoPaths    = new PathInfo(dir, "Paths",      "cfg");
			var infoMapEdit  = new PathInfo(dir, "MapEdit",    "cfg");
			var infoImages   = new PathInfo(dir, "Images",     "cfg");

			share.AllocateObject(PathInfo.MapViewers, infoViewers);

			share.AllocateObject(PathInfo.SettingsFile, infoSettings);
			share.AllocateObject(PathInfo.PathsFile,    infoPaths);
			share.AllocateObject(PathInfo.MapEditFile,  infoMapEdit);
			share.AllocateObject(PathInfo.ImagesFile,   infoImages);
			LogFile.WriteLine("PathInfo cached.");


			if (!infoPaths.FileExists()) // check if Paths.cfg exists yet
			{
				using (var f = new InstallWindow())
					if (f.ShowDialog(this) != DialogResult.OK)
						Environment.Exit(-1); // wtf -1

				LogFile.WriteLine("Installation files created.");
			}
			else
				LogFile.WriteLine("Paths.Cfg file exists.");


			if (!infoViewers.FileExists())
			{
				CreateIni();
				LogFile.WriteLine("Window configuration file created.");
			}
			else
				LogFile.WriteLine("Window configuration file exists.");



			InitializeComponent();
			LogFile.WriteLine("MainView window initialized.");

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


			// jijack: These two events keep getting deleted in my designer:
			tvMaps.BeforeSelect += OnMapSelect;
			tvMaps.AfterSelect  += OnMapSelected;
			// ... welcome to your new home

			_instance = this;


			_settingsManager = new SettingsManager(); // goes before LoadDefaults()

			LoadDefaults();
			LogFile.WriteLine("MainView Settings loaded.");


			_mainViewPanel = MapViewPanel.Instance;
			LogFile.WriteLine("MainView panel instantiated.");


			Palette.UfoBattle.SetTransparent(true);
			Palette.TftdBattle.SetTransparent(true);
			Palette.UfoBattle.Grayscale.SetTransparent(true);
			Palette.TftdBattle.Grayscale.SetTransparent(true);
			LogFile.WriteLine("Palette transparencies set");


			var consoleShare = new ConsoleSharedSpace(share);
			_warningHandler  = new ConsoleWarningHandler(consoleShare);


			_mainWindowsManager = new MainWindowsManager();
			_mainViewsManager   = new MainViewsManager(_settingsManager, consoleShare);
			_mainMenusManager   = new MainMenusManager(menuShow, menuHelp);



			MainWindowsManager.ShowAllManager = _mainMenusManager.CreateShowAllManager();
			MainWindowsManager.EditFactory = new EditButtonsFactory(_mainViewPanel);
			MainWindowsManager.Initialize();
			LogFile.WriteLine("MainWindowsManager initialized.");



			_mainMenusManager.PopulateMenus(consoleShare.GetNewConsole(), Settings);
			LogFile.WriteLine("MainView menus populated.");




			GameInfo.ParseLine += ParseLine; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			InitGameInfo(infoPaths);
			LogFile.WriteLine("GameInfo initialized");


			_mainViewsManager.ManageViews();

			MainWindowsManager.TileView.Control.MapChangedEventHandler += OnMapChanged;

			MapViewPanel.ImageUpdateEvent += OnImageUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_mainViewPanel.Dock = DockStyle.Fill;

			tvMaps.TreeViewNodeSorter = StringComparer.OrdinalIgnoreCase;

			tscPanel.ContentPanel.Controls.Add(_mainViewPanel);

			MainWindowsManager.EditFactory.BuildToolStrip(tsEdit);
			tsEdit.Enabled = false;
			tsEdit.Items.Add(new ToolStripSeparator());


			try
			{
				PckSpriteCollection cursor = GameInfo.CachePckPack(
															SharedSpace.Instance.GetString(SharedSpace.CursorFile),
															String.Empty,
															2,
															Palette.UfoBattle);
				_mainViewPanel.MapView.SetCursor(new CursorSprite(cursor));
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
					_mainViewPanel.MapView.SetCursor(new CursorSprite(cursor));
				}
				catch
				{
					_mainViewPanel.Cursor = null;
					throw; // TODO: there's got to be a better way to do that ....
				}
				throw;
			}
			LogFile.WriteLine("Cursor loaded.");

			InitList();
			LogFile.WriteLine("Tilesets created and loaded to Main window's tree panel.");

			if (infoSettings.FileExists())
			{
				_settingsManager.Load(infoSettings.FullPath);
				LogFile.WriteLine("User settings loaded");
			}
			else
				LogFile.WriteLine("User settings NOT loaded - no settings file to load.");


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

//			if (Directory.Exists(sharedSpace[SharedSpace.CustomDirectory].ToString()))
//			{
//				xConsole.AddLine("Custom directory exists: " + sharedSpace[SharedSpace.CustomDirectory].ToString());
//				foreach (string s in Directory.GetFiles(sharedSpace[SharedSpace.CustomDirectory].ToString()))
//					if (s.EndsWith(".dll"))
//					{
//						xConsole.AddLine("Loading dll: " + s);
//						loadedTypes.LoadFrom(Assembly.LoadFrom(s));
//					}
//			}
			/****************************************/

			LogFile.WriteLine("About to show MainView ...");
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

		private void OnSettingChange(object sender, string key, object val)
		{
			Settings[key].Value = val;
			switch (key)
			{
				case "Animation":
					miOn.Checked = (bool)val;
					miOff.Checked = !miOn.Checked;

					if (miOn.Checked)
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

				case "ShowGrid":
					MapViewPanel.Instance.MapView.ShowGrid = (bool)val;
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

				// NOTE: "GraySelection" is handled.
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

		private static void AddMaps(TreeNode tn, IDictionary<string, IMapDesc> maps)
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

				AddMaps(group, tileset.Subsets[key]);
			}
		}

		private void InitList()
		{
			tvMaps.Nodes.Clear();

			foreach (string key in GameInfo.TilesetInfo.Tilesets.Keys)
				AddTileset(GameInfo.TilesetInfo.Tilesets[key]);
		}

		private void LoadDefaults()
		{
//			string file = SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory) + @"\settings\MapViewers.yml";

			string file = Environment.CurrentDirectory + @"\settings\MapViewers.yml";
			using (var sr = new StreamReader(File.OpenRead(file)))
			{
				var str = new YamlStream();
				str.Load(sr);

				var nodeRoot = (YamlMappingNode)str.Documents[0].RootNode;
				foreach (var node in nodeRoot.Children)
				{
					string viewer = ((YamlScalarNode)node.Key).Value;
					var pars = (YamlMappingNode)nodeRoot.Children[new YamlScalarNode(viewer)];
					foreach (var par in pars)
					{
						if (String.Compare(
										viewer,
										"MainView",
										System.Globalization.CultureInfo.InvariantCulture,
										System.Globalization.CompareOptions.OrdinalIgnoreCase) == 0)
						{
							switch (par.Key.ToString().ToUpperInvariant())
							{
								case "LEFT":
									Left = Int32.Parse(par.Value.ToString()); // TODO: Error handling. ->
									break;
								case "TOP":
									Top = Int32.Parse(par.Value.ToString());
									break;
								case "WIDTH":
									Width = Int32.Parse(par.Value.ToString());
									break;
								case "HEIGHT":
									Height = Int32.Parse(par.Value.ToString());
									break;
							}
						}
					}
				}
			}
//			using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
//			{
//				using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
//				{
//					using (var keyMainView = keyMapView.CreateSubKey("MainView"))
//					{
//						Left   = (int)keyMainView.GetValue("Left",   Left);
//						Top    = (int)keyMainView.GetValue("Top",    Top);
//						Width  = (int)keyMainView.GetValue("Width",  Width);
//						Height = (int)keyMainView.GetValue("Height", Height);
//
//						keyMainView.Close();
//					}
//					keyMapView.Close();
//				}
//				keySoftware.Close();
//			}

			var settings = new Settings();
			var handler  = new ValueChangedEventHandler(OnSettingChange);
//			Color.FromArgb(175, 69, 100, 129)

			settings.AddSetting(
							"Animation",
							MapViewPanel.IsAnimated,
							"If true the map will animate itself.",
							"Main",
							handler, false, null);
			settings.AddSetting(
							"Doors",
							false,
							"If true the door tiles will animate themselves.",
							"Main",
							handler, false, null);
			settings.AddSetting(
							"SaveWindowPositions",
							PathsEditor.SaveRegistry,
							"If true the window positions and sizes will be saved in the windows registry.",
							"Main",
							handler, false, null);
			settings.AddSetting(
							"ShowGrid",
							MapViewPanel.Instance.MapView.ShowGrid,
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
							"GraySelection",
							MapViewPanel.Instance.MapView.GraySelection,
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
					using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
					{
						using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
						{
							using (var keyMainView = keyMapView.CreateSubKey("MainView"))
							{
								_mainViewsManager.CloseAllViews();

								WindowState = FormWindowState.Normal;

								keyMainView.SetValue("Left",   Left);
								keyMainView.SetValue("Top",    Top);
								keyMainView.SetValue("Width",  Width);
								keyMainView.SetValue("Height", Height - SystemInformation.CaptionButtonSize.Height);

								keyMainView.Close();
							}
							keyMapView.Close();
						}
						keySoftware.Close();
					}
				}

				_settingsManager.Save(); // save MV_SettingsFile // TODO: Save settings when closing the Options form(s).
			}
		}

		private void OnImageUpdate(object sender, EventArgs e)
		{
			MainWindowsManager.TopView.Control.QuadrantsPanel.Refresh();
		}

/*		private static void myQuit(object sender, string command)
		{
			if (command == "OK")
				Environment.Exit(0);
		} */

		private void OnOnClick(object sender, EventArgs e)
		{
			OnSettingChange(this, "Animation", true);
		}

		private void OnOffClick(object sender, EventArgs e)
		{
			OnSettingChange(this, "Animation", false);
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			if (_mainViewPanel.BaseMap != null)
			{
				_mainViewPanel.BaseMap.Save();
				XConsole.AdZerg("Saved: " + _mainViewPanel.BaseMap.Name);
			}
		}

		private void OnQuitClick(object sender, EventArgs e)
		{
			OnCloseSaveRegistry(null, new CancelEventArgs(true));
			Environment.Exit(0);
		}

		private void OnPathsEditorClick(object sender, EventArgs e)
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
//				miExport.Enabled = true; // disabled in designer w/ Visible=FALSE.

				var tileFactory = new XCTileFactory();
				tileFactory.HandleWarning += _warningHandler.HandleWarning;

				var fileService = new XCMapFileService(tileFactory);

				var baseMap = fileService.Load(desc as XCMapDesc);
				_mainViewPanel.SetMap(baseMap);

				tsEdit.Enabled = true;

				RouteService.CheckNodeBounds(baseMap);

				tsslMap.Text = desc.Label;

				tsslDimensions.Text = (baseMap != null) ? baseMap.MapSize.ToString()
											   : "size: n/a";

				if (miDoors.Checked) // turn off door animations
				{
					miDoors.Checked = false;
					OnDoorsClick(null, null);
				}

				if (!menuShow.Enabled) // open all the forms in the show menu
					_mainMenusManager.StartViewers();

				_mainWindowsManager.SetMap(baseMap); // reset all observer events
			}
//			else
//				miExport.Enabled = false;
		}

		private DialogResult NotifySave()
		{
			if (_mainViewPanel.BaseMap != null && _mainViewPanel.BaseMap.MapChanged)
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
						_mainViewPanel.BaseMap.Save();
						break;

					case DialogResult.Cancel:	// do nothing
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var f = new OptionsForm("MainViewOptions", Settings);
			f.Text = "Main View Options";
			f.Show();
		}

		private void OnSaveImageClick(object sender, EventArgs e)
		{
			if (_mainViewPanel.BaseMap != null)
			{
				sfdSaveDialog.FileName = _mainViewPanel.BaseMap.Name;
				if (sfdSaveDialog.ShowDialog() == DialogResult.OK)
				{
					_loadingProgress.Show();

					try
					{
						_mainViewPanel.BaseMap.SaveGif(sfdSaveDialog.FileName);
					}
					finally
					{
						_loadingProgress.Hide();
					}
				}
			}
		}

		private void OnHq2xClick(object sender, EventArgs e)
		{
			var map = _mainViewPanel.BaseMap as XCMapFile;
			if (map != null)
			{
				map.HQ2X();
				_mainViewPanel.OnResize();
			}
		}

		private void OnDoorsClick(object sender, EventArgs e)
		{
			miDoors.Checked = !miDoors.Checked;

			foreach (XCTile tile in _mainViewPanel.BaseMap.Tiles)
				if (tile.Info.UfoDoor || tile.Info.HumanDoor)	// uhh, human doors don't animate
				{												// only ufo doors animate
					if (miDoors.Checked)						// human doors use their Alternate tile.
						tile.MakeAnimate();
					else
						tile.StopAnimate();
				}
		}

		private void OnResizeClick(object sender, EventArgs e)
		{
			if (_mainViewPanel.MapView.Map != null)
			{
				using (var f = new ChangeMapSizeForm())
				{
					f.Map = _mainViewPanel.MapView.Map;
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						f.Map.ResizeTo(
									f.NewRows,
									f.NewCols,
									f.NewHeight,
									f.AddToCeiling);
						_mainViewPanel.ForceResize();
					}
				}
			}
		}

		private bool _windowFlag = false;

		private void OnMainWindowActivated(object sender, EventArgs e)
		{
			if (!_windowFlag)
			{
				_windowFlag = true;

				foreach (MenuItem it in menuShow.MenuItems)
					if (it.Checked)
						((Form)it.Tag).BringToFront();

				Focus();
				BringToFront();

				_windowFlag = false;
			}
		}

		private void OnInfoClick(object sender, EventArgs e)
		{
			if (_mainViewPanel.BaseMap != null)
			{
				var f = new MapInfoForm();
				f.Show();
				f.Analyze(_mainViewPanel.BaseMap);
			}
		}

		private void OnExportClick(object sender, EventArgs e)
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

		private void OnOpenClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE.
		{}

		private Settings Settings
		{
			get { return _settingsManager["MainWindow"]; }
			set { _settingsManager["MainWindow"] = value; }
		}

		private void OnSelectionBoxClick(object sender, EventArgs e) // NOTE: is disabled w/ Visible=FALSE in designer.
		{
			_mainViewPanel.MapView.DrawSelectionBox = !_mainViewPanel.MapView.DrawSelectionBox;
			tsbSelectionBox.Checked = !tsbSelectionBox.Checked;
		}

		private void OnZoomInClick(object sender, EventArgs e)
		{
			if (Globals.PckImageScale < Globals.MaxPckImageScale)
			{
				Globals.PckImageScale += 0.125;
				Globals.AutoPckImageScale = false;
				tsbAutoZoom.Checked = false;

				_mainViewPanel.SetupMapSize();

				Refresh();

				_mainViewPanel.OnResize();
			}
		}

		private void OnZoomOutClick(object sender, EventArgs e)
		{
			if (Globals.PckImageScale > Globals.MinPckImageScale)
			{
				Globals.PckImageScale -= 0.125;
				Globals.AutoPckImageScale = false;
				tsbAutoZoom.Checked = false;

				_mainViewPanel.SetupMapSize();

				Refresh();

				_mainViewPanel.OnResize();
			}
		}

		private void OnAutoZoomClick(object sender, EventArgs e)
		{
			Globals.AutoPckImageScale = !Globals.AutoPckImageScale;

			if (!Globals.AutoPckImageScale)
				Globals.PckImageScale = 1;

			tsbAutoZoom.Checked = !tsbAutoZoom.Checked;

			_mainViewPanel.SetupMapSize();

			Refresh();

			_mainViewPanel.OnResize();
		}

		public void StatusBarPrintPosition(int col, int row)
		{
			tsslPosition.Text = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"c:{0} r:{1}",
											col, row);
		}



		private Varidia _vars;

		/// <summary>
		/// Transposes all the default viewer sizes and positions from the
		/// embedded MapViewers.yml manifest to a /settings/MapViewers.yml file.
		/// Based on InstallWindow.
		/// </summary>
		private void CreateIni()
		{
			_vars = new Varidia();
			_vars["##RunPath##"] = SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory);

			var info = (PathInfo)SharedSpace.Instance[PathInfo.MapViewers];
			info.CreateDirectory();

			string pfeViewers = info.FullPath;

			// Create MapViewers.yml, will overwrite the file if it exists.
			using (var fs = new FileStream(pfeViewers, FileMode.Create))
			{}

			using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													 .GetManifestResourceStream("MapView._Embedded.MapViewers.yml")))
			using (var fs = new FileStream(pfeViewers, FileMode.Append))
			using (var sw = new StreamWriter(fs))
			{
				while (sr.Peek() != -1)
					sw.WriteLine(sr.ReadLine());
			}
		}
	}
}
