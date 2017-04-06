using System.Collections.Generic;
using System.Drawing;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal class QuadrantPanelDrawService
	{
		public SolidBrush Brush
		{ get; set; }

		public Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		public Dictionary<string, Pen> Pens
		{ get; set; }

		public Font Font
		{ get; set; }

		private const int _width  = 32;
		private const int _height = 40;

		private const int _pad    =  2;

		public const int QuadsWidthTotal = _width + _pad * 2;

		public const int startX = 5;
		public const int startY = 0;

		public void Draw(
				Graphics g,
				XCMapTile mapTile,
				QuadrantType selectedQuadrant)
		{
			switch (selectedQuadrant) // Draw selection
			{
				case QuadrantType.Ground:
					g.FillRectangle(
								Brush,
								startX,
								startY,
								_width  + 1,
								_height + 2);
					break;

				case QuadrantType.West:
					g.FillRectangle(
								Brush,
								startX + QuadsWidthTotal,
								startY,
								_width  + 1,
								_height + 2);
					break;

				case QuadrantType.North:
					g.FillRectangle(
								Brush,
								startX + QuadsWidthTotal * 2,
								startY,
								_width  + 1,
								_height + 2);
					break;

				case QuadrantType.Content:
					g.FillRectangle(
								Brush,
								startX + QuadsWidthTotal * 3,
								startY,
								_width  + 1,
								_height + 2);
					break;
			}

			var topView = MainWindowsManager.TopView.Control;

			if (!topView.GroundVisible) // Ground not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX,
							startY,
							_width  + 1,
							_height + 2);

			if (mapTile != null && mapTile.Ground != null)
			{
				g.DrawImage(
							mapTile.Ground[MainViewPanel.Current].Image,
							startX,
							startY - mapTile.Ground.Info.TileOffset);

				if (mapTile.Ground.Info.HumanDoor || mapTile.Ground.Info.UfoDoor)
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
							startX + QuadsWidthTotal,
							startY,
							_width  + 1,
							_height + 2);

			if (mapTile != null && mapTile.West != null)
			{
				g.DrawImage(
							mapTile.West[MainViewPanel.Current].Image,
							startX + QuadsWidthTotal,
							startY - mapTile.West.Info.TileOffset);

				if (mapTile.West.Info.HumanDoor || mapTile.West.Info.UfoDoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + QuadsWidthTotal,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[1].Image,
							startX + QuadsWidthTotal,
							startY);

			if (!topView.NorthVisible) // Northwall not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX + QuadsWidthTotal * 2,
							startY,
							_width  + 1,
							_height + 2);

			if (mapTile != null && mapTile.North != null)
			{
				g.DrawImage(
							mapTile.North[MainViewPanel.Current].Image,
							startX + QuadsWidthTotal * 2,
							startY - mapTile.North.Info.TileOffset);

				if (mapTile.North.Info.HumanDoor || mapTile.North.Info.UfoDoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + QuadsWidthTotal * 2,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[2].Image,
							startX + QuadsWidthTotal * 2,
							startY);

			if (!topView.ContentVisible) // Content not visible
				g.FillRectangle(
							System.Drawing.Brushes.DarkGray,
							startX + QuadsWidthTotal * 3,
							startY,
							_width  + 1,
							_height + 2);

			if (mapTile != null && mapTile.Content != null)
			{
				g.DrawImage(
							mapTile.Content[MainViewPanel.Current].Image,
							startX + QuadsWidthTotal * 3,
							startY - mapTile.Content.Info.TileOffset);

				if (mapTile.Content.Info.HumanDoor || mapTile.Content.Info.UfoDoor)
					g.DrawString(
							"Door",
							Font,
							System.Drawing.Brushes.Black,
							startX + QuadsWidthTotal * 3,
							startY + PckImage.Height - Font.Height);
			}
			else
				g.DrawImage(
							Globals.ExtraTiles[4].Image,
							startX + QuadsWidthTotal * 3,
							startY);

			DrawGroundAndContent(g);

			g.DrawString(
					"Floor",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX,
								startY + _height + _pad,
								_width,
								50));

			g.DrawString(
					"West",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + QuadsWidthTotal,
								startY + _height + _pad,
								_width,
								50));

			g.DrawString(
					"North",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + QuadsWidthTotal * 2,
								startY + _height + _pad,
								_width,
								50));

			g.DrawString(
					"Object",
					Font,
					System.Drawing.Brushes.Black,
					new RectangleF(
								startX + QuadsWidthTotal * 3,
								startY + _height + _pad,
								_width + 50,
								50));

			for (int i = 0; i < 4; i++)
				g.DrawRectangle(
							System.Drawing.Pens.Black,
							startX - 1 + QuadsWidthTotal * i,
							startY,
							_width  + 2,
							_height + 2);
		}

		private void DrawGroundAndContent(Graphics g)
		{
			if (Brushes != null && Pens != null)
			{
				g.FillRectangle(
							Brushes["GroundColor"],
							new RectangleF(
										startX,
										startY + _height + _pad + Font.Height,
										_width,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["NorthColor"].Color),
							new RectangleF(
										startX + QuadsWidthTotal,
										startY + _height + _pad + Font.Height,
										_width,
										3));

				g.FillRectangle(
							new SolidBrush(Pens["WestColor"].Color),
							new RectangleF(
										startX + QuadsWidthTotal * 2,
										startY + _height + _pad + Font.Height,
										_width,
										3));

				g.FillRectangle(
							Brushes["ContentColor"],
							new RectangleF(
										startX + QuadsWidthTotal * 3,
										startY + _height + _pad + Font.Height,
										_width,
										3));
			}
		}
	}
}
