using System;
using System.IO;
using System.Windows.Forms;


namespace MapView
{
	/// <summary>
	/// A generic form providing the user with textual information.
	/// </summary>
	internal sealed class OutputBox
		:
			Form
	{
		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="notice">text that will be shown</param>
		/// <param name="caption">caption for the titlebar</param>
		public OutputBox(string notice, string caption)
		{
			InitializeComponent();
			
			Text = caption;
			lblNotice.Text = notice;
		}
		/// <summary>
		/// Auxiliary constructor.
		/// </summary>
		/// <param name="notice"></param>
		public OutputBox(string notice)
			:
				this(notice, "Output")
		{}




//		private static readonly OutputBox f = new OutputBox();
//		private static DialogResult result = DialogResult.Cancel;
//
//		private static string dir;
//
//
//		private OutputBox()
//		{
//			InitializeComponent();
//		}


//		public static DialogResult Show(string directory)
//		{
//			dir = directory;
//			f.lblNotice.Text = "Directory " + dir + " not found";
//			f.ShowDialog();
//			return result;
//		}
//
//		private void button1_Click(object sender, EventArgs e)
//		{
//			Directory.CreateDirectory(dir);
//			result = DialogResult.OK;
//			this.Close();
//		}
//
//		private void button2_Click(object sender, EventArgs e)
//		{
//			result = DialogResult.Cancel;
//			Close();
//		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose( disposing );
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblNotice = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblNotice
			// 
			this.lblNotice.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblNotice.Location = new System.Drawing.Point(0, 0);
			this.lblNotice.Name = "lblNotice";
			this.lblNotice.Size = new System.Drawing.Size(392, 45);
			this.lblNotice.TabIndex = 0;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(115, 45);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "Ok";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(200, 45);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 25);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// OutputBox
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 74);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblNotice);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(400, 100);
			this.Name = "OutputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
		#endregion

		private Label lblNotice;
		private Button btnOk;
		private Button btnCancel;
	}
}
