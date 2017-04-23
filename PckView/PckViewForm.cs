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
		#region Fields & Properties

		private ViewPanel _viewPanel;

		private Editor        _editor;
		private ConsoleForm   _console;

		private TabControl _tabs;

		private MenuItem _miEdit;
//		private MenuItem _miReplace;
//		private MenuItem _miSave;
//		private MenuItem _miDelete;
//		private MenuItem _miAdd;

		private Palette _palette;

		private SharedSpace _share;


		private Dictionary<Palette, MenuItem> _paletteDictionary;

		public bool SavedFile
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// Creates the PckView window.
		/// </summary>
		public PckViewForm()
		{
			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			#region SharedSpace information

			var consoleShare = new ConsoleSharedSpace(new SharedSpace());
			_console = consoleShare.GetConsole();
			_console.FormClosing += (sender, e) =>
			{
				e.Cancel = true;
				_console.Hide();
			};
			FormClosed += (sender, e) => _console.Close();

			_share = SharedSpace.Instance;
			_share.SetShare("PckView", this);

//			_share.SetShare(PathInfo.MapViewers, infoViewers);

			_share.SetShare(SharedSpace.ApplicationDirectory, Environment.CurrentDirectory);
			_share.SetShare(SharedSpace.SettingsDirectory,    Environment.CurrentDirectory + @"\settings");
//			_share.SetShare(SharedSpace.CustomDirectory,      Environment.CurrentDirectory + @"\custom");
		
//			XConsole.AdZerg("Application directory: "  + _share[SharedSpace.ApplicationDirectory]);			// TODO: I don't trust that since changing SharedSpace.
//			XConsole.AdZerg("Settings directory: "     + _share[SharedSpace.SettingsDirectory].ToString());	// it may well need an explicit cast to (PathInfo)
//			XConsole.AdZerg("Custom directory: "       + _share[SharedSpace.CustomDirectory].ToString());
			#endregion


			_viewPanel = new ViewPanel();
			_viewPanel.Dock = DockStyle.Fill;
			_viewPanel.ContextMenu = BuildViewerContextMenu();
			_viewPanel.SpritePackChangedEvent += OnSpritePackChanged;
			_viewPanel.SpriteClickEvent       += OnSpriteClick;
			_viewPanel.DoubleClick            += OnSpriteEditClick;

			pViewer.Controls.Add(_viewPanel);


			miSave.Visible = false;

			_share[SharedSpace.Palettes] = new Dictionary<string, Palette>();
			_paletteDictionary = new Dictionary<Palette, MenuItem>();

			AddPalette(Palette.UfoBattle,    miPaletteMenu);
			AddPalette(Palette.UfoGeo,       miPaletteMenu);
			AddPalette(Palette.UfoGraph,     miPaletteMenu);
			AddPalette(Palette.UfoResearch,  miPaletteMenu);
			AddPalette(Palette.TftdBattle,   miPaletteMenu);
			AddPalette(Palette.TftdGeo,      miPaletteMenu);
			AddPalette(Palette.TftdGraph,    miPaletteMenu);
			AddPalette(Palette.TftdResearch, miPaletteMenu);

			_palette = Palette.UfoBattle;
			_palette.EnableTransparency(true);

			_paletteDictionary[_palette].Checked = true;

			_viewPanel.Pal = _palette;

			_editor = new Editor(null);
			_editor.Closing += OnEditorClosing;
			_editor.Palette = _palette;


			var regInfo = new RegistryInfo(this, "PckView"); // subscribe to Load and Closing events.
//			regInfo.AddProperty("SelectedPalette");
		}
		#endregion


		private void OnSpritePackChanged(SpritePackChangedEventArgs e)
		{
			//LogFile.WriteLine("OnSpritePackChanged");

			bool enabled = (e.Sprites != null);
			miSaveAs.Enabled          =
			miTransparentMenu.Enabled =
			miBytesMenu.Enabled       =
			miPaletteMenu.Enabled     = enabled;
		}

		private void OnSpriteClick(int spriteId)
		{
			if (_viewPanel.SelectedSprites.Count > 0) // isSelected
			{
				_miEdit.Enabled   = true;
//				_miSave.Enabled   =
//				_miDelete.Enabled = true;

				var selected = _viewPanel.SelectedSprites[_viewPanel.SelectedSprites.Count - 1];
				BytesFormHelper.ReloadBytes(selected);
			}
			else // selected is null
			{
				_miEdit.Enabled   = false;
//				_miSave.Enabled   =
//				_miDelete.Enabled = false;

				BytesFormHelper.ReloadBytes(null);
			}
		}

		private ContextMenu BuildViewerContextMenu()
		{
			var menu = new ContextMenu();

			_miEdit = new MenuItem("Edit");
			_miEdit.Click += OnSpriteEditClick;
			menu.MenuItems.Add(_miEdit);

			menu.MenuItems.Add(new MenuItem("-"));

//			_miSave = new MenuItem("Save");
//			_miSave.Click += OnSpriteSaveClick;
//			menu.MenuItems.Add(_miSave);

//			_miReplace = new MenuItem("Replace");
//			_miReplace.Click += OnSpriteReplaceClick;
//			menu.MenuItems.Add(_miReplace);

//			_miAdd = new MenuItem("Add");
//			_miAdd.Click += OnSpriteAddClick;
//			menu.MenuItems.Add(_miAdd);

//			_miDelete = new MenuItem("Delete\tDel");
//			_miDelete.Click += OnSpriteDeleteClick;
//			menu.MenuItems.Add(_miDelete);

			_miEdit.Enabled = false;

			return menu;
		}

		private void OnSpriteSaveClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			if (_viewPanel.SelectedSprites.Count != 0)
			{
				var selected = _viewPanel.SelectedSprites[_viewPanel.SelectedSprites.Count - 1];
				if (_viewPanel.SpritePack != null)
				{
					sfdBmpSingle.FileName = _viewPanel.SpritePack.Label + selected.Image.FileId;

					if (sfdBmpSingle.ShowDialog() == DialogResult.OK)
						XCBitmap.Save(sfdBmpSingle.FileName, selected.Image.Image);
				}
			}
		}

		private void OnSpriteReplaceClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			if (_viewPanel.SelectedSprites.Count != 1)
			{
				MessageBox.Show(
							"Must select 1 item only.",
							Text,
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (_viewPanel.SpritePack != null )
			{
				var title = String.Empty;
				foreach (var sprite in _viewPanel.SelectedSprites)
				{
					if (!String.IsNullOrEmpty(title))
						title += ", ";

					title += sprite.Id;
				}

				ofdBmp.Title = "Selected: " + title;
				ofdBmp.Multiselect = false;
				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					var b = new Bitmap(ofdBmp.FileName);
					var image = XCBitmap.Load(
											b,
											_viewPanel.Pal,
											_viewPanel.SpritePack.ImageFile.ImageSize.Width,
											_viewPanel.SpritePack.ImageFile.ImageSize.Height,
											1)[0];
					_viewPanel.SpriteReplace(_viewPanel.SelectedSprites[0].Id, image);
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteAddClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			if (_viewPanel.SpritePack != null)
			{
				ofdBmp.Title = "Hold shift to select multiple files.";
				ofdBmp.Multiselect = true;

				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					foreach (string file in ofdBmp.FileNames)
					{
						var b = new Bitmap(file);
						_viewPanel.SpritePack.Add(XCBitmap.LoadTile(
																b,
																0,
																_viewPanel.Pal,
																0, 0,
																_viewPanel.SpritePack.ImageFile.ImageSize.Width,
																_viewPanel.SpritePack.ImageFile.ImageSize.Height));
					}
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteDeleteClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			_viewPanel.SpriteDelete();
			UpdateCaption();
			Refresh();
		}


		bool _editorInitDone;

		private void OnSpriteEditClick(object sender, EventArgs e)
		{
			if (_viewPanel.SelectedSprites.Count != 0)
			{
				var selected = _viewPanel.SelectedSprites[_viewPanel.SelectedSprites.Count - 1];
				if (selected != null)
				{
					_editor.Image = selected.Image.Clone();

					if (!_editor.Visible)
					{
						_miEdit.Checked = true;	// TODO: show as Checked only if currently selected
												// sprite is actually open in the Editor.
						if (!_editorInitDone)
						{
							_editorInitDone = true;
							_editor.Left = Left + 20;
							_editor.Top  = Top  + 20;
						}
						_editor.Palette = _palette;
						_editor.Show();
					}
					else
						_editor.BringToFront();
				}
			}
		}

		private void OnEditorClosing(object sender, CancelEventArgs e)
		{
			_miEdit.Checked = false;

			e.Cancel = true;
			_editor.Hide();
		}

		public string SelectedPalette
		{
			get { return _palette.Label; }
			set
			{
				foreach (Palette pal in _paletteDictionary.Keys)
					if (pal.Label.Equals(value))
						OnPaletteClick(_paletteDictionary[pal], null);
			}
		}

		private MenuItem AddPalette(Palette pal, Menu it)
		{
			var itPal = new MenuItem(pal.Label);
			itPal.Tag = pal;
			it.MenuItems.Add(itPal);

			itPal.Click += OnPaletteClick;
			_paletteDictionary[pal] = itPal;

			((Dictionary<string, Palette>)_share[SharedSpace.Palettes])[pal.Label] = pal;
			return itPal;
		}

		private void OnPaletteClick(object sender, EventArgs e)
		{
			var pal = (Palette)((MenuItem)sender).Tag;
			if (pal != null && pal != _palette)
			{
				if (_palette != null)
					_paletteDictionary[_palette].Checked = false;

				_palette = pal;
				_paletteDictionary[_palette].Checked = true;

				_viewPanel.Pal = _palette;

				if (_editor != null)
					_editor.Palette = _palette;

				_viewPanel.Refresh();
			}
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

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string pfePck = ofd.FileName;
					LoadSpriteCollectionFile(pfePck, false);
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
		public void LoadSpriteCollectionFile(string pfePck, bool help)
		{
			string pfeTab = pfePck.Substring(0, pfePck.Length - 4);
			pfeTab += PckSpriteCollection.TabExt;

			if (File.Exists(pfeTab))
			{
				var fileType = new XCImageFile(32, 40);

				using (var strPck = File.OpenRead(pfePck))
				using (var strTab = File.OpenRead(pfeTab))
				{
					XCImageCollection pckPack = null;
					try
					{
						pckPack = new PckSpriteCollection(
														strPck,
														strTab,
														2,
														Palette.UfoBattle);
					}
					catch (Exception)
					{
						pckPack = new PckSpriteCollection(
														strPck,
														strTab,
														4,
														Palette.UfoBattle);
					}

					if (pckPack != null)
					{
						pckPack.ImageFile = fileType;
						pckPack.Label = Path.GetFileNameWithoutExtension(pfePck);

						if (pckPack.Pal == null)
							pckPack.Pal = Palette.UfoBattle;
					}

					OnPaletteClick((MenuItem)_paletteDictionary[pckPack.ImageFile.DefaultPalette], null);

					_viewPanel.SpritePack = pckPack;


					UpdateCaption();

					if (help)
					{
						pMapViewHelp.Visible  =
						miMapViewHelp.Visible = true;
					}
				}
			}
			else
				XConsole.AdZerg("ERROR: tab file does not exist: " + pfeTab);
		}

		private void UpdateCaption()
		{
			Text = "Pck View - " + _viewPanel.SpritePack.Label + " [" + _viewPanel.SpritePack.Count + "] total";
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
				if (_viewPanel.SelectedSprites.Count != 0)
				{
					miBytes.Checked = true;

					var selected = _viewPanel.SelectedSprites[_viewPanel.SelectedSprites.Count - 1];
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

		private void OnTransparencyClick(object sender, EventArgs e)
		{
			_palette.EnableTransparency(miTransparent.Checked = !miTransparent.Checked);
			_viewPanel.SpritePack.Pal = _palette;

			_viewPanel.Refresh();
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

		private void OnSaveDirectoryClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
			if (_viewPanel.SpritePack != null)
			{
				string fileStart = String.Empty;
//				string extStart  = String.Empty;

				if (_viewPanel.SpritePack.Label.IndexOf(".", StringComparison.Ordinal) > 0)
				{
					fileStart = _viewPanel.SpritePack.Label.Substring(0, _viewPanel.SpritePack.Label.IndexOf(".", StringComparison.Ordinal));
//					extStart  = _totalViewPck.Collection.Name.Substring(_totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal) + 1);
				}

				sfdBmpSingle.FileName = fileStart;

				sfdBmpSingle.Title = "Select directory to save images in";

				if (sfdBmpSingle.ShowDialog() == DialogResult.OK)
				{
					string path = sfdBmpSingle.FileName.Substring(0, sfdBmpSingle.FileName.LastIndexOf(@"\", StringComparison.Ordinal));
					string file = sfdBmpSingle.FileName.Substring(sfdBmpSingle.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					string fName = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));
					string ext = file.Substring(file.LastIndexOf(".", StringComparison.Ordinal) + 1);

//					int countNum = 0;
//					int charPos = fName.Length - 1;
//					int tens = 1;
//					while (charPos >= 0 && Char.IsDigit(fName[charPos]))
//					{
//						int digit = int.Parse(fName[charPos].ToString());
//						countNum += digit*tens;
//						tens *= 10;
//						fName = fName.Substring(0, charPos--);
//					}

					string zeros = String.Empty;
					int tens = _viewPanel.SpritePack.Count;
					while (tens > 0)
					{
						zeros += "0";
						tens /= 10;
					}

					var progress = new ProgressWindow(this);
					progress.Minimum = 0;
					progress.Maximum = _viewPanel.SpritePack.Count;
					progress.Width = 300;
					progress.Height = 50;

					progress.Show();
					foreach (XCImage xc in _viewPanel.SpritePack)
					{
						//Console.WriteLine("Save to: " + path + @"\" + fName + (xc.FileNum + countNum) + "." + ext);
						//Console.WriteLine("Save: " + path + @"\" + fName + string.Format("{0:" + zeros + "}", xc.FileNum) + "." + ext);
						XCBitmap.Save(
								path + @"\" + fName + string.Format(
																System.Globalization.CultureInfo.InvariantCulture,
																"{0:" + zeros + "}",
																xc.FileId) + "." + ext,
								xc.Image);
						//Console.WriteLine("---");
						progress.Value = xc.FileId;
					}
					progress.Hide();
				}
			}
		}

		private void OnConsoleClick(object sender, EventArgs e)
		{
			if (_console.Visible)
				_console.BringToFront();
			else
				_console.Show();
		}

		private void OnCompareClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
			var original = _viewPanel.SpritePack;

			OnOpenClick(null, null);

			var newCollection = _viewPanel.SpritePack;

			_viewPanel.SpritePack = original;

			if (Controls.Contains(_viewPanel))
			{
				Controls.Remove(_viewPanel);

				_tabs = new TabControl();
				_tabs.Dock = DockStyle.Fill;
				pViewer.Controls.Add(_tabs);

				var tab = new TabPage();
				tab.Controls.Add(_viewPanel);
				tab.Text = "Original";
				_tabs.TabPages.Add(tab);

				tab = new TabPage();
				var panel = new ViewPanel();
				panel.ContextMenu = BuildViewerContextMenu();
				panel.Dock = DockStyle.Fill;
				panel.SpritePack = newCollection;
				tab.Controls.Add(panel);
				tab.Text = "New";
				_tabs.TabPages.Add(tab);
			}
		}

		private void OnSaveCollectionClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
/*			_dialogFilter.SetFilter(XCImageFile.Filter.Save);
			_dictSaveFiles.Clear();
//			_loadedTypes.CreateFilter(_dialogFilter, _dictSaveFiles);
			var dir = Path.GetDirectoryName(_path);
			var fileWithoutExt = Path.GetFileNameWithoutExtension(_path);

			// Backup
			FileBackupManager.Backup(_path);

			// Save
			PckSpriteCollection.Save(
								dir,
								fileWithoutExt,
								_totalViewPck.Collection,
								_depth);
			SavedFile = true; */
		}

		private void OnMapViewHelpClick(object sender, EventArgs e)
		{
			pMapViewHelp.Visible = !pMapViewHelp.Visible;
			miMapViewHelp.Checked = pMapViewHelp.Visible;
		}

		private void OnMapViewGotItClick(object sender, EventArgs e)
		{
			pMapViewHelp.Visible  =
			miMapViewHelp.Checked = false;
		}

		private void OnShown(object sender, EventArgs e)
		{
//			_console.Show();
		}

		private void OnPckViewFormClosing(object sender, FormClosingEventArgs e)
		{
			if (miBytes.Checked)
				BytesFormHelper.CloseBytes();
		}
	}
}
