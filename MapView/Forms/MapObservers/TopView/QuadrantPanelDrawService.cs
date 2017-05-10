using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed class QuadrantPanelDrawService
	{
		#region Fields
		private const int SpriteWidth  = 32; // PckImage.Width
		private const int SpriteHeight = 40; // PckImage.Height

		private const int MarginHori = 5;
		private const int MarginVert = 3;

		internal const int QuadWidthTotal = SpriteWidth + MarginHori * 2;

		internal const int StartX = 26;
		private  const int StartY =  3;

		private const int SwatchHeight = 5;

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

		private readonly GraphicsPath _pathFloor   = new GraphicsPath();
		private readonly GraphicsPath _pathWest    = new GraphicsPath();
		private readonly GraphicsPath _pathNorth   = new GraphicsPath();
		private readonly GraphicsPath _pathContent = new GraphicsPath();


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal QuadrantPanelDrawService()
		{
			Font  = new Font("Comic Sans MS", 7);
			Brush = new SolidBrush(Color.LightBlue);

			// cache each quadrant's rectangular bounding path
			for (int i = 0; i != 4; ++i)
			{
				var path = new GraphicsPath();

				path = new System.Drawing.Drawing2D.GraphicsPath();
				var p0 = new Point(
								StartX + QuadWidthTotal * i - 1,
								StartY);
				var p1 = new Point(
								StartX + QuadWidthTotal * i + SpriteWidth + 1,
								StartY);
				var p2 = new Point(
								StartX + QuadWidthTotal * i + SpriteWidth + 1,
								StartY + SpriteHeight + 1);
				var p3 = new Point(
								StartX + QuadWidthTotal * i,
								StartY + SpriteHeight + 1);
				var p4 = new Point(
								StartX + QuadWidthTotal * i,
								StartY);

				path.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
				path.AddLine(p1, p2); // does not get drawn.
				path.AddLine(p2, p3);
				path.AddLine(p3, p4); // NOTE: try DrawRectangle() it's even worse.

				switch (i)
				{
					case 0: _pathFloor   = path; break;
					case 1: _pathWest    = path; break;
					case 2: _pathNorth   = path; break;
					case 3: _pathContent = path; break;
				}
			}


		}
		#endregion


		#region Methods
		internal void Draw(
				Graphics graphics,
				XCMapTile tile,
				QuadrantType selectedQuadrant)
		{
			if (!Inited)
			{
				Inited = true;

				DoorWidth    = (int)graphics.MeasureString(Door,    Font).Width;
				FloorWidth   = (int)graphics.MeasureString(Floor,   Font).Width;
				WestWidth    = (int)graphics.MeasureString(West,    Font).Width;
				NorthWidth   = (int)graphics.MeasureString(North,   Font).Width;
				ContentWidth = (int)graphics.MeasureString(Content, Font).Width;
			}

			// fill the background of the selected quadrant type
			// NOTE: the selected quadrant will be re-filled with DarkGray
			// if that quadrant's visibility has been toggled off.
			switch (selectedQuadrant)
			{
				case QuadrantType.Ground:
					graphics.FillPath(Brush, _pathFloor);
					break;

				case QuadrantType.West:
					graphics.FillPath(Brush, _pathWest);
					break;

				case QuadrantType.North:
					graphics.FillPath(Brush, _pathNorth);
					break;

				case QuadrantType.Content:
					graphics.FillPath(Brush, _pathContent);
					break;
			}


			// draw the Sprites
			var topView = ViewerFormsManager.TopView.Control;

			// Ground ->
			if (!topView.GroundVisible)
				graphics.FillPath(System.Drawing.Brushes.DarkGray, _pathFloor);

			if (tile != null && tile.Ground != null)
			{
				graphics.DrawImage(
								tile.Ground[MainViewUnderlay.AniStep].Sprite,
								StartX,
								StartY - tile.Ground.Record.TileOffset);

				if (tile.Ground.Record.HumanDoor || tile.Ground.Record.UfoDoor)
					DrawDoorString(graphics, QuadrantType.Ground);
			}
			else
				graphics.DrawImage(
								Globals.ExtraTiles[3].Sprite,
								StartX,
								StartY);


			// Westwall ->
			if (!topView.WestVisible)
				graphics.FillPath(System.Drawing.Brushes.DarkGray, _pathWest);

			if (tile != null && tile.West != null)
			{
				graphics.DrawImage(
								tile.West[MainViewUnderlay.AniStep].Sprite,
								StartX + QuadWidthTotal,
								StartY - tile.West.Record.TileOffset);

				if (tile.West.Record.HumanDoor || tile.West.Record.UfoDoor)
					DrawDoorString(graphics, QuadrantType.West);
			}
			else
				graphics.DrawImage(
								Globals.ExtraTiles[1].Sprite,
								StartX + QuadWidthTotal,
								StartY);


			// Northwall ->
			if (!topView.NorthVisible)
				graphics.FillPath(System.Drawing.Brushes.DarkGray, _pathNorth);

			if (tile != null && tile.North != null)
			{
				graphics.DrawImage(
								tile.North[MainViewUnderlay.AniStep].Sprite,
								StartX + QuadWidthTotal * 2,
								StartY - tile.North.Record.TileOffset);

				if (tile.North.Record.HumanDoor || tile.North.Record.UfoDoor)
					DrawDoorString(graphics, QuadrantType.North);
			}
			else
				graphics.DrawImage(
								Globals.ExtraTiles[2].Sprite,
								StartX + QuadWidthTotal * 2,
								StartY);


			// Content ->
			if (!topView.ContentVisible)
				graphics.FillPath(System.Drawing.Brushes.DarkGray, _pathContent);

			if (tile != null && tile.Content != null)
			{
				graphics.DrawImage(
								tile.Content[MainViewUnderlay.AniStep].Sprite,
								StartX + QuadWidthTotal * 3,
								StartY - tile.Content.Record.TileOffset);

				if (tile.Content.Record.HumanDoor || tile.Content.Record.UfoDoor)
					DrawDoorString(graphics, QuadrantType.Content);
			}
			else
				graphics.DrawImage(
								Globals.ExtraTiles[4].Sprite,
								StartX + QuadWidthTotal * 3,
								StartY);


			// draw each quadrant's bounding rectangle
			graphics.DrawPath(System.Drawing.Pens.Black, _pathFloor);
			graphics.DrawPath(System.Drawing.Pens.Black, _pathWest);
			graphics.DrawPath(System.Drawing.Pens.Black, _pathNorth);
			graphics.DrawPath(System.Drawing.Pens.Black, _pathContent);


			// draw the quad-type label under each quadrant
			DrawTypeString(graphics, QuadrantType.Ground);
			DrawTypeString(graphics, QuadrantType.West);
			DrawTypeString(graphics, QuadrantType.North);
			DrawTypeString(graphics, QuadrantType.Content);


			// fill the color-swatch under each quadrant-label
			if (Brushes != null && Pens != null)
			{
				FillSwatchColor(graphics, QuadrantType.Ground);
				FillSwatchColor(graphics, QuadrantType.West);
				FillSwatchColor(graphics, QuadrantType.North);
				FillSwatchColor(graphics, QuadrantType.Content);
			}
		}

		/// <summary>
		/// Draws the "door" string if applicable.
		/// </summary>
		private void DrawDoorString(Graphics graphics, QuadrantType quadType)
		{
			graphics.DrawString(
							Door,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (SpriteWidth  - DoorWidth) / 2 + QuadWidthTotal * (int)quadType + 1,
							StartY +  SpriteHeight - Font.Height + PrintOffsetY);
		}

		/// <summary>
		/// Draws the type of quadrant under its rectangle.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="quadType"></param>
		private void DrawTypeString(Graphics graphics, QuadrantType quadType)
		{
			string type = String.Empty;
			int width = 0;

			switch (quadType)
			{
				case QuadrantType.Ground:
					type  = Floor;
					width = FloorWidth;
					break;
				case QuadrantType.West:
					type  = West;
					width = WestWidth;
					break;
				case QuadrantType.North:
					type  = North;
					width = NorthWidth;
					break;
				case QuadrantType.Content:
					type  = Content;
					width = ContentWidth;
					break;
			}

			graphics.DrawString(
							type,
							Font,
							System.Drawing.Brushes.Black,
							StartX + (SpriteWidth  - width) / 2 + QuadWidthTotal * (int)quadType + 1,
							StartY +  SpriteHeight + MarginVert);
		}

		/// <summary>
		/// Fills the swatch under a given quadrant.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="quadType"></param>
		private void FillSwatchColor(Graphics graphics, QuadrantType quadType)
		{
			SolidBrush brush = null;
			switch (quadType)
			{
				case QuadrantType.Ground:
					brush = Brushes[TopView.FloorColor];
					break;
				case QuadrantType.West:
					brush = new SolidBrush(Pens[TopView.WestColor].Color);
					break;
				case QuadrantType.North:
					brush = new SolidBrush(Pens[TopView.NorthColor].Color);
					break;
				case QuadrantType.Content:
					brush = Brushes[TopView.ContentColor];
					break;
			}

			graphics.FillRectangle(
								brush,
								new RectangleF(
											StartX + QuadWidthTotal * (int)quadType,
											StartY + SpriteHeight + MarginVert + Font.Height + 1,
											SpriteWidth,
											SwatchHeight));
		}
		#endregion
	}
}
