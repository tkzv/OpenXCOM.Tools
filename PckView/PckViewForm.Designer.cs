namespace PckView
{
	partial class PckViewForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PckViewForm));
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miFileMenu = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miCompare = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveAs = new System.Windows.Forms.MenuItem();
			this.miSaveDir = new System.Windows.Forms.MenuItem();
			this.miHq2x = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miBytesMenu = new System.Windows.Forms.MenuItem();
			this.miBytes = new System.Windows.Forms.MenuItem();
			this.miTransparentMenu = new System.Windows.Forms.MenuItem();
			this.miTransparent = new System.Windows.Forms.MenuItem();
			this.miHelpMenu = new System.Windows.Forms.MenuItem();
			this.miAbout = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miConsole = new System.Windows.Forms.MenuItem();
			this.miMapViewHelp = new System.Windows.Forms.MenuItem();
			this.sfdBmpSingle = new System.Windows.Forms.SaveFileDialog();
			this.ofdBmp = new System.Windows.Forms.OpenFileDialog();
			this.pViewer = new System.Windows.Forms.Panel();
			this.pMapViewHelp = new System.Windows.Forms.Panel();
			this.btnMapViewHelpOk = new System.Windows.Forms.Button();
			this.lblMapViewHelp = new System.Windows.Forms.Label();
			this.pMapViewHelp.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miFileMenu,
			this.miPaletteMenu,
			this.miBytesMenu,
			this.miTransparentMenu,
			this.miHelpMenu});
			// 
			// miFileMenu
			// 
			this.miFileMenu.Index = 0;
			this.miFileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miOpen,
			this.miCompare,
			this.miSave,
			this.miSaveAs,
			this.miSaveDir,
			this.miHq2x,
			this.miQuit});
			this.miFileMenu.Text = "&File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Text = "&Open";
			this.miOpen.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// miCompare
			// 
			this.miCompare.Index = 1;
			this.miCompare.Text = "&Compare";
			this.miCompare.Visible = false;
			this.miCompare.Click += new System.EventHandler(this.OnCompareClick);
			// 
			// miSave
			// 
			this.miSave.Index = 2;
			this.miSave.Text = "&Save";
			this.miSave.Visible = false;
			this.miSave.Click += new System.EventHandler(this.OnSaveSpritesetClick);
			// 
			// miSaveAs
			// 
			this.miSaveAs.Enabled = false;
			this.miSaveAs.Index = 3;
			this.miSaveAs.Text = "Save &As ...";
			this.miSaveAs.Visible = false;
			this.miSaveAs.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// miSaveDir
			// 
			this.miSaveDir.Index = 4;
			this.miSaveDir.Text = "Save &Image";
			this.miSaveDir.Visible = false;
			this.miSaveDir.Click += new System.EventHandler(this.OnSaveDirectoryClick);
			// 
			// miHq2x
			// 
			this.miHq2x.Index = 5;
			this.miHq2x.Text = "Hq&2x";
			this.miHq2x.Visible = false;
			this.miHq2x.Click += new System.EventHandler(this.OnHq2xClick);
			// 
			// miQuit
			// 
			this.miQuit.Index = 6;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Index = 1;
			this.miPaletteMenu.Text = "&Palette";
			// 
			// miBytesMenu
			// 
			this.miBytesMenu.Enabled = false;
			this.miBytesMenu.Index = 2;
			this.miBytesMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miBytes});
			this.miBytesMenu.Text = "&Bytes";
			// 
			// miBytes
			// 
			this.miBytes.Index = 0;
			this.miBytes.Text = "S&how";
			this.miBytes.Click += new System.EventHandler(this.OnShowBytesClick);
			// 
			// miTransparentMenu
			// 
			this.miTransparentMenu.Enabled = false;
			this.miTransparentMenu.Index = 3;
			this.miTransparentMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miTransparent});
			this.miTransparentMenu.Text = "&Transparency";
			// 
			// miTransparent
			// 
			this.miTransparent.Checked = true;
			this.miTransparent.Index = 0;
			this.miTransparent.Text = "&On";
			this.miTransparent.Click += new System.EventHandler(this.OnTransparencyClick);
			// 
			// miHelpMenu
			// 
			this.miHelpMenu.Index = 4;
			this.miHelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miAbout,
			this.miConsole,
			this.miHelp,
			this.miMapViewHelp});
			this.miHelpMenu.Text = "Help";
			// 
			// miAbout
			// 
			this.miAbout.Index = 0;
			this.miAbout.Text = "About";
			this.miAbout.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// miConsole
			// 
			this.miConsole.Index = 1;
			this.miConsole.Text = "Console";
			this.miConsole.Click += new System.EventHandler(this.OnConsoleClick);
			// 
			// miHelp
			// 
			this.miHelp.Index = 2;
			this.miHelp.Text = "Help";
			this.miHelp.Click += new System.EventHandler(this.OnHelpClick);
			// 
			// miMapViewHelp
			// 
			this.miMapViewHelp.Index = 3;
			this.miMapViewHelp.Text = "MapView Help";
			this.miMapViewHelp.Visible = false;
			this.miMapViewHelp.Click += new System.EventHandler(this.OnMapViewHelpClick);
			// 
			// sfdBmpSingle
			// 
			this.sfdBmpSingle.DefaultExt = "*.bmp";
			this.sfdBmpSingle.Filter = "BMP Files (*.bmp)|*.bmp";
			// 
			// ofdBmp
			// 
			this.ofdBmp.Filter = "8-bit 32x40 bmp (*.bmp)|*.bmp";
			// 
			// pViewer
			// 
			this.pViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pViewer.Location = new System.Drawing.Point(0, 100);
			this.pViewer.Name = "pViewer";
			this.pViewer.Size = new System.Drawing.Size(472, 514);
			this.pViewer.TabIndex = 1;
			// 
			// pMapViewHelp
			// 
			this.pMapViewHelp.Controls.Add(this.btnMapViewHelpOk);
			this.pMapViewHelp.Controls.Add(this.lblMapViewHelp);
			this.pMapViewHelp.Dock = System.Windows.Forms.DockStyle.Top;
			this.pMapViewHelp.Location = new System.Drawing.Point(0, 0);
			this.pMapViewHelp.Name = "pMapViewHelp";
			this.pMapViewHelp.Size = new System.Drawing.Size(472, 100);
			this.pMapViewHelp.TabIndex = 2;
			this.pMapViewHelp.Visible = false;
			// 
			// btnMapViewHelpOk
			// 
			this.btnMapViewHelpOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMapViewHelpOk.Location = new System.Drawing.Point(370, 60);
			this.btnMapViewHelpOk.Name = "btnMapViewHelpOk";
			this.btnMapViewHelpOk.Size = new System.Drawing.Size(97, 34);
			this.btnMapViewHelpOk.TabIndex = 2;
			this.btnMapViewHelpOk.Text = "Got it";
			this.btnMapViewHelpOk.UseVisualStyleBackColor = true;
			this.btnMapViewHelpOk.Click += new System.EventHandler(this.OnMapViewGotItClick);
			// 
			// lblMapViewHelp
			// 
			this.lblMapViewHelp.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMapViewHelp.Location = new System.Drawing.Point(0, 0);
			this.lblMapViewHelp.Name = "lblMapViewHelp";
			this.lblMapViewHelp.Padding = new System.Windows.Forms.Padding(3);
			this.lblMapViewHelp.Size = new System.Drawing.Size(472, 55);
			this.lblMapViewHelp.TabIndex = 1;
			this.lblMapViewHelp.Text = resources.GetString("lblMapViewHelp.Text");
			// 
			// PckViewForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(472, 614);
			this.Controls.Add(this.pViewer);
			this.Controls.Add(this.pMapViewHelp);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Location = new System.Drawing.Point(50, 50);
			this.MaximumSize = new System.Drawing.Size(480, 640);
			this.Menu = this.mmMainMenu;
			this.Name = "PckViewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "PckView";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnPckViewFormClosing);
			this.Shown += new System.EventHandler(this.OnShown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.pMapViewHelp.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.MainMenu mmMainMenu;
		private System.Windows.Forms.MenuItem miFileMenu;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.MenuItem miQuit;
		private System.Windows.Forms.SaveFileDialog sfdBmpSingle;
		private System.Windows.Forms.MenuItem miBytes;
		private System.Windows.Forms.MenuItem miBytesMenu;
		private System.Windows.Forms.MenuItem miTransparent;
		private System.Windows.Forms.OpenFileDialog ofdBmp;
		private System.Windows.Forms.MenuItem miHelpMenu;
		private System.Windows.Forms.MenuItem miAbout;
		private System.Windows.Forms.MenuItem miSaveAs;
		private System.Windows.Forms.MenuItem miTransparentMenu;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miPaletteMenu;
		private System.Windows.Forms.MenuItem miHq2x;
		private System.Windows.Forms.MenuItem miSaveDir;
		private System.Windows.Forms.MenuItem miConsole;
		private System.Windows.Forms.MenuItem miCompare;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.Panel pViewer;
		private System.Windows.Forms.Panel pMapViewHelp;
		private System.Windows.Forms.Label lblMapViewHelp;
		private System.Windows.Forms.MenuItem miMapViewHelp;
		private System.Windows.Forms.Button btnMapViewHelpOk;
	}
}
