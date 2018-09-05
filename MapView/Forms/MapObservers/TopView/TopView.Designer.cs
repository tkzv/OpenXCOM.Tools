namespace MapView.Forms.MapObservers.TopViews
{
	partial class TopView
	{
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

		// The #develop designer is going to delete this. Copy it back in at the
		// top of InitializeComponent().
		/*
			this.quadrants = new QuadrantPanel();
		*/

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.quadrants = new QuadrantPanel();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopView));
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.pMain = new System.Windows.Forms.Panel();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbVisibleQuads = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsddbEdit = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tscPanel.ContentPanel.SuspendLayout();
			this.tscPanel.LeftToolStripPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// quadrants
			// 
			this.quadrants.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.quadrants.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.quadrants.Location = new System.Drawing.Point(0, 410);
			this.quadrants.Name = "quadrants";
			this.quadrants.Size = new System.Drawing.Size(640, 70);
			this.quadrants.TabIndex = 0;
			this.quadrants.Text = "bottom";
			// 
			// tscPanel
			// 
			// 
			// tscPanel.BottomToolStripPanel
			// 
			this.tscPanel.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tscPanel.ContentPanel
			// 
			this.tscPanel.ContentPanel.Controls.Add(this.pMain);
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(609, 360);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tscPanel.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.Controls.Add(this.tsTools);
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Location = new System.Drawing.Point(0, 25);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Size = new System.Drawing.Size(640, 385);
			this.tscPanel.TabIndex = 0;
			// 
			// tscPanel.TopToolStripPanel
			// 
			this.tscPanel.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// pMain
			// 
			this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pMain.Location = new System.Drawing.Point(0, 0);
			this.pMain.Name = "pMain";
			this.pMain.Size = new System.Drawing.Size(609, 360);
			this.pMain.TabIndex = 2;
			// 
			// tsTools
			// 
			this.tsTools.Dock = System.Windows.Forms.DockStyle.None;
			this.tsTools.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsTools.Location = new System.Drawing.Point(0, 3);
			this.tsTools.Name = "tsTools";
			this.tsTools.Padding = new System.Windows.Forms.Padding(0);
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(31, 30);
			this.tsTools.TabIndex = 1;
			this.tsTools.Text = "tsTools";
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbVisibleQuads,
			this.tsddbEdit});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.Text = "tsMain";
			// 
			// tsddbVisibleQuads
			// 
			this.tsddbVisibleQuads.AutoToolTip = false;
			this.tsddbVisibleQuads.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbVisibleQuads.Image = ((System.Drawing.Image)(resources.GetObject("tsddbVisibleQuads.Image")));
			this.tsddbVisibleQuads.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbVisibleQuads.Name = "tsddbVisibleQuads";
			this.tsddbVisibleQuads.Size = new System.Drawing.Size(54, 22);
			this.tsddbVisibleQuads.Text = "Visible";
			// 
			// tsddbEdit
			// 
			this.tsddbEdit.AutoToolTip = false;
			this.tsddbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiOptions});
			this.tsddbEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsddbEdit.Image")));
			this.tsddbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbEdit.Name = "tsddbEdit";
			this.tsddbEdit.Size = new System.Drawing.Size(38, 22);
			this.tsddbEdit.Text = "Edit";
			// 
			// tsmiOptions
			// 
			this.tsmiOptions.Name = "tsmiOptions";
			this.tsmiOptions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiOptions.Size = new System.Drawing.Size(154, 22);
			this.tsmiOptions.Text = "Options";
			this.tsmiOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// TopView
			// 
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.quadrants);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "TopView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tscPanel.ContentPanel.ResumeLayout(false);
			this.tscPanel.LeftToolStripPanel.ResumeLayout(false);
			this.tscPanel.LeftToolStripPanel.PerformLayout();
			this.tscPanel.ResumeLayout(false);
			this.tscPanel.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.IContainer components = null;

		private QuadrantPanel quadrants;
		private System.Windows.Forms.ToolStrip tsTools;
		private System.Windows.Forms.Panel pMain;
		private System.Windows.Forms.ToolStripContainer tscPanel;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripDropDownButton tsddbVisibleQuads;
		private System.Windows.Forms.ToolStripDropDownButton tsddbEdit;
		private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
	}
}
