namespace MapView
{
	partial class XCMainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XCMainWindow));
			this.mmMain = new System.Windows.Forms.MainMenu(this.components);
			this.fileMenu = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.saveItem = new System.Windows.Forms.MenuItem();
			this.miSaveImage = new System.Windows.Forms.MenuItem();
			this.miExport = new System.Windows.Forms.MenuItem();
			this.miResize = new System.Windows.Forms.MenuItem();
			this.miHq = new System.Windows.Forms.MenuItem();
			this.bar = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.miPaths = new System.Windows.Forms.MenuItem();
			this.miOptions = new System.Windows.Forms.MenuItem();
			this.miInfo = new System.Windows.Forms.MenuItem();
			this.miAnimation = new System.Windows.Forms.MenuItem();
			this.onItem = new System.Windows.Forms.MenuItem();
			this.offItem = new System.Windows.Forms.MenuItem();
			this.miDoors = new System.Windows.Forms.MenuItem();
			this.showMenu = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.tvMaps = new System.Windows.Forms.TreeView();
			this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.tsslMap = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslDimensions = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslPosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.tsEdit = new System.Windows.Forms.ToolStrip();
			this.tsbSelectionBox = new System.Windows.Forms.ToolStripButton();
			this.tsbZoomIn = new System.Windows.Forms.ToolStripButton();
			this.tsbZoomOut = new System.Windows.Forms.ToolStripButton();
			this.tsbAutoZoom = new System.Windows.Forms.ToolStripButton();
			this.csSplitter = new DSShared.Windows.CollapsibleSplitter();
			this.ssMain.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.tsEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.fileMenu,
			this.miEdit,
			this.miAnimation,
			this.showMenu,
			this.miHelp});
			// 
			// fileMenu
			// 
			this.fileMenu.Index = 0;
			this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miOpen,
			this.saveItem,
			this.miSaveImage,
			this.miExport,
			this.miResize,
			this.miHq,
			this.bar,
			this.miQuit});
			this.fileMenu.Text = "&File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Text = "Open";
			this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
			// 
			// saveItem
			// 
			this.saveItem.Index = 1;
			this.saveItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.saveItem.Text = "&Save";
			this.saveItem.Click += new System.EventHandler(this.saveItem_Click);
			// 
			// miSaveImage
			// 
			this.miSaveImage.Index = 2;
			this.miSaveImage.Text = "Save Image";
			this.miSaveImage.Click += new System.EventHandler(this.miSaveImage_Click);
			// 
			// miExport
			// 
			this.miExport.Index = 3;
			this.miExport.Text = "Export";
			this.miExport.Visible = false;
			this.miExport.Click += new System.EventHandler(this.miExport_Click);
			// 
			// miResize
			// 
			this.miResize.Index = 4;
			this.miResize.Text = "Resize map";
			this.miResize.Click += new System.EventHandler(this.miResize_Click);
			// 
			// miHq
			// 
			this.miHq.Index = 5;
			this.miHq.Text = "Hq2x";
			this.miHq.Click += new System.EventHandler(this.miHq_Click);
			// 
			// bar
			// 
			this.bar.Index = 6;
			this.bar.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 7;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.miQuit_Click);
			// 
			// miEdit
			// 
			this.miEdit.Index = 1;
			this.miEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPaths,
			this.miOptions,
			this.miInfo});
			this.miEdit.Text = "Edit";
			// 
			// miPaths
			// 
			this.miPaths.Index = 0;
			this.miPaths.Text = "Paths";
			this.miPaths.Click += new System.EventHandler(this.miPaths_Click);
			// 
			// miOptions
			// 
			this.miOptions.Index = 1;
			this.miOptions.Text = "Options";
			this.miOptions.Click += new System.EventHandler(this.miOptions_Click);
			// 
			// miInfo
			// 
			this.miInfo.Index = 2;
			this.miInfo.Text = "Map Info";
			this.miInfo.Click += new System.EventHandler(this.miInfo_Click);
			// 
			// miAnimation
			// 
			this.miAnimation.Index = 2;
			this.miAnimation.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.onItem,
			this.offItem,
			this.miDoors});
			this.miAnimation.Text = "&Animation";
			this.miAnimation.Visible = false;
			// 
			// onItem
			// 
			this.onItem.Checked = true;
			this.onItem.Index = 0;
			this.onItem.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.onItem.Text = "O&n";
			this.onItem.Click += new System.EventHandler(this.onItem_Click);
			// 
			// offItem
			// 
			this.offItem.Index = 1;
			this.offItem.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.offItem.Text = "O&ff";
			this.offItem.Click += new System.EventHandler(this.offItem_Click);
			// 
			// miDoors
			// 
			this.miDoors.Index = 2;
			this.miDoors.Text = "Doors";
			this.miDoors.Click += new System.EventHandler(this.miDoors_Click);
			// 
			// showMenu
			// 
			this.showMenu.Enabled = false;
			this.showMenu.Index = 3;
			this.showMenu.Text = "&View";
			// 
			// miHelp
			// 
			this.miHelp.Index = 4;
			this.miHelp.Text = "Help";
			// 
			// tvMaps
			// 
			this.tvMaps.BackColor = System.Drawing.SystemColors.Control;
			this.tvMaps.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvMaps.Location = new System.Drawing.Point(0, 0);
			this.tvMaps.Name = "tvMaps";
			this.tvMaps.Size = new System.Drawing.Size(249, 454);
			this.tvMaps.TabIndex = 0;
			// 
			// sfdSaveDialog
			// 
			this.sfdSaveDialog.DefaultExt = "gif";
			this.sfdSaveDialog.Filter = "gif files|*.gif";
			this.sfdSaveDialog.RestoreDirectory = true;
			// 
			// ssMain
			// 
			this.ssMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsslMap,
			this.tsslDimensions,
			this.tsslPosition});
			this.ssMain.Location = new System.Drawing.Point(257, 432);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(375, 22);
			this.ssMain.TabIndex = 2;
			this.ssMain.Text = "statusStrip1";
			// 
			// tsslMap
			// 
			this.tsslMap.AutoSize = false;
			this.tsslMap.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslMap.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsslMap.Name = "tsslMap";
			this.tsslMap.Size = new System.Drawing.Size(150, 17);
			// 
			// tsslDimensions
			// 
			this.tsslDimensions.AutoSize = false;
			this.tsslDimensions.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslDimensions.Name = "tsslDimensions";
			this.tsslDimensions.Size = new System.Drawing.Size(80, 17);
			// 
			// tsslPosition
			// 
			this.tsslPosition.AutoSize = false;
			this.tsslPosition.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslPosition.Name = "tsslPosition";
			this.tsslPosition.Size = new System.Drawing.Size(80, 17);
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.BottomToolStripPanel
			// 
			this.toolStripContainer1.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(375, 407);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// toolStripContainer1.LeftToolStripPanel
			// 
			this.toolStripContainer1.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStripContainer1.Location = new System.Drawing.Point(257, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			// 
			// toolStripContainer1.RightToolStripPanel
			// 
			this.toolStripContainer1.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStripContainer1.Size = new System.Drawing.Size(375, 432);
			this.toolStripContainer1.TabIndex = 3;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsEdit);
			this.toolStripContainer1.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tsEdit
			// 
			this.tsEdit.Dock = System.Windows.Forms.DockStyle.None;
			this.tsEdit.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsbSelectionBox,
			this.tsbZoomIn,
			this.tsbZoomOut,
			this.tsbAutoZoom});
			this.tsEdit.Location = new System.Drawing.Point(3, 0);
			this.tsEdit.Name = "tsEdit";
			this.tsEdit.Size = new System.Drawing.Size(196, 25);
			this.tsEdit.TabIndex = 0;
			// 
			// tsbSelectionBox
			// 
			this.tsbSelectionBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsbSelectionBox.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsbSelectionBox.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelectionBox.Image")));
			this.tsbSelectionBox.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionBox.Name = "tsbSelectionBox";
			this.tsbSelectionBox.Size = new System.Drawing.Size(84, 22);
			this.tsbSelectionBox.Text = "Selection Box";
			this.tsbSelectionBox.ToolTipText = "Draws a selection box in the floor";
			this.tsbSelectionBox.Click += new System.EventHandler(this.drawSelectionBoxButton_Click);
			// 
			// tsbZoomIn
			// 
			this.tsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbZoomIn.Image = global::MapView.Properties.Resources._12_Zoom_in_16;
			this.tsbZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbZoomIn.Name = "tsbZoomIn";
			this.tsbZoomIn.Size = new System.Drawing.Size(23, 22);
			this.tsbZoomIn.Text = "Zoom In";
			this.tsbZoomIn.Click += new System.EventHandler(this.ZoomInButton_Click);
			// 
			// tsbZoomOut
			// 
			this.tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbZoomOut.Image = global::MapView.Properties.Resources._13_Zoom_out_16;
			this.tsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbZoomOut.Name = "tsbZoomOut";
			this.tsbZoomOut.Size = new System.Drawing.Size(23, 22);
			this.tsbZoomOut.Text = "Zoom Out";
			this.tsbZoomOut.Click += new System.EventHandler(this.ZoomOutButton_Click);
			// 
			// tsbAutoZoom
			// 
			this.tsbAutoZoom.Checked = true;
			this.tsbAutoZoom.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsbAutoZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbAutoZoom.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsbAutoZoom.Image = global::MapView.Properties.Resources._11_Search_16;
			this.tsbAutoZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbAutoZoom.Name = "tsbAutoZoom";
			this.tsbAutoZoom.Size = new System.Drawing.Size(23, 22);
			this.tsbAutoZoom.Text = "Auto Zoom";
			this.tsbAutoZoom.Click += new System.EventHandler(this.AutoZoomButton_Click);
			// 
			// csSplitter
			// 
			this.csSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
			this.csSplitter.ControlToHide = this.tvMaps;
			this.csSplitter.Location = new System.Drawing.Point(249, 0);
			this.csSplitter.MinimumSize = new System.Drawing.Size(5, 5);
			this.csSplitter.Name = "cSplitList";
			this.csSplitter.Size = new System.Drawing.Size(8, 454);
			this.csSplitter.TabIndex = 1;
			this.csSplitter.TabStop = false;
			// 
			// XCMainWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.ssMain);
			this.Controls.Add(this.csSplitter);
			this.Controls.Add(this.tvMaps);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(640, 480);
			this.Menu = this.mmMain;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "XCMainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Map Editor";
			this.Activated += new System.EventHandler(this.MainWindow_Activated);
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.tsEdit.ResumeLayout(false);
			this.tsEdit.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.MenuItem fileMenu;
		private System.Windows.Forms.MenuItem miQuit;
		private System.Windows.Forms.MenuItem showMenu;
		private System.Windows.Forms.MenuItem saveItem;
		private System.Windows.Forms.MainMenu mmMain;
		private System.Windows.Forms.MenuItem bar;
		private System.Windows.Forms.MenuItem onItem;
		private System.Windows.Forms.MenuItem offItem;
		private System.Windows.Forms.MenuItem miAnimation;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.TreeView tvMaps;
		private System.Windows.Forms.MenuItem miPaths;
		private System.Windows.Forms.MenuItem miOptions;
		private System.Windows.Forms.MenuItem miSaveImage;
		private System.Windows.Forms.SaveFileDialog sfdSaveDialog;
		private System.Windows.Forms.MenuItem miHq;
		private System.Windows.Forms.MenuItem miDoors;
		private System.Windows.Forms.MenuItem miResize;
		private System.Windows.Forms.MenuItem miInfo;
		private System.Windows.Forms.MenuItem miExport;
		private DSShared.Windows.CollapsibleSplitter csSplitter;
		private System.Windows.Forms.StatusStrip ssMain;
		private System.Windows.Forms.ToolStripStatusLabel tsslMap;
		private System.Windows.Forms.ToolStripStatusLabel tsslDimensions;
		private System.Windows.Forms.ToolStripStatusLabel tsslPosition;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStrip tsEdit;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.ToolStripButton tsbSelectionBox;
		private System.Windows.Forms.ToolStripButton tsbZoomIn;
		private System.Windows.Forms.ToolStripButton tsbZoomOut;
		private System.Windows.Forms.ToolStripButton tsbAutoZoom;

		#endregion
	}
}
