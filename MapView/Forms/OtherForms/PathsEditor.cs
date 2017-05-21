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
	internal sealed partial class PathsEditor
		:
			Form
	{
//		private static bool _saveRegistry = true;
//		internal static bool SaveRegistry
//		{
//			get { return _saveRegistry; }
//			set { _saveRegistry = value; }
//		}

		private string _paths;
		private List<string> _terrains;


		internal PathsEditor(string pathsPath)
		{
			_paths = pathsPath;

			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			tbPathsMaps.Text    = ResourceInfo.TileGroupInfo.Path;
			tbPathsImages.Text  = ResourceInfo.TerrainInfo.Path;
			tbImagesImages.Text = ResourceInfo.TerrainInfo.Path;

//			txtCursor.Text   = GameInfo.CursorPath;
//			txtCursor.Text   = GameInfo.MiscInfo.CursorFile;
//			txtPalettes.Text = GameInfo.PalettePath;

			PopulateTerrainList();

			populateTree();

			cbMapsPalette.Items.Add(Palette.UfoBattle);
			cbMapsPalette.Items.Add(Palette.TftdBattle);
		}


		private void populateTree()
		{
			tvMaps.Nodes.Clear();

			var groups = new ArrayList();
			foreach (object keyGroup in ResourceInfo.TileGroupInfo.TileGroups.Keys)
				groups.Add(keyGroup);

			groups.Sort();

			var categories = new ArrayList();
			foreach (string gruop in groups)
			{
				var itGroup = ResourceInfo.TileGroupInfo.TileGroups[gruop];
				if (itGroup != null)
				{
					var nodeGroup = tvMaps.Nodes.Add(gruop);

					categories.Clear();

					foreach (string keyCategory in itGroup.Categories.Keys)
						categories.Add(keyCategory);

					categories.Sort();

					foreach (string category in categories)
					{
						var itCategory = itGroup.Categories[category];
						if (itCategory != null)
						{
							var nodeCategory = nodeGroup.Nodes.Add(category);

							var tilesets = new ArrayList();
							foreach (string keyTileset in itCategory.Keys)
								tilesets.Add(keyTileset);

							tilesets.Sort();

							foreach (string tileset in tilesets)
								if (itCategory[tileset] != null)
									nodeCategory.Nodes.Add(tileset);
						}
					}
				}
			}
		}

		private void PopulateTerrainList()
		{
			var terrains = new ArrayList();

			var infoTerrain = ResourceInfo.TerrainInfo;
			foreach (object keyTerrain in infoTerrain.Terrains.Keys)
				terrains.Add(keyTerrain);

			terrains.Sort();

			foreach (object terrain in terrains)
			{
				lbImages.Items.Add(terrain);
				lbMapsImagesAll.Items.Add(terrain);
			}
		}

		private void lbImages_SelectedIndexChanged(object sender, EventArgs e)
		{
			tbImagesTerrain.Text = ResourceInfo.TerrainInfo[(string)lbImages.SelectedItem].PathDirectory;
		}

		private void tvMaps_AfterSelect(object sender, TreeViewEventArgs e)
		{
			gbMapsTerrain.Enabled = true;
			gbMapsBlock.Enabled   =
			delMap.Enabled        = false;

			TileGroup tilegroup = null;

			var node = e.Node;
			if (node.Parent != null)
			{
				addMap.Enabled =
				delSub.Enabled = true;

				if (node.Parent.Parent != null) // tileset node
				{
					gbMapsTerrain.Enabled = false;
					gbMapsBlock.Enabled   =
					delMap.Enabled        = true;

					tilegroup = ResourceInfo.TileGroupInfo.TileGroups[node.Parent.Parent.Text] as TileGroup;

					var descriptor = (Descriptor)tilegroup[node.Text];

					lbMapsImagesUsed.Items.Clear();

					if (descriptor != null)
					{
						foreach (string terrain in descriptor.Terrains)
							lbMapsImagesUsed.Items.Add(terrain);
					}
					else
					{
						tilegroup.AddTileset(
										new Descriptor(
													node.Text,
//													tilegroup.RouteDirectory,
//													tilegroup.OccultDirectory,
													new List<string>(),
													tilegroup.MapDirectory,	// TODO: fix the Map directory.
													tilegroup.Palette),		// TODO: fix the Palette determination.
										node.Parent.Text);
					}
				}
				else // category node
				{
					tilegroup = (TileGroup)ResourceInfo.TileGroupInfo.TileGroups[node.Parent.Text];
				}
			}
			else // group node
			{
				tilegroup = (TileGroup)ResourceInfo.TileGroupInfo.TileGroups[node.Text];
				addMap.Enabled =
				delMap.Enabled =
				delSub.Enabled = false;
			}

			tbMapsMaps.Text    = tilegroup.MapDirectory;
			tbMapsRoutes.Text  = tilegroup.RouteDirectory;
			tbMapsOccults.Text = tilegroup.OccultDirectory; // TODO: remove that.

			cbMapsPalette.SelectedItem = tilegroup.Palette;
		}

		private void tbMapsMaps_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Enter, System.Globalization.CultureInfo.InvariantCulture))
				tbMapsMaps_Leave(null, null);
		}

		private void tbMapsMaps_Leave(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
			if (!Directory.Exists(tbMapsMaps.Text))
			{
				using (var output = new OutputBox("Directory not found: " + tbMapsMaps.Text))
				{
					// TODO: Directory.CreateDirectory(dir);
					output.ShowDialog();
					tbMapsMaps.Text = tileset.MapDirectory;
				}
			}
			tileset.MapDirectory = tbMapsMaps.Text;
		}

		private void tbMapsRoutes_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Enter, System.Globalization.CultureInfo.InvariantCulture))
				tbMapsRoutes_Leave(null, null);
		}

		private void tbMapsRoutes_Leave(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
			if (!Directory.Exists(tbMapsRoutes.Text))
			{
				using (var output = new OutputBox("Directory not found: " + tbMapsRoutes.Text))
				{
					// TODO: Directory.CreateDirectory(dir);
					output.ShowDialog();
					tbMapsRoutes.Text = tileset.RouteDirectory;
				}
			}
			tileset.RouteDirectory = tbMapsRoutes.Text;
		}

		private void btnMapsUp_Click(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
			var terrains = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains;

			for (int id = 1; id != terrains.Count; ++id)
			{
				if (terrains[id] == lbMapsImagesUsed.SelectedItem as String)
				{
					string t = terrains[id - 1];
					terrains[id - 1] = terrains[id];
					terrains[id] = t;

					((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains = terrains;

					lbMapsImagesUsed.Items.Clear();

					foreach (string terrain in terrains)
						lbMapsImagesUsed.Items.Add(terrain);

					lbMapsImagesUsed.SelectedItem = terrains[id - 1];
					return;
				}
			}
		}

		private void btnMapsDown_Click(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
			var terrains = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains;

			for (int id = 0; id != terrains.Count - 1; ++id)
			{
				if (terrains[id] == lbMapsImagesUsed.SelectedItem as String)
				{
					string t = terrains[id + 1];
					terrains[id + 1] = terrains[id];
					terrains[id] = t;

					((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains = terrains;
					
					lbMapsImagesUsed.Items.Clear();

					foreach (string terrain in terrains)
						lbMapsImagesUsed.Items.Add(terrain);

					lbMapsImagesUsed.SelectedItem = terrains[id + 1];
					return;
				}
			}
		}

		private void btnMapsRight_Click(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
//			var terrains = new ArrayList(((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains);
			var terrains = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains;

//			terrains.Remove(lbMapsImagesUsed.SelectedItem);
			terrains.Remove(lbMapsImagesUsed.SelectedItem as String);

//			((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains = (string[])terrains.ToArray(typeof(string));
			
			lbMapsImagesUsed.Items.Clear();

			foreach (string terrain in terrains)
				lbMapsImagesUsed.Items.Add(terrain);
		}

		private void btnMapsLeft_Click(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset
//			var terrains = new ArrayList(((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains);
			var terrains = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains;

			foreach (string terrain in lbMapsImagesAll.SelectedItems)
			{
				if (!terrains.Contains(terrain)) // safety.
				{
					terrains.Add(terrain);
//					((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains = (string[])terrain.ToArray(typeof(string));
				
					lbMapsImagesUsed.Items.Clear();

					foreach (string terrain0 in terrains)
						lbMapsImagesUsed.Items.Add(terrain0);
				}
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
			ResourceInfo.TerrainInfo[(string)lbImages.SelectedItem].PathDirectory = tbImagesTerrain.Text;
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
	
						ResourceInfo.TerrainInfo[file] = new Terrain(file, path);
					}
	
					lbImages.Items.Clear();
					lbMapsImagesAll.Items.Clear();
	
					PopulateTerrainList();
				}
			}
		}

		private void cmImagesDeleteImageset_Click(object sender, EventArgs e)
		{
			ResourceInfo.TerrainInfo.Terrains.Remove(lbImages.SelectedItem.ToString());

			lbImages.Items.Clear();
			lbMapsImagesAll.Items.Clear();

			PopulateTerrainList();
		}

		private void btnPathsInstall_Click(object sender, EventArgs e)
		{
			using (var f = new ConfigurationForm())
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
			ResourceInfo.TerrainInfo.Save(tbPathsImages.Text);
		}

		private void newGroup_Click(object sender, EventArgs e)
		{
			using (var f = new TileGroupForm())
			{
				f.ShowDialog(this);

				if (f.TileGroupLabel != null)
				{
					var tilegroup = (TileGroup)ResourceInfo.TileGroupInfo.AddTileGroup(
																				f.TileGroupLabel,
																				f.MapsPath,
																				f.RoutesPath,
																				f.OccultsPath);
//					addTileset(tileset.Name);
					tvMaps.Nodes.Add(tilegroup.Label);

					tbMapsMaps.Text   = tilegroup.MapDirectory;
					tbMapsRoutes.Text = tilegroup.RouteDirectory;

//					saveMapedit();
				}
			}
		}

		private void cbMapsPalette_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tvMaps.SelectedNode.Parent == null)
				GetCurrentTileGroup().Palette = cbMapsPalette.SelectedItem as Palette; // <- could be a group, category, or tileset
		}

		private void OnSubsetClick(object sender, EventArgs e)
		{
			using (var f = new CategoryForm())
			{
				f.ShowDialog(this);
				if (!String.IsNullOrEmpty(f.CategoryLabel))
				{
					var tilegroup = GetCurrentTileGroup(); // <- could be a group, category, or tileset

//					TreeNode tn = treeMaps.SelectedNode; // TODO: Check if not used.

					tilegroup.Categories[f.CategoryLabel] = new Dictionary<string, Descriptor>();

//					tileset.NewSubset(f.SubsetName);
//					saveMapedit();

					populateTree();
				}
			}
		}

		private TileGroup GetCurrentTileGroup() // <- could be a group, category, or tileset
		{
			var node = tvMaps.SelectedNode;

			if (node.Parent == null)
				return ResourceInfo.TileGroupInfo.TileGroups[node.Text] as TileGroup;

			if (node.Parent.Parent == null)
				return ResourceInfo.TileGroupInfo.TileGroups[node.Parent.Text] as TileGroup;

			return ResourceInfo.TileGroupInfo.TileGroups[node.Parent.Parent.Text] as TileGroup;
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
						string pfe = tbMapsMaps.Text + f.MapLabel + MapFileChild.MapExt; // TODO: this string is probly wrong since YAML.
						if (File.Exists(pfe))
						{
							using (var fdialog = new ChoiceDialog(pfe))
							{
								fdialog.ShowDialog(this);
	
								if (fdialog.Choice == Choice.UseExisting)
									return;
							}
						}

						MapFileChild.CreateMap(
											File.OpenWrite(pfe),
											f.MapRows,
											f.MapCols,
											f.MapHeight);

						using (var fs = File.OpenWrite(tbMapsRoutes.Text + f.MapLabel + RouteNodeCollection.RouteExt)) // TODO: wtf.
						{}

						TileGroup tileset;
						string label;

						if (tvMaps.SelectedNode.Parent.Parent == null)
						{
							tileset = (TileGroup)ResourceInfo.TileGroupInfo.TileGroups[tvMaps.SelectedNode.Parent.Text];
							tvMaps.SelectedNode.Nodes.Add(f.MapLabel);
							label = tvMaps.SelectedNode.Text;
						}
						else
						{
							tileset = (TileGroup)ResourceInfo.TileGroupInfo.TileGroups[tvMaps.SelectedNode.Parent.Parent.Text];
							tvMaps.SelectedNode.Parent.Nodes.Add(f.MapLabel);
							label = tvMaps.SelectedNode.Parent.Text;
						}

						tileset.AddTileset(f.MapLabel, label);

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

						var tileset = (TileGroup)ResourceInfo.TileGroupInfo.TileGroups[tn.Parent.Text];
						foreach (string file in f.FileNames)
						{
							int start = file.LastIndexOf(@"\", StringComparison.Ordinal) + 1;
							int end   = file.LastIndexOf(".", StringComparison.Ordinal);

							string name = file.Substring(start, end-start);
							try
							{
								tileset.AddTileset(name, tn.Text);
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

		private void OnDeleteGroupClick(object sender, EventArgs e)
		{
			var tilegroup = GetCurrentTileGroup(); // <- could be a group, category, or tileset

			ResourceInfo.TileGroupInfo.TileGroups[tilegroup.Label] = null;

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
		}

		private void OnDeleteCategoryClick(object sender, EventArgs e)
		{
			var node = tvMaps.SelectedNode.Parent;
			if (node != null)
			{
				if (node.Parent == null)
					node = tvMaps.SelectedNode;

				if (node != null)
				{
					var tilegroup = GetCurrentTileGroup(); // <- could be a group, category, or tileset

					tilegroup.Categories[node.Text] = null;

					node.Parent.Nodes.Remove(node);
				}
			}
		}

		private void OnDeleteTilesetClick(object sender, EventArgs e)
		{
			var node = tvMaps.SelectedNode;
			if (node.Parent != null && node.Parent.Parent != null)
			{
				var tilegroup = GetCurrentTileGroup(); // <- could be a group, category, or tileset

				tilegroup.Categories[node.Parent.Text][node.Text] = null;
//				tilegroup[node.Text] = null;

				node.Parent.Nodes.Remove(node);
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
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset

			int length = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains.Count;
			_terrains = new List<string>();

			for (int id = 0; id != length; ++id)
				_terrains[id] = ((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains[id];
		}

		private void btnMapsPaste_Click(object sender, EventArgs e)
		{
			var tileset = GetCurrentTileGroup(); // <- could be a group, category, or tileset

			((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains = new List<string>();// new string[_images.Length];

			lbMapsImagesUsed.Items.Clear();

			for (int id = 0; id != _terrains.Count; ++id)
			{
				((Descriptor)tileset[tvMaps.SelectedNode.Text]).Terrains[id] = _terrains[id];
				lbMapsImagesUsed.Items.Add(_terrains[id]);
			}
		}
	}
}
