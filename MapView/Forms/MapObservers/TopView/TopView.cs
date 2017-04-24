using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//using Microsoft.Win32;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopView
		:
			MapObserverControl0
	{
//		private readonly Dictionary<ToolStripMenuItem, int> _visQuadsDictionary = new Dictionary<ToolStripMenuItem, int>();

		private Dictionary<string, Pen> _topPens;
		private Dictionary<string, SolidBrush> _topBrushes;

		private readonly TopViewPanel _topViewPanel;
		private EditButtonsFactory _editButtonsFactory;

		private event EventHandler VisibleTileChangedEvent;

		public QuadrantPanel QuadrantsPanel
		{
			get { return quadrants; }
		}


		/// <summary>
		/// cTor.
		/// </summary>
		public TopView()
		{
			InitializeComponent();

			SuspendLayout();

			_topViewPanel = new TopViewPanel();
			_topViewPanel.Width  = 100;
			_topViewPanel.Height = 100;

			pMain.AutoScroll = true;
			pMain.Controls.Add(_topViewPanel);

			pMain.Resize += (sender, e) => _topViewPanel.HandleParentResize(pMain.Width, pMain.Height);

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			_topViewPanel.Ground = new ToolStripMenuItem("Floor");
			visQuads.Add(_topViewPanel.Ground);
			_topViewPanel.Ground.ShortcutKeys = Keys.F1;
			_topViewPanel.Ground.Checked = true;
//			_visQuadsDictionary[_topViewPanel.Ground] = 0;

			_topViewPanel.West = new ToolStripMenuItem("West");
			visQuads.Add(_topViewPanel.West);
			_topViewPanel.West.ShortcutKeys = Keys.F2;
			_topViewPanel.West.Checked = true;
//			_visQuadsDictionary[_topViewPanel.West] = 1;

			_topViewPanel.North = new ToolStripMenuItem("North");
			visQuads.Add(_topViewPanel.North);
			_topViewPanel.North.ShortcutKeys = Keys.F3;
			_topViewPanel.North.Checked = true;
//			_visQuadsDictionary[_topViewPanel.North] = 2;

			_topViewPanel.Content = new ToolStripMenuItem("Content");
			visQuads.Add(_topViewPanel.Content);
			_topViewPanel.Content.ShortcutKeys = Keys.F4;
			_topViewPanel.Content.Checked = true;
//			_visQuadsDictionary[_topViewPanel.Content] = 3;

			foreach (ToolStripMenuItem it in visQuads)
				it.Click += OnToggleQuadrantVisibilityClick;

			_topViewPanel.QuadrantsPanel = QuadrantsPanel;

			MoreObservers.Add("QuadrantsPanel", QuadrantsPanel);
			MoreObservers.Add("TopViewPanel", _topViewPanel);

			ResumeLayout();
		}

		public void InitializeEditStrip(EditButtonsFactory editButtons)
		{
			_editButtonsFactory = editButtons;
			_editButtonsFactory.BuildEditStrip(tsEdit);
		}

		private void OnToggleQuadrantVisibilityClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			it.Checked = !it.Checked;

			if (VisibleTileChangedEvent != null)
				VisibleTileChangedEvent(this, new EventArgs());

			MainViewPanel.Instance.Refresh();
			Refresh();
		}

		private void OnDiamondHeight(object sender, string keyword, object val)
		{
			_topViewPanel.MinHeight = (int)val;
		}

/*		/// <summary>
		/// Loads the VisibleQuadrants flags for the MenuItem-toggles.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnExtraRegistrySettingsLoad(DSShared.Windows.RegistryEventArgs e)
		{
			switch (e.Key)
			{
				case "vis0":
					_topViewPanel.Ground.Checked = e.Value;
					break;
				case "vis1":
					_topViewPanel.West.Checked = e.Value;
					break;
				case "vis2":
					_topViewPanel.North.Checked = e.Value;
					break;
				case "vis3":
					_topViewPanel.Content.Checked = e.Value;
					break;
			}


//			QuadrantsPanel.Height = 74;

//			var regkey = e.OpenRegistryKey;
//			foreach (var it in _visQuadsDictionary.Keys)
//				it.Checked = Boolean.Parse((string)regkey.GetValue("vis" + _visQuadsDictionary[it], "true"));
		} */

/*		/// <summary>
		/// Saves the VisibleQuadrants flags for the menu.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnExtraRegistrySettingsSave(DSShared.Windows.RegistryEventArgs e)
		{
//			var regkey = e.OpenRegistryKey;
//
//			foreach (var it in _visQuadsDictionary.Keys)
//				regkey.SetValue("vis" + _visQuadsDictionary[it], it.Checked);
		} */

		private Form _foptions;
		private bool _closing;

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var it = (ToolStripMenuItem)sender;
			if (!it.Checked)
			{
				it.Checked = true;

				_foptions = new OptionsForm("TopViewOptions", Settings);
				_foptions.Text = "Top View Options";

				_foptions.Show();

				_foptions.Closing += (sender1, e1) =>
				{
					if (!_closing)
						OnOptionsClick(sender, e);

					_closing = false;
				};
			}
			else
			{
				_closing = true;

				it.Checked = false;
				_foptions.Close();
			}
		}

		private void OnBrushChanged(object sender, string key, object val)
		{
			_topBrushes[key].Color = (Color)val;

			if (key == "SelectTileColor")
				QuadrantsPanel.SelectColor = _topBrushes[key];

			Refresh();
		}

		private void OnPenColorChanged(object sender, string key, object val)
		{
			_topPens[key].Color = (Color)val;
			Refresh();
		}

		private void OnPenWidthChanged(object sender, string key, object val)
		{
			_topPens[key].Width = (int)val;
			Refresh();
		}

		public bool GroundVisible
		{
			get { return _topViewPanel.Ground.Checked; }
		}

		public bool NorthVisible
		{
			get { return _topViewPanel.North.Checked; }
		}

		public bool WestVisible
		{
			get { return _topViewPanel.West.Checked; }
		}

		public bool ContentVisible
		{
			get { return _topViewPanel.Content.Checked; }
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S
				&& BaseMap != null)
			{
				BaseMap.Save();
				e.Handled = true;
			}
		}

		public void SelectQuadrant(TileType tileType)
		{
			switch (tileType)
			{
				case TileType.Ground:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.Ground;
					break;

				case TileType.WestWall:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.West;
					break;

				case TileType.NorthWall:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.North;
					break;

				case TileType.Object:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.Content;
					break;
			}
		}

		/// <summary>
		/// Loads default settings for TopView in TopRouteView screens.
		/// </summary>
		public override void LoadControl0Settings()
		{
			_topBrushes = new Dictionary<string, SolidBrush>();
			_topPens    = new Dictionary<string, Pen>();

			_topBrushes.Add("GroundColor", new SolidBrush(Color.Orange));
			_topBrushes.Add("ContentColor", new SolidBrush(Color.Green));
			_topBrushes.Add("SelectTileColor", QuadrantsPanel.SelectColor);

			var northPen = new Pen(new SolidBrush(Color.Red), 4);
			_topPens.Add("NorthColor", northPen);
			_topPens.Add("NorthWidth", northPen);

			var westPen = new Pen(new SolidBrush(Color.Red), 4);
			_topPens.Add("WestColor", westPen);
			_topPens.Add("WestWidth", westPen);

			var selPen = new Pen(new SolidBrush(Color.Black), 2);
			_topPens.Add("SelectColor", selPen);
			_topPens.Add("SelectWidth", selPen);

			var gridPen = new Pen(new SolidBrush(Color.Black), 1);
			_topPens.Add("GridColor", gridPen);
			_topPens.Add("GridWidth", gridPen);

			var mousePen = new Pen(new SolidBrush(Color.Blue), 2);
			_topPens.Add("MouseColor", mousePen);
			_topPens.Add("MouseWidth", mousePen);

			ValueChangedEventHandler bc = OnBrushChanged;
			ValueChangedEventHandler pc = OnPenColorChanged;
			ValueChangedEventHandler pw = OnPenWidthChanged;
			ValueChangedEventHandler dh = OnDiamondHeight;

			Settings.AddSetting("GroundColor",      Color.Orange,            "Color of the ground tile indicator",          "Tile",   bc);
			Settings.AddSetting("NorthColor",       Color.Red,               "Color of the north tile indicator",           "Tile",   pc);
			Settings.AddSetting("WestColor",        Color.Red,               "Color of the west tile indicator",            "Tile",   pc);
			Settings.AddSetting("ContentColor",     Color.Green,             "Color of the content tile indicator",         "Tile",   bc);
			Settings.AddSetting("NorthWidth",       4,                       "Width of the north tile indicator in pixels", "Tile",   pw);
			Settings.AddSetting("WestWidth",        4,                       "Width of the west tile indicator in pixels",  "Tile",   pw);
			Settings.AddSetting("SelectColor",      Color.Black,             "Color of the selection line",                 "Select", pc);
			Settings.AddSetting("SelectWidth",      2,                       "Width of the selection line in pixels",       "Select", pw);
			Settings.AddSetting("GridColor",        Color.Black,             "Color of the grid lines",                     "Grid",   pc);
			Settings.AddSetting("GridWidth",        1,                       "Width of the grid lines",                     "Grid",   pw);
			Settings.AddSetting("MouseWidth",       2,                       "Width of the mouse-over indicator",           "Grid",   pw);
			Settings.AddSetting("MouseColor",       Color.Blue,              "Color of the mouse-over indicator",           "Grid",   pc);
			Settings.AddSetting("SelectTileColor",  Color.LightBlue,         "Background color of the selected tile part",  "Other",  bc);
			Settings.AddSetting("DiamondMinHeight", _topViewPanel.MinHeight, "Minimum height of the grid tiles",            "Tile",   dh);

			_topViewPanel.Brushes  =
			QuadrantsPanel.Brushes = _topBrushes;

			_topViewPanel.Pens  =
			QuadrantsPanel.Pens = _topPens;

			Invalidate();
		}
	}
}
