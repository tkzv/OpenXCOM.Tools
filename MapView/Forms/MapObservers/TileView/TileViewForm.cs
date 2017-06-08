﻿using System;
using System.Drawing;
using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed class TileViewForm
		:
			Form,
			IMapObserverProvider
	{
		private TileView TileViewControl;

		internal TileViewForm()
		{
			InitializeComponent();

			Activated += OnActivated;
		}

		/// <summary>
		/// Fires when the form is activated.
		/// </summary>
		private void OnActivated(object sender, EventArgs e)
		{
			TileViewControl.GetSelectedPanel().Focus();
		}

		/// <summary>
		/// Gets TileView as a child of MapObserverControl0.
		/// </summary>
		internal TileView Control
		{
			get { return TileViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return TileViewControl; }
		}

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

		/* The #develop designer is going to delete this:

			TileViewControl = new TileView();

		- so copy it back into InitializeComponent() */

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			TileViewControl = new TileView();
			this.SuspendLayout();
			// 
			// TileViewControl
			// 
			this.TileViewControl.Dock = DockStyle.Fill;
			this.TileViewControl.Font = new Font("Verdana", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			this.TileViewControl.Location = new System.Drawing.Point(0, 0);
			this.TileViewControl.Name = "TileViewControl";
			this.TileViewControl.Size = new Size(632, 454);
			this.TileViewControl.TabIndex = 0;
			// 
			// TileViewForm
			// 
			this.AutoScaleDimensions = new SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.TileViewControl);
			this.Font = new Font("Verdana", 7F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new Size(322, 122);
			this.Name = "TileViewForm";
			this.ShowInTaskbar = false;
			this.Text = "TileView";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
