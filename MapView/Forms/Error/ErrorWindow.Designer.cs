namespace MapView.Forms.XCError
{
	partial class ErrorWindow
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
			this.lblHead = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.gbDetails = new System.Windows.Forms.GroupBox();
			this.tbDetails = new System.Windows.Forms.TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.gbDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblHead
			// 
			this.lblHead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblHead.Font = new System.Drawing.Font("Comic Sans MS", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHead.Location = new System.Drawing.Point(8, 0);
			this.lblHead.Name = "lblHead";
			this.lblHead.Size = new System.Drawing.Size(775, 148);
			this.lblHead.TabIndex = 0;
			this.lblHead.Text = "Fuck !\r\n\r\noh sry";
			this.lblHead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.gbDetails);
			this.panel1.Location = new System.Drawing.Point(12, 148);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(768, 370);
			this.panel1.TabIndex = 1;
			// 
			// gbDetails
			// 
			this.gbDetails.Controls.Add(this.tbDetails);
			this.gbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbDetails.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbDetails.Location = new System.Drawing.Point(0, 0);
			this.gbDetails.Margin = new System.Windows.Forms.Padding(0);
			this.gbDetails.Name = "gbDetails";
			this.gbDetails.Size = new System.Drawing.Size(768, 370);
			this.gbDetails.TabIndex = 0;
			this.gbDetails.TabStop = false;
			this.gbDetails.Text = "Error Details";
			// 
			// tbDetails
			// 
			this.tbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbDetails.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbDetails.Location = new System.Drawing.Point(3, 15);
			this.tbDetails.Multiline = true;
			this.tbDetails.Name = "tbDetails";
			this.tbDetails.ReadOnly = true;
			this.tbDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDetails.Size = new System.Drawing.Size(762, 352);
			this.tbDetails.TabIndex = 1;
			this.tbDetails.Text = "tbDetails";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(631, 530);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(152, 40);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// ErrorWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 574);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblHead);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ErrorWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "oop Exception";
			this.Load += new System.EventHandler(this.OnLoad);
			this.panel1.ResumeLayout(false);
			this.gbDetails.ResumeLayout(false);
			this.gbDetails.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblHead;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox gbDetails;
		private System.Windows.Forms.TextBox tbDetails;
		private System.Windows.Forms.Button btnClose;
	}
}
