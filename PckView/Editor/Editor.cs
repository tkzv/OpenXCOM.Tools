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


		private readonly EditorPanel _editPanel;


		public Editor(PckImage image)
		{
			_editPanel    = new EditorPanel(image);
			_buttonsPanel = new ButtonPanel();
			_trackBar     = new TrackBar();


			_trackBar.Minimum = 1;
			_trackBar.Maximum = 10;

			InitializeComponent();

			Controls.Add(_editPanel);
			Controls.Add(_buttonsPanel);
			Controls.Add(_trackBar);

			_buttonsPanel.Location = new Point(0, 0);
			_buttonsPanel.Width = _buttonsPanel.PreferredWidth;

			_trackBar.Left = _buttonsPanel.Right;
			_trackBar.Top  = _buttonsPanel.Top;

			_editPanel.Top  = _trackBar.Bottom;
			_editPanel.Left = _buttonsPanel.Right;

			ClientSize = new Size(
								EditorPane.PreferredWidth  + _buttonsPanel.PreferredWidth,
								EditorPane.PreferredHeight + _trackBar.Height);

			_palView = new PalView();
			_palView.Closing += OnPaletteClosing;

			_palView.PaletteIndexChanged += _editPanel.Editor.SelectColor;

			_trackBar.Scroll += sizeScroll;
		}

		private void sizeScroll(object sender, EventArgs e)
		{
			_editPanel.Editor.ScaleVal=_trackBar.Value;
		}

		public Palette Palette
		{
//			get { return _editPanel.Editor.Palette; }
			set
			{
				_palView.Palette          =
				_buttonsPanel.Palette     =
				_editPanel.Editor.Palette = value;
			}
		}

		public void ShowPalView()
		{
			if (_palView.Visible)
				_palView.BringToFront();
			else
			{
				_palView.Left = Right;
				_palView.Top = Top;
				_palView.Show();
			}
			showPalette.Checked = true;
		}

		private void OnPaletteClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			_palView.Hide();
			showPalette.Checked = false;

//			if (PalViewClosing != null)
//				PalViewClosing(this, new EventArgs());
		}

		protected override void OnResize(EventArgs e)
		{
			_editPanel.Width  = ClientSize.Width  - _buttonsPanel.PreferredWidth;
			_editPanel.Height = ClientSize.Height - _trackBar.Height;

			_buttonsPanel.Height = ClientSize.Height;
			_trackBar.Width = _editPanel.Width;

			_editPanel.Left = _buttonsPanel.Right;
			_trackBar.Left = _editPanel.Left;
		}

		public XCImage Image
		{
//			get { return _editPanel.Editor.Image; }
			set
			{
//				_buttonsPanel.Image = value;
				_editPanel.Editor.Image = value;
				OnResize(null);
			}
		}


		private void showPalette_Click(object sender, EventArgs e)
		{
			if (showPalette.Checked)
				_palView.Close();
			else
				ShowPalView();
		}

		private void showLines_Click(object sender, EventArgs e)
		{
			showLines.Checked =! showLines.Checked;
			_editPanel.Editor.Lines = showLines.Checked;
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
			this.menu = new System.Windows.Forms.MainMenu();
			this.paletteMain = new System.Windows.Forms.MenuItem();
			this.showPalette = new System.Windows.Forms.MenuItem();
			this.linesItem = new System.Windows.Forms.MenuItem();
			this.showLines = new System.Windows.Forms.MenuItem();
			//
			// menu
			//
			this.menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{
																		this.paletteMain,
																		this.linesItem });
			//
			// paletteMain
			//
			this.paletteMain.Index = 0;
			this.paletteMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{ this.showPalette });
			this.paletteMain.Text = "Palette";
			//
			// showPalette
			//
			this.showPalette.Index = 0;
			this.showPalette.Text = "Show";
			this.showPalette.Click += new System.EventHandler(this.showPalette_Click);
			//
			// linesItem
			//
			this.linesItem.Index = 1;
			this.linesItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{ this.showLines });
			this.linesItem.Text = "Lines";
			//
			// showLines
			//
			this.showLines.Index = 0;
			this.showLines.Text = "Show";
			this.showLines.Click += new System.EventHandler(this.showLines_Click);
			//
			// Editor
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Menu = this.menu;
			this.Name = "Editor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Editor";

		}
		#endregion

		private Container components = null;
		private PalView _palView;
		private MainMenu menu;
		private MenuItem paletteMain;
		private MenuItem showPalette;
		private ButtonPanel _buttonsPanel;
		private MenuItem linesItem;
		private MenuItem showLines;
		private TrackBar _trackBar;
	}
}
