using System;
using System.Windows.Forms;


namespace MapView
{
	public class TilesetForm
		:
			Form
	{
		public TilesetForm()
		{
			InitializeComponent();
			_label      =
			_pathMaps   =
			_pathRoutes = null;	// NOTE: whynot 'blanksPath' also
		}						// probably because they all default to null.

		private string _label, _pathMaps, _pathRoutes, _pathBlanks;
		public string TilesetLabel
		{
			get { return _label; }
		}

		public string MapsPath
		{
			get { return _pathMaps; }
		}

		public string RoutesPath
		{
			get { return _pathRoutes; }
		}

		public string BlanksPath
		{
			get { return _pathBlanks; }
		}

		private void OnOkClick(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(txtTileset.Text))
			{
//				Dialog.ShowDialog(this, "You must specify a map name");
				MessageBox.Show(
							this,
							"You must specify a MAP name",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (String.IsNullOrEmpty(txtMap.Text))
			{
//				Dialog.ShowDialog(this, "You must have a map path");
				MessageBox.Show(
							this,
							"You must have a MAP path",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (String.IsNullOrEmpty(txtRmp.Text))
			{
				MessageBox.Show(
							this,
							"You must have a RMP path",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (String.IsNullOrEmpty(txtBlank.Text))
			{
				MessageBox.Show(
							this,
							"You must have a BLANKS path",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (!System.IO.Directory.Exists(txtMap.Text))
			{
				MessageBox.Show(
							this,
							"MAP directory " + txtMap.Text + " does not exist",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (!System.IO.Directory.Exists(txtRmp.Text))
			{
				MessageBox.Show(
							this,
							"RMP directory " + txtRmp.Text + " does not exist", "Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			if (!System.IO.Directory.Exists(txtBlank.Text))
			{
				MessageBox.Show(
							this,
							"BLANKS directory " + txtBlank.Text + " does not exist",
							"Err..",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				return;
			}

			_label = txtTileset.Text;
			_pathMaps = txtMap.Text;
			_pathRoutes = txtRmp.Text;
			_pathBlanks = txtBlank.Text;

			Close();
		}

		private void OnFindMapsClick(object sender, System.EventArgs e)
		{
			var fs = new FolderBrowserDialog();
			fs.Description = "Find MAP directory";
			if (fs.ShowDialog(this) == DialogResult.OK)
				txtMap.Text = fs.SelectedPath;
		}

		private void OnFindRoutesClick(object sender, System.EventArgs e)
		{
			var fs = new FolderBrowserDialog();
			fs.Description = "Find RMP directory";
			if (fs.ShowDialog(this) == DialogResult.OK)
				txtRmp.Text=fs.SelectedPath;
		}

		private void OnFindBlanksClick(object sender, System.EventArgs e)
		{
			var fs = new FolderBrowserDialog();
			fs.Description = "Find BLANKS directory";
			if (fs.ShowDialog(this) == DialogResult.OK)
				txtBlank.Text=fs.SelectedPath;
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
			this.label1 = new Label();
			this.txtTileset = new TextBox();
			this.btnOk = new Button();
			this.txtRmp = new TextBox();
			this.label2 = new Label();
			this.txtMap = new TextBox();
			this.label3 = new Label();
			this.btnFindMap = new Button();
			this.btnFindRmp = new Button();
			this.btnFindBlank = new Button();
			this.txtBlank = new TextBox();
			this.label4 = new Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Tileset label";
			// 
			// txtTileset
			// 
			this.txtTileset.Location = new System.Drawing.Point(5, 20);
			this.txtTileset.Name = "txtTileset";
			this.txtTileset.Size = new System.Drawing.Size(305, 20);
			this.txtTileset.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(115, 160);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			// 
			// txtRmp
			// 
			this.txtRmp.Location = new System.Drawing.Point(5, 95);
			this.txtRmp.Name = "txtRmp";
			this.txtRmp.Size = new System.Drawing.Size(265, 20);
			this.txtRmp.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "RMP directory";
			// 
			// txtMap
			// 
			this.txtMap.Location = new System.Drawing.Point(5, 60);
			this.txtMap.Name = "txtMap";
			this.txtMap.Size = new System.Drawing.Size(265, 20);
			this.txtMap.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 45);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "MAP directory";
			// 
			// btnFindMap
			// 
			this.btnFindMap.Location = new System.Drawing.Point(270, 60);
			this.btnFindMap.Name = "btnFindMap";
			this.btnFindMap.Size = new System.Drawing.Size(40, 20);
			this.btnFindMap.TabIndex = 7;
			this.btnFindMap.Text = "Find";
			this.btnFindMap.Click += new System.EventHandler(this.OnFindMapsClick);
			// 
			// btnFindRmp
			// 
			this.btnFindRmp.Location = new System.Drawing.Point(270, 95);
			this.btnFindRmp.Name = "btnFindRmp";
			this.btnFindRmp.Size = new System.Drawing.Size(40, 20);
			this.btnFindRmp.TabIndex = 8;
			this.btnFindRmp.Text = "Find";
			this.btnFindRmp.Click += new System.EventHandler(this.OnFindRoutesClick);
			// 
			// btnFindBlank
			// 
			this.btnFindBlank.Location = new System.Drawing.Point(270, 130);
			this.btnFindBlank.Name = "btnFindBlank";
			this.btnFindBlank.Size = new System.Drawing.Size(40, 20);
			this.btnFindBlank.TabIndex = 11;
			this.btnFindBlank.Text = "Find";
			this.btnFindBlank.Click += new System.EventHandler(this.OnFindBlanksClick);
			// 
			// txtBlank
			// 
			this.txtBlank.Location = new System.Drawing.Point(5, 130);
			this.txtBlank.Name = "txtBlank";
			this.txtBlank.Size = new System.Drawing.Size(265, 20);
			this.txtBlank.TabIndex = 10;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 15);
			this.label4.TabIndex = 9;
			this.label4.Text = "BLANKS directory";
			// 
			// TilesetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(314, 192);
			this.Controls.Add(this.btnFindBlank);
			this.Controls.Add(this.txtBlank);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnFindRmp);
			this.Controls.Add(this.btnFindMap);
			this.Controls.Add(this.txtMap);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtRmp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.txtTileset);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TilesetForm";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "TilesetForm";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion

		private System.ComponentModel.Container components = null;

		private Label label1;
		private TextBox txtTileset;
		private Button btnOk;
		private Label label2;
		private Label label3;
		private TextBox txtRmp;
		private TextBox txtMap;
		private Button btnFindMap;
		private Button btnFindBlank;
		private TextBox txtBlank;
		private Label label4;
		private Button btnFindRmp;
	}
}
