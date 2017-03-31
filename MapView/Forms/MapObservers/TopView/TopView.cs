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
		private readonly Dictionary<ToolStripMenuItem, int> _visibleHash;

		private Dictionary<string, Pen> _topPens;
		private Dictionary<string, SolidBrush> _topBrushes;

		private readonly TopViewPanel _topViewPanel;
		private EditButtonsFactory _editButtonsFactory;

		public event EventHandler VisibleTileChanged;


		public TopView()
		{
			InitializeComponent();

			SuspendLayout();

			_topViewPanel = new TopViewPanel();
			_topViewPanel.Width  = 100;
			_topViewPanel.Height = 100;

			center.AutoScroll = true;
			center.Controls.Add(_topViewPanel);

			center.Resize += (sender, e) => _topViewPanel.ParentSize(center.Width, center.Height);

			_visibleHash = new Dictionary<ToolStripMenuItem, int>();

			var visItems = VisibleToolStripButton.DropDown.Items;
			var ground = new ToolStripMenuItem("Floor");
			visItems.Add(ground);
			_topViewPanel.Ground = ground;
			_visibleHash[_topViewPanel.Ground] = 0;
			_topViewPanel.Ground.ShortcutKeys = Keys.F1;

			var west = new ToolStripMenuItem("West");
			visItems.Add(west);
			_topViewPanel.West = west;
			_visibleHash[_topViewPanel.West] = 1;
			_topViewPanel.West.ShortcutKeys = Keys.F2;

			var north = new ToolStripMenuItem("North");
			visItems.Add(north);
			_topViewPanel.North = north;
			_visibleHash[_topViewPanel.North] = 2;
			_topViewPanel.West.ShortcutKeys = Keys.F3;

			var content = new ToolStripMenuItem("Content");
			visItems.Add(content);
			_topViewPanel.Content = content;
			_visibleHash[_topViewPanel.Content] = 3;
			_topViewPanel.Content.ShortcutKeys = Keys.F4;

			foreach (ToolStripItem visItem in visItems)
				visItem.Click += VisibleClick;

			_topViewPanel.QuadrantPanel = bottom;

			MoreObservers.Add("BottomPanel", bottom);
			MoreObservers.Add("TopViewPanel", _topViewPanel);

			ResumeLayout();
		}

		public void Initialize(EditButtonsFactory editButtons)
		{
			_editButtonsFactory = editButtons;
			_editButtonsFactory.MakeToolStrip(toolStrip);
		}

		public QuadrantPanel BottomPanel
		{
			get { return bottom; }
		}

		private void VisibleClick(object sender, EventArgs e)
		{
			var ts = sender as ToolStripMenuItem;
			ts.Checked = !ts.Checked;

			if (VisibleTileChanged != null)
				VisibleTileChanged(this, new EventArgs());

			MapViewPanel.Instance.Refresh();
			Refresh();
		}

		private void DiamondHeight(object sender, string keyword, object val)
		{
			_topViewPanel.MinHeight = (int)val;
		}

		protected override void OnRISettingsLoad(DSShared.Windows.RegistryEventArgs e)
		{
			bottom.Height = 74;
			RegistryKey riKey = e.OpenRegistryKey;

			foreach (var mi in _visibleHash.Keys)
				mi.Checked = bool.Parse((string)riKey.GetValue("vis" + _visibleHash[mi], "true"));
		}

		protected override void OnRISettingsSave(DSShared.Windows.RegistryEventArgs e)
		{
			RegistryKey riKey = e.OpenRegistryKey;

			foreach (var mi in _visibleHash.Keys)
				riKey.SetValue("vis" + _visibleHash[mi], mi.Checked);
		}

		public void FillClick(object sender, EventArgs e)
		{
			var map = MapViewPanel.Instance.MapView.Map;

			if (map != null)
			{
				var start = new Point(0, 0);
				var end   = new Point(0, 0);

				start.X = Math.Min(MapViewPanel.Instance.MapView.DragStart.X, MapViewPanel.Instance.MapView.DragEnd.X);
				start.Y = Math.Min(MapViewPanel.Instance.MapView.DragStart.Y, MapViewPanel.Instance.MapView.DragEnd.Y);
	
				end.X = Math.Max(MapViewPanel.Instance.MapView.DragStart.X, MapViewPanel.Instance.MapView.DragEnd.X);
				end.Y = Math.Max(MapViewPanel.Instance.MapView.DragStart.Y, MapViewPanel.Instance.MapView.DragEnd.Y);

				var tileView = MainWindowsManager.TileView.TileViewControl;
				for (int c = start.X; c <= end.X; c++)
					for (int r = start.Y; r <= end.Y; r++)
						((XCMapTile)map[r, c])[bottom.SelectedQuadrant] = tileView.SelectedTile;

				map.MapChanged = true;
				MapViewPanel.Instance.Refresh();
				Refresh();
			}
		}

		private void options_click(object sender, EventArgs e)
		{
			var f = new OptionsForm("TopViewOptions", Settings);

			f.Text = "Top View Options";
			f.Show();
		}

		private void BrushChanged(object sender, string key, object val)
		{
			_topBrushes[key].Color = (Color)val;

			if (key == "SelectTileColor")
				bottom.SelectColor = _topBrushes[key];

			Refresh();
		}

		private void PenColorChanged(object sender, string key, object val)
		{
			_topPens[key].Color = (Color)val;
			Refresh();
		}

		private void PenWidthChanged(object sender, string key, object val)
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

		private void TopView_KeyDown(object sender, KeyEventArgs e)
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
					BottomPanel.SelectedQuadrant = QuadrantType.Ground;
					break;

				case TileType.WestWall:
					BottomPanel.SelectedQuadrant = QuadrantType.West;
					break;

				case TileType.NorthWall:
					BottomPanel.SelectedQuadrant = QuadrantType.North;
					break;

				case TileType.Object:
					BottomPanel.SelectedQuadrant = QuadrantType.Content;
					break;
			}
		}

		/// <summary>
		/// Loads default settings for TopView in TopRouteView screen.
		/// </summary>
		public override void LoadDefaultSettings()
		{
			_topBrushes = new Dictionary<string, SolidBrush>();
			_topPens = new Dictionary<string, Pen>();

			_topBrushes.Add("GroundColor", new SolidBrush(Color.Orange));
			_topBrushes.Add("ContentColor", new SolidBrush(Color.Green));
			_topBrushes.Add("SelectTileColor", bottom.SelectColor);

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

			ValueChangedEventHandler bc = BrushChanged;
			ValueChangedEventHandler pc = PenColorChanged;
			ValueChangedEventHandler pw = PenWidthChanged;
			ValueChangedEventHandler dh = DiamondHeight;

			Settings.AddSetting("GroundColor",		Color.Orange,				"Color of the ground tile indicator",			"Tile",		bc, false, null);
			Settings.AddSetting("NorthColor",		Color.Red,					"Color of the north tile indicator",			"Tile",		pc, false, null);
			Settings.AddSetting("WestColor",		Color.Red,					"Color of the west tile indicator",				"Tile",		pc, false, null);
			Settings.AddSetting("ContentColor",		Color.Green,				"Color of the content tile indicator",			"Tile",		bc, false, null);
			Settings.AddSetting("NorthWidth",		4,							"Width of the north tile indicator in pixels",	"Tile",		pw, false, null);
			Settings.AddSetting("WestWidth",		4,							"Width of the west tile indicator in pixels",	"Tile",		pw, false, null);
			Settings.AddSetting("SelectColor",		Color.Black,				"Color of the selection line",					"Select",	pc, false, null);
			Settings.AddSetting("SelectWidth",		2,							"Width of the selection line in pixels",		"Select",	pw, false, null);
			Settings.AddSetting("GridColor",		Color.Black,				"Color of the grid lines",						"Grid",		pc, false, null);
			Settings.AddSetting("GridWidth",		1,							"Width of the grid lines",						"Grid",		pw, false, null);
			Settings.AddSetting("MouseWidth",		2,							"Width of the mouse-over indicator",			"Grid",		pw, false, null);
			Settings.AddSetting("MouseColor",		Color.Blue,					"Color of the mouse-over indicator",			"Grid",		pc, false, null);
			Settings.AddSetting("SelectTileColor",	Color.LightBlue,			"Background color of the selected tile part",	"Other",	bc, false, null);
			Settings.AddSetting("DiamondMinHeight",	_topViewPanel.MinHeight,	"Minimum height of the grid tiles",				"Tile",		dh, false, null);

			_topViewPanel.Brushes =
			bottom.Brushes        = _topBrushes;

			_topViewPanel.Pens =
			bottom.Pens        = _topPens;

			Invalidate();
		}
	}
}
