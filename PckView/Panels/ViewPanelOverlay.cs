using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal delegate void SpriteClickEventHandler(int spriteId);
	internal delegate void SpriteOverEventHandler(int spriteId);


	internal sealed class ViewPanelOverlay
		:
			Panel
	{
		internal event SpriteClickEventHandler SpriteClickEvent;
		internal event SpriteOverEventHandler SpriteOverEvent;


		#region Fields & Properties

		private readonly List<SpriteSelected> _sprites = new List<SpriteSelected>();

		private XCImageCollection _spritePack;

//		private Color _goodColor = Color.FromArgb(204, 204, 255);
//		private SolidBrush _goodBrush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private const int Pad = 2;

		private int _tileX;
		private int _tileY;

		private int _startY;
		internal int StartY
		{
			get { return _startY; }
			set
			{
				_startY = value;
				Refresh();
			}
		}

		internal Palette Pal
		{
			get { return _spritePack.Pal; }
			set
			{
				if (_spritePack != null)
					_spritePack.Pal = value;
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

				Height = (_spritePack != null) ? CountTilesVertical()
											   : 0;

				OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
				OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				Refresh();
			}
		}

		internal ReadOnlyCollection<SpriteSelected> Sprites
		{
			get
			{
				return (_spritePack != null) ? _sprites.AsReadOnly()
											 : null;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ViewPanelOverlay()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
		}
		#endregion



		#region EventCalls

		protected override void OnMouseDown(MouseEventArgs e)
		{
//			base.OnMouseDown(e);

			if (_spritePack != null)
			{
				int tileX =  e.X / GetPaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int tileY = (e.Y - _startY) / (_spritePack.ImageFile.ImageSize.Height + Pad * 2);

				int tilesX = CountTilesHorizontal();
				if (tileX >= tilesX)
					tileX =  tilesX - 1;

				int spriteId = tileX + tileY * tilesX;

				var selected = new SpriteSelected();
				selected.X = tileX;
				selected.Y = tileY;
				selected.Id = spriteId;
				selected.Image = _spritePack[spriteId];

				if (spriteId < SpritePack.Count)
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

					if (SpriteClickEvent != null)
						SpriteClickEvent(spriteId);
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

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

					if (SpriteOverEvent != null)
						SpriteOverEvent(_tileX + _tileY * tilesHori);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			XConsole.AdZerg("OnPaint");
//			base.OnPaint(e);

			if (_spritePack != null && _spritePack.Count != 0)
			{
				var g = e.Graphics;

				int width  = GetPaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int height = _spritePack.ImageFile.ImageSize.Height + Pad * 2;

				int across = CountTilesHorizontal();

				int tilesX = across + 1;
				for (int tileX = 0; tileX != tilesX; ++tileX)
					g.DrawLine(
							Pens.Black,
							new Point(tileX * width,          _startY),
							new Point(tileX * width, Height - _startY));

				int tilesY = _spritePack.Count / across + 1;
				for (int tileY = 0; tileY <= tilesY; ++tileY)
					g.DrawLine(
							Pens.Black,
							new Point(0,     tileY * height + _startY),
							new Point(Width, tileY * height + _startY));


				var selected = new List<int>();
				foreach (var sprite in _sprites)
					selected.Add(sprite.Id);

				for (int id = 0; id != _spritePack.Count; ++id)
				{
					int tileX = id % across;
					int tileY = id / across;

					if (selected.Contains(id))
						g.FillRectangle(
									Brushes.Red, // _goodBrush
									tileX * width  + 1,
									tileY * height + 1 + _startY,
									width  - 1,
									height - 1);

					g.DrawImage(
							_spritePack[id].Image,
							tileX * width  + Pad,
							tileY * height + Pad + _startY);
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
					idList.Add(sprite.Id);

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
				selected.Id = selected.X + selected.Y * tilesHori;

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

		private static int GetPaddedWidth(int width)
		{
			return width + Pad * 2;
		}

//		private int GetId(SpriteSelected sprite0)
//		{
//			return sprite0.X + sprite0.Y * CountTilesHorizontal();
//		}
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
