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
//using MapView.SettingServices;

//using Microsoft.Win32;

using XCom;
using XCom.GameFiles.Map;
using XCom.GameFiles.Map.RouteData;
//using XCom.Interfaces;
using XCom.Interfaces.Base;

using YamlDotNet.RepresentationModel;	// read values (deserialization)
using YamlDotNet.Serialization;			// write values


namespace MapView
{
	/// <summary>
	/// Instantiates a MainView screen as the basis for all user-interaction.
	/// </summary>
	internal sealed partial class XCMainWindow
		:
			Form
	{
		#region Fields & Properties
		private readonly MainViewUnderlay      _mainViewUnderlay; // has static functs.

		private readonly ViewersManager        _viewersManager;
		private readonly ViewerFormsManager    _viewerFormsManager; // has static functs.
		private readonly MainMenusManager      _mainMenusManager;

		private readonly LoadingForm           _loadingProgress;
		private readonly ConsoleWarningHandler _warningHandler;


		private readonly SettingsManager _settingsManager;
		private Settings Settings
		{
			get { return _settingsManager["MainWindow"]; }
			set { _settingsManager["MainWindow"] = value; }
		}

		private static XCMainWindow _instance;
		internal static XCMainWindow Instance
		{
			get { return _instance; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// This is where the user-app end of things *really* starts.
		/// </summary>
		internal XCMainWindow()
		{
			// TODO: further optimize this loading sequence ....

			LogFile.CreateLogFile();
			LogFile.WriteLine("Starting MAIN MapView window ...");


			var share = SharedSpace.Instance;

			share.SetShare("MapView", this);
			share.SetShare(
						SharedSpace.ApplicationDirectory,
						Environment.CurrentDirectory);

			string dir = share.SetShare(
									SharedSpace.SettingsDirectory,
									Path.Combine(Environment.CurrentDirectory, "settings"))
							  .ToString();
			LogFile.WriteLine("Environment cached.");


			var pathViewerPositions = new PathInfo(dir, "MapViewers", "yml");
			share.SetShare(PathInfo.MapViewers, pathViewerPositions);

			var pathSettings = new PathInfo(dir, "MVSettings", "cfg");
			share.SetShare(PathInfo.SettingsFile, pathSettings);

			var pathPaths = new PathInfo(dir, "Paths", "cfg");
			share.SetShare(PathInfo.PathsFile, pathPaths);

			var pathMapEdit = new PathInfo(dir, "MapEdit", "cfg");
			share.SetShare(PathInfo.MapEditFile, pathMapEdit);

			var pathImages = new PathInfo(dir, "Images", "cfg");
			share.SetShare(PathInfo.ImagesFile, pathImages);

			LogFile.WriteLine("PathInfo cached.");


			if (!pathPaths.FileExists()) // check if Paths.cfg exists yet
			{
				using (var f = new InstallationForm())
					if (f.ShowDialog(this) != DialogResult.OK)
						Environment.Exit(-1); // wtf -1

				LogFile.WriteLine("Installation files created.");
			}
			else
				LogFile.WriteLine("Paths.Cfg file exists.");


			if (!pathViewerPositions.FileExists()) // check if MapViewers.yml exists yet
			{
				CreateViewersFile();
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
			tvMaps.BeforeSelect += OnMapTreeSelect;
			tvMaps.AfterSelect  += OnMapTreeSelected;
			// welcome to your new home

			_instance = this;


			_settingsManager = new SettingsManager(); // goes before LoadSettings()

			Settings = new Settings(); // NOTE: Settings hold Options. And should probly be called 'Options'.

			LoadSettings();									// TODO: check if this should go after the managers load
			LogFile.WriteLine("MainView Settings loaded.");	// since managers might be re-instantiating needlessly
															// when OnSettingsChange() runs ....

			_mainViewUnderlay = MainViewUnderlay.Instance;
			_mainViewUnderlay.Dock = DockStyle.Fill;
			_mainViewUnderlay.BorderStyle = BorderStyle.Fixed3D;
			LogFile.WriteLine("MainView panel instantiated.");


			Palette.UfoBattle.EnableTransparency(true);
			Palette.TftdBattle.EnableTransparency(true);
			Palette.UfoBattle.Grayscale.EnableTransparency(true);
			Palette.TftdBattle.Grayscale.EnableTransparency(true);
			LogFile.WriteLine("Palette transparencies set.");


			var consoleShare = new ConsoleSharedSpace(share);
			_warningHandler  = new ConsoleWarningHandler(consoleShare);


			_viewerFormsManager = new ViewerFormsManager();
			_viewersManager     = new ViewersManager(_settingsManager, consoleShare);
			_mainMenusManager   = new MainMenusManager(menuView, menuHelp);

			_mainMenusManager.PopulateMenus(consoleShare.GetConsole(), Settings);
			LogFile.WriteLine("MainView menus populated.");

			ViewerFormsManager.HideViewersManager = _mainMenusManager.CreateShowHideManager(); // subsidiary viewers hide when PckView is invoked from TileView.
			LogFile.WriteLine("HideViewersManager created.");


			ViewerFormsManager.EditFactory = new EditButtonsFactory(_mainViewUnderlay);
			ViewerFormsManager.Initialize();
			LogFile.WriteLine("ViewerFormsManager initialized.");


//			GameInfo.ParseConfigLineEvent += OnParseConfigLine;
			InitializeGameInfo(pathPaths);
//			GameInfo.ParseConfigLineEvent -= OnParseConfigLine;
			LogFile.WriteLine("GameInfo initialized.");


			_viewersManager.ManageViewers();


			ViewerFormsManager.TileView.Control.PckSavedEvent += OnPckSavedEvent;

			MainViewUnderlay.AnimationUpdateEvent += OnAnimationUpdate;	// FIX: "Subscription to static events without unsubscription may cause memory leaks."
																		// NOTE: it's not really a problem, since both publisher and subscriber are expected to
																		// live the lifetime of the app. And this class, XCMainWindow, never re-instantiates.
			tvMaps.TreeViewNodeSorter = StringComparer.OrdinalIgnoreCase;

			tscPanel.ContentPanel.Controls.Add(_mainViewUnderlay);

			ViewerFormsManager.EditFactory.CreateEditorStrip(tsEdit);
			tsEdit.Enabled = false;


			PckSpriteCollection cuboid = null;
			try
			{
				cuboid = GameInfo.CachePckPack(
											SharedSpace.Instance.GetString(SharedSpace.CursorFile),
											String.Empty,
											2,
											Palette.UfoBattle);
				_mainViewUnderlay.MainViewOverlay.Cuboid = new CursorSprite(cuboid);
			}
			catch
			{
				try
				{
					cuboid = GameInfo.CachePckPack(
												SharedSpace.Instance.GetString(SharedSpace.CursorFile),
												String.Empty,
												4,
												Palette.TftdBattle);
					_mainViewUnderlay.MainViewOverlay.Cuboid = new CursorSprite(cuboid);
				}
				catch
				{
					_mainViewUnderlay.Cursor = null; // NOTE: this is the system cursor, NOT the cuboid-sprite.
					throw; // TODO: there's got to be a better way to do that ....
				}
				throw;
			}
			LogFile.WriteLine("Cursor loaded.");

			PopulateMapTree();
			LogFile.WriteLine("Tilesets created and loaded to tree panel.");

			if (pathSettings.FileExists())
			{
				_settingsManager.Load(pathSettings.FullPath);
				LogFile.WriteLine("User settings loaded.");
			}
			else
				LogFile.WriteLine("User settings NOT loaded - no settings file to load.");


			Closing += OnCloseSaveRegistry;


			_loadingProgress = new LoadingForm();
			XCBitmap.LoadingEvent += _loadingProgress.HandleProgress; // TODO: fix or remove that.


			// I should rewrite the hq2x wrapper for .NET sometime -- not the code it's pretty insane
//			if (!File.Exists("hq2xa.dll"))
			miHq.Visible = false;

//			LogFile.WriteLine("Loading user-made plugins");

			/****************************************/
			// Copied from PckView
//			loadedTypes = new LoadOfType<MapDesc>();
//			sharedSpace["MapMods"] = loadedTypes.AllLoaded;

			// There are no currently loadable maps in this assembly so this is more for future use
//			loadedTypes.LoadFrom(Assembly.GetAssembly(typeof(XCom.Interfaces.Base.MapDesc)));

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
		#endregion


		private static void InitializeGameInfo(PathInfo pathInfo)
		{
			GameInfo.Initialize(Palette.UfoBattle, pathInfo);
		}

/*		private void OnParseConfigLine(KeyvalPair line, Varidia vars)
		{
			switch (line.Keyword.ToUpperInvariant())
			{
				case "CURSOR": // NOTE: moved to GameInfo.InitializeGameInfo()
				{
					string directorySeparator = String.Empty;
					if (!line.Value.EndsWith(@"\", StringComparison.Ordinal))
						directorySeparator = @"\";

					SharedSpace.Instance.SetShare(
											SharedSpace.CursorFile,
											line.Value + directorySeparator + SharedSpace.Cursor);
					break;
				}

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
		} */

		private void PopulateMapTree()
		{
			tvMaps.Nodes.Clear();

			foreach (string key in GameInfo.TilesetInfo.Tilesets.Keys)
				AddMapGroup(GameInfo.TilesetInfo.Tilesets[key]);
		}

		private void AddMapGroup(TilesetBase tileset)
		{
			var node = new SortableTreeNode(tileset.Name);
			node.Tag = tileset;
			tvMaps.Nodes.Add(node);

			foreach (string key in tileset.Subsets.Keys)
			{
				var treeGroup = new SortableTreeNode(key);
				treeGroup.Tag = tileset.Subsets[key];
				node.Nodes.Add(treeGroup);

				AddMapNodes(treeGroup, tileset.Subsets[key]);
			}
		}

		private static void AddMapNodes(TreeNode tn, IDictionary<string, MapDesc> maps)
		{
			foreach (string key in maps.Keys)
			{
				var node = new SortableTreeNode(key);
				node.Tag = maps[key];
				tn.Nodes.Add(node);
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


		#region Settings
		// headers
		private const string Global              = "Global";
		private const string MapView             = "MapView";
		private const string Sprites             = "Sprites";

		// options
		private const string Animation           = "Animation";
		private const string Doors               = "Doors";
		private const string SaveWindowPositions = "SaveWindowPositions";
//		private const string SaveOnExit          = "SaveOnExit";

		private const string ShowGrid            = "ShowGrid";
		private const string GridLayerColor      = "GridLayerColor";
		private const string GridLayerOpacity    = "GridLayerOpacity";
		private const string GridLineColor       = "GridLineColor";
		private const string GridLineWidth       = "GridLineWidth";

		private const string GraySelection       = "GraySelection";

		private const string SpriteDarkness      = "SpriteDarkness";
		private const string Interpolation       = "Interpolation";


		private void LoadSettings()
		{
			string file = Path.Combine(SharedSpace.Instance.GetString(SharedSpace.SettingsDirectory), PathInfo.YamlViewers);
			using (var sr = new StreamReader(File.OpenRead(file)))
			{
				var str = new YamlStream();
				str.Load(sr);

				var nodeRoot = (YamlMappingNode)str.Documents[0].RootNode;
				foreach (var node in nodeRoot.Children)
				{
					string viewer = ((YamlScalarNode)node.Key).Value;
					if (String.Equals(viewer, "MainView", StringComparison.OrdinalIgnoreCase))
					{
						var keyvals = (YamlMappingNode)nodeRoot.Children[new YamlScalarNode(viewer)];
						foreach (var keyval in keyvals)
						{
							switch (keyval.Key.ToString().ToUpperInvariant())
							{
								case "LEFT": // TODO: Error handling. ->
									Left   = Int32.Parse(keyval.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
									break;
								case "TOP":
									Top    = Int32.Parse(keyval.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
									break;
								case "WIDTH":
									Width  = Int32.Parse(keyval.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
									break;
								case "HEIGHT":
									Height = Int32.Parse(keyval.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
									break;
							}
						}
					}
				}
			}

			// kL_note: This is for retrieving MainViewer size and position from
			// the Windows Registry:
//			using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
//			using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
//			using (var keyMainView = keyMapView.CreateSubKey("MainView"))
//			{
//				Left   = (int)keyMainView.GetValue("Left",   Left);
//				Top    = (int)keyMainView.GetValue("Top",    Top);
//				Width  = (int)keyMainView.GetValue("Width",  Width);
//				Height = (int)keyMainView.GetValue("Height", Height);
//				keyMainView.Close();
//				keyMapView.Close();
//				keySoftware.Close();
//			}

			var handler = new OptionChangedEventHandler(OnSettingChange);

			Settings.AddSetting(
							Animation,
							MainViewUnderlay.IsAnimated,
							"If true the sprites will animate",
							Global,
							handler);
			Settings.AddSetting(
							Doors,
							false,
							"If true the doors will animate",
							Global,
							handler);
			Settings.AddSetting(
							SaveWindowPositions,
							true, //PathsEditor.SaveRegistry,
							"If true the window positions and sizes will be saved",
							Global,
							handler);

			Settings.AddSetting(
							ShowGrid,
							MainViewUnderlay.Instance.MainViewOverlay.ShowGrid,
							"If true a grid will display at the current level of editing",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);
			Settings.AddSetting(
							GridLayerColor,
							MainViewUnderlay.Instance.MainViewOverlay.GridLayerColor,
							"Color of the grid",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);
			Settings.AddSetting(
							GridLayerOpacity,
							MainViewUnderlay.Instance.MainViewOverlay.GridLayerOpacity,
							"Opacity of the grid (0..255)",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);
			Settings.AddSetting(
							GridLineColor,
							MainViewUnderlay.Instance.MainViewOverlay.GridLineColor,
							"Color of the lines that make up the grid",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);
			Settings.AddSetting(
							GridLineWidth,
							MainViewUnderlay.Instance.MainViewOverlay.GridLineWidth,
							"Width of the grid lines in pixels",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);
			Settings.AddSetting(
							GraySelection,
							MainViewUnderlay.Instance.MainViewOverlay.GraySelection,
							"If true the selection area will show up in gray",
							MapView,
							null, MainViewUnderlay.Instance.MainViewOverlay);

			Settings.AddSetting(
							SpriteDarkness,
							MainViewUnderlay.Instance.MainViewOverlay.SpriteDarkness,
							"The darkness of the tile sprites (10..100)",
							Sprites,
							null, MainViewUnderlay.Instance.MainViewOverlay);

			string desc = "The technique used for resizing sprites (0..7)" + Environment.NewLine
						+ "0 - default"                                    + Environment.NewLine
						+ "1 - low (default)"                              + Environment.NewLine
						+ "2 - high"                                       + Environment.NewLine
						+ "3 - bilinear (default)"                         + Environment.NewLine
						+ "4 - bicubic"                                    + Environment.NewLine
						+ "5 - nearest neighbor"                           + Environment.NewLine
						+ "6 - high quality bilinear"                      + Environment.NewLine
						+ "7 - high quality bicubic";
			Settings.AddSetting(
							Interpolation,
							MainViewUnderlay.Instance.MainViewOverlay.Interpolation,
							desc,
							Sprites,
							null, MainViewUnderlay.Instance.MainViewOverlay);

//			Settings.AddSetting(
//							SaveOnExit,
//							true,
//							"If true these settings will be saved on program exit",
//							Main);
		}

		private void OnSettingChange(object sender, string key, object val)
		{
			Settings[key].Value = val;
			switch (key)
			{
				case Animation:
					miOn.Checked = (bool)val;		// NOTE: 'miOn.Checked' and 'miOff.Checked' are used
					miOff.Checked = !miOn.Checked;	// by the F1 and F2 keys to switch animations on/off.
					MainViewUnderlay.Animate(miOn.Checked);

					if (!miOn.Checked) // show the doorsprites closed in TileView and QuadrantPanel.
					{
						if (miDoors.Checked) // toggle off doors if general animations stop.
						{
							miDoors.Checked = false;
							ToggleDoorSprites(false);
						}
						ViewerFormsManager.TileView.Refresh();
						ViewerFormsManager.TopView.Control.QuadrantsPanel.Refresh();
					}
					else if (miOn.Checked && miDoors.Checked) // doors need to animate if they were already toggled on.
						ToggleDoorSprites(true);
					break;

				case Doors:
					miDoors.Checked = (bool)val; // NOTE: 'miDoors.Checked' is used by the F3 key to toggle door animations.

					if (miOn.Checked)
					{
						ToggleDoorSprites(miDoors.Checked);
					}
					else if (miDoors.Checked) // switch to the doors' alt-tile (whether ufo-door or wood-door)
					{
						if (_mainViewUnderlay.MapBase != null) // NOTE: MapBase is null on MapView load.
						{
							foreach (XCTile tile in _mainViewUnderlay.MapBase.Tiles)
								tile.SetDoorToAlternateSprite();

							Refresh();
						}
					}
					else // switch doors to Image1.
						ToggleDoorSprites(false);
					break;

				case SaveWindowPositions:
//					PathsEditor.SaveRegistry = (bool)val; // TODO: find a place to cache this value.
					break;

				case ShowGrid:
					MainViewUnderlay.Instance.MainViewOverlay.ShowGrid = (bool)val;
					break;

				case GridLayerColor:
					MainViewUnderlay.Instance.MainViewOverlay.GridLayerColor = (Color)val;
					break;

				case GridLayerOpacity:
					MainViewUnderlay.Instance.MainViewOverlay.GridLayerOpacity = (int)val;
					break;

				case GridLineColor:
					MainViewUnderlay.Instance.MainViewOverlay.GridLineColor = (Color)val;
					break;

				case GridLineWidth:
					MainViewUnderlay.Instance.MainViewOverlay.GridLineWidth = (int)val;
					break;

				case SpriteDarkness:
					MainViewUnderlay.Instance.MainViewOverlay.SpriteDarkness = (int)val;
					break;

				case Interpolation:
					MainViewUnderlay.Instance.MainViewOverlay.Interpolation = (int)val;
					break;

				// NOTE: 'GraySelection' is handled. reasons ...
			}
		}
		#endregion


		private void OnCloseSaveRegistry(object sender, CancelEventArgs args)
		{
			if (NotifySave() == DialogResult.Cancel)
			{
				args.Cancel = true;
			}
			else
			{
				//LogFile.WriteLine("OnCloseSaveRegistry MainView");
				_mainMenusManager.IsQuitting();

				_settingsManager.Save(); // save MV_SettingsFile // TODO: Save Settings when closing the Options form(s).


//				if (PathsEditor.SaveRegistry) // TODO: re-implement.
				{
					WindowState = FormWindowState.Normal;
					_viewersManager.CloseSubsidiaryViewers();

					string path = SharedSpace.Instance.GetString(SharedSpace.SettingsDirectory);
					string src  = Path.Combine(path, PathInfo.YamlViewers);
					string dst  = Path.Combine(path, PathInfo.YamlViewersOld);

					File.Copy(src, dst, true);

					using (var sr = new StreamReader(File.OpenRead(dst))) // but now use dst as src ->

					using (var fs = new FileStream(src, FileMode.Create)) // overwrite previous config.
					using (var sw = new StreamWriter(fs))
					{
						while (sr.Peek() != -1)
						{
							string line = sr.ReadLine();

							if (String.Equals(line, "MainView:", StringComparison.OrdinalIgnoreCase))
							{
								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine(); // heh

								object node = new
								{
									MainView = new
									{
										Left   = Left, // relax, YamlDotNet figures it out.
										Top    = Top,
										Width  = Width,
										Height = Height - SystemInformation.CaptionButtonSize.Height
									},
								};

								var ser = new Serializer();
								ser.Serialize(sw, node);
							}
							else
								sw.WriteLine(line);
						}
					}
					File.Delete(dst);
				}

				// kL_note: This is for storing MainViewer size and position in
				// the Windows Registry:
/*				if (PathsEditor.SaveRegistry)
				{
					using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
					using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
					using (var keyMainView = keyMapView.CreateSubKey("MainView"))
					{
						_mainViewsManager.CloseAllViewers();
						WindowState = FormWindowState.Normal;
						keyMainView.SetValue("Left",   Left);
						keyMainView.SetValue("Top",    Top);
						keyMainView.SetValue("Width",  Width);
						keyMainView.SetValue("Height", Height - SystemInformation.CaptionButtonSize.Height);
						keyMainView.Close();
						keyMapView.Close();
						keySoftware.Close();
					}
				} */
			}
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			ViewerFormsManager.TopView.Control.QuadrantsPanel.Refresh();
		}

/*		private static void myQuit(object sender, string command)
		{
			if (command == "OK")
				Environment.Exit(0);
		} */

		/// <summary>
		/// Fired by the F1 key to turn animations On.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOnClick(object sender, EventArgs e)
		{
			OnSettingChange(this, Animation, true);
		}

		/// <summary>
		/// Fired by the F2 key to turn animations Off.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOffClick(object sender, EventArgs e)
		{
			OnSettingChange(this, Animation, false);
		}

		/// <summary>
		/// Fired by the F3 key to toggle door animations.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnToggleDoorsClick(object sender, EventArgs e)
		{
			OnSettingChange(this, Doors, !miDoors.Checked);
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MapBase != null)
				_mainViewUnderlay.MapBase.Save();
		}

		private void OnQuitClick(object sender, EventArgs e)
		{
			OnCloseSaveRegistry(null, new CancelEventArgs(true));
			Environment.Exit(0);
		}

		private void OnPathsEditorClick(object sender, EventArgs e)
		{
			var share = SharedSpace.Instance[PathInfo.PathsFile];

			using (var f = new PathsEditor(share.ToString()))
				f.ShowDialog();

			var pathInfo = (PathInfo)share;

//			GameInfo.ParseConfigLineEvent += OnParseConfigLine;
			InitializeGameInfo(pathInfo);
//			GameInfo.ParseConfigLineEvent -= OnParseConfigLine;

			PopulateMapTree();
		}

		/// <summary>
		/// De-colorizes the background-field of a previously selected label
		/// in the left-panel's MapBlocks' tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeSelect(object sender, CancelEventArgs e)
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
		private void OnMapTreeSelected(object sender, TreeViewEventArgs e)
		{
			tvMaps.SelectedNode.BackColor = Color.Gold;
			LoadSelectedMap();
		}

		/// <summary>
		/// Reloads the map when a save is done in PckView (via TileView).
		/// </summary>
		private void OnPckSavedEvent()
		{
			LoadSelectedMap();
		}

		private void LoadSelectedMap()
		{
			var desc = tvMaps.SelectedNode.Tag as MapDesc;
			if (desc != null)
			{
//				miExport.Enabled = true; // disabled in designer w/ Visible=FALSE.

				_mainViewUnderlay.MainViewOverlay.FirstClick = false;

				var tileFactory = new XCTileFactory();
				tileFactory.WarningEvent += _warningHandler.HandleWarning;	// used to send a message to the Console if
																			// a DeadTile or AlternateTile is out of bounds.
				var fileService = new XCMapFileService(tileFactory);

				var mapBase = fileService.Load(desc as XCMapDesc);
				_mainViewUnderlay.MapBase = mapBase;

				tsEdit.Enabled = true;

				RouteCheckService.CheckNodeBounds(mapBase);

				tsslMap.Text = desc.Label;
				tsslDimensions.Text = (mapBase != null) ? mapBase.MapSize.ToString()
														: "size: n/a";
				tsslPosition.Text = String.Empty;
				ViewerFormsManager.RouteView.Control.ClearSelectedLocation();

				Settings[Doors].Value = false; // toggle off door-animations; not sure that this is necessary to do.
				miDoors.Checked = false;
				ToggleDoorSprites(false);

				if (!menuView.Enabled) // open/close the forms that appear in the Views menu.
					_mainMenusManager.StartAllViewers();

				_viewerFormsManager.SetObservers(mapBase); // reset all observer events
			}
//			else
//				miExport.Enabled = false;
		}

		private void ToggleDoorSprites(bool animate)
		{
			if (_mainViewUnderlay.MapBase != null) // NOTE: MapBase is null on MapView load.
			{
				foreach (XCTile tile in _mainViewUnderlay.MapBase.Tiles)
					tile.SetDoorSprites(animate);

				Refresh();
			}
		}


		private DialogResult NotifySave()
		{
			if (_mainViewUnderlay.MapBase != null && _mainViewUnderlay.MapBase.MapChanged)
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
						_mainViewUnderlay.MapBase.Save();
						break;

					case DialogResult.Cancel:	// do nothing
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		private Form _foptions;
		private bool _closing;

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var it = (MenuItem)sender;
			if (!it.Checked)
			{
				it.Checked = true;

				_foptions = new OptionsForm("MainViewOptions", Settings);
				_foptions.Text = "Main View Options";

				_foptions.Show();

				_foptions.Closing += (sender1, e1) =>
				{
					if (!_closing)
						OnOptionsClick(sender, e);

					_closing = false;
				};
			}
			else
			{
				_closing = true;

				it.Checked = false;
				_foptions.Close();
			}
		}

		private void OnSaveImageClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MapBase != null)
			{
				sfdSaveDialog.FileName = _mainViewUnderlay.MapBase.Label;
				if (sfdSaveDialog.ShowDialog() == DialogResult.OK)
				{
					_loadingProgress.Show();

					try
					{
						_mainViewUnderlay.MapBase.SaveGif(sfdSaveDialog.FileName);
					}
					finally
					{
						_loadingProgress.Hide();
					}
				}
			}
		}

		private void OnHq2xClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE.
		{
//			var map = _mainViewPanel.MapBase as XCMapFile;
//			if (map != null)
//			{
//				map.HQ2X();
//				_mainViewPanel.OnResize();
//			}
		}

		private void OnMapResizeClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MainViewOverlay.MapBase != null)
			{
				using (var f = new MapResizeForm())
				{
					f.MapBase = _mainViewUnderlay.MainViewOverlay.MapBase;
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						f.MapBase.MapResize(
										f.Rows,
										f.Cols,
										f.Levs,
										f.CeilingChecked);
						_mainViewUnderlay.ResizeUnderlay();
					}
				}
			}
		}


		private bool _windowFlag;

		private void OnMainWindowActivated(object sender, EventArgs e)
		{
			if (!_windowFlag)
			{
				_windowFlag = true;

				foreach (MenuItem it in menuView.MenuItems)
					if (it.Checked)
						((Form)it.Tag).BringToFront();

				Select();
				BringToFront();

				_windowFlag = false;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			// NOTE: if the form is maximized with scrollbars visible
			// and the new size doesn't need scrollbars but
			// the map was offset, the scrollbars disappear
			// but the map is still offset. So fix it.
			//
			// TODO: this is a workaround.
			// It simply relocates the overlay to the origin, but it should
			// try to maintain focus on a selected tile for cases when the
			// form is maximized *and the overlay still needs* scrollbars.

			if (WindowState == FormWindowState.Maximized)
				_mainViewUnderlay.ResetScrollers();
		}

		private void OnInfoClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MapBase != null)
			{
				var f = new MapInfoForm();
				f.Show();
				f.Analyze(_mainViewUnderlay.MapBase);
			}
		}

		private void OnExportClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE.
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

		private void OnSelectionBoxClick(object sender, EventArgs e) // NOTE: is disabled w/ Visible=FALSE in designer.
		{
//			_mainViewPanel.MapView.DrawSelectionBox = !_mainViewPanel.MapView.DrawSelectionBox;
//			tsbSelectionBox.Checked = !tsbSelectionBox.Checked;
		}


		private const double ScaleDelta = 0.125;

		private void OnZoomInClick(object sender, EventArgs e)
		{
			if (Globals.Scale < Globals.ScaleMaximum)
			{
				Globals.Scale += Math.Min(
										Globals.ScaleMaximum - Globals.Scale,
										ScaleDelta);
				Zoom();
			}
		}

		private void OnZoomOutClick(object sender, EventArgs e)
		{
			if (Globals.Scale > Globals.ScaleMinimum)
			{
				Globals.Scale -= Math.Min(
										Globals.Scale - Globals.ScaleMinimum,
										ScaleDelta);
				Zoom();
			}
		}

		private void Zoom()
		{
			Globals.AutoScale   =
			tsbAutoZoom.Checked = false;

			_mainViewUnderlay.SetOverlaySize();
			_mainViewUnderlay.UpdateScrollers();

			Refresh();
		}

		private void OnAutoScaleClick(object sender, EventArgs e)
		{
			Globals.AutoScale   = 
			tsbAutoZoom.Checked = !tsbAutoZoom.Checked;

			if (Globals.AutoScale)
			{
				_mainViewUnderlay.SetScale();
				_mainViewUnderlay.SetOverlaySize();
			}
			_mainViewUnderlay.UpdateScrollers();
		}

		/// <summary>
		/// Prints the currently selected location to the status bar.
		/// NOTE: The 'lev' should be inverted before it's passed in.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		internal void StatusBarPrintPosition(int col, int row, int lev)
		{
			if (_mainViewUnderlay.MainViewOverlay.FirstClick)
				tsslPosition.Text = String.Format(
												System.Globalization.CultureInfo.CurrentCulture,
												"c {0}  r {1}  L {2}",
												col, row, lev);
		}

		internal void StatusBarPrintScale()
		{
			tsslScale.Text = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"scale {0:0.00}",
										Globals.Scale);
		}


		/// <summary>
		/// Transposes all the default viewer positions and sizes from the
		/// embedded MapViewers manifest to '/settings/MapViewers.yml'.
		/// Based on InstallationForm.
		/// </summary>
		private static void CreateViewersFile()
		{
			var info = (PathInfo)SharedSpace.Instance[PathInfo.MapViewers];
			info.CreateDirectory();

			string pfe = info.FullPath;

			using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													 .GetManifestResourceStream("MapView._Embedded.MapViewers.yml")))
			using (var fs = new FileStream(pfe, FileMode.Create))
			using (var sw = new StreamWriter(fs))
			{
				while (sr.Peek() != -1)
					sw.WriteLine(sr.ReadLine());
			}
		}
	}
}
