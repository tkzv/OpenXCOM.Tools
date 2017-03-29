namespace DSShared.Windows
{
	partial class InputBox
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
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblNotice = new System.Windows.Forms.Label();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelBottom.SuspendLayout();
			this.panelTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(90, 0);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Ok";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(185, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 25);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// lblNotice
			// 
			this.lblNotice.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblNotice.Location = new System.Drawing.Point(0, 0);
			this.lblNotice.Name = "lblNotice";
			this.lblNotice.Size = new System.Drawing.Size(352, 22);
			this.lblNotice.TabIndex = 2;
			this.lblNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbInput
			// 
			this.tbInput.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbInput.Location = new System.Drawing.Point(0, 22);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(352, 19);
			this.tbInput.TabIndex = 3;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.btnOk);
			this.panelBottom.Controls.Add(this.btnCancel);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 44);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(352, 30);
			this.panelBottom.TabIndex = 4;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.tbInput);
			this.panelTop.Controls.Add(this.lblNotice);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(352, 44);
			this.panelTop.TabIndex = 5;
			// 
			// InputBox
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(352, 74);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.panelBottom);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 100);
			this.Name = "InputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.panelBottom.ResumeLayout(false);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblNotice;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelTop;
	}
}
