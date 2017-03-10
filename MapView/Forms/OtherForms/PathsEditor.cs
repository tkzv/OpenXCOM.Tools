using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;

using XCom;
using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Summary description for PathsEditor.
	/// </summary>
	public class PathsEditor
		:
		Form
	{
		private MainMenu mainMenu;
		private MenuItem miFile;
		private TabControl tabs;
		private TabPage tabPaths;
		private TabPage tabMaps;
		private TabPage tabImages;
		private Label label1;
		private Label label2;
		private Label label3;
		private TextBox txtMap;
		private TextBox txtImages;
		private TextBox txtCursor;
		private Label lblReminder;
		private ListBox lstImages;
		private TextBox txtImagePath;
		private Label label4;
		private GroupBox grpMapGroup;
		private GroupBox grpMap;
		private TextBox txtRoot;
		private TextBox txtRmp;
		private Label label5;
		private Label label6;
		private Label label7;
		private TreeView treeMaps;
		private ContextMenu cmTree;
		private Label label8;
		private ListBox listMapImages;
		private Label label9;
		private ListBox listAllImages;
		private Button btnMoveLeft;
		private Button button2;
		private Button btnDown;
		private Button btnUp;
		private Button btnClearRegistry;
		private IContainer components;
		private TextBox txtPalettes;
		private Label label10;
		private Button btnFindMap;
		private Button btnFindImage;
		private OpenFileDialog openFile;
		private ContextMenu imagesCM;
		private MenuItem addImageset;
		private MenuItem delImageset;
		private MenuItem newGroup;
		private MenuItem addMap;
		private MenuItem delMap;
		private MenuItem delGroup;

		private static bool saveRegistry = true;
		private MenuItem menuItem1;
		private Button runInstaller;
		private Button btnSave;
		private Button btnSaveImages;
		private Label lblImage2;
		private TextBox txtImage2;
		private ComboBox cbPalette;
		private Button btnSaveMapEdit;
		private MenuItem addSub;
		private MenuItem delSub;
		private MenuItem addNewMap;
		private MenuItem addExistingMap;
		private Button btnEditTree;
		private MenuItem closeItem;
		private Label label11;
		private TextBox txtBlank;
		private Button btnCopy;
		private Button btnPaste;
		private string paths;
		private string[] images;

		public PathsEditor(string pathsPath)
		{
			paths = pathsPath;

			InitializeComponent();

			txtMap.Text    = GameInfo.TilesetInfo.Path;
			txtImages.Text = GameInfo.ImageInfo.Path;
			txtImage2.Text = GameInfo.ImageInfo.Path;

//			txtCursor.Text   = GameInfo.CursorPath;
//			txtCursor.Text   = GameInfo.MiscInfo.CursorFile;
//			txtPalettes.Text = GameInfo.PalettePath;

			populateImageList();

			populateTree();

			cbPalette.Items.Add(Palette.UFOBattle);
			cbPalette.Items.Add(Palette.TFTDBattle);
		}

		private void populateTree()
		{
			treeMaps.Nodes.Clear();

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
					var tn = treeMaps.Nodes.Add(ob); // make the node for the tileset

					list1.Clear();

					foreach (string ob1 in it.Subsets.Keys) // subsets
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

		private void populateImageList()
		{
			ImageInfo ii = GameInfo.ImageInfo;
			var list = new ArrayList();
			foreach (object ob in ii.Images.Keys)
				list.Add(ob);

			list.Sort();
			foreach (object ob in list)
			{
				lstImages.Items.Add(ob);
				listAllImages.Items.Add(ob);
			}
		}

		public static bool SaveRegistry
		{
			get { return saveRegistry; }
			set { saveRegistry = value; }
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
			this.mainMenu = new MainMenu(this.components);
			this.miFile = new MenuItem();
			this.menuItem1 = new MenuItem();
			this.closeItem = new MenuItem();
			this.tabs = new TabControl();
			this.tabPaths = new TabPage();
			this.btnSave = new Button();
			this.runInstaller = new Button();
			this.btnFindImage = new Button();
			this.btnFindMap = new Button();
			this.txtPalettes = new TextBox();
			this.label10 = new Label();
			this.btnClearRegistry = new Button();
			this.lblReminder = new Label();
			this.txtCursor = new TextBox();
			this.txtImages = new TextBox();
			this.txtMap = new TextBox();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.tabMaps = new TabPage();
			this.grpMap = new GroupBox();
			this.btnPaste = new Button();
			this.btnCopy = new Button();
			this.btnUp = new Button();
			this.btnDown = new Button();
			this.button2 = new Button();
			this.btnMoveLeft = new Button();
			this.listAllImages = new ListBox();
			this.label9 = new Label();
			this.listMapImages = new ListBox();
			this.label8 = new Label();
			this.grpMapGroup = new GroupBox();
			this.label11 = new Label();
			this.txtBlank = new TextBox();
			this.cbPalette = new ComboBox();
			this.label7 = new Label();
			this.label6 = new Label();
			this.label5 = new Label();
			this.txtRmp = new TextBox();
			this.txtRoot = new TextBox();
			this.treeMaps = new TreeView();
			this.cmTree = new ContextMenu();
			this.newGroup = new MenuItem();
			this.delGroup = new MenuItem();
			this.addSub = new MenuItem();
			this.delSub = new MenuItem();
			this.addMap = new MenuItem();
			this.addNewMap = new MenuItem();
			this.addExistingMap = new MenuItem();
			this.delMap = new MenuItem();
			this.btnSaveMapEdit = new Button();
			this.btnEditTree = new Button();
			this.tabImages = new TabPage();
			this.lblImage2 = new Label();
			this.txtImage2 = new TextBox();
			this.btnSaveImages = new Button();
			this.label4 = new Label();
			this.txtImagePath = new TextBox();
			this.lstImages = new ListBox();
			this.imagesCM = new ContextMenu();
			this.addImageset = new MenuItem();
			this.delImageset = new MenuItem();
			this.openFile = new OpenFileDialog();
			this.tabs.SuspendLayout();
			this.tabPaths.SuspendLayout();
			this.tabMaps.SuspendLayout();
			this.grpMap.SuspendLayout();
			this.grpMapGroup.SuspendLayout();
			this.tabImages.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new MenuItem[] {
			this.miFile});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new MenuItem[] {
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
			this.tabs.Dock = DockStyle.Fill;
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(634, 633);
			this.tabs.TabIndex = 0;
			// 
			// tabPaths
			// 
			this.tabPaths.Controls.Add(this.btnSave);
			this.tabPaths.Controls.Add(this.runInstaller);
			this.tabPaths.Controls.Add(this.btnFindImage);
			this.tabPaths.Controls.Add(this.btnFindMap);
			this.tabPaths.Controls.Add(this.txtPalettes);
			this.tabPaths.Controls.Add(this.label10);
			this.tabPaths.Controls.Add(this.btnClearRegistry);
			this.tabPaths.Controls.Add(this.lblReminder);
			this.tabPaths.Controls.Add(this.txtCursor);
			this.tabPaths.Controls.Add(this.txtImages);
			this.tabPaths.Controls.Add(this.txtMap);
			this.tabPaths.Controls.Add(this.label3);
			this.tabPaths.Controls.Add(this.label2);
			this.tabPaths.Controls.Add(this.label1);
			this.tabPaths.Location = new System.Drawing.Point(4, 21);
			this.tabPaths.Name = "tabPaths";
			this.tabPaths.Size = new System.Drawing.Size(626, 608);
			this.tabPaths.TabIndex = 0;
			this.tabPaths.Text = "Paths";
			this.tabPaths.Click += new System.EventHandler(this.TabPathsClick);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(10, 140);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(125, 35);
			this.btnSave.TabIndex = 14;
			this.btnSave.Text = "Save paths";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// runInstaller
			// 
			this.runInstaller.Location = new System.Drawing.Point(145, 185);
			this.runInstaller.Name = "runInstaller";
			this.runInstaller.Size = new System.Drawing.Size(125, 35);
			this.runInstaller.TabIndex = 13;
			this.runInstaller.Text = "Run installer";
			this.runInstaller.Click += new System.EventHandler(this.runInstaller_Click);
			// 
			// btnFindImage
			// 
			this.btnFindImage.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnFindImage.Location = new System.Drawing.Point(495, 35);
			this.btnFindImage.Name = "btnFindImage";
			this.btnFindImage.Size = new System.Drawing.Size(70, 20);
			this.btnFindImage.TabIndex = 11;
			this.btnFindImage.Text = "Find";
			this.btnFindImage.Click += new System.EventHandler(this.btnFindImage_Click);
			// 
			// btnFindMap
			// 
			this.btnFindMap.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnFindMap.Location = new System.Drawing.Point(495, 10);
			this.btnFindMap.Name = "btnFindMap";
			this.btnFindMap.Size = new System.Drawing.Size(70, 20);
			this.btnFindMap.TabIndex = 10;
			this.btnFindMap.Text = "Find";
			this.btnFindMap.Click += new System.EventHandler(this.btnFindMap_Click);
			// 
			// txtPalettes
			// 
			this.txtPalettes.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtPalettes.Location = new System.Drawing.Point(70, 85);
			this.txtPalettes.Name = "txtPalettes";
			this.txtPalettes.Size = new System.Drawing.Size(420, 19);
			this.txtPalettes.TabIndex = 9;
			this.txtPalettes.Visible = false;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(5, 90);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(60, 15);
			this.label10.TabIndex = 8;
			this.label10.Text = "Palettes";
			this.label10.Visible = false;
			// 
			// btnClearRegistry
			// 
			this.btnClearRegistry.Location = new System.Drawing.Point(10, 185);
			this.btnClearRegistry.Name = "btnClearRegistry";
			this.btnClearRegistry.Size = new System.Drawing.Size(125, 35);
			this.btnClearRegistry.TabIndex = 7;
			this.btnClearRegistry.Text = "Clear Registry Settings";
			this.btnClearRegistry.Click += new System.EventHandler(this.btnClearRegistry_Click);
			// 
			// lblReminder
			// 
			this.lblReminder.Location = new System.Drawing.Point(10, 120);
			this.lblReminder.Name = "lblReminder";
			this.lblReminder.Size = new System.Drawing.Size(380, 15);
			this.lblReminder.TabIndex = 6;
			this.lblReminder.Text = "None of your changes will be made until you click the Save button.";
			// 
			// txtCursor
			// 
			this.txtCursor.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtCursor.Location = new System.Drawing.Point(70, 60);
			this.txtCursor.Name = "txtCursor";
			this.txtCursor.Size = new System.Drawing.Size(420, 19);
			this.txtCursor.TabIndex = 5;
			// 
			// txtImages
			// 
			this.txtImages.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtImages.Location = new System.Drawing.Point(70, 35);
			this.txtImages.Name = "txtImages";
			this.txtImages.Size = new System.Drawing.Size(420, 19);
			this.txtImages.TabIndex = 4;
			// 
			// txtMap
			// 
			this.txtMap.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtMap.Location = new System.Drawing.Point(70, 10);
			this.txtMap.Name = "txtMap";
			this.txtMap.Size = new System.Drawing.Size(420, 19);
			this.txtMap.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Cursor";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Images";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Map Data";
			this.label1.Click += new System.EventHandler(this.Label1Click);
			// 
			// tabMaps
			// 
			this.tabMaps.Controls.Add(this.grpMap);
			this.tabMaps.Controls.Add(this.grpMapGroup);
			this.tabMaps.Controls.Add(this.treeMaps);
			this.tabMaps.Controls.Add(this.btnSaveMapEdit);
			this.tabMaps.Controls.Add(this.btnEditTree);
			this.tabMaps.Location = new System.Drawing.Point(4, 22);
			this.tabMaps.Name = "tabMaps";
			this.tabMaps.Size = new System.Drawing.Size(626, 590);
			this.tabMaps.TabIndex = 1;
			this.tabMaps.Text = "Map Files";
			// 
			// grpMap
			// 
			this.grpMap.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
			| AnchorStyles.Left)));
			this.grpMap.Controls.Add(this.btnPaste);
			this.grpMap.Controls.Add(this.btnCopy);
			this.grpMap.Controls.Add(this.btnUp);
			this.grpMap.Controls.Add(this.btnDown);
			this.grpMap.Controls.Add(this.button2);
			this.grpMap.Controls.Add(this.btnMoveLeft);
			this.grpMap.Controls.Add(this.listAllImages);
			this.grpMap.Controls.Add(this.label9);
			this.grpMap.Controls.Add(this.listMapImages);
			this.grpMap.Controls.Add(this.label8);
			this.grpMap.Enabled = false;
			this.grpMap.Location = new System.Drawing.Point(240, 170);
			this.grpMap.Name = "grpMap";
			this.grpMap.Size = new System.Drawing.Size(385, 422);
			this.grpMap.TabIndex = 2;
			this.grpMap.TabStop = false;
			this.grpMap.Text = "Map File";
			// 
			// btnPaste
			// 
			this.btnPaste.Location = new System.Drawing.Point(165, 275);
			this.btnPaste.Name = "btnPaste";
			this.btnPaste.Size = new System.Drawing.Size(55, 25);
			this.btnPaste.TabIndex = 9;
			this.btnPaste.Text = "Paste";
			this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
			// 
			// btnCopy
			// 
			this.btnCopy.Location = new System.Drawing.Point(165, 245);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(55, 25);
			this.btnCopy.TabIndex = 8;
			this.btnCopy.Text = "Copy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// btnUp
			// 
			this.btnUp.Location = new System.Drawing.Point(165, 65);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(55, 25);
			this.btnUp.TabIndex = 7;
			this.btnUp.Text = "Up";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Location = new System.Drawing.Point(165, 95);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(55, 25);
			this.btnDown.TabIndex = 6;
			this.btnDown.Text = "Down";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(165, 175);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(55, 25);
			this.button2.TabIndex = 5;
			this.button2.Text = ">";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnMoveLeft
			// 
			this.btnMoveLeft.Location = new System.Drawing.Point(165, 145);
			this.btnMoveLeft.Name = "btnMoveLeft";
			this.btnMoveLeft.Size = new System.Drawing.Size(55, 25);
			this.btnMoveLeft.TabIndex = 4;
			this.btnMoveLeft.Text = "<";
			this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
			// 
			// listAllImages
			// 
			this.listAllImages.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
			| AnchorStyles.Left)));
			this.listAllImages.ItemHeight = 12;
			this.listAllImages.Location = new System.Drawing.Point(225, 30);
			this.listAllImages.Name = "listAllImages";
			this.listAllImages.SelectionMode = SelectionMode.MultiExtended;
			this.listAllImages.Size = new System.Drawing.Size(155, 376);
			this.listAllImages.TabIndex = 3;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(215, 15);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(55, 15);
			this.label9.TabIndex = 2;
			this.label9.Text = "Available Images";
			// 
			// listMapImages
			// 
			this.listMapImages.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
			| AnchorStyles.Left)));
			this.listMapImages.ItemHeight = 12;
			this.listMapImages.Location = new System.Drawing.Point(5, 30);
			this.listMapImages.Name = "listMapImages";
			this.listMapImages.Size = new System.Drawing.Size(155, 376);
			this.listMapImages.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 15);
			this.label8.TabIndex = 0;
			this.label8.Text = "Image List";
			// 
			// grpMapGroup
			// 
			this.grpMapGroup.Controls.Add(this.label11);
			this.grpMapGroup.Controls.Add(this.txtBlank);
			this.grpMapGroup.Controls.Add(this.cbPalette);
			this.grpMapGroup.Controls.Add(this.label7);
			this.grpMapGroup.Controls.Add(this.label6);
			this.grpMapGroup.Controls.Add(this.label5);
			this.grpMapGroup.Controls.Add(this.txtRmp);
			this.grpMapGroup.Controls.Add(this.txtRoot);
			this.grpMapGroup.Enabled = false;
			this.grpMapGroup.Location = new System.Drawing.Point(240, 0);
			this.grpMapGroup.Name = "grpMapGroup";
			this.grpMapGroup.Size = new System.Drawing.Size(385, 170);
			this.grpMapGroup.TabIndex = 1;
			this.grpMapGroup.TabStop = false;
			this.grpMapGroup.Text = "Map Group";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(10, 95);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(75, 15);
			this.label11.TabIndex = 7;
			this.label11.Text = "Blanks Path";
			// 
			// txtBlank
			// 
			this.txtBlank.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtBlank.Location = new System.Drawing.Point(5, 112);
			this.txtBlank.Name = "txtBlank";
			this.txtBlank.Size = new System.Drawing.Size(373, 19);
			this.txtBlank.TabIndex = 6;
			// 
			// cbPalette
			// 
			this.cbPalette.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbPalette.Location = new System.Drawing.Point(65, 140);
			this.cbPalette.Name = "cbPalette";
			this.cbPalette.Size = new System.Drawing.Size(115, 20);
			this.cbPalette.TabIndex = 5;
			this.cbPalette.SelectedIndexChanged += new System.EventHandler(this.cbPalette_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(10, 145);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 15);
			this.label7.TabIndex = 4;
			this.label7.Text = "Palette";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 55);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 15);
			this.label6.TabIndex = 3;
			this.label6.Text = "Rmp Path";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "Root Path";
			// 
			// txtRmp
			// 
			this.txtRmp.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtRmp.Location = new System.Drawing.Point(5, 72);
			this.txtRmp.Name = "txtRmp";
			this.txtRmp.Size = new System.Drawing.Size(373, 19);
			this.txtRmp.TabIndex = 1;
			this.txtRmp.KeyPress += new KeyPressEventHandler(this.txtRmp_KeyPress);
			this.txtRmp.Leave += new System.EventHandler(this.txtRmp_Leave);
			// 
			// txtRoot
			// 
			this.txtRoot.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtRoot.Location = new System.Drawing.Point(5, 32);
			this.txtRoot.Name = "txtRoot";
			this.txtRoot.Size = new System.Drawing.Size(373, 19);
			this.txtRoot.TabIndex = 0;
			this.txtRoot.KeyPress += new KeyPressEventHandler(this.txtRoot_KeyPress);
			this.txtRoot.Leave += new System.EventHandler(this.txtRoot_Leave);
			// 
			// treeMaps
			// 
			this.treeMaps.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
			| AnchorStyles.Left)));
			this.treeMaps.ContextMenu = this.cmTree;
			this.treeMaps.Location = new System.Drawing.Point(0, 35);
			this.treeMaps.Name = "treeMaps";
			this.treeMaps.Size = new System.Drawing.Size(240, 557);
			this.treeMaps.TabIndex = 0;
			this.treeMaps.AfterSelect += new TreeViewEventHandler(this.treeMaps_AfterSelect);
			// 
			// cmTree
			// 
			this.cmTree.MenuItems.AddRange(new MenuItem[] {
			this.newGroup,
			this.delGroup,
			this.addSub,
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
			// addSub
			// 
			this.addSub.Index = 2;
			this.addSub.Text = "Add sub-group";
			this.addSub.Click += new System.EventHandler(this.addSub_Click);
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
			this.addMap.MenuItems.AddRange(new MenuItem[] {
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
			// btnSaveMapEdit
			// 
			this.btnSaveMapEdit.Location = new System.Drawing.Point(130, 5);
			this.btnSaveMapEdit.Name = "btnSaveMapEdit";
			this.btnSaveMapEdit.Size = new System.Drawing.Size(85, 30);
			this.btnSaveMapEdit.TabIndex = 8;
			this.btnSaveMapEdit.Text = "Save";
			this.btnSaveMapEdit.Click += new System.EventHandler(this.btnSaveMapEdit_Click);
			// 
			// btnEditTree
			// 
			this.btnEditTree.Location = new System.Drawing.Point(25, 5);
			this.btnEditTree.Name = "btnEditTree";
			this.btnEditTree.Size = new System.Drawing.Size(85, 30);
			this.btnEditTree.TabIndex = 6;
			this.btnEditTree.Text = "Edit";
			this.btnEditTree.Click += new System.EventHandler(this.moveMaps_Click);
			// 
			// tabImages
			// 
			this.tabImages.Controls.Add(this.lblImage2);
			this.tabImages.Controls.Add(this.txtImage2);
			this.tabImages.Controls.Add(this.btnSaveImages);
			this.tabImages.Controls.Add(this.label4);
			this.tabImages.Controls.Add(this.txtImagePath);
			this.tabImages.Controls.Add(this.lstImages);
			this.tabImages.Location = new System.Drawing.Point(4, 22);
			this.tabImages.Name = "tabImages";
			this.tabImages.Size = new System.Drawing.Size(626, 590);
			this.tabImages.TabIndex = 2;
			this.tabImages.Text = "Image Files";
			// 
			// lblImage2
			// 
			this.lblImage2.Location = new System.Drawing.Point(245, 195);
			this.lblImage2.Name = "lblImage2";
			this.lblImage2.Size = new System.Drawing.Size(95, 15);
			this.lblImage2.TabIndex = 14;
			this.lblImage2.Text = "Image.dat path";
			// 
			// txtImage2
			// 
			this.txtImage2.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtImage2.Location = new System.Drawing.Point(245, 212);
			this.txtImage2.Name = "txtImage2";
			this.txtImage2.ReadOnly = true;
			this.txtImage2.Size = new System.Drawing.Size(380, 19);
			this.txtImage2.TabIndex = 12;
			// 
			// btnSaveImages
			// 
			this.btnSaveImages.Location = new System.Drawing.Point(245, 295);
			this.btnSaveImages.Name = "btnSaveImages";
			this.btnSaveImages.Size = new System.Drawing.Size(85, 35);
			this.btnSaveImages.TabIndex = 3;
			this.btnSaveImages.Text = "Save";
			this.btnSaveImages.Click += new System.EventHandler(this.btnSaveImages_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(245, 95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "Path";
			// 
			// txtImagePath
			// 
			this.txtImagePath.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
			| AnchorStyles.Right)));
			this.txtImagePath.Location = new System.Drawing.Point(245, 112);
			this.txtImagePath.Name = "txtImagePath";
			this.txtImagePath.ReadOnly = true;
			this.txtImagePath.Size = new System.Drawing.Size(380, 19);
			this.txtImagePath.TabIndex = 1;
			this.txtImagePath.TextChanged += new System.EventHandler(this.txtImagePath_TextChanged);
			// 
			// lstImages
			// 
			this.lstImages.ContextMenu = this.imagesCM;
			this.lstImages.Dock = DockStyle.Left;
			this.lstImages.ItemHeight = 12;
			this.lstImages.Location = new System.Drawing.Point(0, 0);
			this.lstImages.Name = "lstImages";
			this.lstImages.Size = new System.Drawing.Size(240, 590);
			this.lstImages.TabIndex = 0;
			this.lstImages.SelectedIndexChanged += new System.EventHandler(this.lstImages_SelectedIndexChanged);
			// 
			// imagesCM
			// 
			this.imagesCM.MenuItems.AddRange(new MenuItem[] {
			this.addImageset,
			this.delImageset});
			// 
			// addImageset
			// 
			this.addImageset.Index = 0;
			this.addImageset.Text = "Add";
			this.addImageset.Click += new System.EventHandler(this.addImageset_Click);
			// 
			// delImageset
			// 
			this.delImageset.Index = 1;
			this.delImageset.Text = "Remove";
			this.delImageset.Click += new System.EventHandler(this.delImageset_Click);
			// 
			// openFile
			// 
			this.openFile.Filter = "map files|*.map|dat files|*.dat|Pck files|*.pck|All files|*.*";
			// 
			// PathsEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(634, 633);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Name = "PathsEditor";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "PathsEditor";
			this.Load += new System.EventHandler(this.PathsEditorLoad);
			this.tabs.ResumeLayout(false);
			this.tabPaths.ResumeLayout(false);
			this.tabPaths.PerformLayout();
			this.tabMaps.ResumeLayout(false);
			this.grpMap.ResumeLayout(false);
			this.grpMapGroup.ResumeLayout(false);
			this.grpMapGroup.PerformLayout();
			this.tabImages.ResumeLayout(false);
			this.tabImages.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void lstImages_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtImagePath.Text = GameInfo.ImageInfo[(string)lstImages.SelectedItem].BasePath;
		}

		private void treeMaps_AfterSelect(object sender, TreeViewEventArgs e)
		{
			grpMapGroup.Enabled = true;
			grpMap.Enabled =
			delMap.Enabled = false;

			IXCTileset tileset = null;

			var tn = e.Node;
			if (tn.Parent != null)
			{
				addMap.Enabled =
				delSub.Enabled = true;

				if (tn.Parent.Parent != null) // inner node
				{
					grpMapGroup.Enabled = false;
					grpMap.Enabled =
					delMap.Enabled = true;

					tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Parent.Text];

					var desc = (XCMapDesc)tileset[tn.Text];

					listMapImages.Items.Clear();

					if (desc != null)
					{
						foreach (string st in desc.Dependencies)
							listMapImages.Items.Add(st);
					}
					else
					{
						tileset.AddMap(
									new XCMapDesc(
												tn.Text,
												tileset.MapPath,
												tileset.BlankPath,
												tileset.RmpPath, new string[]{},
												tileset.Palette),
									tn.Parent.Text);
					}
				}
				else // subset node
				{
					tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Text];
				}
			}
			else // parent node
			{
				tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Text];
				addMap.Enabled =
				delMap.Enabled =
				delSub.Enabled = false;
			}

			txtRoot.Text = tileset.MapPath;
			txtRmp.Text = tileset.RmpPath;
			txtBlank.Text = tileset.BlankPath;
			cbPalette.SelectedItem = tileset.Palette;
		}

		private void txtRoot_Leave(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			if (!Directory.Exists(txtRoot.Text)
				&& NoDirForm.Show(txtRoot.Text) != DialogResult.OK)
			{
				txtRoot.Text = tileset.MapPath;
			}
			tileset.MapPath = txtRoot.Text;
		}

		private void txtRoot_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
				txtRoot_Leave(null, null);
		}

		private void txtRmp_Leave(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			if (!Directory.Exists(txtRmp.Text)
				&& NoDirForm.Show(txtRmp.Text) != DialogResult.OK)
			{
				txtRmp.Text = tileset.RmpPath;
			}
			tileset.RmpPath = txtRmp.Text;
		}

		private void txtRmp_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
				txtRmp_Leave(null, null);
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			string[] dep = ((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies;

			for (int i = 1; i < dep.Length; i++)
			{
				if (dep[i] == (string)listMapImages.SelectedItem)
				{
					string old = dep[i - 1];
					dep[i - 1] = dep[i];
					dep[i] = old;

					((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies = dep;

					listMapImages.Items.Clear();

					foreach (string st in dep)
						listMapImages.Items.Add(st);

					listMapImages.SelectedItem = dep[i - 1];
					return;
				}
			}
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			string[] dep = ((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies;

			for (int i = 0; i < dep.Length - 1; i++)
			{
				if (dep[i] == (string)listMapImages.SelectedItem)
				{
					string old = dep[i + 1];
					dep[i + 1] = dep[i];
					dep[i] = old;

					((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies = dep;
					
					listMapImages.Items.Clear();

					foreach (string st in dep)
						listMapImages.Items.Add(st);

					listMapImages.SelectedItem = dep[i + 1];
					return;
				}
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			var dep = new ArrayList(((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies);
			dep.Remove(listMapImages.SelectedItem);
			((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies = (string[])dep.ToArray(typeof(string));
			
			listMapImages.Items.Clear();

			foreach (string st in dep)
				listMapImages.Items.Add(st);
		}

		private void btnMoveLeft_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			var dep = new ArrayList(((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies);

			foreach (object ob in listAllImages.SelectedItems)
				if (!dep.Contains(ob))
				{
					dep.Add(ob);
					((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies = (string[])dep.ToArray(typeof(string));
				
					listMapImages.Items.Clear();

					foreach (string st in dep)
						listMapImages.Items.Add(st);
				}
		}

		private void btnClearRegistry_Click(object sender, System.EventArgs e)
		{
			RegistryKey swKey = Registry.CurrentUser.OpenSubKey("Software", true);
			swKey.DeleteSubKeyTree("MapView");
			saveRegistry = false;
		}

		private void btnFindMap_Click(object sender, System.EventArgs e)
		{
			openFile.Title = "Find the map data file";
			openFile.Multiselect = false;
			openFile.FilterIndex = 1;
			if (openFile.ShowDialog() == DialogResult.OK)
				txtMap.Text = openFile.FileName;
		}

		private void btnFindImage_Click(object sender, System.EventArgs e)
		{
			openFile.Title = "Find the image data file";
			openFile.Multiselect = false;
			openFile.FilterIndex = 1;

			if (openFile.ShowDialog() == DialogResult.OK)
				txtImages.Text = openFile.FileName;
		}

/*		private void miSave_Click(object sender, System.EventArgs e)
		{
			StreamWriter sw = new StreamWriter(new FileStream(this.paths, FileMode.Create));
			
			GameInfo.CursorPath		= txtCursor.Text;
			GameInfo.MapPath		= txtMap.Text;
			GameInfo.ImagePath		= txtImages.Text;
//			GameInfo.PalettePath	= txtPalettes.Text;

			sw.WriteLine("mapdata:" + txtMap.Text);
			sw.WriteLine("images:" + txtImages.Text);
			sw.WriteLine("cursor:" + txtCursor.Text);
//			sw.WriteLine("palettes:" + txtPalettes.Text);

			sw.Flush();
			sw.Close();

//			GameInfo.GetImageInfo().Save(new FileStream(txtImages.Text, FileMode.Create));
//			GameInfo.GetTileInfo().Save(new FileStream(txtMap.Text, FileMode.Create));
		}*/

		private void txtImagePath_TextChanged(object sender, System.EventArgs e)
		{
			GameInfo.ImageInfo[(string)lstImages.SelectedItem].BasePath = txtImagePath.Text;
		}

		private void addImageset_Click(object sender, System.EventArgs e)
		{
			openFile.Title = "Add images";
			openFile.Multiselect = true;
			openFile.FilterIndex = 2;

			if (openFile.ShowDialog(this) == DialogResult.OK)
			{
				foreach (string st in openFile.FileNames)
				{
					string path = st.Substring(0, st.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					string file = st.Substring(st.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					file = file.Substring(0, file.IndexOf(".", StringComparison.Ordinal));
					GameInfo.ImageInfo[file] = new ImageDescriptor(file, path);
				}

				lstImages.Items.Clear();
				listAllImages.Items.Clear();

				populateImageList();
			}
		}

		private void delImageset_Click(object sender, System.EventArgs e)
		{
			GameInfo.ImageInfo.Images.Remove(lstImages.SelectedItem.ToString());

			lstImages.Items.Clear();
			listAllImages.Items.Clear();

			populateImageList();
		}

		private void runInstaller_Click(object sender, System.EventArgs e)
		{
			var iw = new InstallWindow();
			iw.ShowDialog(this);
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			var sw = new StreamWriter(new FileStream(this.paths, FileMode.Create));
			
//			GameInfo.CursorPath		= txtCursor.Text;
//			GameInfo.MapPath		= txtMap.Text;
//			GameInfo.ImagePath		= txtImages.Text;
//			GameInfo.PalettePath	= txtPalettes.Text;

			sw.WriteLine("mapdata:" + txtMap.Text);
			sw.WriteLine("images:" + txtImages.Text);
			sw.WriteLine("cursor:" + txtCursor.Text);
//			sw.WriteLine("palettes:" + txtPalettes.Text);

			sw.Flush();
			sw.Close();
		}

		private void btnSaveImages_Click(object sender, System.EventArgs e)
		{
			GameInfo.ImageInfo.Save(txtImages.Text);
		}

		private void newGroup_Click(object sender, System.EventArgs e)
		{
			var tf = new TilesetForm();
			tf.ShowDialog(this);

			if (tf.TilesetText != null)
			{
				var tileset = (IXCTileset)GameInfo.TilesetInfo.AddTileset(
																	tf.TilesetText,
																	tf.MapPath,
																	tf.RmpPath,
																	tf.BlankPath);
//				addTileset(tileset.Name);
				treeMaps.Nodes.Add(tileset.Name);

				txtRoot.Text = tileset.MapPath;
				txtRmp.Text = tileset.RmpPath;

//				saveMapedit();
			}
		}

		private void cbPalette_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (treeMaps.SelectedNode.Parent == null)
				getCurrentTileset().Palette = (Palette)cbPalette.SelectedItem;
		}

		private void saveMapedit()
		{
			var cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			string path = txtMap.Text.Substring(0, txtMap.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
			string file = txtMap.Text.Substring(txtMap.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
			string ext = file.Substring(file.LastIndexOf(".", StringComparison.Ordinal));
			file = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));

			try
			{
				GameInfo.TilesetInfo.Save(path + file + ".new");

				if (File.Exists(txtMap.Text))
				{
					if (File.Exists(path + file + ".old"))
						File.Delete(path + file + ".old");

					File.Move(txtMap.Text, path + file + ".old");
				}

				File.Move(path + file + ".new", txtMap.Text);

				Cursor.Current = cursor;
				Text = "PathsEditor: Saved Mapedit.dat";
			}
			catch (Exception e)
			{
				if (File.Exists(path + file + ".new"))
					File.Delete(path + file + ".new");

				throw e;
			}
		}

		private void btnSaveMapEdit_Click(object sender, System.EventArgs e)
		{
			saveMapedit();
		}

		private void addSub_Click(object sender, System.EventArgs e)
		{
			var sf = new SubsetForm();
			sf.ShowDialog(this);
			if (sf.SubsetName != null)
			{
				var tileset = getCurrentTileset();

//				TreeNode tn = treeMaps.SelectedNode; // TODO: Check if not used.

				tileset.Subsets[sf.SubsetName] = new Dictionary<string, IMapDesc>();

//				tileset.NewSubset(sf.SubsetName);
//				saveMapedit();

				populateTree();
			}
		}

		private IXCTileset getCurrentTileset()
		{
			var tn = treeMaps.SelectedNode;
			if (tn.Parent != null)
			{
				if (tn.Parent.Parent != null)
					return (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Parent.Text]; // inner node

				return (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Text]; // subset node
			}
			return (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Text]; // parent node
		}

//		private void btnSave2_Click(object sender, System.EventArgs e)
//		{
//			saveMapedit();
//		}

		private void addNewMap_Click(object sender, System.EventArgs e)
		{
			var nf = new NewMapForm();
			nf.ShowDialog(this);

			if (nf.MapName != null)
			{
				if (treeMaps.SelectedNode.Parent != null) // add to here
				{
					string path = txtRoot.Text + nf.MapName + ".MAP";
					if (File.Exists(path))
					{
						var cd = new ChoiceDialog(path);
						cd.ShowDialog(this);
						switch (cd.Choice)
						{
							case Choice.Cancel:
							case Choice.UseExisting:
								return;

							case Choice.Overwrite:
								XCMapFile.NewMap(
											File.OpenWrite(txtRoot.Text + nf.MapName + ".MAP"),
											nf.MapRows,
											nf.MapCols,
											nf.MapHeight);
								var fs = File.OpenWrite(txtRmp.Text + nf.MapName + ".RMP");
								fs.Close();
								break;
						}
					}
					else
					{
						XCMapFile.NewMap(
									File.OpenWrite(txtRoot.Text + nf.MapName + ".MAP"),
									nf.MapRows,
									nf.MapCols,
									nf.MapHeight);
						var fs = File.OpenWrite(txtRmp.Text + nf.MapName + ".RMP");
						fs.Close();
					}

					IXCTileset tileset;
					string label = String.Empty;

					if (treeMaps.SelectedNode.Parent.Parent == null) // subset
					{
						tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[treeMaps.SelectedNode.Parent.Text];
						treeMaps.SelectedNode.Nodes.Add(nf.MapName);
						label = treeMaps.SelectedNode.Text;
					}
					else // subset is parent
					{
						tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[treeMaps.SelectedNode.Parent.Parent.Text];
						treeMaps.SelectedNode.Parent.Nodes.Add(nf.MapName);
						label = treeMaps.SelectedNode.Parent.Text;
					}

//					Type1MapData tmd = new Type1MapData(nf.MapName, nf.mapp
					
					tileset.AddMap(nf.MapName, label);
					//Console.WriteLine("map added: " + tileset.Name);

//					saveMapedit();
				}
//				else // top node, baaaaad
//				{
//					tileset = GameInfo.GetTileInfo()[treeMaps.SelectedNode.Parent.Text];
//					treeMaps.SelectedNode.Parent.Nodes.Add(nf.MapName);
//				}
			}
		}

		private void addExistingMap_Click(object sender, System.EventArgs e)
		{
			openFile.InitialDirectory = txtRoot.Text;
			openFile.Title = "Select maps from this directory only";
			openFile.Multiselect      =
			openFile.RestoreDirectory = true;

			if (openFile.ShowDialog() == DialogResult.OK)
			{
				if (treeMaps.SelectedNode.Parent != null) // add to here
				{
					TreeNode tn;
					if (treeMaps.SelectedNode.Parent.Parent == null)
						tn = treeMaps.SelectedNode;
					else
						tn = treeMaps.SelectedNode.Parent;

					var tileset = (IXCTileset)GameInfo.TilesetInfo.Tilesets[tn.Parent.Text];
					foreach (string file in openFile.FileNames)
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
							MessageBox.Show("Could not add map: " + name + ", Error: " + ex.Message);
						}
					}
//					saveMapedit();
				}
//				else // top node, baaaaad
//				{
//					tileset = GameInfo.GetTileInfo()[treeMaps.SelectedNode.Parent.Text];
//					treeMaps.SelectedNode.Parent.Nodes.Add(nfm.MapName);
//				}
			}
		}

		private void delGroup_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			GameInfo.TilesetInfo.Tilesets[tileset.Name] = null;
			if (treeMaps.SelectedNode.Parent == null)
			{
				treeMaps.Nodes.Remove(treeMaps.SelectedNode);
			}
			else if (treeMaps.SelectedNode.Parent.Parent == null)
			{
				treeMaps.Nodes.Remove(treeMaps.SelectedNode.Parent);
			}
			else
				treeMaps.Nodes.Remove(treeMaps.SelectedNode.Parent.Parent);

//			saveMapedit();
		}

		private void delSub_Click(object sender, System.EventArgs e)
		{
			var tn = treeMaps.SelectedNode.Parent;
			if (tn != null)
			{
				if (treeMaps.SelectedNode.Parent.Parent == null)
					tn = treeMaps.SelectedNode;

				if (tn != null)
				{
					var tileset = getCurrentTileset();
					tileset.Subsets[tn.Text] = null;
					tn.Parent.Nodes.Remove(tn);
				}
			}
		}

		private void delMap_Click(object sender, System.EventArgs e)
		{
			if (   treeMaps.SelectedNode.Parent != null
				&& treeMaps.SelectedNode.Parent.Parent != null)
			{
				TreeNode tn = treeMaps.SelectedNode;
				if (tn != null)
				{
					var tileset = getCurrentTileset();
					tileset.Subsets[tn.Parent.Text][tn.Text] = null;
					tileset[tn.Text] = null;
					tn.Parent.Nodes.Remove(tn);
				}
			}
		}

		private void moveMaps_Click(object sender, System.EventArgs e)
		{
//			TreeEditor mmf = new TreeEditor();
//			mmf.ShowDialog(this);
//			populateTree();

//			saveMapedit();
		}

		private void closeItem_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			images = new string[((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies.Length];

			for (int i = 0; i < ((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies.Length; i++)
				images[i] = ((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies[i];
		}

		private void btnPaste_Click(object sender, System.EventArgs e)
		{
			var tileset = getCurrentTileset();
			((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies = new string[images.Length];

			listMapImages.Items.Clear();

			for (int i = 0; i < images.Length; i++)
			{
				((XCMapDesc)tileset[treeMaps.SelectedNode.Text]).Dependencies[i] = images[i];
				listMapImages.Items.Add(images[i]);
			}
		}
		
		void Label1Click(object sender, EventArgs e)
		{
			// TODO: Implement Label1Click
		}
		
		void PathsEditorLoad(object sender, EventArgs e)
		{
			// TODO: Implement PathsEditorLoad
		}
		
		void TabPathsClick(object sender, EventArgs e)
		{
			// TODO: Implement TabPathsClick
		}
	}
}
