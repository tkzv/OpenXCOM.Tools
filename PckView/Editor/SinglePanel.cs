using System;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	/// <summary>
	/// Summary description for SinglePanel.
	/// </summary>
	internal sealed class SinglePanel
		:
			Panel
	{
		private XCImage _image;
		public XCImage Image
		{
//			get { return _image; }
			set
			{
				_image = value;

				Width  = _image.Image.Width;
				Height = _image.Image.Height;

				Refresh();
			}
		}


		public SinglePanel()
		{
			Width  = PckImage.Width;
			Height = PckImage.Height;
		}


		public void SetPalette(Palette pal)
		{
			if (_image != null)
			{
				_image.Image.Palette = pal.Colors;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_image != null)
				e.Graphics.DrawImage(_image.Image, 0, 0);
		}
	}
}
