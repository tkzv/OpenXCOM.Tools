using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal delegate void SelectedTileChangedEventHandler(TileBase tile);


	internal sealed class TilePanel
		:
			Panel
	{
		internal event SelectedTileChangedEventHandler PanelSelectedTileChanged;


		private TileBase[] _tiles;

		private const int SpriteMargin = 2;
		private const int SpriteWidth  = 32 + SpriteMargin * 2;
		private const int SpriteHeight = 40 + SpriteMargin * 2;

//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));
		private Pen _pen = new Pen(Brushes.Red, 3); // TODO: find some happy colors

		private static Hashtable _brushes;

		private int _tilesX = 1;
		private int _startY;
		private int _id;

		private VScrollBar _scrollBar;

		private TileType _type;

		internal static readonly Color[] TileColors =
		{
			Color.Cornsilk,
			Color.Lavender,
			Color.DarkRed,
			Color.Fuchsia,
			Color.Aqua,
			Color.DarkOrange,
			Color.DeepPink,
			Color.LightBlue,
			Color.Lime,
			Color.LightGreen,
			Color.MediumPurple,
			Color.LightCoral,
			Color.LightCyan,
			Color.Yellow,
			Color.Blue
		};

//		private static PckSpriteCollection extraFile;
//		public static PckSpriteCollection ExtraFile
//		{
//			get { return extraFile; }
//			set { extraFile = value; }
//		}

		internal static void SetColors(Hashtable table)
		{
			_brushes = table;
		}


		internal TilePanel(TileType type)
		{
			_type = type;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.LargeChange = SpriteHeight;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			Controls.Add(_scrollBar);
			MainViewPanel.AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Globals.LoadExtras();
		}


		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			_scrollBar.Maximum = Math.Max(TableHeight + _scrollBar.LargeChange - Height, 0);
			// That is needed only for initialization.
			// OnResize, which also sets '_scrollBar.Maximum', fires a dozen
			// times during init, but it gets the value right only once (else
			// '0') and not on the last call either. So just do it here and
			// marvel at the wonders of c#/.NET

			_startY = -_scrollBar.Value;
			Refresh();
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			_tilesX = Math.Max((Width - 1 - (_scrollBar.Visible ? _scrollBar.Width : 0)) / SpriteWidth, 1);

//			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);
//			_scrollBar.Height = Height;

			_scrollBar.Maximum = Math.Max(TableHeight + _scrollBar.LargeChange - Height, 0);
			_scrollBar.Visible = (_scrollBar.Maximum != 0);
		}

		private int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				if (_tiles != null)
				{
					int extra = 0;
					if (_tiles.Length % _tilesX != 0)
						extra = 1;

					return (_tiles.Length / _tilesX + extra) * SpriteHeight;
				}
				return 0;
			}
		}

		internal void SetTiles(IList<TileBase> tiles)
		{
			if (tiles != null)
			{
				if (_type == TileType.All)
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
						if (tiles[i].Record.TileType == _type)
							++qtyTiles;

					_tiles = new TileBase[qtyTiles + 1];
					_tiles[0] = null;

					for (int i = 0, j = 1; i != tiles.Count; ++i)
						if (tiles[i].Record.TileType == _type)
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

			OnResize(null);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var handledMouseEventArgs = e as HandledMouseEventArgs;
			if (handledMouseEventArgs != null)
				handledMouseEventArgs.Handled = true;

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

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			if (_tiles != null)
			{
				int tileX =  e.X            / SpriteWidth;
				int tileY = (e.Y - _startY) / SpriteHeight;

				if (tileX >= _tilesX)
					tileX  = _tilesX - 1;

				int tile = tileX + tileY * _tilesX;
				if (tile < _tiles.Length)
				{
					_id = tile;

					if (PanelSelectedTileChanged != null)
						PanelSelectedTileChanged(SelectedTile);

					TileScrollClient();
					Refresh();
				}
			}
		}

		/// <summary>
		/// this.Fill(black)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_tiles != null)
			{
				var g = e.Graphics;

				int x = 0;
				int y = 0;
				int top;
				int left;

				foreach (var tile in _tiles)
				{
					top  = y * SpriteHeight + _startY;
					left = x * SpriteWidth;

					var rect = new Rectangle(
										left,  top,
										SpriteWidth, SpriteHeight);

					if (tile != null)
					{
						if (_type == TileType.All || _type == tile.Record.TileType)
						{
							var targetType = tile.Record.TargetType.ToString();
							if (_brushes.ContainsKey(targetType))
								g.FillRectangle((SolidBrush)_brushes[targetType], rect);

							g.DrawImage(
									tile[MainViewPanel.AniStep].Image,
									left,
									top - tile.Record.TileOffset);

							if (tile.Record.HumanDoor || tile.Record.UfoDoor)
								g.DrawString(
										"Door",
										Font,
										Brushes.Black,
										left,
										top + PckImage.Height - Font.Height);

							x = (x + 1) % _tilesX;
							if (x == 0)
								y++;
						}
					}
					else
					{
						g.FillRectangle(Brushes.AliceBlue, rect);

						if (Globals.ExtraTiles != null)
							g.DrawImage(
									Globals.ExtraTiles[0].Image,
									left, top);

						x = (x + 1) % _tilesX;
						if (x == 0)
							y++;
					}
				}

//				g.DrawRectangle(
//							_brush,
//							(_sel % _across) * (_width + _space),
//							_startY + (_sel / _across) * (_height + _space),
//							_width  + _space,
//							_height + _space)

				for (int i = 0; i <= _tilesX; ++i)
					g.DrawLine(
							Pens.Black,
							i * SpriteWidth, _startY,
							i * SpriteWidth, _startY + TableHeight);

				for (int i = 0; i <= TableHeight; i += SpriteHeight)
					g.DrawLine(
							Pens.Black,
							0,               _startY + i,
							_tilesX * SpriteWidth, _startY + i);

				g.DrawRectangle(
							_pen,
							_id % _tilesX * SpriteWidth,
							_startY + _id / _tilesX * SpriteHeight,
							SpriteWidth, SpriteHeight);
			}
		}

		/// <summary>
		/// Gets the selected-tile-id.
		/// Sets the selected-tile-id when a valid QuadrantPanel quad is
		/// double-clicked.
		/// </summary>
		internal TileBase SelectedTile
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
					_id = value.TileListId + 1;

					if (PanelSelectedTileChanged != null)
						PanelSelectedTileChanged(SelectedTile);

					TileScrollClient();
				}
				else
					_id = 0;
			}
		}

		private void TileScrollClient()
		{
			int tileY = _id / _tilesX;

			int cutoff = tileY * SpriteHeight;
			if (cutoff < -_startY)		// <- check cutoff high
			{
				_scrollBar.Value = cutoff;
			}
			else						// <- check cutoff low
			{
				cutoff = (tileY + 1) * SpriteHeight - ClientSize.Height;
				if (cutoff > -_startY)
				{
					_scrollBar.Value = cutoff;
				}
			}
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
