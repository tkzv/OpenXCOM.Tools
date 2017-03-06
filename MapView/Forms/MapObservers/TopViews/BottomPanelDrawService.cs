using System.Collections.Generic;
using System.Drawing;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal class BottomPanelDrawService
	{
		public SolidBrush Brush
		{ get; set; }

		public Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		public Dictionary<string, Pen> Pens
		{ get; set; }

		public Font Font
		{ get; set; }

		private const int tileWidth  = 32;
		private const int tileHeight = 40;
		private const int space = 2;
		public const int TOTAL_QUADRANT_SPACE = tileWidth + space * 2;

		public const int startX = 5;
		public const int startY = 0;

		public void Draw(
						Graphics g,
						XCMapTile mapTile,
						XCMapTile.MapQuadrant selectedQuadrant) // NOTE: These are not "quadrants"; they are tile-part types.
		{
			switch (selectedQuadrant) // Draw selection
			{
				case XCMapTile.MapQuadrant.Ground:
					g.FillRectangle(
								Brush,
								startX,
								startY,
								tileWidth  + 1,
								tileHeight + 2);
					break;

				case XCMapTile.MapQuadrant.West:
					g.FillRectangle(
								Brush,
								startX + TOTAL_QUADRANT_SPACE,
								startY,
								tileWidth  + 1,
								tileHeight + 2);
					break;

				case XCMapTile.MapQuadrant.North:
					g.FillRectangle(
								Brush,
								startX + TOTAL_QUADRANT_SPACE * 2,
								startY,
								tileWidth  + 1,
								tileHeight + 2);
					break;

				case XCMapTile.MapQuadrant.Content:
					g.FillRectangle(
								Brush,
								startX + TOTAL_QUADRANT_SPACE * 3,
								startY,
								tileWidth  + 1,
								tileHeight + 2);
					break;
			}

			var topView = MainWindowsManager.TopView.TopViewControl;

			if (!topView.GroundVisible) // Ground not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX,
							startY,
							tileWidth  + 1,
							tileHeight + 2);

			if (mapTile != null && mapTile.Ground != null)
			{
				g.DrawImage(
							mapTile.Ground[MapViewPanel.Current].Image,
							startX,
							startY - mapTile.Ground.Info.TileOffset);

				if (mapTile.Ground.Info.HumanDoor || mapTile.Ground.Info.UFODoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[3].Image,
							startX,
							startY);

			if (!topView.WestVisible) // Westwall not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX + TOTAL_QUADRANT_SPACE,
							startY,
							tileWidth  + 1,
							tileHeight + 2);

			if (mapTile != null && mapTile.West != null)
			{
				g.DrawImage(
							mapTile.West[MapViewPanel.Current].Image,
							startX + TOTAL_QUADRANT_SPACE,
							startY - mapTile.West.Info.TileOffset);

				if (mapTile.West.Info.HumanDoor || mapTile.West.Info.UFODoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + TOTAL_QUADRANT_SPACE,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[1].Image,
							startX + TOTAL_QUADRANT_SPACE,
							startY);

			if (!topView.NorthVisible) // Northwall not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX + TOTAL_QUADRANT_SPACE * 2,
							startY,
							tileWidth  + 1,
							tileHeight + 2);

			if (mapTile != null && mapTile.North != null)
			{
				g.DrawImage(
							mapTile.North[MapViewPanel.Current].Image,
							startX + TOTAL_QUADRANT_SPACE * 2,
							startY - mapTile.North.Info.TileOffset);

				if (mapTile.North.Info.HumanDoor || mapTile.North.Info.UFODoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + TOTAL_QUADRANT_SPACE * 2,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[2].Image,
							startX + TOTAL_QUADRANT_SPACE * 2,
							startY);

			if (!topView.ContentVisible) // Content not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX + TOTAL_QUADRANT_SPACE * 3,
							startY,
							tileWidth  + 1,
							tileHeight + 2);

			if (mapTile != null && mapTile.Content != null)
			{
				g.DrawImage(
							mapTile.Content[MapViewPanel.Current].Image,
							startX + TOTAL_QUADRANT_SPACE * 3,
							startY - mapTile.Content.Info.TileOffset);

				if (mapTile.Content.Info.HumanDoor || mapTile.Content.Info.UFODoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + TOTAL_QUADRANT_SPACE * 3,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[4].Image,
							startX + TOTAL_QUADRANT_SPACE * 3,
							startY);

			DrawGroundAndContent(g);

			g.DrawString(
					"Floor",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX,
								startY + tileHeight + space,
								tileWidth,
								50));

			g.DrawString(
					"West",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + TOTAL_QUADRANT_SPACE,
								startY + tileHeight + space,
								tileWidth,
								50));

			g.DrawString(
					"North",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + TOTAL_QUADRANT_SPACE * 2,
								startY + tileHeight + space,
								tileWidth,
								50));

			g.DrawString(
					"Object",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + TOTAL_QUADRANT_SPACE * 3,
								startY + tileHeight + space,
								tileWidth + 50,
								50));

			for (int i = 0; i < 4; i++)
				g.DrawRectangle(
							System.Drawing.Pens.Black,
							startX - 1 + TOTAL_QUADRANT_SPACE * i,
							startY,
							tileWidth  + 2,
							tileHeight + 2);
		}

		private void DrawGroundAndContent(Graphics g)
		{
			if (Brushes != null && Pens != null)
			{
				g.FillRectangle(
							Brushes["GroundColor"],
							new RectangleF(
										startX,
										startY + tileHeight + space + Font.Height,
										tileWidth,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["NorthColor"].Color),
							new RectangleF(
										startX + TOTAL_QUADRANT_SPACE,
										startY + tileHeight + space + Font.Height,
										tileWidth,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["WestColor"].Color),
							new RectangleF(
										startX + TOTAL_QUADRANT_SPACE * 2,
										startY + tileHeight + space + Font.Height,
										tileWidth,
										3));

				g.FillRectangle(
							Brushes["ContentColor"],
							new RectangleF(
										startX + TOTAL_QUADRANT_SPACE * 3,
										startY + tileHeight + space + Font.Height,
										tileWidth,
										3));
			}
		}
	}
}
