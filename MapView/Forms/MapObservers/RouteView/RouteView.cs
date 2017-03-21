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
		MapObserverControl0
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
			_routePanel.MapPanelClicked += RoutePanel_Click;
			_routePanel.MouseMove += RoutePanel_MouseMove;
			_routePanel.Dock = DockStyle.Fill;

			var unitTypes = new object[]
			{
				UnitType.Any,
				UnitType.Small,
				UnitType.Large,
				UnitType.Flying,
				UnitType.FlyingLarge
			};

			cbUnitType.Items.AddRange(unitTypes);

			cbLink1UnitType.Items.AddRange(unitTypes);
			cbLink2UnitType.Items.AddRange(unitTypes);
			cbLink3UnitType.Items.AddRange(unitTypes);
			cbLink4UnitType.Items.AddRange(unitTypes);
			cbLink5UnitType.Items.AddRange(unitTypes);

			cbUnitType.DropDownStyle = ComboBoxStyle.DropDownList;

			cbLink1UnitType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink2UnitType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink3UnitType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink4UnitType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink5UnitType.DropDownStyle = ComboBoxStyle.DropDownList;

			cbSpawnRank.Items.AddRange(RouteFile.UnitRankUFO);
			cbSpawnRank.DropDownStyle = ComboBoxStyle.DropDownList;

			foreach (var value in Enum.GetValues(typeof(NodeImportance)))
				cbPriority.Items.Add(value);

			cbPriority.DropDownStyle = ComboBoxStyle.DropDownList;

			foreach (var value in Enum.GetValues(typeof(BaseModuleAttack)))
				cbAttack.Items.Add(value);

			cbAttack.DropDownStyle = ComboBoxStyle.DropDownList;

			cbLink1Dest.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink2Dest.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink3Dest.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink4Dest.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLink5Dest.DropDownStyle = ComboBoxStyle.DropDownList;

			cbSpawnWeight.Items.AddRange(RouteFile.SpawnUsage);
			cbSpawnWeight.DropDownStyle = ComboBoxStyle.DropDownList;

			DeselectNode();
		}


		private void options_click(object sender, EventArgs e)
		{
			var f = new PropertyForm("RouteViewOptions", Settings);
			f.Text = "Route View Options";
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
		
		private void RoutePanel_MouseMove(object sender, MouseEventArgs args)
		{
			var tile = _routePanel.GetTile(args.X, args.Y);
			if (tile != null && tile.Node != null)
			{
				labelCurrentMouseOver.Text = "Over " + tile.Node.Index;
			}
			else
				labelCurrentMouseOver.Text = String.Empty;

			_routePanel.Pos = new Point(args.X, args.Y);
			_routePanel.Refresh(); // mouseover refresh for RouteView.
		}

		private void RoutePanel_Click(object sender, MapPanelClickEventArgs args)
		{
			_routePanel.Focus();
			labelSelectedPos.Text = Text;

			if (_nodeSelected == null)
			{
				if ((_nodeSelected = ((XCMapTile)args.ClickTile).Node) == null
					&& args.MouseEventArgs.Button == MouseButtons.Right)
				{
					_mapFile.MapChanged = true;
					_nodeSelected = _mapFile.AddRouteNode(args.ClickLocation);
				}

				if (_nodeSelected != null)
					FillNodeInformation();
			}
			else // if a node is already selected ...
			{
				var node = ((XCMapTile)args.ClickTile).Node;

				if (node != null && !_nodeSelected.Equals(node)) // NOTE: a null node "Equals" any valid node ....
				{
					if (args.MouseEventArgs.Button == MouseButtons.Right)
						ConnectNode(node);

					_nodeSelected = node;
					FillNodeInformation();
				}
				else if (node == null)
				{
					if (args.MouseEventArgs.Button == MouseButtons.Right)
					{
						_mapFile.MapChanged = true;
						node = _mapFile.AddRouteNode(args.ClickLocation);

						ConnectNode(node);
					}

					_nodeSelected = node;
					FillNodeInformation();
				}
				// else the selected node is the node clicked.
			}
		}

		private ConnectNodeType GetConnectionSetting()
		{
			if (connectNodesToolStripMenuItem.Text == "Connect One way")
				return ConnectNodeType.ConnectOneWay;

			if (connectNodesToolStripMenuItem.Text == "Connect Two ways")
				return ConnectNodeType.ConnectTwoWays;

			return ConnectNodeType.DoNotConnect;
		}

		private void ConnectNode(RouteNode node)
		{
			var type = GetConnectionSetting();
			if (type != ConnectNodeType.DoNotConnect)
			{
				int linkId = GetOpenLinkSlot(_nodeSelected, node.Index);
				if (linkId != -1)
				{
					_nodeSelected[linkId].Destination = node.Index;
					_nodeSelected[linkId].Distance = CalculateLinkDistance(_nodeSelected, node);
				}

				if (type == ConnectNodeType.ConnectTwoWays)
				{
					linkId = GetOpenLinkSlot(node, _nodeSelected.Index);
					if (linkId != -1)
					{
						node[linkId].Destination = _nodeSelected.Index;
						node[linkId].Distance = CalculateLinkDistance(node, _nodeSelected);
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

		private void FillNodeInformation()
		{
			_loadingGui = true;

			gbNodeData.SuspendLayout();
			gbPatrolData.SuspendLayout();
			gbSpawnData.SuspendLayout();
			gbLinkData.SuspendLayout();

			if (_nodeSelected == null)
			{
				labelSelected.Text = String.Empty;

				gbNodeData.Enabled   =
				gbPatrolData.Enabled =
				gbSpawnData.Enabled  =
				gbLinkData.Enabled   = false;

				cbUnitType.SelectedItem = UnitType.Any;
				cbPriority.SelectedItem = NodeImportance.Zero;
				cbAttack.SelectedItem   = BaseModuleAttack.Zero;

				if (_mapFile.Tiles[0][0].Palette == Palette.UFOBattle)
					cbSpawnRank.SelectedItem = RouteFile.UnitRankUFO[(int)UnitRankUFO.Civilian];
				else
					cbSpawnRank.SelectedItem = RouteFile.UnitRankUFO[(int)UnitRankTFTD.Civilian];

				cbSpawnWeight.SelectedItem = RouteFile.SpawnUsage[(int)SpawnUsage.NoSpawn];

				cbLink1Dest.SelectedItem = LinkType.NotUsed;
				cbLink2Dest.SelectedItem = LinkType.NotUsed;
				cbLink3Dest.SelectedItem = LinkType.NotUsed;
				cbLink4Dest.SelectedItem = LinkType.NotUsed;
				cbLink5Dest.SelectedItem = LinkType.NotUsed;

				cbLink1UnitType.SelectedItem = UnitType.Any;
				cbLink2UnitType.SelectedItem = UnitType.Any;
				cbLink3UnitType.SelectedItem = UnitType.Any;
				cbLink4UnitType.SelectedItem = UnitType.Any;
				cbLink5UnitType.SelectedItem = UnitType.Any;

				tbLink1Dist.Text = "0";
				tbLink2Dist.Text = "0";
				tbLink3Dist.Text = "0";
				tbLink4Dist.Text = "0";
				tbLink5Dist.Text = "0";
			}
			else
			{
				labelSelected.Text = "Selected " + _nodeSelected.Index;

				gbNodeData.Enabled   =
				gbPatrolData.Enabled =
				gbSpawnData.Enabled  =
				gbLinkData.Enabled   = true;


				cbUnitType.SelectedItem = _nodeSelected.UsableType;
				cbPriority.SelectedItem = _nodeSelected.Priority;
				cbAttack.SelectedItem   = _nodeSelected.Attack;

				if (_mapFile.Tiles[0][0].Palette == Palette.UFOBattle)
					cbSpawnRank.SelectedItem = RouteFile.UnitRankUFO[_nodeSelected.UsableRank];
				else
					cbSpawnRank.SelectedItem = RouteFile.UnitRankTFTD[_nodeSelected.UsableRank];

				cbSpawnWeight.SelectedItem = RouteFile.SpawnUsage[(byte)_nodeSelected.Spawn];

				cbLink1Dest.Items.Clear();
				cbLink2Dest.Items.Clear();
				cbLink3Dest.Items.Clear();
				cbLink4Dest.Items.Clear();
				cbLink5Dest.Items.Clear();

				_linksList.Clear();

				for (byte i = 0; i != _mapFile.RouteFile.Length; ++i)
					if (i != _nodeSelected.Index)
						_linksList.Add(i); // add all linkable (ie. other) nodes

				var linkTypes = new object[]
				{
					LinkType.NotUsed,
					LinkType.ExitNorth,
					LinkType.ExitEast,
					LinkType.ExitSouth,
					LinkType.ExitWest
				};
				_linksList.AddRange(linkTypes); // add the four compass-points + link-not-used.

				object[] linkListArray = _linksList.ToArray();

				cbLink1Dest.Items.AddRange(linkListArray);
				cbLink2Dest.Items.AddRange(linkListArray);
				cbLink3Dest.Items.AddRange(linkListArray);
				cbLink4Dest.Items.AddRange(linkListArray);
				cbLink5Dest.Items.AddRange(linkListArray);

				if (_nodeSelected[0].Destination < Link.EXIT_WEST)
					cbLink1Dest.SelectedItem = _nodeSelected[0].Destination;
				else
					cbLink1Dest.SelectedItem = (LinkType)_nodeSelected[0].Destination;

				if (_nodeSelected[1].Destination < Link.EXIT_WEST)
					cbLink2Dest.SelectedItem = _nodeSelected[1].Destination;
				else
					cbLink2Dest.SelectedItem = (LinkType)_nodeSelected[1].Destination;

				if (_nodeSelected[2].Destination < Link.EXIT_WEST)
					cbLink3Dest.SelectedItem = _nodeSelected[2].Destination;
				else
					cbLink3Dest.SelectedItem = (LinkType)_nodeSelected[2].Destination;

				if (_nodeSelected[3].Destination < Link.EXIT_WEST)
					cbLink4Dest.SelectedItem = _nodeSelected[3].Destination;
				else
					cbLink4Dest.SelectedItem = (LinkType)_nodeSelected[3].Destination;

				if (_nodeSelected[4].Destination < Link.EXIT_WEST)
					cbLink5Dest.SelectedItem = _nodeSelected[4].Destination;
				else
					cbLink5Dest.SelectedItem = (LinkType)_nodeSelected[4].Destination;

				cbLink1UnitType.SelectedItem = _nodeSelected[0].UsableType;
				cbLink2UnitType.SelectedItem = _nodeSelected[1].UsableType;
				cbLink3UnitType.SelectedItem = _nodeSelected[2].UsableType;
				cbLink4UnitType.SelectedItem = _nodeSelected[3].UsableType;
				cbLink5UnitType.SelectedItem = _nodeSelected[4].UsableType;

				tbLink1Dist.Text = Convert.ToString(_nodeSelected[0].Distance, System.Globalization.CultureInfo.InvariantCulture);
				tbLink2Dist.Text = Convert.ToString(_nodeSelected[1].Distance, System.Globalization.CultureInfo.InvariantCulture);
				tbLink3Dist.Text = Convert.ToString(_nodeSelected[2].Distance, System.Globalization.CultureInfo.InvariantCulture);
				tbLink4Dist.Text = Convert.ToString(_nodeSelected[3].Distance, System.Globalization.CultureInfo.InvariantCulture);
				tbLink5Dist.Text = Convert.ToString(_nodeSelected[4].Distance, System.Globalization.CultureInfo.InvariantCulture);
			}

			gbNodeData.ResumeLayout();
			gbPatrolData.ResumeLayout();
			gbSpawnData.ResumeLayout();
			gbLinkData.ResumeLayout();

			_loadingGui = false;
		}

		public void SetMap(object sender, SetMapEventArgs args)
		{
			Map = args.Map;
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
					textboxExtraHeight.Text = _mapFile.RouteFile.ExtraHeight.ToString(System.Globalization.CultureInfo.InvariantCulture);

					DeselectNode();

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
						cbSpawnRank.Items.Clear();

						if (_mapFile.Tiles[0][0].Palette == Palette.UFOBattle)
							cbSpawnRank.Items.AddRange(RouteFile.UnitRankUFO);
						else
							cbSpawnRank.Items.AddRange(RouteFile.UnitRankTFTD);

						FillNodeInformation();
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
			DeselectNode();
			FillNodeInformation();

			Refresh();
		}

		private void cbUnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
				_nodeSelected.UsableType = (UnitType)cbUnitType.SelectedItem;
		}

		private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected.Priority = (NodeImportance)cbPriority.SelectedItem;
				Refresh();
			}
		}

		private void cbAttack_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
				_nodeSelected.Attack = (BaseModuleAttack)cbAttack.SelectedItem;
		}

		private void cbSpawnRank_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
				_nodeSelected.UsableRank = (byte)((StrEnum)cbSpawnRank.SelectedItem).Enum;
		}

		private void cbSpawnWeight_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected.Spawn = (SpawnUsage)((StrEnum)cbSpawnWeight.SelectedItem).Enum;
				Refresh();
			}
		}

		private void cbLink_SelectedIndexChanged(
				ComboBox sender,
				int linkId,
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
						_nodeSelected[linkId].Destination = selId.Value;
						if (_nodeSelected[linkId].Destination < Link.EXIT_WEST)
						{
							var node = _mapFile.RouteFile[_nodeSelected[linkId].Destination];
							_nodeSelected[linkId].Distance = CalculateLinkDistance(
																				_nodeSelected,
																				node,
																				textBox);
						}
						else
							_nodeSelected[linkId].Distance = 0;
					}
					catch (Exception ex)
					{
						MessageBox.Show(
									this,
									ex.Message,
									"Exception",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1,
									0);
						throw;
					}
					Refresh();
				}
			}
		}

		private static byte CalculateLinkDistance(
				RouteNode nodeA,
				RouteNode nodeB,
				Control textBox = null)
		{
			var dist = (byte)Math.Sqrt(
									Math.Pow(nodeA.Col    - nodeB.Col,    2) +
									Math.Pow(nodeA.Row    - nodeB.Row,    2) +
									Math.Pow(nodeA.Height - nodeB.Height, 2));
			if (textBox != null)
				textBox.Text = dist.ToString(System.Globalization.CultureInfo.InvariantCulture);

			return dist;
		}


/*		private void cbLink_Leave(ComboBox sender, int id) // TODO: don't do any node-linking OnLeave unless i vet it first.
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

		private void cbLink1Dest_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink1Dest, 0, tbLink1Dist);
		}

		private void cbLink1Dest_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink1, 0); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void cbLink2_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink2Dest, 1, tbLink2Dist);
		}

		private void cbLink2Dest_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink2, 1); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void cbLink3Dest_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink3Dest, 2, tbLink3Dist);
		}

		private void cbLink3Dest_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink3, 2); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void cbLink4Dest_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink4Dest, 3, tbLink4Dist);
		}

		private void cbLink4Dest_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink4, 3); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void cbLink5Dest_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbLink_SelectedIndexChanged(cbLink5Dest, 4, tbLink5Dist);
		}

		private void cbLink5Dest_Leave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink5, 4); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void cbLink1UnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected[0].UsableType = (UnitType)cbLink1UnitType.SelectedItem;
				Refresh();
			}
		}

		private void cbLink2UnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected[1].UsableType = (UnitType)cbLink2UnitType.SelectedItem;
				Refresh();
			}
		}

		private void cbLink3UnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected[2].UsableType = (UnitType)cbLink3UnitType.SelectedItem;
				Refresh();
			}
		}

		private void cbLink4UnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected[3].UsableType = (UnitType)cbLink4UnitType.SelectedItem;
				Refresh();
			}
		}

		private void cbLink5UnitType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingGui)
			{
				_nodeSelected[4].UsableType = (UnitType)cbLink5UnitType.SelectedItem;
				Refresh();
			}
		}

		private void tbLink1Dist_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[0].Distance = byte.Parse(tbLink1Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						tbLink1Dist.Text = _nodeSelected[0].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void tbLink1Dist_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[0].Distance = byte.Parse(tbLink1Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					tbLink1Dist.Text = _nodeSelected[0].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void tbLink2Dist_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[1].Distance = byte.Parse(tbLink2Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						tbLink2Dist.Text = _nodeSelected[1].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void tbLink2Dist_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[1].Distance = byte.Parse(tbLink2Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					tbLink2Dist.Text = _nodeSelected[1].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void tbLink3Dist_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[2].Distance = byte.Parse(tbLink3Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						tbLink3Dist.Text = _nodeSelected[2].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void tbLink3Dist_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[2].Distance = byte.Parse(tbLink3Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					tbLink3Dist.Text = _nodeSelected[2].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void tbLink4Dist_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[3].Distance = byte.Parse(tbLink4Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						tbLink4Dist.Text = _nodeSelected[3].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void tbLink4Dist_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[3].Distance = byte.Parse(tbLink4Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					tbLink4Dist.Text = _nodeSelected[3].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void tbLink5Dist_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					try
					{
						_nodeSelected[4].Distance = byte.Parse(tbLink5Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						tbLink5Dist.Text = _nodeSelected[4].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
						throw;
					}
					break;
			}
		}

		private void tbLink5Dist_Leave(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				try
				{
					_nodeSelected[4].Distance = byte.Parse(tbLink5Dist.Text, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					tbLink5Dist.Text = _nodeSelected[4].Distance.ToString(System.Globalization.CultureInfo.InvariantCulture);
					throw;
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var nodeText = string.Format(
									System.Globalization.CultureInfo.InvariantCulture,
									"MVNode|{0}|{1}|{2}|{3}|{4}",
									cbUnitType.SelectedIndex,
									cbSpawnRank.SelectedIndex,
									cbPriority.SelectedIndex,
									cbAttack.SelectedIndex,
									cbSpawnWeight.SelectedIndex);
			Clipboard.SetText(nodeText);
		}

		private void btnPaste_Click(object sender, EventArgs e)
		{
			var nodeData = Clipboard.GetText().Split('|');
			if (nodeData[0] == "MVNode")
			{
				cbUnitType.SelectedIndex    = Int32.Parse(nodeData[1], System.Globalization.CultureInfo.InvariantCulture);
				cbSpawnRank.SelectedIndex   = Int32.Parse(nodeData[2], System.Globalization.CultureInfo.InvariantCulture);
				cbPriority.SelectedIndex    = Int32.Parse(nodeData[3], System.Globalization.CultureInfo.InvariantCulture);
				cbAttack.SelectedIndex      = Int32.Parse(nodeData[4], System.Globalization.CultureInfo.InvariantCulture);
				cbSpawnWeight.SelectedIndex = Int32.Parse(nodeData[5], System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (_nodeSelected != null)
			{
				_mapFile.MapChanged = true;

				_mapFile.RouteFile.Delete(_nodeSelected);

				((XCMapTile)_mapFile[_nodeSelected.Row,
									 _nodeSelected.Col,
									 _nodeSelected.Height]).Node = null;

				DeselectNode();

				gbSpawnData.Enabled  =
				gbPatrolData.Enabled =
				gbNodeData.Enabled   =
				gbLinkData.Enabled   = false;

				Refresh();
			}
		}

		private void DeselectNode()
		{
			_nodeSelected = null;
			_routePanel.DeselectLocation();
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

		private void ExtraHeight_TextChanged(object sender, EventArgs e)
		{
			byte bite;
			if (byte.TryParse(
							textboxExtraHeight.Text,
							System.Globalization.NumberStyles.Integer,
							System.Globalization.CultureInfo.InvariantCulture,
							out bite))
			{
				_mapFile.RouteFile.ExtraHeight = bite;
				textboxExtraHeight.Text = bite.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				_mapFile.RouteFile.ExtraHeight = 0;
				textboxExtraHeight.Text = "0";
			}

			_mapFile.MapChanged |= !_loadingMap;
		}

		private void makeAllNodeRank0ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var changeCount = 0;
			foreach (RouteNode node in _mapFile.RouteFile)
				if (node.UsableRank != 0)
				{
					++changeCount;
					node.UsableRank = 0;
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

		/// <summary>
		/// Loads default settings for RouteView in TopRouteView screen.
		/// </summary>
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
	}
}
