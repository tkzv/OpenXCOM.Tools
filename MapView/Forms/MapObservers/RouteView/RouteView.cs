using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	/*
		UFO			TFTD
		Commander	Commander
		Leader		Navigator
		Engineer	Medic
		Medic		Technition
		Navigator	SquadLeader
		Soldier		Soldier
	*/

	public partial class RouteView
		:
		MapObserverControl
	{
		private readonly RoutePanel _routePanel;

		private Panel _contentPanel;

		private XCMapFile _mapFile;
		private RouteNode _nodeSelected;

		private bool _loadingGui;
		private bool _loadingMap;

		private readonly List<object> _linksList = new List<object>();


		public RouteView()
		{
			InitializeComponent();

			_routePanel = new RoutePanel();
			_contentPanel.Controls.Add(_routePanel);
			_routePanel.MapPanelClicked += RmpPanel_PanelClick;
			_routePanel.MouseMove += RmpPanel_MouseMove;
			_routePanel.Dock = DockStyle.Fill;

			var unitTypes = new object[]
			{
				UnitType.Any,
				UnitType.Small,
				UnitType.Large,
				UnitType.Flying,
				UnitType.FlyingLarge
			};

			cbType.Items.AddRange(unitTypes);

			cbUse1.Items.AddRange(unitTypes);
			cbUse2.Items.AddRange(unitTypes);
			cbUse3.Items.AddRange(unitTypes);
			cbUse4.Items.AddRange(unitTypes);
			cbUse5.Items.AddRange(unitTypes);

			cbType.DropDownStyle = ComboBoxStyle.DropDownList;

			cbUse1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbUse2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbUse3.DropDownStyle = ComboBoxStyle.DropDownList;
			cbUse4.DropDownStyle = ComboBoxStyle.DropDownList;
			cbUse5.DropDownStyle = ComboBoxStyle.DropDownList;

			cbRank1.Items.AddRange(RouteFile.UnitRankUFO);
			cbRank1.DropDownStyle = ComboBoxStyle.DropDownList;

			foreach (var value in Enum.GetValues(typeof(NodeImportance)))
				cbRank2.Items.Add(value);

			cbRank2.DropDownStyle = ComboBoxStyle.DropDownList;

			foreach (var value in Enum.GetValues(typeof(BaseModuleAttack)))
				AttackBaseCombo.Items.Add(value);

			AttackBaseCombo.DropDownStyle = ComboBoxStyle.DropDownList;

			cbLink1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink3.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink4.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink5.DropDownStyle = ComboBoxStyle.DropDownList;

			cbUsage.Items.AddRange(RouteFile.SpawnUsage);
			cbUsage.DropDownStyle = ComboBoxStyle.DropDownList;

			ClearSelected();
		}


		private void options_click(object sender, EventArgs e)
		{
			var f = new PropertyForm("rmpViewOptions", Settings);
			f.Text = "Route Settings";
			f.Show();
		}

		private void BrushColorChanged(object sender, string key, object val)
		{
			_routePanel.MapBrushes[key].Color = (Color)val;
			Refresh();
		}

		private void PenColorChanged(object sender, string key, object val)
		{
			_routePanel.MapPens[key].Color = (Color)val;
			Refresh();
		}

		private void PenWidthChanged(object sender, string key, object val)
		{
			_routePanel.MapPens[key].Width = (int)val;
			Refresh();
		}
		
		private void RmpPanel_MouseMove(object sender, MouseEventArgs e)
		{
			var tile = _routePanel.GetTile(e.X, e.Y);
			if (tile != null && tile.Node != null)
			{
				lblMouseOver.Text = "Over " + tile.Node.Index;
			}
			else
				lblMouseOver.Text = String.Empty;

			_routePanel.Pos = new Point(e.X, e.Y);
			_routePanel.Refresh(); // mouseover refresh for RouteView.
		}

		private void RmpPanel_PanelClick(object sender, MapPanelClickEventArgs e)
		{
			_routePanel.Focus();

			idxLabel.Text = Text;
			try
			{
				var node = ((XCMapTile)e.ClickTile).Node;

				if (e.MouseEventArgs.Button == MouseButtons.Right && _nodeSelected != null)
				{
					if (node != null)
					{
						if (!_nodeSelected.Equals(node))
						{
							_mapFile.MapChanged = true;

							ConnectNode(node);
	
							_nodeSelected = node;
							FillRouteInformation();

							Refresh();
						}
						return;
					}
				}

				var nodePre = _nodeSelected;
				_nodeSelected = node;

				if (e.MouseEventArgs.Button == MouseButtons.Right && _nodeSelected == null)
				{
					_mapFile.MapChanged = true;

					_nodeSelected = _mapFile.AddRouteNode(e.ClickLocation);

					ConnectCreatedNode(nodePre);
				}
			}
			catch
			{
				// TODO: this.
				return;
			}
			FillRouteInformation();
		}

		private ConnectNodeType GetConnectionSetting()
		{
			if (connectNodesToolStripMenuItem.Text == "Connect One way")
				return ConnectNodeType.ConnectOneWay;

			if (connectNodesToolStripMenuItem.Text == "Connect Two ways")
				return ConnectNodeType.ConnectTwoWays;

			return ConnectNodeType.DontConnect;
		}

		private void ConnectNode(RouteNode node)
		{
			var type = GetConnectionSetting();
			if (type != ConnectNodeType.DontConnect)
			{
				int slot = GetOpenLinkSlot(_nodeSelected, node.Index);
				if (slot != -1)
				{
					_nodeSelected[slot].Destination = node.Index;
					_nodeSelected[slot].Distance = calcLinkDistance(
															_nodeSelected,
															node,
															null);
					// TODO: work in UsableType.
				}

				if (type == ConnectNodeType.ConnectTwoWays)
				{
					slot = GetOpenLinkSlot(node, _nodeSelected.Index);
					if (slot != -1)
					{
						node[slot].Destination = _nodeSelected.Index;
						node[slot].Distance = calcLinkDistance(
															node,
															_nodeSelected,
															null);
						// TODO: work in UsableType.
					}
				}
			}
		}

		private void ConnectCreatedNode(RouteNode node)
		{
			if (node != null)
			{
				var type = GetConnectionSetting();
				if (type != ConnectNodeType.DontConnect)
				{
					var linkId = GetOpenLinkSlot(node);
					if (linkId != -1)
					{
						node[linkId].Destination = (byte)(_mapFile.RouteFile.Length - 1);
						node[linkId].Distance = calcLinkDistance(
															node,
															_nodeSelected,
															null);
					}

					if (type == ConnectNodeType.ConnectTwoWays)
					{
						_nodeSelected[0].Destination = node.Index;
						_nodeSelected[0].Distance = calcLinkDistance(
																_nodeSelected,
																node,
																txtDist1);
					}
				}
			}
		}

		private static int GetOpenLinkSlot(RouteNode node, int idOther = -1)
		{
			if (node != null)
			{
				for (int i = 0; i != RouteNode.LinkSlots; ++i)
				{
					if (idOther != -1 && node[i].Destination == idOther)
						break;

					if (node[i].Destination == (byte)LinkType.NotUsed)
						return i;
				}
			}
			return -1;
		}

		private void FillRouteInformation()
		{
			if (_nodeSelected == null)
			{
				gbNodeInfo.Enabled   =
				groupBox1.Enabled    =
				groupBox2.Enabled    =
				LinkGroupBox.Enabled = false;
			}
			else
			{
				gbNodeInfo.Enabled   =
				groupBox1.Enabled    =
				groupBox2.Enabled    =
				LinkGroupBox.Enabled = true;

				gbNodeInfo.SuspendLayout();
				LinkGroupBox.SuspendLayout();

				_loadingGui = true;

				_linksList.Clear();

				cbLink1.Items.Clear();
				cbLink2.Items.Clear();
				cbLink3.Items.Clear();
				cbLink4.Items.Clear();
				cbLink5.Items.Clear();

				for (byte i = 0; i != _mapFile.RouteFile.Length; ++i)
					if (i != _nodeSelected.Index)
						_linksList.Add(i); // add all other nodes

				var linkTypes = new object[]
				{
					LinkType.NotUsed,
					LinkType.ExitNorth,
					LinkType.ExitEast,
					LinkType.ExitSouth,
					LinkType.ExitWest
				};
				_linksList.AddRange(linkTypes); // add the four compass-points + not used.

				object[] linkListArray = _linksList.ToArray();

				cbLink1.Items.AddRange(linkListArray);
				cbLink2.Items.AddRange(linkListArray);
				cbLink3.Items.AddRange(linkListArray);
				cbLink4.Items.AddRange(linkListArray);
				cbLink5.Items.AddRange(linkListArray);

				cbType.SelectedItem = _nodeSelected.UsableType;

				if (_mapFile.Tiles[0][0].Palette == Palette.UFOBattle)
					cbRank1.SelectedItem = RouteFile.UnitRankUFO[_nodeSelected.UsableRank];
				else
					cbRank1.SelectedItem = RouteFile.UnitRankTFTD[_nodeSelected.UsableRank];

				cbRank2.SelectedItem = _nodeSelected.Priority;
				AttackBaseCombo.SelectedItem = _nodeSelected.Attack;
				cbUsage.SelectedItem = RouteFile.SpawnUsage[(byte)_nodeSelected.Spawn];

				idxLabel2.Text = "Current " + _nodeSelected.Index;

				if (_nodeSelected[0].Destination < Link.EXIT_WEST)
					cbLink1.SelectedItem = _nodeSelected[0].Destination;
				else
					cbLink1.SelectedItem = (LinkType)_nodeSelected[0].Destination;

				if (_nodeSelected[1].Destination < Link.EXIT_WEST)
					cbLink2.SelectedItem = _nodeSelected[1].Destination;
				else
					cbLink2.SelectedItem = (LinkType)_nodeSelected[1].Destination;

				if (_nodeSelected[2].Destination < Link.EXIT_WEST)
					cbLink3.SelectedItem = _nodeSelected[2].Destination;
				else
					cbLink3.SelectedItem = (LinkType)_nodeSelected[2].Destination;

				if (_nodeSelected[3].Destination < Link.EXIT_WEST)
					cbLink4.SelectedItem = _nodeSelected[3].Destination;
				else
					cbLink4.SelectedItem = (LinkType)_nodeSelected[3].Destination;

				if (_nodeSelected[4].Destination < Link.EXIT_WEST)
					cbLink5.SelectedItem = _nodeSelected[4].Destination;
				else
					cbLink5.SelectedItem = (LinkType)_nodeSelected[4].Destination;

				cbUse1.SelectedItem = _nodeSelected[0].UsableType;
				cbUse2.SelectedItem = _nodeSelected[1].UsableType;
				cbUse3.SelectedItem = _nodeSelected[2].UsableType;
				cbUse4.SelectedItem = _nodeSelected[3].UsableType;
				cbUse5.SelectedItem = _nodeSelected[4].UsableType;

				txtDist1.Text = Convert.ToString(_nodeSelected[0].Distance, System.Globalization.CultureInfo.InvariantCulture);
				txtDist2.Text = Convert.ToString(_nodeSelected[1].Distance, System.Globalization.CultureInfo.InvariantCulture);
				txtDist3.Text = Convert.ToString(_nodeSelected[2].Distance, System.Globalization.CultureInfo.InvariantCulture);
				txtDist4.Text = Convert.ToString(_nodeSelected[3].Distance, System.Globalization.CultureInfo.InvariantCulture);
				txtDist5.Text = Convert.ToString(_nodeSelected[4].Distance, System.Globalization.CultureInfo.InvariantCulture);

				gbNodeInfo.ResumeLayout();
				gbNodeInfo.ResumeLayout();
				LinkGroupBox.ResumeLayout();

				_loadingGui = false;
			}
		}

		public void SetMap(object sender, SetMapEventArgs e)
		{
			Map = e.Map;
		}

		public override IMap_Base Map
		{
			set
			{
				base.Map = value;
				_mapFile = (XCMapFile)value;

				_loadingMap = true;
				try
				{
					HeightDifTextbox.Text = _mapFile.RouteFile.ExtraHeight.ToString(System.Globalization.CultureInfo.InvariantCulture);

					ClearSelected();

//					var route = _map.Rmp.GetEntryAtHeight(_map.CurrentHeight); // this forces a selected node when RouteView opens.
//					if (route != null)
//					{
//						_currEntry = route;
//						_rmpPanel.ClickPoint = new Point(
//													_currEntry.Col,
//													_currEntry.Row);
//					}

					if ((_routePanel.MapFile = _mapFile) != null)
					{
						cbRank1.Items.Clear();

						if (_mapFile.Tiles[0][0].Palette == Palette.UFOBattle)
							cbRank1.Items.AddRange(RouteFile.UnitRankUFO);
						else
							cbRank1.Items.AddRange(RouteFile.UnitRankTFTD);

						FillRouteInformation();
					}
				}
				finally
				{
					_loadingMap = false;
				}
			}
		}

		public override void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
		{
			Text = string.Format(
							System.Globalization.CultureInfo.InvariantCulture,
							"Position{0}c:{1} r:{2}",
							Environment.NewLine, e.MapPosition.Col, e.MapPosition.Row);
		}

		public override void HeightChanged(IMap_Base sender, HeightChangedEventArgs e)
		{
			ClearSelected();
			FillRouteInformation();

			Refresh();
		}

		private void cbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected.UsableType = (UnitType)cbType.SelectedItem;
		}

		private void cbRank1_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected.UsableRank = (byte)((StrEnum)cbRank1.SelectedItem).Enum;
		}

		private void cbRank2_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected.Priority = (NodeImportance)cbRank2.SelectedItem;
			Refresh();
		}

		private void AttackBaseCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected.Attack = (BaseModuleAttack)AttackBaseCombo.SelectedItem;
		}

		private byte calcLinkDistance(RouteNode start, RouteNode end, Control textBox)
		{
			var dist = (byte)Math.Sqrt(
									Math.Pow(start.Col    - end.Col,    2) +
									Math.Pow(start.Row    - end.Row,    2) +
									Math.Pow(start.Height - end.Height, 2));
			if (textBox != null)
				textBox.Text = dist.ToString(System.Globalization.CultureInfo.InvariantCulture);

			return dist;
		}

		private void cbLink_SelectedIndexChanged(
				ComboBox sender,
				int id,
				Control textBox)
		{
			if (!_loadingGui)
			{
				var selId = sender.SelectedItem as byte?;
				if (!selId.HasValue)
					selId = (byte?)(sender.SelectedItem as LinkType?);

				if (!selId.HasValue)
				{
					MessageBox.Show(
								this,
								"SelectedIndex value failed.",
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
				{
					try
					{
						_nodeSelected[id].Destination = selId.Value;
						if (_nodeSelected[id].Destination < Link.EXIT_WEST)
						{
							var node = _mapFile.RouteFile[_nodeSelected[id].Destination];
							_nodeSelected[id].Distance = calcLinkDistance(
																	_nodeSelected,
																	node,
																	textBox);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
					Refresh();
				}
			}
		}


/*		private void cbLink_Leave(ComboBox sender, int id) // TODO: don't do any node-linking unless i vet it first.
		{
			if (!_loadingGui
				&& _nodeSelected != null
				&& sender.SelectedItem != null)
			{
				var type = GetConnectionSetting();
				if (type == ConnectNodeType.ConnectTwoWays) // is this wise, to connect OnLeave
				{
					var node = _mapFile.RouteFile[_nodeSelected[id].Destination];

					int linkId = GetOpenLinkSlot(node, (byte)sender.SelectedItem);
					if (linkId != -1)
					{
						node[linkId].Destination = _nodeSelected.Index;
						node[linkId].Distance = calcLinkDistance(
																node,
																_nodeSelected,
																null);
					}
					Refresh();
				}
			}
		} */

		private void cbLink1_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink1, 0, txtDist1);
		}

		private void cbLink1_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink1, 0); // don't do any node-linking unless i vet it first.
		}

		private void cbLink2_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink2, 1, txtDist2);
		}

		private void cbLink2_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink2, 1); // don't do any node-linking unless i vet it first.
		}

		private void cbLink3_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink3, 2, txtDist3);
		}

		private void cbLink3_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink3, 2); // don't do any node-linking unless i vet it first.
		}

		private void cbLink4_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink4, 3, txtDist4);
		}

		private void cbLink4_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink4, 3); // don't do any node-linking unless i vet it first.
		}

		private void cbLink5_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink5, 4, txtDist5);
		}

		private void cbLink5_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink5, 4); // don't do any node-linking unless i vet it first.
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			RemoveSelected();
		}

		private void cbUse1_SelectedIndexChanged(object sender, EventArgs e)
		{

			_nodeSelected[0].UsableType = (UnitType)cbUse1.SelectedItem;
			Refresh();
		}

		private void cbUse2_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected[1].UsableType = (UnitType)cbUse2.SelectedItem;
			Refresh();
		}

		private void cbUse3_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected[2].UsableType = (UnitType)cbUse3.SelectedItem;
			Refresh();
		}

		private void cbUse4_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected[3].UsableType = (UnitType)cbUse4.SelectedItem;
			Refresh();
		}

		private void cbUse5_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected[4].UsableType = (UnitType)cbUse5.SelectedItem;
			Refresh();
		}

		private void txtDist1_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[0].Distance = byte.Parse(txtDist1.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					txtDist1.Text = _nodeSelected[0].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void txtDist1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[0].Distance = byte.Parse(txtDist1.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						txtDist1.Text = _nodeSelected[0].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void txtDist2_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[1].Distance = byte.Parse(txtDist2.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					txtDist2.Text = _nodeSelected[1].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void txtDist2_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[1].Distance = byte.Parse(txtDist2.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						txtDist2.Text = _nodeSelected[1].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void txtDist3_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[2].Distance = byte.Parse(txtDist3.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					txtDist3.Text = _nodeSelected[2].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void txtDist3_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[2].Distance = byte.Parse(txtDist3.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						txtDist3.Text = _nodeSelected[2].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void txtDist4_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[3].Distance = byte.Parse(txtDist4.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					txtDist4.Text = _nodeSelected[3].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void txtDist4_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[3].Distance = byte.Parse(txtDist4.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						txtDist4.Text = _nodeSelected[3].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void txtDist5_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[4].Distance = byte.Parse(txtDist5.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					txtDist5.Text = _nodeSelected[4].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void txtDist5_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[4].Distance = byte.Parse(txtDist5.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						txtDist5.Text = _nodeSelected[4].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void cbUsage_SelectedIndexChanged(object sender, EventArgs e)
		{
			_nodeSelected.Spawn = (SpawnUsage)((StrEnum)cbUsage.SelectedItem).Enum;
			Refresh();
		}

		public override void LoadDefaultSettings()
		{
			var brushes = _routePanel.MapBrushes;
			var pens = _routePanel.MapPens;

			var bc = new ValueChangedDelegate(BrushColorChanged);
			var pc = new ValueChangedDelegate(PenColorChanged);
			var pw = new ValueChangedDelegate(PenWidthChanged);

			var settings = Settings;
			var redPen = new Pen(new SolidBrush(Color.Red), 2);
			pens["UnselectedLinkColor"] = redPen;
			pens["UnselectedLinkWidth"] = redPen;
			settings.AddSetting(
							"UnselectedLinkColor",
							redPen.Color,
							"Color of unselected link lines",
							"Links",
							pc, false, null);
			settings.AddSetting(
							"UnselectedLinkWidth",
							2,
							"Width of unselected link lines",
							"Links",
							pw, false, null);

			var bluePen = new Pen(new SolidBrush(Color.Blue), 2);
			pens["SelectedLinkColor"] = bluePen;
			pens["SelectedLinkWidth"] = bluePen;
			settings.AddSetting(
							"SelectedLinkColor",
							bluePen.Color,
							"Color of selected link lines",
							"Links",
							pc, false, null);
			settings.AddSetting(
							"SelectedLinkWidth",
							2,
							"Width of selected link lines",
							"Links",
							pw, false, null);

			var wallPen = new Pen(new SolidBrush(Color.Black), 4);
			pens["WallColor"] = wallPen;
			pens["WallWidth"] = wallPen;
			settings.AddSetting(
							"WallColor",
							wallPen.Color,
							"Color of wall indicators",
							"View",
							pc, false, null);
			settings.AddSetting(
							"WallWidth",
							4,
							"Width of wall indicators",
							"View",
							pw, false, null);

			var gridPen = new Pen(new SolidBrush(Color.Black), 1);
			pens["GridLineColor"] = gridPen;
			pens["GridLineWidth"] = gridPen;
			settings.AddSetting(
							"GridLineColor",
							gridPen.Color,
							"Color of grid lines",
							"View",
							pc, false, null);
			settings.AddSetting(
							"GridLineWidth",
							1,
							"Width of grid lines",
							"View",
							pw, false, null);

			var selBrush = new SolidBrush(Color.Blue);
			brushes["SelectedNodeColor"] = selBrush;
			settings.AddSetting(
							"SelectedNodeColor",
							selBrush.Color,
							"Color of selected nodes",
							"Nodes",
							bc, false, null);

			var spawnBrush = new SolidBrush(Color.GreenYellow);
			brushes["SpawnNodeColor"] = spawnBrush;
			settings.AddSetting(
							"SpawnNodeColor",
							spawnBrush.Color,
							"Color of spawn nodes",
							"Nodes",
							bc, false, null);

			var nodeBrush = new SolidBrush(Color.Green);
			brushes["UnselectedNodeColor"] = nodeBrush;
			settings.AddSetting(
							"UnselectedNodeColor",
							nodeBrush.Color,
							"Color of unselected nodes",
							"Nodes",
							bc, false, null);

			var contentBrush = new SolidBrush(Color.DarkGray);
			brushes["ContentTiles"] = contentBrush;
			settings.AddSetting(
							"ContentTiles",
							contentBrush.Color,
							"Color of map tiles with a content tile",
							"Other",
							bc, false, null);

			connectNodesToolStripMenuItem.SelectedIndex = 0;
		}

		private void copyNode_Click(object sender, EventArgs e)
		{
			var nodeText = string.Format(
									System.Globalization.CultureInfo.InvariantCulture,
									"MVNode|{0}|{1}|{2}|{3}|{4}",
									cbType.SelectedIndex,
									cbRank1.SelectedIndex,
									cbRank2.SelectedIndex,
									AttackBaseCombo.SelectedIndex,
									cbUsage.SelectedIndex);
			Clipboard.SetText(nodeText);
		}

		private void pasteNode_Click(object sender, EventArgs e)
		{
			var nodeData = Clipboard.GetText().Split('|');
			if (nodeData[0] == "MVNode")
			{
				cbType.SelectedIndex			= Int32.Parse(nodeData[1], System.Globalization.CultureInfo.InvariantCulture);
				cbRank1.SelectedIndex			= Int32.Parse(nodeData[2], System.Globalization.CultureInfo.InvariantCulture);
				cbRank2.SelectedIndex			= Int32.Parse(nodeData[3], System.Globalization.CultureInfo.InvariantCulture);
				AttackBaseCombo.SelectedIndex	= Int32.Parse(nodeData[4], System.Globalization.CultureInfo.InvariantCulture);
				cbUsage.SelectedIndex			= Int32.Parse(nodeData[5], System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		private void ClearSelected()
		{
			_nodeSelected = null;
			_routePanel.ClearSelected();
		}

		private void RemoveSelected()
		{
			if (_nodeSelected != null)
			{
				_mapFile.MapChanged = true;

				_mapFile.RouteFile.RemoveEntry(_nodeSelected);

				((XCMapTile)_mapFile[_nodeSelected.Row,
									 _nodeSelected.Col,
									 _nodeSelected.Height]).Node = null;

				ClearSelected();

				gbNodeInfo.Enabled   =
				groupBox1.Enabled    =
				groupBox2.Enabled    =
				LinkGroupBox.Enabled = false;

				Refresh();
			}
		}

		private void RmpView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S
				&& _mapFile != null)
			{
				_mapFile.Save();
				e.Handled = true;
			}
		}

		private void HeightDifTextbox_TextChanged(object sender, EventArgs e)
		{
			byte bite;
			if (byte.TryParse(
						HeightDifTextbox.Text,
						System.Globalization.NumberStyles.Integer,
						System.Globalization.CultureInfo.InvariantCulture,
						out bite))
			{
				_mapFile.RouteFile.ExtraHeight = bite;
			}
			else
			{
				_mapFile.RouteFile.ExtraHeight = 0;
				HeightDifTextbox.Text = _mapFile.RouteFile.ExtraHeight.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}

			_mapFile.MapChanged |= !_loadingMap;
		}

		private void makeAllNodeRank0ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var changeCount = 0;
			foreach (RouteNode rmp in _mapFile.RouteFile)
				if (rmp.UsableRank != 0)
				{
					changeCount++;
					rmp.UsableRank = 0;
				}

			if (changeCount > 0)
			{
				_mapFile.MapChanged = true;
				MessageBox.Show(
							changeCount + " nodes were changed.",
							"Node Fix",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else
			{
				MessageBox.Show(
							"All nodes are already 0 rank.",
							"Node Fix",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}
	}
}
