using System;


namespace MapView
{
	public class LoadingForm
		:
		System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar _progress;
		private System.ComponentModel.Container components = null;


		public LoadingForm()
		{
			InitializeComponent();
		}


		public void Update(int cur, int total)
		{
			_progress.Value = cur;
			_progress.Maximum = total;
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
			this._progress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// progress
			// 
			this._progress.Dock = System.Windows.Forms.DockStyle.Fill;
			this._progress.Location = new System.Drawing.Point(0, 0);
			this._progress.Name = "progress";
			this._progress.Size = new System.Drawing.Size(292, 27);
			this._progress.TabIndex = 0;
			// 
			// LoadingForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 27);
			this.ControlBox = false;
			this.Controls.Add(this._progress);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "LoadingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);
		}
		#endregion
	}
}
