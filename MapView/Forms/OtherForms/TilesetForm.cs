using System;
using System.Windows.Forms;


namespace MapView
{
	internal sealed class TilesetForm
		:
			Form
	{
		internal TilesetForm()
		{
			InitializeComponent();
		}

		private string _label;
		internal string TilesetLabel
		{
			get { return _label; }
		}

		private string _pathMaps;
		internal string MapsPath
		{
			get { return _pathMaps; }
		}

		private string _pathRoutes;
		internal string RoutesPath
		{
			get { return _pathRoutes; }
		}

		private string _pathOccults;
		internal string OccultsPath
		{
			get { return _pathOccults; }
		}

		private void OnOkClick(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(tbLabel.Text))
			{
				MessageBox.Show(
							this,
							"You must specify a label.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (String.IsNullOrEmpty(tbMaps.Text))
			{
				MessageBox.Show(
							this,
							"You must have a MAP path.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (String.IsNullOrEmpty(tbRoutes.Text))
			{
				MessageBox.Show(
							this,
							"You must have a RMP path.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (String.IsNullOrEmpty(tbOccults.Text))
			{
				MessageBox.Show(
							this,
							"You must have an OTD path.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (!System.IO.Directory.Exists(tbMaps.Text))
			{
				MessageBox.Show(
							this,
							"MAP directory " + tbMaps.Text + " does not exist.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (!System.IO.Directory.Exists(tbRoutes.Text))
			{
				MessageBox.Show(
							this,
							"RMP directory " + tbRoutes.Text + " does not exist.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else if (!System.IO.Directory.Exists(tbOccults.Text))
			{
				MessageBox.Show(
							this,
							"OTD directory " + tbOccults.Text + " does not exist.",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else
			{
				_label       = tbLabel.Text;
				_pathMaps    = tbMaps.Text;
				_pathRoutes  = tbRoutes.Text;
				_pathOccults = tbOccults.Text;

				Close();
			}
		}

		private void OnFindMapsClick(object sender, System.EventArgs e)
		{
			var f = new FolderBrowserDialog();
			f.Description = "Find MAP directory";
			if (f.ShowDialog(this) == DialogResult.OK)
				tbMaps.Text = f.SelectedPath;
		}

		private void OnFindRoutesClick(object sender, System.EventArgs e)
		{
			var f = new FolderBrowserDialog();
			f.Description = "Find RMP directory";
			if (f.ShowDialog(this) == DialogResult.OK)
				tbRoutes.Text = f.SelectedPath;
		}

		private void OnFindOccultsClick(object sender, System.EventArgs e)
		{
			var f = new FolderBrowserDialog();
			f.Description = "Find OTD directory";
			if (f.ShowDialog(this) == DialogResult.OK)
				tbOccults.Text = f.SelectedPath;
		}


		#region Windows Form Designer generated code
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblLabel = new System.Windows.Forms.Label();
			this.tbLabel = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.tbRoutes = new System.Windows.Forms.TextBox();
			this.lblRoutes = new System.Windows.Forms.Label();
			this.tbMaps = new System.Windows.Forms.TextBox();
			this.lblMaps = new System.Windows.Forms.Label();
			this.btnFindMaps = new System.Windows.Forms.Button();
			this.btnFindRoutes = new System.Windows.Forms.Button();
			this.btnFindOccults = new System.Windows.Forms.Button();
			this.tbOccults = new System.Windows.Forms.TextBox();
			this.lblOccults = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblLabel
			// 
			this.lblLabel.Location = new System.Drawing.Point(10, 15);
			this.lblLabel.Name = "lblLabel";
			this.lblLabel.Size = new System.Drawing.Size(75, 15);
			this.lblLabel.TabIndex = 0;
			this.lblLabel.Text = "Tileset label";
			// 
			// tbLabel
			// 
			this.tbLabel.Location = new System.Drawing.Point(90, 10);
			this.tbLabel.Name = "tbLabel";
			this.tbLabel.Size = new System.Drawing.Size(220, 19);
			this.tbLabel.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(115, 155);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 30);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			// 
			// tbRoutes
			// 
			this.tbRoutes.Location = new System.Drawing.Point(5, 95);
			this.tbRoutes.Name = "tbRoutes";
			this.tbRoutes.Size = new System.Drawing.Size(275, 19);
			this.tbRoutes.TabIndex = 4;
			// 
			// lblRoutes
			// 
			this.lblRoutes.Location = new System.Drawing.Point(5, 80);
			this.lblRoutes.Name = "lblRoutes";
			this.lblRoutes.Size = new System.Drawing.Size(65, 15);
			this.lblRoutes.TabIndex = 3;
			this.lblRoutes.Text = "RMP folder";
			// 
			// tbMaps
			// 
			this.tbMaps.Location = new System.Drawing.Point(5, 60);
			this.tbMaps.Name = "tbMaps";
			this.tbMaps.Size = new System.Drawing.Size(275, 19);
			this.tbMaps.TabIndex = 6;
			// 
			// lblMaps
			// 
			this.lblMaps.Location = new System.Drawing.Point(5, 45);
			this.lblMaps.Name = "lblMaps";
			this.lblMaps.Size = new System.Drawing.Size(65, 15);
			this.lblMaps.TabIndex = 5;
			this.lblMaps.Text = "MAP folder";
			// 
			// btnFindMaps
			// 
			this.btnFindMaps.Location = new System.Drawing.Point(280, 60);
			this.btnFindMaps.Name = "btnFindMaps";
			this.btnFindMaps.Size = new System.Drawing.Size(30, 20);
			this.btnFindMaps.TabIndex = 7;
			this.btnFindMaps.Text = "...";
			this.btnFindMaps.Click += new System.EventHandler(this.OnFindMapsClick);
			// 
			// btnFindRoutes
			// 
			this.btnFindRoutes.Location = new System.Drawing.Point(280, 95);
			this.btnFindRoutes.Name = "btnFindRoutes";
			this.btnFindRoutes.Size = new System.Drawing.Size(30, 20);
			this.btnFindRoutes.TabIndex = 8;
			this.btnFindRoutes.Text = "...";
			this.btnFindRoutes.Click += new System.EventHandler(this.OnFindRoutesClick);
			// 
			// btnFindOccults
			// 
			this.btnFindOccults.Location = new System.Drawing.Point(280, 130);
			this.btnFindOccults.Name = "btnFindOccults";
			this.btnFindOccults.Size = new System.Drawing.Size(30, 20);
			this.btnFindOccults.TabIndex = 11;
			this.btnFindOccults.Text = "...";
			this.btnFindOccults.Click += new System.EventHandler(this.OnFindOccultsClick);
			// 
			// tbOccults
			// 
			this.tbOccults.Location = new System.Drawing.Point(5, 130);
			this.tbOccults.Name = "tbOccults";
			this.tbOccults.Size = new System.Drawing.Size(275, 19);
			this.tbOccults.TabIndex = 10;
			// 
			// lblOccults
			// 
			this.lblOccults.Location = new System.Drawing.Point(5, 115);
			this.lblOccults.Name = "lblOccults";
			this.lblOccults.Size = new System.Drawing.Size(65, 15);
			this.lblOccults.TabIndex = 9;
			this.lblOccults.Text = "OTD folder";
			// 
			// TilesetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(314, 192);
			this.Controls.Add(this.btnFindOccults);
			this.Controls.Add(this.tbOccults);
			this.Controls.Add(this.lblOccults);
			this.Controls.Add(this.btnFindRoutes);
			this.Controls.Add(this.btnFindMaps);
			this.Controls.Add(this.tbMaps);
			this.Controls.Add(this.lblMaps);
			this.Controls.Add(this.tbRoutes);
			this.Controls.Add(this.lblRoutes);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tbLabel);
			this.Controls.Add(this.lblLabel);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TilesetForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Tileset";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.Container components = null;

		private Label lblLabel;
		private TextBox tbLabel;
		private Button btnOk;
		private Label lblRoutes;
		private Label lblMaps;
		private TextBox tbRoutes;
		private TextBox tbMaps;
		private Button btnFindMaps;
		private Button btnFindOccults;
		private TextBox tbOccults;
		private Label lblOccults;
		private Button btnFindRoutes;
	}
}
