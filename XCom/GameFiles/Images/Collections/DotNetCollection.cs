/*
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom.Interfaces;


namespace XCom
{
	public class DotNetCollection
		:
		XCImageCollection
	{
		public DotNetCollection(
				Bitmap src,
				int width,
				int height,
				int pad)
		{
			var pvf = (Form)SharedSpace.Instance["PckView"];

//			xConsole.AddLine("File: " + file);
//			Image img = Image.FromFile(file);
//			Bitmap src = new Bitmap(img);

			int across = (src.Width  + pad) / (width  + pad);
			int down   = (src.Height + pad) / (height + pad);

			var pw = new DSShared.Windows.ProgressWindow(pvf);
			pw.Minimum = 0;
			pw.Maximum = across * down;
			pw.Width = 300;
			pw.Height = 50;

			pw.Show();

			BitmapData srcData = src.LockBits(
										new Rectangle(0, 0, src.Width, src.Height),
										ImageLockMode.ReadOnly,
										src.PixelFormat);

//			xConsole.AddLine("Pixelformat is: " + src.PixelFormat.ToString());

			int bpp;
			switch (src.PixelFormat)
			{
				case PixelFormat.Format24bppRgb:
					bpp = 3;
					break;

				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format32bppPArgb:
				case PixelFormat.Format32bppRgb:
					bpp = 4;
					break;

				default:
					throw new Exception("Image is not 24 or 32 bit, a different collection is needed");
			}

			for (int i = 0, idx = 0; i < src.Height; i += height + pad)
			{
				for (int j = 0; j < src.Width; j += width + pad, idx++)
				{
					var dst = new Bitmap(width, height, src.PixelFormat);
					BitmapData destData = dst.LockBits(
													new Rectangle(0, 0, dst.Width, dst.Height),
													ImageLockMode.WriteOnly,
													dst.PixelFormat);

					copyData(
							srcData,
							j,
							i,
							destData,
//							0, 0,
							destData.Width,
							destData.Height,
							bpp);

					dst.UnlockBits(destData);

					Add(new XCImage(dst, idx));
					try
					{
						pw.Value = idx;
					}
					catch {} // TODO: that.
				}
//				srcPtr += srcData.Stride - srcData.Width * bpp;
			}
			src.UnlockBits(srcData);

			pw.Hide();
		}

		private unsafe void copyData(
				BitmapData srcData,
				int srcX,
				int srcY,
				BitmapData dstData,
//				int dstX, // TODO: not used.
//				int dstY, // TODO: not used.
				int width,
				int height,
				int bpp)
		{
			for (int y = 0; y < height && y + srcY < srcData.Height; y++)
			{
				int srcRow = (srcY + y) * srcData.Stride;
				int destRow = y * dstData.Stride;

				for (int x = 0; x < width && srcX + x < srcData.Width; x++)
				{
					byte* srcPixel = ((byte*)(srcData.Scan0)) + srcRow + ((srcX + x) * bpp);
					byte* dstPixel = ((byte*)(dstData.Scan0)) + destRow + (x * bpp);

					for (int k = 0; k < bpp; k++)
						*dstPixel++ = *srcPixel++;
				}
			}
		}


		public static void Save(string outFile, XCImageCollection images)
		{}

		/// <summary>
		/// 32-bit images dont have palettes.
		/// </summary>
		public override Palette Pal
		{
			get { return null; }
//			set {}
		}
	}
}
*/
