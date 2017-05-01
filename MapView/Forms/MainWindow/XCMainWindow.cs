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
//	public delegate void MapChangedDelegate(object sender, SetMapEventArgs e);
//	public delegate void StringDelegate(object sender, string args);


	/// <summary>
	/// Instantiates a MainView screen as the basis for all user-interaction.
	/// </summary>
	internal sealed partial class XCMainWindow
		:
			Form
	{
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

			// I think this is needed only for PckView. so I'll assume 'PckViewForm' can handle it.
//			share.AllocateObject(SharedSpace.CustomDirectory, Environment.CurrentDirectory + @"\custom");

			LogFile.WriteLine("Environment cached.");

//			string dir = SharedSpace.Instance.GetString(SharedSpace.SettingsDirectory);
			var infoViewers  = new PathInfo(dir, "MapViewers", "yml");

			var infoSettings = new PathInfo(dir, "MVSettings", "cfg");
			var infoPaths    = new PathInfo(dir, "Paths",      "cfg");
			var infoMapEdit  = new PathInfo(dir, "MapEdit",    "cfg");
			var infoImages   = new PathInfo(dir, "Images",     "cfg");

			share.SetShare(PathInfo.MapViewers, infoViewers);

			share.SetShare(PathInfo.SettingsFile, infoSettings);
			share.SetShare(PathInfo.PathsFile,    infoPaths);
			share.SetShare(PathInfo.MapEditFile,  infoMapEdit);
			share.SetShare(PathInfo.ImagesFile,   infoImages);
			LogFile.WriteLine("PathInfo cached.");


			if (!infoPaths.FileExists()) // check if Paths.cfg exists yet
			{
				using (var f = new InstallationForm())
					if (f.ShowDialog(this) != DialogResult.OK)
						Environment.Exit(-1); // wtf -1

				LogFile.WriteLine("Installation files created.");
			}
			else
				LogFile.WriteLine("Paths.Cfg file exists.");


			if (!infoViewers.FileExists())
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
			tvMaps.BeforeSelect += OnMapSelect;
			tvMaps.AfterSelect  += OnMapSelected;
			// welcome to your new home

			_instance = this;


			_settingsManager = new SettingsManager(); // goes before LoadSettings()

			Settings = new Settings();
			LoadSettings();									// TODO: check if this should go after the managers load
			LogFile.WriteLine("MainView Settings loaded.");	// since managers might be re-instantiating needlessly
															// when OnSettingsChange() runs ....

			_mainViewUnderlay = MainViewUnderlay.Instance;
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
			ViewerFormsManager.ShowAllManager = _mainMenusManager.CreateShowAllManager();
			LogFile.WriteLine("ShowAllManager created.");


			ViewerFormsManager.EditFactory = new EditButtonsFactory(_mainViewUnderlay);
			ViewerFormsManager.Initialize();
			LogFile.WriteLine("MainWindowsManager initialized.");


			GameInfo.ParseLine += ParseLine; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			InitGameInfo(infoPaths);
			LogFile.WriteLine("GameInfo initialized.");


			_viewersManager.ManageViewers();


			ViewerFormsManager.TileView.Control.MapChangedEvent += OnMapChanged;

			MainViewUnderlay.AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."


			_mainViewUnderlay.Dock = DockStyle.Fill;

			tvMaps.TreeViewNodeSorter = StringComparer.OrdinalIgnoreCase;

			tscPanel.ContentPanel.Controls.Add(_mainViewUnderlay);

			ViewerFormsManager.EditFactory.BuildEditStrip(tsEdit);
			tsEdit.Enabled = false;


			try
			{
				PckSpriteCollection cursor = GameInfo.CachePckPack(
															SharedSpace.Instance.GetString(SharedSpace.CursorFile),
															String.Empty,
															2,
															Palette.UfoBattle);
				_mainViewUnderlay.MainView.SetCursor(new CursorSprite(cursor));
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
					_mainViewUnderlay.MainView.SetCursor(new CursorSprite(cursor));
				}
				catch
				{
					_mainViewUnderlay.Cursor = null;
					throw; // TODO: there's got to be a better way to do that ....
				}
				throw;
			}
			LogFile.WriteLine("Cursor loaded.");

			InitList();
			LogFile.WriteLine("Tilesets created and loaded to tree panel.");

			if (infoSettings.FileExists())
			{
				_settingsManager.Load(infoSettings.FullPath);
				LogFile.WriteLine("User settings loaded.");
			}
			else
				LogFile.WriteLine("User settings NOT loaded - no settings file to load.");


			OnResize(null);
			Closing += OnCloseSaveRegistry;

			_loadingProgress = new LoadingForm();
			XCBitmap.LoadingEvent += _loadingProgress.HandleProgress;

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


		private static void InitGameInfo(PathInfo filePaths)
		{
			GameInfo.Init(Palette.UfoBattle, filePaths);
		}

		private static XCMainWindow _instance;
		internal static XCMainWindow Instance
		{
			get { return _instance; }
		}

		private void ParseLine(KeyvalPair line, Varidia vars)
		{
			switch (line.Keyword.ToUpperInvariant())
			{
				case "CURSOR":
					if (line.Value.EndsWith(@"\", StringComparison.Ordinal))
						SharedSpace.Instance.SetShare(
													SharedSpace.CursorFile,
													line.Value + SharedSpace.Cursor);
					else
						SharedSpace.Instance.SetShare(
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

		private static void AddMaps(TreeNode tn, IDictionary<string, MapDesc> maps)
		{
			foreach (string key in maps.Keys)
			{
				var node = new SortableTreeNode(key);
				node.Tag = maps[key];
				tn.Nodes.Add(node);
			}
		}

		private void AddTileset(TilesetBase tileset)
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


		#region Settings
		const string MapView             = "MapView";
		const string Main                = "Main";

		const string Animation           = "Animation";
		const string Doors               = "Doors";
		const string SaveWindowPositions = "SaveWindowPositions";
//		const string SaveOnExit          = "SaveOnExit";

		const string ShowGrid            = "ShowGrid";
		const string GridLayerColor      = "GridLayerColor";
		const string GridLineColor       = "GridLineColor";
		const string GridLineWidth       = "GridLineWidth";

		const string GraySelection       = "GraySelection";


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

			var handler = new ValueChangedEventHandler(OnSettingChange);

			Settings.AddSetting(
							Animation,
							MainViewUnderlay.IsAnimated,
							"If true the sprites will animate",
							Main,
							handler);
			Settings.AddSetting(
							Doors,
							false,
							"If true the doors will animate",
							Main,
							handler);
			Settings.AddSetting(
							SaveWindowPositions,
							true, //PathsEditor.SaveRegistry,
							"If true the window positions and sizes will be saved",
							Main,
							handler);

			Settings.AddSetting(
							ShowGrid,
							MainViewUnderlay.Instance.MainView.ShowGrid,
							"If true a grid will display at the current level of editing",
							MapView,
							null, MainViewUnderlay.Instance.MainView);
			Settings.AddSetting(
							GridLayerColor,
							MainViewUnderlay.Instance.MainView.GridLayerColor,
							"Color of the grid (a,r,g,b)",
							MapView,
							null, MainViewUnderlay.Instance.MainView);
			Settings.AddSetting(
							GridLineColor,
							MainViewUnderlay.Instance.MainView.GridLineColor,
							"Color of the lines that make up the grid",
							MapView,
							null, MainViewUnderlay.Instance.MainView);
			Settings.AddSetting(
							GridLineWidth,
							MainViewUnderlay.Instance.MainView.GridLineWidth,
							"Width of the grid lines in pixels",
							MapView,
							null, MainViewUnderlay.Instance.MainView);
			Settings.AddSetting(
							GraySelection,
							MainViewUnderlay.Instance.MainView.GraySelection,
							"If true the selection area will show up in gray",
							MapView,
							null, MainViewUnderlay.Instance.MainView);

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
					MainViewUnderlay.Instance.MainView.ShowGrid = (bool)val;
					break;

				case GridLayerColor:
					MainViewUnderlay.Instance.MainView.GridLayerColor = (Color)val;
					break;

				case GridLineColor:
					MainViewUnderlay.Instance.MainView.GridLineColor = (Color)val;
					break;

				case GridLineWidth:
					MainViewUnderlay.Instance.MainView.GridLineWidth = (int)val;
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
			var desc = tvMaps.SelectedNode.Tag as MapDesc;
			if (desc != null)
			{
//				miExport.Enabled = true; // disabled in designer w/ Visible=FALSE.

				var tileFactory = new XCTileFactory();
				tileFactory.WarningEvent += _warningHandler.HandleWarning;	// used to send a message to the Console if
																			// a DeadTile or AlternateTile is out of bounds.
				var fileService = new XCMapFileService(tileFactory);

				var mapBase = fileService.Load(desc as XCMapDesc);
				_mainViewUnderlay.SetMapBase(mapBase);

				tsEdit.Enabled = true;

				RouteCheckService.CheckNodeBounds(mapBase);

				tsslMap.Text = desc.Label;

				tsslDimensions.Text = (mapBase != null) ? mapBase.MapSize.ToString()
														: "size: n/a";

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

//		private void OnDoorsClick(object sender, EventArgs e)
//		{
//			miDoors.Checked = !miDoors.Checked;
//			foreach (XCTile tile in _mainViewPanel.MapBase.Tiles)
//				if (tile.Record.UfoDoor || tile.Record.HumanDoor)
//					tile.SetAnimationSprites(miDoors.Checked, tile.Record.UfoDoor);
//		}

		private void OnResizeClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MainView.MapBase != null)
			{
				using (var f = new ChangeMapSizeForm())
				{
					f.MapBase = _mainViewUnderlay.MainView.MapBase;
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						f.MapBase.ResizeTo(
										f.Rows,
										f.Cols,
										f.Levs,
										f.CeilingChecked);
						_mainViewUnderlay.ForceResize();
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

				foreach (MenuItem it in menuView.MenuItems)
					if (it.Checked)
						((Form)it.Tag).BringToFront();

				Select();
				BringToFront();

				_windowFlag = false;
			}
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

		private void OnZoomInClick(object sender, EventArgs e)
		{
			if (Globals.PckImageScale < Globals.MaxPckImageScale)
			{
				Globals.PckImageScale += 0.125;
				Zoom(false);
			}
		}

		private void OnZoomOutClick(object sender, EventArgs e)
		{
			if (Globals.PckImageScale > Globals.MinPckImageScale)
			{
				Globals.PckImageScale -= 0.125;
				Zoom(false);
			}
		}

		private void OnAutoZoomClick(object sender, EventArgs e)
		{
			if (!(Globals.AutoPckImageScale = !Globals.AutoPckImageScale))
				Globals.PckImageScale = 1.0;

			tsbAutoZoom.Checked = !tsbAutoZoom.Checked;

			Zoom(true);
		}

		private void Zoom(bool auto)
		{
			if (!auto)
			{
				tsbAutoZoom.Checked       =
				Globals.AutoPckImageScale = false;
			}

			_mainViewUnderlay.SetPanelSize();
			_mainViewUnderlay.ForceResize();

			Refresh();
		}

		internal void StatusBarPrintPosition(int col, int row)
		{
			tsslPosition.Text = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"c {0}  r {1}",
											col, row);
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
