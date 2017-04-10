using System;
using System.Windows.Forms;


namespace MapView
{
	internal sealed class SubsetForm
		:
			Form
	{
		private string _label;

		public SubsetForm()
		{
			InitializeComponent();
		}

		public string SubsetLabel
		{
			get { return _label; }
		}

		private void OnOkClick(object sender, EventArgs e)
		{
			_label = tbLabel.Text;
			Close();
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
			this.lblSubset = new Label();
			this.tbLabel = new TextBox();
			this.btnOk = new Button();
			this.SuspendLayout();
			// 
			// lblSubset
			// 
			this.lblSubset.Location = new System.Drawing.Point(5, 5);
			this.lblSubset.Name = "lblSubset";
			this.lblSubset.Size = new System.Drawing.Size(80, 15);
			this.lblSubset.TabIndex = 0;
			this.lblSubset.Text = "Subset Label";
			// 
			// tbLabel
			// 
			this.tbLabel.Location = new System.Drawing.Point(5, 20);
			this.tbLabel.Name = "tbLabel";
			this.tbLabel.Size = new System.Drawing.Size(235, 19);
			this.tbLabel.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(80, 45);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			// 
			// SubsetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(244, 76);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tbLabel);
			this.Controls.Add(this.lblSubset);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SubsetForm";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "New Subset";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.Container components = null;

		private Label lblSubset;
		private TextBox tbLabel;
		private Button btnOk;
	}
}
