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
	internal sealed class TotalViewPck
		:
			Panel
	{
		private readonly ViewPck _viewPanel;
		internal ViewPck ViewPanel
		{
			get { return _viewPanel; }
		}

		private VScrollBar _scrollBar;
		private StatusBar _statusBar;
		private StatusBarPanel _statusOverTile;
		private StatusBarPanel _statusBPP;

		private int _click;
		private int _move;

		internal event MouseClickedEventHandler MouseClickedEvent;
		internal event ImageCollectionSetHandler ImageCollectionSetEvent;


		internal TotalViewPck()
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
			_scrollBar.Maximum = 5000;
			_scrollBar.Scroll += this.OnScroll;

			_viewPanel = new ViewPck();
			_viewPanel.Location = new Point(0, 0);
			_viewPanel.MouseClickedEvent += OnViewClick0;
			_viewPanel.MouseClickedEvent += OnViewClick1;
			_viewPanel.MouseMovedEvent += OnViewMoved;
			_viewPanel.Dock = DockStyle.Fill;
			_scrollBar.Minimum = 0;

			this.Controls.AddRange(new Control[] { _statusBar, _scrollBar, _viewPanel });

			_viewPanel.SizeChanged += OnSizeChanged;
			OnResize(null); // FIX: "Virtual member call in constructor."
		}

		internal Palette Pal
		{
			get { return _viewPanel != null ? _viewPanel.Pal : null; }
			set
			{
				if (_viewPanel != null)
				{
					_viewPanel.Pal = value;
					Console.WriteLine("Pal set: " + value.ToString());
				}
			}
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (_viewPanel.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = _viewPanel.PreferredHeight - Height + 50;
			}
			else
				_scrollBar.Visible = false;
		}

		internal ReadOnlyCollection<ViewPckItemImage> SelectedItems
		{
			get { return _viewPanel.SelectedItems; }
		}

		internal void ChangeItem(int index, XCImage image)
		{
			_viewPanel.ChangeItem(index, image);
		}

		internal XCImageCollection Collection
		{
			get { return _viewPanel.Collection; }
			set
			{
				_viewPanel.Collection = value;
				if (_viewPanel.Collection is PckSpriteCollection)
					XConsole.AdZerg("is PckSpriteCollection");
				else
					XConsole.AdZerg("is NOT PckSpriteCollection");

				if (_viewPanel.Collection is XCImageCollection)
					XConsole.AdZerg("is XCImageCollection");
				else
					XConsole.AdZerg("is NOT XCImageCollection");

//				try
//				{
//					_viewPanel.Collection = value;
//					if (value is PckSpriteCollection)
//					{
//						_statusBPP.Text = "Bpp: " + ((PckSpriteCollection)_viewPanel.Collection).Bpp + "  ";
//					}
//					else
//						_statusBPP.Text = String.Empty;
//
//					if (ImageCollectionSetEvent != null)
//						ImageCollectionSetEvent(this, new ImageCollectionSetEventArgs(value));
//				}
//				catch (Exception ex)
//				{
//					if (ImageCollectionSetEvent != null)
//						ImageCollectionSetEvent(this, new ImageCollectionSetEventArgs(null));
//
//					throw ex;
//				}
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			_scrollBar.Value = _scrollBar.Minimum;
			_viewPanel.StartY = -_scrollBar.Value;
			if (_viewPanel.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = _viewPanel.PreferredHeight - Height;
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
			_viewPanel.StartY = -_scrollBar.Value;
			_viewPanel.Refresh();
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

		internal void Hq2x()
		{
			_viewPanel.Hq2x();
		}

		internal void RemoveSelected()
		{
			_viewPanel.RemoveSelected();
		}
	}


	/// <summary>
	/// EventArgs for ImageCollectionSet.
	/// </summary>
	internal sealed class ImageCollectionSetEventArgs
	{
		private readonly XCImageCollection _collection;

		internal ImageCollectionSetEventArgs(XCImageCollection collection)
		{
			_collection = collection;
		}

		internal XCImageCollection Collection
		{
			get { return _collection; }
		}
	}

	internal delegate void ImageCollectionSetHandler(object sender, ImageCollectionSetEventArgs e);
}
