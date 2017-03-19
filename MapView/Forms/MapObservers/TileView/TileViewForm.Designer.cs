namespace MapView.Forms.MapObservers.TileViews
{
	partial class TileViewForm
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
			this.controlTileView = new global::MapView.Forms.MapObservers.TileViews.TileView();
			this.SuspendLayout();
			// 
			// controlTileView
			// 
			this.controlTileView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlTileView.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.controlTileView.Location = new System.Drawing.Point(0, 0);
			this.controlTileView.Name = "controlTileView";
			this.controlTileView.Size = new System.Drawing.Size(472, 334);
			this.controlTileView.TabIndex = 0;
			// 
			// TileViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 334);
			this.Controls.Add(this.controlTileView);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(480, 360);
			this.Name = "TileViewForm";
			this.ShowInTaskbar = false;
			this.Text = "Tile View";
			this.ResumeLayout(false);

		}

		#endregion

		private TileView controlTileView;
	}
}
