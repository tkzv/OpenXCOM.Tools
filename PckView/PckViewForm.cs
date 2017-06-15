using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;

using PckView.Forms.ImageBytes;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public sealed partial class PckViewForm
		:
			Form
	{
		#region Events (static)
		internal static event PaletteChangedEventHandler PaletteChangedEvent;
		#endregion


		#region Properties (static)
		internal static Palette Pal
		{ get; set; }
		#endregion


		#region Fields
		private PckViewPanel _pnlView = new PckViewPanel();

		private EditorForm _feditor = new EditorForm();
		private ConsoleForm _fconsole;

		private TabControl _tcTabs;

		private MenuItem _miEdit;
		private MenuItem _miExport;
//		private MenuItem _miReplace;
//		private MenuItem _miDelete;
//		private MenuItem _miAdd;

//		private SharedSpace _share = SharedSpace.Instance;

		private Dictionary<Palette, MenuItem> _paletteItems = new Dictionary<Palette, MenuItem>();
		#endregion


		#region Properties
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


			miSave.Visible = false;

//			_share[SharedSpace.Palettes] = new Dictionary<string, Palette>();

			AddPalette(Palette.UfoBattle);
			AddPalette(Palette.UfoGeo);
			AddPalette(Palette.UfoGraph);
			AddPalette(Palette.UfoResearch);
			AddPalette(Palette.TftdBattle);
			AddPalette(Palette.TftdGeo);
			AddPalette(Palette.TftdGraph);
			AddPalette(Palette.TftdResearch);

			Pal = Palette.UfoBattle;
			Pal.SetTransparent(true);

			_paletteItems[Pal].Checked = true;

			_feditor.FormClosing += OnEditorFormClosing;


			var regInfo = new RegistryInfo(RegistryInfo.PckView, this); // subscribe to Load and Closing events.
//			regInfo.AddProperty("SelectedPalette");
		}
		#endregion


		private ContextMenu ViewerContextMenu()
		{
			var menu = new ContextMenu();

			_miEdit = new MenuItem("Edit");
			_miEdit.Click += OnSpriteEditorClick;
			menu.MenuItems.Add(_miEdit);

			menu.MenuItems.Add(new MenuItem("-"));

			_miExport = new MenuItem("Export");
			_miExport.Click += OnExportSpriteClick;
			menu.MenuItems.Add(_miExport);

//			_miReplace = new MenuItem("Replace");
//			_miReplace.Click += OnSpriteReplaceClick;
//			menu.MenuItems.Add(_miReplace);

//			_miAdd = new MenuItem("Add");
//			_miAdd.Click += OnSpriteAddClick;
//			menu.MenuItems.Add(_miAdd);

//			_miDelete = new MenuItem("Delete\tDel");
//			_miDelete.Click += OnSpriteDeleteClick;
//			menu.MenuItems.Add(_miDelete);

//			_miEdit.Enabled = false;

			return menu;
		}

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
			miSaveAs.Enabled          =
			miExportSprites.Enabled   =

			miPaletteMenu.Enabled     =
			miTransparentMenu.Enabled =
			miBytesMenu.Enabled       = valid;
		}

		private void OnSpriteClick(object sender, EventArgs e)
		{
			bool valid = _pnlView.Selected != null
					  && _pnlView.Selected.Count != 0;

			_miEdit.Enabled   =
			_miExport.Enabled = valid;
//			_miDelete.Enabled = valid;

			if (valid)
			{
				var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
				BytesFormHelper.ReloadBytes(selected);
			}
			else
				BytesFormHelper.ReloadBytes(null);
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
												selected.Image.FileId);

					sfdSingleSprite.FileName = _pnlView.Spriteset.Label + suffix;

					if (sfdSingleSprite.ShowDialog() == DialogResult.OK)
						BitmapService.ExportSprite(sfdSingleSprite.FileName, selected.Image.Image);
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
					var sprite = BitmapService.LoadSpriteset(
															bitmap,
															Pal,
															XCImageFile.SpriteWidth,
															XCImageFile.SpriteHeight,
															1)[0];
					_pnlView.SpriteReplace(_pnlView.Selected[0].Id, sprite);
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteAddClick(object sender, EventArgs e) // disabled in ViewerContextMenu()
		{
			if (_pnlView.Spriteset != null)
			{
				ofdBmp.Title = "Hold shift to select multiple files.";
				ofdBmp.Multiselect = true;

				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					foreach (string file in ofdBmp.FileNames)
					{
						var bitmap = new Bitmap(file);
						_pnlView.Spriteset.Add(BitmapService.LoadSprite(
																	bitmap,
																	0,
																	Pal,
																	0, 0,
																	XCImageFile.SpriteWidth,
																	XCImageFile.SpriteHeight));
					}
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteDeleteClick(object sender, EventArgs e) // disabled in ViewerContextMenu()
		{
			_pnlView.SpriteDelete();
			UpdateCaption();
			Refresh();
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

				//LogFile.WriteLine("###");
				//for (int i = 0; i != 256; ++i)
				//{
				//	LogFile.WriteLine(i + ": " + Pal[i]);
				//}
				//LogFile.WriteLine("###");

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

			var handler = PaletteChangedEvent;
			if (handler != null)
				handler(Pal);
		}

		private void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}

		private void OnOpenClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Pck Files (*.pck)|*.pck|All Files (*.*)|*.*";
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
			string pfeTab = pfePck.Substring(0, pfePck.Length - 4) + SpriteCollection.TabExt;
			if (File.Exists(pfeTab))
			{
				using (var strPck = File.OpenRead(pfePck))
				using (var strTab = File.OpenRead(pfeTab))
				{
					SpriteCollectionBase spriteset = null;
					try
					{
						spriteset = new SpriteCollection(
														strPck,
														strTab,
														2,
														Palette.UfoBattle);
					}
					catch (Exception)
					{
						spriteset = new SpriteCollection(
														strPck,
														strTab,
														4,
														Palette.UfoBattle); // TODO: uh, should be 'TftdBattle'
					}

					if (spriteset != null)
					{
						spriteset.ImageFile = new XCImageFile();
						spriteset.Label = Path.GetFileNameWithoutExtension(pfePck);

						if (spriteset.Pal == null)
							spriteset.Pal = Palette.UfoBattle;
					}

					OnPaletteClick(
								_paletteItems[spriteset.ImageFile.DefaultPalette],
								EventArgs.Empty);

					_pnlView.Spriteset = spriteset;


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

		private void UpdateCaption() // TODO: replace calls w/ UpdateCaption(string)
		{
			Text = "PckView - " + _pnlView.Spriteset.Label + " [" + _pnlView.Spriteset.Count + "] total";
		}
		private void UpdateCaption(string fullpath)
		{
			Text = "PckView - " + fullpath;
		}

		private void OnSaveAsClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
/*			var saveFile = new SaveFileDialog();

			_dialogFilter.SetFilter(XCImageFile.Filter.Save);
			_dictSaveFiles.Clear();
//			saveFile.Filter = _loadedTypes.CreateFilter(_dialogFilter, _dictSaveFiles);

			if (saveFile.ShowDialog() == DialogResult.OK)
			{
				string dir = saveFile.FileName.Substring(0, saveFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal));
				_dictSaveFiles[saveFile.FilterIndex].SaveCollection(
																dir,
																Path.GetFileNameWithoutExtension(saveFile.FileName),
																_totalViewPck.Collection);
			} */
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
												"Select a folder for the sprites"
													+ Environment.NewLine
													+ "Spriteset " + file);

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
														sprite.FileId);
							string fullpath = Path.Combine(path, file + suffix + BitmapService.BmpExt);
							BitmapService.ExportSprite(fullpath, sprite.Image);

//							progress.Value = sprite.FileId;
						}
//						progress.Hide(); // TODO: I suspect this is essentially the same as a memory leak.
					}
				}
			}
		}

		private void OnConsoleClick(object sender, EventArgs e)
		{
			if (_fconsole.Visible)
				_fconsole.BringToFront();
			else
				_fconsole.Show();
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

		private void OnSaveSpritesetClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
/*			_dialogFilter.SetFilter(XCImageFile.Filter.Save);
			_dictSaveFiles.Clear();
//			_loadedTypes.CreateFilter(_dialogFilter, _dictSaveFiles);
			var dir = Path.GetDirectoryName(_path);
			var fileWithoutExt = Path.GetFileNameWithoutExtension(_path);

			// Backup
			FileBackupManager.Backup(_path);

			// Save
			SpriteCollection.Save(
								dir,
								fileWithoutExt,
								_totalViewPck.Collection,
								_depth);
			SavedFile = true; */
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
			if (miBytes.Checked)
				BytesFormHelper.CloseBytes();
		}
		#endregion
	}


	internal delegate void PaletteChangedEventHandler(Palette pal);
}
