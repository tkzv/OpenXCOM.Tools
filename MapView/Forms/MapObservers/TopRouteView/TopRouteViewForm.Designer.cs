using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MapObservers.TileViews
{
	partial class TopRouteViewForm
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
			this.controlTopView = new global::MapView.Forms.MapObservers.TopViews.TopView();
			this.controlRouteView = new global::MapView.Forms.MapObservers.RouteViews.RouteView();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// controlTopView
			// 
			this.controlTopView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlTopView.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.controlTopView.Location = new System.Drawing.Point(3, 3);
			this.controlTopView.Name = "controlTopView";
			this.controlTopView.Size = new System.Drawing.Size(618, 423);
			this.controlTopView.TabIndex = 0;
			// 
			// controlRouteView
			// 
			this.controlRouteView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlRouteView.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.controlRouteView.Location = new System.Drawing.Point(3, 3);
			this.controlRouteView.Name = "controlRouteView";
			this.controlRouteView.Size = new System.Drawing.Size(618, 423);
			this.controlRouteView.TabIndex = 0;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(632, 454);
			this.tabControl.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.controlTopView);
			this.tabPage1.Location = new System.Drawing.Point(4, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(624, 429);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Top View";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.controlRouteView);
			this.tabPage2.Location = new System.Drawing.Point(4, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(624, 429);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Route View";
			// 
			// TopRouteView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.tabControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "TopRouteView";
			this.ShowInTaskbar = false;
			this.Text = "Top/Route View";
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private TopView controlTopView;
		private RouteView controlRouteView;

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
	}
}
