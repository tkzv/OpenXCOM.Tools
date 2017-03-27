using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared.FileSystems;
using DSShared.Loadable;
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
	public partial class PckViewForm
		:
		Form
	{
		private TotalViewPck _totalViewPck;
		private Palette _palette;
		private Editor _editor;
		private SharedSpace _share;
		private LoadOfType<IXCImageFile> _loadedTypes;

		private ConsoleForm console;
		private TabControl tabs;
		private MenuItem editImage, replaceImage, saveImage, deleteImage, addMany;

		private OpenSaveFilter osFilter;
		private xcCustom xcCustom;

		private Dictionary<Palette, MenuItem> _dictPalettes;

		private Dictionary<int, IXCImageFile> openDictionary;
		private Dictionary<int, IXCImageFile> saveDictionary;

		private string _currentFilePath;
		private int _currentFileBpp; // kL_note: what's with all the 'current' -> what else would it be.

		private readonly IFileBackupManager _fileBackupManager = new FileBackupManager();

		public bool SavedFile
		{ get; private set; }


		public PckViewForm()
		{
			InitializeComponent();

			#region shared space information

			var consoleShare = new ConsoleSharedSpace(new SharedSpace());
			console = consoleShare.GetNewConsole();
			console.FormClosing += delegate(object sender, FormClosingEventArgs e)
			{
				e.Cancel = true;
				console.Hide();
			};
			FormClosed += (sender, e) => console.Close();

			_share = SharedSpace.Instance;
			_share.AllocateObject("PckView", this);
			_share.AllocateObject(SharedSpace.AppDir,      Environment.CurrentDirectory);
			_share.AllocateObject(SharedSpace.SettingsDir, Environment.CurrentDirectory + @"\settings");
			_share.AllocateObject(SharedSpace.CustomDir,   Environment.CurrentDirectory + @"\custom");
		
			xConsole.AddLine("Current directory: "  + _share[SharedSpace.AppDir]);					// TODO: I don't trust that since changing SharedSpace.
			xConsole.AddLine("Settings directory: " + _share[SharedSpace.SettingsDir].ToString());	// it may well need an explicit cast to (PathInfo)
			xConsole.AddLine("Custom directory: "   + _share[SharedSpace.CustomDir].ToString());
			#endregion

			_totalViewPck = new TotalViewPck();
			_totalViewPck.Dock = DockStyle.Fill;
			DrawPanel.Controls.Add(_totalViewPck);

			_totalViewPck.View.DoubleClick += doubleClick;
			_totalViewPck.ViewClicked += viewClicked;
			_totalViewPck.XCImageCollectionSet += v_XCImageCollectionSet;
			_totalViewPck.ContextMenu = makeContextMenu();

			SaveMenuItem.Visible = false ;

			_share[SharedSpace.Palettes] = new Dictionary<string, Palette>();
			_dictPalettes = new Dictionary<Palette, MenuItem>();

			AddPalette(Palette.UFOBattle,		miPalette);
			AddPalette(Palette.UFOGeo,			miPalette);
			AddPalette(Palette.UFOGraph,		miPalette);
			AddPalette(Palette.UFOResearch,		miPalette);
			AddPalette(Palette.TFTDBattle,		miPalette);
			AddPalette(Palette.TFTDGeo,			miPalette);
			AddPalette(Palette.TFTDGraph,		miPalette);
			AddPalette(Palette.TFTDResearch,	miPalette);

			_palette = Palette.UFOBattle;

			_dictPalettes[_palette].Checked = true;
			_totalViewPck.Pal = _palette;

			_editor = new Editor(null);
			_editor.Closing += new CancelEventHandler(editorClosing);

			if (_editor != null)
				_editor.Palette = _palette;


			var ri = new RegistryInfo(this, "PckView");
			ri.AddProperty("FilterIndex");
			ri.AddProperty("SelectedPalette");

			miHq2x.Visible &= File.Exists("hq2xa.dll");

			_loadedTypes = new LoadOfType<IXCImageFile>();
			_loadedTypes.OnLoad += loadedTypes_OnLoad;
			_share[SharedSpace.ImageMods] = _loadedTypes.AllLoaded;

//			loadedTypes.OnLoad += sortLoaded;

			_loadedTypes.LoadFrom(Assembly.GetExecutingAssembly());
			_loadedTypes.LoadFrom(Assembly.GetAssembly(typeof(IXCImageFile)));

			string dir = _share[SharedSpace.CustomDir].ToString();	// TODO: I don't trust that since changing SharedSpace.
			if (Directory.Exists(dir))								// it may well need an explicit cast to (PathInfo)
			{
				xConsole.AddLine("Custom directory exists: " + dir);
				foreach (string st in Directory.GetFiles(dir))
				{
					if (st.EndsWith(".dll", StringComparison.Ordinal))
					{
						xConsole.AddLine("Loading dll: " + st);
						_loadedTypes.LoadFrom(Assembly.LoadFrom(st));
					}
					else if (st.EndsWith(XCProfile.ProfileExt, StringComparison.Ordinal))
					{
						foreach (XCProfile ip in ImgProfile.LoadFile(st))
							_loadedTypes.Add(ip);
					}
				}
			}

			osFilter = new OpenSaveFilter();
			osFilter.SetFilter(IXCImageFile.Filter.Open);

			openDictionary = new Dictionary<int, IXCImageFile>();
			saveDictionary = new Dictionary<int, IXCImageFile>();

			osFilter.SetFilter(IXCImageFile.Filter.Open);
			string filter = _loadedTypes.CreateFilter(osFilter, openDictionary);
			openFile.Filter = filter;
		}


		private void v_XCImageCollectionSet(object sender, XCImageCollectionSetEventArgs e)
		{
			bool enabled = (e.Collection != null);

			saveitem.Enabled  =
			transItem.Enabled =
			bytesMenu.Enabled =
			miPalette.Enabled = enabled;

			if (enabled)
			{
				bytesMenu.Enabled =
				miPalette.Enabled =
				transItem.Enabled = (e.Collection.IXCFile.FileOptions.BitDepth == 8);

				xConsole.AddLine("bpp is: " + e.Collection.IXCFile.FileOptions.BitDepth);
			}
		}

		void loadedTypes_OnLoad(object sender, LoadOfType<IXCImageFile>.TypeLoadArgs e)
		{
			var obj = e.LoadedObj as xcCustom;
			if (obj != null)
				xcCustom = obj;
		}

		public void LoadProfile(string s)
		{
			foreach (XCProfile ip in ImgProfile.LoadFile(s))
				_loadedTypes.Add(ip);

			osFilter.SetFilter(IXCImageFile.Filter.Open);
			openDictionary.Clear();
			openFile.Filter = _loadedTypes.CreateFilter(osFilter, openDictionary);
		}

		private ContextMenu makeContextMenu()
		{
			var cm = new ContextMenu();
			saveImage = new MenuItem("Save as BMP");
			cm.MenuItems.Add(saveImage);
			saveImage.Click += new EventHandler(viewClick);

			replaceImage = new MenuItem("Replace with BMP");
			cm.MenuItems.Add(replaceImage);
			replaceImage.Click += new EventHandler(replaceClick);

			addMany = new MenuItem("Add Bmp");
			cm.MenuItems.Add(addMany);
			addMany.Click += new EventHandler(addMany_Click);

			var sb = new MenuItem("Show Bytes");
			cm.MenuItems.Add(sb);
			sb.Click += new EventHandler(sb_Click);

			deleteImage = new MenuItem("Delete\tDel");
			cm.MenuItems.Add(deleteImage);
			deleteImage.Click += new EventHandler(removeClick);

			editImage = new MenuItem("Edit");
			cm.MenuItems.Add(editImage);
			editImage.Click += new EventHandler(editClick);
			editImage.Enabled = false;
//			editImage.Visible = false;

			return cm;
		}

		void sb_Click(object sender, EventArgs e)
		{
			TotalViewPck totalViewPck = _totalViewPck;

			if (tabs != null)
			{
				foreach (object o in tabs.SelectedTab.Controls)
				{
					var totalView = o as TotalViewPck;
					if (totalView != null)
						totalViewPck = totalView;
				}
			}

			if (totalViewPck != null)
			{
				if (totalViewPck.SelectedItems.Count != 1)
				{
					MessageBox.Show(
								"Must select 1 item only",
								Text,
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation);
				}
				else
				{
					var f = new Form();
					var rtb = new RichTextBox();
					rtb.Dock = DockStyle.Fill;
					f.Controls.Add(rtb);

					foreach (byte b in totalViewPck.SelectedItems[0].Image.Offsets)
						rtb.Text += string.Format("{0:x} ", b);

					f.Text = "Bytes: " + totalViewPck.SelectedItems[0].Image.Offsets.Length;
					f.Show();
				}
			}
		}

		void addMany_Click(object sender, EventArgs e)
		{
			if (_totalViewPck.Collection != null)
			{
				openBMP.Title = "Hold shift to select multiple files";
				openBMP.Multiselect = true;

				if (openBMP.ShowDialog() == DialogResult.OK)
				{
					foreach (string st in openBMP.FileNames)
					{
						var b = new Bitmap(st);
						_totalViewPck.Collection.Add(Bmp.LoadTile(
															b,
															0,
															_totalViewPck.Pal,
															0, 0,
															_totalViewPck.Collection.IXCFile.ImageSize.Width,
															_totalViewPck.Collection.IXCFile.ImageSize.Height));
					}
					Refresh();
				}
				UpdateText();
			}
		}

		public string SelectedPalette
		{
			get { return _palette.Name; }
			set
			{
				foreach (Palette pal in _dictPalettes.Keys)
					if (pal.Name.Equals(value))
						palClick(_dictPalettes[pal], null);
			}
		}

		public int FilterIndex
		{
			get { return openFile.FilterIndex; }
			set { openFile.FilterIndex = value; }
		}

		private void doubleClick(object sender, EventArgs e)
		{
			if (_editor.Visible)
				editClick(sender, e);
		}

		public MenuItem AddPalette(Palette pal, MenuItem it)
		{
			var it0 = new MenuItem(pal.Name);
			it0.Tag = pal;
			it.MenuItems.Add(it0);

			it0.Click += palClick;
			_dictPalettes[pal] = it0;

			((Dictionary<string, Palette>)_share[SharedSpace.Palettes])[pal.Name] = pal;
			return it0;
		}

		private void palClick(object sender, EventArgs e)
		{
			if (_palette != null)
				_dictPalettes[_palette].Checked = false;

			_palette = (Palette)((MenuItem)sender).Tag;
			_dictPalettes[_palette].Checked = true;

			_totalViewPck.Pal = _palette;

			if (_editor != null)
				_editor.Palette = _palette;

			_totalViewPck.Refresh();
		}

		private void viewClicked(object sender, PckViewMouseClickArgs e)
		{
			if (_totalViewPck.SelectedItems.Count > 0)
			{
				editImage.Enabled = true;
				saveImage.Enabled = true;
				deleteImage.Enabled = true;
				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				BytesFormHelper.ReloadBytes(selected);
			}
			else // selected is null
			{
				BytesFormHelper.ReloadBytes(null);

				editImage.Enabled = false;
				saveImage.Enabled = false;
				deleteImage.Enabled = false;
			}
		}

		private void quitItem_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void openItem_Click(object sender, System.EventArgs e)
		{
			if (openFile.ShowDialog() == DialogResult.OK)
			{
				OnResize(null);

				string fName = openFile.FileName.Substring(openFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1).ToLower();

				string ext  = fName.Substring(fName.LastIndexOf(".", StringComparison.Ordinal));
				string file = fName.Substring(0, fName.LastIndexOf(".", StringComparison.Ordinal));
				string path = openFile.FileName.Substring(0, openFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

				XCImageCollection toLoad = null;
				bool recover = false;

				// remove saving - there are too many formats and stuff,
				// I will implement only one type of direct saving.
				_currentFilePath = null;
				SaveMenuItem.Visible = false;

				//Console.WriteLine(openFile.FilterIndex+" -> " + filterIndex[openFile.FilterIndex].GetType());
#if !DEBUG
				try
				{
#endif
					IXCImageFile filterIdx = openDictionary[openFile.FilterIndex]; //filterIndex[openFile.FilterIndex];
					if (filterIdx.GetType() == typeof(xcForceCustom)) // special case
					{
						toLoad = filterIdx.LoadFile(path, fName);
						recover = true;
					}
					else if (filterIdx.GetType() == typeof(xcCustom)) // for *.* files, try singles and then extensions
					{
						// try singles
						foreach (XCom.Interfaces.IXCImageFile ixf in _loadedTypes.AllLoaded)
							if (ixf.SingleFile != null && ixf.SingleFile.ToUpperInvariant() == fName.ToUpperInvariant())
							{
								try
								{
									toLoad = ixf.LoadFile(path, fName);
									break;
								}
								catch {} // TODO: that.
							}

						if (toLoad == null) // singles not loaded, try non singles
						{
							foreach (XCom.Interfaces.IXCImageFile ixf in _loadedTypes.AllLoaded)
								if (ixf.SingleFile == null && ixf.FileExtension.ToUpperInvariant() == ext.ToUpperInvariant())
								{
									try
									{
										toLoad = ixf.LoadFile(path, fName);
										break;
									}
									catch {} // TODO: that.
								}

							if (toLoad == null) // nothing loaded, force the custom dialog
								toLoad = xcCustom.LoadFile(path, fName, 0, 0);
						}
					}
					else
						toLoad = LoadImageCollection(filterIdx, path, fName);
#if !DEBUG
				}
				catch (Exception ex)
				{
					if (MessageBox.Show(
									this,
									"Error loading file: " + fName + "\nPath: " + openFile.FileName
										+ "\nError loading file, do you wish to try and recover?\n\nError Message: "
										+ ex + ":" + ex.Message,
									"Error loading file",
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Error) == DialogResult.Yes)
					{
						toLoad = xcCustom.LoadFile(path, fName, 0, 0);
						recover = true;
					}
				}
#endif
				if (!recover && toLoad != null)
				{
					SetImages(toLoad);
					UpdateText();
				}
			}
		}

		public void LoadPckFile(string filePath, int bpp)
		{
			_currentFilePath = filePath;
			_currentFileBpp = bpp;

			SaveMenuItem.Visible = true;
			var filterIdx = openDictionary[7];

			var file = Path.GetFileName(filePath);
			var path = Path.GetDirectoryName(filePath);

			if (file != null)
			{
				var images = LoadImageCollection(filterIdx, path, file.ToLower());
				SetImages(images);
				UpdateText();

				MapViewIntegrationMenuItem.Visible = true;
				MapViewIntegrationHelpPanel.Visible |= (Settings.Default.MapViewIntegrationHelpShown < 2);
			}
		}

		private static XCImageCollection LoadImageCollection(
				IXCImageFile filterIdx,
				string path,
				string fName)
		{
			return filterIdx.LoadFile( // load file based on its filterIndex
									path,
									fName,
									filterIdx.ImageSize.Width,
									filterIdx.ImageSize.Height);
		}

		public void SetImages(XCImageCollection toLoad)
		{
			palClick(((MenuItem)_dictPalettes[toLoad.IXCFile.DefaultPalette]), null);
			_totalViewPck.Collection = toLoad;
		}

		public TotalViewPck MainPanel
		{
			get { return _totalViewPck; }
		}

		private void saveAs_Click(object sender, System.EventArgs e)
		{
			var saveFile = new SaveFileDialog();

			osFilter.SetFilter(IXCImageFile.Filter.Save);
			saveDictionary.Clear();
			saveFile.Filter = _loadedTypes.CreateFilter(osFilter, saveDictionary);

			if (saveFile.ShowDialog() == DialogResult.OK)
			{
				string dir = saveFile.FileName.Substring(0, saveFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal));
				saveDictionary[saveFile.FilterIndex].SaveCollection(
																dir,
																Path.GetFileNameWithoutExtension(saveFile.FileName),
																_totalViewPck.Collection);
			}
		}

		private void viewClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count == 0) return;
			var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
			if (_totalViewPck.Collection != null)
			{
				if (_totalViewPck.Collection.IXCFile.SingleFile != null)
				{
					string file = _totalViewPck.Collection.Name.Substring(0, _totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal));
					string ext  = _totalViewPck.Collection.Name.Substring(_totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal) + 1);
					saveBmpSingle.FileName = file + selected.Image.FileId;
				}
				else
					saveBmpSingle.FileName = _totalViewPck.Collection.Name + selected.Image.FileId;

				if (saveBmpSingle.ShowDialog() == DialogResult.OK)
					Bmp.Save(saveBmpSingle.FileName, selected.Image.Image);
			}
		}

		private void replaceClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 1)
			{
				MessageBox.Show(
							"Must select 1 item only",
							Text,
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation);
			}
			else if (_totalViewPck.Collection != null )
			{
				var title = string.Empty ;
				foreach (var selectedIndex in _totalViewPck.SelectedItems)
				{
					if (title != string.Empty) title += ", ";
					title += selectedIndex.Item.Index ;
				}

				openBMP.Title = "Selected number: " + title;
				openBMP.Multiselect = false;
				if (openBMP.ShowDialog() == DialogResult.OK)
				{
					var b = new Bitmap(openBMP.FileName);
					var image = Bmp.Load(
										b,
										_totalViewPck.Pal,
										_totalViewPck.Collection.IXCFile.ImageSize.Width,
										_totalViewPck.Collection.IXCFile.ImageSize.Height,
										1)[0];
					_totalViewPck.ChangeItem(_totalViewPck.SelectedItems[0].Item.Index, image);
					Refresh();
				}
				UpdateText();
			}
		}

		private void UpdateText()
		{
			Text = _totalViewPck.Collection.Name + ":" + _totalViewPck.Collection.Count;
		}

		private void removeClick(object sender, EventArgs e)
		{
			_totalViewPck.RemoveSelected();
			UpdateText();
			Refresh();
		}

		private void showBytes_Click(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 0)
			{
				showBytes.Checked = true;

				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				BytesFormHelper.ShowBytes(selected, bClosing, new Point(this.Right, this.Top));
			}
		}

		private void bClosing()
		{
			showBytes.Checked = false;
		}

		private void transOn_Click(object sender, System.EventArgs e)
		{
			transOn.Checked = !transOn.Checked;

			_palette.SetTransparent(transOn.Checked);
			_totalViewPck.Collection.Pal = _palette;
			Refresh();
		}

		private void aboutItem_Click(object sender, System.EventArgs e)
		{
			new About().ShowDialog(this);
		}

		private void helpItem_Click(object sender, System.EventArgs e)
		{
			new HelpForm().ShowDialog(this);
		}

		private void editClick(object sender, EventArgs e)
		{
			if (_totalViewPck.SelectedItems.Count != 0)
			{
				var selected = _totalViewPck.SelectedItems[_totalViewPck.SelectedItems.Count - 1];
				if (selected != null)
				{
					_editor.CurrImage = (XCImage)selected.Image.Clone();

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

		private void editorClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			_editor.Hide();
		}

		private void miHq2x_Click(object sender, System.EventArgs e)
		{
			miPalette.Enabled = false;
			bytesMenu.Enabled = false;

			_totalViewPck.Hq2x();

			OnResize(null);
			Refresh();
		}

		private void PckView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && deleteImage.Enabled)
				removeClick(null, null);
		}

		private void miModList_Click(object sender, EventArgs e)
		{
			var f = new ModForm();
			f.SharedSpace = _share;
			f.ShowDialog();
		}

		private void miSaveDir_Click(object sender, EventArgs e)
		{
			if (_totalViewPck.Collection != null)
			{
				string fileStart = String.Empty;
				string extStart  = String.Empty;

				if (_totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal) > 0)
				{
					fileStart = _totalViewPck.Collection.Name.Substring(0, _totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal));
					extStart  = _totalViewPck.Collection.Name.Substring(_totalViewPck.Collection.Name.IndexOf(".", StringComparison.Ordinal) + 1);
				}

				saveBmpSingle.FileName = fileStart;

				saveBmpSingle.Title = "Select directory to save images in";

				if (saveBmpSingle.ShowDialog() == DialogResult.OK)
				{
					string path = saveBmpSingle.FileName.Substring(0, saveBmpSingle.FileName.LastIndexOf(@"\", StringComparison.Ordinal));
					string file = saveBmpSingle.FileName.Substring(saveBmpSingle.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
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

		private void miConsole_Click(object sender, EventArgs e)
		{
			if (console.Visible)
				console.BringToFront();
			else
				console.Show();
		}

		private void miCompare_Click(object sender, EventArgs e)
		{
			var original = _totalViewPck.Collection;

			openItem_Click(null, null);

			var newCollection = _totalViewPck.Collection;

			_totalViewPck.Collection = original;

			if (Controls.Contains(_totalViewPck))
			{
				Controls.Remove(_totalViewPck);

				tabs = new TabControl();
				tabs.Dock = DockStyle.Fill;
				DrawPanel.Controls.Add(tabs);

				var tp = new TabPage();
				tp.Controls.Add(_totalViewPck);
				tp.Text = "Original";
				tabs.TabPages.Add(tp);

				tp = new TabPage();
				var tvNew = new TotalViewPck();
				tvNew.ContextMenu = makeContextMenu();
				tvNew.Dock = DockStyle.Fill;
				tvNew.Collection = newCollection;
				tp.Controls.Add(tvNew);
				tp.Text = "New";
				tabs.TabPages.Add(tp);
			}
		}

		private void SaveMenuItem_Click(object sender, EventArgs e)
		{
			osFilter.SetFilter(IXCImageFile.Filter.Save);
			saveDictionary.Clear();
			_loadedTypes.CreateFilter(osFilter, saveDictionary);
			var dir = Path.GetDirectoryName(_currentFilePath);
			var fileWithoutExt = Path.GetFileNameWithoutExtension(_currentFilePath);

			// Backup
			_fileBackupManager.Backup(_currentFilePath);

			// Save
			PckFile.Save(
						dir,
						fileWithoutExt,
						_totalViewPck.Collection,
						_currentFileBpp);
			SavedFile = true;
		}

		private void MapViewIntegrationMenuItem_Click(object sender, EventArgs e)
		{
			MapViewIntegrationHelpPanel.Visible = !MapViewIntegrationHelpPanel.Visible;
			MapViewIntegrationMenuItem.Checked = MapViewIntegrationHelpPanel.Visible;
		}

		private void GotItMapViewButton_Click(object sender, EventArgs e)
		{
			MapViewIntegrationHelpPanel.Visible = false;
			MapViewIntegrationMenuItem.Checked = false;
			Settings.Default.MapViewIntegrationHelpShown++;
			Settings.Default.Save();
		}

		private void PckViewForm_Shown(object sender, EventArgs e)
		{
			console.Show();
		}
	}
}
