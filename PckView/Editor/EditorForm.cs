using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces;


namespace PckView
{
	internal sealed class EditorForm
		:
			Form
	{
		#region Fields
		private readonly EditorPanel _pnlEditor;

		private TrackBar _trackBar;
		private PaletteForm _fpalette = new PaletteForm();
		#endregion


		#region Properties
		internal XCImage Sprite
		{
			set { _pnlEditor.Sprite = value; }
		}

		private bool Inited
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal EditorForm()
		{
			_trackBar = new TrackBar();
			_trackBar.AutoSize    = false;
			_trackBar.Height      = 23;
			_trackBar.Minimum     =  1;
			_trackBar.Maximum     = 10;
			_trackBar.Value       = 10;
			_trackBar.LargeChange =  1;
			_trackBar.BackColor   = Color.Silver;
			_trackBar.Scroll += OnTrackScroll;

			_pnlEditor = new EditorPanel();
			_pnlEditor.Top = _trackBar.Bottom;


			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			MaximumSize = new Size(0, 0); // fu.net


			Controls.Add(_pnlEditor);
			Controls.Add(_trackBar);

			_fpalette.FormClosing += OnPaletteFormClosing;

			OnTrackScroll(null, EventArgs.Empty);
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// Sets the *proper* ClientSize.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
		{
			ClientSize = new Size(
								XCImageFile.SpriteWidth  * 10,
								XCImageFile.SpriteHeight * 10 + _trackBar.Height);
		}

		private void OnTrackScroll(object sender, EventArgs e)
		{
			_pnlEditor.ScaleFactor = _trackBar.Value;
		}

		protected override void OnResize(EventArgs e)
		{
//			base.OnResize(e);

			_trackBar.Width   =
			_pnlEditor.Width  = ClientSize.Width;
			_pnlEditor.Height = ClientSize.Height - _trackBar.Height;
		}

		private void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (!miPalette.Checked)
			{
				miPalette.Checked = true;

				if (!Inited)
				{
					Inited = true;
					_fpalette.Left = Left + 20;
					_fpalette.Top  = Top  + 20;
				}
				_fpalette.Show();
			}
			else
				_fpalette.Close(); // hide, not close -> see OnPaletteFormClosing()
		}

		private void OnPaletteFormClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;

			e.Cancel = true;
			_fpalette.Hide();
		}

		private void OnShowGridClick(object sender, EventArgs e)
		{
			_pnlEditor.Grid = (miGrid.Checked = !miGrid.Checked);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Closes the palette-form when PckView closes. This is required when
		/// PckView opens via MapView.
		/// </summary>
		internal void ClosePalette()
		{
			_fpalette.Close();
		}
		#endregion


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.miPalette.Text = "Show Palette";
			this.miPalette.Click += new System.EventHandler(this.OnShowPaletteClick);
			// 
			// miGridMenu
			// 
			this.miGridMenu.Index = 1;
			this.miGridMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miGrid});
			this.miGridMenu.Text = "Grid";
			// 
			// miGrid
			// 
			this.miGrid.Index = 0;
			this.miGrid.Text = "Show Grid";
			this.miGrid.Click += new System.EventHandler(this.OnShowGridClick);
			// 
			// EditorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 276);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(300, 300);
			this.Menu = this.mmMainMenu;
			this.MinimizeBox = false;
			this.Name = "EditorForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Sprite Editor";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);

		}
		#endregion

		private IContainer components;

		private MainMenu mmMainMenu;
		private MenuItem miPaletteMenu;
		private MenuItem miPalette;
		private MenuItem miGridMenu;
		private MenuItem miGrid;
	}
}
