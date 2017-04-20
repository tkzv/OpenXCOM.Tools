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

		private readonly List<SpriteSelected> _spritesSelected = new List<SpriteSelected>();

		private XCImageCollection _spritePack;

		private const int Pad = 2;

		private int _tileX = -1;
		private int _tileY = -1;

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

		internal int AbstractHeight
		{
			get { return (_spritePack != null) ? _spritePack.Count * GetSpritePaddedHeight(_spritePack.ImageFile.ImageSize.Height) / FindTilesX()
											   : 0; }
		}

		internal XCImageCollection SpritePack
		{
			get { return _spritePack; }
			set
			{
				_spritePack = value;

				Height = AbstractHeight;

//				OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
//				OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				_spritesSelected.Clear();

				Refresh();
			}
		}

		internal ReadOnlyCollection<SpriteSelected> SelectedSprites
		{
			get
			{
				return (_spritePack != null) ? _spritesSelected.AsReadOnly()
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
				int tileX =  e.X / GetSpritePaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int tileY = (e.Y - _startY) / (_spritePack.ImageFile.ImageSize.Height + Pad * 2);

				int tilesX = FindTilesX();
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

						foreach (var sprite in _spritesSelected)
						{
							if (sprite.X == tileX && sprite.Y == tileY)
								spritePre = sprite;
						}

						if (spritePre != null)
						{
							_spritesSelected.Remove(spritePre);
						}
						else
							_spritesSelected.Add(selected);
					}
					else
					{
						_spritesSelected.Clear();
						_spritesSelected.Add(selected);
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
				int tileX =  e.X            / GetSpritePaddedWidth (_spritePack.ImageFile.ImageSize.Width);
				int tileY = (e.Y - _startY) / GetSpritePaddedHeight(_spritePack.ImageFile.ImageSize.Height);

				if (tileX != _tileX || tileY != _tileY)
				{
					_tileX = tileX;
					_tileY = tileY;

					int tilesX = FindTilesX();
					if (_tileX >= tilesX)
						_tileX  = tilesX - 1;

					if (SpriteOverEvent != null)
						SpriteOverEvent(_tileX + _tileY * tilesX);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (_spritePack != null && _spritePack.Count != 0)
			{
				var g = e.Graphics;

				int width  = GetSpritePaddedWidth(_spritePack.ImageFile.ImageSize.Width);
				int height = _spritePack.ImageFile.ImageSize.Height + Pad * 2;

				int tilesX = FindTilesX();

				if (_spritePack.Count < tilesX) tilesX = _spritePack.Count;

				for (int tileX = 0; tileX <= tilesX; ++tileX) // draw vertical lines
					g.DrawLine(
							Pens.Black,
							new Point(tileX * width,          _startY),
							new Point(tileX * width, Height - _startY));

				int tilesY = _spritePack.Count / tilesX;
				if (_spritePack.Count % tilesX != 0) ++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					g.DrawLine(
							Pens.Black,
							new Point(0,              tileY * height + _startY),
							new Point(width * tilesX, tileY * height + _startY));


				var selected = new List<int>();
				foreach (var sprite in _spritesSelected)
					selected.Add(sprite.Id);

				for (int id = 0; id != _spritePack.Count; ++id) // fill selected tiles and draw sprites.
				{
					int tileX = id % tilesX;
					int tileY = id / tilesX;

					if (selected.Contains(id))
						g.FillRectangle(
									Brushes.Crimson,
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
			if (SelectedSprites.Count != 0)
			{
				var lowestId = int.MaxValue;

				var idList = new List<int>();
				foreach (var sprite in SelectedSprites)
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
			_spritesSelected.Clear();

			if (SpritePack.Count != 0)
			{
				int tilesX = FindTilesX();

				var selected = new SpriteSelected();
				selected.Y = lowestId / tilesX;
				selected.X = lowestId - selected.Y;
				selected.Id = selected.X + selected.Y * tilesX;

				_spritesSelected.Add(selected);
			}
		}

		private int FindTilesX()
		{
			int tiles = (Width - 1) / GetSpritePaddedWidth(_spritePack.ImageFile.ImageSize.Width);
			return (tiles > 0) ? tiles
							   : 1;
		}

		private static int GetSpritePaddedWidth(int width)
		{
			return width + Pad * 2;
		}

		private static int GetSpritePaddedHeight(int height)
		{
			return height + Pad * 2;
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
