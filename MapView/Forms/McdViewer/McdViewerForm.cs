using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.McdViewer
{
	internal sealed partial class McdViewerForm
		:
			Form
	{
		public McdViewerForm()
		{
			InitializeComponent();

			rtbInfo.WordWrap = false;
			rtbInfo.ReadOnly = true;
		}


		public void UpdateData(McdRecord record)
		{
			bsInfo.DataSource = record;

			rtbInfo.Text = String.Empty;

			if (record != null)
			{
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText(record.Reference0To30);
				rtbInfo.AppendText(Environment.NewLine);
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText(record.Reference30To62);
				rtbInfo.AppendText(Environment.NewLine + Environment.NewLine);

				rtbInfo.SelectionColor = Color.Black;
				rtbInfo.AppendText(string.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"Images: {0} {1} {2} {3} {4} {5} {6} {7}" + Environment.NewLine,
											record.Image1,
											record.Image2,
											record.Image3,
											record.Image4,
											record.Image5,
											record.Image6,
											record.Image7,
											record.Image8));

				rtbInfo.AppendText(record.LoftReference);

				rtbInfo.AppendText(record.ScanGReference);

//				short int ScanG; // A reference into the GEODATA\SCANG.DAT
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[22])); // unsigned char u23;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[23])); // unsigned char u24;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[24])); // unsigned char u25;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[25])); // unsigned char u26;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[26])); // unsigned char u27;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[27])); // unsigned char u28;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[28])); // unsigned char u29;
//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[29])); // unsigned char u30;

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "UFO Door: {0}" + Environment.NewLine, record.UfoDoor));
//				unsigned char UFO_Door;	// If it's a UFO door it uses only Frame[0] until it is walked through, then
										// it animates once and becomes Alt_MCD. It changes back at the end of the turn

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "SeeThru: {0}" + Environment.NewLine, record.StopLOS));
//				unsigned char Stop_LOS; // You cannot see through this tile.

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "No Floor: {0}" + Environment.NewLine, record.NoGround));
//				unsigned char No_Floor; // If 1, then a non-flying unit can't stand here

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Big Wall: {0}" + Environment.NewLine, record.BigWall));
//				unsigned char Big_Wall; // It's an object (tile type 3), but it acts like a wall

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Grav Lift: {0}" + Environment.NewLine, record.GravLift));
//				unsigned char Gravlift;

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "People Door: {0}" + Environment.NewLine, record.HumanDoor));
//				unsigned char Door; // It's a human style door--you walk through it and it changes to Alt_MCD

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Block fire: {0}" + Environment.NewLine, record.BlockFire));
//				unsigned char Block_Fire; // If 1, fire won't go through the tile

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Block Smoke: {0}" + Environment.NewLine, record.BlockSmoke));
//				unsigned char Block_Smoke; // If 1, smoke won't go through the tile

				//rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[38]));
//				unsigned char u39;

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "TU Walk: {0}" + Environment.NewLine, record.TU_Walk));
//				unsigned char TU_Walk; // The number of TUs required to pass the tile while walking. 0xFF (255) means it's unpassable.

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "TU Fly: {0}" + Environment.NewLine, record.TU_Fly));
//				unsigned char TU_Fly; // remember, 0xFF means it's impassable!

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "TU Slide: {0}" + Environment.NewLine, record.TU_Slide));
//				unsigned char TU_Slide; // sliding things include snakemen and silacoids

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Armor: {0}" + Environment.NewLine, record.Armor));
//				unsigned char Armour; // The higher this is the less likely it is that a weapon will destroy this tile when it's hit.

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "HE Block: {0}" + Environment.NewLine, record.HE_Block));
//				unsigned char HE_Block; // How much of an explosion this tile will block

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Flammable: {0}" + Environment.NewLine, record.Flammable));
//				unsigned char Flammable; // How flammable it is (the higher the harder it is to set aflame)

				rtbInfo.SelectionColor = Color.Crimson;
				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Death tile: {0}" + Environment.NewLine, record.DieTile));
//				unsigned char Die_MCD; // If the terrain is destroyed, it is set to 0 and a tile of type Die_MCD is added

				rtbInfo.SelectionColor = Color.Crimson;
				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Door open tile: {0}" + Environment.NewLine, record.Alt_MCD));
//				unsigned char Alt_MCD; // If "Door" or "UFO_Door" is on, then when a unit walks through it the door is set to 0 and a tile type Alt_MCD is added.

//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[47])); // unsigned char u48;

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unit y offset: {0}" + Environment.NewLine, record.StandOffset));
//				signed char T_Level; // When a unit or object is standing on the tile, the unit is shifted by this amount

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "tile y offset: {0}" + Environment.NewLine, record.TileOffset));
//				unsigned char P_Level; // When the tile is drawn, this amount is subtracted from its y (so y position-P_Level is where it's drawn)

//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[50]));// unsigned char u51;

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "block light[0-10]: {0}" + Environment.NewLine, record.LightBlock));
//				unsigned char Light_Block; // The amount of light it blocks, from 0 to 10

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "footstep sound effect: {0}" + Environment.NewLine, record.Footstep));
//				unsigned char Footstep; // The Sound Effect set to choose from when footsteps are on the tile

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "tile type: {0} - {1}" + Environment.NewLine, (sbyte)record.TileType, Enum.GetName(typeof(TileType), record.TileType)));
//				info.TileType==0?"floor":info.TileType==1?"west wall":info.TileType==2?"north wall":info.TileType==3?"object":"Unknown"));
//				unsigned char Tile_Type;	// This is the type of tile it is meant to be -- 0=floor, 1=west wall, 2=north wall, 3=object.
											// When this type of tile is in the Die_As or Open_As flags, this value is added to the tile
											// coordinate to determine the byte in which the tile type should be written.

				rtbInfo.AppendText(string.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"high explosive type: {0} - {1}" + Environment.NewLine,
										record.HE_Type,
										(record.HE_Type == 0) ? "HE" : (record.HE_Type == 1) ? "smoke" : "unknown"));
//				unsigned char HE_Type; // 0=HE 1=Smoke

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "HE Strength: {0}" + Environment.NewLine, record.HE_Strength));
//				unsigned char HE_Strength; // The strength of the explosion caused when it's destroyed. 0 means no explosion.

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "smoke blockage: {0}" + Environment.NewLine, record.SmokeBlockage));
//				unsigned char Smoke_Blockage; // ? Not sure about this

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "turns to burn: {0}" + Environment.NewLine, record.Fuel));
//				unsigned char Fuel; // The number of turns the tile will burn when set aflame

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "light produced: {0}" + Environment.NewLine, record.LightSource));
//				unsigned char Light_Source; // The amount of light this tile produces

				rtbInfo.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "special: {0}" + Environment.NewLine, record.TargetType));
//				unsigned char Target_Type; // The special properties of the tile

//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[60]));
//				unsigned char u61;

//				rtb.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[61]));
//				unsigned char u62;
			}
		}
	}
}
