namespace MapView
{
	partial class MapTreeTilesetInputBox
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
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblHeaderGroup = new System.Windows.Forms.Label();
			this.tbTileset = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelTop = new System.Windows.Forms.Panel();
			this.gbTerrains = new System.Windows.Forms.GroupBox();
			this.lbTerrainsAllocated = new System.Windows.Forms.ListBox();
			this.lbTerrainsAvailable = new System.Windows.Forms.ListBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblAllocated = new System.Windows.Forms.Label();
			this.lblAvailable = new System.Windows.Forms.Label();
			this.pSpacer = new System.Windows.Forms.Panel();
			this.btnMoveLeft = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveRight = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.gbTileset = new System.Windows.Forms.GroupBox();
			this.btnCreateMap = new System.Windows.Forms.Button();
			this.btnFindDirectory = new System.Windows.Forms.Button();
			this.lblTilesetMap = new System.Windows.Forms.Label();
			this.btnFindTileset = new System.Windows.Forms.Button();
			this.lblPathCurrent = new System.Windows.Forms.Label();
			this.lblTilesetPath = new System.Windows.Forms.Label();
			this.gbHeader = new System.Windows.Forms.GroupBox();
			this.lblTilesetCurrent = new System.Windows.Forms.Label();
			this.lblHeaderTileset = new System.Windows.Forms.Label();
			this.lblGroupCurrent = new System.Windows.Forms.Label();
			this.lblHeaderCategory = new System.Windows.Forms.Label();
			this.lblCategoryCurrent = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panelBottom.SuspendLayout();
			this.panelTop.SuspendLayout();
			this.gbTerrains.SuspendLayout();
			this.panel1.SuspendLayout();
			this.pSpacer.SuspendLayout();
			this.gbTileset.SuspendLayout();
			this.gbHeader.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOk.Location = new System.Drawing.Point(155, 0);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += new System.EventHandler(this.OnAcceptClick);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(240, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 25);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// lblHeaderGroup
			// 
			this.lblHeaderGroup.Location = new System.Drawing.Point(10, 15);
			this.lblHeaderGroup.Name = "lblHeaderGroup";
			this.lblHeaderGroup.Size = new System.Drawing.Size(65, 15);
			this.lblHeaderGroup.TabIndex = 2;
			this.lblHeaderGroup.Text = "GROUP";
			// 
			// tbTileset
			// 
			this.tbTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTileset.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.tbTileset.Location = new System.Drawing.Point(65, 15);
			this.tbTileset.Name = "tbTileset";
			this.tbTileset.Size = new System.Drawing.Size(315, 19);
			this.tbTileset.TabIndex = 3;
			this.tbTileset.TextChanged += new System.EventHandler(this.OnTilesetTextChanged);
			this.tbTileset.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnTilesetLabelKeyUp);
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.btnOk);
			this.panelBottom.Controls.Add(this.btnCancel);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 445);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(472, 29);
			this.panelBottom.TabIndex = 4;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.gbTerrains);
			this.panelTop.Controls.Add(this.gbTileset);
			this.panelTop.Controls.Add(this.gbHeader);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(472, 445);
			this.panelTop.TabIndex = 5;
			// 
			// gbTerrains
			// 
			this.gbTerrains.Controls.Add(this.lbTerrainsAllocated);
			this.gbTerrains.Controls.Add(this.lbTerrainsAvailable);
			this.gbTerrains.Controls.Add(this.panel1);
			this.gbTerrains.Controls.Add(this.pSpacer);
			this.gbTerrains.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTerrains.Location = new System.Drawing.Point(0, 110);
			this.gbTerrains.Name = "gbTerrains";
			this.gbTerrains.Size = new System.Drawing.Size(472, 335);
			this.gbTerrains.TabIndex = 2;
			this.gbTerrains.TabStop = false;
			this.gbTerrains.Text = "Terrains";
			// 
			// lbTerrainsAllocated
			// 
			this.lbTerrainsAllocated.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbTerrainsAllocated.FormattingEnabled = true;
			this.lbTerrainsAllocated.ItemHeight = 12;
			this.lbTerrainsAllocated.Location = new System.Drawing.Point(3, 30);
			this.lbTerrainsAllocated.Name = "lbTerrainsAllocated";
			this.lbTerrainsAllocated.Size = new System.Drawing.Size(202, 302);
			this.lbTerrainsAllocated.TabIndex = 0;
			this.lbTerrainsAllocated.SelectedIndexChanged += new System.EventHandler(this.OnAllocatedIndexChanged);
			// 
			// lbTerrainsAvailable
			// 
			this.lbTerrainsAvailable.Dock = System.Windows.Forms.DockStyle.Right;
			this.lbTerrainsAvailable.FormattingEnabled = true;
			this.lbTerrainsAvailable.ItemHeight = 12;
			this.lbTerrainsAvailable.Location = new System.Drawing.Point(270, 30);
			this.lbTerrainsAvailable.Name = "lbTerrainsAvailable";
			this.lbTerrainsAvailable.Size = new System.Drawing.Size(199, 302);
			this.lbTerrainsAvailable.TabIndex = 1;
			this.lbTerrainsAvailable.SelectedIndexChanged += new System.EventHandler(this.OnAvailableIndexChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lblAllocated);
			this.panel1.Controls.Add(this.lblAvailable);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 15);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(466, 15);
			this.panel1.TabIndex = 9;
			// 
			// lblAllocated
			// 
			this.lblAllocated.Location = new System.Drawing.Point(145, 0);
			this.lblAllocated.Name = "lblAllocated";
			this.lblAllocated.Size = new System.Drawing.Size(55, 15);
			this.lblAllocated.TabIndex = 7;
			this.lblAllocated.Text = "allocated";
			this.lblAllocated.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblAvailable
			// 
			this.lblAvailable.Location = new System.Drawing.Point(270, 0);
			this.lblAvailable.Name = "lblAvailable";
			this.lblAvailable.Size = new System.Drawing.Size(55, 15);
			this.lblAvailable.TabIndex = 8;
			this.lblAvailable.Text = "available";
			// 
			// pSpacer
			// 
			this.pSpacer.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pSpacer.Controls.Add(this.btnMoveLeft);
			this.pSpacer.Controls.Add(this.btnMoveDown);
			this.pSpacer.Controls.Add(this.btnMoveRight);
			this.pSpacer.Controls.Add(this.btnMoveUp);
			this.pSpacer.Location = new System.Drawing.Point(210, 30);
			this.pSpacer.Name = "pSpacer";
			this.pSpacer.Size = new System.Drawing.Size(55, 110);
			this.pSpacer.TabIndex = 6;
			// 
			// btnMoveLeft
			// 
			this.btnMoveLeft.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnMoveLeft.Enabled = false;
			this.btnMoveLeft.Location = new System.Drawing.Point(5, 5);
			this.btnMoveLeft.Name = "btnMoveLeft";
			this.btnMoveLeft.Size = new System.Drawing.Size(45, 23);
			this.btnMoveLeft.TabIndex = 2;
			this.btnMoveLeft.Text = "Left";
			this.btnMoveLeft.UseVisualStyleBackColor = true;
			this.btnMoveLeft.Click += new System.EventHandler(this.OnMoveLeftClick);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnMoveDown.Enabled = false;
			this.btnMoveDown.Location = new System.Drawing.Point(5, 80);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(45, 23);
			this.btnMoveDown.TabIndex = 5;
			this.btnMoveDown.Text = "Down";
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.OnMoveDownClick);
			// 
			// btnMoveRight
			// 
			this.btnMoveRight.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnMoveRight.Enabled = false;
			this.btnMoveRight.Location = new System.Drawing.Point(5, 30);
			this.btnMoveRight.Name = "btnMoveRight";
			this.btnMoveRight.Size = new System.Drawing.Size(45, 23);
			this.btnMoveRight.TabIndex = 3;
			this.btnMoveRight.Text = "Right";
			this.btnMoveRight.UseVisualStyleBackColor = true;
			this.btnMoveRight.Click += new System.EventHandler(this.OnMoveRightClick);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnMoveUp.Enabled = false;
			this.btnMoveUp.Location = new System.Drawing.Point(5, 55);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(45, 23);
			this.btnMoveUp.TabIndex = 4;
			this.btnMoveUp.Text = "Up";
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.OnMoveUpClick);
			// 
			// gbTileset
			// 
			this.gbTileset.Controls.Add(this.btnCreateMap);
			this.gbTileset.Controls.Add(this.btnFindDirectory);
			this.gbTileset.Controls.Add(this.tbTileset);
			this.gbTileset.Controls.Add(this.lblTilesetMap);
			this.gbTileset.Controls.Add(this.btnFindTileset);
			this.gbTileset.Controls.Add(this.lblPathCurrent);
			this.gbTileset.Controls.Add(this.lblTilesetPath);
			this.gbTileset.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbTileset.Location = new System.Drawing.Point(0, 50);
			this.gbTileset.Name = "gbTileset";
			this.gbTileset.Size = new System.Drawing.Size(472, 60);
			this.gbTileset.TabIndex = 1;
			this.gbTileset.TabStop = false;
			this.gbTileset.Text = "Tileset";
			// 
			// btnCreateMap
			// 
			this.btnCreateMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCreateMap.Location = new System.Drawing.Point(385, 15);
			this.btnCreateMap.Name = "btnCreateMap";
			this.btnCreateMap.Size = new System.Drawing.Size(50, 20);
			this.btnCreateMap.TabIndex = 12;
			this.btnCreateMap.Text = "Create";
			this.toolTip1.SetToolTip(this.btnCreateMap, "Map must be created before terrains can be added.");
			this.btnCreateMap.UseVisualStyleBackColor = true;
			this.btnCreateMap.Click += new System.EventHandler(this.OnCreateTilesetClick);
			// 
			// btnFindDirectory
			// 
			this.btnFindDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindDirectory.Location = new System.Drawing.Point(440, 35);
			this.btnFindDirectory.Name = "btnFindDirectory";
			this.btnFindDirectory.Size = new System.Drawing.Size(25, 20);
			this.btnFindDirectory.TabIndex = 11;
			this.btnFindDirectory.Text = "...";
			this.btnFindDirectory.UseVisualStyleBackColor = true;
			this.btnFindDirectory.Click += new System.EventHandler(this.OnFindDirectoryClick);
			// 
			// lblTilesetMap
			// 
			this.lblTilesetMap.Location = new System.Drawing.Point(10, 15);
			this.lblTilesetMap.Name = "lblTilesetMap";
			this.lblTilesetMap.Size = new System.Drawing.Size(50, 15);
			this.lblTilesetMap.TabIndex = 7;
			this.lblTilesetMap.Text = "MAP";
			// 
			// btnFindTileset
			// 
			this.btnFindTileset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindTileset.Location = new System.Drawing.Point(440, 15);
			this.btnFindTileset.Name = "btnFindTileset";
			this.btnFindTileset.Size = new System.Drawing.Size(25, 20);
			this.btnFindTileset.TabIndex = 8;
			this.btnFindTileset.Text = "...";
			this.btnFindTileset.UseVisualStyleBackColor = true;
			this.btnFindTileset.Click += new System.EventHandler(this.OnFindTilesetClick);
			// 
			// lblPathCurrent
			// 
			this.lblPathCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblPathCurrent.Location = new System.Drawing.Point(65, 35);
			this.lblPathCurrent.Name = "lblPathCurrent";
			this.lblPathCurrent.Size = new System.Drawing.Size(370, 15);
			this.lblPathCurrent.TabIndex = 10;
			this.lblPathCurrent.Text = "lblPathCurrent";
			// 
			// lblTilesetPath
			// 
			this.lblTilesetPath.Location = new System.Drawing.Point(10, 35);
			this.lblTilesetPath.Name = "lblTilesetPath";
			this.lblTilesetPath.Size = new System.Drawing.Size(50, 15);
			this.lblTilesetPath.TabIndex = 9;
			this.lblTilesetPath.Text = "PATH";
			// 
			// gbHeader
			// 
			this.gbHeader.Controls.Add(this.lblTilesetCurrent);
			this.gbHeader.Controls.Add(this.lblHeaderTileset);
			this.gbHeader.Controls.Add(this.lblGroupCurrent);
			this.gbHeader.Controls.Add(this.lblHeaderGroup);
			this.gbHeader.Controls.Add(this.lblHeaderCategory);
			this.gbHeader.Controls.Add(this.lblCategoryCurrent);
			this.gbHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbHeader.Location = new System.Drawing.Point(0, 0);
			this.gbHeader.Name = "gbHeader";
			this.gbHeader.Size = new System.Drawing.Size(472, 50);
			this.gbHeader.TabIndex = 0;
			this.gbHeader.TabStop = false;
			this.gbHeader.Text = "Jurisdiction";
			// 
			// lblTilesetCurrent
			// 
			this.lblTilesetCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTilesetCurrent.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTilesetCurrent.ForeColor = System.Drawing.SystemColors.Highlight;
			this.lblTilesetCurrent.Location = new System.Drawing.Point(295, 30);
			this.lblTilesetCurrent.Name = "lblTilesetCurrent";
			this.lblTilesetCurrent.Size = new System.Drawing.Size(170, 15);
			this.lblTilesetCurrent.TabIndex = 8;
			this.lblTilesetCurrent.Text = "lblTilesetCurrent";
			this.lblTilesetCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblHeaderTileset
			// 
			this.lblHeaderTileset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblHeaderTileset.Location = new System.Drawing.Point(415, 15);
			this.lblHeaderTileset.Name = "lblHeaderTileset";
			this.lblHeaderTileset.Size = new System.Drawing.Size(50, 15);
			this.lblHeaderTileset.TabIndex = 7;
			this.lblHeaderTileset.Text = "TILESET";
			this.lblHeaderTileset.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblGroupCurrent
			// 
			this.lblGroupCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblGroupCurrent.Location = new System.Drawing.Point(90, 15);
			this.lblGroupCurrent.Name = "lblGroupCurrent";
			this.lblGroupCurrent.Size = new System.Drawing.Size(200, 15);
			this.lblGroupCurrent.TabIndex = 5;
			this.lblGroupCurrent.Text = "lblGroupCurrent";
			// 
			// lblHeaderCategory
			// 
			this.lblHeaderCategory.Location = new System.Drawing.Point(10, 30);
			this.lblHeaderCategory.Name = "lblHeaderCategory";
			this.lblHeaderCategory.Size = new System.Drawing.Size(65, 15);
			this.lblHeaderCategory.TabIndex = 4;
			this.lblHeaderCategory.Text = "CATEGORY";
			// 
			// lblCategoryCurrent
			// 
			this.lblCategoryCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblCategoryCurrent.Location = new System.Drawing.Point(90, 30);
			this.lblCategoryCurrent.Name = "lblCategoryCurrent";
			this.lblCategoryCurrent.Size = new System.Drawing.Size(200, 15);
			this.lblCategoryCurrent.TabIndex = 6;
			this.lblCategoryCurrent.Text = "lblCategoryCurrent";
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.UseAnimation = false;
			// 
			// MapTreeTilesetInputBox
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(472, 474);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.panelBottom);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 350);
			this.Name = "MapTreeTilesetInputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.panelBottom.ResumeLayout(false);
			this.panelTop.ResumeLayout(false);
			this.gbTerrains.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.pSpacer.ResumeLayout(false);
			this.gbTileset.ResumeLayout(false);
			this.gbTileset.PerformLayout();
			this.gbHeader.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblHeaderGroup;
		private System.Windows.Forms.TextBox tbTileset;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label lblCategoryCurrent;
		private System.Windows.Forms.Label lblGroupCurrent;
		private System.Windows.Forms.Label lblHeaderCategory;
		private System.Windows.Forms.Button btnFindTileset;
		private System.Windows.Forms.Label lblTilesetMap;
		private System.Windows.Forms.Label lblPathCurrent;
		private System.Windows.Forms.Label lblTilesetPath;
		private System.Windows.Forms.GroupBox gbTerrains;
		private System.Windows.Forms.GroupBox gbHeader;
		private System.Windows.Forms.GroupBox gbTileset;
		private System.Windows.Forms.ListBox lbTerrainsAvailable;
		private System.Windows.Forms.ListBox lbTerrainsAllocated;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.Button btnMoveRight;
		private System.Windows.Forms.Button btnMoveLeft;
		private System.Windows.Forms.Panel pSpacer;
		private System.Windows.Forms.Button btnFindDirectory;
		private System.Windows.Forms.Button btnCreateMap;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblAvailable;
		private System.Windows.Forms.Label lblAllocated;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblTilesetCurrent;
		private System.Windows.Forms.Label lblHeaderTileset;
	}
}
