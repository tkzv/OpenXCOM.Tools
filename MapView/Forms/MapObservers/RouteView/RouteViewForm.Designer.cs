namespace MapView.Forms.MapObservers.RouteViews
{
	partial class RouteViewForm
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
			this.controlRouteView = new global::MapView.Forms.MapObservers.RouteViews.RouteView();
			this.SuspendLayout();
			// 
			// controlRouteView
			// 
			this.controlRouteView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlRouteView.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.controlRouteView.Location = new System.Drawing.Point(0, 0);
			this.controlRouteView.Name = "controlRouteView";
			this.controlRouteView.Size = new System.Drawing.Size(632, 454);
			this.controlRouteView.TabIndex = 0;
			// 
			// RouteViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.controlRouteView);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "RouteViewForm";
			this.ShowInTaskbar = false;
			this.Text = "Waypoint View";
			this.ResumeLayout(false);

		}

		#endregion

		private RouteView controlRouteView;
	}
}
