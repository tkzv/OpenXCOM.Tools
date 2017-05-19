namespace MapView
{
	partial class PathsEditor
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

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
			this.tabImages.Location = new System.Drawing.Point(4, 21);
			this.tabImages.Name = "tabImages";
			this.tabImages.Size = new System.Drawing.Size(624, 509);
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
			this.lbImages.Size = new System.Drawing.Size(240, 509);
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

		private System.Windows.Forms.MainMenu mmMenu;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabPaths;
		private System.Windows.Forms.TabPage tabMaps;
		private System.Windows.Forms.TabPage tabImages;
		private System.Windows.Forms.Label lblPathsMaps;
		private System.Windows.Forms.Label lblPathsImages;
		private System.Windows.Forms.Label lblPathsCursor;
		private System.Windows.Forms.TextBox tbPathsMaps;
		private System.Windows.Forms.TextBox tbPathsImages;
		private System.Windows.Forms.TextBox tbPathsCursor;
		private System.Windows.Forms.Label lblPathsSave;
		private System.Windows.Forms.ListBox lbImages;
		private System.Windows.Forms.TextBox tbImagesTerrain;
		private System.Windows.Forms.Label lblImagesTerrain;
		private System.Windows.Forms.GroupBox gbMapsTerrain;
		private System.Windows.Forms.GroupBox gbMapsBlock;
		private System.Windows.Forms.TextBox tbMapsMaps;
		private System.Windows.Forms.TextBox tbMapsRoutes;
		private System.Windows.Forms.Label lblMapsMaps;
		private System.Windows.Forms.Label lblMapsRoutes;
		private System.Windows.Forms.Label lblMapsPalette;
		private System.Windows.Forms.TreeView tvMaps;
		private System.Windows.Forms.ContextMenu cmMain;
		private System.Windows.Forms.Label lblMapsImagesUsed;
		private System.Windows.Forms.ListBox lbMapsImagesUsed;
		private System.Windows.Forms.Label lblMapsImagesAll;
		private System.Windows.Forms.ListBox lbMapsImagesAll;
		private System.Windows.Forms.Button btnMapsLeft;
		private System.Windows.Forms.Button btnMapsRight;
		private System.Windows.Forms.Button btnMapsDown;
		private System.Windows.Forms.Button btnMapsUp;
		private System.Windows.Forms.Button btnPathsClearRegistry;
		private System.Windows.Forms.TextBox tbPathsPalettes;
		private System.Windows.Forms.Label lblPathsPalettes;
		private System.Windows.Forms.Button btnPathsMaps;
		private System.Windows.Forms.Button btnPathsImages;
		private System.Windows.Forms.OpenFileDialog ofdFileDialog;
		private System.Windows.Forms.ContextMenu cmImages;
		private System.Windows.Forms.MenuItem addImageset;
		private System.Windows.Forms.MenuItem delImageset;
		private System.Windows.Forms.MenuItem newGroup;
		private System.Windows.Forms.MenuItem addMap;
		private System.Windows.Forms.MenuItem delMap;
		private System.Windows.Forms.MenuItem delGroup;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.Button btnPathsInstall;
		private System.Windows.Forms.Button btnPathsSave;
		private System.Windows.Forms.Button btnImagesSave;
		private System.Windows.Forms.Label lblImagesImages;
		private System.Windows.Forms.TextBox tbImagesImages;
		private System.Windows.Forms.ComboBox cbMapsPalette;
		private System.Windows.Forms.Button btnMapsSaveTree;
		private System.Windows.Forms.MenuItem miAddSubset;
		private System.Windows.Forms.MenuItem delSub;
		private System.Windows.Forms.MenuItem addNewMap;
		private System.Windows.Forms.MenuItem addExistingMap;
		private System.Windows.Forms.Button btnMapsEditTree;
		private System.Windows.Forms.MenuItem closeItem;
		private System.Windows.Forms.Label lblMapsOccults;
		private System.Windows.Forms.TextBox tbMapsOccults;
		private System.Windows.Forms.Button btnMapsCopy;
		private System.Windows.Forms.Button btnMapsPaste;
		private System.Windows.Forms.Label lblPathsInstall;
		private System.Windows.Forms.Label lblPathsClearRegistry;
	}
}
