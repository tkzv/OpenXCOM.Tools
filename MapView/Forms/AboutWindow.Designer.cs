namespace MapView
{
	partial class AboutWindow
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.MoveTimer = new System.Windows.Forms.Timer(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 90);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(305, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "AUTHORED - Ben Ratzlaff";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 105);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(305, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "ASSISTED - BladeFireLight / J Farceur";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblVersion
			// 
			this.lblVersion.Location = new System.Drawing.Point(5, 10);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(305, 60);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(305, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "REVISED - TheBigSot";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MoveTimer
			// 
			this.MoveTimer.Enabled = true;
			this.MoveTimer.Interval = 1000;
			this.MoveTimer.Tick += new System.EventHandler(this.MoveTimer_Tick);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 135);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(305, 15);
			this.label4.TabIndex = 3;
			this.label4.Text = "REVISED - kevL";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AboutWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(314, 176);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 200);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 200);
			this.Name = "AboutWindow";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.Shown += new System.EventHandler(this.AboutWindow_Shown);
			this.LocationChanged += new System.EventHandler(this.AboutWindow_LocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyClose);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Timer MoveTimer;
	}
}
