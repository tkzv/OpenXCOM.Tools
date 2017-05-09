using System.Collections.Generic;
using System.Drawing;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed class QuadrantPanelDrawService
	{
		#region Fields
		private const int QuadWidth  = 32;
		private const int QuadHeight = 40;

		private const int MarginHori = 5;
		private const int MarginVert = 3;

		internal const int QuadWidthTotal = QuadWidth + MarginHori * 2;

		internal const int StartX = 26;
		private  const int StartY = 2;

		// NOTE: keep the door-string and its placement consistent with
		// TilePanel.OnPaint().
		private const string Door = "door";
		private static int DoorWidth;
		private const int PrintOffsetY = 2;

		private const string Floor   = "fLoOr";
		private const string West    = "WEst";
		private const string North   = "noRtH";
		private const string Content = "ConTeNt";

		private static int FloorWidth;
		private static int WestWidth;
		private static int NorthWidth;
		private static int ContentWidth;

		private static bool Inited;
		#endregion


		#region Properties
		internal SolidBrush Brush
		{ get; set; }

		internal Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		internal Dictionary<string, Pen> Pens
		{ get; set; }

		internal Font Font
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal QuadrantPanelDrawService()
		{
			Brush = new SolidBrush(Color.LightBlue);
			Font  = new Font("Comic Sans MS", 7);
		}
		#endregion


		#region Methods
		internal void Draw(
				Graphics g,
				XCMapTile mapTile,
				QuadrantType selectedQuadrant)
		{
			if (!Inited)
			{
				Inited = true;

				DoorWidth    = (int)g.MeasureString(Door,    Font).Width;

				FloorWidth   = (int)g.MeasureString(Floor,   Font).Width;
				WestWidth    = (int)g.MeasureString(West,    Font).Width;
				NorthWidth   = (int)g.MeasureString(North,   Font).Width;
				ContentWidth = (int)g.MeasureString(Content, Font).Width;
			}

			// fill the background of the selected quadrant type
			// NOTE: the selected quadrant will be re-filled with DarkGray
			// if user has toggled that quadrant's visibility off.
			switch (selectedQuadrant)
			{
				case QuadrantType.Ground:
					g.FillRectangle(
								Brush,
								StartX,
								StartY,
								QuadWidth  + 1,
								QuadHeight + 2);
					break;

				case QuadrantType.West:
					g.FillRectangle(
								Brush,
								StartX + QuadWidthTotal,
								StartY,
								QuadWidth  + 1,
								QuadHeight + 2);
					break;

				case QuadrantType.North:
					g.FillRectangle(
								Brush,
								StartX + QuadWidthTotal * 2,
								StartY,
								QuadWidth  + 1,
								QuadHeight + 2);
					break;

				case QuadrantType.Content:
					g.FillRectangle(
								Brush,
								StartX + QuadWidthTotal * 3,
								StartY,
								QuadWidth  + 1,
								QuadHeight + 2);
					break;
			}


			// draw the Sprites
			var topView = ViewerFormsManager.TopView.Control;

			if (!topView.GroundVisible) // Ground
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							StartX,
							StartY,
							QuadWidth  + 1,
							QuadHeight + 2);

			if (mapTile != null && mapTile.Ground != null)
			{
				g.DrawImage(
							mapTile.Ground[MainViewUnderlay.AniStep].Sprite,
							StartX,
							StartY - mapTile.Ground.Record.TileOffset);

				if (mapTile.Ground.Record.HumanDoor || mapTile.Ground.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - DoorWidth) / 2,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[3].Sprite,
							StartX,
							StartY);


			if (!topView.WestVisible) // Westwall
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							StartX + QuadWidthTotal,
							StartY,
							QuadWidth  + 1,
							QuadHeight + 2);

			if (mapTile != null && mapTile.West != null)
			{
				g.DrawImage(
							mapTile.West[MainViewUnderlay.AniStep].Sprite,
							StartX + QuadWidthTotal,
							StartY - mapTile.West.Record.TileOffset);

				if (mapTile.West.Record.HumanDoor || mapTile.West.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - DoorWidth) / 2 + QuadWidthTotal,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[1].Sprite,
							StartX + QuadWidthTotal,
							StartY);


			if (!topView.NorthVisible) // Northwall
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							StartX + QuadWidthTotal * 2,
							StartY,
							QuadWidth  + 1,
							QuadHeight + 2);

			if (mapTile != null && mapTile.North != null)
			{
				g.DrawImage(
							mapTile.North[MainViewUnderlay.AniStep].Sprite,
							StartX + QuadWidthTotal * 2,
							StartY - mapTile.North.Record.TileOffset);

				if (mapTile.North.Record.HumanDoor || mapTile.North.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - DoorWidth) / 2 + QuadWidthTotal * 2,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[2].Sprite,
							StartX + QuadWidthTotal * 2,
							StartY);


			if (!topView.ContentVisible) // Content
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							StartX + QuadWidthTotal * 3,
							StartY,
							QuadWidth  + 1,
							QuadHeight + 2);

			if (mapTile != null && mapTile.Content != null)
			{
				g.DrawImage(
							mapTile.Content[MainViewUnderlay.AniStep].Sprite,
							StartX + QuadWidthTotal * 3,
							StartY - mapTile.Content.Record.TileOffset);

				if (mapTile.Content.Record.HumanDoor || mapTile.Content.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - DoorWidth) / 2 + QuadWidthTotal * 3,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[4].Sprite,
							StartX + QuadWidthTotal * 3,
							StartY);


			// draw each quadrant's bounding rectangle
			for (int i = 0; i != 4; ++i)
				g.DrawRectangle(
							System.Drawing.Pens.Black,
							StartX - 1 + QuadWidthTotal * i,
							StartY - 1,
							QuadWidth  + 2,
							QuadHeight + 2);


			// draw the label under each quadrant
			g.DrawString(
					Floor,
					Font,
					System.Drawing.Brushes.Black,
					StartX + (QuadWidth  - FloorWidth) / 2,
					StartY +  QuadHeight + MarginVert);

			g.DrawString(
					West,
					Font,
					System.Drawing.Brushes.Black,
					StartX + (QuadWidth  - WestWidth) / 2 + QuadWidthTotal,
					StartY +  QuadHeight + MarginVert);

			g.DrawString(
					North,
					Font,
					System.Drawing.Brushes.Black,
					StartX + (QuadWidth  - NorthWidth) / 2 + QuadWidthTotal * 2,
					StartY +  QuadHeight + MarginVert);

			g.DrawString(
					Content,
					Font,
					System.Drawing.Brushes.Black,
					StartX + (QuadWidth  - ContentWidth) / 2 + QuadWidthTotal * 3,
					StartY +  QuadHeight + MarginVert);


			// fill the color-tip under each quadrant
			if (Brushes != null && Pens != null)
			{
				g.FillRectangle(
							Brushes[TopView.FloorColor],
							new RectangleF(
										StartX,
										StartY + QuadHeight + MarginVert + Font.Height + 1,
										QuadWidth + 1,
										5));

				g.FillRectangle(
							new SolidBrush(Pens[TopView.WestColor].Color),
							new RectangleF(
										StartX + QuadWidthTotal,
										StartY + QuadHeight + MarginVert + Font.Height + 1,
										QuadWidth + 1,
										5));

				g.FillRectangle(
							new SolidBrush(Pens[TopView.NorthColor].Color),
							new RectangleF(
										StartX + QuadWidthTotal * 2,
										StartY + QuadHeight + MarginVert + Font.Height + 1,
										QuadWidth + 1,
										5));

				g.FillRectangle(
							Brushes[TopView.ContentColor],
							new RectangleF(
										StartX + QuadWidthTotal * 3,
										StartY + QuadHeight + MarginVert + Font.Height + 1,
										QuadWidth + 1,
										5));
			}
		}
		#endregion
	}
}
