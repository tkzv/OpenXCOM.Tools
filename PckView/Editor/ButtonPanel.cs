using System;
using System.Drawing;
using System.Windows.Forms;
using XCom;
using XCom.Interfaces;


namespace PckView
{
	/// <summary>
	/// Summary description for ButtonPanel.
	/// </summary>
	internal sealed class ButtonPanel
		:
			Panel
	{
/*		private XCImage _image;
		public XCImage Image
		{
//			get { return _image; }
			set
			{
				_image          =
				_topPanel.Image = value;

				Width = _topPanel.Width;
			}
		} */

		private readonly SinglePanel _topPanel;

		public ButtonPanel()
		{
			_topPanel = new SinglePanel();

			Controls.Add(_topPanel);

			_topPanel.Location = new Point(0, 0);
		}

		public int PreferredWidth
		{
			get { return _topPanel.Width; }
		}

/*		public int PreferredHeight
		{
			get
			{
				return (Parent != null) ? Parent.Height
										: _topPanel.Height;
			}
		} */

		public Palette Palette
		{
			set { _topPanel.SetPalette(value); }
		}
	}
}
