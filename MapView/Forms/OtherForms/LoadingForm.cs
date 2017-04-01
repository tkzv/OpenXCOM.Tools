using System;


namespace MapView
{
	internal sealed class LoadingForm
		:
			System.Windows.Forms.Form
	{
		public LoadingForm()
		{
			InitializeComponent();
		}


		public void HandleProgress(int cur, int total)
		{
			pbProgress.Value = cur;
			pbProgress.Maximum = total;
		}


		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;

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
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// pbProgress
			// 
			this.pbProgress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pbProgress.Location = new System.Drawing.Point(0, 0);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(290, 20);
			this.pbProgress.TabIndex = 0;
			// 
			// LoadingForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(290, 20);
			this.ControlBox = false;
			this.Controls.Add(this.pbProgress);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoadingForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ProgressBar pbProgress;
	}
}
