using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using PckView.Args;
using PckView.Panels;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class PckViewPanel0
		:
			Panel
	{
		internal event MouseClickedEventHandler MouseClickedEvent;
		internal event ImageCollectionSetHandler ImageCollectionChangedEvent;


		private readonly PckViewPanel1 _viewPanel_1;
		internal PckViewPanel1 ViewPanel
		{
			get { return _viewPanel_1; }
		}

		private VScrollBar _scrollBar;
		private StatusBar _statusBar;
		private StatusBarPanel _statusOverTile;
		private StatusBarPanel _statusBPP;

		private int _click;
		private int _move;

		internal Palette Pal
		{
			get { return _viewPanel_1 != null ? _viewPanel_1.Pal
											  : null; }
			set
			{
				if (_viewPanel_1 != null)
					_viewPanel_1.Pal = value;
			}
		}


		internal PckViewPanel0()
		{
			_scrollBar = new VScrollBar();

			_statusBar = new StatusBar();
			_statusOverTile = new StatusBarPanel();
			_statusOverTile.AutoSize = StatusBarPanelAutoSize.Spring;
			_statusBar.Panels.Add(_statusOverTile);
			_statusBar.ShowPanels = true;
			_statusBar.Dock = DockStyle.Bottom;

			_statusBPP = new StatusBarPanel();
			_statusBPP.AutoSize = StatusBarPanelAutoSize.Contents;
			_statusBPP.Width = 50;
			_statusBPP.Alignment = HorizontalAlignment.Right;
			_statusBar.Panels.Add(_statusBPP);

			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.Minimum = 0;
			_scrollBar.Maximum = 5000;
			_scrollBar.Scroll += this.OnScroll;

			_viewPanel_1 = new PckViewPanel1();
			_viewPanel_1.Location = new Point(0, 0);
			_viewPanel_1.MouseClickedEvent += OnViewClick0;
			_viewPanel_1.MouseClickedEvent += OnViewClick1;
			_viewPanel_1.MouseMovedEvent += OnViewMoved;
			_viewPanel_1.Dock = DockStyle.Fill;

			this.Controls.AddRange(new Control[] { _statusBar, _scrollBar, _viewPanel_1 });

			_viewPanel_1.SizeChanged += OnSizeChanged;
			OnResize(null);
		}


		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (_viewPanel_1.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = _viewPanel_1.PreferredHeight - Height + 50;
			}
			else
				_scrollBar.Visible = false;
		}

		internal ReadOnlyCollection<SpriteSelected> Sprites
		{
			get { return _viewPanel_1.Sprites; }
		}

		internal void ChangeSprite(int index, XCImage image)
		{
			_viewPanel_1.ChangeSprite(index, image);
		}

		internal XCImageCollection SpritePack
		{
			get { return _viewPanel_1.SpritePack; }
			set
			{
				_viewPanel_1.SpritePack = value;
				try
				{
					if (value is PckSpriteCollection)
					{
						_statusBPP.Text = "Bpp: " + ((PckSpriteCollection)_viewPanel_1.SpritePack).Bpp;
					}
					else
						_statusBPP.Text = String.Empty;

					if (ImageCollectionChangedEvent != null)
						ImageCollectionChangedEvent(this, new ImageCollectionChangedEventArgs(value));
				}
				catch (Exception ex)
				{
					if (ImageCollectionChangedEvent != null)
						ImageCollectionChangedEvent(this, new ImageCollectionChangedEventArgs(null));

					throw ex;
				}
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			_scrollBar.Value = _scrollBar.Minimum;
			_viewPanel_1.StartY = -_scrollBar.Value;
			if (_viewPanel_1.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = _viewPanel_1.PreferredHeight - Height;
			}
			else
				_scrollBar.Visible = false;
		}

/*		private void tileChooser_SelectedIndexChanged(object sender, EventArgs e)
		{
//			view.Pck = ImageCollection.GetPckFile(tileChooser.SelectedItem.ToString());
			view.Refresh();
//			_scrollBar.Maximum = Math.Max((view.Height - Height + tileChooser.Height + 50), _scrollBar.Minimum);
			_scrollBar.Value = _scrollBar.Minimum;
			scroll_Scroll(null, null);
		} */

		private void OnScroll(object sender, ScrollEventArgs e)
		{
			_viewPanel_1.StartY = -_scrollBar.Value;
			_viewPanel_1.Refresh();
		}

		private void OnViewClick0(object sender, PckViewMouseEventArgs e)
		{
			_click = e.Clicked;
			_statusOverTile.Text = "Selected: " + _click + " Over: " + _move;
		}

		private void OnViewClick1(object sender, PckViewMouseEventArgs e)
		{
			if (MouseClickedEvent != null)
				MouseClickedEvent(sender, e);
		}

		private void OnViewMoved(int x)
		{
			_move = x;
			_statusOverTile.Text = "Selected: " + _click + " Over: " + _move;
		}

/*		internal void Hq2x()
		{
			_viewPanel.Hq2x();
		} */

		internal void RemoveSelected()
		{
			_viewPanel_1.RemoveSelected();
		}
	}


	/// <summary>
	/// EventArgs for ImageCollectionChanged.
	/// </summary>
	internal sealed class ImageCollectionChangedEventArgs
	{
		private readonly XCImageCollection _collection;

		internal ImageCollectionChangedEventArgs(XCImageCollection collection)
		{
			_collection = collection;
		}

		internal XCImageCollection Collection
		{
			get { return _collection; }
		}
	}

	internal delegate void ImageCollectionSetHandler(object sender, ImageCollectionChangedEventArgs e);
}
