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

	internal sealed partial class RouteView
		:
			MapObserverControl0
	{
		private enum ConnectNodeType
		{
			ConnectNone,
			ConnectOneWay,
			ConnectTwoWays
		}

		#region Fields
		private readonly RoutePanel _routePanel;
		internal RoutePanel RoutePanel
		{
			get { return _routePanel; }
		}

		private Panel pRoutes;

		private MapFileChild _mapFile;

		private RouteNode _nodeSelected;
		internal RouteNode NodeSelected
		{
			get { return _nodeSelected; }
			set
			{
				_nodeSelected           =
				RoutePanel.NodeSelected = value;
			}
		}

		private bool _loadingInfo;

		private readonly List<object> _linksList = new List<object>();

		private const string DontConnect   = " DontConnect"; // NOTE: the space is 'cause ComboBox entries don't get a Margin property.
		private const string OneWayConnect = " OneWayConnect";
		private const string TwoWayConnect = " TwoWayConnect";

		private int _col; // these are used only to print the clicked location info.
		private int _row;
		private int _lev;
		#endregion


		#region Properties
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		public override MapFileBase MapBase
		{
			set
			{
				base.MapBase = value;
				_mapFile     = value as MapFileChild;

				DeselectNode();

				if ((_routePanel.MapFile = _mapFile) != null)
				{
					cbSpawnRank.Items.Clear();

					if (_mapFile.Parts[0][0].Palette == Palette.UfoBattle)				// ie. Get the palette of the 1st image of the
						cbSpawnRank.Items.AddRange(RouteNodeCollection.UnitRankUfo);	// 1st tilepart ... of the MapFileChild object.
					else
						cbSpawnRank.Items.AddRange(RouteNodeCollection.UnitRankTftd);

					UpdateNodeInformation();
				}
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Instantiates the RouteView viewer and its components/controls.
		/// </summary>
		public RouteView()
		{
			InitializeComponent();

			_routePanel = new RoutePanel();
			_routePanel.Dock = DockStyle.Fill;
			_routePanel.RoutePanelClickedEvent += OnRoutePanelClicked;
			_routePanel.MouseMove              += OnRoutePanelMouseMove;
			_routePanel.MouseLeave             += OnRoutePanelMouseLeave;
			_routePanel.KeyDown                += OnKeyDown;
			pRoutes.Controls.Add(_routePanel);

			// setup the connect-type dropdown entries
			tscbConnectType.Items.AddRange(new object[]
			{
				DontConnect,
				OneWayConnect,
				TwoWayConnect,
			});


			var unitTypes = new object[]
			{
				UnitType.Any,
				UnitType.Small,
				UnitType.Large,
				UnitType.Flying,
				UnitType.FlyingLarge
			};

			// patrol data ->
			cbUnitType.Items.AddRange(unitTypes);

			foreach (var value in Enum.GetValues(typeof(NodeImportance)))
				cbPriority.Items.Add(value);

			foreach (var value in Enum.GetValues(typeof(BaseModuleAttack)))
				cbAttack.Items.Add(value);

			// spawn data ->
			cbSpawnRank.Items.AddRange(RouteNodeCollection.UnitRankUfo);

			cbSpawnWeight.Items.AddRange(RouteNodeCollection.SpawnUsage);

			// link data ->
			cbLink1UnitType.Items.AddRange(unitTypes);
			cbLink2UnitType.Items.AddRange(unitTypes);
			cbLink3UnitType.Items.AddRange(unitTypes);
			cbLink4UnitType.Items.AddRange(unitTypes);
			cbLink5UnitType.Items.AddRange(unitTypes);

			// TODO: change the distance textboxes to labels.

			// set all dropdowns' ComboBoxStyle ->
			cbUnitType.DropDownStyle      =
			cbPriority.DropDownStyle      =
			cbAttack.DropDownStyle        =

			cbSpawnRank.DropDownStyle     =
			cbSpawnWeight.DropDownStyle   =

			cbLink1Dest.DropDownStyle     =
			cbLink2Dest.DropDownStyle     =
			cbLink3Dest.DropDownStyle     =
			cbLink4Dest.DropDownStyle     =
			cbLink5Dest.DropDownStyle     =

			cbLink1UnitType.DropDownStyle =
			cbLink2UnitType.DropDownStyle =
			cbLink3UnitType.DropDownStyle =
			cbLink4UnitType.DropDownStyle =
			cbLink5UnitType.DropDownStyle = ComboBoxStyle.DropDownList;

			DeselectNode();
		}
		#endregion


		#region EventCalls inherited from IMapObserver/MapObserverControl0
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("RouteView.OnLocationSelectedObserver");

			// as long as MainViewOverlay.OnLocationSelected_Main()
			// fires before the subsidiary viewers' OnLocationSelectedObserver()
			// functions fire, FirstClick is set okay by the former.
			//
			// See also, TopViewPanelParent.OnLocationSelectedObserver()
//			MainViewUnderlay.Instance.MainViewOverlay.FirstClick = true;

			_col = args.Location.Col;
			_row = args.Location.Row;
			_lev = args.Location.Lev;
			PrintSelectedLocation();
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl0.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLevelChangedObserver(LevelChangedEventArgs args)
		{
			_lev = args.Level;
			PrintSelectedLocation();

			DeselectNode();
			UpdateNodeInformation();

			Refresh();
		}
		#endregion


		/// <summary>
		/// Prints the currently selected location to the panel.
		/// NOTE: The displayed level is inverted here.
		/// </summary>
		private void PrintSelectedLocation()
		{
			if (MainViewUnderlay.Instance.MainViewOverlay.FirstClick)
				lblSelectedPosition.Text = String.Format(
													System.Globalization.CultureInfo.InvariantCulture,
													"c {0}  r {1}  L {2}",
													_col, _row, _mapFile.MapSize.Levs - _lev);
		}

		/// <summary>
		/// Clears the selected location text when another Map loads.
		/// </summary>
		internal void ClearSelectedLocation()
		{
			lblSelectedPosition.Text = String.Empty;
		}


		private Form _foptions;
		private bool _closing;

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (!it.Checked)
			{
				it.Checked = true;

				_foptions = new OptionsForm("RouteViewOptions", Options);
				_foptions.Text = "RouteView Options";

				_foptions.Show();

				_foptions.FormClosing += (sender1, e1) =>
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

		private void OnRoutePanelMouseMove(object sender, MouseEventArgs args)
		{
			var tile = _routePanel.GetTile(args.X, args.Y);
			if (tile != null && tile.Node != null)
			{
				lblOverId.Text = "Over " + tile.Node.Index;
			}
			else
				lblOverId.Text = String.Empty;

			_routePanel.CursorPosition = new Point(args.X, args.Y);
			_routePanel.Refresh();	// 3nd mouseover refresh for RouteView.
		}							// See OnRoutePanelMouseLeave(), RoutePanelParent.OnMouseMove()

		/// <summary>
		/// Hides the info-overlay when the mouse leaves this control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRoutePanelMouseLeave(object sender, EventArgs e)
		{
			_routePanel.CursorPosition = new Point(-1, -1);
			_routePanel.Refresh();	// 3rd mouseover refresh for RouteView.
		}							// See OnRoutePanelMouseMove(), RoutePanelParent.OnMouseMove()

		/// <summary>
		/// Selects a node on LMB, creates and/or connects nodes on RMB.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnRoutePanelClicked(object sender, RoutePanelClickedEventArgs args)
		{
			_routePanel.Focus();

			if (NodeSelected == null)
			{
				if ((NodeSelected = ((XCMapTile)args.ClickedTile).Node) == null
					&& args.MouseEventArgs.Button == MouseButtons.Right)
				{
					NodeSelected = _mapFile.AddRouteNode(args.ClickedLocation);
				}

				if (NodeSelected != null)
					UpdateNodeInformation();
			}
			else // if a node is already selected ...
			{
				var node = ((XCMapTile)args.ClickedTile).Node;

				if (node != null && !node.Equals(NodeSelected)) // NOTE: a null node "Equals" any valid node ....
				{
					if (args.MouseEventArgs.Button == MouseButtons.Right)
						ConnectNode(node);

					NodeSelected = node;
					UpdateNodeInformation();
				}
				else if (node == null)
				{
					if (args.MouseEventArgs.Button == MouseButtons.Right)
					{
						node = _mapFile.AddRouteNode(args.ClickedLocation);
						ConnectNode(node);
					}

					NodeSelected = node;
					UpdateNodeInformation();
				}
				// else the selected node is the node clicked.
			}

			if (NodeSelected != null)
			{
				btnCut.Enabled    =
				btnCopy.Enabled   =
				btnDelete.Enabled = true;

				var nodeData = Clipboard.GetText().Split(NodeCopySeparator);
				if (nodeData[0] == NodeCopyPrefix)
					btnPaste.Enabled = true;

				tsmiClearLinkData.Enabled = true;
			}
			else
			{
				btnCut.Enabled    =
				btnCopy.Enabled   =
				btnPaste.Enabled  =
				btnDelete.Enabled = false;

				tsmiClearLinkData.Enabled = false;
			}
		}

		/// <summary>
		/// Checks connector and connects nodes if applicable.
		/// </summary>
		/// <param name="node">the node to try to link the currently selected
		/// node to</param>
		private void ConnectNode(RouteNode node)
		{
			var type = GetConnectorType();
			if (type != ConnectNodeType.ConnectNone)
			{
				int linkId = GetOpenLinkSlot(NodeSelected, node.Index);
				if (linkId != -1)
				{
					_mapFile.MapChanged = true;
					NodeSelected[linkId].Destination = node.Index;
					NodeSelected[linkId].Distance = CalculateLinkDistance(NodeSelected, node);
				}

				if (type == ConnectNodeType.ConnectTwoWays)
				{
					linkId = GetOpenLinkSlot(node, NodeSelected.Index);
					if (linkId != -1)
					{
						_mapFile.MapChanged = true;
						node[linkId].Destination = NodeSelected.Index;
						node[linkId].Distance = CalculateLinkDistance(node, NodeSelected);
					}
				}
			}
		}

		/// <summary>
		/// Gets the user-selected Connector type.
		/// </summary>
		/// <returns></returns>
		private ConnectNodeType GetConnectorType()
		{
			if (tscbConnectType.Text == OneWayConnect)
				return ConnectNodeType.ConnectOneWay;

			if (tscbConnectType.Text == TwoWayConnect)
				return ConnectNodeType.ConnectTwoWays;

			return ConnectNodeType.ConnectNone;
		}

		/// <summary>
		/// Gets the first available link-slot, else -1. Also returns -1 if the
		/// node already has a link to destination.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="idOther"></param>
		/// <returns></returns>
		private static int GetOpenLinkSlot(RouteNode node, int idOther)
		{
			if (node != null)
			{
				for (int i = 0; i != RouteNode.LinkSlots; ++i) // first check if destination-id already exists
				{
					if (idOther != -1 && node[i].Destination == idOther)
						return -1;
				}

				for (int i = 0; i != RouteNode.LinkSlots; ++i) // then check for an open slot
				{
					if (node[i].Destination == (byte)LinkType.NotUsed)
						return i;
				}
			}
			return -1;
		}

		private void UpdateNodeInformation()
		{
			_loadingInfo = true;

			gbNodeData.SuspendLayout();
			gbPatrolData.SuspendLayout();
			gbSpawnData.SuspendLayout();
			gbLinkData.SuspendLayout();
			gbNodeEditor.SuspendLayout();

			if (NodeSelected == null)
			{
				lblSelectedId.Text = String.Empty;

				btnCut.Enabled    =
				btnCopy.Enabled   =
				btnPaste.Enabled  =
				btnDelete.Enabled = false;

				gbNodeData.Enabled   =
				gbPatrolData.Enabled =
				gbSpawnData.Enabled  =
				gbLinkData.Enabled   =
				gbNodeEditor.Enabled = false;


				cbUnitType.SelectedItem = UnitType.Any;
				cbPriority.SelectedItem = NodeImportance.Zero;
				cbAttack.SelectedItem   = BaseModuleAttack.Zero;

				if (_mapFile.Parts[0][0].Palette == Palette.UfoBattle)
					cbSpawnRank.SelectedItem = RouteNodeCollection.UnitRankUfo[(int)UnitRankUfo.Civilian];
				else
					cbSpawnRank.SelectedItem = RouteNodeCollection.UnitRankUfo[(int)UnitRankTftd.Civilian];

				cbSpawnWeight.SelectedItem = RouteNodeCollection.SpawnUsage[(int)SpawnUsage.NoSpawn];

				cbLink1Dest.SelectedItem = // TODO: figure out why these show blank and not "NotUsed"
				cbLink2Dest.SelectedItem = // when the app loads its very first map.
				cbLink3Dest.SelectedItem =
				cbLink4Dest.SelectedItem =
				cbLink5Dest.SelectedItem = LinkType.NotUsed;

				cbLink1UnitType.SelectedItem =
				cbLink2UnitType.SelectedItem =
				cbLink3UnitType.SelectedItem =
				cbLink4UnitType.SelectedItem =
				cbLink5UnitType.SelectedItem = UnitType.Any;

				tbLink1Dist.Text =
				tbLink2Dist.Text =
				tbLink3Dist.Text =
				tbLink4Dist.Text =
				tbLink5Dist.Text = String.Empty;
			}
			else
			{
				lblSelectedId.Text = "Selected " + NodeSelected.Index;

				gbNodeData.Enabled   =
				gbPatrolData.Enabled =
				gbSpawnData.Enabled  =
				gbLinkData.Enabled   =
				gbNodeEditor.Enabled = true;


				cbUnitType.SelectedItem = NodeSelected.UsableType;
				cbPriority.SelectedItem = NodeSelected.Priority;
				cbAttack.SelectedItem   = NodeSelected.Attack;

				if (_mapFile.Parts[0][0].Palette == Palette.UfoBattle)
					cbSpawnRank.SelectedItem = RouteNodeCollection.UnitRankUfo[NodeSelected.SpawnRank];
				else
					cbSpawnRank.SelectedItem = RouteNodeCollection.UnitRankTftd[NodeSelected.SpawnRank];

				cbSpawnWeight.SelectedItem = RouteNodeCollection.SpawnUsage[(byte)NodeSelected.SpawnWeight];

				cbLink1Dest.Items.Clear();
				cbLink2Dest.Items.Clear();
				cbLink3Dest.Items.Clear();
				cbLink4Dest.Items.Clear();
				cbLink5Dest.Items.Clear();

				_linksList.Clear();

				for (byte i = 0; i != _mapFile.Routes.Length; ++i)
				{
					if (i != NodeSelected.Index)
						_linksList.Add(i); // add all linkable (ie. other) nodes
				}

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

				if (NodeSelected[0].Destination < Link.ExitWest)
					cbLink1Dest.SelectedItem = NodeSelected[0].Destination;
				else
					cbLink1Dest.SelectedItem = (LinkType)NodeSelected[0].Destination;

				if (NodeSelected[1].Destination < Link.ExitWest)
					cbLink2Dest.SelectedItem = NodeSelected[1].Destination;
				else
					cbLink2Dest.SelectedItem = (LinkType)NodeSelected[1].Destination;

				if (NodeSelected[2].Destination < Link.ExitWest)
					cbLink3Dest.SelectedItem = NodeSelected[2].Destination;
				else
					cbLink3Dest.SelectedItem = (LinkType)NodeSelected[2].Destination;

				if (NodeSelected[3].Destination < Link.ExitWest)
					cbLink4Dest.SelectedItem = NodeSelected[3].Destination;
				else
					cbLink4Dest.SelectedItem = (LinkType)NodeSelected[3].Destination;

				if (NodeSelected[4].Destination < Link.ExitWest)
					cbLink5Dest.SelectedItem = NodeSelected[4].Destination;
				else
					cbLink5Dest.SelectedItem = (LinkType)NodeSelected[4].Destination;

				cbLink1UnitType.SelectedItem = NodeSelected[0].UsableType;
				cbLink2UnitType.SelectedItem = NodeSelected[1].UsableType;
				cbLink3UnitType.SelectedItem = NodeSelected[2].UsableType;
				cbLink4UnitType.SelectedItem = NodeSelected[3].UsableType;
				cbLink5UnitType.SelectedItem = NodeSelected[4].UsableType;


				if (NodeSelected[0].Destination == Link.NotUsed)
					tbLink1Dist.Text = String.Empty;
				else
				{
					tbLink1Dist.Text = Convert.ToString(
													NodeSelected[0].Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									 + GetDistanceSuffix(0);
				}

				if (NodeSelected[1].Destination == Link.NotUsed)
					tbLink2Dist.Text = String.Empty;
				else
					tbLink2Dist.Text = Convert.ToString(
													NodeSelected[1].Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									 + GetDistanceSuffix(1);

				if (NodeSelected[2].Destination == Link.NotUsed)
					tbLink3Dist.Text = String.Empty;
				else
					tbLink3Dist.Text = Convert.ToString(
													NodeSelected[2].Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									 + GetDistanceSuffix(2);

				if (NodeSelected[3].Destination == Link.NotUsed)
					tbLink4Dist.Text = String.Empty;
				else
					tbLink4Dist.Text = Convert.ToString(
													NodeSelected[3].Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									 + GetDistanceSuffix(3);

				if (NodeSelected[4].Destination == Link.NotUsed)
					tbLink5Dist.Text = String.Empty;
				else
					tbLink5Dist.Text = Convert.ToString(
													NodeSelected[4].Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									 + GetDistanceSuffix(4);
			}

			gbNodeData.ResumeLayout();
			gbPatrolData.ResumeLayout();
			gbSpawnData.ResumeLayout();
			gbLinkData.ResumeLayout();
			gbNodeEditor.ResumeLayout();

			_loadingInfo = false;
		}

		/// <summary>
		/// Gets an up/down suffix for the linked distance from the currently
		/// selected node, given the link-slot to the destination node. If the
		/// destination is on the same level as the selected node, a blank
		/// string is returned.
		/// </summary>
		/// <param name="slotId"></param>
		/// <returns></returns>
		private string GetDistanceSuffix(int slotId)
		{
			if (NodeSelected[slotId].Destination < Link.ExitWest)
			{
				var nodeDst = _mapFile.Routes[NodeSelected[slotId].Destination];
				if (nodeDst != null)
				{
					if (NodeSelected.Lev > nodeDst.Lev)
						return " \u2191"; // up arrow

					if (NodeSelected.Lev < nodeDst.Lev)
						return " \u2193"; // down arrow
				}
			}
			return String.Empty;
		}


		private void OnUnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected.UsableType = (UnitType)cbUnitType.SelectedItem;
			}
		}

		private void OnPatrolPrioritySelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected.Priority = (NodeImportance)cbPriority.SelectedItem;
				Refresh(); // update the importance bar
			}
		}

		private void OnBaseAttackSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected.Attack = (BaseModuleAttack)cbAttack.SelectedItem;
			}
		}

		private void OnSpawnRankSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected.SpawnRank = (byte)((EnumString)cbSpawnRank.SelectedItem).Enum;
			}
		}

		private void OnSpawnWeightSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected.SpawnWeight = (SpawnUsage)((EnumString)cbSpawnWeight.SelectedItem).Enum;
				Refresh(); // update the importance bar
			}
		}


		private void LinkDestinationSelectedIndexChanged(
				ComboBox sender,
				int slotId,
				Control textBox)
		{
			if (!_loadingInfo)
			{
				var dst = sender.SelectedItem as byte?;

				if (!dst.HasValue)
					dst = (byte?)(sender.SelectedItem as LinkType?);

				if (!dst.HasValue)
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
//					try
//					{
					_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

					switch (NodeSelected[slotId].Destination = dst.Value)
					{
						case Link.NotUsed:
							NodeSelected[slotId].Distance = 0;
							textBox.Text = String.Empty;
							break;
						case Link.ExitWest:
						case Link.ExitNorth:
						case Link.ExitEast:
						case Link.ExitSouth:
							NodeSelected[slotId].Distance = 0;
							textBox.Text = "0";
							break;

						default:
							var node = _mapFile.Routes[NodeSelected[slotId].Destination];
							NodeSelected[slotId].Distance = CalculateLinkDistance(
																				NodeSelected,
																				node,
																				textBox,
																				slotId);
							break;
					}
//					}
//					catch (Exception ex)
//					{
//						MessageBox.Show(
//									this,
//									ex.Message,
//									"Exception",
//									MessageBoxButtons.OK,
//									MessageBoxIcon.Error,
//									MessageBoxDefaultButton.Button1,
//									0);
//						throw;
//					}
					Refresh();
				}
			}
		}

		/// <summary>
		/// Calculates the distance between two nodes by Pythagoras.
		/// </summary>
		/// <param name="nodeA">a RouteNode</param>
		/// <param name="nodeB">another RouteNode</param>
		/// <param name="textBox">the textbox that shows the distance (default null)</param>
		/// <param name="slotId">the slot of the textbox - not used unless 'textBox'
		/// is specified (default -1)</param>
		/// <returns></returns>
		private byte CalculateLinkDistance(
				RouteNode nodeA,
				RouteNode nodeB,
				Control textBox = null,
				int slotId = 0)
		{
			var dist = (byte)Math.Sqrt(
									Math.Pow(nodeA.Col - nodeB.Col, 2) +
									Math.Pow(nodeA.Row - nodeB.Row, 2) +
									Math.Pow(nodeA.Lev - nodeB.Lev, 2));
			if (textBox != null)
				textBox.Text = dist.ToString(System.Globalization.CultureInfo.InvariantCulture)
							 + GetDistanceSuffix(slotId);

			return dist;
		}


/*		private void cbLink_Leave(ComboBox sender, int id) // TODO: don't do any node-linking OnLeave unless i vet it first.
		{
			if (!_loadingInfo
				&& NodeSelected != null
				&& sender.SelectedItem != null)
			{
				var type = GetConnectionSetting();
				if (type == ConnectNodeType.ConnectTwoWays) // is this wise, to connect OnLeave
				{
					var node = _mapFile.RouteFile[NodeSelected[id].Destination];

					int linkId = GetOpenLinkSlot(node, (byte)sender.SelectedItem);
					if (linkId != -1)
					{
						node[linkId].Destination = NodeSelected.Index;
						node[linkId].Distance = calcLinkDistance(
																node,
																NodeSelected,
																null);
					}
					Refresh();
				}
			}
		} */

		private void OnLink1DestSelectedIndexChanged(object sender, EventArgs e)
		{
			LinkDestinationSelectedIndexChanged(cbLink1Dest, 0, tbLink1Dist);
		}

		private void OnLink1DestLeave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink1, 0); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void OnLink2DestSelectedIndexChanged(object sender, EventArgs e)
		{
			LinkDestinationSelectedIndexChanged(cbLink2Dest, 1, tbLink2Dist);
		}

		private void OnLink2DestLeave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink2, 1); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void OnLink3DestSelectedIndexChanged(object sender, EventArgs e)
		{
			LinkDestinationSelectedIndexChanged(cbLink3Dest, 2, tbLink3Dist);
		}

		private void OnLink3DestLeave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink3, 2); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void OnLink4DestSelectedIndexChanged(object sender, EventArgs e)
		{
			LinkDestinationSelectedIndexChanged(cbLink4Dest, 3, tbLink4Dist);
		}

		private void OnLink4DestLeave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink4, 3); // don't do any node-linking OnLeave unless i vet it first.
		}

		private void OnLink5DestSelectedIndexChanged(object sender, EventArgs e)
		{
			LinkDestinationSelectedIndexChanged(cbLink5Dest, 4, tbLink5Dist);
		}

		private void OnLink5DestLeave(object sender, EventArgs e)
		{
//			cbLink_Leave(cbLink5, 4); // don't do any node-linking OnLeave unless i vet it first.
		}


		private void OnLink1UnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected[0].UsableType = (UnitType)cbLink1UnitType.SelectedItem;
				Refresh();
			}
		}

		private void OnLink2UnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected[1].UsableType = (UnitType)cbLink2UnitType.SelectedItem;
				Refresh();
			}
		}

		private void OnLink3UnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected[2].UsableType = (UnitType)cbLink3UnitType.SelectedItem;
				Refresh();
			}
		}

		private void OnLink4UnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected[3].UsableType = (UnitType)cbLink4UnitType.SelectedItem;
				Refresh();
			}
		}

		private void OnLink5UnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

				NodeSelected[4].UsableType = (UnitType)cbLink5UnitType.SelectedItem;
				Refresh();
			}
		}


		private const string NodeCopyPrefix  = "MVNode"; // TODO: use a struct to copy/paste the info.
		private const char NodeCopySeparator = '|';

		/// <summary>
		/// Prevents two error-dialogs from showing if a key-cut is underway.
		/// </summary>
		private bool _asterisk;

		private void OnCutClick(object sender, EventArgs e)
		{
			OnCopyClick(null, null);
			OnDeleteClick(null, null);
		}

		private void OnCopyClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				btnPaste.Enabled = true;

				var nodeText = string.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"{0}{6}{1}{6}{2}{6}{3}{6}{4}{6}{5}",
										NodeCopyPrefix,
										cbUnitType.SelectedIndex,
										cbPriority.SelectedIndex,
										cbAttack.SelectedIndex,
										cbSpawnRank.SelectedIndex,
										cbSpawnWeight.SelectedIndex,
										NodeCopySeparator); // TODO: include Link info ... perhaps.
				Clipboard.SetText(nodeText);
			}
			else
				ShowDialogAsterisk("A node must be selected.");
		}

		private void OnPasteClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				var nodeData = Clipboard.GetText().Split(NodeCopySeparator);
				if (nodeData[0] == NodeCopyPrefix)// TODO: include Link info ... perhaps.
				{
					_mapFile.MapChanged = true;

					cbUnitType.SelectedIndex    = Int32.Parse(nodeData[1], System.Globalization.CultureInfo.InvariantCulture);
					cbPriority.SelectedIndex    = Int32.Parse(nodeData[2], System.Globalization.CultureInfo.InvariantCulture);
					cbAttack.SelectedIndex      = Int32.Parse(nodeData[3], System.Globalization.CultureInfo.InvariantCulture);
					cbSpawnRank.SelectedIndex   = Int32.Parse(nodeData[4], System.Globalization.CultureInfo.InvariantCulture);
					cbSpawnWeight.SelectedIndex = Int32.Parse(nodeData[5], System.Globalization.CultureInfo.InvariantCulture);
				}
				else
					ShowDialogAsterisk("The data on the clipboard is not a node.");
			}
			else
				ShowDialogAsterisk("A node must be selected.");
		}

		private void OnDeleteClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				_mapFile.MapChanged = true;

				_mapFile.Routes.DeleteNode(NodeSelected);

				((XCMapTile)_mapFile[NodeSelected.Row,
									 NodeSelected.Col,
									 NodeSelected.Lev]).Node = null;

				DeselectNode();

				gbPatrolData.Enabled =
				gbSpawnData.Enabled  =
				gbNodeData.Enabled   =
				gbLinkData.Enabled   = false;

				Refresh();
			}
			else if (!_asterisk)
				ShowDialogAsterisk("A node must be selected.");
		}

		private void ShowDialogAsterisk(string asterisk)
		{
			MessageBox.Show(
						this,
						asterisk,
						"Err..",
						MessageBoxButtons.OK,
						MessageBoxIcon.Asterisk,
						MessageBoxDefaultButton.Button1,
						0);
		}

		/// <summary>
		/// Deselects any currently selected node.
		/// </summary>
		private void DeselectNode()
		{
			NodeSelected = null;
			_routePanel.ClearClickPoint();

			tsmiClearLinkData.Enabled = false;
		}

		/// <summary>
		/// Handles keyboard input.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
//					case Keys.S: // TODO: vet that.
//						if (_mapFile != null)
//						{
//							_mapFile.Save();
//							e.Handled = true; // why.
//						}
//						break;

					case Keys.X:
						_asterisk = true;
						OnCopyClick(null, null);
						OnDeleteClick(null, null);
						_asterisk = false;
						break;

					case Keys.C:
						OnCopyClick(null, null);
						break;

					case Keys.V:
						OnPasteClick(null, null);
						break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					case Keys.Delete:
						OnDeleteClick(null, null);
						break;
				}
			}
		}

		private void OnAllNodeSpawnRank0Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
							this,
							"Are you sure you want to make all nodes spawn Rank"
								+ " 0 Civ-Scout?",
							"Warning",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button2,
							0) == DialogResult.Yes)
			{
				int changed = 0;
				foreach (RouteNode node in _mapFile.Routes)
					if (node.SpawnRank != 0)
					{
						++changed;
						node.SpawnRank = 0;
					}

				if (changed != 0)
				{
					_mapFile.MapChanged = true;

					UpdateNodeInformation();

					MessageBox.Show(
								changed + " nodes were changed.",
								"All nodes spawn Rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
					MessageBox.Show(
								"All nodes are already rank 0.",
								"All nodes spawn Rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.Asterisk,
								MessageBoxDefaultButton.Button1,
								0);
			}
		}

		private void OnClearLinkDataClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				if (MessageBox.Show(
								this,
								"Are you sure you want to clear the selected node's Link data?",
								"Warning",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.Yes)
				{
					_mapFile.MapChanged = true; // TODO: investigate and separate saving of the MAP and the RMP files.

					for (int slotId = 0; slotId != RouteNode.LinkSlots; ++slotId)
					{
						NodeSelected[slotId].Destination = Link.NotUsed;
						NodeSelected[slotId].Distance = 0;

						NodeSelected[slotId].UsableType = UnitType.Any;
					}
					UpdateNodeInformation();

					Refresh();
				}
			}
		}

		private void OnConnectDropDownClosed(object sender, EventArgs e)
		{
			pRoutes.Select();	// take focus off the stupid combobox. Tks.
		}						// NOTE: sometimes it still stays "highlighted"
								// but at least it's no longer "selected".


		#region Options
		// headers
		private const string Links = "Links";
		private const string View  = "View";
		private const string Nodes = "Nodes";

		// options
		internal const string UnselectedLinkColor = "UnselectedLinkColor";
		private  const string UnselectedLinkWidth = "UnselectedLinkWidth";
		internal const string SelectedLinkColor   = "SelectedLinkColor";
		private  const string SelectedLinkWidth   = "SelectedLinkWidth";

		internal const string WallColor           = "WallColor";
		private  const string WallWidth           = "WallWidth";
		internal const string ContentColor        = "ContentColor";

		internal const string GridLineColor       = "GridLineColor";
		private  const string GridLineWidth       = "GridLineWidth";

		internal const string UnselectedNodeColor = "UnselectedNodeColor";
		internal const string SelectedNodeColor   = "SelectedNodeColor";
		internal const string SpawnNodeColor      = "SpawnNodeColor";
		private  const string NodeOpacity         = "NodeOpacity";

		private  const string ShowOverlay         = "ShowOverlay";
		private  const string ShowPriorityBars    = "ShowPriorityBars";


		/// <summary>
		/// Loads default options for RouteView in TopRouteView screens.
		/// </summary>
		protected internal override void LoadControl0Options()
		{
			tscbConnectType.SelectedIndex = 0;

			var pens    = _routePanel.RoutePens;
			var brushes = _routePanel.RouteBrushes;

			var bc = new OptionChangedEventHandler(OnBrushColorChanged);
			var pc = new OptionChangedEventHandler(OnPenColorChanged);
			var pw = new OptionChangedEventHandler(OnPenWidthChanged);
			var oc = new OptionChangedEventHandler(OnNodeOpacityChanged);
			var sp = new OptionChangedEventHandler(OnShowPriorityChanged);
			var so = new OptionChangedEventHandler(OnShowOverlayChanged);

			var pen = new Pen(new SolidBrush(Color.OrangeRed), 2);
			pens[UnselectedLinkColor] = pen;
			pens[UnselectedLinkWidth] = pen;
			Options.AddOption(
							UnselectedLinkColor,
							pen.Color,
							"Color of unselected link lines",
							Links,
							pc);
			Options.AddOption(
							UnselectedLinkWidth,
							2,
							"Width of unselected link lines",
							Links,
							pw);

			pen = new Pen(new SolidBrush(Color.RoyalBlue), 2);
			pens[SelectedLinkColor] = pen;
			pens[SelectedLinkWidth] = pen;
			Options.AddOption(
							SelectedLinkColor,
							pen.Color,
							"Color of selected link lines",
							Links,
							pc);
			Options.AddOption(
							SelectedLinkWidth,
							2,
							"Width of selected link lines",
							Links,
							pw);

			pen = new Pen(new SolidBrush(Color.BurlyWood), 3);
			pens[WallColor] = pen;
			pens[WallWidth] = pen;
			Options.AddOption(
							WallColor,
							pen.Color,
							"Color of wall indicators",
							View,
							pc);
			Options.AddOption(
							WallWidth,
							3,
							"Width of wall indicators",
							View,
							pw);

			var brush = new SolidBrush(Color.DarkGoldenrod);
			brushes[ContentColor] = brush;
			Options.AddOption(
							ContentColor,
							brush.Color,
							"Color of content indicators",
							View,
							bc);

			pen = new Pen(new SolidBrush(Color.Black), 1);
			pens[GridLineColor] = pen;
			pens[GridLineWidth] = pen;
			Options.AddOption(
							GridLineColor,
							pen.Color,
							"Color of grid lines",
							View,
							pc);
			Options.AddOption(
							GridLineWidth,
							1,
							"Width of grid lines",
							View,
							pw);

			brush = new SolidBrush(Color.MediumSeaGreen);
			brushes[UnselectedNodeColor] = brush;
			Options.AddOption(
							UnselectedNodeColor,
							brush.Color,
							"Color of unselected nodes",
							Nodes,
							bc);

			brush = new SolidBrush(Color.RoyalBlue);
			brushes[SelectedNodeColor] = brush;
			Options.AddOption(
							SelectedNodeColor,
							brush.Color,
							"Color of selected nodes",
							Nodes,
							bc);

			brush = new SolidBrush(Color.GreenYellow);
			brushes[SpawnNodeColor] = brush;
			Options.AddOption(
							SpawnNodeColor,
							brush.Color,
							"Color of spawn nodes",
							Nodes,
							bc);

			Options.AddOption(
							NodeOpacity,
							255,
							"Opacity of node colors (0..255)",
							Nodes,
							oc);

			Options.AddOption(
							ShowPriorityBars,
							true,
							"True to show patrol-priority and spawn-weight bars",
							Nodes,
							sp);

			Options.AddOption(
							ShowOverlay,
							true,
							"True to show mouse-over information",
							View,
							so);
		}

		private void OnBrushColorChanged(object sender, string key, object val)
		{
			var color = (Color)val;
			_routePanel.RouteBrushes[key].Color = color;

			switch (key)
			{
				case SelectedNodeColor:
					lblSelectedPosition.ForeColor =
					lblSelectedId.ForeColor       = color;
					break;
				case UnselectedNodeColor:
					lblOverId.ForeColor = color;
					break;
			}
			Refresh();
		}

		private void OnPenColorChanged(object sender, string key, object val)
		{
			_routePanel.RoutePens[key].Color = (Color)val;
			Refresh();
		}

		private void OnPenWidthChanged(object sender, string key, object val)
		{
			_routePanel.RoutePens[key].Width = (int)val;
			Refresh();
		}

		private void OnNodeOpacityChanged(object sender, string key, object val)
		{
			_routePanel.Opacity = (int)val;
			Refresh();
		}

		private void OnShowPriorityChanged(object sender, string key, object val)
		{
			_routePanel.ShowPriorityBars = (bool)val;
			Refresh();
		}

		private void OnShowOverlayChanged(object sender, string key, object val)
		{
			_routePanel.ShowOverlay = (bool)val;
			Refresh();
		}


		/// <summary>
		/// Gets the wall-color for use by the Help screen.
		/// </summary>
		/// <returns></returns>
		internal Dictionary<string, Pen> GetWallPens()
		{
			return _routePanel.RoutePens;
		}

		/// <summary>
		/// Gets the content-color for use by the Help screen.
		/// </summary>
		/// <returns></returns>
		internal Dictionary<string, SolidBrush> GetContentBrushes()
		{
			return _routePanel.RouteBrushes;
		}
		#endregion
	}
}
