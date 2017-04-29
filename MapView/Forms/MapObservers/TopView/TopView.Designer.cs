namespace MapView.Forms.MapObservers.TopViews
{
	partial class TopView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopView));
			this.tscMain = new System.Windows.Forms.ToolStripContainer();
			this.pMain = new System.Windows.Forms.Panel();
			this.tsEdit = new System.Windows.Forms.ToolStrip();
			this.quadrants = new MapView.Forms.MapObservers.TopViews.QuadrantPanel();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbVisibleQuads = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tscMain.ContentPanel.SuspendLayout();
			this.tscMain.LeftToolStripPanel.SuspendLayout();
			this.tscMain.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// tscMain
			// 
			// 
			// tscMain.BottomToolStripPanel
			// 
			this.tscMain.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tscMain.ContentPanel
			// 
			this.tscMain.ContentPanel.Controls.Add(this.pMain);
			this.tscMain.ContentPanel.Size = new System.Drawing.Size(609, 359);
			this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tscMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// tscMain.LeftToolStripPanel
			// 
			this.tscMain.LeftToolStripPanel.Controls.Add(this.tsEdit);
			this.tscMain.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscMain.Location = new System.Drawing.Point(0, 25);
			this.tscMain.Name = "tscMain";
			// 
			// tscMain.RightToolStripPanel
			// 
			this.tscMain.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscMain.Size = new System.Drawing.Size(640, 384);
			this.tscMain.TabIndex = 4;
			this.tscMain.Text = "toolStripContainer2";
			// 
			// tscMain.TopToolStripPanel
			// 
			this.tscMain.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// pMain
			// 
			this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pMain.Location = new System.Drawing.Point(0, 0);
			this.pMain.Name = "pMain";
			this.pMain.Size = new System.Drawing.Size(609, 359);
			this.pMain.TabIndex = 2;
			// 
			// tsEdit
			// 
			this.tsEdit.Dock = System.Windows.Forms.DockStyle.None;
			this.tsEdit.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsEdit.Location = new System.Drawing.Point(0, 3);
			this.tsEdit.Name = "tsEdit";
			this.tsEdit.Padding = new System.Windows.Forms.Padding(0);
			this.tsEdit.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsEdit.Size = new System.Drawing.Size(31, 30);
			this.tsEdit.TabIndex = 1;
			this.tsEdit.Text = "toolStrip1";
			// 
			// quadrants
			// 
			this.quadrants.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.quadrants.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.quadrants.Location = new System.Drawing.Point(0, 409);
			this.quadrants.Name = "quadrants";
			this.quadrants.SelectedQuadrant = XCom.QuadrantType.Ground;
			this.quadrants.Size = new System.Drawing.Size(640, 71);
			this.quadrants.TabIndex = 0;
			this.quadrants.Text = "bottom";
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbVisibleQuads,
			this.toolStripDropDownButton1});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.Text = "toolStrip1";
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
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.AutoToolTip = false;
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiOptions});
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
			this.toolStripDropDownButton1.Text = "Edit";
			// 
			// tsmiOptions
			// 
			this.tsmiOptions.Name = "tsmiOptions";
			this.tsmiOptions.Size = new System.Drawing.Size(113, 22);
			this.tsmiOptions.Text = "Options";
			this.tsmiOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// TopView
			// 
			this.Controls.Add(this.tscMain);
			this.Controls.Add(this.quadrants);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "TopView";
			this.Size = new System.Drawing.Size(640, 480);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.tscMain.ContentPanel.ResumeLayout(false);
			this.tscMain.LeftToolStripPanel.ResumeLayout(false);
			this.tscMain.LeftToolStripPanel.PerformLayout();
			this.tscMain.ResumeLayout(false);
			this.tscMain.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private QuadrantPanel quadrants;
		private System.Windows.Forms.ToolStrip tsEdit;
		private System.Windows.Forms.Panel pMain;
		private System.Windows.Forms.ToolStripContainer tscMain;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripDropDownButton tsddbVisibleQuads;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
	}
}
