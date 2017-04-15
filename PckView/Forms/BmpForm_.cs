/*
using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;


namespace PckView
{
	internal sealed partial class BmpForm
	{
		private Bitmap _bmp;
		private Pen _pen;


		public class CbxItem
		{
			public XCom.Interfaces.XCImageFile _imageFile;

			public string _text;


			public CbxItem(XCom.Interfaces.XCImageFile imageFile, string text)
			{
				_imageFile = imageFile;
				_text = text;
			}


			public override string ToString()
			{
				return _text;
			}
		}


		public BmpForm()
		{
			InitializeComponent();

			_pen = new Pen(Brushes.Red, 1);

			var ri = new RegistryInfo(this);
			ri.AddProperty("LineColor");

			DialogResult = DialogResult.Cancel;

			foreach (XCom.Interfaces.XCImageFile xcf in XCom.SharedSpace.Instance.GetImageModList())
				if (xcf.FileOptions[XCom.Interfaces.XCImageFile.Filter.Bmp])
					cbTypes.Items.Add(new CbxItem(xcf, xcf.Brief));

			if (cbTypes.Items.Count > 0)
				cbTypes.SelectedIndex = 0;
		}


		public string LineColor
		{
			get { return _pen.Color.R + "|" + _pen.Color.G + "|" + _pen.Color.B; }
			set
			{
				string[] cols = value.Split('|');
				_pen.Color = Color.FromArgb(
										int.Parse(cols[0], System.Globalization.CultureInfo.InvariantCulture), // System.Globalization.NumberStyles.Integer),
										int.Parse(cols[1], System.Globalization.CultureInfo.InvariantCulture),
										int.Parse(cols[2], System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		public System.Drawing.Size SelectedSize
		{
			get
			{
				return new System.Drawing.Size(scrollWidth.Value, scrollHeight.Value);
			}
		}

		public int SelectedSpace
		{
			get { return scrollSpace.Value; }
		}

		public void SetBitmapFormData(Bitmap bmp)
		{
			_bmp = bmp;
			guess();
			Refresh();
		}

		private void guess()
		{
			if (_bmp != null && cbTypes.Items.Count != 0)
			{
				foreach (CbxItem cb in cbTypes.Items)
				{
					XCom.Interfaces.XCImageFile imageFile = cb._imageFile;
					if (   (_bmp.Width  + 1) % (imageFile.ImageSize.Width  + 1) == 0
						&& (_bmp.Height + 1) % (imageFile.ImageSize.Height + 1) == 0)
					{
						cbTypes.SelectedItem = cb;
						break;
					}
				}
			}
		}

		private void scrollWidth_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			txtWidth.Text = scrollWidth.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			drawPanel.Refresh();
		}

		private void scrollHeight_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			txtHeight.Text = scrollHeight.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			drawPanel.Refresh();
		}

		private void scrollSpace_Scroll(object sender, ScrollEventArgs e)
		{
			txtSpace.Text = scrollSpace.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			drawPanel.Refresh();
		}

		private void drawPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawImage(_bmp, 0, 0);
			for (int i = scrollWidth.Value; i < drawPanel.Width; i += scrollWidth.Value + scrollSpace.Value)
			{
				e.Graphics.DrawLine(_pen, i, 0, i, drawPanel.Height);
				e.Graphics.DrawLine(_pen, i + scrollSpace.Value - 1, 0, i + scrollSpace.Value - 1, drawPanel.Height);
			}

			for (int i = scrollHeight.Value; i < drawPanel.Height; i += scrollHeight.Value + scrollSpace.Value)
			{
				e.Graphics.DrawLine(_pen, 0, i, drawPanel.Width, i);
				e.Graphics.DrawLine(_pen, 0, i + scrollSpace.Value - 1, drawPanel.Width, i + scrollSpace.Value - 1);
			}
		}

		private void miClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void miLineColor_Click(object sender, System.EventArgs e)
		{
			colors.Color = _pen.Color;
			if (colors.ShowDialog() == DialogResult.OK)
			{
				_pen.Color = colors.Color;
				Refresh();
			}
		}

		private void cbTypes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			scrollWidth.Value  = ((CbxItem)cbTypes.SelectedItem)._imageFile.ImageSize.Width;
			scrollHeight.Value = ((CbxItem)cbTypes.SelectedItem)._imageFile.ImageSize.Height;
			scrollSpace.Value  = ((CbxItem)cbTypes.SelectedItem)._imageFile.FileOptions.Pad;

			scrollWidth_Scroll(null, null);
			scrollHeight_Scroll(null, null);
			scrollSpace_Scroll(null, null);
		}

		private void txtWidth_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				scrollWidth.Value = int.Parse(txtWidth.Text, System.Globalization.CultureInfo.InvariantCulture);
				scrollWidth_Scroll(null, null);
			}
			catch {} // TODO: that.
		}

		private void txtHeight_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				scrollHeight.Value = int.Parse(txtHeight.Text, System.Globalization.CultureInfo.InvariantCulture);
				scrollHeight_Scroll(null, null);
			}
			catch {} // TODO: that.
		}

		private void txtSpace_TextChanged(object sender, EventArgs e)
		{
			try
			{
				scrollSpace.Value = int.Parse(txtSpace.Text, System.Globalization.CultureInfo.InvariantCulture);
				scrollSpace_Scroll(null, null);
			}
			catch {} // TODO: that.
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
*/
