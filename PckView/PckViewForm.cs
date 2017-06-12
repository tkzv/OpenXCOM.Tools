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
	internal delegate void PaletteChangedEventHandler(Palette pal);


	public sealed partial class PckViewForm
		:
			Form
	{
		#region Events (static)
		internal static event PaletteChangedEventHandler PaletteChangedEvent;
		#endregion


		#region Properties (static)
		private static Palette _palette;
		internal static Palette Pal
		{
			get { return _palette; }
			set
			{
				_palette = value;

				var handler = PaletteChangedEvent;
				if (handler != null)
					handler(_palette);
			}
		}
		#endregion


		#region Fields
		private PckViewPanel _pnlView = new PckViewPanel();

		private EditorForm _feditor = new EditorForm();
		private ConsoleForm _fconsole;

		private TabControl _tcTabs;

		private MenuItem _miEdit;
//		private MenuItem _miReplace;
//		private MenuItem _miSave;
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

//			XConsole.AdZerg("Application directory: " + _share[SharedSpace.ApplicationDirectory]);			// TODO: I don't trust that since changing SharedSpace.
//			XConsole.AdZerg("Settings directory: "    + _share[SharedSpace.SettingsDirectory].ToString());	// it may well need an explicit cast to (PathInfo)
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
			miTransparentMenu.Enabled =
			miBytesMenu.Enabled       =
			miPaletteMenu.Enabled     = valid;
		}

		private void OnSpriteClick(object sender, EventArgs e)
		{
			bool valid = _pnlView.Selected != null
					  && _pnlView.Selected.Count != 0;

			_miEdit.Enabled   = valid;
//			_miSave.Enabled   =
//			_miDelete.Enabled = valid;

			if (valid)
			{
				var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
				BytesFormHelper.ReloadBytes(selected);
			}
			else
				BytesFormHelper.ReloadBytes(null);
		}

		private void OnSpriteSaveClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			if (_pnlView.Selected.Count != 0)
			{
				var selected = _pnlView.Selected[_pnlView.Selected.Count - 1];
				if (_pnlView.Spriteset != null)
				{
					sfdBmpSingle.FileName = _pnlView.Spriteset.Label + selected.Image.FileId;

					if (sfdBmpSingle.ShowDialog() == DialogResult.OK)
						XCBitmap.Save(sfdBmpSingle.FileName, selected.Image.Image);
				}
			}
		}

		private void OnSpriteReplaceClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
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
					var b = new Bitmap(ofdBmp.FileName);
					var image = XCBitmap.LoadSpriteset(
													b,
													Pal,
													_pnlView.Spriteset.ImageFile.ImageSize.Width,
													_pnlView.Spriteset.ImageFile.ImageSize.Height,
													1)[0];
					_pnlView.SpriteReplace(_pnlView.Selected[0].Id, image);
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteAddClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
		{
			if (_pnlView.Spriteset != null)
			{
				ofdBmp.Title = "Hold shift to select multiple files.";
				ofdBmp.Multiselect = true;

				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					foreach (string file in ofdBmp.FileNames)
					{
						var b = new Bitmap(file);
						_pnlView.Spriteset.Add(XCBitmap.LoadSprite(
																b,
																0,
																Pal,
																0, 0,
																_pnlView.Spriteset.ImageFile.ImageSize.Width,
																_pnlView.Spriteset.ImageFile.ImageSize.Height));
					}
					Refresh();
				}
				UpdateCaption();
			}
		}

		private void OnSpriteDeleteClick(object sender, EventArgs e) // disabled in BuildViewerContextMenu()
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

				pal.SetTransparent(miTransparent.Checked);
				Pal = pal;

				_paletteItems[Pal].Checked = true;

				_pnlView.Spriteset.Pal = Pal;

				_pnlView.Refresh();
			}
		}

		private void OnTransparencyClick(object sender, EventArgs e)
		{
			Pal.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

			_pnlView.Spriteset.Pal = Pal;

			var handler = PaletteChangedEvent;
			if (handler != null)
				handler(_palette);
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
				var fileType = new XCImageFile();

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
						spriteset.ImageFile = fileType;
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

		private void OnSaveDirectoryClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
			if (_pnlView.Spriteset != null)
			{
				string fileStart = String.Empty;
//				string extStart  = String.Empty;

				if (_pnlView.Spriteset.Label.IndexOf(".", StringComparison.Ordinal) > 0)
				{
					fileStart = _pnlView.Spriteset.Label.Substring(0, _pnlView.Spriteset.Label.IndexOf(".", StringComparison.Ordinal));
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
					int tens = _pnlView.Spriteset.Count;
					while (tens > 0)
					{
						zeros += "0";
						tens /= 10;
					}

					var progress = new ProgressWindow(this);
					progress.Width  = 300;
					progress.Height =  50;
					progress.Minimum = 0;
					progress.Maximum = _pnlView.Spriteset.Count;

					progress.Show();
					foreach (XCImage sprite in _pnlView.Spriteset)
					{
						//Console.WriteLine("Save to: " + path + @"\" + fName + (xc.FileNum + countNum) + "." + ext);
						//Console.WriteLine("Save: " + path + @"\" + fName + string.Format("{0:" + zeros + "}", xc.FileNum) + "." + ext);
						XCBitmap.Save(
								path + @"\" + fName + string.Format(
																System.Globalization.CultureInfo.InvariantCulture,
																"{0:" + zeros + "}",
																sprite.FileId) + "." + ext,
								sprite.Image);
						//Console.WriteLine("---");
						progress.Value = sprite.FileId;
					}
					progress.Hide();
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
}
