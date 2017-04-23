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

		public static int SpriteMargin = 2;
		private int _spriteWidth;
		private int _spriteHeight;

		private int _tilesX = 1;
		internal int TilesX
		{
			set { _tilesX = value; }
		}

		private int _tileX = -1;
		private int _tileY = -1;

		internal int StartY
		{ get; set; }

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
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				return (_spritePack != null) ? (_spritePack.Count / _tilesX + 2) * _spriteHeight
											 : 0;
			}
		}

		internal XCImageCollection SpritePack
		{
			get { return _spritePack; }
			set
			{
				_spritePack = value;

				_spriteWidth  = _spritePack.ImageFile.ImageSize.Width  + SpriteMargin * 2;
				_spriteHeight = _spritePack.ImageFile.ImageSize.Height + SpriteMargin * 2;

//				Height = AbstractHeight; ... nobody cares about the Height of Overlay. Let .NET deal with it.

//				OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
//				OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				_spritesSelected.Clear();

//				Refresh();
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
				int tileX =  e.X           / _spriteWidth;
				int tileY = (e.Y - StartY) / _spriteHeight;

				if (tileX >= _tilesX)
					tileX =  _tilesX - 1;

				int spriteId = tileX + tileY * _tilesX;

				var selected   = new SpriteSelected();
				selected.X     = tileX;
				selected.Y     = tileY;
				selected.Id    = spriteId;
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
				int tileX =  e.X           / _spriteWidth;
				int tileY = (e.Y - StartY) / _spriteHeight;

				if (tileX != _tileX || tileY != _tileY)
				{
					_tileX = tileX;
					_tileY = tileY;

					if (_tileX >= _tilesX)
						_tileX  = _tilesX - 1;

					if (SpriteOverEvent != null)
						SpriteOverEvent(_tileX + _tileY * _tilesX);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (_spritePack != null && _spritePack.Count != 0)
			{
				var g = e.Graphics;

				if (_spritePack.Count < _tilesX) _tilesX = _spritePack.Count;

				for (int tileX = 0; tileX <= _tilesX; ++tileX) // draw vertical lines
					g.DrawLine(
							Pens.Black,
							new Point(tileX * _spriteWidth,          StartY),
							new Point(tileX * _spriteWidth, Height - StartY));

				int tilesY = _spritePack.Count / _tilesX;
				if (_spritePack.Count % _tilesX != 0) ++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					g.DrawLine(
							Pens.Black,
							new Point(0,                      tileY * _spriteHeight + StartY),
							new Point(_spriteWidth * _tilesX, tileY * _spriteHeight + StartY));


				var selected = new List<int>();
				foreach (var sprite in _spritesSelected)
					selected.Add(sprite.Id);

				for (int id = 0; id != _spritePack.Count; ++id) // fill selected tiles and draw sprites.
				{
					int tileX = id % _tilesX;
					int tileY = id / _tilesX;

					if (selected.Contains(id))
						g.FillRectangle(
									Brushes.Crimson,
									tileX * _spriteWidth  + 1,
									tileY * _spriteHeight + 1 + StartY,
									_spriteWidth  - 1,
									_spriteHeight - 1);

					g.DrawImage(
							_spritePack[id].Image,
							tileX * _spriteWidth  + SpriteMargin,
							tileY * _spriteHeight + SpriteMargin + StartY);
				}
			}
		}
		#endregion


		#region Methods
		internal void ChangeSprite(int id, XCImage image)
		{
			_spritePack[id] = image;
		}

		/// <summary>
		/// Removes the currently selected sprite.
		/// </summary>
		internal void RemoveSelected()
		{
			if (_spritesSelected.Count != 0)
			{
				var lowestId = int.MaxValue;

				var idList = new List<int>();
				foreach (var sprite in _spritesSelected)
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

				_spritesSelected.Clear();
	
				if (SpritePack.Count != 0)
				{
					var selected = new SpriteSelected();
					selected.Y   = lowestId / _tilesX;
					selected.X   = lowestId - selected.Y;
					selected.Id  = selected.X + selected.Y * _tilesX;
	
					_spritesSelected.Add(selected);
				}
			}
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
