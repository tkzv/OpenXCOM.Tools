namespace MapView.Forms.MapObservers.RouteViews
{
	partial class RouteView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RouteView));
			this.cbLink5Dest = new System.Windows.Forms.ComboBox();
			this.cbLink4Dest = new System.Windows.Forms.ComboBox();
			this.cbLink3Dest = new System.Windows.Forms.ComboBox();
			this.cbLink2Dest = new System.Windows.Forms.ComboBox();
			this.cbLink1Dest = new System.Windows.Forms.ComboBox();
			this.labelLink5 = new System.Windows.Forms.Label();
			this.labelLink4 = new System.Windows.Forms.Label();
			this.labelLink3 = new System.Windows.Forms.Label();
			this.labelLink2 = new System.Windows.Forms.Label();
			this.labelLink1 = new System.Windows.Forms.Label();
			this.cbPriority = new System.Windows.Forms.ComboBox();
			this.cbSpawnRank = new System.Windows.Forms.ComboBox();
			this.cbUnitType = new System.Windows.Forms.ComboBox();
			this.labelSpawnWeight = new System.Windows.Forms.Label();
			this.labelPriority = new System.Windows.Forms.Label();
			this.labelSpawnRank = new System.Windows.Forms.Label();
			this.labelUnitType = new System.Windows.Forms.Label();
			this.lblSelectedPosition = new System.Windows.Forms.Label();
			this.gbSpawnData = new System.Windows.Forms.GroupBox();
			this.cbSpawnWeight = new System.Windows.Forms.ComboBox();
			this.btnPaste = new System.Windows.Forms.Button();
			this.lblOverId = new System.Windows.Forms.Label();
			this.btnCopy = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.lblSelectedId = new System.Windows.Forms.Label();
			this.tbLink5Dist = new System.Windows.Forms.TextBox();
			this.tbLink4Dist = new System.Windows.Forms.TextBox();
			this.tbLink3Dist = new System.Windows.Forms.TextBox();
			this.tbLink2Dist = new System.Windows.Forms.TextBox();
			this.tbLink1Dist = new System.Windows.Forms.TextBox();
			this.labelDist = new System.Windows.Forms.Label();
			this.cbLink5UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink4UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink3UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink2UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink1UnitType = new System.Windows.Forms.ComboBox();
			this.labelUnitInfo = new System.Windows.Forms.Label();
			this.pnlRoutes = new System.Windows.Forms.Panel();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tscbConnectType = new System.Windows.Forms.ToolStripComboBox();
			this.tsddbEdit = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAllNodesRank0 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiClearLinkData = new System.Windows.Forms.ToolStripMenuItem();
			this.gbLinkData = new System.Windows.Forms.GroupBox();
			this.btnOg = new System.Windows.Forms.Button();
			this.btnGoLink5 = new System.Windows.Forms.Button();
			this.btnGoLink4 = new System.Windows.Forms.Button();
			this.btnGoLink3 = new System.Windows.Forms.Button();
			this.btnGoLink2 = new System.Windows.Forms.Button();
			this.btnGoLink1 = new System.Windows.Forms.Button();
			this.pnlDataFields = new System.Windows.Forms.Panel();
			this.gbNodeEditor = new System.Windows.Forms.GroupBox();
			this.btnCut = new System.Windows.Forms.Button();
			this.pnlDataFieldsLeft = new System.Windows.Forms.Panel();
			this.gbPatrolData = new System.Windows.Forms.GroupBox();
			this.cbAttack = new System.Windows.Forms.ComboBox();
			this.labelAttack = new System.Windows.Forms.Label();
			this.gbNodeData = new System.Windows.Forms.GroupBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gbSpawnData.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.gbLinkData.SuspendLayout();
			this.pnlDataFields.SuspendLayout();
			this.gbNodeEditor.SuspendLayout();
			this.pnlDataFieldsLeft.SuspendLayout();
			this.gbPatrolData.SuspendLayout();
			this.gbNodeData.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbLink5Dest
			// 
			this.cbLink5Dest.Location = new System.Drawing.Point(45, 125);
			this.cbLink5Dest.Name = "cbLink5Dest";
			this.cbLink5Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink5Dest.TabIndex = 20;
			this.cbLink5Dest.SelectedIndexChanged += new System.EventHandler(this.OnLink5DestSelectedIndexChanged);
			this.cbLink5Dest.Leave += new System.EventHandler(this.OnLink5DestLeave);
			this.cbLink5Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink5Dest.MouseHover += new System.EventHandler(this.OnLink5MouseEnter);
			// 
			// cbLink4Dest
			// 
			this.cbLink4Dest.Location = new System.Drawing.Point(45, 100);
			this.cbLink4Dest.Name = "cbLink4Dest";
			this.cbLink4Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink4Dest.TabIndex = 19;
			this.cbLink4Dest.SelectedIndexChanged += new System.EventHandler(this.OnLink4DestSelectedIndexChanged);
			this.cbLink4Dest.Leave += new System.EventHandler(this.OnLink4DestLeave);
			this.cbLink4Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink4Dest.MouseHover += new System.EventHandler(this.OnLink4MouseEnter);
			// 
			// cbLink3Dest
			// 
			this.cbLink3Dest.Location = new System.Drawing.Point(45, 75);
			this.cbLink3Dest.Name = "cbLink3Dest";
			this.cbLink3Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink3Dest.TabIndex = 18;
			this.cbLink3Dest.SelectedIndexChanged += new System.EventHandler(this.OnLink3DestSelectedIndexChanged);
			this.cbLink3Dest.Leave += new System.EventHandler(this.OnLink3DestLeave);
			this.cbLink3Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink3Dest.MouseHover += new System.EventHandler(this.OnLink3MouseEnter);
			// 
			// cbLink2Dest
			// 
			this.cbLink2Dest.Location = new System.Drawing.Point(45, 50);
			this.cbLink2Dest.Name = "cbLink2Dest";
			this.cbLink2Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink2Dest.TabIndex = 17;
			this.cbLink2Dest.SelectedIndexChanged += new System.EventHandler(this.OnLink2DestSelectedIndexChanged);
			this.cbLink2Dest.Leave += new System.EventHandler(this.OnLink2DestLeave);
			this.cbLink2Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink2Dest.MouseHover += new System.EventHandler(this.OnLink2MouseEnter);
			// 
			// cbLink1Dest
			// 
			this.cbLink1Dest.Location = new System.Drawing.Point(45, 25);
			this.cbLink1Dest.Name = "cbLink1Dest";
			this.cbLink1Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink1Dest.TabIndex = 16;
			this.cbLink1Dest.SelectedIndexChanged += new System.EventHandler(this.OnLink1DestSelectedIndexChanged);
			this.cbLink1Dest.Leave += new System.EventHandler(this.OnLink1DestLeave);
			this.cbLink1Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink1Dest.MouseHover += new System.EventHandler(this.OnLink1MouseEnter);
			// 
			// labelLink5
			// 
			this.labelLink5.Location = new System.Drawing.Point(5, 130);
			this.labelLink5.Name = "labelLink5";
			this.labelLink5.Size = new System.Drawing.Size(40, 15);
			this.labelLink5.TabIndex = 15;
			this.labelLink5.Text = "Link5";
			this.labelLink5.MouseEnter += new System.EventHandler(this.OnLink5MouseEnter);
			this.labelLink5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink4
			// 
			this.labelLink4.Location = new System.Drawing.Point(5, 105);
			this.labelLink4.Name = "labelLink4";
			this.labelLink4.Size = new System.Drawing.Size(40, 15);
			this.labelLink4.TabIndex = 14;
			this.labelLink4.Text = "Link4";
			this.labelLink4.MouseEnter += new System.EventHandler(this.OnLink4MouseEnter);
			this.labelLink4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink3
			// 
			this.labelLink3.Location = new System.Drawing.Point(5, 80);
			this.labelLink3.Name = "labelLink3";
			this.labelLink3.Size = new System.Drawing.Size(40, 15);
			this.labelLink3.TabIndex = 13;
			this.labelLink3.Text = "Link3";
			this.labelLink3.MouseEnter += new System.EventHandler(this.OnLink3MouseEnter);
			this.labelLink3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink2
			// 
			this.labelLink2.Location = new System.Drawing.Point(5, 55);
			this.labelLink2.Name = "labelLink2";
			this.labelLink2.Size = new System.Drawing.Size(40, 15);
			this.labelLink2.TabIndex = 12;
			this.labelLink2.Text = "Link2";
			this.labelLink2.MouseEnter += new System.EventHandler(this.OnLink2MouseEnter);
			this.labelLink2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink1
			// 
			this.labelLink1.Location = new System.Drawing.Point(5, 30);
			this.labelLink1.Name = "labelLink1";
			this.labelLink1.Size = new System.Drawing.Size(40, 15);
			this.labelLink1.TabIndex = 11;
			this.labelLink1.Text = "Link1";
			this.labelLink1.MouseEnter += new System.EventHandler(this.OnLink1MouseEnter);
			this.labelLink1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// cbPriority
			// 
			this.cbPriority.Location = new System.Drawing.Point(85, 40);
			this.cbPriority.Name = "cbPriority";
			this.cbPriority.Size = new System.Drawing.Size(130, 20);
			this.cbPriority.TabIndex = 8;
			this.toolTip1.SetToolTip(this.cbPriority, "patrol priority");
			this.cbPriority.SelectedIndexChanged += new System.EventHandler(this.OnPatrolPrioritySelectedIndexChanged);
			// 
			// cbSpawnRank
			// 
			this.cbSpawnRank.Location = new System.Drawing.Point(85, 15);
			this.cbSpawnRank.Name = "cbSpawnRank";
			this.cbSpawnRank.Size = new System.Drawing.Size(130, 20);
			this.cbSpawnRank.TabIndex = 7;
			this.toolTip1.SetToolTip(this.cbSpawnRank, "faction or rank (if aLiens) that may spawn here. Nodes for aLiens outside their U" +
		"FO or base should be set to 0");
			this.cbSpawnRank.SelectedIndexChanged += new System.EventHandler(this.OnSpawnRankSelectedIndexChanged);
			// 
			// cbUnitType
			// 
			this.cbUnitType.Location = new System.Drawing.Point(85, 15);
			this.cbUnitType.Name = "cbUnitType";
			this.cbUnitType.Size = new System.Drawing.Size(130, 20);
			this.cbUnitType.TabIndex = 6;
			this.toolTip1.SetToolTip(this.cbUnitType, "characteristics of units that may patrol the node");
			this.cbUnitType.SelectedIndexChanged += new System.EventHandler(this.OnUnitTypeSelectedIndexChanged);
			// 
			// labelSpawnWeight
			// 
			this.labelSpawnWeight.Location = new System.Drawing.Point(10, 45);
			this.labelSpawnWeight.Name = "labelSpawnWeight";
			this.labelSpawnWeight.Size = new System.Drawing.Size(75, 15);
			this.labelSpawnWeight.TabIndex = 5;
			this.labelSpawnWeight.Text = "Weight";
			this.labelSpawnWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelSpawnWeight, "chance that an aLien will spawn here");
			// 
			// labelPriority
			// 
			this.labelPriority.Location = new System.Drawing.Point(10, 45);
			this.labelPriority.Name = "labelPriority";
			this.labelPriority.Size = new System.Drawing.Size(75, 15);
			this.labelPriority.TabIndex = 3;
			this.labelPriority.Text = "Priority";
			this.labelPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelPriority, "patrol priority");
			// 
			// labelSpawnRank
			// 
			this.labelSpawnRank.Location = new System.Drawing.Point(10, 20);
			this.labelSpawnRank.Name = "labelSpawnRank";
			this.labelSpawnRank.Size = new System.Drawing.Size(75, 15);
			this.labelSpawnRank.TabIndex = 2;
			this.labelSpawnRank.Text = "Rank";
			this.labelSpawnRank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelSpawnRank, "faction or rank (if aLiens) that may spawn here. Nodes for aLiens outside their U" +
		"FO or base should be set to 0");
			// 
			// labelUnitType
			// 
			this.labelUnitType.Location = new System.Drawing.Point(10, 20);
			this.labelUnitType.Name = "labelUnitType";
			this.labelUnitType.Size = new System.Drawing.Size(75, 15);
			this.labelUnitType.TabIndex = 1;
			this.labelUnitType.Text = "Unit Type";
			this.labelUnitType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelUnitType, "characteristics of units that may patrol the node");
			// 
			// lblSelectedPosition
			// 
			this.lblSelectedPosition.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSelectedPosition.Location = new System.Drawing.Point(15, 15);
			this.lblSelectedPosition.Name = "lblSelectedPosition";
			this.lblSelectedPosition.Size = new System.Drawing.Size(85, 20);
			this.lblSelectedPosition.TabIndex = 0;
			this.lblSelectedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// gbSpawnData
			// 
			this.gbSpawnData.Controls.Add(this.cbSpawnRank);
			this.gbSpawnData.Controls.Add(this.labelSpawnRank);
			this.gbSpawnData.Controls.Add(this.labelSpawnWeight);
			this.gbSpawnData.Controls.Add(this.cbSpawnWeight);
			this.gbSpawnData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSpawnData.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSpawnData.Location = new System.Drawing.Point(0, 135);
			this.gbSpawnData.Name = "gbSpawnData";
			this.gbSpawnData.Size = new System.Drawing.Size(265, 72);
			this.gbSpawnData.TabIndex = 1;
			this.gbSpawnData.TabStop = false;
			this.gbSpawnData.Text = "Spawn data";
			// 
			// cbSpawnWeight
			// 
			this.cbSpawnWeight.Location = new System.Drawing.Point(85, 40);
			this.cbSpawnWeight.Name = "cbSpawnWeight";
			this.cbSpawnWeight.Size = new System.Drawing.Size(130, 20);
			this.cbSpawnWeight.TabIndex = 10;
			this.toolTip1.SetToolTip(this.cbSpawnWeight, "chance that an aLien will spawn here");
			this.cbSpawnWeight.SelectedIndexChanged += new System.EventHandler(this.OnSpawnWeightSelectedIndexChanged);
			// 
			// btnPaste
			// 
			this.btnPaste.Enabled = false;
			this.btnPaste.Location = new System.Drawing.Point(145, 15);
			this.btnPaste.Name = "btnPaste";
			this.btnPaste.Size = new System.Drawing.Size(65, 30);
			this.btnPaste.TabIndex = 35;
			this.btnPaste.Text = "Paste";
			this.toolTip1.SetToolTip(this.btnPaste, "pastes Patrol data and Spawn data to the selected node");
			this.btnPaste.Click += new System.EventHandler(this.OnPasteClick);
			// 
			// lblOverId
			// 
			this.lblOverId.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOverId.ForeColor = System.Drawing.Color.Sienna;
			this.lblOverId.Location = new System.Drawing.Point(190, 15);
			this.lblOverId.Name = "lblOverId";
			this.lblOverId.Size = new System.Drawing.Size(70, 20);
			this.lblOverId.TabIndex = 2;
			this.lblOverId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnCopy
			// 
			this.btnCopy.Enabled = false;
			this.btnCopy.Location = new System.Drawing.Point(75, 15);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(65, 30);
			this.btnCopy.TabIndex = 34;
			this.btnCopy.Text = "Copy";
			this.toolTip1.SetToolTip(this.btnCopy, "copies the selected node");
			this.btnCopy.Click += new System.EventHandler(this.OnCopyClick);
			// 
			// btnDelete
			// 
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(215, 15);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(65, 30);
			this.btnDelete.TabIndex = 36;
			this.btnDelete.Text = "Delete";
			this.toolTip1.SetToolTip(this.btnDelete, "deletes the selected node");
			this.btnDelete.Click += new System.EventHandler(this.OnDeleteClick);
			// 
			// lblSelectedId
			// 
			this.lblSelectedId.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSelectedId.ForeColor = System.Drawing.Color.Orchid;
			this.lblSelectedId.Location = new System.Drawing.Point(110, 15);
			this.lblSelectedId.Name = "lblSelectedId";
			this.lblSelectedId.Size = new System.Drawing.Size(75, 20);
			this.lblSelectedId.TabIndex = 2;
			this.lblSelectedId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tbLink5Dist
			// 
			this.tbLink5Dist.Location = new System.Drawing.Point(240, 125);
			this.tbLink5Dist.Name = "tbLink5Dist";
			this.tbLink5Dist.ReadOnly = true;
			this.tbLink5Dist.Size = new System.Drawing.Size(40, 19);
			this.tbLink5Dist.TabIndex = 32;
			this.tbLink5Dist.WordWrap = false;
			this.tbLink5Dist.MouseEnter += new System.EventHandler(this.OnLink5MouseEnter);
			this.tbLink5Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink4Dist
			// 
			this.tbLink4Dist.Location = new System.Drawing.Point(240, 100);
			this.tbLink4Dist.Name = "tbLink4Dist";
			this.tbLink4Dist.ReadOnly = true;
			this.tbLink4Dist.Size = new System.Drawing.Size(40, 19);
			this.tbLink4Dist.TabIndex = 31;
			this.tbLink4Dist.WordWrap = false;
			this.tbLink4Dist.MouseEnter += new System.EventHandler(this.OnLink4MouseEnter);
			this.tbLink4Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink3Dist
			// 
			this.tbLink3Dist.Location = new System.Drawing.Point(240, 75);
			this.tbLink3Dist.Name = "tbLink3Dist";
			this.tbLink3Dist.ReadOnly = true;
			this.tbLink3Dist.Size = new System.Drawing.Size(40, 19);
			this.tbLink3Dist.TabIndex = 30;
			this.tbLink3Dist.WordWrap = false;
			this.tbLink3Dist.MouseEnter += new System.EventHandler(this.OnLink3MouseEnter);
			this.tbLink3Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink2Dist
			// 
			this.tbLink2Dist.Location = new System.Drawing.Point(240, 50);
			this.tbLink2Dist.Name = "tbLink2Dist";
			this.tbLink2Dist.ReadOnly = true;
			this.tbLink2Dist.Size = new System.Drawing.Size(40, 19);
			this.tbLink2Dist.TabIndex = 29;
			this.tbLink2Dist.WordWrap = false;
			this.tbLink2Dist.MouseEnter += new System.EventHandler(this.OnLink2MouseEnter);
			this.tbLink2Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink1Dist
			// 
			this.tbLink1Dist.Location = new System.Drawing.Point(240, 25);
			this.tbLink1Dist.Name = "tbLink1Dist";
			this.tbLink1Dist.ReadOnly = true;
			this.tbLink1Dist.Size = new System.Drawing.Size(40, 19);
			this.tbLink1Dist.TabIndex = 28;
			this.tbLink1Dist.WordWrap = false;
			this.tbLink1Dist.MouseEnter += new System.EventHandler(this.OnLink1MouseEnter);
			this.tbLink1Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelDist
			// 
			this.labelDist.Location = new System.Drawing.Point(240, 10);
			this.labelDist.Name = "labelDist";
			this.labelDist.Size = new System.Drawing.Size(55, 15);
			this.labelDist.TabIndex = 27;
			this.labelDist.Text = "Distance";
			// 
			// cbLink5UnitType
			// 
			this.cbLink5UnitType.Location = new System.Drawing.Point(125, 125);
			this.cbLink5UnitType.Name = "cbLink5UnitType";
			this.cbLink5UnitType.Size = new System.Drawing.Size(110, 20);
			this.cbLink5UnitType.TabIndex = 26;
			this.cbLink5UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLink5UnitTypeSelectedIndexChanged);
			this.cbLink5UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink5UnitType.MouseHover += new System.EventHandler(this.OnLink5MouseEnter);
			// 
			// cbLink4UnitType
			// 
			this.cbLink4UnitType.Location = new System.Drawing.Point(125, 100);
			this.cbLink4UnitType.Name = "cbLink4UnitType";
			this.cbLink4UnitType.Size = new System.Drawing.Size(110, 20);
			this.cbLink4UnitType.TabIndex = 25;
			this.cbLink4UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLink4UnitTypeSelectedIndexChanged);
			this.cbLink4UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink4UnitType.MouseHover += new System.EventHandler(this.OnLink4MouseEnter);
			// 
			// cbLink3UnitType
			// 
			this.cbLink3UnitType.Location = new System.Drawing.Point(125, 75);
			this.cbLink3UnitType.Name = "cbLink3UnitType";
			this.cbLink3UnitType.Size = new System.Drawing.Size(110, 20);
			this.cbLink3UnitType.TabIndex = 24;
			this.cbLink3UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLink3UnitTypeSelectedIndexChanged);
			this.cbLink3UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink3UnitType.MouseHover += new System.EventHandler(this.OnLink3MouseEnter);
			// 
			// cbLink2UnitType
			// 
			this.cbLink2UnitType.Location = new System.Drawing.Point(125, 50);
			this.cbLink2UnitType.Name = "cbLink2UnitType";
			this.cbLink2UnitType.Size = new System.Drawing.Size(110, 20);
			this.cbLink2UnitType.TabIndex = 23;
			this.cbLink2UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLink2UnitTypeSelectedIndexChanged);
			this.cbLink2UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink2UnitType.MouseHover += new System.EventHandler(this.OnLink2MouseEnter);
			// 
			// cbLink1UnitType
			// 
			this.cbLink1UnitType.Location = new System.Drawing.Point(125, 25);
			this.cbLink1UnitType.Name = "cbLink1UnitType";
			this.cbLink1UnitType.Size = new System.Drawing.Size(110, 20);
			this.cbLink1UnitType.TabIndex = 22;
			this.cbLink1UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLink1UnitTypeSelectedIndexChanged);
			this.cbLink1UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink1UnitType.MouseHover += new System.EventHandler(this.OnLink1MouseEnter);
			// 
			// labelUnitInfo
			// 
			this.labelUnitInfo.Location = new System.Drawing.Point(125, 10);
			this.labelUnitInfo.Name = "labelUnitInfo";
			this.labelUnitInfo.Size = new System.Drawing.Size(55, 15);
			this.labelUnitInfo.TabIndex = 21;
			this.labelUnitInfo.Text = "Unit Info";
			// 
			// pnlRoutes
			// 
			this.pnlRoutes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlRoutes.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pnlRoutes.Location = new System.Drawing.Point(0, 25);
			this.pnlRoutes.Name = "pnlRoutes";
			this.pnlRoutes.Size = new System.Drawing.Size(640, 245);
			this.pnlRoutes.TabIndex = 0;
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F);
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tscbConnectType,
			this.tsddbEdit});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.Text = "tsMain";
			// 
			// tscbConnectType
			// 
			this.tscbConnectType.Font = new System.Drawing.Font("Verdana", 7F);
			this.tscbConnectType.Name = "tscbConnectType";
			this.tscbConnectType.Size = new System.Drawing.Size(120, 25);
			this.tscbConnectType.DropDownClosed += new System.EventHandler(this.OnConnectDropDownClosed);
			// 
			// tsddbEdit
			// 
			this.tsddbEdit.AutoToolTip = false;
			this.tsddbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiOptions,
			this.tsmiAllNodesRank0,
			this.tsmiClearLinkData});
			this.tsddbEdit.Font = new System.Drawing.Font("Verdana", 7F);
			this.tsddbEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsddbEdit.Image")));
			this.tsddbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbEdit.Name = "tsddbEdit";
			this.tsddbEdit.Size = new System.Drawing.Size(38, 22);
			this.tsddbEdit.Text = "Edit";
			// 
			// tsmiOptions
			// 
			this.tsmiOptions.Name = "tsmiOptions";
			this.tsmiOptions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiOptions.Size = new System.Drawing.Size(165, 22);
			this.tsmiOptions.Text = "Options";
			this.tsmiOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// tsmiAllNodesRank0
			// 
			this.tsmiAllNodesRank0.Name = "tsmiAllNodesRank0";
			this.tsmiAllNodesRank0.Size = new System.Drawing.Size(165, 22);
			this.tsmiAllNodesRank0.Text = "All Nodes Rank 0";
			this.tsmiAllNodesRank0.Click += new System.EventHandler(this.OnAllNodeSpawnRank0Click);
			// 
			// tsmiClearLinkData
			// 
			this.tsmiClearLinkData.Name = "tsmiClearLinkData";
			this.tsmiClearLinkData.Size = new System.Drawing.Size(165, 22);
			this.tsmiClearLinkData.Text = "Clear Link data";
			this.tsmiClearLinkData.Click += new System.EventHandler(this.OnClearLinkDataClick);
			// 
			// gbLinkData
			// 
			this.gbLinkData.Controls.Add(this.btnOg);
			this.gbLinkData.Controls.Add(this.btnGoLink5);
			this.gbLinkData.Controls.Add(this.btnGoLink4);
			this.gbLinkData.Controls.Add(this.btnGoLink3);
			this.gbLinkData.Controls.Add(this.btnGoLink2);
			this.gbLinkData.Controls.Add(this.btnGoLink1);
			this.gbLinkData.Controls.Add(this.labelLink1);
			this.gbLinkData.Controls.Add(this.tbLink5Dist);
			this.gbLinkData.Controls.Add(this.cbLink5Dest);
			this.gbLinkData.Controls.Add(this.tbLink4Dist);
			this.gbLinkData.Controls.Add(this.cbLink3Dest);
			this.gbLinkData.Controls.Add(this.tbLink3Dist);
			this.gbLinkData.Controls.Add(this.cbLink2Dest);
			this.gbLinkData.Controls.Add(this.tbLink2Dist);
			this.gbLinkData.Controls.Add(this.cbLink1Dest);
			this.gbLinkData.Controls.Add(this.tbLink1Dist);
			this.gbLinkData.Controls.Add(this.labelLink2);
			this.gbLinkData.Controls.Add(this.labelDist);
			this.gbLinkData.Controls.Add(this.labelLink3);
			this.gbLinkData.Controls.Add(this.cbLink5UnitType);
			this.gbLinkData.Controls.Add(this.cbLink4Dest);
			this.gbLinkData.Controls.Add(this.cbLink4UnitType);
			this.gbLinkData.Controls.Add(this.labelLink5);
			this.gbLinkData.Controls.Add(this.cbLink3UnitType);
			this.gbLinkData.Controls.Add(this.labelLink4);
			this.gbLinkData.Controls.Add(this.cbLink2UnitType);
			this.gbLinkData.Controls.Add(this.labelUnitInfo);
			this.gbLinkData.Controls.Add(this.cbLink1UnitType);
			this.gbLinkData.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbLinkData.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbLinkData.Location = new System.Drawing.Point(265, 3);
			this.gbLinkData.Name = "gbLinkData";
			this.gbLinkData.Size = new System.Drawing.Size(375, 150);
			this.gbLinkData.TabIndex = 0;
			this.gbLinkData.TabStop = false;
			this.gbLinkData.Text = "Link data";
			// 
			// btnOg
			// 
			this.btnOg.Location = new System.Drawing.Point(320, 25);
			this.btnOg.Name = "btnOg";
			this.btnOg.Size = new System.Drawing.Size(20, 120);
			this.btnOg.TabIndex = 39;
			this.btnOg.Text = "o\r\ng";
			this.btnOg.UseVisualStyleBackColor = true;
			this.btnOg.Click += new System.EventHandler(this.OnOgClick);
			this.btnOg.MouseEnter += new System.EventHandler(this.OnOgMouseEnter);
			this.btnOg.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink5
			// 
			this.btnGoLink5.Enabled = false;
			this.btnGoLink5.Location = new System.Drawing.Point(285, 125);
			this.btnGoLink5.Name = "btnGoLink5";
			this.btnGoLink5.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink5.TabIndex = 38;
			this.btnGoLink5.Text = "go";
			this.btnGoLink5.UseVisualStyleBackColor = true;
			this.btnGoLink5.Click += new System.EventHandler(this.OnLink5GoClick);
			this.btnGoLink5.MouseEnter += new System.EventHandler(this.OnLink5MouseEnter);
			this.btnGoLink5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink4
			// 
			this.btnGoLink4.Enabled = false;
			this.btnGoLink4.Location = new System.Drawing.Point(285, 100);
			this.btnGoLink4.Name = "btnGoLink4";
			this.btnGoLink4.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink4.TabIndex = 37;
			this.btnGoLink4.Text = "go";
			this.btnGoLink4.UseVisualStyleBackColor = true;
			this.btnGoLink4.Click += new System.EventHandler(this.OnLink4GoClick);
			this.btnGoLink4.MouseEnter += new System.EventHandler(this.OnLink4MouseEnter);
			this.btnGoLink4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink3
			// 
			this.btnGoLink3.Enabled = false;
			this.btnGoLink3.Location = new System.Drawing.Point(285, 75);
			this.btnGoLink3.Name = "btnGoLink3";
			this.btnGoLink3.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink3.TabIndex = 36;
			this.btnGoLink3.Text = "go";
			this.btnGoLink3.UseVisualStyleBackColor = true;
			this.btnGoLink3.Click += new System.EventHandler(this.OnLink3GoClick);
			this.btnGoLink3.MouseEnter += new System.EventHandler(this.OnLink3MouseEnter);
			this.btnGoLink3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink2
			// 
			this.btnGoLink2.Enabled = false;
			this.btnGoLink2.Location = new System.Drawing.Point(285, 50);
			this.btnGoLink2.Name = "btnGoLink2";
			this.btnGoLink2.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink2.TabIndex = 35;
			this.btnGoLink2.Text = "go";
			this.btnGoLink2.UseVisualStyleBackColor = true;
			this.btnGoLink2.Click += new System.EventHandler(this.OnLink2GoClick);
			this.btnGoLink2.MouseEnter += new System.EventHandler(this.OnLink2MouseEnter);
			this.btnGoLink2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink1
			// 
			this.btnGoLink1.Enabled = false;
			this.btnGoLink1.Location = new System.Drawing.Point(285, 25);
			this.btnGoLink1.Name = "btnGoLink1";
			this.btnGoLink1.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink1.TabIndex = 34;
			this.btnGoLink1.Text = "go";
			this.btnGoLink1.UseVisualStyleBackColor = true;
			this.btnGoLink1.Click += new System.EventHandler(this.OnLink1GoClick);
			this.btnGoLink1.MouseEnter += new System.EventHandler(this.OnLink1MouseEnter);
			this.btnGoLink1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// pnlDataFields
			// 
			this.pnlDataFields.Controls.Add(this.gbNodeEditor);
			this.pnlDataFields.Controls.Add(this.gbLinkData);
			this.pnlDataFields.Controls.Add(this.pnlDataFieldsLeft);
			this.pnlDataFields.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDataFields.Location = new System.Drawing.Point(0, 270);
			this.pnlDataFields.Name = "pnlDataFields";
			this.pnlDataFields.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.pnlDataFields.Size = new System.Drawing.Size(640, 210);
			this.pnlDataFields.TabIndex = 4;
			// 
			// gbNodeEditor
			// 
			this.gbNodeEditor.Controls.Add(this.btnCut);
			this.gbNodeEditor.Controls.Add(this.btnCopy);
			this.gbNodeEditor.Controls.Add(this.btnPaste);
			this.gbNodeEditor.Controls.Add(this.btnDelete);
			this.gbNodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbNodeEditor.Location = new System.Drawing.Point(265, 153);
			this.gbNodeEditor.Name = "gbNodeEditor";
			this.gbNodeEditor.Size = new System.Drawing.Size(375, 57);
			this.gbNodeEditor.TabIndex = 12;
			this.gbNodeEditor.TabStop = false;
			this.gbNodeEditor.Text = "Node editor";
			// 
			// btnCut
			// 
			this.btnCut.Enabled = false;
			this.btnCut.Location = new System.Drawing.Point(5, 15);
			this.btnCut.Name = "btnCut";
			this.btnCut.Size = new System.Drawing.Size(65, 30);
			this.btnCut.TabIndex = 37;
			this.btnCut.Text = "Cut";
			this.toolTip1.SetToolTip(this.btnCut, "cuts the selected node");
			this.btnCut.Click += new System.EventHandler(this.OnCutClick);
			// 
			// pnlDataFieldsLeft
			// 
			this.pnlDataFieldsLeft.Controls.Add(this.gbSpawnData);
			this.pnlDataFieldsLeft.Controls.Add(this.gbPatrolData);
			this.pnlDataFieldsLeft.Controls.Add(this.gbNodeData);
			this.pnlDataFieldsLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlDataFieldsLeft.Location = new System.Drawing.Point(0, 3);
			this.pnlDataFieldsLeft.Name = "pnlDataFieldsLeft";
			this.pnlDataFieldsLeft.Size = new System.Drawing.Size(265, 207);
			this.pnlDataFieldsLeft.TabIndex = 11;
			// 
			// gbPatrolData
			// 
			this.gbPatrolData.Controls.Add(this.cbUnitType);
			this.gbPatrolData.Controls.Add(this.cbAttack);
			this.gbPatrolData.Controls.Add(this.cbPriority);
			this.gbPatrolData.Controls.Add(this.labelAttack);
			this.gbPatrolData.Controls.Add(this.labelUnitType);
			this.gbPatrolData.Controls.Add(this.labelPriority);
			this.gbPatrolData.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbPatrolData.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbPatrolData.Location = new System.Drawing.Point(0, 45);
			this.gbPatrolData.Name = "gbPatrolData";
			this.gbPatrolData.Size = new System.Drawing.Size(265, 90);
			this.gbPatrolData.TabIndex = 11;
			this.gbPatrolData.TabStop = false;
			this.gbPatrolData.Text = "Patrol data";
			// 
			// cbAttack
			// 
			this.cbAttack.Location = new System.Drawing.Point(85, 65);
			this.cbAttack.Name = "cbAttack";
			this.cbAttack.Size = new System.Drawing.Size(130, 20);
			this.cbAttack.TabIndex = 8;
			this.toolTip1.SetToolTip(this.cbAttack, "attacts an aLien to shoot at XCom base tiles");
			this.cbAttack.SelectedIndexChanged += new System.EventHandler(this.OnBaseAttackSelectedIndexChanged);
			// 
			// labelAttack
			// 
			this.labelAttack.Location = new System.Drawing.Point(10, 70);
			this.labelAttack.Name = "labelAttack";
			this.labelAttack.Size = new System.Drawing.Size(75, 15);
			this.labelAttack.TabIndex = 3;
			this.labelAttack.Text = "Attack Base";
			this.labelAttack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelAttack, "attacts an aLien to shoot at XCom base tiles");
			// 
			// gbNodeData
			// 
			this.gbNodeData.Controls.Add(this.lblOverId);
			this.gbNodeData.Controls.Add(this.lblSelectedPosition);
			this.gbNodeData.Controls.Add(this.lblSelectedId);
			this.gbNodeData.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbNodeData.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbNodeData.Location = new System.Drawing.Point(0, 0);
			this.gbNodeData.Name = "gbNodeData";
			this.gbNodeData.Size = new System.Drawing.Size(265, 45);
			this.gbNodeData.TabIndex = 1;
			this.gbNodeData.TabStop = false;
			this.gbNodeData.Text = "Tile data";
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 300;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.UseAnimation = false;
			// 
			// RouteView
			// 
			this.Controls.Add(this.pnlRoutes);
			this.Controls.Add(this.tsMain);
			this.Controls.Add(this.pnlDataFields);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "RouteView";
			this.Size = new System.Drawing.Size(640, 480);
			this.Click += new System.EventHandler(this.OnOgClick);
			this.gbSpawnData.ResumeLayout(false);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.gbLinkData.ResumeLayout(false);
			this.gbLinkData.PerformLayout();
			this.pnlDataFields.ResumeLayout(false);
			this.gbNodeEditor.ResumeLayout(false);
			this.pnlDataFieldsLeft.ResumeLayout(false);
			this.gbPatrolData.ResumeLayout(false);
			this.gbNodeData.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label lblSelectedPosition;
		private System.Windows.Forms.Label labelUnitType;
		private System.Windows.Forms.ComboBox cbUnitType;
		private System.Windows.Forms.ComboBox cbSpawnRank;
		private System.Windows.Forms.ComboBox cbPriority;
		private System.Windows.Forms.Label labelSpawnRank;
		private System.Windows.Forms.Label labelPriority;
		private System.Windows.Forms.Label labelSpawnWeight;
		private System.Windows.Forms.Label labelLink5;
		private System.Windows.Forms.Label labelLink4;
		private System.Windows.Forms.Label labelLink3;
		private System.Windows.Forms.Label labelLink2;
		private System.Windows.Forms.Label labelLink1;
		private System.Windows.Forms.ComboBox cbLink1Dest;
		private System.Windows.Forms.ComboBox cbLink2Dest;
		private System.Windows.Forms.ComboBox cbLink3Dest;
		private System.Windows.Forms.ComboBox cbLink4Dest;
		private System.Windows.Forms.ComboBox cbLink5Dest;
		private System.Windows.Forms.GroupBox gbSpawnData;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Label labelUnitInfo;
		private System.Windows.Forms.Label labelDist;
		private System.Windows.Forms.ComboBox cbLink1UnitType;
		private System.Windows.Forms.ComboBox cbLink2UnitType;
		private System.Windows.Forms.ComboBox cbLink3UnitType;
		private System.Windows.Forms.ComboBox cbLink4UnitType;
		private System.Windows.Forms.ComboBox cbLink5UnitType;
		private System.Windows.Forms.TextBox tbLink1Dist;
		private System.Windows.Forms.TextBox tbLink2Dist;
		private System.Windows.Forms.TextBox tbLink3Dist;
		private System.Windows.Forms.TextBox tbLink4Dist;
		private System.Windows.Forms.TextBox tbLink5Dist;
		private System.Windows.Forms.Label lblSelectedId;
		private System.Windows.Forms.ComboBox cbSpawnWeight;
		private System.Windows.Forms.Label lblOverId;
		private System.Windows.Forms.Button btnPaste;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.GroupBox gbLinkData;
		private System.Windows.Forms.Panel pnlDataFields;
		private System.Windows.Forms.Panel pnlDataFieldsLeft;
		private System.Windows.Forms.GroupBox gbPatrolData;
		private System.Windows.Forms.ComboBox cbAttack;
		private System.Windows.Forms.Label labelAttack;
		private System.Windows.Forms.GroupBox gbNodeData;
		private System.Windows.Forms.GroupBox gbNodeEditor;
		private System.Windows.Forms.Button btnCut;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripComboBox tscbConnectType;
		private System.Windows.Forms.ToolStripDropDownButton tsddbEdit;
		private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
		private System.Windows.Forms.ToolStripMenuItem tsmiAllNodesRank0;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripMenuItem tsmiClearLinkData;
		private System.Windows.Forms.Button btnGoLink5;
		private System.Windows.Forms.Button btnGoLink4;
		private System.Windows.Forms.Button btnGoLink3;
		private System.Windows.Forms.Button btnGoLink2;
		private System.Windows.Forms.Button btnGoLink1;
		private System.Windows.Forms.Button btnOg;
	}
}
