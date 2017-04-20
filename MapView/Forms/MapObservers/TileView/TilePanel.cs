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
		public event SelectedTileChangedEventHandler PanelSelectedTileChanged;


		private TileBase[] _tiles;

		private const int SpriteMargin = 2;
		private const int TileX = 32 + SpriteMargin * 2;
		private const int TileY = 40 + SpriteMargin * 2;

//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));
		private Pen _pen = new Pen(Brushes.Red, 2);

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

		public static void SetColors(Hashtable table)
		{
			_brushes = table;
		}


		public TilePanel(TileType type)
		{
			_type = type;

			_scrollBar = new VScrollBar();
			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);
			_scrollBar.LargeChange = TileY;
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


		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			_startY = -_scrollBar.Value;
			Refresh();
		}

		//int i;
		protected override void OnResize(EventArgs eventargs)
		{
			//XConsole.AdZerg("OnResize " + (++i));

			_tilesX = (Width - (_scrollBar.Visible ? _scrollBar.Width : 0)) / (TileX);

			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);
			_scrollBar.Height = Height;

//			_scrollBar.Maximum = Math.Max(AbstractHeight - Height + 10, 0);
			_scrollBar.Maximum = Math.Max(AbstractHeight - Height + _scrollBar.LargeChange, 0);
//			_scrollBar.Maximum = Math.Max(AbstractHeight - Height, 0);
//			_scrollBar.Maximum = Math.Max(AbstractHeight - Height + _scrollBar.LargeChange / 2, 0);

			_scrollBar.Visible = (_scrollBar.Maximum != 0);

//			Refresh();
		}

/*		public int StartY
		{
			get { return _startY; }
			set
			{
				_startY = value;
				Refresh();
			}
		} */

		private int AbstractHeight
		{
			get
			{
				if (_tiles != null && _tilesX > 0)
				{
					if (_tiles.Length % _tilesX == 0)
						return (_tiles.Length / _tilesX) * (TileY);

					return (_tiles.Length / _tilesX + 1) * (TileY);
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

			if (e.Delta < 0)
			{
				if (_scrollBar.Value  + _scrollBar.LargeChange < _scrollBar.Maximum)
					_scrollBar.Value += _scrollBar.LargeChange;
				else
					_scrollBar.Value = _scrollBar.Maximum;
			}
			else if (e.Delta > 0)
			{
				if (_scrollBar.Value  - _scrollBar.LargeChange > 0)
					_scrollBar.Value -= _scrollBar.LargeChange;
				else
					_scrollBar.Value = 0;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			if (_tiles != null)
			{
				int x =  e.X            / (TileX);
				int y = (e.Y - _startY) / (TileY);

				if (x >= _tilesX)
					x  = _tilesX - 1;

				int tile = x + y * _tilesX;
				if (tile < _tiles.Length)
				{
					_id = tile;

					if (PanelSelectedTileChanged != null)
						PanelSelectedTileChanged(SelectedTile);

					Refresh();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_tiles != null)
			{
				Graphics g = e.Graphics;

				int x = 0;
				int y = 0;
				int top;
				int left;

				foreach (var tile in _tiles)
				{
					top  = y * TileY + _startY;
					left = x * TileX;

					var rect = new Rectangle(
										left,  top,
										TileX, TileY);

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
							i * TileX, _startY,
							i * TileX, _startY + AbstractHeight);

				for (int i = 0; i <= AbstractHeight; i += TileY)
					g.DrawLine(
							Pens.Black,
							0,               _startY + i,
							_tilesX * TileX, _startY + i);

				g.DrawRectangle(
							_pen,
							_id % _tilesX * TileX,
							_startY + _id / _tilesX * TileY,
							TileX, TileY);
			}
		}

		public TileBase SelectedTile
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

					int y   =     _startY + (_id / _tilesX) * (TileY);
					int val = y - _startY;

					if (val > 0)
					{
						_scrollBar.Value = (val < _scrollBar.Maximum) ? val
																	  : _scrollBar.Maximum;
					}
					else
						_scrollBar.Value = 0;
				}
				else
					_id = 0;
			}
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
