using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class ViewPanelUnderlay
		:
			Panel
	{
		internal event SpritePackChangedEventHandler SpritePackChangedEvent;

		internal event SpriteClickEventHandler SpriteClickEvent;


		#region Fields & Properties

		private readonly ViewPanelOverlay _overlay;
		internal ViewPanelOverlay Overlay
		{
			get { return _overlay; }
		}

		private VScrollBar _scrollBar;
		private StatusBar _statusBar;
		private StatusBarPanel _statusOverTile;

		private int _selectedId;
		private int _overId;

		internal Palette Pal
		{
			get { return (_overlay != null) ? _overlay.Pal
											: null; }
			set
			{
				if (_overlay != null)
					_overlay.Pal = value;
			}
		}

		internal ReadOnlyCollection<SpriteSelected> SelectedSprites
		{
			get { return _overlay.SelectedSprites; }
		}

		internal XCImageCollection SpritePack
		{
			get { return _overlay.SpritePack; }
			set
			{
				_overlay.SpritePack = value; // NOTE: this value shall never be null.

				_scrollBar.LargeChange = value.ImageFile.ImageSize.Height + ViewPanelOverlay.SpriteMargin * 2;

				UpdateScrollbar(true);
				OnSpriteOver(-1);
				OnSpriteClick(-1);

				if (SpritePackChangedEvent != null)
					SpritePackChangedEvent(new SpritePackChangedEventArgs(value));
			}
		}

		private int _forceLargeChange = 43;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ViewPanelUnderlay()
		{
			_overlay = new ViewPanelOverlay();
			_overlay.Dock = DockStyle.Fill;
			_overlay.SpriteClickEvent += OnSpriteClick;
			_overlay.SpriteOverEvent  += OnSpriteOver;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
//			_scrollBar.LargeChange = SpritePack.ImageFile.ImageSize.Height + ViewPanelOverlay.SpriteMargin * 2;	// fu.net
																												// NOTE: LargeChange is set in the SpritePack setter.
//			_scrollBar.Scroll += OnSpritesScroll;																// Even there (or anywhere else apparently) it returns
			_scrollBar.ValueChanged += OnScrollBarValueChanged;													// a value of "1" until everything settles ....

			_statusOverTile = new StatusBarPanel();
			_statusOverTile.AutoSize = StatusBarPanelAutoSize.Contents;

			_statusBar = new StatusBar();
			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.ShowPanels = true;
			_statusBar.Panels.Add(_statusOverTile);

			Controls.AddRange(new Control[]
			{
				_overlay,
				_scrollBar,
				_statusBar
			});

			OnResize(null);
		}
		#endregion



		#region EventCalls

//		protected override void OnSizeChanged(EventArgs e)
//		{
//			LogFile.WriteLine("");
//			LogFile.WriteLine("Underlay.OnSizeChanged");
//
//			base.OnSizeChanged(e);
//////
//////			_scrollBar.Value         =
//////			_viewPanelOverlay.StartY = 0;
//////
////			UpdateScrollbar();
////
////			Refresh();
////			_viewPanelOverlay.Refresh();
//		}

		protected override void OnResize(EventArgs eventargs)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Underlay.OnResize");

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
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Underlay.UpdateScrollbar");

			if (SpritePack != null)
			{
				if (resetTrack)
					_scrollBar.Value = 0;

				SetTilesX();
				_scrollBar.Maximum = Math.Max(_overlay.AbstractHeight
											+ _scrollBar.LargeChange
											+ _forceLargeChange
											- _statusBar.Height
											- Height, 0);
				_forceLargeChange = 0; // fu.net - if LargeChange can't stick on the 1st pass we'll do it the hard way.
			}
			else
				_scrollBar.Maximum = 0;

			//LogFile.WriteLine(". Maximum= " + _scrollBar.Maximum);

			_scrollBar.Visible = (_scrollBar.Maximum != 0);
		}

		internal void SetTilesX()
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Underlay.SetTilesX");

			int tilesX = 1;

			if (SpritePack != null && SpritePack.Count != 0)
			{
				int widthSprite = SpritePack.ImageFile.ImageSize.Width + ViewPanelOverlay.SpriteMargin * 2;
				tilesX = (Width - 1) / widthSprite; // calculate without widthScroll first

				if (tilesX * widthSprite + _scrollBar.Width > Width - 1)
				{
					int heightSprite = SpritePack.ImageFile.ImageSize.Height + ViewPanelOverlay.SpriteMargin * 2;
					if ((SpritePack.Count / tilesX + 2) * heightSprite > Height - 1)
						--tilesX;
				}
			}
			_overlay.TilesX = tilesX;
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Underlay.OnScrollBarValueChanged");

//			_scrollBar.Maximum = Math.Max(_viewPanelOverlay.AbstractHeight + _scrollBar.LargeChange - Height, 0);

			_overlay.StartY = -_scrollBar.Value;
			//LogFile.WriteLine(". startY= " + _overlay.StartY);

			Refresh(); // TODO: might not be needed.
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
				const int delta = 18;

				if (e.Delta > 0)
				{
					if (_scrollBar.Value < delta)
					{
						_scrollBar.Value =
						_overlay.StartY  = 0;
					}
					else
					{
						_scrollBar.Value -= delta;
						_overlay.StartY  += delta;
					}
				}
				else if (e.Delta < 0)
				{
					if (_scrollBar.Maximum - _scrollBar.Value < delta)
					{
						_scrollBar.Value =  _scrollBar.Maximum;
						_overlay.StartY  = -_scrollBar.Maximum;
					}
					else
					{
						_scrollBar.Value += delta;
						_overlay.StartY  -= delta;
					}
				}
			}
		}

		private void OnSpriteOver(int spriteId)
		{
			_overId = spriteId;

			string selected = (_selectedId != -1) ? _selectedId.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";
			string over = (_overId != -1) ? _overId.ToString(System.Globalization.CultureInfo.InvariantCulture)
										  : "-";

			_statusOverTile.Text = String.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"Selected {0}  Over {1}",
											selected, over);
			Refresh();
		}

		private void OnSpriteClick(int spriteId)
		{
			_selectedId = spriteId;

			string selected = (_selectedId != -1) ? _selectedId.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : "-";
			string over = (_overId != -1) ? _overId.ToString(System.Globalization.CultureInfo.InvariantCulture)
										  : "-";

			_statusOverTile.Text = String.Format(
											System.Globalization.CultureInfo.InvariantCulture,
											"Selected {0}  Over {1}",
											selected, over);

			if (spriteId != -1)
			{
				_overlay.Focus();

				if (SpriteClickEvent != null)
					SpriteClickEvent(spriteId);
			}
		}

		#endregion


		#region Methods

		/// <summary>
		/// Forwards a sprite change from PckViewForm to the Overlay panel.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="image"></param>
		internal void ChangeSprite(int id, XCImage image)
		{
			_overlay.ChangeSprite(id, image);
		}

		/// <summary>
		/// Forwards a sprite removed call from PckViewForm to the Overlay panel.
		/// </summary>
		internal void RemoveSelected()
		{
			_overlay.RemoveSelected();
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
			_viewPanel.Hq2x();
		} */
	}


	#region SpritePackChanged event handler & args
	internal delegate void SpritePackChangedEventHandler(SpritePackChangedEventArgs e);

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
