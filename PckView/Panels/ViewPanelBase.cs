using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class ViewPanelBase
		:
			Panel
	{
		internal event SpritePackChangedEventHandler SpritePackChangedEvent;

		internal event SpriteClickEventHandler SpriteClickEvent;


		#region Fields & Properties

		private readonly ViewPanelOverlay _viewPanelOverlay;
		internal ViewPanelOverlay Overlay
		{
			get { return _viewPanelOverlay; }
		}

		private VScrollBar _scrollBar;
		private StatusBar _statusBar;
		private StatusBarPanel _statusOverTile;

		private int _selectedId;
		private int _overId;

		internal Palette Pal
		{
			get { return (_viewPanelOverlay != null) ? _viewPanelOverlay.Pal
													 : null; }
			set
			{
				if (_viewPanelOverlay != null)
					_viewPanelOverlay.Pal = value;
			}
		}

		internal ReadOnlyCollection<SpriteSelected> Sprites
		{
			get { return _viewPanelOverlay.Sprites; }
		}

		internal XCImageCollection SpritePack
		{
			get { return _viewPanelOverlay.SpritePack; }
			set
			{
				_viewPanelOverlay.SpritePack = value; // NOTE: this value shall never be null.

				if (SpritePackChangedEvent != null)
					SpritePackChangedEvent(new SpritePackChangedEventArgs(value));
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ViewPanelBase()
		{
//			this.Layout;

			_viewPanelOverlay = new ViewPanelOverlay();
			_viewPanelOverlay.Dock = DockStyle.Fill;
			_viewPanelOverlay.SpriteClickEvent += OnSpriteClick;
			_viewPanelOverlay.SpriteOverEvent += OnSpriteOver;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.Scroll += OnSpritesScroll;

			_statusBar = new StatusBar();
			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.ShowPanels = true;

			_statusOverTile = new StatusBarPanel();
			_statusOverTile.AutoSize = StatusBarPanelAutoSize.Contents;
			_statusBar.Panels.Add(_statusOverTile);

			Controls.AddRange(new Control[]
			{
				_viewPanelOverlay,
				_scrollBar,
				_statusBar
			});

			OnResize(null);
		}
		#endregion



		#region EventCalls

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			_scrollBar.Value         =
			_viewPanelOverlay.StartY = 0;

			UpdateScrollbar();
		}

		/// <summary>
		/// Scrolls the sprites.
		/// NOTE: The .NET OnScroll() method doesn't work too well here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSpritesScroll(object sender, ScrollEventArgs e)
		{
			_viewPanelOverlay.StartY = -_scrollBar.Value;
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
						_scrollBar.Value         =
						_viewPanelOverlay.StartY = 0;
					}
					else
					{
						_scrollBar.Value         -= delta;
						_viewPanelOverlay.StartY += delta;
					}
				}
				else if (e.Delta < 0)
				{
					if (_scrollBar.Maximum - _scrollBar.Value < delta)
					{
						_scrollBar.Value         =  _scrollBar.Maximum;
						_viewPanelOverlay.StartY = -_scrollBar.Maximum;
					}
					else
					{
						_scrollBar.Value         += delta;
						_viewPanelOverlay.StartY -= delta;
					}
				}
			}
		}

		private void OnSpriteClick(int spriteId)
		{
			_viewPanelOverlay.Focus();

			_selectedId = spriteId;
			_statusOverTile.Text = "Selected: " + _selectedId + " Over: " + _overId;

			if (SpriteClickEvent != null)
				SpriteClickEvent(spriteId);
		}

		private void OnSpriteOver(int spriteId)
		{
			_overId = spriteId;
			_statusOverTile.Text = "Selected: " + _selectedId + " Over: " + _overId;
		}
		#endregion


		#region Methods

		internal void UpdateScrollbar()
		{
			if (_viewPanelOverlay.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = _viewPanelOverlay.PreferredHeight - Height;
			}
			else
				_scrollBar.Visible = false;
		}

		internal void ChangeSprite(int id, XCImage image)
		{
			_viewPanelOverlay.ChangeSprite(id, image);
		}

		internal void RemoveSelected()
		{
			_viewPanelOverlay.RemoveSelected();
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
