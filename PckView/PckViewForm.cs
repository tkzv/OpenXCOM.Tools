using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Windows;

using PckView.Forms.ImageBytes;

using XCom;
using XCom.Interfaces;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace PckView
{
	public sealed partial class PckViewForm
		:
			Form
	{
		#region Events (static)
		internal static event PaletteChangedEventHandler PaletteChangedEvent;
		#endregion


		#region Fields
		private PckViewPanel _pnlView = new PckViewPanel();

		private EditorForm _feditor = new EditorForm();
		private ConsoleForm _fconsole;

		private TabControl _tcTabs;

		private MenuItem _miEdit;
		private MenuItem _miExport;
		private MenuItem _miAdd;
//		private MenuItem _miReplace;
//		private MenuItem _miDelete;

//		private SharedSpace _share = SharedSpace.Instance;

		private Dictionary<Palette, MenuItem> _paletteItems = new Dictionary<Palette, MenuItem>();
		#endregion


		#region Properties (static)
		internal static Palette Pal
		{ get; set; }
		#endregion


		#region Properties
		private string SpritesetDirectory
		{ get; set; }

		private string SpritesetLabel
		{ get; set; }

		public bool SavedFile
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// Creates the PckView window.
		/// </summary>
		public PckViewForm()
		{
			// NOTE: Set the debug-logfile-path in PckViewPanel, since it instantiates first.

			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			MaximumSize = new Size(0, 0); // fu.net

			SetMetrics();


			#region SharedSpace information
			_fconsole = new ConsoleSharedSpace(new SharedSpace()).Console;
			_fconsole.FormClosing += (sender, e) => {
				e.Cancel = true;
				_fconsole.Hide();
			};

			FormClosed += (sender, e) => _fconsole.Close();


//			string dirApplication = Path.GetDirectoryName(Application.ExecutablePath);
//			string dirSettings    = Path.Combine(dirApplication, DSShared.PathInfo.SettingsDirectory);
//
//			_share.SetShare(
//						SharedSpace.ApplicationDirectory,
//						dirApplication);
//			_share.SetShare(
//						SharedSpace.SettingsDirectory,
//						dirSettings);

//			XConsole.AdZerg("Application directory: " + _share[SharedSpace.ApplicationDirectory]);
//			XConsole.AdZerg("Settings directory: "    + _share[SharedSpace.SettingsDirectory].ToString());
//			XConsole.AdZerg("Custom directory: "      + _share[SharedSpace.CustomDirectory].ToString());
			#endregion


			_pnlView.Dock = DockStyle.Fill;
			_pnlView.ContextMenu = ViewerContextMenu();
			_pnlView.SpritesetChangedEvent += OnSpritesetChanged;
			_pnlView.Click                 += OnSpriteClick;
			_pnlView.DoubleClick           += OnSpriteEditorClick;

			pnlView.Controls.Add(_pnlView);


//			_share[SharedSpace.Palettes] = new Dictionary<string, Palette>();

			AddPalette(Palette.UfoBattle);
			AddPalette(Palette.UfoGeo);
			AddPalette(Palette.UfoGraph);
			AddPalette(Palette.UfoResearch);
			AddPalette(Palette.TftdBattle);
			AddPalette(Palette.TftdGeo);
			AddPalette(Palette.TftdGraph);
			AddPalette(Palette.TftdResearch);

			Pal = XCImageFile.GetDefaultPalette();
			Pal.SetTransparent(true);

			_paletteItems[Pal].Checked = true;

			_feditor.FormClosing += OnEditorFormClosing;


			var regInfo = new RegistryInfo(RegistryInfo.PckView, this); // subscribe to Load and Closing events.
//			regInfo.AddProperty("SelectedPalette");
		}
		#endregion


		/// <summary>
		/// Positions the window at user-defined coordinates w/ size.
		/// </summary>
		private void SetMetrics()
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers);
			if (File.Exists(fileViewers))
			{
				using (var sr = new StreamReader(File.OpenRead(fileViewers)))
				{
					var str = new YamlStream();
					str.Load(sr);

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, RegistryInfo.PckView, StringComparison.Ordinal))
						{
							int x = 0;
							int y = 0;
							int w = 0;
							int h = 0;

							var invariant = System.Globalization.CultureInfo.InvariantCulture;

							string key = String.Empty;

							var keyvals = nodeRoot.Children[new YamlScalarNode(viewer)] as YamlMappingNode;
							foreach (var keyval in keyvals) // NOTE: There is a better way to do this. See TilesetManager..cTor
							{
								switch (keyval.Key.ToString()) // TODO: Error handling. ->
								{
									case "left":
										x = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "top":
										y = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "width":
										w = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "height":
										h = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
								}
							}

							var rectScreen = Screen.GetWorkingArea(new Point(x, y));
							if (!rectScreen.Contains(x + 200, y + 100)) // check to ensure that PckView is at least partly onscreen.
							{
								x = 100;
								y =  50;
							}

							Left = x;
							Top  = y;

							ClientSize = new Size(w, h);
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves the window position and size to YAML.
		/// </summary>
		private void SaveMetrics()
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers);

			if (File.Exists(fileViewers))
			{
				WindowState = FormWindowState.Normal;

				string src = Path.Combine(dirSettings, PathInfo.ConfigViewers);
				string dst = Path.Combine(dirSettings, PathInfo.ConfigViewersOld);

				File.Copy(src, dst, true);

				using (var sr = new StreamReader(File.OpenRead(dst))) // but now use dst as src ->

				using (var fs = new FileStream(src, FileMode.Create)) // overwrite previous viewers-config.
				using (var sw = new StreamWriter(fs))
				{
					while (sr.Peek() != -1)
					{
						string line = sr.ReadLine().TrimEnd();

						if (String.Equals(line, RegistryInfo.PckView + ":", StringComparison.Ordinal))
						{
							sw.WriteLine(line);

							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine(); // heh

							sw.WriteLine("  left: "   + Math.Max(0, Location.X));	// =Left
							sw.WriteLine("  top: "    + Math.Max(0, Location.Y));	// =Top
							sw.WriteLine("  width: "  + ClientSize.Width);			// <- use ClientSize, since Width and Height
							sw.WriteLine("  height: " + ClientSize.Height);			// screw up due to the titlebar/menubar area.
						}
						else
							sw.WriteLine(line);
					}
				}
				File.Delete(dst);
			}
		}

		/// <summary>
		/// Builds the RMB context-menu.
		/// </summary>
		/// <returns></returns>
		private ContextMenu ViewerContextMenu()
		{
			var contextmenu = new ContextMenu();

			_miEdit = new MenuItem("Edit");
			_miEdit.Enabled = false;
			_miEdit.Click += OnSpriteEditorClick;
			contextmenu.MenuItems.Add(_miEdit);

			contextmenu.MenuItems.Add(new MenuItem("-"));

			_miAdd = new MenuItem("Add ...");
			_miAdd.Enabled = false;
			_miAdd.Click += OnAddSpriteClick;
			contextmenu.MenuItems.Add(_miAdd);

//			_miReplace = new MenuItem("Replace");
//			_miReplace.Enabled = false;
//			_miReplace.Click += OnSpriteReplaceClick;
//			menu.MenuItems.Add(_miReplace);

//			_miDelete = new MenuItem("Delete\tDel");
//			_miDelete.Enabled = false;
//			_miDelete.Click += OnSpriteDeleteClick;
//			menu.MenuItems.Add(_miDelete);

			contextmenu.MenuItems.Add(new MenuItem("-"));

			_miExport = new MenuItem("Export ...");
			_miExport.Enabled = false;
			_miExport.Click += OnExportSpriteClick;
			contextmenu.MenuItems.Add(_miExport);

//			_miEdit.Enabled = false;

			return contextmenu;
		}

		/// <summary>
		/// Adds a palette as a menuitem to the palettes menu on the main menu.
		/// </summary>
		/// <param name="pal"></param>
		private void AddPalette(Palette pal)
		{
			var itPal = new MenuItem(pal.Label);
			itPal.Tag = pal;
			miPaletteMenu.MenuItems.Add(itPal);

			itPal.Click += OnPaletteClick;
			_paletteItems[pal] = itPal;

//			((Dictionary<string, Palette>)_share[SharedSpace.Palettes])[pal.Label] = pal;
		}


		#region EventCalls
		private void OnSpritesetChanged(bool valid)
		{
			miSave.Enabled            =
			miSaveAs.Enabled          =
			miExportSprites.Enabled   =

			miPaletteMenu.Enabled     =
			miTransparentMenu.Enabled =
			miBytesMenu.Enabled       =

			_miAdd.Enabled            = valid;
		}

		private void OnSpriteClick(object sender, EventArgs e)
		{
			bool valid = _pnlView.Selected != null
					  && _pnlView.Selected.Count != 0;

			_miEdit.Enabled   =
			_miExport.Enabled = valid;
//			_miDelete.Enabled = valid;

			SelectedSprite selected = null;
			if (valid)
				selected = _pnlView.Selected[_pnlView.Selected.Count - 1];

			BytesFormHelper.ReloadBytes(selected);
		}

		/// <summary>
		/// Exports the last selected sprite to a BMP file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportSpriteClick(object sender, EventArgs e)
		{
			if (_pnlView.Selected.Count != 0)
			{
				var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
				if (_pnlView.Spriteset != null)
				{
					string digits = String.Empty;

					int count = _pnlView.Spriteset.Count;
					do
					{
						digits += "0";
						count /= 10;
					}
					while (count != 0);

					string suffix = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"{0:" + digits + "}",
												selected.Image.TerrainId);

					sfdSingleSprite.FileName = _pnlView.Spriteset.Label + suffix;

					if (sfdSingleSprite.ShowDialog() == DialogResult.OK)
						BitmapService.ExportSprite(sfdSingleSprite.FileName, selected.Image.Image);
				}
			}
		}

		private void OnAddSpriteClick(object sender, EventArgs e)
		{
			if (_pnlView.Spriteset != null)
			{
				var ofd = new OpenFileDialog();
				ofd.Filter = "32x40 8-bpp BMP files (*.bmp)|*.BMP|All Files (*.*)|*.*";
				ofd.Title = "Add BMP file(s)";
				ofd.Multiselect = true;

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					int terrainId = _pnlView.Spriteset.Count;

					foreach (string file in ofd.FileNames)
					{
						var bitmap = new Bitmap(file);
						_pnlView.Spriteset.Add(BitmapService.CreateSprite(
																	bitmap,
																	terrainId++,
																	Pal,
																	0, 0,
																	XCImageFile.SpriteWidth,
																	XCImageFile.SpriteHeight));
					}
					Refresh();
					// TODO: update statusbar.
				}
			}
		}

		private void OnSpriteReplaceClick(object sender, EventArgs e) // disabled in ViewerContextMenu()
		{
			if (_pnlView.Selected.Count != 1)
			{
				MessageBox.Show(
							this,
							"Must select 1 item only.",
							Text,
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (_pnlView.Spriteset != null )
			{
				var title = String.Empty;
				foreach (var sprite in _pnlView.Selected)
				{
					if (!String.IsNullOrEmpty(title))
						title += ", ";

					title += sprite.Id;
				}

				ofdBmp.Title = "Selected: " + title;
				ofdBmp.Multiselect = false;
				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					var bitmap = new Bitmap(ofdBmp.FileName);
					var spriteset = BitmapService.CreateSpriteset(
															bitmap,
															Pal,
															XCImageFile.SpriteWidth,
															XCImageFile.SpriteHeight,
															1)[0];
					_pnlView.SpriteReplace(_pnlView.Selected[0].Id, spriteset);
					Refresh();
				}
			}
		}

		private void OnSpriteDeleteClick(object sender, EventArgs e) // disabled in ViewerContextMenu()
		{
			_pnlView.SpriteDelete();
			Refresh();
			// TODO: update statusbar.
		}


		bool _editorInited;

		private void OnSpriteEditorClick(object sender, EventArgs e)
		{
			if (   _pnlView.Selected != null
				&& _pnlView.Selected.Count != 0)
			{
				var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
				if (selected != null)
				{
//					_feditor.Sprite = selected.Image.Clone(); // NOTE: that's only a *clone**
					_feditor.Sprite = selected.Image;

					if (!_feditor.Visible)
					{
						_miEdit.Checked = true;	// TODO: show as Checked only if currently selected
												// sprite is actually open in the Editor.
						if (!_editorInited)
						{
							_editorInited = true;
							_feditor.Left = Left + 20;
							_feditor.Top  = Top  + 20;
						}
						_feditor.Show();
					}
					else
						_feditor.BringToFront();
				}
			}
		}

		private void OnEditorFormClosing(object sender, CancelEventArgs e)
		{
			_miEdit.Checked = false;

			e.Cancel = true;
			_feditor.Hide();
		}

		private void OnPaletteClick(object sender, EventArgs e)
		{
			var pal = ((MenuItem)sender).Tag as Palette;
			if (pal != Pal)
			{
				_paletteItems[Pal].Checked = false;

				Pal = pal;
				Pal.SetTransparent(miTransparent.Checked);

				_paletteItems[Pal].Checked = true;

//				if (_pnlView.Spriteset != null) // NOTE: menu won't show until a spriteset is loaded.
				_pnlView.Spriteset.Pal = Pal;

				var handler = PaletteChangedEvent;
				if (handler != null)
					handler(Pal);
			}
		}

		private void OnTransparencyClick(object sender, EventArgs e)
		{
			Pal.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

//			if (_pnlView.Spriteset != null) // NOTE: menu won't show until a spriteset is loaded.
			_pnlView.Spriteset.Pal = Pal;

			PalettePanel.Instance.PrintStatusPaletteId();	// update the palette-panel's statusbar
															// in case palette-id #0 is currently selected.
			var handler = PaletteChangedEvent;
			if (handler != null)
				handler(Pal);
		}

		private void OnQuitClick(object sender, EventArgs e)
		{
			OnPckViewFormClosing(null, null);
			Close();
		}

		private void OnOpenClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Pck Files (*.pck)|*.PCK|All Files (*.*)|*.*";
				ofd.Title  = "Select a Pck File";
//				ofd.InitialDirectory = _share.GetString(SharedSpace.ApplicationDirectory);

				if (ofd.ShowDialog() == DialogResult.OK)
					LoadSpriteset(ofd.FileName, false);
			}
		}

		/// <summary>
		/// Sets the current palette.
		/// NOTE: Called only from TileView to set the palette externally.
		/// </summary>
		/// <param name="palette"></param>
		public void SetPalette(string palette)
		{
			foreach (var pal in _paletteItems.Keys)
			{
				if (pal.Label.Equals(palette))
				{
					OnPaletteClick(_paletteItems[pal], EventArgs.Empty);
					break;
				}
			}
		}

		/// <summary>
		/// Loads a PCK file. May be called from MapView.Forms.MapObservers.TileViews.TileView.OnPckEditorClick()
		/// NOTE: with a string like that you'd think this was .NET itself.
		/// </summary>
		/// <param name="pfePck">fullpath of a PCK file; check existence of the
		/// file before call</param>
		/// <param name="help">true to show help-splash for a call from MapView</param>
		public void LoadSpriteset(string pfePck, bool help)
		{
			//LogFile.WriteLine("PckViewForm.LoadSpriteset");
			//LogFile.WriteLine(". " + pfePck);

			SpritesetDirectory = Path.GetDirectoryName(pfePck);
			SpritesetLabel     = Path.GetFileNameWithoutExtension(pfePck);

			string pfeTab = pfePck.Substring(0, pfePck.Length - 4) + SpriteCollection.TabExt;
			if (File.Exists(pfeTab))
			{
				using (var fsPck = File.OpenRead(pfePck))
				using (var fsTab = File.OpenRead(pfeTab))
				{
					SpriteCollectionBase spriteset = null;
					try
					{
						//LogFile.WriteLine(". . try UFO");
						spriteset = new SpriteCollection(
													fsPck,
													fsTab,
													2,
													Palette.UfoBattle);
					}
					catch (Exception)
					{
						//LogFile.WriteLine(". . catch TFTD");
						spriteset = new SpriteCollection(
													fsPck,
													fsTab,
													4,
													Palette.TftdBattle); // NOTE: was 'Palette.UfoBattle'
					}

					if (spriteset != null)
					{
						spriteset.ImageFile = new XCImageFile();
						spriteset.Label = SpritesetLabel;

						if (spriteset.Pal == null)
							spriteset.Pal = XCImageFile.GetDefaultPalette();
					}

					OnPaletteClick(
								_paletteItems[XCImageFile.GetDefaultPalette()],
								EventArgs.Empty);

					_pnlView.Spriteset = spriteset;

//					miSave.Enabled = (spriteset != null); // ... not sure if spriteset can even be null here.

					UpdateCaption(pfePck);

//					if (help) // disabled until editing and saving get reinstated.
//					{
//						pMapViewHelp.Visible  =
//						miMapViewHelp.Visible = true;
//					}
				}
			}
			else
			{
//				XConsole.AdZerg("ERROR: tab file does not exist: " + pfeTab);
				MessageBox.Show(
							this,
							"Tab file does not exist"
								+ Environment.NewLine + Environment.NewLine
								+ pfeTab,
							Text,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}

		private void UpdateCaption(string fullpath)
		{
			Text = "PckView - " + fullpath;
		}

		private void OnShowBytesClick(object sender, EventArgs e)
		{
			if (!miBytes.Checked)
			{
				if (_pnlView.Selected.Count != 0)
				{
					miBytes.Checked = true;

					var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
					BytesFormHelper.ShowBytes(selected, CallbackShowBytesClosing, new Point(Right, Top));
				}
			}
			else
			{
				miBytes.Checked = false;
				BytesFormHelper.CloseBytes();
			}
		}

		private void CallbackShowBytesClosing()
		{
			miBytes.Checked = false;
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			new About().ShowDialog(this);
		}

		private void OnHelpClick(object sender, EventArgs e)
		{
			new Help().ShowDialog(this);
		}

		private void OnHq2xClick(object sender, EventArgs e) // disabled w/ Visible=FALSE in the designer.
		{
//			miPaletteMenu.Enabled = false;
//			miBytesMenu.Enabled = false;
//
//			_totalViewPck.Hq2x();
//
//			OnResize(null);
//			Refresh();
		}

		private void OnKeyDown(object sender, KeyEventArgs e) // disabled in BuildViewersContextMenu()
		{
//			if (_miDelete.Enabled && e.KeyCode == Keys.Delete)
//				OnSpriteDeleteClick(null, null);
		}

		private void OnConsoleClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
//			if (_fconsole.Visible)
//				_fconsole.BringToFront();
//			else
//				_fconsole.Show();
		}

		private void OnCompareClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
			var original = _pnlView.Spriteset;

			OnOpenClick(null, EventArgs.Empty);

			var spriteset = _pnlView.Spriteset;

			_pnlView.Spriteset = original;

			if (Controls.Contains(_pnlView))
			{
				Controls.Remove(_pnlView);

				_tcTabs = new TabControl();
				_tcTabs.Dock = DockStyle.Fill;
				pnlView.Controls.Add(_tcTabs);

				var tabpage = new TabPage();
				tabpage.Controls.Add(_pnlView);
				tabpage.Text = "Original";
				_tcTabs.TabPages.Add(tabpage);

				var viewpanel = new PckViewPanel();
				viewpanel.ContextMenu = ViewerContextMenu();
				viewpanel.Dock = DockStyle.Fill;
				viewpanel.Spriteset = spriteset;

				tabpage = new TabPage();
				tabpage.Controls.Add(viewpanel);
				tabpage.Text = "New";
				_tcTabs.TabPages.Add(tabpage);
			}
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			Directory.CreateDirectory(SpritesetDirectory); // in case user deleted the dir.

			string pfePck = Path.Combine(SpritesetDirectory, SpritesetLabel + SpriteCollection.PckExt);
			string pfeTab = Path.Combine(SpritesetDirectory, SpritesetLabel + SpriteCollection.TabExt);

			string dirBackup = Path.Combine(SpritesetDirectory, "MV_Backup");

			if (File.Exists(pfePck))
			{
				Directory.CreateDirectory(dirBackup);

				string pfePckOld = Path.Combine(dirBackup, SpritesetLabel + SpriteCollection.PckExt);
				File.Copy(pfePck, pfePckOld, true);
			}

			if (File.Exists(pfeTab))
			{
				Directory.CreateDirectory(dirBackup);

				string pfeTabOld = Path.Combine(dirBackup, SpritesetLabel + SpriteCollection.TabExt);
				File.Copy(pfeTab, pfeTabOld, true);
			}


			// http://www.ufopaedia.org/index.php/Image_Formats
			// - that says that all TFTD terrains use 2-byte tab-offsets ...
//			const int lenTabOffset = 2;
//			if (   Pal.Equals(Palette.TftdBattle)
//				|| Pal.Equals(Palette.TftdGeo)
//				|| Pal.Equals(Palette.TftdGraph)
//				|| Pal.Equals(Palette.TftdResearch))
//			{
//				lenTabOffset = 4; // NOTE: I don't have TFTD and I do have no clue if this works correctly.
//			}

			SpriteCollection.SaveSpriteset(
										SpritesetDirectory,
										SpritesetLabel,
										_pnlView.Spriteset,
										((SpriteCollection)_pnlView.Spriteset).TabOffset); //lenTabOffset
			SavedFile = true; // NOTE: used only by MapView's TileView to flag the Map to reload.
		}

		private void OnSaveAsClick(object sender, EventArgs e)
		{
			var sfd = new SaveFileDialog();
			sfd.FileName = SpritesetLabel;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				string dir  = Path.GetDirectoryName(sfd.FileName);
				string file = Path.GetFileNameWithoutExtension(sfd.FileName);
				SpriteCollection.SaveSpriteset(
											dir,
											file,
											_pnlView.Spriteset,
											((SpriteCollection)_pnlView.Spriteset).TabOffset);

				string pfePck = Path.Combine(dir, file) + SpriteCollection.PckExt;
				LoadSpriteset(pfePck, false);
			}
		}

		/// <summary>
		/// Exports all sprites in the currently loaded spriteset to BMP files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportSpritesClick(object sender, EventArgs e)
		{
			if (_pnlView.Spriteset != null && _pnlView.Spriteset.Count != 0)
			{
				using (var fbd = new FolderBrowserDialog())
				{
					string file = _pnlView.Spriteset.Label.ToUpperInvariant();

//					fbd.SelectedPath = ;
					fbd.Description = String.Format(
												System.Globalization.CultureInfo.CurrentCulture,
												"Select a folder for the sprites in Spriteset"
													+ Environment.NewLine + Environment.NewLine
													+ file);

					if (fbd.ShowDialog() == DialogResult.OK)
					{
						string path = fbd.SelectedPath;

						string digits = String.Empty;
						int count = _pnlView.Spriteset.Count;
						do
						{
							digits += "0";
							count /= 10;
						}
						while (count != 0);

//						var progress     = new ProgressWindow(this);
//						progress.Width   = 300;
//						progress.Height  = 50;
//						progress.Minimum = 0;
//						progress.Maximum = _pnlView.Spriteset.Count;
//
//						progress.Show();
						foreach (XCImage sprite in _pnlView.Spriteset)
						{
							string suffix = String.Format(
														System.Globalization.CultureInfo.InvariantCulture,
														"{0:" + digits + "}",
														sprite.TerrainId);
							string fullpath = Path.Combine(path, file + suffix + BitmapService.BmpExt);
							BitmapService.ExportSprite(fullpath, sprite.Image);

//							progress.Value = sprite.FileId;
						}
//						progress.Hide(); // TODO: I suspect this is essentially the same as a memory leak.
					}
				}
			}
		}

		private void OnMapViewHelpClick(object sender, EventArgs e)
		{
			pnlMapViewHelp.Visible = !pnlMapViewHelp.Visible;
			miMapViewHelp.Checked = pnlMapViewHelp.Visible;
		}

		private void OnMapViewGotItClick(object sender, EventArgs e)
		{
			pnlMapViewHelp.Visible  =
			miMapViewHelp.Checked = false;
		}

		private void OnShown(object sender, EventArgs e)
		{
			_pnlView.Select();
//			_console.Show();
		}

		private void OnPckViewFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveMetrics();

			_feditor.ClosePalette();	// these are needed when PckView
			_feditor.Close();			// was opened via MapView.

			if (miBytes.Checked)
				BytesFormHelper.CloseBytes();
		}
		#endregion
	}


	internal delegate void PaletteChangedEventHandler(Palette pal);
}
