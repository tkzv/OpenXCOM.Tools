﻿namespace MapView
{
	partial class ConfigurationForm
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
			this.tbUfo = new System.Windows.Forms.TextBox();
			this.tbTftd = new System.Windows.Forms.TextBox();
			this.labelUfo = new System.Windows.Forms.Label();
			this.labelTftd = new System.Windows.Forms.Label();
			this.btnFindUfo = new System.Windows.Forms.Button();
			this.btnFindTftd = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.pUfo = new System.Windows.Forms.Panel();
			this.pTftd = new System.Windows.Forms.Panel();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pInfo = new System.Windows.Forms.Panel();
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblOptions = new System.Windows.Forms.Label();
			this.cbTilesets = new System.Windows.Forms.CheckBox();
			this.cbResources = new System.Windows.Forms.CheckBox();
			this.pUfo.SuspendLayout();
			this.pTftd.SuspendLayout();
			this.pInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbUfo
			// 
			this.tbUfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbUfo.Location = new System.Drawing.Point(77, 2);
			this.tbUfo.Name = "tbUfo";
			this.tbUfo.Size = new System.Drawing.Size(361, 19);
			this.tbUfo.TabIndex = 0;
			// 
			// tbTftd
			// 
			this.tbTftd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTftd.Location = new System.Drawing.Point(77, 2);
			this.tbTftd.Name = "tbTftd";
			this.tbTftd.Size = new System.Drawing.Size(361, 19);
			this.tbTftd.TabIndex = 1;
			// 
			// labelUfo
			// 
			this.labelUfo.Location = new System.Drawing.Point(5, 5);
			this.labelUfo.Name = "labelUfo";
			this.labelUfo.Size = new System.Drawing.Size(75, 15);
			this.labelUfo.TabIndex = 2;
			this.labelUfo.Text = "UFO Folder";
			this.labelUfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTftd
			// 
			this.labelTftd.Location = new System.Drawing.Point(5, 5);
			this.labelTftd.Name = "labelTftd";
			this.labelTftd.Size = new System.Drawing.Size(75, 15);
			this.labelTftd.TabIndex = 3;
			this.labelTftd.Text = "TFTD Folder";
			this.labelTftd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnFindUfo
			// 
			this.btnFindUfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindUfo.Location = new System.Drawing.Point(438, 2);
			this.btnFindUfo.Name = "btnFindUfo";
			this.btnFindUfo.Size = new System.Drawing.Size(30, 20);
			this.btnFindUfo.TabIndex = 4;
			this.btnFindUfo.Text = "...";
			this.btnFindUfo.Click += new System.EventHandler(this.OnFindUfoClick);
			// 
			// btnFindTftd
			// 
			this.btnFindTftd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindTftd.Location = new System.Drawing.Point(438, 2);
			this.btnFindTftd.Name = "btnFindTftd";
			this.btnFindTftd.Size = new System.Drawing.Size(30, 20);
			this.btnFindTftd.TabIndex = 5;
			this.btnFindTftd.Text = "...";
			this.btnFindTftd.Click += new System.EventHandler(this.OnFindTftdClick);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOk.Location = new System.Drawing.Point(65, 145);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(165, 25);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "the Paths are correct";
			this.btnOk.Click += new System.EventHandler(this.OnAcceptClick);
			// 
			// pUfo
			// 
			this.pUfo.Controls.Add(this.tbUfo);
			this.pUfo.Controls.Add(this.labelUfo);
			this.pUfo.Controls.Add(this.btnFindUfo);
			this.pUfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.pUfo.Location = new System.Drawing.Point(0, 0);
			this.pUfo.Name = "pUfo";
			this.pUfo.Padding = new System.Windows.Forms.Padding(2);
			this.pUfo.Size = new System.Drawing.Size(472, 25);
			this.pUfo.TabIndex = 7;
			// 
			// pTftd
			// 
			this.pTftd.Controls.Add(this.tbTftd);
			this.pTftd.Controls.Add(this.labelTftd);
			this.pTftd.Controls.Add(this.btnFindTftd);
			this.pTftd.Dock = System.Windows.Forms.DockStyle.Top;
			this.pTftd.Location = new System.Drawing.Point(0, 25);
			this.pTftd.Name = "pTftd";
			this.pTftd.Padding = new System.Windows.Forms.Padding(2);
			this.pTftd.Size = new System.Drawing.Size(472, 25);
			this.pTftd.TabIndex = 8;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(245, 145);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(165, 25);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// pInfo
			// 
			this.pInfo.Controls.Add(this.lblInfo);
			this.pInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.pInfo.Location = new System.Drawing.Point(0, 50);
			this.pInfo.Name = "pInfo";
			this.pInfo.Size = new System.Drawing.Size(472, 35);
			this.pInfo.TabIndex = 10;
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Padding = new System.Windows.Forms.Padding(5);
			this.lblInfo.Size = new System.Drawing.Size(472, 35);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "Enter the paths to either or both the UFO and TFTD resource folders. These are th" +
	"e respective parent folder(s) of the MAPS, ROUTES, and TERRAIN folders.";
			// 
			// lblOptions
			// 
			this.lblOptions.Location = new System.Drawing.Point(5, 95);
			this.lblOptions.Name = "lblOptions";
			this.lblOptions.Size = new System.Drawing.Size(50, 15);
			this.lblOptions.TabIndex = 11;
			this.lblOptions.Text = "Options";
			// 
			// cbTilesets
			// 
			this.cbTilesets.Location = new System.Drawing.Point(60, 110);
			this.cbTilesets.Name = "cbTilesets";
			this.cbTilesets.Size = new System.Drawing.Size(405, 25);
			this.cbTilesets.TabIndex = 14;
			this.cbTilesets.Text = "create and/or replace the tilesets configuration file [MapConfig.yml]";
			this.cbTilesets.UseVisualStyleBackColor = true;
			// 
			// cbResources
			// 
			this.cbResources.Location = new System.Drawing.Point(60, 90);
			this.cbResources.Name = "cbResources";
			this.cbResources.Size = new System.Drawing.Size(405, 24);
			this.cbResources.TabIndex = 15;
			this.cbResources.Text = "create and/or replace the default resource file [MapDirectory.yml]";
			this.cbResources.UseVisualStyleBackColor = true;
			// 
			// ConfigurationForm
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(472, 174);
			this.Controls.Add(this.cbResources);
			this.Controls.Add(this.cbTilesets);
			this.Controls.Add(this.lblOptions);
			this.Controls.Add(this.pInfo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.pTftd);
			this.Controls.Add(this.pUfo);
			this.Controls.Add(this.btnOk);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(480, 200);
			this.MinimumSize = new System.Drawing.Size(480, 200);
			this.Name = "ConfigurationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "set Resource folder(s)";
			this.pUfo.ResumeLayout(false);
			this.pUfo.PerformLayout();
			this.pTftd.ResumeLayout(false);
			this.pTftd.PerformLayout();
			this.pInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label labelUfo;
		private System.Windows.Forms.Label labelTftd;
		private System.Windows.Forms.Button btnFindUfo;
		private System.Windows.Forms.Button btnFindTftd;
		private System.Windows.Forms.TextBox tbUfo;
		private System.Windows.Forms.TextBox tbTftd;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Panel pUfo;
		private System.Windows.Forms.Panel pTftd;
		private System.Windows.Forms.FolderBrowserDialog folderBrowser;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Panel pInfo;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblOptions;
		private System.Windows.Forms.CheckBox cbTilesets;
		private System.Windows.Forms.CheckBox cbResources;
	}
}
