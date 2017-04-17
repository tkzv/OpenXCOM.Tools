using System;


namespace PckView
{
	partial class Help
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.lblHelp = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblHelp
			// 
			this.lblHelp.Location = new System.Drawing.Point(5, 0);
			this.lblHelp.Name = "lblHelp";
			this.lblHelp.Size = new System.Drawing.Size(345, 175);
			this.lblHelp.TabIndex = 0;
			// 
			// Help
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(354, 176);
			this.Controls.Add(this.lblHelp);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Help";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Help";
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label lblHelp;
	}
}
