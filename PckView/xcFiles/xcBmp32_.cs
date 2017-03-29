/*
using System;
using System.Drawing;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public class xcDotNet
		:
		IXCImageFile
	{
		public xcDotNet()
			:
			this(32, 40)
		{}

		public xcDotNet(int wid, int hei)
			:
			base(wid, hei)
		{
			author	= "Ben Ratzlaff";
			ext		= ".*";
			desc	= "Interface for any file type the .net framework can load";
			expDesc	= ".net image loader";

			_fileOptions.Init(false, false, true, true);
			_fileOptions.BitDepth = 32;
		}

		public override bool RegisterFile()
		{
			return true;
		}

		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				Palette pal)
		{
			var img = Image.FromFile(directory + @"\" + file);
			var bmp = new System.Drawing.Bitmap(img);
			var bmf = new BmpForm();

			bmf.SetBitmapFormData(bmp);

			xConsole.AddLine("File: " + directory + @"\" + file);
			if (bmf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_imageSize = bmf.SelectedSize;
				return new DotNetCollection(bmp, width, height, bmf.SelectedSpace);
			}

			return null;
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCImageCollection images)
		{
			DotNetCollection.Save(directory + @"\" + file, images);
		}
	}
}
*/
