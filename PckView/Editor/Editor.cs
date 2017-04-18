using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class Editor
		:
			Form
	{
//		public event EventHandler PalViewClosing;


		private readonly EditorPanel _editorPanel;
		private PaletteView _palView;
		private TrackBar _trackBar;
//		private ButtonPanel _buttonsPanel;


		internal Editor(PckImage image)
		{
			_editorPanel  = new EditorPanel(image);
//			_buttonsPanel = new ButtonPanel();
			_trackBar     = new TrackBar();

			_trackBar.Minimum = 1;
			_trackBar.Maximum = 10;

			_trackBar.Value = _trackBar.Maximum;


			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			Controls.Add(_editorPanel);
//			Controls.Add(_buttonsPanel);
			Controls.Add(_trackBar);

//			_buttonsPanel.Location = new Point(0, 0);
//			_buttonsPanel.Width = _buttonsPanel.PreferredWidth;

			_trackBar.Left = 10; //_buttonsPanel.Right;
			_trackBar.Top  = 5;  //_buttonsPanel.Top;

			_editorPanel.Top  = _trackBar.Bottom;
			_editorPanel.Left = 0; //_buttonsPanel.Right;

			ClientSize = new Size(
								EditorPanel.PreferredWidth,//  + _buttonsPanel.PreferredWidth,
								EditorPanel.PreferredHeight + _trackBar.Height);

			_palView = new PaletteView();
			_palView.Closing += OnPaletteClosing;

//			_palView.PaletteIndexChanged += _editPanel.Editor.SelectColor; // does nothing.

			_trackBar.Scroll += OnScroll;
		}

		private void OnScroll(object sender, EventArgs e)
		{
			_editorPanel.ScaleFactor =_trackBar.Value;
		}

		internal Palette Palette
		{
//			get { return _editPanel.Editor.Palette; }
			set
			{
				_palView.Palette            =
//				_buttonsPanel.Palette       =
				_editorPanel.Palette = value;
			}
		}

		private void OnPaletteClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;

			e.Cancel = true;
			_palView.Hide();

//			if (PalViewClosing != null)
//				PalViewClosing(this, new EventArgs());
		}

		protected override void OnResize(EventArgs e)
		{
			_editorPanel.Width  = ClientSize.Width;//  - _buttonsPanel.PreferredWidth;
			_editorPanel.Height = ClientSize.Height - _trackBar.Height;

//			_buttonsPanel.Height = ClientSize.Height;
			_trackBar.Width = _editorPanel.Width;

			_editorPanel.Left = 0;// _buttonsPanel.Right;
			_trackBar.Left = _editorPanel.Left;
		}

		internal XCImage Image
		{
//			get { return _editPanel.Editor.Image; }
			set
			{
//				_buttonsPanel.Image = value;
				_editorPanel.Image = value;
				OnResize(null);
			}
		}


		private void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (!miPalette.Checked)
			{
				miPalette.Checked = true;

				if (!_palView.Visible)
				{
					_palView.Left = Right;
					_palView.Top = Top;
					_palView.Show();
				}
				else
					_palView.BringToFront(); // NOTE: this doesn't actually make sense under '!miPalette.Checked'
			}
			else
				_palView.Close(); // see OnPaletteClosing()
		}

		private void OnShowGridClick(object sender, EventArgs e)
		{
			miGrid.Checked = !miGrid.Checked;
			_editorPanel.Grid = miGrid.Checked;
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
			this.components = new System.ComponentModel.Container();
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miPalette = new System.Windows.Forms.MenuItem();
			this.miGridMenu = new System.Windows.Forms.MenuItem();
			this.miGrid = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPaletteMenu,
			this.miGridMenu});
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Index = 0;
			this.miPaletteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPalette});
			this.miPaletteMenu.Text = "Palette";
			// 
			// miPalette
			// 
			this.miPalette.Index = 0;
			this.miPalette.Text = "Show";
			this.miPalette.Click += new System.EventHandler(this.OnShowPaletteClick);
			// 
			// miGridMenu
			// 
			this.miGridMenu.Index = 1;
			this.miGridMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miGrid});
			this.miGridMenu.Text = "Lines";
			// 
			// miGrid
			// 
			this.miGrid.Index = 0;
			this.miGrid.Text = "Show";
			this.miGrid.Click += new System.EventHandler(this.OnShowGridClick);
			// 
			// Editor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(300, 300);
			this.Menu = this.mmMainMenu;
			this.Name = "Editor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Editor";
			this.ResumeLayout(false);

		}
		#endregion

		private System.ComponentModel.IContainer components;
		private MainMenu mmMainMenu;
		private MenuItem miPaletteMenu;
		private MenuItem miPalette;
		private MenuItem miGridMenu;
		private MenuItem miGrid;
	}
}
