using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

//using Microsoft.Win32;

using XCom;
using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// The PathsEditor.
	/// </summary>
	internal sealed class PathsEditor
		:
			Form
	{
/*		private static bool _saveRegistry = true;
		internal static bool SaveRegistry
		{
			get { return _saveRegistry; }
			set { _saveRegistry = value; }
		} */

		private string _paths;
		private string[] _images;


		internal PathsEditor(string pathsPath)
		{
			_paths = pathsPath;

			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			tbPathsMaps.Text    = GameInfo.TilesetInfo.Path;
			tbPathsImages.Text  = GameInfo.ImageInfo.Path;
			tbImagesImages.Text = GameInfo.ImageInfo.Path;

//			txtCursor.Text   = GameInfo.CursorPath;
//			txtCursor.Text   = GameInfo.MiscInfo.CursorFile;
//			txtPalettes.Text = GameInfo.PalettePath;

			PopulateImageList();

			populateTree();

			cbMapsPalette.Items.Add(Palette.UfoBattle);
			cbMapsPalette.Items.Add(Palette.TftdBattle);
		}


		private void populateTree()
		{
			tvMaps.Nodes.Clear();

			var list = new ArrayList();
			foreach (object ob in GameInfo.TilesetInfo.Tilesets.Keys)
				list.Add(ob);

			list.Sort();

			var list1 = new ArrayList();
			foreach (string ob in list) // tileset
			{
				var it = GameInfo.TilesetInfo.Tilesets[ob];
				if (it != null)
				{
					var tn = tvMaps.Nodes.Add(ob); // make the node for the tileset

					list1.Clear();

					foreach (string ob1 in it.Subsets.Keys) // make a node for each subset
						list1.Add(ob1);

					list1.Sort();

					foreach (string ob1 in list1)
					{
						var subset = it.Subsets[ob1];
						if (subset != null)
						{
							var tn1 = tn.Nodes.Add(ob1);

							var keys = new ArrayList();
							foreach (string key in subset.Keys)
								keys.Add(key);

							keys.Sort();

							foreach (string key in keys)
								if (subset[key] != null)
									tn1.Nodes.Add(key);
						}
					}
				}
			}
		}

		private void PopulateImageList()
		{
			var list = new ArrayList();

			var info = GameInfo.ImageInfo;
			foreach (object ob in info.Images.Keys)
				list.Add(ob);

			list.Sort();

			foreach (object ob in list)
			{
				lbImages.Items.Add(ob);
				lbMapsImagesAll.Items.Add(ob);
			}
		}

		private void lbImages_SelectedIndexChanged(object sender, EventArgs e)
		{
			tbImagesTerrain.Text = GameInfo.ImageInfo[(string)lbImages.SelectedItem].Path;
		}

		private void tvMaps_AfterSelect(object sender, TreeViewEventArgs e)
		{
			gbMapsTerrain.Enabled = true;
			gbMapsBlock.Enabled   =
			delMap.Enabled        = false;

			Tileset tileset = null;

			var node = e.Node;
			if (node.Parent != null)
			{
				addMap.Enabled =
				delSub.Enabled = true;

				if (node.Parent.Parent != null) // inner node
				{
					gbMapsTerrain.Enabled = false;
					gbMapsBlock.Enabled   =
					delMap.Enabled        = true;

					tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[node.Parent.Parent.Text];

					var desc = (XCMapDesc)tileset[node.Text];

					lbMapsImagesUsed.Items.Clear();

					if (desc != null)
					{
						foreach (string dep in desc.Dependencies)
							lbMapsImagesUsed.Items.Add(dep);
					}
					else
					{
						tileset.AddMap(
									new XCMapDesc(
												node.Text,
												tileset.MapPath,
												tileset.RoutePath,
												tileset.OccultPath,
												new string[]{},
												tileset.Palette),
									node.Parent.Text);
					}
				}
				else // subset node
				{
					tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[node.Parent.Text];
				}
			}
			else // parent node
			{
				tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[node.Text];
				addMap.Enabled =
				delMap.Enabled =
				delSub.Enabled = false;
			}

			tbMapsMaps.Text    = tileset.MapPath;
			tbMapsRoutes.Text  = tileset.RoutePath;
			tbMapsOccults.Text = tileset.OccultPath;

			cbMapsPalette.SelectedItem = tileset.Palette;
		}

		private void tbMapsMaps_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Enter, System.Globalization.CultureInfo.InvariantCulture))
				tbMapsMaps_Leave(null, null);
		}

		private void tbMapsMaps_Leave(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			if (!Directory.Exists(tbMapsMaps.Text))
			{
				using (var output = new OutputBox("Directory not found: " + tbMapsMaps.Text))
				{
					// TODO: Directory.CreateDirectory(dir);
					output.ShowDialog();
					tbMapsMaps.Text = tileset.MapPath;
				}
			}
			tileset.MapPath = tbMapsMaps.Text;
		}

		private void tbMapsRoutes_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Enter, System.Globalization.CultureInfo.InvariantCulture))
				tbMapsRoutes_Leave(null, null);
		}

		private void tbMapsRoutes_Leave(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			if (!Directory.Exists(tbMapsRoutes.Text))
			{
				using (var output = new OutputBox("Directory not found: " + tbMapsRoutes.Text))
				{
					// TODO: Directory.CreateDirectory(dir);
					output.ShowDialog();
					tbMapsRoutes.Text = tileset.RoutePath;
				}
			}
			tileset.RoutePath = tbMapsRoutes.Text;
		}

		private void btnMapsUp_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			string[] deps = ((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies;

			for (int i = 1; i != deps.Length; ++i)
			{
				if (deps[i] == (string)lbMapsImagesUsed.SelectedItem)
				{
					string old = deps[i - 1];
					deps[i - 1] = deps[i];
					deps[i] = old;

					((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies = deps;

					lbMapsImagesUsed.Items.Clear();

					foreach (string dep in deps)
						lbMapsImagesUsed.Items.Add(dep);

					lbMapsImagesUsed.SelectedItem = deps[i - 1];
					return;
				}
			}
		}

		private void btnMapsDown_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			string[] deps = ((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies;

			for (int i = 0; i != deps.Length - 1; ++i)
			{
				if (deps[i] == (string)lbMapsImagesUsed.SelectedItem)
				{
					string old = deps[i + 1];
					deps[i + 1] = deps[i];
					deps[i] = old;

					((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies = deps;
					
					lbMapsImagesUsed.Items.Clear();

					foreach (string dep in deps)
						lbMapsImagesUsed.Items.Add(dep);

					lbMapsImagesUsed.SelectedItem = deps[i + 1];
					return;
				}
			}
		}

		private void btnMapsRight_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			var deps = new ArrayList(((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies);
			deps.Remove(lbMapsImagesUsed.SelectedItem);
			((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies = (string[])deps.ToArray(typeof(string));
			
			lbMapsImagesUsed.Items.Clear();

			foreach (string dep in deps)
				lbMapsImagesUsed.Items.Add(dep);
		}

		private void btnMapsLeft_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			var dep = new ArrayList(((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies);

			foreach (object ob in lbMapsImagesAll.SelectedItems)
				if (!dep.Contains(ob))
				{
					dep.Add(ob);
					((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies = (string[])dep.ToArray(typeof(string));
				
					lbMapsImagesUsed.Items.Clear();

					foreach (string st in dep)
						lbMapsImagesUsed.Items.Add(st);
				}
		}

		private void btnPathsClearRegistry_Click(object sender, EventArgs e) // NOTE: disabled w/ Visible=FALSE in the designer.
		{
/*			_saveRegistry = false;

			using (var keySoftware = Registry.CurrentUser.OpenSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry, true))
			{
				keySoftware.DeleteSubKeyTree(DSShared.Windows.RegistryInfo.MapViewRegistry);
				keySoftware.Close();
			} */
		}

		private void btnPathsMaps_Click(object sender, EventArgs e)
		{
			using (var f = ofdFileDialog)
			{
				f.Title = "Find MapEdit.cfg";
				f.Multiselect = false;
				f.FilterIndex = 1; // *.cfg

				if (f.ShowDialog() == DialogResult.OK)
					tbPathsMaps.Text = f.FileName;
			}
		}

		private void btnPathsImages_Click(object sender, EventArgs e)
		{
			using (var f = ofdFileDialog)
			{
				f.Title = "Find Images.cfg";
				f.Multiselect = false;
				f.FilterIndex = 1; // *.cfg

				if (f.ShowDialog() == DialogResult.OK)
					tbPathsImages.Text = f.FileName;
			}
		}

/*		private void miSave_Click(object sender, EventArgs e)
		{
			StreamWriter sw = new StreamWriter(new FileStream(this._paths, FileMode.Create));
			
			GameInfo.CursorPath  = txtCursor.Text;
			GameInfo.MapPath     = txtMap.Text;
			GameInfo.ImagePath   = txtImages.Text;
//			GameInfo.PalettePath = txtPalettes.Text;

			sw.WriteLine("mapdata:"  + txtMap.Text);
			sw.WriteLine("images:"   + txtImages.Text);
			sw.WriteLine("cursor:"   + txtCursor.Text);
//			sw.WriteLine("palettes:" + txtPalettes.Text);

			sw.Flush();
			sw.Close();

//			GameInfo.GetImageInfo().Save(new FileStream(txtImages.Text, FileMode.Create));
//			GameInfo.GetTileInfo().Save(new FileStream(txtMap.Text, FileMode.Create));
		}*/

		private void tbImagesTerrain_TextChanged(object sender, EventArgs e)
		{
			GameInfo.ImageInfo[(string)lbImages.SelectedItem].Path = tbImagesTerrain.Text;
		}

		private void cmImagesAddImageset_Click(object sender, EventArgs e)
		{
			using (var f = ofdFileDialog)
			{
				f.Title = "Add images";
				f.Multiselect = true;
				f.FilterIndex = 2; // *.map

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					foreach (string pfe in f.FileNames) // pfe=PathFileExt
					{
						string path = pfe.Substring(0, pfe.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						string file = pfe.Substring(pfe.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						file        = file.Substring(0, file.IndexOf(".", StringComparison.Ordinal));
	
						GameInfo.ImageInfo[file] = new ImageDescriptor(file, path);
					}
	
					lbImages.Items.Clear();
					lbMapsImagesAll.Items.Clear();
	
					PopulateImageList();
				}
			}
		}

		private void cmImagesDeleteImageset_Click(object sender, EventArgs e)
		{
			GameInfo.ImageInfo.Images.Remove(lbImages.SelectedItem.ToString());

			lbImages.Items.Clear();
			lbMapsImagesAll.Items.Clear();

			PopulateImageList();
		}

		private void btnPathsInstall_Click(object sender, EventArgs e)
		{
			using (var f = new InstallationForm())
				f.ShowDialog(this);
		}

		private void btnPathsSave_Click(object sender, EventArgs e)
		{
			using (var sw = new StreamWriter(new FileStream(_paths, FileMode.Create)))
			{
//				GameInfo.MapPath     = txtMap.Text;
//				GameInfo.ImagePath   = txtImages.Text;
//				GameInfo.CursorPath  = txtCursor.Text;
//				GameInfo.PalettePath = txtPalettes.Text;

//				sw.WriteLine("palettes:" + txtPalettes.Text);
	
				sw.WriteLine("mapdata:" + tbPathsMaps.Text);
				sw.WriteLine("images:"  + tbPathsImages.Text);
				sw.WriteLine("cursor:"  + tbPathsCursor.Text);
			}
		}

		private void btnImagesSave_Click(object sender, EventArgs e)
		{
			GameInfo.ImageInfo.Save(tbPathsImages.Text);
		}

		private void newGroup_Click(object sender, EventArgs e)
		{
			using (var f = new TilesetForm())
			{
				f.ShowDialog(this);

				if (f.TilesetLabel != null)
				{
					var tileset = (Tileset)GameInfo.TilesetInfo.AddTileset(
																		f.TilesetLabel,
																		f.MapsPath,
																		f.RoutesPath,
																		f.OccultsPath);
//					addTileset(tileset.Name);
					tvMaps.Nodes.Add(tileset.Name);

					tbMapsMaps.Text   = tileset.MapPath;
					tbMapsRoutes.Text = tileset.RoutePath;

//					saveMapedit();
				}
			}
		}

		private void cbMapsPalette_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tvMaps.SelectedNode.Parent == null)
				getCurrentTileset().Palette = cbMapsPalette.SelectedItem as Palette;
		}

		private void OnSubsetClick(object sender, EventArgs e)
		{
			using (var f = new SubsetForm())
			{
				f.ShowDialog(this);
				if (!String.IsNullOrEmpty(f.SubsetLabel))
				{
					var tileset = getCurrentTileset();

//					TreeNode tn = treeMaps.SelectedNode; // TODO: Check if not used.

					tileset.Subsets[f.SubsetLabel] = new Dictionary<string, MapDesc>();

//					tileset.NewSubset(f.SubsetName);
//					saveMapedit();

					populateTree();
				}
			}
		}

		private Tileset getCurrentTileset()
		{
			var tn = tvMaps.SelectedNode;
			if (tn.Parent == null)
				return (Tileset)GameInfo.TilesetInfo.Tilesets[tn.Text];

			if (tn.Parent.Parent == null)
				return (Tileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Text];

			return (Tileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Parent.Text];
		}

//		private void btnSave2_Click(object sender, System.EventArgs e)
//		{
//			saveMapedit();
//		}

		private void addNewMap_Click(object sender, EventArgs e)
		{
			using (var f = new NewMapForm())
			{
				f.ShowDialog(this);

				if (f.MapLabel != null)
				{
					if (tvMaps.SelectedNode.Parent != null) // add to here
					{
						string pfe = tbMapsMaps.Text + f.MapLabel + XCMapFile.MapExt;
						if (File.Exists(pfe))
						{
							using (var fdialog = new ChoiceDialog(pfe))
							{
								fdialog.ShowDialog(this);
	
								if (fdialog.Choice == Choice.UseExisting)
									return;
							}
						}

						XCMapFile.CreateMap(
										File.OpenWrite(pfe),
										f.MapRows,
										f.MapCols,
										f.MapHeight);

						using (var fs = File.OpenWrite(tbMapsRoutes.Text + f.MapLabel + RouteNodeCollection.RouteExt)) // TODO: wtf.
						{}

						Tileset tileset;
						string label;

						if (tvMaps.SelectedNode.Parent.Parent == null)
						{
							tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[tvMaps.SelectedNode.Parent.Text];
							tvMaps.SelectedNode.Nodes.Add(f.MapLabel);
							label = tvMaps.SelectedNode.Text;
						}
						else
						{
							tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[tvMaps.SelectedNode.Parent.Parent.Text];
							tvMaps.SelectedNode.Parent.Nodes.Add(f.MapLabel);
							label = tvMaps.SelectedNode.Parent.Text;
						}

						tileset.AddMap(f.MapLabel, label);

//						saveMapedit();
					}
//					else // top node, baaaaad
//					{
//						tileset = GameInfo.GetTileInfo()[treeMaps.SelectedNode.Parent.Text];
//						treeMaps.SelectedNode.Parent.Nodes.Add(nf.MapName);
//					}
				}
			}
		}

		private void addExistingMap_Click(object sender, EventArgs e)
		{
			using (var f = ofdFileDialog)
			{
				f.InitialDirectory = tbMapsMaps.Text;
				f.Title = "Select maps from this directory only";
				f.Multiselect      =
				f.RestoreDirectory = true;

				if (f.ShowDialog() == DialogResult.OK)
				{
					if (tvMaps.SelectedNode.Parent != null) // add to here
					{
						var tn = (tvMaps.SelectedNode.Parent.Parent == null) ? tvMaps.SelectedNode
																			 : tvMaps.SelectedNode.Parent;

						var tileset = (Tileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Text];
						foreach (string file in f.FileNames)
						{
							int start = file.LastIndexOf(@"\", StringComparison.Ordinal) + 1;
							int end   = file.LastIndexOf(".", StringComparison.Ordinal);

							string name = file.Substring(start, end-start);
							try
							{
								tileset.AddMap(name, tn.Text);
								tn.Nodes.Add(name);
							}
							catch (Exception ex)
							{
								MessageBox.Show(
											this,
											"Could not add map: " + name + Environment.NewLine + "ERROR: " + ex.Message,
											"Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Exclamation,
											MessageBoxDefaultButton.Button1,
											0);
								throw;
							}
						}
//						saveMapedit();
					}
//					else // top node, baaaaad
//					{
//						tileset = GameInfo.GetTileInfo()[treeMaps.SelectedNode.Parent.Text];
//						treeMaps.SelectedNode.Parent.Nodes.Add(nfm.MapName);
//					}
				}
			}
		}

		private void delGroup_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();
			GameInfo.TilesetInfo.Tilesets[tileset.Name] = null;
			if (tvMaps.SelectedNode.Parent == null)
			{
				tvMaps.Nodes.Remove(tvMaps.SelectedNode);
			}
			else if (tvMaps.SelectedNode.Parent.Parent == null)
			{
				tvMaps.Nodes.Remove(tvMaps.SelectedNode.Parent);
			}
			else
				tvMaps.Nodes.Remove(tvMaps.SelectedNode.Parent.Parent);

//			saveMapedit();
		}

		private void delSub_Click(object sender, EventArgs e)
		{
			var tn = tvMaps.SelectedNode.Parent;
			if (tn != null)
			{
				if (tvMaps.SelectedNode.Parent.Parent == null)
					tn = tvMaps.SelectedNode;

				if (tn != null)
				{
					var tileset = getCurrentTileset();
					tileset.Subsets[tn.Text] = null;
					tn.Parent.Nodes.Remove(tn);
				}
			}
		}

		private void delMap_Click(object sender, EventArgs e)
		{
			if (   tvMaps.SelectedNode.Parent != null
				&& tvMaps.SelectedNode.Parent.Parent != null)
			{
				TreeNode tn = tvMaps.SelectedNode;
				if (tn != null)
				{
					var tileset = getCurrentTileset();
					tileset.Subsets[tn.Parent.Text][tn.Text] = null;
					tileset[tn.Text] = null;
					tn.Parent.Nodes.Remove(tn);
				}
			}
		}

		private void btnMapsEditTree_Click(object sender, EventArgs e)
		{
//			TreeEditor mmf = new TreeEditor();
//			mmf.ShowDialog(this);
//			populateTree();

//			saveMapedit();
		}

		private void btnMapsSaveTree_Click(object sender, EventArgs e)
		{
/*			var cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

//			string path	= txtMap.Text.Substring(0, txtMap.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
//			string file	= txtMap.Text.Substring(txtMap.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
//			string ext	= file.Substring(file.LastIndexOf(".", StringComparison.Ordinal));
//			file		= file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));

			string path_file = txtMap.Text.Substring(0, txtMap.Text.LastIndexOf(".", StringComparison.Ordinal) + 1); // includes "."
			string fileRecent = path_file + "new";

			try
			{
				string fileBackup = path_file + "old";

				GameInfo.TilesetInfo.Save(fileRecent);

				if (File.Exists(txtMap.Text))
				{
					if (File.Exists(fileBackup))
						File.Delete(fileBackup);

					File.Move(txtMap.Text, fileBackup);
				}

				File.Move(fileRecent, txtMap.Text);

				Cursor.Current = cursor;
				Text = "Paths Editor - saved MapEdit.cfg";
			}
			catch (Exception except)
			{
				if (File.Exists(fileRecent))
					File.Delete(fileRecent);

				throw except;
			} */
		}

		private void closeItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnMapsCopy_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();

			int length = ((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies.Length;
			_images = new string[length];

			for (int i = 0; i != length; ++i)
				_images[i] = ((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies[i];
		}

		private void btnMapsPaste_Click(object sender, EventArgs e)
		{
			var tileset = getCurrentTileset();

			((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies = new string[_images.Length];

			lbMapsImagesUsed.Items.Clear();

			for (int i = 0; i != _images.Length; ++i)
			{
				((XCMapDesc)tileset[tvMaps.SelectedNode.Text]).Dependencies[i] = _images[i];
				lbMapsImagesUsed.Items.Add(_images[i]);
			}
		}


		#region Windows Form Designer generated code

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mmMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miFile = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.closeItem = new System.Windows.Forms.MenuItem();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabPaths = new System.Windows.Forms.TabPage();
			this.lblPathsInstall = new System.Windows.Forms.Label();
			this.lblPathsClearRegistry = new System.Windows.Forms.Label();
			this.btnPathsSave = new System.Windows.Forms.Button();
			this.btnPathsInstall = new System.Windows.Forms.Button();
			this.btnPathsImages = new System.Windows.Forms.Button();
			this.btnPathsMaps = new System.Windows.Forms.Button();
			this.tbPathsPalettes = new System.Windows.Forms.TextBox();
			this.lblPathsPalettes = new System.Windows.Forms.Label();
			this.btnPathsClearRegistry = new System.Windows.Forms.Button();
			this.lblPathsSave = new System.Windows.Forms.Label();
			this.tbPathsCursor = new System.Windows.Forms.TextBox();
			this.tbPathsImages = new System.Windows.Forms.TextBox();
			this.tbPathsMaps = new System.Windows.Forms.TextBox();
			this.lblPathsCursor = new System.Windows.Forms.Label();
			this.lblPathsImages = new System.Windows.Forms.Label();
			this.lblPathsMaps = new System.Windows.Forms.Label();
			this.tabMaps = new System.Windows.Forms.TabPage();
			this.gbMapsBlock = new System.Windows.Forms.GroupBox();
			this.btnMapsPaste = new System.Windows.Forms.Button();
			this.btnMapsCopy = new System.Windows.Forms.Button();
			this.btnMapsUp = new System.Windows.Forms.Button();
			this.btnMapsDown = new System.Windows.Forms.Button();
			this.btnMapsRight = new System.Windows.Forms.Button();
			this.btnMapsLeft = new System.Windows.Forms.Button();
			this.lbMapsImagesAll = new System.Windows.Forms.ListBox();
			this.lblMapsImagesAll = new System.Windows.Forms.Label();
			this.lbMapsImagesUsed = new System.Windows.Forms.ListBox();
			this.lblMapsImagesUsed = new System.Windows.Forms.Label();
			this.gbMapsTerrain = new System.Windows.Forms.GroupBox();
			this.lblMapsOccults = new System.Windows.Forms.Label();
			this.tbMapsOccults = new System.Windows.Forms.TextBox();
			this.cbMapsPalette = new System.Windows.Forms.ComboBox();
			this.lblMapsPalette = new System.Windows.Forms.Label();
			this.lblMapsRoutes = new System.Windows.Forms.Label();
			this.lblMapsMaps = new System.Windows.Forms.Label();
			this.tbMapsRoutes = new System.Windows.Forms.TextBox();
			this.tbMapsMaps = new System.Windows.Forms.TextBox();
			this.tvMaps = new System.Windows.Forms.TreeView();
			this.cmMain = new System.Windows.Forms.ContextMenu();
			this.newGroup = new System.Windows.Forms.MenuItem();
			this.delGroup = new System.Windows.Forms.MenuItem();
			this.miAddSubset = new System.Windows.Forms.MenuItem();
			this.delSub = new System.Windows.Forms.MenuItem();
			this.addMap = new System.Windows.Forms.MenuItem();
			this.addNewMap = new System.Windows.Forms.MenuItem();
			this.addExistingMap = new System.Windows.Forms.MenuItem();
			this.delMap = new System.Windows.Forms.MenuItem();
			this.btnMapsSaveTree = new System.Windows.Forms.Button();
			this.btnMapsEditTree = new System.Windows.Forms.Button();
			this.tabImages = new System.Windows.Forms.TabPage();
			this.lblImagesImages = new System.Windows.Forms.Label();
			this.tbImagesImages = new System.Windows.Forms.TextBox();
			this.btnImagesSave = new System.Windows.Forms.Button();
			this.lblImagesTerrain = new System.Windows.Forms.Label();
			this.tbImagesTerrain = new System.Windows.Forms.TextBox();
			this.lbImages = new System.Windows.Forms.ListBox();
			this.cmImages = new System.Windows.Forms.ContextMenu();
			this.addImageset = new System.Windows.Forms.MenuItem();
			this.delImageset = new System.Windows.Forms.MenuItem();
			this.ofdFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.tabs.SuspendLayout();
			this.tabPaths.SuspendLayout();
			this.tabMaps.SuspendLayout();
			this.gbMapsBlock.SuspendLayout();
			this.gbMapsTerrain.SuspendLayout();
			this.tabImages.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMenu
			// 
			this.mmMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miFile});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuItem1,
			this.closeItem});
			this.miFile.Text = "File";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Import map settings";
			// 
			// closeItem
			// 
			this.closeItem.Index = 1;
			this.closeItem.Text = "Close";
			this.closeItem.Click += new System.EventHandler(this.closeItem_Click);
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabPaths);
			this.tabs.Controls.Add(this.tabMaps);
			this.tabs.Controls.Add(this.tabImages);
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(632, 534);
			this.tabs.TabIndex = 0;
			// 
			// tabPaths
			// 
			this.tabPaths.Controls.Add(this.lblPathsInstall);
			this.tabPaths.Controls.Add(this.lblPathsClearRegistry);
			this.tabPaths.Controls.Add(this.btnPathsSave);
			this.tabPaths.Controls.Add(this.btnPathsInstall);
			this.tabPaths.Controls.Add(this.btnPathsImages);
			this.tabPaths.Controls.Add(this.btnPathsMaps);
			this.tabPaths.Controls.Add(this.tbPathsPalettes);
			this.tabPaths.Controls.Add(this.lblPathsPalettes);
			this.tabPaths.Controls.Add(this.btnPathsClearRegistry);
			this.tabPaths.Controls.Add(this.lblPathsSave);
			this.tabPaths.Controls.Add(this.tbPathsCursor);
			this.tabPaths.Controls.Add(this.tbPathsImages);
			this.tabPaths.Controls.Add(this.tbPathsMaps);
			this.tabPaths.Controls.Add(this.lblPathsCursor);
			this.tabPaths.Controls.Add(this.lblPathsImages);
			this.tabPaths.Controls.Add(this.lblPathsMaps);
			this.tabPaths.Location = new System.Drawing.Point(4, 21);
			this.tabPaths.Name = "tabPaths";
			this.tabPaths.Size = new System.Drawing.Size(624, 509);
			this.tabPaths.TabIndex = 0;
			this.tabPaths.Text = "Paths";
			// 
			// lblPathsInstall
			// 
			this.lblPathsInstall.Location = new System.Drawing.Point(145, 215);
			this.lblPathsInstall.Name = "lblPathsInstall";
			this.lblPathsInstall.Size = new System.Drawing.Size(120, 15);
			this.lblPathsInstall.TabIndex = 16;
			this.lblPathsInstall.Text = "Re-run the installer.";
			// 
			// lblPathsClearRegistry
			// 
			this.lblPathsClearRegistry.Location = new System.Drawing.Point(145, 165);
			this.lblPathsClearRegistry.Name = "lblPathsClearRegistry";
			this.lblPathsClearRegistry.Size = new System.Drawing.Size(300, 25);
			this.lblPathsClearRegistry.TabIndex = 15;
			this.lblPathsClearRegistry.Text = "Clear MapView from the Windows Registry and switch the option to save window posi" +
	"tions and sizes off.";
			this.lblPathsClearRegistry.Visible = false;
			// 
			// btnPathsSave
			// 
			this.btnPathsSave.Location = new System.Drawing.Point(10, 115);
			this.btnPathsSave.Name = "btnPathsSave";
			this.btnPathsSave.Size = new System.Drawing.Size(125, 35);
			this.btnPathsSave.TabIndex = 14;
			this.btnPathsSave.Text = "Save paths";
			this.btnPathsSave.Click += new System.EventHandler(this.btnPathsSave_Click);
			// 
			// btnPathsInstall
			// 
			this.btnPathsInstall.Location = new System.Drawing.Point(10, 205);
			this.btnPathsInstall.Name = "btnPathsInstall";
			this.btnPathsInstall.Size = new System.Drawing.Size(125, 35);
			this.btnPathsInstall.TabIndex = 13;
			this.btnPathsInstall.Text = "Run installer";
			this.btnPathsInstall.Click += new System.EventHandler(this.btnPathsInstall_Click);
			// 
			// btnPathsImages
			// 
			this.btnPathsImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPathsImages.Location = new System.Drawing.Point(495, 35);
			this.btnPathsImages.Name = "btnPathsImages";
			this.btnPathsImages.Size = new System.Drawing.Size(60, 20);
			this.btnPathsImages.TabIndex = 11;
			this.btnPathsImages.Text = "...";
			this.btnPathsImages.Click += new System.EventHandler(this.btnPathsImages_Click);
			// 
			// btnPathsMaps
			// 
			this.btnPathsMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPathsMaps.Location = new System.Drawing.Point(495, 10);
			this.btnPathsMaps.Name = "btnPathsMaps";
			this.btnPathsMaps.Size = new System.Drawing.Size(60, 20);
			this.btnPathsMaps.TabIndex = 10;
			this.btnPathsMaps.Text = "...";
			this.btnPathsMaps.Click += new System.EventHandler(this.btnPathsMaps_Click);
			// 
			// tbPathsPalettes
			// 
			this.tbPathsPalettes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPathsPalettes.Location = new System.Drawing.Point(70, 85);
			this.tbPathsPalettes.Name = "tbPathsPalettes";
			this.tbPathsPalettes.Size = new System.Drawing.Size(420, 19);
			this.tbPathsPalettes.TabIndex = 9;
			this.tbPathsPalettes.Visible = false;
			// 
			// lblPathsPalettes
			// 
			this.lblPathsPalettes.Location = new System.Drawing.Point(5, 90);
			this.lblPathsPalettes.Name = "lblPathsPalettes";
			this.lblPathsPalettes.Size = new System.Drawing.Size(60, 15);
			this.lblPathsPalettes.TabIndex = 8;
			this.lblPathsPalettes.Text = "Palettes";
			this.lblPathsPalettes.Visible = false;
			// 
			// btnPathsClearRegistry
			// 
			this.btnPathsClearRegistry.Location = new System.Drawing.Point(10, 160);
			this.btnPathsClearRegistry.Name = "btnPathsClearRegistry";
			this.btnPathsClearRegistry.Size = new System.Drawing.Size(125, 35);
			this.btnPathsClearRegistry.TabIndex = 7;
			this.btnPathsClearRegistry.Text = "Clear Registry Settings";
			this.btnPathsClearRegistry.Visible = false;
			this.btnPathsClearRegistry.Click += new System.EventHandler(this.btnPathsClearRegistry_Click);
			// 
			// lblPathsSave
			// 
			this.lblPathsSave.Location = new System.Drawing.Point(145, 120);
			this.lblPathsSave.Name = "lblPathsSave";
			this.lblPathsSave.Size = new System.Drawing.Size(355, 15);
			this.lblPathsSave.TabIndex = 6;
			this.lblPathsSave.Text = "Path changes will not be made until the Save button is clicked.";
			// 
			// tbPathsCursor
			// 
			this.tbPathsCursor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPathsCursor.Location = new System.Drawing.Point(70, 60);
			this.tbPathsCursor.Name = "tbPathsCursor";
			this.tbPathsCursor.Size = new System.Drawing.Size(420, 19);
			this.tbPathsCursor.TabIndex = 5;
			// 
			// tbPathsImages
			// 
			this.tbPathsImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPathsImages.Location = new System.Drawing.Point(70, 35);
			this.tbPathsImages.Name = "tbPathsImages";
			this.tbPathsImages.Size = new System.Drawing.Size(420, 19);
			this.tbPathsImages.TabIndex = 4;
			// 
			// tbPathsMaps
			// 
			this.tbPathsMaps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPathsMaps.Location = new System.Drawing.Point(70, 10);
			this.tbPathsMaps.Name = "tbPathsMaps";
			this.tbPathsMaps.Size = new System.Drawing.Size(420, 19);
			this.tbPathsMaps.TabIndex = 3;
			// 
			// lblPathsCursor
			// 
			this.lblPathsCursor.Location = new System.Drawing.Point(5, 65);
			this.lblPathsCursor.Name = "lblPathsCursor";
			this.lblPathsCursor.Size = new System.Drawing.Size(60, 15);
			this.lblPathsCursor.TabIndex = 2;
			this.lblPathsCursor.Text = "Cursor";
			// 
			// lblPathsImages
			// 
			this.lblPathsImages.Location = new System.Drawing.Point(5, 40);
			this.lblPathsImages.Name = "lblPathsImages";
			this.lblPathsImages.Size = new System.Drawing.Size(65, 15);
			this.lblPathsImages.TabIndex = 1;
			this.lblPathsImages.Text = "Images";
			// 
			// lblPathsMaps
			// 
			this.lblPathsMaps.Location = new System.Drawing.Point(5, 15);
			this.lblPathsMaps.Name = "lblPathsMaps";
			this.lblPathsMaps.Size = new System.Drawing.Size(65, 15);
			this.lblPathsMaps.TabIndex = 0;
			this.lblPathsMaps.Text = "Maps";
			// 
			// tabMaps
			// 
			this.tabMaps.Controls.Add(this.gbMapsBlock);
			this.tabMaps.Controls.Add(this.gbMapsTerrain);
			this.tabMaps.Controls.Add(this.tvMaps);
			this.tabMaps.Controls.Add(this.btnMapsSaveTree);
			this.tabMaps.Controls.Add(this.btnMapsEditTree);
			this.tabMaps.Location = new System.Drawing.Point(4, 21);
			this.tabMaps.Name = "tabMaps";
			this.tabMaps.Size = new System.Drawing.Size(624, 509);
			this.tabMaps.TabIndex = 1;
			this.tabMaps.Text = "Map Files";
			// 
			// gbMapsBlock
			// 
			this.gbMapsBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.gbMapsBlock.Controls.Add(this.btnMapsPaste);
			this.gbMapsBlock.Controls.Add(this.btnMapsCopy);
			this.gbMapsBlock.Controls.Add(this.btnMapsUp);
			this.gbMapsBlock.Controls.Add(this.btnMapsDown);
			this.gbMapsBlock.Controls.Add(this.btnMapsRight);
			this.gbMapsBlock.Controls.Add(this.btnMapsLeft);
			this.gbMapsBlock.Controls.Add(this.lbMapsImagesAll);
			this.gbMapsBlock.Controls.Add(this.lblMapsImagesAll);
			this.gbMapsBlock.Controls.Add(this.lbMapsImagesUsed);
			this.gbMapsBlock.Controls.Add(this.lblMapsImagesUsed);
			this.gbMapsBlock.Enabled = false;
			this.gbMapsBlock.Location = new System.Drawing.Point(240, 170);
			this.gbMapsBlock.Name = "gbMapsBlock";
			this.gbMapsBlock.Size = new System.Drawing.Size(385, 341);
			this.gbMapsBlock.TabIndex = 2;
			this.gbMapsBlock.TabStop = false;
			this.gbMapsBlock.Text = "MAP BLOCK";
			// 
			// btnMapsPaste
			// 
			this.btnMapsPaste.Location = new System.Drawing.Point(165, 275);
			this.btnMapsPaste.Name = "btnMapsPaste";
			this.btnMapsPaste.Size = new System.Drawing.Size(55, 25);
			this.btnMapsPaste.TabIndex = 9;
			this.btnMapsPaste.Text = "Paste";
			this.btnMapsPaste.Click += new System.EventHandler(this.btnMapsPaste_Click);
			// 
			// btnMapsCopy
			// 
			this.btnMapsCopy.Location = new System.Drawing.Point(165, 245);
			this.btnMapsCopy.Name = "btnMapsCopy";
			this.btnMapsCopy.Size = new System.Drawing.Size(55, 25);
			this.btnMapsCopy.TabIndex = 8;
			this.btnMapsCopy.Text = "Copy";
			this.btnMapsCopy.Click += new System.EventHandler(this.btnMapsCopy_Click);
			// 
			// btnMapsUp
			// 
			this.btnMapsUp.Location = new System.Drawing.Point(165, 65);
			this.btnMapsUp.Name = "btnMapsUp";
			this.btnMapsUp.Size = new System.Drawing.Size(55, 25);
			this.btnMapsUp.TabIndex = 7;
			this.btnMapsUp.Text = "Up";
			this.btnMapsUp.Click += new System.EventHandler(this.btnMapsUp_Click);
			// 
			// btnMapsDown
			// 
			this.btnMapsDown.Location = new System.Drawing.Point(165, 95);
			this.btnMapsDown.Name = "btnMapsDown";
			this.btnMapsDown.Size = new System.Drawing.Size(55, 25);
			this.btnMapsDown.TabIndex = 6;
			this.btnMapsDown.Text = "Down";
			this.btnMapsDown.Click += new System.EventHandler(this.btnMapsDown_Click);
			// 
			// btnMapsRight
			// 
			this.btnMapsRight.Location = new System.Drawing.Point(165, 175);
			this.btnMapsRight.Name = "btnMapsRight";
			this.btnMapsRight.Size = new System.Drawing.Size(55, 25);
			this.btnMapsRight.TabIndex = 5;
			this.btnMapsRight.Text = ">";
			this.btnMapsRight.Click += new System.EventHandler(this.btnMapsRight_Click);
			// 
			// btnMapsLeft
			// 
			this.btnMapsLeft.Location = new System.Drawing.Point(165, 145);
			this.btnMapsLeft.Name = "btnMapsLeft";
			this.btnMapsLeft.Size = new System.Drawing.Size(55, 25);
			this.btnMapsLeft.TabIndex = 4;
			this.btnMapsLeft.Text = "<";
			this.btnMapsLeft.Click += new System.EventHandler(this.btnMapsLeft_Click);
			// 
			// lbMapsImagesAll
			// 
			this.lbMapsImagesAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbMapsImagesAll.ItemHeight = 12;
			this.lbMapsImagesAll.Location = new System.Drawing.Point(225, 30);
			this.lbMapsImagesAll.Name = "lbMapsImagesAll";
			this.lbMapsImagesAll.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbMapsImagesAll.Size = new System.Drawing.Size(155, 304);
			this.lbMapsImagesAll.TabIndex = 3;
			// 
			// lblMapsImagesAll
			// 
			this.lblMapsImagesAll.Location = new System.Drawing.Point(225, 15);
			this.lblMapsImagesAll.Name = "lblMapsImagesAll";
			this.lblMapsImagesAll.Size = new System.Drawing.Size(65, 15);
			this.lblMapsImagesAll.TabIndex = 2;
			this.lblMapsImagesAll.Text = "not in use";
			// 
			// lbMapsImagesUsed
			// 
			this.lbMapsImagesUsed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.lbMapsImagesUsed.ItemHeight = 12;
			this.lbMapsImagesUsed.Location = new System.Drawing.Point(5, 30);
			this.lbMapsImagesUsed.Name = "lbMapsImagesUsed";
			this.lbMapsImagesUsed.Size = new System.Drawing.Size(155, 304);
			this.lbMapsImagesUsed.TabIndex = 1;
			// 
			// lblMapsImagesUsed
			// 
			this.lblMapsImagesUsed.Location = new System.Drawing.Point(5, 15);
			this.lblMapsImagesUsed.Name = "lblMapsImagesUsed";
			this.lblMapsImagesUsed.Size = new System.Drawing.Size(100, 15);
			this.lblMapsImagesUsed.TabIndex = 0;
			this.lblMapsImagesUsed.Text = "SpriteSets in use";
			// 
			// gbMapsTerrain
			// 
			this.gbMapsTerrain.Controls.Add(this.lblMapsOccults);
			this.gbMapsTerrain.Controls.Add(this.tbMapsOccults);
			this.gbMapsTerrain.Controls.Add(this.cbMapsPalette);
			this.gbMapsTerrain.Controls.Add(this.lblMapsPalette);
			this.gbMapsTerrain.Controls.Add(this.lblMapsRoutes);
			this.gbMapsTerrain.Controls.Add(this.lblMapsMaps);
			this.gbMapsTerrain.Controls.Add(this.tbMapsRoutes);
			this.gbMapsTerrain.Controls.Add(this.tbMapsMaps);
			this.gbMapsTerrain.Enabled = false;
			this.gbMapsTerrain.Location = new System.Drawing.Point(240, 0);
			this.gbMapsTerrain.Name = "gbMapsTerrain";
			this.gbMapsTerrain.Size = new System.Drawing.Size(385, 170);
			this.gbMapsTerrain.TabIndex = 1;
			this.gbMapsTerrain.TabStop = false;
			this.gbMapsTerrain.Text = "TERRAIN GROUP";
			// 
			// lblMapsOccults
			// 
			this.lblMapsOccults.Location = new System.Drawing.Point(5, 95);
			this.lblMapsOccults.Name = "lblMapsOccults";
			this.lblMapsOccults.Size = new System.Drawing.Size(100, 15);
			this.lblMapsOccults.TabIndex = 7;
			this.lblMapsOccults.Text = "Path to OTD files";
			// 
			// tbMapsOccults
			// 
			this.tbMapsOccults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbMapsOccults.Location = new System.Drawing.Point(5, 112);
			this.tbMapsOccults.Name = "tbMapsOccults";
			this.tbMapsOccults.Size = new System.Drawing.Size(373, 19);
			this.tbMapsOccults.TabIndex = 6;
			// 
			// cbMapsPalette
			// 
			this.cbMapsPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMapsPalette.Location = new System.Drawing.Point(5, 140);
			this.cbMapsPalette.Name = "cbMapsPalette";
			this.cbMapsPalette.Size = new System.Drawing.Size(115, 20);
			this.cbMapsPalette.TabIndex = 5;
			this.cbMapsPalette.SelectedIndexChanged += new System.EventHandler(this.cbMapsPalette_SelectedIndexChanged);
			// 
			// lblMapsPalette
			// 
			this.lblMapsPalette.Location = new System.Drawing.Point(125, 145);
			this.lblMapsPalette.Name = "lblMapsPalette";
			this.lblMapsPalette.Size = new System.Drawing.Size(45, 15);
			this.lblMapsPalette.TabIndex = 4;
			this.lblMapsPalette.Text = "Palette";
			// 
			// lblMapsRoutes
			// 
			this.lblMapsRoutes.Location = new System.Drawing.Point(5, 55);
			this.lblMapsRoutes.Name = "lblMapsRoutes";
			this.lblMapsRoutes.Size = new System.Drawing.Size(100, 15);
			this.lblMapsRoutes.TabIndex = 3;
			this.lblMapsRoutes.Text = "Path to RMP files";
			// 
			// lblMapsMaps
			// 
			this.lblMapsMaps.Location = new System.Drawing.Point(5, 15);
			this.lblMapsMaps.Name = "lblMapsMaps";
			this.lblMapsMaps.Size = new System.Drawing.Size(100, 15);
			this.lblMapsMaps.TabIndex = 2;
			this.lblMapsMaps.Text = "Path to MAP files";
			// 
			// tbMapsRoutes
			// 
			this.tbMapsRoutes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbMapsRoutes.Location = new System.Drawing.Point(5, 72);
			this.tbMapsRoutes.Name = "tbMapsRoutes";
			this.tbMapsRoutes.Size = new System.Drawing.Size(373, 19);
			this.tbMapsRoutes.TabIndex = 1;
			this.tbMapsRoutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMapsRoutes_KeyPress);
			this.tbMapsRoutes.Leave += new System.EventHandler(this.tbMapsRoutes_Leave);
			// 
			// tbMapsMaps
			// 
			this.tbMapsMaps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbMapsMaps.Location = new System.Drawing.Point(5, 32);
			this.tbMapsMaps.Name = "tbMapsMaps";
			this.tbMapsMaps.Size = new System.Drawing.Size(373, 19);
			this.tbMapsMaps.TabIndex = 0;
			this.tbMapsMaps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMapsMaps_KeyPress);
			this.tbMapsMaps.Leave += new System.EventHandler(this.tbMapsMaps_Leave);
			// 
			// tvMaps
			// 
			this.tvMaps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.tvMaps.ContextMenu = this.cmMain;
			this.tvMaps.Location = new System.Drawing.Point(0, 35);
			this.tvMaps.Name = "tvMaps";
			this.tvMaps.Size = new System.Drawing.Size(240, 473);
			this.tvMaps.TabIndex = 0;
			this.tvMaps.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMaps_AfterSelect);
			// 
			// cmMain
			// 
			this.cmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.newGroup,
			this.delGroup,
			this.miAddSubset,
			this.delSub,
			this.addMap,
			this.delMap});
			// 
			// newGroup
			// 
			this.newGroup.Index = 0;
			this.newGroup.Text = "New group";
			this.newGroup.Click += new System.EventHandler(this.newGroup_Click);
			// 
			// delGroup
			// 
			this.delGroup.Index = 1;
			this.delGroup.Text = "Delete group";
			this.delGroup.Click += new System.EventHandler(this.delGroup_Click);
			// 
			// miAddSubset
			// 
			this.miAddSubset.Index = 2;
			this.miAddSubset.Text = "Add sub-group";
			this.miAddSubset.Click += new System.EventHandler(this.OnSubsetClick);
			// 
			// delSub
			// 
			this.delSub.Index = 3;
			this.delSub.Text = "Delete sub-group";
			this.delSub.Click += new System.EventHandler(this.delSub_Click);
			// 
			// addMap
			// 
			this.addMap.Index = 4;
			this.addMap.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.addNewMap,
			this.addExistingMap});
			this.addMap.Text = "Add map";
			// 
			// addNewMap
			// 
			this.addNewMap.Index = 0;
			this.addNewMap.Text = "New Map";
			this.addNewMap.Click += new System.EventHandler(this.addNewMap_Click);
			// 
			// addExistingMap
			// 
			this.addExistingMap.Index = 1;
			this.addExistingMap.Text = "Existing Map";
			this.addExistingMap.Click += new System.EventHandler(this.addExistingMap_Click);
			// 
			// delMap
			// 
			this.delMap.Index = 5;
			this.delMap.Text = "Delete map";
			this.delMap.Click += new System.EventHandler(this.delMap_Click);
			// 
			// btnMapsSaveTree
			// 
			this.btnMapsSaveTree.Location = new System.Drawing.Point(130, 5);
			this.btnMapsSaveTree.Name = "btnMapsSaveTree";
			this.btnMapsSaveTree.Size = new System.Drawing.Size(85, 35);
			this.btnMapsSaveTree.TabIndex = 8;
			this.btnMapsSaveTree.Text = "Save - out of order";
			this.btnMapsSaveTree.Click += new System.EventHandler(this.btnMapsSaveTree_Click);
			// 
			// btnMapsEditTree
			// 
			this.btnMapsEditTree.Location = new System.Drawing.Point(25, 5);
			this.btnMapsEditTree.Name = "btnMapsEditTree";
			this.btnMapsEditTree.Size = new System.Drawing.Size(85, 35);
			this.btnMapsEditTree.TabIndex = 6;
			this.btnMapsEditTree.Text = "Edit - out of order";
			this.btnMapsEditTree.Click += new System.EventHandler(this.btnMapsEditTree_Click);
			// 
			// tabImages
			// 
			this.tabImages.Controls.Add(this.lblImagesImages);
			this.tabImages.Controls.Add(this.tbImagesImages);
			this.tabImages.Controls.Add(this.btnImagesSave);
			this.tabImages.Controls.Add(this.lblImagesTerrain);
			this.tabImages.Controls.Add(this.tbImagesTerrain);
			this.tabImages.Controls.Add(this.lbImages);
			this.tabImages.Location = new System.Drawing.Point(4, 22);
			this.tabImages.Name = "tabImages";
			this.tabImages.Size = new System.Drawing.Size(624, 508);
			this.tabImages.TabIndex = 2;
			this.tabImages.Text = "Image Files";
			// 
			// lblImagesImages
			// 
			this.lblImagesImages.Location = new System.Drawing.Point(245, 50);
			this.lblImagesImages.Name = "lblImagesImages";
			this.lblImagesImages.Size = new System.Drawing.Size(115, 15);
			this.lblImagesImages.TabIndex = 14;
			this.lblImagesImages.Text = "Path to Images.cfg";
			// 
			// tbImagesImages
			// 
			this.tbImagesImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbImagesImages.Location = new System.Drawing.Point(245, 67);
			this.tbImagesImages.Name = "tbImagesImages";
			this.tbImagesImages.ReadOnly = true;
			this.tbImagesImages.Size = new System.Drawing.Size(375, 19);
			this.tbImagesImages.TabIndex = 12;
			// 
			// btnImagesSave
			// 
			this.btnImagesSave.Location = new System.Drawing.Point(250, 115);
			this.btnImagesSave.Name = "btnImagesSave";
			this.btnImagesSave.Size = new System.Drawing.Size(85, 35);
			this.btnImagesSave.TabIndex = 3;
			this.btnImagesSave.Text = "Save";
			this.btnImagesSave.Click += new System.EventHandler(this.btnImagesSave_Click);
			// 
			// lblImagesTerrain
			// 
			this.lblImagesTerrain.Location = new System.Drawing.Point(245, 10);
			this.lblImagesTerrain.Name = "lblImagesTerrain";
			this.lblImagesTerrain.Size = new System.Drawing.Size(155, 15);
			this.lblImagesTerrain.TabIndex = 2;
			this.lblImagesTerrain.Text = "Path to MCD/PCK/TAB files";
			// 
			// tbImagesTerrain
			// 
			this.tbImagesTerrain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbImagesTerrain.Location = new System.Drawing.Point(245, 25);
			this.tbImagesTerrain.Name = "tbImagesTerrain";
			this.tbImagesTerrain.ReadOnly = true;
			this.tbImagesTerrain.Size = new System.Drawing.Size(375, 19);
			this.tbImagesTerrain.TabIndex = 1;
			this.tbImagesTerrain.TextChanged += new System.EventHandler(this.tbImagesTerrain_TextChanged);
			// 
			// lbImages
			// 
			this.lbImages.ContextMenu = this.cmImages;
			this.lbImages.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbImages.ItemHeight = 12;
			this.lbImages.Location = new System.Drawing.Point(0, 0);
			this.lbImages.Name = "lbImages";
			this.lbImages.Size = new System.Drawing.Size(240, 508);
			this.lbImages.TabIndex = 0;
			this.lbImages.SelectedIndexChanged += new System.EventHandler(this.lbImages_SelectedIndexChanged);
			// 
			// cmImages
			// 
			this.cmImages.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.addImageset,
			this.delImageset});
			// 
			// addImageset
			// 
			this.addImageset.Index = 0;
			this.addImageset.Text = "Add";
			this.addImageset.Click += new System.EventHandler(this.cmImagesAddImageset_Click);
			// 
			// delImageset
			// 
			this.delImageset.Index = 1;
			this.delImageset.Text = "Remove";
			this.delImageset.Click += new System.EventHandler(this.cmImagesDeleteImageset_Click);
			// 
			// ofdFileDialog
			// 
			this.ofdFileDialog.Filter = "Config files|*.cfg|Map files|*.map|Pck files|*.pck|All files|*.*";
			// 
			// PathsEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(632, 534);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(640, 560);
			this.Menu = this.mmMenu;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(640, 560);
			this.Name = "PathsEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Paths Editor";
			this.tabs.ResumeLayout(false);
			this.tabPaths.ResumeLayout(false);
			this.tabPaths.PerformLayout();
			this.tabMaps.ResumeLayout(false);
			this.gbMapsBlock.ResumeLayout(false);
			this.gbMapsTerrain.ResumeLayout(false);
			this.gbMapsTerrain.PerformLayout();
			this.tabImages.ResumeLayout(false);
			this.tabImages.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private MainMenu mmMenu;
		private MenuItem miFile;
		private TabControl tabs;
		private TabPage tabPaths;
		private TabPage tabMaps;
		private TabPage tabImages;
		private Label lblPathsMaps;
		private Label lblPathsImages;
		private Label lblPathsCursor;
		private TextBox tbPathsMaps;
		private TextBox tbPathsImages;
		private TextBox tbPathsCursor;
		private Label lblPathsSave;
		private ListBox lbImages;
		private TextBox tbImagesTerrain;
		private Label lblImagesTerrain;
		private GroupBox gbMapsTerrain;
		private GroupBox gbMapsBlock;
		private TextBox tbMapsMaps;
		private TextBox tbMapsRoutes;
		private Label lblMapsMaps;
		private Label lblMapsRoutes;
		private Label lblMapsPalette;
		private TreeView tvMaps;
		private ContextMenu cmMain;
		private Label lblMapsImagesUsed;
		private ListBox lbMapsImagesUsed;
		private Label lblMapsImagesAll;
		private ListBox lbMapsImagesAll;
		private Button btnMapsLeft;
		private Button btnMapsRight;
		private Button btnMapsDown;
		private Button btnMapsUp;
		private Button btnPathsClearRegistry;
		private IContainer components;
		private TextBox tbPathsPalettes;
		private Label lblPathsPalettes;
		private Button btnPathsMaps;
		private Button btnPathsImages;
		private OpenFileDialog ofdFileDialog;
		private ContextMenu cmImages;
		private MenuItem addImageset;
		private MenuItem delImageset;
		private MenuItem newGroup;
		private MenuItem addMap;
		private MenuItem delMap;
		private MenuItem delGroup;
		private MenuItem menuItem1;
		private Button btnPathsInstall;
		private Button btnPathsSave;
		private Button btnImagesSave;
		private Label lblImagesImages;
		private TextBox tbImagesImages;
		private ComboBox cbMapsPalette;
		private Button btnMapsSaveTree;
		private MenuItem miAddSubset;
		private MenuItem delSub;
		private MenuItem addNewMap;
		private MenuItem addExistingMap;
		private Button btnMapsEditTree;
		private MenuItem closeItem;
		private Label lblMapsOccults;
		private TextBox tbMapsOccults;
		private Button btnMapsCopy;
		private Button btnMapsPaste;
		private Label lblPathsInstall;
		private Label lblPathsClearRegistry;
	}
}
