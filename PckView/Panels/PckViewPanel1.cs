using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using PckView.Args;
using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal delegate void MouseClickedEventHandler(object sender, PckViewMouseEventArgs e);
	internal delegate void MouseMovedEventHandler(int pixels);


	internal sealed class PckViewPanel1
		:
			Panel
	{
		internal event MouseClickedEventHandler MouseClickedEvent;
		internal event MouseMovedEventHandler MouseMovedEvent;


		#region Fields & Properties

		private readonly List<SpriteSelected> _sprites = new List<SpriteSelected>();

		private XCImageCollection _spritePack;

//		private Color _goodColor = Color.FromArgb(204, 204, 255);
//		private SolidBrush _goodBrush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private int _tileX;
		private int _tileY;

		private int _startY;

		private const int Pad = 2;

		internal Palette Pal
		{
			get { return _spritePack.Pal; }
			set
			{
				if (_spritePack != null)
					_spritePack.Pal = value;
			}
		}

		internal int StartY
		{
			set
			{
				_startY = value;
				Refresh();
			}
		}

		internal int PreferredHeight
		{
			get { return (_spritePack != null) ? CountTilesVertical()
											   : 0; }
		}

		internal XCImageCollection SpritePack
		{
			get { return _spritePack; }
			set
			{
				_spritePack = value;

				if (_spritePack != null)
					Height = CountTilesVertical();
				else
					Height = 0;

				OnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
				OnMouseMove(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				Refresh();
			}
		}

		internal ReadOnlyCollection<SpriteSelected> Sprites
		{
			get
			{
				if (_spritePack != null)
				{
					return _sprites.AsReadOnly();

//					var sprites = new List<SpriteSelected>();
//					foreach (var sprite0 in _sprites)
//					{
//						var sprite1 = new SpriteSelected();
//						sprite1.Item = sprite0;
//						sprite1.Image = _spritePack[GetId(sprite0)];
//						sprites.Add(sprite1);
//					}
//					return sprites.AsReadOnly();
				}
				return null;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PckViewPanel1()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			MouseDown += OnMouseDown;
			MouseMove += OnMouseMove;

			Paint += OnPaint;
		}
		#endregion



		#region EventCalls
		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (_spritePack != null)
			{
				int tileX =  e.X / GetPaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int tileY = (e.Y - _startY) / (_spritePack.ImageFile.ImageSize.Height + Pad * 2);

				int tilesHori = CountTilesHorizontal();
				if (tileX >= tilesHori)
					tileX =  tilesHori - 1;

				int id = tileX + tileY * tilesHori;

				var selected = new SpriteSelected();
				selected.X = tileX;
				selected.Y = tileY;
				selected.Index = id;
				selected.Image = _spritePack[id];

				if (id < SpritePack.Count)
				{
					if (ModifierKeys == Keys.Control)
					{
						SpriteSelected spritePre = null;

						foreach (var sprite in _sprites)
						{
							if (sprite.X == tileX && sprite.Y == tileY)
								spritePre = sprite;
						}

						if (spritePre != null)
						{
							_sprites.Remove(spritePre);
						}
						else
							_sprites.Add(selected);
					}
					else
					{
						_sprites.Clear();
						_sprites.Add(selected);
					}

					Refresh();

					if (MouseClickedEvent != null)
					{
						var args = new PckViewMouseEventArgs(e, id);
						MouseClickedEvent(this, args);
					}
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_spritePack != null)
			{
				int tileX =  e.X / GetPaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int tileY = (e.Y - _startY) / (_spritePack.ImageFile.ImageSize.Height + Pad * 2);

				if (tileX != _tileX || tileY != _tileY)
				{
					_tileX = tileX;
					_tileY = tileY;

					int tilesHori = CountTilesHorizontal();
					if (_tileX >= tilesHori)
						_tileX  = tilesHori - 1;

					if (MouseMovedEvent != null)
						MouseMovedEvent(_tileX + _tileY * tilesHori);
				}
			}
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			if (_spritePack != null && _spritePack.Count != 0)
			{
				var g = e.Graphics;

				var widthPadded = GetPaddedWidth(_spritePack.ImageFile.ImageSize.Width);

				foreach (var sprite in _sprites)
				{
//					if (_collection.ImageFile.FileOptions.BitDepth == 8 && _collection[0].Palette.Transparent.A == 0)
//					{
//						g.FillRectangle(
//									_goodBrush,
//									selectedItem.X * specialWidth - Pad,
//									_startY + selectedItem.Y * (_collection.ImageFile.ImageSize.Height + Pad * 2) - Pad,
//									specialWidth,
//									_collection.ImageFile.ImageSize.Height + Pad * 2);
//					}
//					else
//					{
					g.FillRectangle(
								Brushes.Red,
								sprite.X * widthPadded - Pad,
								_startY + sprite.Y * (_spritePack.ImageFile.ImageSize.Height + Pad * 2) - Pad,
								widthPadded,
								_spritePack.ImageFile.ImageSize.Height + Pad * 2);
//					}
				}

				int across = CountTilesHorizontal();

				for (int i = 0; i < across + 1; ++i)
					g.DrawLine(
							Pens.Black,
							new Point(i * widthPadded - Pad,          _startY),
							new Point(i * widthPadded - Pad, Height - _startY));

				for (int i = 0; i < _spritePack.Count / across + 1; ++i)
					g.DrawLine(
							Pens.Black,
							new Point(0,     _startY + i * (_spritePack.ImageFile.ImageSize.Height + Pad * 2) - Pad),
							new Point(Width, _startY + i * (_spritePack.ImageFile.ImageSize.Height + Pad * 2) - Pad));

				for (int i = 0; i < _spritePack.Count; ++i)
				{
					int x = i % across;
					int y = i / across;
					try
					{
						g.DrawImage(
								_spritePack[i].Image, x * widthPadded,
								_startY + y * (_spritePack.ImageFile.ImageSize.Height + Pad * 2));
					}
					catch {} // TODO: that.
				}
			}
		}
		#endregion


		#region Methods
		internal void ChangeSprite(int id, XCImage image)
		{
			_spritePack[id] = image;
		}

		internal void RemoveSelected()
		{
			if (Sprites.Count != 0)
			{
				var lowestId = int.MaxValue;

				var idList = new List<int>();
				foreach (var sprite in Sprites)
					idList.Add(sprite.Index);

				idList.Sort();
				idList.Reverse();

				foreach (var id in idList)
				{
					if (id < lowestId)
						lowestId = id;

					SpritePack.Remove(id);
				}

				if (lowestId > 0 && lowestId == SpritePack.Count)
					lowestId = SpritePack.Count - 1;

				SetSelected(lowestId);
			}
		}

		private void SetSelected(int lowestId)
		{
			_sprites.Clear();

			if (SpritePack.Count != 0)
			{
				int tilesHori = CountTilesHorizontal();

				var selected = new SpriteSelected();
				selected.Y = lowestId / tilesHori;
				selected.X = lowestId - selected.Y;
				selected.Index = selected.X + selected.Y * tilesHori;

				_sprites.Add(selected);
			}
		}

		private int CountTilesHorizontal()
		{
			return Math.Max(
						1,
						(Width - 8) / (_spritePack.ImageFile.ImageSize.Width + Pad * 2));
		}

		private int CountTilesVertical()
		{
			return _spritePack.Count * (_spritePack.ImageFile.ImageSize.Height + Pad * 2)
				 / CountTilesHorizontal() + 60;
		}

		private int GetId(SpriteSelected sprite0)
		{
			return sprite0.X + sprite0.Y * CountTilesHorizontal();
		}

		private static int GetPaddedWidth(int width)
		{
			return width + Pad * 2;
		}
		#endregion
	}
}

/*		/// <summary>
		/// Saves a bitmap as an 8-bit image.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="pal"></param>
		public void SaveBMP(string file, Palette pal)
		{
			Bmp.SendToSaver(file, _collection, pal, numAcross(), 1);
		} */

/*		internal void Hq2x()
		{
			_collection.HQ2X();
		} */
