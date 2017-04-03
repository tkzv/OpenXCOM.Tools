namespace XCom
{
	partial class ConsoleForm
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
			this.rtbConsole = new System.Windows.Forms.RichTextBox();
			this.msMenuStrip = new System.Windows.Forms.MenuStrip();
			this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miClose = new System.Windows.Forms.ToolStripMenuItem();
			this.msMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// rtbConsole
			// 
			this.rtbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbConsole.Location = new System.Drawing.Point(0, 24);
			this.rtbConsole.Name = "rtbConsole";
			this.rtbConsole.Size = new System.Drawing.Size(492, 250);
			this.rtbConsole.TabIndex = 0;
			this.rtbConsole.Text = "";
			// 
			// msMenuStrip
			// 
			this.msMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiFile});
			this.msMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.msMenuStrip.Name = "msMenuStrip";
			this.msMenuStrip.Size = new System.Drawing.Size(492, 24);
			this.msMenuStrip.TabIndex = 1;
			this.msMenuStrip.Text = "menuStrip1";
			// 
			// tsmiFile
			// 
			this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miClose});
			this.tsmiFile.Name = "tsmiFile";
			this.tsmiFile.Size = new System.Drawing.Size(47, 20);
			this.tsmiFile.Text = "File";
			// 
			// miClose
			// 
			this.miClose.Name = "miClose";
			this.miClose.Size = new System.Drawing.Size(152, 22);
			this.miClose.Text = "Close";
			this.miClose.Click += new System.EventHandler(this.OnCloseClick);
			// 
			// ConsoleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 274);
			this.Controls.Add(this.rtbConsole);
			this.Controls.Add(this.msMenuStrip);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.msMenuStrip;
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "ConsoleForm";
			this.Text = "Output Console";
			this.msMenuStrip.ResumeLayout(false);
			this.msMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtbConsole;
		private System.Windows.Forms.MenuStrip msMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem tsmiFile;
		private System.Windows.Forms.ToolStripMenuItem miClose;
	}
}
