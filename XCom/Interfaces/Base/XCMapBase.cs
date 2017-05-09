using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

//using XCom.Services;


namespace XCom.Interfaces.Base
{
	public delegate void LocationSelectedEventHandler(LocationSelectedEventArgs e);
	public delegate void LevelChangedEventHandler(LevelChangedEventArgs e);


	/// <summary>
	/// This is basically the currently loaded Map.
	/// </summary>
	public class XCMapBase
	{
		public event LocationSelectedEventHandler LocationSelectedEvent;
		public event LevelChangedEventHandler LevelChangedEvent;


		private const int HalfWidth  = 16;
		private const int HalfHeight =  8;

		private int _level;
		/// <summary>
		/// Gets this MapBase's currently displayed level.
		/// Changing level will fire a LevelChanged event.
		/// WARNING: Level 0 is the top level of the displayed Map.
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
						LevelChangedEvent(new LevelChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// User will be shown a dialog asking to save if true.
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
		/// Gets/Sets the currently selected location. Setting the location will
		/// fire a LocationSelected event.
		/// </summary>
		public MapLocation Location
		{
			get { return _location; }
			set
			{
				if (   value.Row > -1 && value.Row < MapSize.Rows
					&& value.Col > -1 && value.Col < MapSize.Cols)
				{
					_location = value;
					var tile = this[_location.Row, _location.Col];
					var args = new LocationSelectedEventArgs(value, tile);

					if (LocationSelectedEvent != null)
						LocationSelectedEvent(args);
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
			get { return this[row, col, Level]; }
			set { this[row, col, Level] = value; }
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
		/// cTor. Instantiated only as the parent of XCMapFile.
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
		/// Changes the 'Level' property and fires a LevelChanged event.
		/// </summary>
		public void LevelUp()
		{
			if (Level > 0)
			{
				--Level;

				if (LevelChangedEvent != null)
					LevelChangedEvent(new LevelChangedEventArgs(Level));
			}
		}

		/// <summary>
		/// Changes the 'Level' property and fires a LevelChanged event.
		/// </summary>
		public void LevelDown()
		{
			if (Level < MapSize.Levs - 1)
			{
				++Level;

				if (LevelChangedEvent != null)
					LevelChangedEvent(new LevelChangedEventArgs(Level));
			}
		}

		/// <summary>
		/// Forwards the call to XCMapFile.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		/// <param name="ceiling"></param>
		public virtual void MapResize(	// NOTE: This doesn't handle Routes or node-checking
				int rows,				// which XCMapFile.ResizeTo() does.
				int cols,
				int levs,
				bool ceiling)
		{}

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
								(MapSize.Levs - Level) * 24 + rowPlusCols * 8,
								palette.Colors);

			var start = new Point(
								(MapSize.Rows - 1) * (PckImage.Width / 2),
								-(24 * Level));

			int i = 0;
			if (MapTiles != null)
			{
				for (int lev = MapSize.Levs - 1; lev >= Level; --lev)
				{
					for (int
							row = 0,
								startX = start.X,
								startY = start.Y + lev * 24;
							row != MapSize.Rows;
							++row,
								startX -= HalfWidth,
								startY += HalfHeight)
					{
						for (int
								col = 0,
									x = startX,
									y = startY;
								col != MapSize.Cols;
								++col,
									x += HalfWidth,
									y += HalfHeight,
									++i)
						{
							var tiles = this[row, col, lev].UsedTiles;
							foreach (var tileBase in tiles)
							{
								var tile = (XCTile)tileBase;
								XCBitmap.Draw(tile[0].Sprite, b, x, y - tile.Record.TileOffset);
							}

							XCBitmap.FireLoadingEvent(i, (MapSize.Levs - Level) * MapSize.Rows * MapSize.Cols);
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
			for (int lev = 0; lev != MapSize.Levs; ++lev)
				for (int row = 0; row != MapSize.Rows; ++row)
					for (int col = 0; col != MapSize.Cols; ++col)
					{
						var tile = (XCMapTile)this[row, col, lev];
						if (tile.Ground != null)
							return tile.Ground[0].Palette;
					}

			return null;
		}
	}
}
