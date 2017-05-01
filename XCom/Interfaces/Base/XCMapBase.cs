using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

//using XCom.Services;


namespace XCom.Interfaces.Base
{
	public delegate void LevelChangedEventHandler(XCMapBase sender, LevelChangedEventArgs e);

	public delegate void LocationChangedEventHandler(XCMapBase sender, LocationChangedEventArgs e);


	/// <summary>
	/// This is basically the currently loaded Map.
	/// </summary>
	public class XCMapBase
	{
		public event LevelChangedEventHandler LevelChangedEvent;
		public event LocationChangedEventHandler LocationChangedEvent;


		private const int HalfWidth  = 16;
		private const int HalfHeight =  8;

		private int _level;
		/// <summary>
		/// Gets this MapBase's currently displayed level.
		/// Changing level will fire a LevelChanged event.
		/// </summary>
		public int Level
		{
			get { return _level; }
			set
			{
				if (value < MapSize.Levs)
				{
					_level = value;

					if (LevelChangedEvent != null)
						LevelChangedEvent(this, new LevelChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// User is shown the "Do you want to save?" dialog if true.
		/// </summary>
		public bool MapChanged
		{ get; set; }

		public string Label
		{ get; private set; }

		internal MapTileList MapTiles
		{ get; set; }

		private readonly List<TileBase> _tiles;
		public List<TileBase> Tiles
		{
			get { return _tiles; }
		}

		private MapLocation _location;
		/// <summary>
		/// Gets/Sets the current selected location. Setting the location will
		/// fire a LocationChanged event.
		/// </summary>
		public MapLocation Location
		{
			get { return _location; }
			set
			{
				if (   value.Row > -1 && value.Row < this.MapSize.Rows
					&& value.Col > -1 && value.Col < this.MapSize.Cols)
				{
					_location = value;
					var tile = this[_location.Row, _location.Col];
					var args = new LocationChangedEventArgs(value, tile);

					if (LocationChangedEvent != null)
						LocationChangedEvent(this, args);
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
		/// <param name="lev"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col, int lev]
		{
			get { return (MapTiles != null) ? MapTiles[row, col, lev]
											: null; }
			set { MapTiles[row, col, lev] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile at the current height using row,col values.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col]
		{
			get { return this[row, col, _level]; }
			set { this[row, col, _level] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile using a MapLocation.
		/// </summary>
		public MapTileBase this[MapLocation loc]
		{
			get { return this[loc.Row, loc.Col, loc.Lev]; }
			set { this[loc.Row, loc.Col, loc.Lev] = value; }
		}


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="tiles"></param>
		protected XCMapBase(string label, List<TileBase> tiles)
		{
			Label = label;
			_tiles = tiles;
		}
		#endregion


		public virtual void Save()
		{
//			throw new InvalidOperationException("XCMapBase: Save() is not implemented."); // ... odd ....
		}

		/// <summary>
		/// Changes the '_level' property and fires a LevelChanged event.
		/// </summary>
		public void Up()
		{
			if (_level > 0)
			{
				var args = new LevelChangedEventArgs(--_level);

				if (LevelChangedEvent != null)
					LevelChangedEvent(this, args);
			}
		}

		/// <summary>
		/// Changes the '_level' property and fires a LevelChanged event.
		/// </summary>
		public void Down()
		{
			if (_level < MapSize.Levs - 1)
			{
				++_level; // TODO: wait a second !
				var args = new LevelChangedEventArgs(_level + 1);

				if (LevelChangedEvent != null)
					LevelChangedEvent(this, args);
			}
		}

		public virtual void ResizeTo(	// NOTE: This doesn't handle Routes or node-checking
				int rPost,				// which XCMapFile.ResizeTo() does.
				int cPost,
				int lPost,
				bool toCeiling)
		{
//			var tileList = MapResizeService.ResizeMap(
//												rPost,
//												cPost,
//												lPost,
//												MapSize,
//												MapTiles,
//												toCeiling);
//			if (tileList != null)
//			{
//				MapChanged = true;
//
//				MapTiles = tileList;
//				MapSize  = new MapSize(rPost, cPost, lPost);
//
//				if (lPost > 0) // assuage FxCop re 'possible' underflow.
//					_level = hPost - 1;
//			}
		}

		/// <summary>
		/// Not generic enough to call with custom derived classes other than
		/// XCMapFile.
		/// </summary>
		/// <param name="file"></param>
		public void SaveGif(string file)
		{
			var palette = GetFirstGroundPalette();
			if (palette == null)
				throw new ArgumentNullException("file", "XCMapBase: At least 1 ground tile is required.");

			var rowPlusCols = MapSize.Rows + MapSize.Cols;
			var b = XCBitmap.MakeBitmap(
								rowPlusCols * (PckImage.Width / 2),
								(MapSize.Levs - _level) * 24 + rowPlusCols * 8,
								palette.Colors);

			var start = new Point(
								(MapSize.Rows - 1) * (PckImage.Width / 2),
								-(24 * _level));

			int i = 0;
			if (MapTiles != null)
			{
				for (int l = MapSize.Levs - 1; l >= _level; --l)
				{
					for (int
							r = 0, startX = start.X, startY = start.Y + l * 24;
							r != MapSize.Rows;
							++r, startX -= HalfWidth, startY += HalfHeight)
					{
						for (int
								c = 0, x = startX, y = startY;
								c != MapSize.Cols;
								++c, x += HalfWidth, y += HalfHeight, ++i)
						{
							var tiles = this[r, c, l].UsedTiles;
							foreach (var tileBase in tiles)
							{
								var tile = (XCTile)tileBase;
								XCBitmap.Draw(tile[0].Image, b, x, y - tile.Record.TileOffset);
							}

							XCBitmap.FireLoadingEvent(i, (MapSize.Levs - _level) * MapSize.Rows * MapSize.Cols);
						}
					}
				}
			}
			try
			{
				var rect = XCBitmap.GetBoundsRect(b, Palette.TransparentId);
				b = XCBitmap.Crop(b, rect);
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
			for (int l = 0; l != MapSize.Levs; ++l)
				for (int r = 0; r != MapSize.Rows; ++r)
					for (int c = 0; c != MapSize.Cols; ++c)
					{
						var tile = (XCMapTile)this[r, c, l];
						if (tile.Ground != null)
							return tile.Ground[0].Palette;
					}

			return null;
		}
	}
}
