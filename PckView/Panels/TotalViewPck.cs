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
		private readonly ViewPck view;
		public ViewPck View
		{
			get { return view; }
		}

		private VScrollBar _scrollBar;
		private StatusBar _statusBar;
		private StatusBarPanel _statusOverTile;
		private StatusBarPanel _statusBPP;

		private int click;
		private int move;

		public event PckViewMouseClicked ViewClicked;
		public event XCImageCollectionHandler XCImageCollectionSet;


		public TotalViewPck()
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

			_scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			_scrollBar.Maximum = 5000;
			_scrollBar.Scroll += this.scroll_Scroll;

			view = new ViewPck();
			view.Location = new Point(0, 0);
			view.ViewClicked += new PckViewMouseClicked(viewClicked);
			view.ViewMoved += new PckViewMouseMoved(viewMoved);
			view.Dock = DockStyle.Fill;
			view.ViewClicked += new PckViewMouseClicked(viewClik);
			_scrollBar.Minimum = 0;

			this.Controls.AddRange(new Control[] { _statusBar, _scrollBar, view });

			view.SizeChanged += new EventHandler(viewSizeChange);
			OnResize(null); // FIX: "Virtual member call in constructor."
		}

		private void viewClik(object sender, PckViewMouseEventArgs e)
		{
			if (ViewClicked != null)
				ViewClicked(sender, e);
		}

		public Palette Pal
		{
			get { return view != null ? view.Pal : null; }
			set
			{
				if (view != null)
				{
					view.Pal = value;
					Console.WriteLine("Pal set: " + value.ToString());
				}
			}
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (view.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = view.PreferredHeight - Height + 50;
			}
			else
				_scrollBar.Visible = false;
		}

		public ReadOnlyCollection<ViewPckItemImage> SelectedItems
		{
			get { return view.SelectedItems; }
		}

		public void ChangeItem(int index, XCImage image)
		{
			view.ChangeItem(index, image);
		}

		public XCImageCollection Collection
		{
			get { return view.Collection; }
			set
			{
				try
				{
					view.Collection = value;
					if (value is PckSpriteCollection)
					{
						_statusBPP.Text = "Bpp: " + ((PckSpriteCollection)view.Collection).Bpp + "  ";
					}
					else
						_statusBPP.Text = String.Empty;

					if (XCImageCollectionSet != null)
						XCImageCollectionSet(this, new XCImageCollectionSetEventArgs(value));
				}
				catch (Exception ex)
				{
					if (XCImageCollectionSet != null)
						XCImageCollectionSet(this, new XCImageCollectionSetEventArgs(null));

					throw ex;
				}
			}
		}

		private void viewSizeChange(object sender, EventArgs e)
		{
			_scrollBar.Value = _scrollBar.Minimum;
			view.StartY = -_scrollBar.Value;
			if (view.PreferredHeight >= Height)
			{
				_scrollBar.Visible = true;
				_scrollBar.Maximum = view.PreferredHeight - Height;
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

		private void scroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			view.StartY = -_scrollBar.Value;
			view.Refresh();
		}

		private void viewClicked(object sender, PckViewMouseEventArgs e)
		{
			click = e.Clicked;
			_statusOverTile.Text = "Selected: " + click + " Over: " + move;
		}

		private void viewMoved(int x)
		{
			move = x;
			_statusOverTile.Text = "Selected: " + click + " Over: " + move;
		}

		public void Hq2x()
		{
			view.Hq2x();
		}

		public void RemoveSelected()
		{
			view.RemoveSelected();
		}
	}


	/// <summary>
	/// EventArgs for XCImageCollectionSet.
	/// </summary>
	internal sealed class XCImageCollectionSetEventArgs
	{
		private readonly XCImageCollection _collection;

		public XCImageCollectionSetEventArgs(XCImageCollection collection)
		{
			_collection = collection;
		}

		public XCImageCollection Collection
		{
			get { return _collection; }
		}
	}

	internal delegate void XCImageCollectionHandler(object sender, XCImageCollectionSetEventArgs e);
}
