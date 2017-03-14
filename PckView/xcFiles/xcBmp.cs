/*
using System;

using XCom.Interfaces;


namespace PckView
{
	public class xcBmp
		:
		IXCImageFile
	{
		public xcBmp()
			:
			base(0, 0)
		{
//			fileOptions.BmpDialog = false;
//			fileOptions.SaveDialog = false; // save as bmp has its own menu item
//			fileOptions.OpenDialog = true;
//			fileOptions.CustomDialog = false;

			_fileOptions.Init(true, false, true, false);

			author	= "Ben Ratzlaff";
			ext		= ".bmp";
			desc	= "Provides 8-bit BMP support";
			expDesc	= "8-bit bmp file";
		}

		protected override XCom.XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				XCom.Palette pal)
		{
			var bmp = new System.Drawing.Bitmap(directory + @"\" + file);
			var bmf = new BmpForm();

			bmf.SetBitmapFormData(bmp);

			if (bmf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_imageSize = bmf.SelectedSize;
				return XCom.Bmp.Load(
								bmp,
								pal,
								_imageSize.Width,
								_imageSize.Height,
								bmf.SelectedSpace);
			}
			return null;
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCom.XCImageCollection images)
		{
			throw new NotImplementedException();
//			TotalViewPck.Instance.View.SaveBMP(directory + @"/" + file + ".bmp", TotalViewPck.Instance.Pal);
		}
	}
}
*/
