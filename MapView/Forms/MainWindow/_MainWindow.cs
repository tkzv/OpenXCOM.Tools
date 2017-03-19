using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.XCError.WarningConsole;
using MapView.Forms.MainWindow;
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


	public sealed partial class MainWindow
		:
		Form
	{
		private readonly SettingsManager _settingsManager;

		private readonly MapViewPanel _mapViewPanel;
		private readonly LoadingForm _loadingProgress;
		private readonly IWarningHandler _warningHandler;
		private readonly IMainWindowWindowsManager _mainWindowWindowsManager;
		private readonly MainWindowsManager _mainWindowsManager;
		private readonly WindowMenuManager _windowMenuManager;


		public MainWindow()
		{
			LogFile.CreateLogFile();
			LogFile.WriteLine("Starting MAIN MapView window ...");

			InitializeComponent();
			LogFile.WriteLine("MapView window created");

			// WORKAROUND: The size of the form in the designer keeps increasing
			// (for whatever reason) based on the
			// 'SystemInformation.CaptionButtonSize.Height' value (the titlebar
			// height - note that 'SystemInformation.CaptionHeight' seems to be
			// 1 pixel larger, which (for whatever reason) is not the
			// 'SystemInformation.BorderSize.Height'). To prevent all that, in
			// the designer cap the form's Size by setting its MaximumSize to
			// the Size - but now run this code that sets the MaximumSize to
			// "0,0" (unlimited, as wanted). And, as a safety, do the same thing
			// with MinimumSize ....
			//
			// - observed & tested in SharpDevelop 5.1
			//
			// NOTE: This code (the constructor of a Form) shouldn't run when
			// opening the designer; it appears to run only when actually
			// running the application:

			var size = new Size();
			size.Width  = 0;
			size.Height = 0;
			MaximumSize = size; // fu.net
//			MinimumSize = size;


			_mapViewPanel = MapViewPanel.Instance; // "MapView panel created"

			_settingsManager = new SettingsManager();
			_windowMenuManager = new WindowMenuManager(showMenu, miHelp);
			LogFile.WriteLine("Quick Help and About created");

			LoadDefaults();
			LogFile.WriteLine("Default settings loaded");

			Palette.UFOBattle.SetTransparent(true);
			Palette.TFTDBattle.SetTransparent(true);
			Palette.UFOBattle.Grayscale.SetTransparent(true);
			Palette.TFTDBattle.Grayscale.SetTransparent(true);
			LogFile.WriteLine("Palette transparencies set");

			#region Setup SharedSpace information and paths

			var share = SharedSpace.Instance;
			var consoleShare = new ConsoleSharedSpace(share);
			_warningHandler = new ConsoleWarningHandler(consoleShare);

			MainWindowsManager.MainToolStripButtonsFactory = new MainToolStripButtonsFactory(_mapViewPanel);

			_mainWindowsManager = new MainWindowsManager();
			_mainWindowWindowsManager = new MainWindowWindowsManager(_settingsManager, consoleShare);

			_windowMenuManager.SetMenus(consoleShare.GetNewConsole(), GetSettings());

			MainWindowsManager.MainWindowsShowAllManager = _windowMenuManager.CreateShowAll();
			MainWindowsManager.Initialize();
			LogFile.WriteLine("MainWindowsManager initialized");

			share.AllocateObject("MapView",		this);
			share.AllocateObject("AppDir",		Environment.CurrentDirectory);
			share.AllocateObject("SettingsDir",	Environment.CurrentDirectory + @"\settings");
//			share.AllocateObject("CustomDir",	Environment.CurrentDirectory + @"\custom");	// I think this is needed only for PckView.
																							// and so I'll assume '_PckView' handles it.
			LogFile.WriteLine("Environment set");

			var dir = SharedSpace.Instance.GetString("SettingsDir");
			var fileSettings	= new PathInfo(dir, "MVSettings",	"dat");
			var filePaths		= new PathInfo(dir, "Paths",		"pth");
			var fileMapEdit		= new PathInfo(dir, "MapEdit",		"dat");
			var fileImages		= new PathInfo(dir, "Images",		"dat");

			share.AllocateObject(SettingsService.SettingsFile, fileSettings);
			share.AllocateObject("MV_PathsFile",	filePaths);
			share.AllocateObject("MV_MapEditFile",	fileMapEdit);
			share.AllocateObject("MV_ImagesFile",	fileImages);
			LogFile.WriteLine("Paths set");

			#endregion

			if (!filePaths.Exists())
			{
				var iw = new InstallWindow();

				if (iw.ShowDialog(this) != DialogResult.OK)
					Environment.Exit(-1);
			}
			LogFile.WriteLine("Installation checked");

			GameInfo.ParseLine += parseLine; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			LogFile.WriteLine("Line parsed");

			InitGameInfo(filePaths);
			LogFile.WriteLine("GameInfo initialized");

			_mainWindowWindowsManager.Register();

			MainWindowsManager.TileView.TileViewControl.MapChanged += TileView_MapChanged;

			MapViewPanel.ImageUpdate += update; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_mapViewPanel.Dock = DockStyle.Fill;

			_instance = this;

			mapList.TreeViewNodeSorter = new System.Collections.CaseInsensitiveComparer();

			toolStripContainer1.ContentPanel.Controls.Add(_mapViewPanel);
			MainWindowsManager.MainToolStripButtonsFactory.MakeToolstrip(toolStrip);
			toolStrip.Enabled = false;
			toolStrip.Items.Add(new ToolStripSeparator());

			try
			{
				_mapViewPanel.MapView.CursorSprite = new CursorSprite(GameInfo.CachePckFile(
															SharedSpace.Instance.GetString("cursorFile"),
															String.Empty,
															2,
															Palette.UFOBattle));
			}
			catch
			{
				try
				{
					_mapViewPanel.MapView.CursorSprite = new CursorSprite(GameInfo.CachePckFile(
															SharedSpace.Instance.GetString("cursorFile"),
															String.Empty,
															4,
															Palette.TFTDBattle));
				}
				catch
				{
					_mapViewPanel.Cursor = null;
				}
			}
			LogFile.WriteLine("Cursor loaded");

			InitList();
			LogFile.WriteLine("Tilesets created and loaded to MapView's tree panel");

			if (fileSettings.Exists())
			{
				_settingsManager.Load(fileSettings.ToString());
				LogFile.WriteLine("User settings loaded");
			}
			else
				LogFile.WriteLine("User settings NOT loaded - no settings file to load");


			OnResize(null);
			this.Closing += new CancelEventHandler(closing);

			_loadingProgress = new LoadingForm();
			Bmp.LoadingEvent += _loadingProgress.Update;

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

//			if (Directory.Exists(sharedSpace["CustomDir"].ToString()))
//			{
//				xConsole.AddLine("Custom directory exists: " + sharedSpace["CustomDir"].ToString());
//				foreach (string s in Directory.GetFiles(sharedSpace["CustomDir"].ToString()))
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
			GameInfo.Init(Palette.UFOBattle, filePaths);
		}

		private static MainWindow _instance;

		public static MainWindow Instance
		{
			get { return _instance; }
		}

		private void parseLine(XCom.KeyVal line, XCom.VarCollection vars)
		{
			switch (line.Keyword.ToLower(System.Globalization.CultureInfo.InvariantCulture))
			{
				case "cursor":
					if (line.Rest.EndsWith(@"\", StringComparison.Ordinal))
						SharedSpace.Instance.AllocateObject("cursorFile", line.Rest + "CURSOR");
					else
						SharedSpace.Instance.AllocateObject("cursorFile", line.Rest + @"\CURSOR");
					break;

//				case "logfile":
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
			GetSettings()[key].Value = val;
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

		private class SortableTreeNode
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

		private void addMaps(TreeNode tn, IDictionary<string, IMapDesc> maps)
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
			mapList.Nodes.Add(node);

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
			mapList.Nodes.Clear();

			foreach (string key in GameInfo.TilesetInfo.Tilesets.Keys)
				AddTileset(GameInfo.TilesetInfo.Tilesets[key]);
		}

		private void closing(object sender, CancelEventArgs e)
		{
			if (NotifySave() == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
			else
			{
				_windowMenuManager.Dispose();

				if (PathsEditor.SaveRegistry)
				{
					var keySoftware = Registry.CurrentUser.CreateSubKey("Software");
					var keyMapView  = keySoftware.CreateSubKey("MapView");
					var keyMainView = keyMapView.CreateSubKey("MainView");

					_mainWindowWindowsManager.CloseAll();

					WindowState = FormWindowState.Normal;

					keyMainView.SetValue("Left",	Left);
					keyMainView.SetValue("Top",		Top);
					keyMainView.SetValue("Width",	Width);
					keyMainView.SetValue("Height",	Height - SystemInformation.CaptionButtonSize.Height);

//					keyMainView.SetValue("Animation", onItem.Checked.ToString());
//					keyMainView.SetValue("Doors", miDoors.Checked.ToString());

					keyMainView.Close();
					keyMapView.Close();
					keySoftware.Close();
				}

				_settingsManager.Save();
			}
		}

		private void LoadDefaults()
		{
			var keySoftware = Registry.CurrentUser.CreateSubKey("Software");
			var keyMapView  = keySoftware.CreateSubKey("MapView");
			var keyMainView = keyMapView.CreateSubKey("MainView");

			Left	= (int)keyMainView.GetValue("Left",		Left);
			Top		= (int)keyMainView.GetValue("Top",		Top);
			Width	= (int)keyMainView.GetValue("Width",	Width);
			Height	= (int)keyMainView.GetValue("Height",	Height);

			keyMainView.Close();
			keyMapView.Close();
			keySoftware.Close();

			var settings = new Settings();

//			Color.FromArgb(175, 69, 100, 129)

			var change = new ValueChangedDelegate(ChangeSetting);

			settings.AddSetting(
							"Animation",
							MapViewPanel.Updating,
							"If true the map will animate itself",
							"Main",
							change, false, null);
			settings.AddSetting(
							"Doors",
							false,
							"If true the door tiles will animate themselves",
							"Main",
							change, false, null);
			settings.AddSetting(
							"SaveWindowPositions",
							PathsEditor.SaveRegistry,
							"If true the window positions and sizes will be saved in the windows registry",
							"Main",
							change, false, null);
			settings.AddSetting(
							"UseGrid",
							MapViewPanel.Instance.MapView.UseGrid,
							"If true a grid will show up at the current level of editing",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridColor",
							MapViewPanel.Instance.MapView.GridColor,
							"Color of the grid in (a,r,g,b) format",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridLineColor",
							MapViewPanel.Instance.MapView.GridLineColor,
							"Color of the lines that make up the grid",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"GridLineWidth",
							MapViewPanel.Instance.MapView.GridLineWidth,
							"Width of the grid lines in pixels",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
			settings.AddSetting(
							"SelectGrayscale",
							MapViewPanel.Instance.MapView.SelectGrayscale,
							"If true the selection area will show up in gray",
							"MapView",
							null, true, MapViewPanel.Instance.MapView);
//			settings.AddSetting(
//							"SaveOnExit",
//							true,
//							"If true these settings will be saved on program exit",
//							"Main",
//							null, false, null);

			SetSettings(settings);
		}

		private void update(object sender, EventArgs e)
		{
			MainWindowsManager.TopView.Control.BottomPanel.Refresh();
		}

/*		private static void myQuit(object sender, string command)
		{
			if (command == "OK")
				Environment.Exit(0);
		} */

		private void onItem_Click(object sender, System.EventArgs e)
		{
			ChangeSetting(this, "Animation", true);
		}

		private void offItem_Click(object sender, System.EventArgs e)
		{
			ChangeSetting(this, "Animation", false);
		}

		private void saveItem_Click(object sender, System.EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				_mapViewPanel.BaseMap.Save();
				xConsole.AddLine("Saved: " + _mapViewPanel.BaseMap.Name);
			}
		}

		private void quititem_Click(object sender, System.EventArgs e)
		{
			closing(null, new CancelEventArgs(true));
			Environment.Exit(0);
		}

		private void miPaths_Click(object sender, System.EventArgs e)
		{
			var share = SharedSpace.Instance["MV_PathsFile"];

			var paths = new PathsEditor(share.ToString());
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
		private void mapList_BeforeSelect(object sender, CancelEventArgs e)
		{
			if (NotifySave() == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
			else if (mapList.SelectedNode != null)
			{
				mapList.SelectedNode.BackColor = SystemColors.Control;
			}
		}

		/// <summary>
		/// Colorizes the background-field of the newly selected label in the
		/// left-panel's MapBlocks' tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mapList_AfterSelect(object sender, TreeViewEventArgs e)
		{
			mapList.SelectedNode.BackColor = Color.Gold;
			LoadSelectedMap();
		}

		private void TileView_MapChanged()
		{
			LoadSelectedMap();
		}

		private void LoadSelectedMap()
		{
			var desc = mapList.SelectedNode.Tag as IMapDesc;
			if (desc != null)
			{
				miExport.Enabled = true;

				var tileFactory = new XCTileFactory();
				tileFactory.HandleWarning += _warningHandler.HandleWarning;

				var mapService = new XCMapFileService(tileFactory);

				var map = mapService.Load(desc as XCMapDesc);
				_mapViewPanel.SetMap(map);

				toolStrip.Enabled = true;

				RouteService.CheckNodeBounds(map);

				statusMapName.Text = desc.Name;

				tsMapSize.Text = (map != null) ? map.MapSize.ToString()
											   : "size: n/a";

				if (miDoors.Checked) // turn off door animations
				{
					miDoors.Checked = false;
					miDoors_Click(null, null);
				}

				if (!showMenu.Enabled) // open all the forms in the show menu once
					_windowMenuManager.LoadState();

				_mainWindowsManager.SetMap(map); // reset all observer events
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

		private void miOptions_Click(object sender, System.EventArgs e)
		{
			var pf = new PropertyForm("MainViewSettings", GetSettings());
			pf.Text = "MainWindow Options";
			pf.Show();
		}

		private void miSaveImage_Click(object sender, System.EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				saveFile.FileName = _mapViewPanel.BaseMap.Name;
				if (saveFile.ShowDialog() == DialogResult.OK)
				{
					_loadingProgress.Show();

					try
					{
						_mapViewPanel.BaseMap.SaveGif(saveFile.FileName);
					}
					finally
					{
						_loadingProgress.Hide();
					}
				}
			}
		}

		private void miHq_Click(object sender, System.EventArgs e)
		{
			var map = _mapViewPanel.BaseMap as XCMapFile;
			if (map != null)
			{
				map.Hq2x();
				_mapViewPanel.OnResize();
			}
		}

		private void miDoors_Click(object sender, System.EventArgs e)
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

		private void miResize_Click(object sender, System.EventArgs e)
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

		private void MainWindow_Activated(object sender, System.EventArgs e)
		{
			if (!_windowFlag)
			{
				_windowFlag = true;

				foreach (MenuItem mi in showMenu.MenuItems)
					if (mi.Checked)
						((Form)mi.Tag).BringToFront();

				Focus();
				BringToFront();

				_windowFlag = false;
			}
		}

		private void miInfo_Click(object sender, System.EventArgs e)
		{
			if (_mapViewPanel.BaseMap != null)
			{
				var f = new MapInfoForm();
				f.Show();
				f.BaseMap = _mapViewPanel.BaseMap;
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

		private void SetSettings(Settings settings)
		{
			_settingsManager["MainWindow"] = settings;
		}

		private Settings GetSettings()
		{
			return _settingsManager["MainWindow"];
		}

		private void drawSelectionBoxButton_Click(object sender, EventArgs e)
		{
			_mapViewPanel.MapView.DrawSelectionBox = !_mapViewPanel.MapView.DrawSelectionBox;
			drawSelectionBoxButton.Checked = !drawSelectionBoxButton.Checked;
		}

		private void ZoomInButton_Click(object sender, EventArgs e)
		{
			if (Globals.PckImageScale < Globals.MaxPckImageScale)
			{
				Globals.PckImageScale += 0.125;
				Globals.AutoPckImageScale = false;
				AutoZoomButton.Checked = false;

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
				AutoZoomButton.Checked = false;

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

			AutoZoomButton.Checked = !AutoZoomButton.Checked;

			_mapViewPanel.SetupMapSize();

			Refresh();

			_mapViewPanel.OnResize();
		}

		public void StatusBarPrintPosition(int col, int row)
		{
			statusPosition.Text = "c:" + col + " r:" + row;
		}
	}
}
