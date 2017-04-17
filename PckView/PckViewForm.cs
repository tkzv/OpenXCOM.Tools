using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
//using System.Reflection;
using System.Windows.Forms;

//using DSShared.FileSystems;
//using DSShared.Loadable;
using DSShared.Windows;

using PckView.Args;
using PckView.Forms.ImageBytes;
using PckView.Properties;

using XCom;
using XCom.Interfaces;

//.net file security
// http://www.c-sharpcorner.com/Code/2002/April/DotNetSecurity.asp


namespace PckView
{
	public sealed partial class PckViewForm
		:
			Form
	{
		private TotalViewPck _totalViewPck;
		private Palette _palette;
		private Editor _editor;
		private SharedSpace _share;

		private ConsoleForm _console;
		private TabControl tabs;
		private MenuItem editImage, replaceImage, saveImage, deleteImage, addMany;

		private Dictionary<Palette, MenuItem> _paletteDictionary;

		public bool SavedFile
		{ get; private set; }


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

			_totalViewPck = new TotalViewPck();
			_totalViewPck.Dock = DockStyle.Fill;
			pViewer.Controls.Add(_totalViewPck);

			_totalViewPck.ViewPanel.DoubleClick += OnDoubleClick;
			_totalViewPck.MouseClickedEvent += OnViewClick;
			_totalViewPck.ImageCollectionSetEvent += OnImageCollectionSet;
			_totalViewPck.ContextMenu = BuildContextMenu();

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
			_paletteDictionary[_palette].Checked = true;
			_totalViewPck.Pal = _palette;

			_editor = new Editor(null);
			_editor.Closing += OnEditorClosing;

			if (_editor != null)
				_editor.Palette = _palette;


			var regInfo = new RegistryInfo(this, "PckView");	// subscribe to Load and Closing events.
//			regInfo.AddProperty("FilterIndex");					// TODO: these won't work until I implement them in RegistryInfo.
//			regInfo.AddProperty("SelectedPalette");

//			miHq2x.Visible &= File.Exists("hq2xa.dll");

//			_loadedTypes = new LoadOfType<XCImageFile>();
//			_loadedTypes.OnLoad += OnTypesLoaded;
//			_share[SharedSpace.ImageTypes] = _loadedTypes.AllLoaded;

//			_loadedTypes.OnLoad += sortLoaded;

//			_loadedTypes.LoadFrom(Assembly.GetExecutingAssembly());
//			_loadedTypes.LoadFrom(Assembly.GetAssembly(typeof(XCImageFile)));

//			string dir = _share[SharedSpace.CustomDirectory].ToString();	// TODO: I don't trust that since changing SharedSpace.
//			if (Directory.Exists(dir))										// it may well need an explicit cast to (PathInfo)
//			{
//				XConsole.AdZerg("Custom directory exists: " + dir);
//				foreach (string st in Directory.GetFiles(dir))
//				{
//					if (st.EndsWith(".dll", StringComparison.Ordinal))
//					{
//						XConsole.AdZerg("Loading dll: " + st);
//						_loadedTypes.LoadFrom(Assembly.LoadFrom(st));
//					}
//					else if (st.EndsWith(XCProfile.ProfileExt, StringComparison.Ordinal))
//					{
//						foreach (XCProfile ip in ImageProfile.LoadFile(st))
//							_loadedTypes.Add(ip);
//					}
//				}
//			}

//			_dialogFilter = new OpenSaveFilter();
//			_dialogFilter.SetFilter(XCImageFile.Filter.Open);

//			_dictOpenFiles = new Dictionary<int, XCImageFile>();
//			_dictSaveFiles = new Dictionary<int, XCImageFile>();

//			_dialogFilter.SetFilter(XCImageFile.Filter.Open);
//			string filter = _loadedTypes.CreateFilter(_dialogFilter, _dictOpenFiles);
//			openFile.Filter = filter;
		}


		private void OnImageCollectionSet(object sender, ImageCollectionSetEventArgs e)
		{
			bool enabled = (e.Collection != null);

			miSaveAs.Enabled          =
			miTransparentMenu.Enabled =
			miBytesMenu.Enabled       =
			miPaletteMenu.Enabled     = enabled;

//			if (enabled)
//			{
//				bytesMenu.Enabled =
//				miPalette.Enabled =
//				transItem.Enabled = (e.Collection.ImageFile.FileOptions.BitDepth == 8);

//				XConsole.AdZerg("bpp is: " + e.Collection.ImageFile.FileOptions.BitDepth);
//			}
		}

/*		private void OnTypesLoaded(object sender, LoadOfType<XCImageFile>.TypeLoadArgs e)
		{
			var obj = e.LoadedObj as xcCustom;
			if (obj != null)
				_xcCustom = obj;
		} */

/*		internal void LoadProfile(string file)
		{
			foreach (XCProfile ip in ImageProfile.LoadFile(file))
				_loadedTypes.Add(ip);

			_dialogFilter.SetFilter(XCImageFile.Filter.Open);
			_dictOpenFiles.Clear();
			openFile.Filter = _loadedTypes.CreateFilter(_dialogFilter, _dictOpenFiles);
		} */

		private ContextMenu BuildContextMenu()
		{
			var menu = new ContextMenu();

			saveImage = new MenuItem("Save as BMP");
			menu.MenuItems.Add(saveImage);
			saveImage.Click += OnViewClick;

			replaceImage = new MenuItem("Replace with BMP");
			menu.MenuItems.Add(replaceImage);
			replaceImage.Click += OnReplaceClick;

			addMany = new MenuItem("Add Bmp");
			menu.MenuItems.Add(addMany);
			addMany.Click += OnAddManyClick;

			deleteImage = new MenuItem("Delete\tDel");
			menu.MenuItems.Add(deleteImage);
			deleteImage.Click += OnRemoveClick;

			editImage = new MenuItem("Edit");
			menu.MenuItems.Add(editImage);
			editImage.Click += OnEditClick;

			editImage.Enabled = false;

			return menu;
		}

		private void OnAddManyClick(object sender, EventArgs e)
		{
			if (_totalViewPck.Collection != null)
			{
				ofdBmp.Title = "Hold shift to select multiple files.";
				ofdBmp.Multiselect = true;

				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					foreach (string st in ofdBmp.FileNames)
					{
						var b = new Bitmap(st);
						_totalViewPck.Collection.Add(Bmp.LoadTile(
															b,
															0,
															_totalViewPck.Pal,
															0, 0,
															_totalViewPck.Collection.ImageFile.ImageSize.Width,
															_totalViewPck.Collection.ImageFile.ImageSize.Height));
					}
					Refresh();
				}
				UpdateText();
			}
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

/*		public int FilterIndex
		{
			get { return openFile.FilterIndex; }
			set { openFile.FilterIndex = value; }
		} */

		private void OnDoubleClick(object sender, EventArgs e)
		{
//			if (_editor.Visible)
			OnEditClick(sender, e);
		}

		private MenuItem AddPalette(Palette pal, Menu it)
		{
			var it0 = new MenuItem(pal.Label);
			it0.Tag = pal;
			it.MenuItems.Add(it0);

			it0.Click += OnPaletteClick;
			_paletteDictionary[pal] = it0;

			((Dictionary<string, Palette>)_share[SharedSpace.Palettes])[pal.Label] = pal;
			return it0;
		}

		private void OnPaletteClick(object sender, EventArgs e)
		{
			if (_palette != null)
				_paletteDictionary[_palette].Checked = false;

			_palette = (Palette)((MenuItem)sender).Tag;
			_paletteDictionary[_palette].Checked = true;

			_totalViewPck.Pal = _palette;

			if (_editor != null)
				_editor.Palette = _palette;

			_totalViewPck.Refresh();
		}

		private void OnViewClick(object sender, PckViewMouseEventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count > 0)
			{
				editImage.Enabled   =
				saveImage.Enabled   =
				deleteImage.Enabled = true;
				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				BytesFormHelper.ReloadBytes(selected);
			}
			else // selected is null
			{
				BytesFormHelper.ReloadBytes(null);

				editImage.Enabled   =
				saveImage.Enabled   =
				deleteImage.Enabled = false;
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
				ofd.Title = "Select a Pck File";
//				ofd.InitialDirectory = _share.GetString(SharedSpace.ApplicationDirectory);

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string pfePck = ofd.FileName;
					LoadPckFile(pfePck);
				}
			}
		}

		/// <summary>
		/// Loads a file from MapView.Forms.MapObservers.TileViews.TileView.OnPckEditorClick()
		/// NOTE: with a string like that you'd think this was .NET itself.
		/// </summary>
		/// <param name="pfePck">fullpath of a PCK file; check existence of file
		/// before call</param>
		/// <param name="help">true to show help-splash for a call from MapView</param>
		public void LoadPckFile(string pfePck, bool help = false)
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
					_totalViewPck.Collection = pckPack;

					UpdateText();

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

		private void OnSaveAsClick(object sender, EventArgs e)
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

		private void OnViewClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 0)
			{
				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				if (_totalViewPck.Collection != null)
				{
					if (_totalViewPck.Collection.ImageFile.SingleFile != null)
					{
						string file = _totalViewPck.Collection.Label.Substring(0, _totalViewPck.Collection.Label.IndexOf(".", StringComparison.Ordinal));
//						string ext  = _totalViewPck.Collection.Name.Substring(_totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal) + 1);
						sfdBmpSingle.FileName = file + selected.Image.FileId;
					}
					else
						sfdBmpSingle.FileName = _totalViewPck.Collection.Label + selected.Image.FileId;
	
					if (sfdBmpSingle.ShowDialog() == DialogResult.OK)
						Bmp.Save(sfdBmpSingle.FileName, selected.Image.Image);
				}
			}
		}

		private void OnReplaceClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 1)
			{
				MessageBox.Show(
							"Must select 1 item only.",
							Text,
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (_totalViewPck.Collection != null )
			{
				var title = string.Empty ;
				foreach (var selectedIndex in _totalViewPck.SelectedItems)
				{
					if (!String.IsNullOrEmpty(title))
						title += ", ";

					title += selectedIndex.Item.Index ;
				}

				ofdBmp.Title = "Selected number: " + title;
				ofdBmp.Multiselect = false;
				if (ofdBmp.ShowDialog() == DialogResult.OK)
				{
					var b = new Bitmap(ofdBmp.FileName);
					var image = Bmp.Load(
										b,
										_totalViewPck.Pal,
										_totalViewPck.Collection.ImageFile.ImageSize.Width,
										_totalViewPck.Collection.ImageFile.ImageSize.Height,
										1)[0];
					_totalViewPck.ChangeItem(_totalViewPck.SelectedItems[0].Item.Index, image);
					Refresh();
				}
				UpdateText();
			}
		}

		private void UpdateText()
		{
			Text = "Pck View - " + _totalViewPck.Collection.Label + " [" + _totalViewPck.Collection.Count + "] total";
		}

		private void OnRemoveClick(object sender, EventArgs e)
		{
			_totalViewPck.RemoveSelected();
			UpdateText();
			Refresh();
		}

		private void OnShowBytesClick(object sender, EventArgs e)
		{
			if (!miBytes.Checked)
			{
				if (_totalViewPck.SelectedItems.Count != 0)
				{
					miBytes.Checked = true;

					var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
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
			_palette.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

			_totalViewPck.Collection.Pal = _palette;
			Refresh();
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			new About().ShowDialog(this);
		}

		private void OnHelpClick(object sender, EventArgs e)
		{
			new Help().ShowDialog(this);
		}

		private void OnEditClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 0)
			{
				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				if (selected != null)
				{
					_editor.Image = (XCImage)selected.Image.Clone();

					if (_editor.Visible)
						_editor.BringToFront();
					else
					{
						_editor.Left = Right;
						_editor.Top = Top;
						_editor.Palette = _palette;
						_editor.Show();
					}
				}
			}
		}

		private void OnEditorClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			_editor.Hide();
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

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (deleteImage.Enabled && e.KeyCode == Keys.Delete)
				OnRemoveClick(null, null);
		}

		private void OnModListClick(object sender, EventArgs e) // disabled w/ Visible=FALSE in designer.
		{
//			using (var f = new ModForm())
//			{
//				f.SharedSpace = _share;
//				f.ShowDialog();
//			}
		}

		private void OnSaveDirectoryClick(object sender, EventArgs e)
		{
			if (_totalViewPck.Collection != null)
			{
				string fileStart = String.Empty;
//				string extStart  = String.Empty;

				if (_totalViewPck.Collection.Label.IndexOf(".", StringComparison.Ordinal) > 0)
				{
					fileStart = _totalViewPck.Collection.Label.Substring(0, _totalViewPck.Collection.Label.IndexOf(".", StringComparison.Ordinal));
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
					int tens = _totalViewPck.Collection.Count;
					while (tens > 0)
					{
						zeros += "0";
						tens /= 10;
					}

					var progress = new ProgressWindow(this);
					progress.Minimum = 0;
					progress.Maximum = _totalViewPck.Collection.Count;
					progress.Width = 300;
					progress.Height = 50;

					progress.Show();
					foreach (XCImage xc in _totalViewPck.Collection)
					{
						//Console.WriteLine("Save to: " + path + @"\" + fName + (xc.FileNum + countNum) + "." + ext);
						//Console.WriteLine("Save: " + path + @"\" + fName + string.Format("{0:" + zeros + "}", xc.FileNum) + "." + ext);
						Bmp.Save(
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

		private void OnCompareClick(object sender, EventArgs e)
		{
			var original = _totalViewPck.Collection;

			OnOpenClick(null, null);

			var newCollection = _totalViewPck.Collection;

			_totalViewPck.Collection = original;

			if (Controls.Contains(_totalViewPck))
			{
				Controls.Remove(_totalViewPck);

				tabs = new TabControl();
				tabs.Dock = DockStyle.Fill;
				pViewer.Controls.Add(tabs);

				var tp = new TabPage();
				tp.Controls.Add(_totalViewPck);
				tp.Text = "Original";
				tabs.TabPages.Add(tp);

				tp = new TabPage();
				var tvNew = new TotalViewPck();
				tvNew.ContextMenu = BuildContextMenu();
				tvNew.Dock = DockStyle.Fill;
				tvNew.Collection = newCollection;
				tp.Controls.Add(tvNew);
				tp.Text = "New";
				tabs.TabPages.Add(tp);
			}
		}

		private void OnSaveClick(object sender, EventArgs e)
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
