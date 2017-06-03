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
		#region Fields
		private readonly EditorPanel _editorPanel;
		private PaletteView _palView;
		private TrackBar _trackBar;

		bool _paletteInitDone;
		#endregion


		#region Properties
		internal Palette Palette
		{
			set
			{
				_palView.Palette     =
				_editorPanel.Palette = value;
			}
		}

		internal XCImage Sprite
		{
			set { _editorPanel.Sprite = value; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="image"></param>
		internal Editor(PckImage image)
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

			_editorPanel = new EditorPanel(image);
			_editorPanel.Top = _trackBar.Bottom;


			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			Controls.Add(_editorPanel);
			Controls.Add(_trackBar);

			ClientSize = new Size(
								EditorPanel.PreferredWidth,
								EditorPanel.PreferredHeight + _trackBar.Height);

			_palView = new PaletteView();
			_palView.FormClosing += OnPaletteClosing;

			OnTrackScroll(null, null);
		}
		#endregion


		#region Eventcalls
		private void OnTrackScroll(object sender, EventArgs e)
		{
			_editorPanel.ScaleFactor =_trackBar.Value;
		}

		protected override void OnResize(EventArgs e)
		{
//			base.OnResize(e);

			_editorPanel.Width  = ClientSize.Width;
			_editorPanel.Height = ClientSize.Height - _trackBar.Height;

			_trackBar.Width = _editorPanel.Width;
		}

		private void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (!miPalette.Checked)
			{
				miPalette.Checked = true;

				if (!_palView.Visible)
				{
					if (!_paletteInitDone)
					{
						_paletteInitDone = true;
						_palView.Left = Left + 20;
						_palView.Top  = Top  + 20;
					}
					_palView.Show();
				}
				else
					_palView.BringToFront(); // NOTE: this doesn't actually make sense under '!miPalette.Checked'
			}
			else
				_palView.Close(); // see OnPaletteClosing()
		}

		private void OnPaletteClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;

			e.Cancel = true;
			_palView.Hide();
		}

		private void OnShowGridClick(object sender, EventArgs e)
		{
			miGrid.Checked = !miGrid.Checked;
			_editorPanel.Grid = miGrid.Checked;
		}
		#endregion


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
			components = new Container();
			mmMainMenu = new MainMenu(components);
			miPaletteMenu = new MenuItem();
			miPalette = new MenuItem();
			miGridMenu = new MenuItem();
			miGrid = new MenuItem();
			SuspendLayout();
			// 
			// mmMainMenu
			// 
			mmMainMenu.MenuItems.AddRange(new [] {
			miPaletteMenu,
			miGridMenu});
			// 
			// miPaletteMenu
			// 
			miPaletteMenu.Index = 0;
			miPaletteMenu.MenuItems.AddRange(new [] {
			miPalette});
			miPaletteMenu.Text = "Palette";
			// 
			// miPalette
			// 
			miPalette.Index = 0;
			miPalette.Text = "Show";
			miPalette.Click += OnShowPaletteClick;
			// 
			// miGridMenu
			// 
			miGridMenu.Index = 1;
			miGridMenu.MenuItems.AddRange(new [] {
			miGrid});
			miGridMenu.Text = "Grid";
			// 
			// miGrid
			// 
			miGrid.Index = 0;
			miGrid.Text = "Show";
			miGrid.Click += OnShowGridClick;
			// 
			// Editor
			// 
			AutoScaleBaseSize = new Size(5, 12);
			ClientSize = new Size(292, 274);
			Font = new Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			MaximumSize = new Size(300, 300);
			Menu = mmMainMenu;
			Name = "Editor";
			StartPosition = FormStartPosition.Manual;
			Text = "Editor";
			ResumeLayout(false);
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
