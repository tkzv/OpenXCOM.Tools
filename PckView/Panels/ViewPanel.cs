using System;
using System.Collections.Generic; // List
using System.Collections.ObjectModel; // ReadOnlyCollection
using System.Drawing; // Pens, Brushes
using System.Windows.Forms; // Panel

using XCom; // Palette, XCImageCollection
using XCom.Interfaces; // XCImage


namespace PckView
{
	internal delegate void SpritePackChangedEventHandler(SpritePackChangedEventArgs e);

	internal delegate void SpriteOverEventHandler(int spriteId);
	internal delegate void SpriteClickEventHandler(int spriteId);


	internal sealed class ViewPanel
		:
			Panel
	{
		internal event SpritePackChangedEventHandler SpritePackChangedEvent;
		internal event SpriteClickEventHandler       SpriteClickEvent;
		internal event SpriteOverEventHandler        SpriteOverEvent;


		#region Fields & Properties

		private VScrollBar     _scrollBar;
		private StatusBar      _statusBar;
		private StatusBarPanel _statusTileSelected;
		private StatusBarPanel _statusTileOver;


		internal Palette Pal
		{ get; set; }

		private XCImageCollection _spritePack;
		internal XCImageCollection SpritePack
		{
			get { return _spritePack; }
			set
			{
				LogFile.WriteLine("");
				LogFile.WriteLine("SpritePack set");

				_spritePack = value;

				_spriteWidth  = value.ImageFile.ImageSize.Width  + SpriteMargin * 2;
				_spriteHeight = value.ImageFile.ImageSize.Height + SpriteMargin * 2;
				LogFile.WriteLine(". spriteWidth= " + _spriteWidth);
				LogFile.WriteLine(". spriteHeight= " + _spriteHeight);

//				Height = AbstractHeight; ... nobody cares about the Height of Overlay. Let .NET deal with it.

//				OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
//				OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				_selectedSprites.Clear();

//				Refresh();
				_scrollBar.LargeChange = value.ImageFile.ImageSize.Height + SpriteMargin * 2;
				LogFile.WriteLine(". LargeChange= " + (_scrollBar.LargeChange + _forceLargeChange));

				UpdateScrollbar(true);
				OnSpriteOver(-1);
				OnSpriteClick(-1);

				if (SpritePackChangedEvent != null)
					SpritePackChangedEvent(new SpritePackChangedEventArgs(value));
			}
		}

		private const int SpriteMargin = 2;

		private int _spriteWidth;
		private int _spriteHeight;

		private int _tilesX = 1;

		private int _tileX = -1;
		private int _tileY = -1;

		private int _startY;


		private readonly List<SpriteSelected> _selectedSprites = new List<SpriteSelected>();
		internal ReadOnlyCollection<SpriteSelected> SelectedSprites
		{
			get
			{
				return (SpritePack != null) ? _selectedSprites.AsReadOnly()
											: null;
			}
		}

		private int _idSelected;
		private int _idOver;

		/// <summary>
		/// Used by UpdateScrollBar() to set its Maximum value.
		/// </summary>
		internal int AbstractHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				LogFile.WriteLine("AbstractHeight");

				SetTilesX();
				LogFile.WriteLine(". Count= " + SpritePack.Count);
				LogFile.WriteLine(". tilesX= " + _tilesX);

				int height = 0;
				if (SpritePack != null)
					height = (SpritePack.Count / _tilesX + 2) * _spriteHeight;

				LogFile.WriteLine(". height= " + height);
				LogFile.WriteLine(". ClientHeight= " + FindForm().ClientSize.Height);
				return height;
			}
		}

		private int _forceLargeChange = 43;
		#endregion


		#region cTor
		internal ViewPanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);


			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
//			_scrollBar.LargeChange = 44; // NOTE: this won't stick unless Visible, perhaps. else "1"
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			_statusTileSelected = new StatusBarPanel();
			_statusTileSelected.AutoSize = StatusBarPanelAutoSize.Contents;

			_statusTileOver = new StatusBarPanel();
			_statusTileOver.AutoSize = StatusBarPanelAutoSize.Contents;

			_statusBar = new StatusBar();
			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.ShowPanels = true;
			_statusBar.Panels.Add(_statusTileSelected);
			_statusBar.Panels.Add(_statusTileOver);

			Controls.AddRange(new Control[]
			{
				_scrollBar,
				_statusBar
			});

//			OnResize(null);
		}
		#endregion


		#region EventCalls

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnScrollBarValueChanged");

//			_scrollBar.Maximum = Math.Max(AbstractHeight + _scrollBar.LargeChange - Height, 0);

			_startY = -_scrollBar.Value;
			LogFile.WriteLine(". startY= " + _startY);

			Refresh(); // TODO: might not be needed.
		}

		protected override void OnResize(EventArgs eventargs)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnResize");

			base.OnResize(eventargs);

//			_tilesX = Math.Max((Width - 1 - (_scrollBar.Visible ? _scrollBar.Width : 0)) / SpriteWidth, 1);
//			_scrollBar.Maximum = Math.Max(AbstractHeight + _scrollBar.LargeChange - Height, 0);
//			_scrollBar.Visible = (_scrollBar.Maximum != 0);

			UpdateScrollbar(false);
		}

		/// <summary>
		/// Updates the scrollbar after a resize event or a sprite-pack changed
		/// event.
		/// </summary>
		/// <param name="resetTrack">true to set the thingie to the top of the track</param>
		private void UpdateScrollbar(bool resetTrack)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("UpdateScrollbar");

			if (SpritePack != null)
			{
				if (resetTrack)
					_scrollBar.Value = 0;

				_scrollBar.Maximum = Math.Max(AbstractHeight
											+ _scrollBar.LargeChange
											+ _forceLargeChange
											- _statusBar.Height
											- Height, 0);
				_forceLargeChange = 0; // fu.net - if LargeChange can't stick on the 1st pass we'll do it the hard way.

				if (_scrollBar.Maximum >= Height)
					_scrollBar.Maximum = 0;
			}
			else
				_scrollBar.Maximum = 0;

			//LogFile.WriteLine(". Maximum= " + _scrollBar.Maximum);

			_scrollBar.Visible = (_scrollBar.Maximum != 0);
		}

		internal void SetTilesX()
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("SetTilesX");

			int tilesX = 1;

			if (SpritePack != null && SpritePack.Count != 0)
			{
				int widthSprite = SpritePack.ImageFile.ImageSize.Width + ViewPanel.SpriteMargin * 2;
				tilesX = (Width - 1) / widthSprite; // calculate without widthScroll first

				if (tilesX > SpritePack.Count)
					tilesX = SpritePack.Count;

				if (tilesX * widthSprite + _scrollBar.Width > Width - 1)
				{
					int heightSprite = SpritePack.ImageFile.ImageSize.Height + ViewPanel.SpriteMargin * 2;
					if ((SpritePack.Count / tilesX + 2) * heightSprite > Height - _statusBar.Height - 1)
						--tilesX;
				}
			}
			_tilesX = tilesX;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
//			base.OnMouseDown(e);

			if (SpritePack != null)
			{
				int tileX =  e.X            / _spriteWidth;
				int tileY = (e.Y - _startY) / _spriteHeight;

				if (tileX >= _tilesX)
					tileX =  _tilesX - 1;

				int spriteId = tileX + tileY * _tilesX;

				var selected   = new SpriteSelected();
				selected.X     = tileX;
				selected.Y     = tileY;
				selected.Id    = spriteId;
				selected.Image = SpritePack[spriteId];

				if (spriteId < SpritePack.Count)
				{
					if (ModifierKeys == Keys.Control)
					{
						SpriteSelected spritePre = null;

						foreach (var sprite in _selectedSprites)
						{
							if (sprite.X == tileX && sprite.Y == tileY)
								spritePre = sprite;
						}

						if (spritePre != null)
						{
							_selectedSprites.Remove(spritePre);
						}
						else
							_selectedSprites.Add(selected);
					}
					else
					{
						_selectedSprites.Clear();
						_selectedSprites.Add(selected);
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

			if (SpritePack != null)
			{
				int tileX =  e.X            / _spriteWidth;
				int tileY = (e.Y - _startY) / _spriteHeight;

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

		/// <summary>
		/// Scrolls the Overlay-panel with the mousewheel after OnSpriteClick
		/// has given it focus (see).
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);

			if (_scrollBar.Visible)
			{
				int delta = _spriteHeight;

				if (e.Delta > 0)
				{
					if (_scrollBar.Value < delta)
					{
						_scrollBar.Value =
						_startY          = 0;
					}
					else
					{
						_scrollBar.Value -= delta;
						_startY          += delta;
					}
				}
				else if (e.Delta < 0)
				{
					if (_scrollBar.Maximum - _scrollBar.Value < delta)
					{
						_scrollBar.Value =  _scrollBar.Maximum;
						_startY          = -_scrollBar.Maximum;
					}
					else
					{
						_scrollBar.Value += delta;
						_startY          -= delta;
					}
				}
			}
		}

		private void OnSpriteOver(int spriteId)
		{
			_idOver = spriteId;

			string selected = (_idSelected != -1) ? _idSelected.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";
			string over     = (_idOver != -1)     ? _idOver.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";

			_statusTileSelected.Text = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"Selected {0}  Over {1}",
												selected, over);
			// TODO: _statusTileOver =

			Refresh();
		}

		private void OnSpriteClick(int spriteId)
		{
			_idSelected = spriteId;

			string selected = (_idSelected != -1) ? _idSelected.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";
			string over     = (_idOver != -1)     ? _idOver.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";

			_statusTileSelected.Text = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"Selected {0}  Over {1}",
												selected, over);
			// TODO: _statusTileOver =

			if (spriteId != -1)
			{
				Focus();

				if (SpriteClickEvent != null)
					SpriteClickEvent(spriteId);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (SpritePack != null && SpritePack.Count != 0)
			{
				var g = e.Graphics;

				for (int tileX = 0; tileX <= _tilesX; ++tileX) // draw vertical lines
					g.DrawLine(
							Pens.Black,
							new Point(tileX * _spriteWidth,          _startY),
							new Point(tileX * _spriteWidth, Height - _startY));

				int tilesY = SpritePack.Count / _tilesX;
				if (SpritePack.Count % _tilesX != 0) ++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					g.DrawLine(
							Pens.Black,
							new Point(0,                      tileY * _spriteHeight + _startY),
							new Point(_spriteWidth * _tilesX, tileY * _spriteHeight + _startY));


				var selected = new List<int>();
				foreach (var sprite in _selectedSprites)
					selected.Add(sprite.Id);

				for (int id = 0; id != SpritePack.Count; ++id) // fill selected tiles and draw sprites.
				{
					int tileX = id % _tilesX;
					int tileY = id / _tilesX;

					if (selected.Contains(id))
						g.FillRectangle(
									Brushes.Crimson,
									tileX * _spriteWidth  + 1,
									tileY * _spriteHeight + 1 + _startY,
									_spriteWidth  - 1,
									_spriteHeight - 1);

					g.DrawImage(
							SpritePack[id].Image,
							tileX * _spriteWidth  + SpriteMargin,
							tileY * _spriteHeight + SpriteMargin + _startY);
				}
			}
		}
		#endregion


		#region Methods
		internal void SpriteReplace(int id, XCImage image)
		{
			SpritePack[id] = image;
		}

		/// <summary>
		/// Deletes the currently selected sprite.
		/// </summary>
		internal void SpriteDelete()
		{
			if (_selectedSprites.Count != 0)
			{
				var lowestId = int.MaxValue;

				var idList = new List<int>();
				foreach (var sprite in _selectedSprites)
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

				_selectedSprites.Clear();
	
				if (SpritePack.Count != 0)
				{
					var selected = new SpriteSelected();
					selected.Y   = lowestId / _tilesX;
					selected.X   = lowestId - selected.Y;
					selected.Id  = selected.X + selected.Y * _tilesX;
	
					_selectedSprites.Add(selected);
				}
			}
		}
		#endregion

/*		private void tileChooser_SelectedIndexChanged(object sender, EventArgs e)
		{
//			view.Pck = ImageCollection.GetPckFile(tileChooser.SelectedItem.ToString());
			view.Refresh();
//			_scrollBar.Maximum = Math.Max((view.Height - Height + tileChooser.Height + 50), _scrollBar.Minimum);
			_scrollBar.Value = _scrollBar.Minimum;
			scroll_Scroll(null, null);
		} */

/*		internal void Hq2x()
		{
			_collection.HQ2X();
		} */
/*		internal void Hq2x()
		{
			_viewPanel.Hq2x();
		} */

/*		/// <summary>
		/// Saves a bitmap as an 8-bit image.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="pal"></param>
		public void SaveBMP(string file, Palette pal)
		{
			Bmp.SendToSaver(file, _collection, pal, numAcross(), 1);
		} */
	}



	#region SpritePackChanged event handler & args

	/// <summary>
	/// EventArgs for SpritePackChangedEvent.
	/// </summary>
	internal sealed class SpritePackChangedEventArgs
	{
		internal XCImageCollection Sprites
		{ get; private set; }


		internal SpritePackChangedEventArgs(XCImageCollection sprites)
		{
			Sprites = sprites;
		}
	}
	#endregion
}
