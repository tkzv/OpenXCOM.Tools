using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal delegate void TileSelectedEventHandler(TileBase tile);


	/// <summary>
	/// A separate panel is created for each tab-page in the Tile viewer.
	/// </summary>
	internal sealed class TilePanel
		:
			Panel
	{
		internal event TileSelectedEventHandler TileSelectedEvent;


		#region Fields & Properties
		private TileBase[] _tiles;

		private readonly VScrollBar _scrollBar;

		private const int SpriteMargin = 2;
		private const int SpriteWidth  = 32 + SpriteMargin * 2;
		private const int SpriteHeight = 40 + SpriteMargin * 2;

		private const int _largeChange = SpriteHeight;	// apparently .NET won't return an accurate value
														// for LargeChange unless the scrollbar is visible.

		private Pen   _penBlack        = new Pen(Brushes.Black, 1);
		private Pen   _penRed          = new Pen(Brushes.Red, 3);
		private Pen   _penControlLight = new Pen(SystemColors.ControlLight, 1);
		private Brush _brushBlack      = new SolidBrush(Color.Black);

		private static Hashtable _specialTypeBrushes;

		private int _tilesX = 1;
		private int _startY;
		private int _id;

		private TileType _quadType;

		private int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				if (_tiles != null && _tiles.Length != 0)
				{
					_tilesX = (Width - _scrollBar.Width - 1) / SpriteWidth; // reserve width for the scrollbar.

					if (_tilesX != 0)	// <- happens when minimizing the TileView form.
					{					// NOTE: that could be intercepted and disallowed w/
										// 'if (WindowState != FormWindowState.Minimized)' in the OnResize().
						if (_tilesX > _tiles.Length)
							_tilesX = _tiles.Length;

						int extra = 0;
						if (_tiles.Length % _tilesX != 0)
							extra = 1;

						return (_tiles.Length / _tilesX + extra) * SpriteHeight;
					}
				}

				_tilesX = 1;
				return 0;
			}
		}

		/// <summary>
		/// Gets the selected-tile-id.
		/// Sets the selected-tile-id when a valid QuadrantPanel quad is
		/// double-clicked.
		/// </summary>
		internal TileBase TileSelected
		{
			get
			{
				if (_id > -1 && _id < _tiles.Length)
					return _tiles[_id];

				return null;
			}
			set
			{
				if (value != null)
				{
					_id = value.TileListId + 1; // +1 to account for the eraser - not sure.

					if (TileSelectedEvent != null)
						TileSelectedEvent(TileSelected);

					ScrollToTile();
				}
				else
					_id = 0;
			}
		}
		#endregion


		/// <summary>
		/// Sets a pointer to a hashtable of the special property brushes/colors.
		/// The owner of the object is TileView.
		/// </summary>
		/// <param name="brushes"></param>
		internal static void SetSpecialPropertyColors(Hashtable brushes)
		{
			_specialTypeBrushes = brushes;
		}

//		private static PckSpriteCollection extraFile;
//		public static PckSpriteCollection ExtraFile
//		{
//			get { return extraFile; }
//			set { extraFile = value; }
//		}


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="quadType"></param>
		internal TilePanel(TileType quadType)
		{
			_quadType = quadType;

			Dock = DockStyle.Fill;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.LargeChange = _largeChange;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			Controls.Add(_scrollBar);

			MainViewUnderlay.AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Globals.LoadExtras();
		}
		#endregion


		#region Event Calls
		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			if (_tiles != null && _tiles.Length != 0)
			{
				_startY = -_scrollBar.Value;
				Refresh();
			}
		}


		private bool _resetTrack;

		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Maximum value.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			int range = 0;
			if (_tiles != null && _tiles.Length != 0)
			{
				if (_resetTrack)
				{
					_resetTrack = false;
					_scrollBar.Value = 0;
				}

				range = TableHeight + _largeChange - Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = (range != 0);
		}

		/// <summary>
		/// Scrolls the table by the mousewheel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var args = e as HandledMouseEventArgs;
			if (args != null)
				args.Handled = true;

			if (_scrollBar.Visible)
			{
				if (e.Delta > 0)
				{
					if (_scrollBar.Value - _scrollBar.LargeChange < 0)
						_scrollBar.Value = 0;
					else
						_scrollBar.Value -= _scrollBar.LargeChange;
				}
				else if (e.Delta < 0)
				{
					if (_scrollBar.Value + (_scrollBar.LargeChange - 1) + _scrollBar.LargeChange > _scrollBar.Maximum)
						_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
					else
						_scrollBar.Value += _scrollBar.LargeChange;
				}
			}
		}

		/// <summary>
		/// Focuses this panel and selects a tile.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			int id = GetTileUnderCursor(e);
			if (id != -1 && id < _tiles.Length)
			{
				_id = id;

				if (TileSelectedEvent != null)
					TileSelectedEvent(TileSelected);

				ScrollToTile();
				Refresh();
			}
		}

		/// <summary>
		/// Opens the MCD-info screen when a valid tile is double-clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			int id = GetTileUnderCursor(e);

			if (id != -1 && id < _tiles.Length)
				MapView.Forms.MainWindow.ViewerFormsManager.TileView.Control.OnMcdInfoClick(null, null);
		}

		private int GetTileUnderCursor(MouseEventArgs e)
		{
			if (_tiles != null && _tiles.Length != 0
				&& e.X < SpriteWidth * _tilesX) // not out of bounds to right
			{
				int tileX =  e.X            / SpriteWidth;
				int tileY = (e.Y - _startY) / SpriteHeight;

				return tileX + tileY * _tilesX;
			}
			return -1;
		}

		/// <summary>
		/// this.Fill(black)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_tiles != null && _tiles.Length != 0)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
//				graphics.SmoothingMode = SmoothingMode.HighQuality;

				int x = 0;
				int y = 0;
				int top;
				int left;

				const string door = "door";
//				int textWidth1 = TextRenderer.MeasureText(door, Font).Width;	// =30
				int textWidth2 = (int)graphics.MeasureString(door, Font).Width;	// =24

				foreach (var tile in _tiles)
				{
					left = x * SpriteWidth;
					top  = y * SpriteHeight + _startY;

					var rect = new Rectangle(
										left, top,
										SpriteWidth, SpriteHeight);

					if (tile != null) // draw tile-sprite ->
					{
						string specialType = tile.Record.TargetType.ToString();	// first fill Special Property color
						if (_specialTypeBrushes.ContainsKey(specialType))
							graphics.FillRectangle((SolidBrush)_specialTypeBrushes[specialType], rect);

						graphics.DrawImage(										// then draw the sprite itself
										tile[MainViewUnderlay.AniStep].Image,
										left + SpriteMargin,
										top  + SpriteMargin - tile.Record.TileOffset);

						// NOTE: keep the door-string and its placement consistent with
						// QuadrantPanelDrawService.Draw().
						if (tile.Record.HumanDoor || tile.Record.UfoDoor)		// finally print "door" if it's a door
							graphics.DrawString(
											door,
											Font,
											_brushBlack,
											left + (SpriteWidth  - textWidth2) / 2,
											top  +  SpriteHeight - Font.Height); // PckImage.Height
					}
					else // draw the eraser ->
					{
						graphics.FillRectangle(Brushes.AliceBlue, rect);

						if (Globals.ExtraTiles != null)
							graphics.DrawImage(
											Globals.ExtraTiles[0].Image,
											left, top);
					}

					x = (x + 1) % _tilesX;
					if (x == 0)
						y++;
				}

//				graphics.DrawRectangle(
//									_brush,
//									(_sel % _across) * (_width + _space),
//									_startY + (_sel / _across) * (_height + _space),
//									_width  + _space,
//									_height + _space)

				int height = TableHeight;

				for (int i = 0; i <= _tilesX; ++i)
					graphics.DrawLine(
									_penBlack,
									i * SpriteWidth, _startY,
									i * SpriteWidth, _startY + height);

				for (int i = 0; i <= height; i += SpriteHeight)
					graphics.DrawLine(
									_penBlack,
									0,                     _startY + i,
									_tilesX * SpriteWidth, _startY + i);

				graphics.DrawRectangle(
									_penRed,
									_id % _tilesX * SpriteWidth,
									_startY + _id / _tilesX * SpriteHeight,
									SpriteWidth, SpriteHeight);


				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					graphics.DrawLine(
									_penControlLight,
									Width - _scrollBar.Width, 0,
									Width - _scrollBar.Width, Height);
			}
		}

		/// <summary>
		/// whee. Handles animations.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			Refresh();
		}
		#endregion


		#region Methods
		internal void SetTiles(IList<TileBase> tiles)
		{
			if (tiles != null)// && _tiles.Length != 0)	// NOTE: This check for Length should be enough
			{											// to cover all other checks for Length==0.
				if (_quadType == TileType.All)			// Except that the eraser needs to be added anyway ....
				{
					_tiles = new TileBase[tiles.Count + 1];
					_tiles[0] = null;

					for (int i = 0; i != tiles.Count; ++i)
						_tiles[i + 1] = tiles[i];
				}
				else
				{
					int qtyTiles = 0;

					for (int i = 0; i != tiles.Count; ++i)
						if (tiles[i].Record.TileType == _quadType)
							++qtyTiles;

					_tiles = new TileBase[qtyTiles + 1];
					_tiles[0] = null;

					for (int i = 0, j = 1; i != tiles.Count; ++i)
						if (tiles[i].Record.TileType == _quadType)
							_tiles[j++] = tiles[i];
				}

				if (_id >= _tiles.Length)
					_id = 0;
			}
			else
			{
				_tiles = null;
				_id = 0;
			}

			_resetTrack = true;
			OnResize(null);
		}

		/// <summary>
		/// Checks if a selected tile is fully visible in the view-panel and
		/// scrolls the table to show it if not.
		/// </summary>
		private void ScrollToTile()
		{
			int tileY = _id / _tilesX;

			int cutoff = tileY * SpriteHeight;
			if (cutoff < -_startY)		// <- check cutoff high
			{
				_scrollBar.Value = cutoff;
			}
			else						// <- check cutoff low
			{
				cutoff = (tileY + 1) * SpriteHeight - Height;
				if (cutoff > -_startY)
				{
					_scrollBar.Value = cutoff;
				}
			}
		}
		#endregion
	}
}
