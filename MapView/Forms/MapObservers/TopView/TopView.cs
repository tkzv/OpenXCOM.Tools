using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopView
		:
			MapObserverControl0
	{
		private readonly Dictionary<ToolStripMenuItem, int> _dictVisibleQuads;

		private Dictionary<string, Pen> _topPens;
		private Dictionary<string, SolidBrush> _topBrushes;

		private readonly TopViewPanel _topViewPanel;
		private EditButtonsFactory _editButtonsFactory;

		private event EventHandler VisibleTileChangedEvent;


		public TopView()
		{
			InitializeComponent();

			SuspendLayout();

			_topViewPanel = new TopViewPanel();
			_topViewPanel.Width  = 100;
			_topViewPanel.Height = 100;

			pMain.AutoScroll = true;
			pMain.Controls.Add(_topViewPanel);

			pMain.Resize += (sender, e) => _topViewPanel.ParentSize(pMain.Width, pMain.Height);

			_dictVisibleQuads = new Dictionary<ToolStripMenuItem, int>();

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			var itGround = new ToolStripMenuItem("Floor");
			visQuads.Add(itGround);
			_topViewPanel.Ground = itGround;
			_dictVisibleQuads[_topViewPanel.Ground] = 0;
			_topViewPanel.Ground.ShortcutKeys = Keys.F1;

			var itWest = new ToolStripMenuItem("West");
			visQuads.Add(itWest);
			_topViewPanel.West = itWest;
			_dictVisibleQuads[_topViewPanel.West] = 1;
			_topViewPanel.West.ShortcutKeys = Keys.F2;

			var itNorth = new ToolStripMenuItem("North");
			visQuads.Add(itNorth);
			_topViewPanel.North = itNorth;
			_dictVisibleQuads[_topViewPanel.North] = 2;
			_topViewPanel.West.ShortcutKeys = Keys.F3;

			var itContent = new ToolStripMenuItem("Content");
			visQuads.Add(itContent);
			_topViewPanel.Content = itContent;
			_dictVisibleQuads[_topViewPanel.Content] = 3;
			_topViewPanel.Content.ShortcutKeys = Keys.F4;

			foreach (ToolStripMenuItem it in visQuads)
				it.Click += OnShowQuadTypeClick;

			_topViewPanel.QuadrantsPanel = QuadrantsPanel;

			MoreObservers.Add("QuadrantsPanel", QuadrantsPanel);
			MoreObservers.Add("TopViewPanel", _topViewPanel);

			ResumeLayout();
		}

		public void Initialize(EditButtonsFactory editButtons)
		{
			_editButtonsFactory = editButtons;
			_editButtonsFactory.BuildToolStrip(tsEdit);
		}

		public QuadrantPanel QuadrantsPanel
		{
			get { return quadrants; }
		}

		private void OnShowQuadTypeClick(object sender, EventArgs e)
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

		protected override void OnRegistrySettingsLoad(DSShared.Windows.RegistryEventArgs e)
		{
			QuadrantsPanel.Height = 74;
			var regkey = e.OpenRegistryKey;

			foreach (var it in _dictVisibleQuads.Keys)
				it.Checked = bool.Parse((string)regkey.GetValue("vis" + _dictVisibleQuads[it], "true"));
		}

		protected override void OnRegistrySettingsSave(DSShared.Windows.RegistryEventArgs e)
		{
			var regkey = e.OpenRegistryKey;

			foreach (var it in _dictVisibleQuads.Keys)
				regkey.SetValue("vis" + _dictVisibleQuads[it], it.Checked);
		}

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var f = new OptionsForm("TopViewOptions", Settings);

			f.Text = "Top View Options";
			f.Show();
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
				&& Map != null)
			{
				Map.Save();
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
		/// Loads default settings for TopView in TopRouteView screen.
		/// </summary>
		public override void LoadDefaultSettings()
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

			Settings.AddSetting("GroundColor",      Color.Orange,            "Color of the ground tile indicator",          "Tile",   bc, false, null);
			Settings.AddSetting("NorthColor",       Color.Red,               "Color of the north tile indicator",           "Tile",   pc, false, null);
			Settings.AddSetting("WestColor",        Color.Red,               "Color of the west tile indicator",            "Tile",   pc, false, null);
			Settings.AddSetting("ContentColor",     Color.Green,             "Color of the content tile indicator",         "Tile",   bc, false, null);
			Settings.AddSetting("NorthWidth",       4,                       "Width of the north tile indicator in pixels", "Tile",   pw, false, null);
			Settings.AddSetting("WestWidth",        4,                       "Width of the west tile indicator in pixels",  "Tile",   pw, false, null);
			Settings.AddSetting("SelectColor",      Color.Black,             "Color of the selection line",                 "Select", pc, false, null);
			Settings.AddSetting("SelectWidth",      2,                       "Width of the selection line in pixels",       "Select", pw, false, null);
			Settings.AddSetting("GridColor",        Color.Black,             "Color of the grid lines",                     "Grid",   pc, false, null);
			Settings.AddSetting("GridWidth",        1,                       "Width of the grid lines",                     "Grid",   pw, false, null);
			Settings.AddSetting("MouseWidth",       2,                       "Width of the mouse-over indicator",           "Grid",   pw, false, null);
			Settings.AddSetting("MouseColor",       Color.Blue,              "Color of the mouse-over indicator",           "Grid",   pc, false, null);
			Settings.AddSetting("SelectTileColor",  Color.LightBlue,         "Background color of the selected tile part",  "Other",  bc, false, null);
			Settings.AddSetting("DiamondMinHeight", _topViewPanel.MinHeight, "Minimum height of the grid tiles",            "Tile",   dh, false, null);

			_topViewPanel.Brushes  =
			QuadrantsPanel.Brushes = _topBrushes;

			_topViewPanel.Pens  =
			QuadrantsPanel.Pens = _topPens;

			Invalidate();
		}
	}
}
