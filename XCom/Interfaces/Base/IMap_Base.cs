using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using XCom.Services;


namespace XCom.Interfaces.Base
{
	public delegate void HeightChangedDelegate(IMap_Base sender, HeightChangedEventArgs e);
	public delegate void SelectedTileChangedDelegate(IMap_Base sender, SelectedTileChangedEventArgs e);


	/// <summary>
	/// Abstract base class definining all common functionality of an editable Map.
	/// </summary>
	public class IMap_Base
	{
		protected MapTileList MapData;

		private byte _curHeight;
		private MapLocation _selLoc;


		/// <summary>
		/// User is shown the "Do you want to save?" dialog if true.
		/// </summary>
		public bool MapChanged
		{ get; set; }


		protected IMap_Base(string name, List<TileBase> tiles)
		{
			Name = name;
			Tiles = tiles;
		}


		public string Name
		{ get; protected set; }

		public List<TileBase> Tiles
		{ get; protected set; }

		public virtual void Save()
		{
			throw new Exception("IMap_Base: Save() is not implemented."); // ... odd ....
		}

		public event HeightChangedDelegate HeightChanged;
		public event SelectedTileChangedDelegate SelectedTileChanged;

		/// <summary>
		/// Changes the '_curHeight' property and fires a HeightChanged event.
		/// </summary>
		public void Up()
		{
			if (_curHeight > 0)
			{
				var args = new HeightChangedEventArgs(_curHeight, _curHeight - 1);
				--_curHeight;

				if (HeightChanged != null)
					HeightChanged(this, args);
			}
		}

		/// <summary>
		/// Changes the '_curHeight' property and fires a HeightChanged event.
		/// </summary>
		public void Down()
		{
			if (_curHeight < MapSize.Height - 1)
			{
				++_curHeight;
				var args = new HeightChangedEventArgs(_curHeight, _curHeight + 1);

				if (HeightChanged != null)
					HeightChanged(this, args);
			}
		}

		/// <summary>
		/// Gets the current height.
		/// Setting the height will fire a HeightChanged event.
		/// </summary>
		public byte CurrentHeight
		{
			get { return _curHeight; }
			set
			{
				if (value < (byte)MapSize.Height)
				{
					var args = new HeightChangedEventArgs(_curHeight, value);
					_curHeight = value;

					if (HeightChanged != null)
						HeightChanged(this, args);
				}
			}
		}

		/// <summary>
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; protected set; }

		/// <summary>
		/// Gets/Sets the current selected location. Setting the location will fire a SelectedTileChanged event.
		/// </summary>
		public MapLocation SelectedTile
		{
			get { return _selLoc; }
			set
			{
				if (   value.Row > -1 && value.Row < this.MapSize.Rows
					&& value.Col > -1 && value.Col < this.MapSize.Cols)
				{
					_selLoc = value;
					var tile = this[_selLoc.Row, _selLoc.Col];
					var args = new SelectedTileChangedEventArgs(value, tile);

					if (SelectedTileChanged != null)
						SelectedTileChanged(this, args);
				}
			}
		}

		/// <summary>
		/// Gets/Sets a MapTile using row,col,height values. No error checking is done to ensure that the location is valid.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col, int height]
		{
			get
			{
				return (MapData != null) ? MapData[row, col, height]
										 : null;
			}

			set { MapData[row, col, height] = value; }
		}

		/// <summary>
		/// Gets/Sets a MapTile at the current height using row,col values.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col]
		{
			get { return this[row, col, _curHeight]; }
			set { this[row, col, _curHeight] = value; }
		}

		/// <summary>
		/// Gets/Sets a MapTile using a MapLocation.
		/// </summary>
		public MapTileBase this[MapLocation position]
		{
			get { return this[position.Row, position.Col, position.Height]; }
			set { this[position.Row, position.Col, position.Height] = value; }
		}

		public virtual void ResizeTo(
				int newR,
				int newC,
				int newH,
				bool wrtCeiling)
		{
			var newMap = MapResizeService.ResizeMap(
												newR,
												newC,
												newH,
												MapSize,
												MapData,
												wrtCeiling);
			if (newMap != null)
			{
				MapData = newMap;
				MapSize = new MapSize(newR, newC, newH);
				_curHeight = (byte)(MapSize.Height - 1);
				MapChanged = true;
			}
		}

		/// <summary>
		/// Not yet generic enough to call with custom derived classes other than XCMapFile.
		/// </summary>
		/// <param name="file"></param>
		public void SaveGif(string file)
		{
			var palette = GetFirstGroundPalette();
			if (palette == null)
				throw new ApplicationException("IMap_Base: At least 1 ground tile is required");

			var rowPlusCols = MapSize.Rows + MapSize.Cols;
			var b = Bmp.MakeBitmap(
								rowPlusCols * (PckImage.Width / 2),
								(MapSize.Height - _curHeight) * 24 + rowPlusCols * 8,
								palette.Colors);

			var start = new Point(
								(MapSize.Rows - 1) * (PckImage.Width / 2),
								-(24 * _curHeight));

			int curr = 0;

			if (MapData != null)
			{
				var hWid = Globals.HalfWidth;
				var hHeight = Globals.HalfHeight;
				for (int h = MapSize.Height - 1; h >= _curHeight; h--)
				{
					for (int
							row = 0, startX = start.X, startY = start.Y + h * 24;
							row < MapSize.Rows;
							++row, startX -= hWid, startY += hHeight)
					{
						for (int
								col = 0, x = startX, y = startY;
								col < MapSize.Cols;
								++col, x += hWid, y += hHeight, ++curr)
						{
							var tiles = this[row, col, h].UsedTiles;
							foreach (var tileBase in tiles)
							{
								var tile = (XCTile)tileBase;
								Bmp.Draw(tile[0].Image, b, x, y - tile.Info.TileOffset);
							}

							Bmp.FireLoadingEvent(curr, (MapSize.Height - _curHeight) * MapSize.Rows * MapSize.Cols);
						}
					}
				}
			}
			try
			{
				var rect = Bmp.GetBoundsRect(b, Bmp.DefaultTransparentIndex);
				var b2 = Bmp.Crop(b, rect);
				b2.Save(file, ImageFormat.Gif);
			}
			catch
			{
				b.Save(file, ImageFormat.Gif);
			}
		}

		private Palette GetFirstGroundPalette()
		{
			for (int h = 0; h != MapSize.Height; ++h)
				for (int r = 0; r != MapSize.Rows; ++r)
					for (int c = 0; c != MapSize.Cols; ++c)
					{
						var tile = (XCMapTile)this[r, c, h];
						if (tile.Ground != null)
							return tile.Ground[0].Palette;
					}

			return null;
		}
	}
}
