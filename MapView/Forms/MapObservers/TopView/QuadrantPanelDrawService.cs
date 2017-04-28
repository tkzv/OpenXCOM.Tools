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

		private const int Margin    =  2;

		internal const int QuadWidthTotal = QuadWidth + Margin * 2;

		internal const int StartX = 5;
		private  const int StartY = 0;

		private const string Door = "door";
		private const int PrintOffsetY = 2;
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


		#region Methods
		internal void Draw(
				Graphics g,
				XCMapTile mapTile,
				QuadrantType selectedQuadrant)
		{
			// NOTE: keep the door-string and its placement consistent with
			// TilePanel.OnPaint().
			int textWidth = (int)g.MeasureString(Door, Font).Width;

			switch (selectedQuadrant) // Fill background of selected quadrant type.
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
							mapTile.Ground[MainViewUnderlay.AniStep].Image,
							StartX,
							StartY - mapTile.Ground.Record.TileOffset);

				if (mapTile.Ground.Record.HumanDoor || mapTile.Ground.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - textWidth) / 2,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[3].Image,
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
							mapTile.West[MainViewUnderlay.AniStep].Image,
							StartX + QuadWidthTotal,
							StartY - mapTile.West.Record.TileOffset);

				if (mapTile.West.Record.HumanDoor || mapTile.West.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - textWidth) / 2 + QuadWidthTotal,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[1].Image,
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
							mapTile.North[MainViewUnderlay.AniStep].Image,
							StartX + QuadWidthTotal * 2,
							StartY - mapTile.North.Record.TileOffset);

				if (mapTile.North.Record.HumanDoor || mapTile.North.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - textWidth) / 2 + QuadWidthTotal * 2,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[2].Image,
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
							mapTile.Content[MainViewUnderlay.AniStep].Image,
							StartX + QuadWidthTotal * 3,
							StartY - mapTile.Content.Record.TileOffset);

				if (mapTile.Content.Record.HumanDoor || mapTile.Content.Record.UfoDoor)
					g.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (QuadWidth  - textWidth) / 2 + QuadWidthTotal * 3,
							StartY +  QuadHeight - Font.Height + PrintOffsetY); // PckImage.Height
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[4].Image,
							StartX + QuadWidthTotal * 3,
							StartY);


			DrawGroundAndContent(g);

			g.DrawString(
					"Floor",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								StartX,
								StartY + QuadHeight + Margin,
								QuadWidth,
								50));

			g.DrawString(
					"West",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								StartX + QuadWidthTotal,
								StartY + QuadHeight + Margin,
								QuadWidth,
								50));

			g.DrawString(
					"North",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								StartX + QuadWidthTotal * 2,
								StartY + QuadHeight + Margin,
								QuadWidth,
								50));

			g.DrawString(
					"Content",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								StartX + QuadWidthTotal * 3,
								StartY + QuadHeight + Margin,
								QuadWidth + 50,
								50));


			for (int i = 0; i != 4; ++i)
				g.DrawRectangle(
							System.Drawing.Pens.Black,
							StartX - 1 + QuadWidthTotal * i,
							StartY,
							QuadWidth  + 2,
							QuadHeight + 2);
		}

		private void DrawGroundAndContent(Graphics g)
		{
			if (Brushes != null && Pens != null)
			{
				g.FillRectangle(
							Brushes["GroundColor"],
							new RectangleF(
										StartX,
										StartY + QuadHeight + Margin + Font.Height,
										QuadWidth,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["NorthColor"].Color),
							new RectangleF(
										StartX + QuadWidthTotal,
										StartY + QuadHeight + Margin + Font.Height,
										QuadWidth,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["WestColor"].Color),
							new RectangleF(
										StartX + QuadWidthTotal * 2,
										StartY + QuadHeight + Margin + Font.Height,
										QuadWidth,
										3));

				g.FillRectangle(
							Brushes["ContentColor"],
							new RectangleF(
										StartX + QuadWidthTotal * 3,
										StartY + QuadHeight + Margin + Font.Height,
										QuadWidth,
										3));
			}
		}
		#endregion
	}
}
