using System;
using System.Collections.Generic;
//using System.Drawing;


namespace XCom
{
	internal static class McdRecordFactory
	{
		internal static McdRecord CreateRecord(IList<byte> info) // question: why is this a List
		{
			var record = new McdRecord();

			record.Image1 = info[0];
			record.Image2 = info[1];
			record.Image3 = info[2];
			record.Image4 = info[3];
			record.Image5 = info[4];
			record.Image6 = info[5];
			record.Image7 = info[6];
			record.Image8 = info[7];

			record.Loft1  = info[8];
			record.Loft2  = info[9];
			record.Loft3  = info[10];
			record.Loft4  = info[11];
			record.Loft5  = info[12];
			record.Loft6  = info[13];
			record.Loft7  = info[14];
			record.Loft8  = info[15];
			record.Loft9  = info[16];
			record.Loft10 = info[17];
			record.Loft11 = info[18];
			record.Loft12 = info[19];

			record.ScanG = (ushort)(info[21] * 255 + info[20]);

			record.Unknown22 = info[22];
			record.Unknown23 = info[23];
			record.Unknown24 = info[24];
			record.Unknown25 = info[25];
			record.Unknown26 = info[26];
			record.Unknown27 = info[27];
			record.Unknown28 = info[28];
			record.Unknown29 = info[29];

			record.UfoDoor    = info[30] == 1;
			record.StopLOS    = info[31] != 1; // unsigned char Stop_LOS;            // You cannot see through this tile.
			record.NoGround   = info[32] == 1; // unsigned char No_Floor;            // If 1, then a non-flying unit can't stand here
			record.BigWall    = info[33] == 1;
			record.GravLift   = info[34] == 1; // unsigned char Gravlift;
			record.HumanDoor  = info[35] == 1;
			record.BlockFire  = info[36] == 1; // unsigned char Block_Fire;          // If 1, fire won't go through the tile
			record.BlockSmoke = info[37] == 1; // unsigned char Block_Smoke;         // If 1, smoke won't go through the tile

			record.Unknown38 = info[38]; // unsigned char u39;

			record.TU_Walk     = info[39];
			record.TU_Fly      = info[40]; // unsigned char TU_Fly;                  // remember, 0xFF means it's impassable!
			record.TU_Slide    = info[41]; // unsigned char TU_Slide;                // sliding things include snakemen and silacoids
			record.Armor       = info[42];
			record.HE_Block    = info[43]; // unsigned char HE_Block;                // How much of an explosion this tile will block
			record.DieTile     = info[44];
			record.Flammable   = info[45];
			record.Alt_MCD     = info[46];
			record.Unknown47   = info[47]; // unsigned char u48;
			record.StandOffset = (sbyte)info[48];
			record.TileOffset  = (sbyte)info[49];
			record.Unknown50   = info[50];        // unsigned char u51;
			record.LightBlock  = (sbyte)info[51]; // unsigned char Light_Block;      // The amount of light it blocks, from 0 to 10
			record.Footstep    = (sbyte)info[52];

			record.TileType      = (TileType)info[53];
			record.HE_Type       = (sbyte)info[54]; // unsigned char HE_Type;        // 0=HE 1=Smoke
			record.HE_Strength   = (sbyte)info[55];
			record.SmokeBlockage = (sbyte)info[56]; // unsigned char Smoke_Blockage; // ? Not sure about this
			record.Fuel          = (sbyte)info[57];
			record.LightSource   = (sbyte)info[58]; // unsigned char Light_Source;   // The amount of light this tile produces
			record.TargetType    = (SpecialType)(sbyte)info[59];
			record.Unknown60     = info[60];        // unsigned char u61;
			record.Unknown61     = info[61];        // unsigned char u62;

			record.ScanGReference = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"scang reference: {0} {1} -> {2}" + Environment.NewLine,
											info[20],
											info[21],
											info[20] * 256 + info[21]);

			record.LoftReference = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"loft references: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}" + Environment.NewLine,
											info[8],
											info[9],
											info[10],
											info[11],
											info[12],
											info[13],
											info[14],
											info[15],
											info[16],
											info[17],
											info[18],
											info[19]);

			record.Reference0To30 = string.Empty;
			for (int i = 0; i != 30; ++i)
				record.Reference0To30 += info[i] + " ";
			
			record.Reference30To62 = string.Empty;
			for (int i = 30; i != 62; ++i)
				record.Reference30To62 += info[i] + " ";

//			record.Bounds = new Rectangle(
//										0,
//										record.TileOffset,
//										PckImage.Width,
//										PckImage.Height - record.TileOffset);
//			record.Width  = PckImage.Width;
//			record.Height = PckImage.Height - record.TileOffset;

			return record;
		}
	}
}
