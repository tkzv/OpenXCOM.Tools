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

//				LogFile.WriteLine("Under.SpritePack LargeChange= " + _scrollBar.LargeChange);
//				LogFile.WriteLine(". imageHeight= " + value.ImageFile.ImageSize.Height);
//				LogFile.WriteLine(". spriteMargin= " + ViewPanelOverlay.SpriteMargin);

				UpdateScrollbar(true);
				ClearSpriteStatus();

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
			_overlay.SpriteOverEvent += OnSpriteOver;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;

//			_scrollBar.LargeChange = SpritePack.ImageFile.ImageSize.Height + ViewPanelOverlay.SpriteMargin * 2; // fu.net
//			_scrollBar.LargeChange = 40 + ViewPanelOverlay.SpriteMargin * 2;
			// you can set LargeChange here but on init it will still be 1.
			
//			_scrollBar.Scroll += OnSpritesScroll;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

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
//			LogFile.WriteLine("Base.OnSizeChanged");
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

		internal void ForceOnResize()
		{
			OnResize(null);
		}

		protected override void OnResize(EventArgs eventargs) // kL
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Base.OnResize");

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
		/// <param name="resetTrack">i don't want to talk about it.</param>
		internal void UpdateScrollbar(bool resetTrack)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Base.UpdateScrollbar");

			if (SpritePack != null)
			{
//				_scrollBar.LargeChange = SpritePack.ImageFile.ImageSize.Height + ViewPanelOverlay.SpriteMargin * 2; // fu.net
				// no, you can't set LargeChange here.

				if (resetTrack)
					_scrollBar.Value = 0;

				_scrollBar.Maximum = Math.Max(_overlay.AbstractHeight + _scrollBar.LargeChange + _forceLargeChange - Height - _statusBar.Height, 0);
				_forceLargeChange = 0;
			}
			else
				_scrollBar.Maximum = 0;

			//LogFile.WriteLine(". Maximum= " + _scrollBar.Maximum);

			_scrollBar.Visible = (_scrollBar.Maximum != 0);
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e) // kL
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("Base.OnScrollBarValueChanged");

//			_scrollBar.Maximum = Math.Max(_viewPanelOverlay.AbstractHeight + _scrollBar.LargeChange - Height, 0); // TODO: might not be needed.

			// That is needed only for initialization.
			// OnResize, which also sets '_scrollBar.Maximum', fires a dozen
			// times during init, but it gets the value right only once (else
			// '0') and not on the last call either. So just do it here and
			// marvel at the wonders of c#/.NET

			_overlay.StartY = -_scrollBar.Value;
			//LogFile.WriteLine(". startY= " + _overlay.StartY);

			Refresh(); // TODO: might not be needed.
		}

//		/// <summary>
//		/// Scrolls the sprites.
//		/// NOTE: The .NET OnScroll() method doesn't work too well here.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void OnSpritesScroll(object sender, ScrollEventArgs e)
//		{
//			_viewPanelOverlay.StartY = -_scrollBar.Value;
//		}

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

		internal void ClearSpriteStatus()
		{
			OnSpriteOver(-1);
			OnSpriteClick(-1);
		}

		#endregion


		#region Methods

		internal void ChangeSprite(int id, XCImage image)
		{
			_overlay.ChangeSprite(id, image);
		}

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
}
