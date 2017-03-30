namespace MapView
{
	partial class InstallWindow
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.btnCancel = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbUfo
			// 
			this.tbUfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbUfo.Location = new System.Drawing.Point(75, 0);
			this.tbUfo.Name = "tbUfo";
			this.tbUfo.Size = new System.Drawing.Size(365, 19);
			this.tbUfo.TabIndex = 0;
			// 
			// tbTftd
			// 
			this.tbTftd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTftd.Location = new System.Drawing.Point(75, 0);
			this.tbTftd.Name = "tbTftd";
			this.tbTftd.Size = new System.Drawing.Size(365, 19);
			this.tbTftd.TabIndex = 1;
			// 
			// labelUfo
			// 
			this.labelUfo.Location = new System.Drawing.Point(0, 5);
			this.labelUfo.Name = "labelUfo";
			this.labelUfo.Size = new System.Drawing.Size(75, 15);
			this.labelUfo.TabIndex = 2;
			this.labelUfo.Text = "UFO Folder";
			this.labelUfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTftd
			// 
			this.labelTftd.Location = new System.Drawing.Point(0, 5);
			this.labelTftd.Name = "labelTftd";
			this.labelTftd.Size = new System.Drawing.Size(75, 15);
			this.labelTftd.TabIndex = 3;
			this.labelTftd.Text = "TFTD Folder";
			this.labelTftd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnFindUfo
			// 
			this.btnFindUfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindUfo.Location = new System.Drawing.Point(440, 0);
			this.btnFindUfo.Name = "btnFindUfo";
			this.btnFindUfo.Size = new System.Drawing.Size(30, 20);
			this.btnFindUfo.TabIndex = 4;
			this.btnFindUfo.Text = "...";
			this.btnFindUfo.Click += new System.EventHandler(this.btnFindUfo_Click);
			// 
			// btnFindTftd
			// 
			this.btnFindTftd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindTftd.Location = new System.Drawing.Point(440, 0);
			this.btnFindTftd.Name = "btnFindTftd";
			this.btnFindTftd.Size = new System.Drawing.Size(30, 20);
			this.btnFindTftd.TabIndex = 5;
			this.btnFindTftd.Text = "...";
			this.btnFindTftd.Click += new System.EventHandler(this.btnFindTftd_Click);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOk.Location = new System.Drawing.Point(84, 50);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(165, 25);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "the Paths are correct";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tbUfo);
			this.panel1.Controls.Add(this.labelUfo);
			this.panel1.Controls.Add(this.btnFindUfo);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(472, 25);
			this.panel1.TabIndex = 7;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.tbTftd);
			this.panel2.Controls.Add(this.labelTftd);
			this.panel2.Controls.Add(this.btnFindTftd);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 25);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(472, 25);
			this.panel2.TabIndex = 8;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(264, 50);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(165, 25);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// InstallWindow
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(472, 79);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnOk);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(480, 105);
			this.MinimumSize = new System.Drawing.Size(480, 105);
			this.Name = "InstallWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "set Resource folder(s)";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
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
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.FolderBrowserDialog folderBrowser;
		private System.Windows.Forms.Button btnCancel;
	}
}
