using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	public class TilePanel
		:
		Panel
	{
		private TileBase[] tiles;

		private const int _width  = 32;
		private const int _height = 40;
		private const int _space  =  4; // NOTE: includes the margin for both sides of '_width'.

//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private Pen _pen = new Pen(Brushes.Red, 2);

		private static Hashtable _brushes;

		private int _startY = 0;
		private int _sel;
		private int _across = 1;

		private VScrollBar _scrollBar;

		private TileType _type;

		public static readonly Color[] _tileTypes =
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

		public event SelectedTileTypeChanged TileChanged;

//		private static PckFile extraFile;
//		public static PckFile ExtraFile
//		{
//			get { return extraFile; }
//			set { extraFile = value; }
//		}

		public static Hashtable Colors
		{
			get { return _brushes; }
			set { _brushes = value; }
		}


		public TilePanel(TileType type)
		{
			_type = type;
			_scrollBar = new VScrollBar();
			_scrollBar.ValueChanged += valChange;
			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);

			Controls.Add(_scrollBar);
			MapViewPanel.ImageUpdate += tick; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
			_sel = 0;

			Globals.LoadExtras();
		}


		private void valChange(object sender, EventArgs e)
		{
			_startY = -_scrollBar.Value;
			Refresh();
		}

		protected override void OnResize(EventArgs eventargs)
		{
			_across = (Width - (_scrollBar.Visible ? _scrollBar.Width : 0)) / (_width + _space);
			_scrollBar.Location = new Point(Width - _scrollBar.Width, 0);
			_scrollBar.Height = Height;
			_scrollBar.Maximum = Math.Max((PreferredHeight - Height) + 10, _scrollBar.Minimum);
			_scrollBar.Visible = (_scrollBar.Maximum != _scrollBar.Minimum);

			Refresh();
		}

		public int StartY
		{
			get { return _startY; }
			set
			{
				_startY = value;
				Refresh();
			}
		}

		public int PreferredHeight
		{
			get
			{
				if (tiles != null && _across > 0)
				{
					if (tiles.Length % _across == 0)
						return (tiles.Length / _across) * (_height + _space);

					return (1 + tiles.Length / _across) * (_height + _space);
				}
				return 0;
			}
		}

		public System.Collections.Generic.List<TileBase> Tiles
		{
			set
			{
				if (value != null)
				{
					if (_type == TileType.All)
					{
						tiles = new TileBase[value.Count + 1];
						tiles[0] = null;

						for (int i = 0; i < value.Count; i++)
							tiles[i + 1] = value[i];
					}
					else
					{
						int qtyTiles = 0;

						for (int i = 0; i < value.Count; i++)
							if (value[i].Info.TileType == _type)
								++qtyTiles;

						tiles = new TileBase[qtyTiles + 1];
						tiles[0] = null;

						for (int i = 0, j = 1; i < value.Count; i++)
							if (value[i].Info.TileType == _type)
								tiles[j++] = value[i];

/*						var list = new List<TileBase>(); // NOTE: Replaced by above^ to add 1st blank/erasure-tile to each tile-group.
						for (int i = 0; i < value.Count; i++)
							if (value[i].Info.TileType == _type)
								list.Add(value[i]);
						tiles = list.ToArray(); */
					}

					if (_sel >= tiles.Length)
						_sel = 0;
				}
				else
				{
					tiles = null;
					_sel = 0;
				}

				OnResize(null);
			}
		}

		private const int scrollAmount = 20;

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var handledMouseEventArgs = e as HandledMouseEventArgs;
			if (handledMouseEventArgs != null)
				handledMouseEventArgs.Handled = true;

			if (e.Delta < 0)
			{
				if (_scrollBar.Value + scrollAmount < _scrollBar.Maximum)
					_scrollBar.Value += scrollAmount;
				else
					_scrollBar.Value = _scrollBar.Maximum;
			}
			else if (e.Delta > 0)
			{
				if (_scrollBar.Value - scrollAmount > _scrollBar.Minimum)
					_scrollBar.Value -= scrollAmount;
				else
					_scrollBar.Value = _scrollBar.Minimum;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			if (tiles != null)
			{
				int x =  e.X / (_width + _space);
				int y = (e.Y - _startY) / (_height + _space);

				if (x >= _across)
					x = _across - 1;

				_sel = y * _across + x;
				_sel = (_sel < tiles.Length) ? _sel : tiles.Length - 1;

				if (TileChanged != null)
					TileChanged(SelectedTile);

				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			PaintTiles(e);
		}

		private void PaintTiles(PaintEventArgs e)
		{
			if (tiles != null)
			{
				Graphics g = e.Graphics;

				int x = 0;
				int y = 0;
				const int width  = _width  + _space;
				const int height = _height + _space;
				int top, left;

				foreach (var tile in tiles)
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
									tile[MapViewPanel.Current].Image,
									left,
									top - tile.Info.TileOffset);

							if (tile.Info.HumanDoor || tile.Info.UFODoor)
								g.DrawString(
										"Door",
										this.Font,
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

				for (int k = 0; k < _across; k++)
					g.DrawLine(
							Pens.Black,
							k * width, _startY,
							k * width, _startY + PreferredHeight);

				for (int k = 0; k <= PreferredHeight; k += height)
					g.DrawLine(
							Pens.Black,
							0,               _startY + k,
							_across * width, _startY + k);

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
				if (_sel > -1 && _sel < tiles.Length)
					return tiles[_sel];

				return null;
			}

			set
			{
				if (value != null)
				{
					_sel = value.MapId + 1;

					if (TileChanged != null)
						TileChanged(SelectedTile);

					int y = _startY + (_sel / _across) * (_height + _space);
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

		private void tick(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
