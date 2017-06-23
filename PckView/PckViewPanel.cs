using System;
using System.Collections.Generic;		// List
using System.Collections.ObjectModel;	// ReadOnlyCollection
using System.Drawing;					// Pens, Brushes
using System.Drawing.Drawing2D;
using System.Windows.Forms;				// Panel

using XCom;								// Palette, XCImageCollection
using XCom.Interfaces;					// XCImage


namespace PckView
{
	internal delegate void SpritePackChangedEventHandler(bool valid);


	internal sealed class PckViewPanel
		:
			Panel
	{
		#region Events
		internal event SpritePackChangedEventHandler SpritesetChangedEvent;
		#endregion


		#region Fields (static)
		private const int SpriteMargin = 2;

		private const string None = "n/a";

		// NOTE: if sprite-size is ever allowed to change these need to be replaced
		// w/ 'Spriteset.ImageFile.ImageSize.Width/Height' here and elsewhere.
		private const int _tileWidth  = XCImageFile.SpriteWidth  + SpriteMargin * 2 + 1;
		private const int _tileHeight = XCImageFile.SpriteHeight + SpriteMargin * 2 + 1;

		private const int TableOffsetHori = 3;
		private const int TableOffsetVert = 2;
		#endregion


		#region Fields
		private readonly VScrollBar _scrollBar = new VScrollBar();
		private readonly StatusBar  _statusBar = new StatusBar();

		private readonly StatusBarPanel _sbpTilesTotal   = new StatusBarPanel();
		private readonly StatusBarPanel _sbpTileSelected = new StatusBarPanel();
		private readonly StatusBarPanel _sbpTileOver     = new StatusBarPanel();
		private readonly StatusBarPanel _sbpSpritesLabel = new StatusBarPanel();

		private int _tilesX = 1;

		private int _startY;

		private int _selectedId;
		private int _overId;

		private Pen   _penBlack        = new Pen(Brushes.Black, 1);
		private Pen   _penControlLight = new Pen(SystemColors.ControlLight, 1);
		private Brush _brushCrimson    = new SolidBrush(Color.Crimson);

		/// <summary>
		/// The LargeChange value for the scrollbar will return "1" when the bar
		/// isn't visible. Therefore this value needs to be used instead of the
		/// actual LargeValue in order to calculate the panel's various dynamics.
		/// </summary>
		private int _largeChange;
		#endregion


		#region Properties (static)
		internal static PckViewPanel Instance
		{ get; set; }
		#endregion


		#region Properties
		private SpriteCollectionBase _spriteset;
		internal SpriteCollectionBase Spriteset
		{
			get { return _spriteset; }
			set
			{
				_spriteset = value;

				_spriteset.Pal = PckViewForm.Pal;

				Selected.Clear();

				_largeChange           =
				_scrollBar.LargeChange = _tileHeight;

				UpdateScrollbar(true);

				Refresh();

				_sbpSpritesLabel.Text = _spriteset.Label;
				PrintStatusTotal();

				if (SpritesetChangedEvent != null)
					SpritesetChangedEvent(value != null);

				EditorPanel.Instance.Sprite = null;
				// TODO: update PaletteViewer if the spriteset palette changes.
			}
		}

		private readonly List<SelectedSprite> _selected = new List<SelectedSprite>();
		internal List<SelectedSprite> Selected
		{
			get { return _selected; }
		}

		/// <summary>
		/// Used by UpdateScrollBar() to determine its Maximum value.
		/// </summary>
		internal int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				SetTilesX();

				int height = 0;
				if (Spriteset != null)
					height = (Spriteset.Count / _tilesX + 2) * _tileHeight;

				return height;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PckViewPanel()
		{
#if DEBUG
			LogFile.SetLogFilePath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)); // creates a logfile/ wipes the old one.
#endif
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			_sbpTilesTotal.Text = String.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"Total " + None);
			_sbpTilesTotal.Width   = 85;
			_sbpTileSelected.Width = 100;
			_sbpTileOver.Width     = 75;

			_sbpSpritesLabel.AutoSize = StatusBarPanelAutoSize.Spring;
			_sbpSpritesLabel.Alignment = HorizontalAlignment.Center;

			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.ShowPanels = true;
			_statusBar.Panels.Add(_sbpTilesTotal);
			_statusBar.Panels.Add(_sbpTileSelected);
			_statusBar.Panels.Add(_sbpTileOver);
			_statusBar.Panels.Add(_sbpSpritesLabel);

			Controls.AddRange(new Control[]
			{
				_scrollBar,
				_statusBar
			});

			UpdateSpriteClick(-1);
			UpdateSpriteOver( -1);

			PckViewForm.PaletteChangedEvent += OnPaletteChanged; // NOTE: lives the life of the app, so no leak.

			Instance = this;
		}
		#endregion


		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		#region Eventcalls (override)
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (FindForm().WindowState != FormWindowState.Minimized)
			{
				UpdateScrollbar(false);

				if (Selected.Count != 0)
					ScrollToTile(Selected[0].Sprite.TerrainId);
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
		/// Selects and shows status-information for a sprite. Overrides core
		/// implementation for the MouseDown event.
		/// NOTE: This fires before PckViewForm.OnSpriteClick().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
//			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left
				&& Spriteset != null && Spriteset.Count != 0)
			{
				// IMPORTANT: 'Selected' is currently allowed only 1 entry,
				// although it is set up as a List. Things might go one way or
				// the other in future.

				int terrainId = GetTileId(e);
				if (terrainId != _selectedId)
				{
					Selected.Clear();

					if (terrainId != -1)
					{
						var selected = new SelectedSprite();
						selected.Sprite = Spriteset[terrainId];

						Selected.Add(selected);

						terrainId = selected.Sprite.TerrainId; // use the proper Id.


//						if (ModifierKeys == Keys.Control)
//						{
//							SpriteSelected spritePre = null;
//							foreach (var sprite in _selectedSprites)
//								if (sprite.X == tileX && sprite.Y == tileY)
//									spritePre = sprite;
//							if (spritePre != null)
//								_selectedSprites.Remove(spritePre);
//							else
//								_selectedSprites.Add(selected);
//						}
//						else
//							Selected.Add(selected);
					}

					UpdateSpriteClick(terrainId);
					Refresh();
				}
				ScrollToTile(terrainId);
			}
		}

		/// <summary>
		/// Shows status-information for a sprite. Overrides core implementation
		/// for the MouseMove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

			if (Spriteset != null && Spriteset.Count != 0)
			{
				int terrainId = GetTileId(e);
				if (terrainId != _overId)
					UpdateSpriteOver(terrainId);
			}
		}

		/// <summary>
		/// Gets the terrain-id of a sprite at coordinates x/y.
		/// </summary>
		/// <param name="e"></param>
		/// <returns>the terrain-id or -1 if out of bounds</returns>
		private int GetTileId(MouseEventArgs e)
		{
			if (e.X < _tilesX * _tileWidth + TableOffsetHori - 1) // not out of bounds to right
			{
				int tileX = (e.X - TableOffsetHori + 1)           / _tileWidth;
				int tileY = (e.Y - TableOffsetHori + 1 - _startY) / _tileHeight;

				int terrainId = _tilesX * tileY + tileX;
				if (terrainId < Spriteset.Count) // not out of bounds below
					return terrainId;
			}
			return -1;
		}

		/// <summary>
		/// Let's draw this puppy.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (Spriteset != null && Spriteset.Count != 0)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;


				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					graphics.DrawLine(
									_penControlLight,
									Width - _scrollBar.Width - 1, 0,
									Width - _scrollBar.Width - 1, Height);


				var selectedIds = new List<int>(); // track currently selected spriteIds.
				foreach (var sprite in Selected)
					selectedIds.Add(sprite.Sprite.TerrainId);

				for (int id = 0; id != Spriteset.Count; ++id) // fill selected tiles and draw sprites.
				{
					int tileX = id % _tilesX;
					int tileY = id / _tilesX;

					if (selectedIds.Contains(id))
						graphics.FillRectangle(
											_brushCrimson,
											TableOffsetHori + _tileWidth  * tileX,
											TableOffsetVert + _tileHeight * tileY + _startY,
											TableOffsetHori + _tileWidth  - SpriteMargin * 2,
											TableOffsetVert + _tileHeight - SpriteMargin - 1);

					graphics.DrawImage(
									Spriteset[id].Image,
									TableOffsetHori + tileX * _tileWidth  + SpriteMargin,
									TableOffsetVert + tileY * _tileHeight + SpriteMargin + _startY);
				}


				graphics.FillRectangle(
									new SolidBrush(_penBlack.Color),
									TableOffsetHori - 1,
									TableOffsetVert + _startY - 1,
									1, 1); // so bite me.

				for (int tileX = 0; tileX <= _tilesX; ++tileX) // draw vertical lines
					graphics.DrawLine(
									_penBlack,
									new Point(
											TableOffsetHori + _tileWidth * tileX,
											TableOffsetVert + _startY),
									new Point(
											TableOffsetHori + _tileWidth * tileX,
											TableOffsetVert - _startY + Height));

				int tilesY = Spriteset.Count / _tilesX;
				if (Spriteset.Count % _tilesX != 0)
					++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					graphics.DrawLine(
									_penBlack,
									new Point(
											TableOffsetHori,
											TableOffsetVert + _tileHeight * tileY + _startY),
									new Point(
											TableOffsetHori + _tileWidth * _tilesX,
											TableOffsetVert + _tileHeight * tileY + _startY));
			}
		}
		#endregion


		#region Eventcalls
		private void OnPaletteChanged()
		{
			Refresh();
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			_startY = -_scrollBar.Value;
			Refresh();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Checks if a selected tile is fully visible in the view-panel and
		/// scrolls the table to show it if not.
		/// </summary>
		/// <param name="terrainId"></param>
		private void ScrollToTile(int terrainId)
		{
			if (terrainId != -1)
			{
				int tileY = terrainId / _tilesX;

				int cutoff = _tileHeight * tileY;
				if (cutoff < -_startY)		// <- check cutoff high
				{
					_scrollBar.Value = cutoff;
				}
				else						// <- check cutoff low
				{
					cutoff = _tileHeight * (tileY + 1) - Height + _statusBar.Height + TableOffsetVert + 1;
					if (cutoff > -_startY)
					{
						_scrollBar.Value = cutoff;
					}
				}
			}
		}

		/// <summary>
		/// Updates the scrollbar after a resize event or a spriteset changed
		/// event.
		/// </summary>
		/// <param name="resetTrack">true to set the thing to the top of the track</param>
		private void UpdateScrollbar(bool resetTrack)
		{
			int range = 0;
			if (Spriteset != null && Spriteset.Count != 0)
			{
				if (resetTrack)
					_scrollBar.Value = 0;

				range = TableOffsetVert + TableHeight + _largeChange - Height - _statusBar.Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = (range != 0);
		}

		internal void SetTilesX()
		{
			int tilesX = 1;

			if (Spriteset != null && Spriteset.Count != 0)
			{
				tilesX = (Width - TableOffsetHori - _scrollBar.Width - 1) / _tileWidth;

				if (tilesX > Spriteset.Count)
					tilesX = Spriteset.Count;
			}
			_tilesX = tilesX;
		}

		/// <summary>
		/// Prints the quantity of sprites in the currently loaded spriteset to
		/// the statusbar. Note that this will also clear the sprite-clicked and
		/// sprite-over info.
		/// </summary>
		internal void PrintStatusTotal()
		{
			_sbpTilesTotal.Text = String.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"Total {0}", _spriteset.Count);
			UpdateSpriteClick(-1);
			UpdateSpriteOver( -1);
		}

		/// <summary>
		/// Updates the status-information for the sprite that the cursor is
		/// currently over.
		/// </summary>
		/// <param name="terrainId">the id of the currently mouse-overed sprite
		/// in the currently loaded spriteset</param>
		private void UpdateSpriteOver(int terrainId)
		{
			_overId = terrainId;
			PrintStatusSpriteIds();
		}

		/// <summary>
		/// Updates the status-information for the sprite that is currently
		/// selected.
		/// </summary>
		/// <param name="terrainId">the id of the currently mouse-clicked sprite
		/// in the currently loaded spriteset</param>
		internal void UpdateSpriteClick(int terrainId)
		{
			_selectedId = terrainId;
			PrintStatusSpriteIds();
		}

		/// <summary>
		/// Prints the current status for the currently selected and/or
		/// mouseovered sprite(s) in the status-bar. Not 0-based.
		/// </summary>
		private void PrintStatusSpriteIds()
		{
			int selectedId = _selectedId + 1;
			string selected = (selectedId != 0) ? selectedId.ToString(System.Globalization.CultureInfo.InvariantCulture)
												: None;
			_sbpTileSelected.Text = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"Selected {0}", selected);

			int overId = _overId + 1;
			string over = (overId != 0) ? overId.ToString(System.Globalization.CultureInfo.InvariantCulture)
										: None;
			_sbpTileOver.Text = String.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"Over {0}", over);
		}
		#endregion
	}
}

//		/// <summary>
//		/// Deletes the currently selected sprite and selects another one.
//		/// </summary>
//		internal void SpriteDelete()
//		{
//			if (Selected.Count != 0)
//			{
//				var lowestId = Int32.MaxValue;
//
//				var selectedIds = new List<int>();
//				foreach (var sprite in Selected)
//					selectedIds.Add(sprite.Id);
//
//				selectedIds.Sort();
//				selectedIds.Reverse();
//
//				foreach (var id in selectedIds)
//				{
//					if (id < lowestId)
//						lowestId = id;
//
//					Spriteset.Remove(id);
//				}
//
//				if (lowestId > 0 && lowestId == Spriteset.Count)
//					lowestId = Spriteset.Count - 1;
//
//				Selected.Clear();
//	
//				if (Spriteset.Count != 0)
//				{
//					var selected = new SelectedSprite();
//					selected.Y   = lowestId / _tilesX;
//					selected.X   = lowestId - selected.Y;
//					selected.Id  = selected.X + selected.Y * _tilesX;
//	
//					Selected.Add(selected);
//				}
//			}
//		}

//		internal void Hq2x()
//		{
//			_collection.HQ2X();
//		}
//		internal void Hq2x()
//		{
//			_viewPanel.Hq2x();
//		}
