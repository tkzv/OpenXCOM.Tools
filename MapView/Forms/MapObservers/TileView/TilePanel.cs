using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal class TilePanel
		:
			Panel
	{
		private TileBase[] _tiles;

		private const int _width  = 32;
		private const int _height = 40;

		private const int Pad     =  4; // NOTE: includes the margin for both sides of '_width'.

//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));
		private Pen _pen = new Pen(Brushes.Red, 2);

		private static Hashtable _brushes;

		private int _startY = 0;
		private int _sel;
		private int _across = 1;

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

		public event SelectedTileTypeChangedEventHandler SelectedTileTypeChangedPanel;

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
			_scrollBar.ValueChanged += OnValueChange;
			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);

			Controls.Add(_scrollBar);
			MainViewPanel.ImageUpdateEvent += OnImageUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
			_sel = 0;

			Globals.LoadExtras();
		}


		private void OnValueChange(object sender, EventArgs e)
		{
			_startY = -_scrollBar.Value;
			Refresh();
		}

		protected override void OnResize(EventArgs eventargs)
		{
			_across = (Width - (_scrollBar.Visible ? _scrollBar.Width : 0)) / (_width + Pad);
			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);
			_scrollBar.Height = Height;
			_scrollBar.Maximum = Math.Max((PreferredHeight - Height) + 10, _scrollBar.Minimum);
			_scrollBar.Visible = (_scrollBar.Maximum != _scrollBar.Minimum);

			Refresh();
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

		private int PreferredHeight
		{
			get
			{
				if (_tiles != null && _across > 0)
				{
					if (_tiles.Length % _across == 0)
						return (_tiles.Length / _across) * (_height + Pad);

					return (1 + _tiles.Length / _across) * (_height + Pad);
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
						if (tiles[i].Info.TileType == _type)
							++qtyTiles;

					_tiles = new TileBase[qtyTiles + 1];
					_tiles[0] = null;

					for (int i = 0, j = 1; i != tiles.Count; ++i)
						if (tiles[i].Info.TileType == _type)
							_tiles[j++] = tiles[i];
				}

				if (_sel >= _tiles.Length)
					_sel = 0;
			}
			else
			{
				_tiles = null;
				_sel = 0;
			}

			OnResize(null);
		}

		private const int SCROLL = 20;

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var handledMouseEventArgs = e as HandledMouseEventArgs;
			if (handledMouseEventArgs != null)
				handledMouseEventArgs.Handled = true;

			if (e.Delta < 0)
			{
				if (_scrollBar.Value + SCROLL < _scrollBar.Maximum)
					_scrollBar.Value += SCROLL;
				else
					_scrollBar.Value = _scrollBar.Maximum;
			}
			else if (e.Delta > 0)
			{
				if (_scrollBar.Value - SCROLL > _scrollBar.Minimum)
					_scrollBar.Value -= SCROLL;
				else
					_scrollBar.Value = _scrollBar.Minimum;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			if (_tiles != null)
			{
				int x =  e.X / (_width + Pad);
				int y = (e.Y - _startY) / (_height + Pad);

				if (x >= _across)
					x = _across - 1;

				int tileTest = y * _across + x;
				if (tileTest < _tiles.Length)
				{
					_sel = tileTest;

					if (SelectedTileTypeChangedPanel != null)
						SelectedTileTypeChangedPanel(SelectedTile);

					Refresh();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			PaintTiles(e);
		}

		private void PaintTiles(PaintEventArgs e)
		{
			if (_tiles != null)
			{
				Graphics g = e.Graphics;

				int x = 0;
				int y = 0;
				const int width  = _width  + Pad;
				const int height = _height + Pad;
				int top, left;

				foreach (var tile in _tiles)
				{
					top  = y * height + _startY;
					left = x * width;

					var rect = new Rectangle(
										left,  top,
										width, height);

					if (tile != null)
					{
						if (_type == TileType.All || _type == tile.Info.TileType)
						{
							var targetType = tile.Info.TargetType.ToString();
							if (_brushes.ContainsKey(targetType))
								g.FillRectangle((SolidBrush)_brushes[targetType], rect);

							g.DrawImage(
									tile[MainViewPanel.Current].Image,
									left,
									top - tile.Info.TileOffset);

							if (tile.Info.HumanDoor || tile.Info.UfoDoor)
								g.DrawString(
										"Door",
										Font,
										Brushes.Black,
										left,
										top + PckImage.Height - Font.Height);

							x = (x + 1) % _across;
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

						x = (x + 1) % _across;
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

				for (int i = 0; i <= _across; i++)
					g.DrawLine(
							Pens.Black,
							i * width, _startY,
							i * width, _startY + PreferredHeight);

				for (int i = 0; i <= PreferredHeight; i += height)
					g.DrawLine(
							Pens.Black,
							0,               _startY + i,
							_across * width, _startY + i);

				g.DrawRectangle(
							_pen,
							(_sel % _across) * width, _startY + (_sel / _across) * height,
							width, height);
			}
		}

		public TileBase SelectedTile
		{
			get
			{
				if (_sel > -1 && _sel < _tiles.Length)
					return _tiles[_sel];

				return null;
			}

			set
			{
				if (value != null)
				{
					_sel = value.TileListId + 1;

					if (SelectedTileTypeChangedPanel != null)
						SelectedTileTypeChangedPanel(SelectedTile);

					int y = _startY + (_sel / _across) * (_height + Pad);
					int val = y - _startY;

					if (val > _scrollBar.Minimum)
					{
						_scrollBar.Value = (val < _scrollBar.Maximum) ? val : _scrollBar.Maximum;
					}
					else
						_scrollBar.Value = _scrollBar.Minimum;
				}
				else
					_sel = 0;
			}
		}

		private void OnImageUpdate(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
