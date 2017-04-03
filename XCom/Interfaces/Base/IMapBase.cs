using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using XCom.Services;


namespace XCom.Interfaces.Base
{
	public delegate void HeightChangedEventHandler(IMapBase sender, HeightChangedEventArgs e);

	public delegate void SelectedTileChangedEventHandler(IMapBase sender, SelectedTileChangedEventArgs e);


	/// <summary>
	/// Abstract base class definining all common functionality of an editable Map.
	/// </summary>
	public class IMapBase // psst. This isn't an interface.
	{
		public event HeightChangedEventHandler HeightChanged;
		public event SelectedTileChangedEventHandler SelectedTileChanged;

		private const int HalfWidth  = 16;
		private const int HalfHeight =  8;

		private byte _height;
		/// <summary>
		/// Gets the current height.
		/// Setting the height will fire a HeightChanged event.
		/// </summary>
		public byte CurrentHeight
		{
			get { return _height; }
			set
			{
				if (value < (byte)MapSize.Height)
				{
//					var args = new HeightChangedEventArgs(_height, value);
					var args = new HeightChangedEventArgs(value);
					_height = value;

					if (HeightChanged != null)
						HeightChanged(this, args);
				}
			}
		}

		/// <summary>
		/// User is shown the "Do you want to save?" dialog if true.
		/// </summary>
		public bool MapChanged
		{ get; set; }

		public string Name
		{ get; protected set; }

		protected MapTileList MapTiles
		{ get; set; }

		private readonly List<TileBase> _tiles;
		public List<TileBase> Tiles
		{
			get { return _tiles; }
		}

		private MapLocation _selLoc;
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
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; protected set; }

		/// <summary>
		/// Gets/Sets a MapTile using row,col,height values. No error checking
		/// is done to ensure that the location is valid.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col, int height]
		{
			get
			{
				return (MapTiles != null) ? MapTiles[row, col, height]
										  : null;
			}
			set { MapTiles[row, col, height] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile at the current height using row,col values.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col]
		{
			get { return this[row, col, _height]; }
			set { this[row, col, _height] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile using a MapLocation.
		/// </summary>
		public MapTileBase this[MapLocation loc]
		{
			get { return this[loc.Row, loc.Col, loc.Height]; }
			set { this[loc.Row, loc.Col, loc.Height] = value; }
		}


		protected IMapBase(string name, List<TileBase> tiles)
		{
			Name = name;
			_tiles = tiles;
		}


		public virtual void Save()
		{
			throw new InvalidOperationException("IMapBase: Save() is not implemented."); // ... odd ....
		}

		/// <summary>
		/// Changes the '_height' property and fires a HeightChanged event.
		/// </summary>
		public void Up()
		{
			if (_height > 0)
			{
//				var args = new HeightChangedEventArgs(_height, _height - 1);
				var args = new HeightChangedEventArgs(_height - 1);
				--_height;

				if (HeightChanged != null)
					HeightChanged(this, args);
			}
		}

		/// <summary>
		/// Changes the '_height' property and fires a HeightChanged event.
		/// </summary>
		public void Down()
		{
			if (_height < MapSize.Height - 1)
			{
				++_height; // TODO: wait a second !
				var args = new HeightChangedEventArgs(_height + 1);
//				var args = new HeightChangedEventArgs(_height, _height + 1);

				if (HeightChanged != null)
					HeightChanged(this, args);
			}
		}

		public virtual void ResizeTo(
				int rPost,
				int cPost,
				int hPost,
				bool toCeiling)
		{
			var tileList = MapResizeService.ResizeMap(
												rPost,
												cPost,
												hPost,
												MapSize,
												MapTiles,
												toCeiling);
			if (tileList != null)
			{
				// NOTE: This doesn't handle Routes or node-checking which XCMapFile.ResizeTo() does.
				MapTiles = tileList;
				MapSize  = new MapSize(rPost, cPost, hPost);

				if (hPost > 0) // assuage FxCop re 'possible' underflow.
					_height = (byte)(hPost - 1);

				MapChanged = true;
			}
		}

		/// <summary>
		/// Not yet generic enough to call with custom derived classes other
		/// than XCMapFile.
		/// </summary>
		/// <param name="file"></param>
		public void SaveGif(string file)
		{
			var palette = GetFirstGroundPalette();
			if (palette == null)
				throw new ArgumentNullException("file", "IMapBase: At least 1 ground tile is required.");

			var rowPlusCols = MapSize.Rows + MapSize.Cols;
			var b = Bmp.MakeBitmap(
								rowPlusCols * (PckImage.Width / 2),
								(MapSize.Height - _height) * 24 + rowPlusCols * 8,
								palette.Colors);

			var start = new Point(
								(MapSize.Rows - 1) * (PckImage.Width / 2),
								-(24 * _height));

			int i = 0;
			if (MapTiles != null)
			{
				for (int h = MapSize.Height - 1; h >= _height; --h)
				{
					for (int
							r = 0, startX = start.X, startY = start.Y + h * 24;
							r != MapSize.Rows;
							++r, startX -= HalfWidth, startY += HalfHeight)
					{
						for (int
								c = 0, x = startX, y = startY;
								c != MapSize.Cols;
								++c, x += HalfWidth, y += HalfHeight, ++i)
						{
							var tiles = this[r, c, h].UsedTiles;
							foreach (var tileBase in tiles)
							{
								var tile = (XCTile)tileBase;
								Bmp.Draw(tile[0].Image, b, x, y - tile.Info.TileOffset);
							}

							Bmp.FireLoadingEvent(i, (MapSize.Height - _height) * MapSize.Rows * MapSize.Cols);
						}
					}
				}
			}
			try
			{
				var rect = Bmp.GetBoundsRect(b, Palette.TransparentId);
				b = Bmp.Crop(b, rect);
				b.Save(file, ImageFormat.Gif);
			}
			catch
			{
				b.Save(file, ImageFormat.Gif);
				throw;
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
