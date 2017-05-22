using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView
{
	/// <summary>
	/// General HelpScreen.
	/// </summary>
	internal sealed class Help
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal Help()
		{
			InitializeComponent();

			OnCheckChanged(null, null);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Wraps the several color-updates into one call.
		/// </summary>
		internal void UpdateColors()
		{
			UpdateTopViewBlobColors();
			UpdateRouteViewBlobColors();
			UpdateSpecialPropertyColors();
		}

		/// <summary>
		/// Updates the TopView blob colors via an arcane methodology from the
		/// user's custom Options.
		/// </summary>
		private void UpdateTopViewBlobColors()
		{
			var pens    = MapView.Forms.MainWindow.ViewerFormsManager.TopView.Control.GetWallPens();
			var brushes = MapView.Forms.MainWindow.ViewerFormsManager.TopView.Control.GetFloorContentBrushes();

			Color color = Color.Empty;

			string partType = MapView.Forms.MapObservers.TopViews.TopView.FloorColor;
			if (brushes.ContainsKey(partType))
			{
				color = ((SolidBrush)brushes[partType]).Color;
				label7.BackColor = color;
				label7.ForeColor = GetTextColor(color);
			}

			partType = MapView.Forms.MapObservers.TopViews.TopView.WestColor;
			if (pens.ContainsKey(partType))
			{
				color = ((Pen)pens[partType]).Color;
				label8.BackColor = color;
				label8.ForeColor = GetTextColor(color);
			}

			partType = MapView.Forms.MapObservers.TopViews.TopView.NorthColor;
			if (pens.ContainsKey(partType))
			{
				color = ((Pen)pens[partType]).Color;
				label9.BackColor = color;
				label9.ForeColor = GetTextColor(color);
			}

			partType = MapView.Forms.MapObservers.TopViews.TopView.ContentColor;
			if (brushes.ContainsKey(partType))
			{
				color = ((SolidBrush)brushes[partType]).Color;
				label10.BackColor = color;
				label10.ForeColor = GetTextColor(color);
			}
		}

		/// <summary>
		/// Updates the RouteView blob colors via an arcane methodology from the
		/// user's custom Options.
		/// </summary>
		private void UpdateRouteViewBlobColors()
		{
			var penWall      = MapView.Forms.MainWindow.ViewerFormsManager.RouteView.Control.GetWallPens();
			var brushContent = MapView.Forms.MainWindow.ViewerFormsManager.RouteView.Control.GetContentBrushes();

			Color color = Color.Empty;

			string partType = MapView.Forms.MapObservers.RouteViews.RouteView.WallColor;
			if (penWall.ContainsKey(partType))
			{
				color = ((Pen)penWall[partType]).Color;

				label14.BackColor = color;
				label15.BackColor = color;

				label14.ForeColor = GetTextColor(color);
				label15.ForeColor = GetTextColor(color);
			}

			partType = MapView.Forms.MapObservers.RouteViews.RouteView.ContentColor;
			if (brushContent.ContainsKey(partType))
			{
				color = ((SolidBrush)brushContent[partType]).Color;
				label16.BackColor = color;
				label16.ForeColor = GetTextColor(color);
			}
		}

		/// <summary>
		/// Updates TileView's special property colors via an arcane methodology
		/// from the user's custom Options.
		/// </summary>
		private void UpdateSpecialPropertyColors()
		{
			// TODO: update special property colors from Options without
			// requiring that the Help screen be reloaded. Neither form
			// (Options or Help) is modal, so the code can't rely on that
			// user-forced effect.
			var brushesSpecial = MapView.Forms.MainWindow.ViewerFormsManager.TileView.Control.GetSpecialPropertyBrushes();

			Color color = Color.Empty;

			// TODO: iterate through the labels using a function-pointer/delegate
			// if possible.

			string specialType = Enum.GetName(typeof(SpecialType), 0);	// "Standard"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType00.BackColor = color;
				lblType00.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 1);			// "EntryPoint"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType01.BackColor = color;
				lblType01.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 2);			// "PowerSource"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType02.BackColor = color;
				lblType02.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 3);			// "Navigation"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType03.BackColor = color;
				lblType03.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 4);			// "Construction"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType04.BackColor = color;
				lblType04.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 5);			// "Food"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType05.BackColor = color;
				lblType05.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 6);			// "Reproduction"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType06.BackColor = color;
				lblType06.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 7);			// "Entertainment"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType07.BackColor = color;
				lblType07.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 8);			// "Surgery"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType08.BackColor = color;
				lblType08.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 9);			// "ExaminationRoom"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType09.BackColor = color;
				lblType09.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 10);		// "Alloys"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType10.BackColor = color;
				lblType10.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 11);		// "Habitat"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType11.BackColor = color;
				lblType11.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 12);		// "DeadTile"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType12.BackColor = color;
				lblType12.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 13);		// "ExitPoint"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType13.BackColor = color;
				lblType13.ForeColor = GetTextColor(color);
			}

			specialType = Enum.GetName(typeof(SpecialType), 14);		// "MustDestroy"
			if (brushesSpecial.ContainsKey(specialType))
			{
				color = ((SolidBrush)brushesSpecial[specialType]).Color;
				lblType14.BackColor = color;
				lblType14.ForeColor = GetTextColor(color);
			}
		}

		/// <summary>
		/// Gets a contrasting color based on the input color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static Color GetTextColor(Color color)
		{
			// TODO: check alpha .....
			return ((int)color.R + color.G + color.B > 485) ? Color.DarkSlateBlue
															: Color.Snow;
		}
		#endregion


		#region Event Calls
		/// <summary>
		/// Toggles the text descriptions between UFO and TFTD special property
		/// types.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckChanged(object sender, EventArgs e)
		{
			if (rbUfo.Checked)
			{
				lblType00.Text = "tile"; // switch to UFO ->
				lblType01.Text = "entry point";
				lblType02.Text = "power source";
				lblType03.Text = "navigation";
				lblType04.Text = "construction";
				lblType05.Text = "food";
				lblType06.Text = "reproduction";
				lblType07.Text = "entertainment";
				lblType08.Text = "surgery";
				lblType09.Text = "examination";
				lblType10.Text = "alien alloys";
				lblType11.Text = "habitat";
				lblType12.Text = "dead tile";
				lblType13.Text = "exit point";
				lblType14.Text = "must destroy";
			}
			else // rbTftd.Checked
			{
				lblType00.Text = "tile"; // switch to TFTD ->
				lblType01.Text = "entry point";
				lblType02.Text = "ion accelerator";
				lblType03.Text = "destroy";
				lblType04.Text = "navigation";
				lblType05.Text = "cryogenics";
				lblType06.Text = "cloning";
				lblType07.Text = "learning arrays";
				lblType08.Text = "implanter";
				lblType09.Text = "unknown";
				lblType10.Text = "plastics";
				lblType11.Text = "re-animation";
				lblType12.Text = "dead tile";
				lblType13.Text = "exit point";
				lblType14.Text = "must destroy";
			}
		}

		/// <summary>
		/// Closes the Help screen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
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

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabMain = new System.Windows.Forms.TabControl();
			this.tpMainView = new System.Windows.Forms.TabPage();
			this.label17 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tpTopView = new System.Windows.Forms.TabPage();
			this.gbTopViewColors = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tpRouteView = new System.Windows.Forms.TabPage();
			this.gbRouteViewColors = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.tpTileView = new System.Windows.Forms.TabPage();
			this.label26 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.rbTftd = new System.Windows.Forms.RadioButton();
			this.rbUfo = new System.Windows.Forms.RadioButton();
			this.gbTileViewColors = new System.Windows.Forms.GroupBox();
			this.lblType09 = new System.Windows.Forms.Label();
			this.lblType14 = new System.Windows.Forms.Label();
			this.lblType13 = new System.Windows.Forms.Label();
			this.lblType12 = new System.Windows.Forms.Label();
			this.lblType11 = new System.Windows.Forms.Label();
			this.lblType10 = new System.Windows.Forms.Label();
			this.lblType08 = new System.Windows.Forms.Label();
			this.lblType07 = new System.Windows.Forms.Label();
			this.lblType06 = new System.Windows.Forms.Label();
			this.lblType05 = new System.Windows.Forms.Label();
			this.lblType04 = new System.Windows.Forms.Label();
			this.lblType03 = new System.Windows.Forms.Label();
			this.lblType02 = new System.Windows.Forms.Label();
			this.lblType01 = new System.Windows.Forms.Label();
			this.lblType00 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.tabMain.SuspendLayout();
			this.tpMainView.SuspendLayout();
			this.tpTopView.SuspendLayout();
			this.gbTopViewColors.SuspendLayout();
			this.tpRouteView.SuspendLayout();
			this.gbRouteViewColors.SuspendLayout();
			this.tpTileView.SuspendLayout();
			this.gbTileViewColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.tpMainView);
			this.tabMain.Controls.Add(this.tpTopView);
			this.tabMain.Controls.Add(this.tpRouteView);
			this.tabMain.Controls.Add(this.tpTileView);
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.Location = new System.Drawing.Point(0, 0);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(454, 276);
			this.tabMain.TabIndex = 0;
			// 
			// tpMainView
			// 
			this.tpMainView.Controls.Add(this.label17);
			this.tpMainView.Controls.Add(this.label12);
			this.tpMainView.Controls.Add(this.label2);
			this.tpMainView.Controls.Add(this.label1);
			this.tpMainView.Location = new System.Drawing.Point(4, 21);
			this.tpMainView.Name = "tpMainView";
			this.tpMainView.Size = new System.Drawing.Size(446, 251);
			this.tpMainView.TabIndex = 0;
			this.tpMainView.Text = "Main View";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(5, 95);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(440, 15);
			this.label17.TabIndex = 3;
			this.label17.Text = "You MUST SAVE before selecting another map or your changes will be lost.";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(5, 70);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(440, 15);
			this.label12.TabIndex = 2;
			this.label12.Text = "Your window locations will be saved on program exit.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(440, 25);
			this.label2.TabIndex = 1;
			this.label2.Text = "Turning the animation off makes it harder to see which tile you are going to clic" +
	"k on.";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(440, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Click anywhere to set the tile to edit.";
			// 
			// tpTopView
			// 
			this.tpTopView.Controls.Add(this.gbTopViewColors);
			this.tpTopView.Controls.Add(this.label22);
			this.tpTopView.Controls.Add(this.label11);
			this.tpTopView.Controls.Add(this.label6);
			this.tpTopView.Controls.Add(this.label5);
			this.tpTopView.Controls.Add(this.label4);
			this.tpTopView.Controls.Add(this.label3);
			this.tpTopView.Location = new System.Drawing.Point(4, 21);
			this.tpTopView.Name = "tpTopView";
			this.tpTopView.Size = new System.Drawing.Size(446, 251);
			this.tpTopView.TabIndex = 1;
			this.tpTopView.Text = "Top View";
			// 
			// gbTopViewColors
			// 
			this.gbTopViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTopViewColors.Controls.Add(this.label7);
			this.gbTopViewColors.Controls.Add(this.label9);
			this.gbTopViewColors.Controls.Add(this.label10);
			this.gbTopViewColors.Controls.Add(this.label8);
			this.gbTopViewColors.Location = new System.Drawing.Point(10, 175);
			this.gbTopViewColors.Name = "gbTopViewColors";
			this.gbTopViewColors.Size = new System.Drawing.Size(430, 55);
			this.gbTopViewColors.TabIndex = 11;
			this.gbTopViewColors.TabStop = false;
			this.gbTopViewColors.Text = "Tile Colors";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label7.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label7.Location = new System.Drawing.Point(10, 20);
			this.label7.Name = "label7";
			this.label7.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label7.Size = new System.Drawing.Size(95, 25);
			this.label7.TabIndex = 4;
			this.label7.Text = "floor";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label9.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label9.Location = new System.Drawing.Point(220, 20);
			this.label9.Name = "label9";
			this.label9.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label9.Size = new System.Drawing.Size(95, 25);
			this.label9.TabIndex = 10;
			this.label9.Text = "north";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label10.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label10.Location = new System.Drawing.Point(325, 20);
			this.label10.Name = "label10";
			this.label10.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label10.Size = new System.Drawing.Size(95, 25);
			this.label10.TabIndex = 5;
			this.label10.Text = "content";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label8.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label8.Location = new System.Drawing.Point(115, 20);
			this.label8.Name = "label8";
			this.label8.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label8.Size = new System.Drawing.Size(95, 25);
			this.label8.TabIndex = 6;
			this.label8.Text = "west";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(5, 135);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(440, 25);
			this.label22.TabIndex = 9;
			this.label22.Text = "Right clicking on the grid will set the selected tile in TileView in the selected" +
	" portion on the bottom.";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(5, 110);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(440, 15);
			this.label11.TabIndex = 8;
			this.label11.Text = "Setting the size will make things larger/smaller.";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 85);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(440, 15);
			this.label6.TabIndex = 3;
			this.label6.Text = "Double right click to clear the clicked on tile.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 60);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(440, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "Right click to set the currently selected tile to the one selected in Tile View.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 35);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(440, 15);
			this.label4.TabIndex = 1;
			this.label4.Text = "Double left click to set the currently selected tile in Tile View.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(440, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "Click anywhere to set the tile to edit.";
			// 
			// tpRouteView
			// 
			this.tpRouteView.Controls.Add(this.gbRouteViewColors);
			this.tpRouteView.Controls.Add(this.label20);
			this.tpRouteView.Controls.Add(this.label18);
			this.tpRouteView.Controls.Add(this.label13);
			this.tpRouteView.Controls.Add(this.label21);
			this.tpRouteView.Location = new System.Drawing.Point(4, 22);
			this.tpRouteView.Name = "tpRouteView";
			this.tpRouteView.Size = new System.Drawing.Size(446, 250);
			this.tpRouteView.TabIndex = 2;
			this.tpRouteView.Text = "Route View";
			// 
			// gbRouteViewColors
			// 
			this.gbRouteViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbRouteViewColors.Controls.Add(this.label16);
			this.gbRouteViewColors.Controls.Add(this.label15);
			this.gbRouteViewColors.Controls.Add(this.label14);
			this.gbRouteViewColors.Location = new System.Drawing.Point(10, 115);
			this.gbRouteViewColors.Name = "gbRouteViewColors";
			this.gbRouteViewColors.Size = new System.Drawing.Size(325, 55);
			this.gbRouteViewColors.TabIndex = 21;
			this.gbRouteViewColors.TabStop = false;
			this.gbRouteViewColors.Text = "Tile Colors";
			// 
			// label16
			// 
			this.label16.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label16.Location = new System.Drawing.Point(220, 20);
			this.label16.Name = "label16";
			this.label16.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label16.Size = new System.Drawing.Size(95, 25);
			this.label16.TabIndex = 14;
			this.label16.Text = "content";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label15.Location = new System.Drawing.Point(115, 20);
			this.label15.Name = "label15";
			this.label15.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label15.Size = new System.Drawing.Size(95, 25);
			this.label15.TabIndex = 20;
			this.label15.Text = "north";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label14.Location = new System.Drawing.Point(10, 20);
			this.label14.Name = "label14";
			this.label14.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label14.Size = new System.Drawing.Size(95, 25);
			this.label14.TabIndex = 15;
			this.label14.Text = "west";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(5, 85);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(440, 15);
			this.label20.TabIndex = 19;
			this.label20.Text = "Right click on the grid to place a new node.";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(5, 60);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(440, 15);
			this.label18.TabIndex = 18;
			this.label18.Text = "After editing the distance, you must press enter to save the change.";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(5, 35);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(440, 15);
			this.label13.TabIndex = 17;
			this.label13.Text = "Clicking a green square will select a node to edit.";
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(5, 10);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(440, 15);
			this.label21.TabIndex = 9;
			this.label21.Text = "Click anywhere to set the tile to edit.";
			// 
			// tpTileView
			// 
			this.tpTileView.Controls.Add(this.label26);
			this.tpTileView.Controls.Add(this.label25);
			this.tpTileView.Controls.Add(this.rbTftd);
			this.tpTileView.Controls.Add(this.rbUfo);
			this.tpTileView.Controls.Add(this.gbTileViewColors);
			this.tpTileView.Controls.Add(this.label19);
			this.tpTileView.Location = new System.Drawing.Point(4, 21);
			this.tpTileView.Name = "tpTileView";
			this.tpTileView.Size = new System.Drawing.Size(446, 251);
			this.tpTileView.TabIndex = 3;
			this.tpTileView.Text = "Tile View";
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(160, 60);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(275, 25);
			this.label26.TabIndex = 15;
			this.label26.Text = "note: Help must be closed and re-opened to update any colors that were changed in" +
	" Options.";
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(5, 35);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(440, 15);
			this.label25.TabIndex = 14;
			this.label25.Text = "These are background colors for the special tile properties.";
			// 
			// rbTftd
			// 
			this.rbTftd.Location = new System.Drawing.Point(20, 75);
			this.rbTftd.Name = "rbTftd";
			this.rbTftd.Size = new System.Drawing.Size(55, 15);
			this.rbTftd.TabIndex = 13;
			this.rbTftd.Text = "TFTD";
			this.rbTftd.UseVisualStyleBackColor = true;
			this.rbTftd.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
			// 
			// rbUfo
			// 
			this.rbUfo.Checked = true;
			this.rbUfo.Location = new System.Drawing.Point(20, 55);
			this.rbUfo.Name = "rbUfo";
			this.rbUfo.Size = new System.Drawing.Size(55, 15);
			this.rbUfo.TabIndex = 12;
			this.rbUfo.TabStop = true;
			this.rbUfo.Text = "UFO";
			this.rbUfo.UseVisualStyleBackColor = true;
			this.rbUfo.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
			// 
			// gbTileViewColors
			// 
			this.gbTileViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTileViewColors.Controls.Add(this.lblType09);
			this.gbTileViewColors.Controls.Add(this.lblType14);
			this.gbTileViewColors.Controls.Add(this.lblType13);
			this.gbTileViewColors.Controls.Add(this.lblType12);
			this.gbTileViewColors.Controls.Add(this.lblType11);
			this.gbTileViewColors.Controls.Add(this.lblType10);
			this.gbTileViewColors.Controls.Add(this.lblType08);
			this.gbTileViewColors.Controls.Add(this.lblType07);
			this.gbTileViewColors.Controls.Add(this.lblType06);
			this.gbTileViewColors.Controls.Add(this.lblType05);
			this.gbTileViewColors.Controls.Add(this.lblType04);
			this.gbTileViewColors.Controls.Add(this.lblType03);
			this.gbTileViewColors.Controls.Add(this.lblType02);
			this.gbTileViewColors.Controls.Add(this.lblType01);
			this.gbTileViewColors.Controls.Add(this.lblType00);
			this.gbTileViewColors.Location = new System.Drawing.Point(10, 100);
			this.gbTileViewColors.Name = "gbTileViewColors";
			this.gbTileViewColors.Size = new System.Drawing.Size(430, 150);
			this.gbTileViewColors.TabIndex = 11;
			this.gbTileViewColors.TabStop = false;
			this.gbTileViewColors.Text = "Tile Colors";
			// 
			// lblType09
			// 
			this.lblType09.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType09.Location = new System.Drawing.Point(10, 95);
			this.lblType09.Name = "lblType09";
			this.lblType09.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType09.Size = new System.Drawing.Size(130, 20);
			this.lblType09.TabIndex = 26;
			this.lblType09.Text = "09";
			this.lblType09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType14
			// 
			this.lblType14.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType14.Location = new System.Drawing.Point(290, 120);
			this.lblType14.Name = "lblType14";
			this.lblType14.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType14.Size = new System.Drawing.Size(130, 20);
			this.lblType14.TabIndex = 25;
			this.lblType14.Text = "14";
			this.lblType14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType13
			// 
			this.lblType13.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType13.Location = new System.Drawing.Point(150, 120);
			this.lblType13.Name = "lblType13";
			this.lblType13.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType13.Size = new System.Drawing.Size(130, 20);
			this.lblType13.TabIndex = 24;
			this.lblType13.Text = "13";
			this.lblType13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType12
			// 
			this.lblType12.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType12.Location = new System.Drawing.Point(10, 120);
			this.lblType12.Name = "lblType12";
			this.lblType12.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType12.Size = new System.Drawing.Size(130, 20);
			this.lblType12.TabIndex = 23;
			this.lblType12.Text = "12";
			this.lblType12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType11
			// 
			this.lblType11.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType11.Location = new System.Drawing.Point(290, 95);
			this.lblType11.Name = "lblType11";
			this.lblType11.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType11.Size = new System.Drawing.Size(130, 20);
			this.lblType11.TabIndex = 22;
			this.lblType11.Text = "11";
			this.lblType11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType10
			// 
			this.lblType10.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType10.Location = new System.Drawing.Point(150, 95);
			this.lblType10.Name = "lblType10";
			this.lblType10.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType10.Size = new System.Drawing.Size(130, 20);
			this.lblType10.TabIndex = 21;
			this.lblType10.Text = "10";
			this.lblType10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType08
			// 
			this.lblType08.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType08.Location = new System.Drawing.Point(290, 70);
			this.lblType08.Name = "lblType08";
			this.lblType08.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType08.Size = new System.Drawing.Size(130, 20);
			this.lblType08.TabIndex = 20;
			this.lblType08.Text = "08";
			this.lblType08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType07
			// 
			this.lblType07.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType07.Location = new System.Drawing.Point(150, 70);
			this.lblType07.Name = "lblType07";
			this.lblType07.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType07.Size = new System.Drawing.Size(130, 20);
			this.lblType07.TabIndex = 19;
			this.lblType07.Text = "07";
			this.lblType07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType06
			// 
			this.lblType06.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType06.Location = new System.Drawing.Point(10, 70);
			this.lblType06.Name = "lblType06";
			this.lblType06.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType06.Size = new System.Drawing.Size(130, 20);
			this.lblType06.TabIndex = 18;
			this.lblType06.Text = "06";
			this.lblType06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType05
			// 
			this.lblType05.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType05.Location = new System.Drawing.Point(290, 45);
			this.lblType05.Name = "lblType05";
			this.lblType05.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType05.Size = new System.Drawing.Size(130, 20);
			this.lblType05.TabIndex = 17;
			this.lblType05.Text = "05";
			this.lblType05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType04
			// 
			this.lblType04.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType04.Location = new System.Drawing.Point(150, 45);
			this.lblType04.Name = "lblType04";
			this.lblType04.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType04.Size = new System.Drawing.Size(130, 20);
			this.lblType04.TabIndex = 16;
			this.lblType04.Text = "04";
			this.lblType04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType03
			// 
			this.lblType03.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType03.Location = new System.Drawing.Point(10, 45);
			this.lblType03.Name = "lblType03";
			this.lblType03.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType03.Size = new System.Drawing.Size(130, 20);
			this.lblType03.TabIndex = 15;
			this.lblType03.Text = "03";
			this.lblType03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType02
			// 
			this.lblType02.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType02.Location = new System.Drawing.Point(290, 20);
			this.lblType02.Name = "lblType02";
			this.lblType02.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType02.Size = new System.Drawing.Size(130, 20);
			this.lblType02.TabIndex = 14;
			this.lblType02.Text = "02";
			this.lblType02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType01
			// 
			this.lblType01.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType01.Location = new System.Drawing.Point(150, 20);
			this.lblType01.Name = "lblType01";
			this.lblType01.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType01.Size = new System.Drawing.Size(130, 20);
			this.lblType01.TabIndex = 13;
			this.lblType01.Text = "01";
			this.lblType01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType00
			// 
			this.lblType00.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType00.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblType00.Location = new System.Drawing.Point(10, 20);
			this.lblType00.Name = "lblType00";
			this.lblType00.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType00.Size = new System.Drawing.Size(130, 20);
			this.lblType00.TabIndex = 12;
			this.lblType00.Text = "00";
			this.lblType00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(5, 10);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(440, 15);
			this.label19.TabIndex = 10;
			this.label19.Text = "Left click to select the tile to place.";
			// 
			// Help
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(454, 276);
			this.Controls.Add(this.tabMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Help";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Help";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.tabMain.ResumeLayout(false);
			this.tpMainView.ResumeLayout(false);
			this.tpTopView.ResumeLayout(false);
			this.gbTopViewColors.ResumeLayout(false);
			this.tpRouteView.ResumeLayout(false);
			this.gbRouteViewColors.ResumeLayout(false);
			this.tpTileView.ResumeLayout(false);
			this.gbTileViewColors.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private TabControl tabMain;
		private TabPage tpMainView;
		private TabPage tpTopView;
		private TabPage tpRouteView;
		private TabPage tpTileView;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label label10;
		private Label label8;
		private Label label11;
		private Label label12;
		private Label label14;
		private Label label16;
		private Label label21;
		private Label label13;
		private Label label17;
		private Label label18;
		private Label label19;
		private Label label20;
		private Label label22;
		private Label label9;
		private Label label15;
		private Label label25;
		private GroupBox gbTileViewColors;
		private Label lblType00;
		private Label lblType01;
		private Label lblType02;
		private Label lblType03;
		private Label lblType04;
		private Label lblType05;
		private Label lblType06;
		private Label lblType07;
		private Label lblType08;
		private Label lblType09;
		private Label lblType10;
		private Label lblType11;
		private Label lblType12;
		private Label lblType13;
		private Label lblType14;
		private RadioButton rbTftd;
		private RadioButton rbUfo;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.GroupBox gbTopViewColors;
		private System.Windows.Forms.GroupBox gbRouteViewColors;
	}
}
