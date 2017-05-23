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
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveImage = new System.Windows.Forms.MenuItem();
			this.miExport = new System.Windows.Forms.MenuItem();
			this.miResize = new System.Windows.Forms.MenuItem();
			this.miInfo = new System.Windows.Forms.MenuItem();
			this.miHq = new System.Windows.Forms.MenuItem();
			this.miBarHori = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miRegenOccult = new System.Windows.Forms.MenuItem();
			this.menuEdit = new System.Windows.Forms.MenuItem();
			this.miPaths = new System.Windows.Forms.MenuItem();
			this.miOptions = new System.Windows.Forms.MenuItem();
			this.menuAnimation = new System.Windows.Forms.MenuItem();
			this.miOn = new System.Windows.Forms.MenuItem();
			this.miOff = new System.Windows.Forms.MenuItem();
			this.miDoors = new System.Windows.Forms.MenuItem();
			this.menuView = new System.Windows.Forms.MenuItem();
			this.menuHelp = new System.Windows.Forms.MenuItem();
			this.tvMaps = new System.Windows.Forms.TreeView();
			this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.tsslScale = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslMapLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslDimensions = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslPosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.tsEdit = new System.Windows.Forms.ToolStrip();
			this.tsbSelectionBox = new System.Windows.Forms.ToolStripButton();
			this.tsbZoomIn = new System.Windows.Forms.ToolStripButton();
			this.tsbZoomOut = new System.Windows.Forms.ToolStripButton();
			this.tsbAutoZoom = new System.Windows.Forms.ToolStripButton();
			this.csSplitter = new DSShared.Windows.CollapsibleSplitter();
			this.ssMain.SuspendLayout();
			this.tscPanel.TopToolStripPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.tsEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuFile,
			this.menuEdit,
			this.menuAnimation,
			this.menuView,
			this.menuHelp});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miOpen,
			this.miSave,
			this.miSaveImage,
			this.miExport,
			this.miResize,
			this.miInfo,
			this.miHq,
			this.miBarHori,
			this.miQuit,
			this.miRegenOccult});
			this.menuFile.Text = "&File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Text = "&Open";
			this.miOpen.Visible = false;
			this.miOpen.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// miSave
			// 
			this.miSave.Index = 1;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "&Save";
			this.miSave.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// miSaveImage
			// 
			this.miSaveImage.Index = 2;
			this.miSaveImage.Text = "Save &Image";
			this.miSaveImage.Click += new System.EventHandler(this.OnSaveImageClick);
			// 
			// miExport
			// 
			this.miExport.Index = 3;
			this.miExport.Text = "&Export";
			this.miExport.Visible = false;
			this.miExport.Click += new System.EventHandler(this.OnExportClick);
			// 
			// miResize
			// 
			this.miResize.Index = 4;
			this.miResize.Text = "&Resize map";
			this.miResize.Click += new System.EventHandler(this.OnMapResizeClick);
			// 
			// miInfo
			// 
			this.miInfo.Index = 5;
			this.miInfo.Text = "&Map Info";
			this.miInfo.Click += new System.EventHandler(this.OnInfoClick);
			// 
			// miHq
			// 
			this.miHq.Index = 6;
			this.miHq.Text = "Hq&2x";
			this.miHq.Click += new System.EventHandler(this.OnHq2xClick);
			// 
			// miBarHori
			// 
			this.miBarHori.Index = 7;
			this.miBarHori.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 8;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// miRegenOccult
			// 
			this.miRegenOccult.Index = 9;
			this.miRegenOccult.Text = "Regen &Occult";
			this.miRegenOccult.Click += new System.EventHandler(this.OnRegenOccultClick);
			// 
			// menuEdit
			// 
			this.menuEdit.Index = 1;
			this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPaths,
			this.miOptions});
			this.menuEdit.Text = "&Edit";
			// 
			// miPaths
			// 
			this.miPaths.Index = 0;
			this.miPaths.Text = "&Paths";
			this.miPaths.Click += new System.EventHandler(this.OnPathsEditorClick);
			// 
			// miOptions
			// 
			this.miOptions.Index = 1;
			this.miOptions.Text = "&Options";
			this.miOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// menuAnimation
			// 
			this.menuAnimation.Index = 2;
			this.menuAnimation.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miOn,
			this.miOff,
			this.miDoors});
			this.menuAnimation.Text = "menuAnimation";
			this.menuAnimation.Visible = false;
			// 
			// miOn
			// 
			this.miOn.Checked = true;
			this.miOn.Index = 0;
			this.miOn.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.miOn.Text = "miOn";
			this.miOn.Click += new System.EventHandler(this.OnOnClick);
			// 
			// miOff
			// 
			this.miOff.Index = 1;
			this.miOff.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miOff.Text = "miOff";
			this.miOff.Click += new System.EventHandler(this.OnOffClick);
			// 
			// miDoors
			// 
			this.miDoors.Index = 2;
			this.miDoors.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.miDoors.Text = "miDoors";
			this.miDoors.Click += new System.EventHandler(this.OnToggleDoorsClick);
			// 
			// menuView
			// 
			this.menuView.Enabled = false;
			this.menuView.Index = 3;
			this.menuView.Text = "&View";
			// 
			// menuHelp
			// 
			this.menuHelp.Index = 4;
			this.menuHelp.Text = "&Help";
			// 
			// tvMaps
			// 
			this.tvMaps.BackColor = System.Drawing.SystemColors.Control;
			this.tvMaps.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvMaps.Location = new System.Drawing.Point(0, 0);
			this.tvMaps.Name = "tvMaps";
			this.tvMaps.Size = new System.Drawing.Size(240, 452);
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
			this.tsslScale,
			this.tsslMapLabel,
			this.tsslDimensions,
			this.tsslPosition});
			this.ssMain.Location = new System.Drawing.Point(248, 430);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(382, 22);
			this.ssMain.TabIndex = 2;
			this.ssMain.Text = "statusStrip1";
			// 
			// tsslScale
			// 
			this.tsslScale.AutoSize = false;
			this.tsslScale.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslScale.Name = "tsslScale";
			this.tsslScale.Size = new System.Drawing.Size(90, 17);
			// 
			// tsslMapLabel
			// 
			this.tsslMapLabel.AutoSize = false;
			this.tsslMapLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslMapLabel.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsslMapLabel.Name = "tsslMapLabel";
			this.tsslMapLabel.Size = new System.Drawing.Size(69, 17);
			this.tsslMapLabel.Spring = true;
			// 
			// tsslDimensions
			// 
			this.tsslDimensions.AutoSize = false;
			this.tsslDimensions.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslDimensions.Name = "tsslDimensions";
			this.tsslDimensions.Size = new System.Drawing.Size(90, 17);
			// 
			// tsslPosition
			// 
			this.tsslPosition.AutoSize = false;
			this.tsslPosition.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.tsslPosition.Name = "tsslPosition";
			this.tsslPosition.Size = new System.Drawing.Size(110, 17);
			// 
			// tscPanel
			// 
			// 
			// tscPanel.BottomToolStripPanel
			// 
			this.tscPanel.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tscPanel.ContentPanel
			// 
			this.tscPanel.ContentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(382, 405);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Location = new System.Drawing.Point(248, 0);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Size = new System.Drawing.Size(382, 430);
			this.tscPanel.TabIndex = 4;
			// 
			// tscPanel.TopToolStripPanel
			// 
			this.tscPanel.TopToolStripPanel.Controls.Add(this.tsEdit);
			this.tscPanel.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tsEdit
			// 
			this.tsEdit.Dock = System.Windows.Forms.DockStyle.None;
			this.tsEdit.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsEdit.GripMargin = new System.Windows.Forms.Padding(0);
			this.tsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsbSelectionBox,
			this.tsbZoomIn,
			this.tsbZoomOut,
			this.tsbAutoZoom});
			this.tsEdit.Location = new System.Drawing.Point(3, 0);
			this.tsEdit.Name = "tsEdit";
			this.tsEdit.Padding = new System.Windows.Forms.Padding(0);
			this.tsEdit.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsEdit.Size = new System.Drawing.Size(76, 25);
			this.tsEdit.TabIndex = 3;
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
			this.tsbSelectionBox.Visible = false;
			this.tsbSelectionBox.Click += new System.EventHandler(this.OnSelectionBoxClick);
			// 
			// tsbZoomIn
			// 
			this.tsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbZoomIn.Image = global::MapView.Properties.Resources._12_Zoom_in_16;
			this.tsbZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbZoomIn.Name = "tsbZoomIn";
			this.tsbZoomIn.Size = new System.Drawing.Size(23, 22);
			this.tsbZoomIn.Text = "Zoom In";
			this.tsbZoomIn.Click += new System.EventHandler(this.OnZoomInClick);
			// 
			// tsbZoomOut
			// 
			this.tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbZoomOut.Image = global::MapView.Properties.Resources._13_Zoom_out_16;
			this.tsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbZoomOut.Name = "tsbZoomOut";
			this.tsbZoomOut.Size = new System.Drawing.Size(23, 22);
			this.tsbZoomOut.Text = "Zoom Out";
			this.tsbZoomOut.Click += new System.EventHandler(this.OnZoomOutClick);
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
			this.tsbAutoZoom.Click += new System.EventHandler(this.OnAutoScaleClick);
			// 
			// csSplitter
			// 
			this.csSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
			this.csSplitter.ControlToHide = this.tvMaps;
			this.csSplitter.Location = new System.Drawing.Point(240, 0);
			this.csSplitter.MinimumSize = new System.Drawing.Size(5, 5);
			this.csSplitter.Name = "cSplitList";
			this.csSplitter.Size = new System.Drawing.Size(8, 452);
			this.csSplitter.TabIndex = 1;
			this.csSplitter.TabStop = false;
			// 
			// XCMainWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(630, 452);
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.ssMain);
			this.Controls.Add(this.csSplitter);
			this.Controls.Add(this.tvMaps);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(640, 480);
			this.Menu = this.mmMain;
			this.MinimumSize = new System.Drawing.Size(538, 295);
			this.Name = "XCMainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Map Editor";
			this.Activated += new System.EventHandler(this.OnMainWindowActivated);
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.tscPanel.TopToolStripPanel.ResumeLayout(false);
			this.tscPanel.TopToolStripPanel.PerformLayout();
			this.tscPanel.ResumeLayout(false);
			this.tscPanel.PerformLayout();
			this.tsEdit.ResumeLayout(false);
			this.tsEdit.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem miQuit;
		private System.Windows.Forms.MenuItem menuView;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MainMenu mmMain;
		private System.Windows.Forms.MenuItem miBarHori;
		private System.Windows.Forms.MenuItem miOn;
		private System.Windows.Forms.MenuItem miOff;
		private System.Windows.Forms.MenuItem menuAnimation;
		private System.Windows.Forms.MenuItem menuHelp;
		private System.Windows.Forms.MenuItem menuEdit;
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
		private System.Windows.Forms.ToolStripStatusLabel tsslMapLabel;
		private System.Windows.Forms.ToolStripStatusLabel tsslDimensions;
		private System.Windows.Forms.ToolStripStatusLabel tsslPosition;
		private System.Windows.Forms.ToolStripContainer tscPanel;
		private System.Windows.Forms.ToolStrip tsEdit;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.ToolStripButton tsbSelectionBox;
		private System.Windows.Forms.ToolStripButton tsbZoomIn;
		private System.Windows.Forms.ToolStripButton tsbZoomOut;
		private System.Windows.Forms.ToolStripButton tsbAutoZoom;
		private System.Windows.Forms.ToolStripStatusLabel tsslScale;
		private System.Windows.Forms.MenuItem miRegenOccult;

		#endregion
	}
}
