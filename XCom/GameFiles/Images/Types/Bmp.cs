//#define hq2xWorks

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using XCom.Interfaces;

//#if hq2xWorks
//using hq2x;
//#endif

// now why did I just *know* that the nutcase who writes the low-level code for
// image-handling was going to cram everything together ....
// And I'm the nutcase who just went through the whole thing adding whitespace.


namespace XCom
{
	public static class Bmp
	{
		public static event LoadingDelegate LoadingEvent;

		public static readonly byte DefaultTransparentIndex = 0xFE;

		// amount of space between saved bmp image blocks
//		private static readonly int space = 1;


		public static void Save(string path, Bitmap image)
		{
			Console.WriteLine("Start");
			var bw = new BinaryWriter(new FileStream(path, FileMode.Create));

			int more = 0;
			while ((image.Width + more) % 4 != 0)
				more++;

			int len = (image.Width + more) * image.Height;

			bw.Write('B');
			bw.Write('M');
			bw.Write(1078 + len); // 14 + 40 + (4 * 256)
			bw.Write((int)0);
			bw.Write((int)1078);

			bw.Write((int)40);
			bw.Write((int)image.Width);
			bw.Write((int)image.Height);
			bw.Write((short)1);
			bw.Write((short)8);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);

			Console.WriteLine("Colors");

			//Console.WriteLine("File size should be: " + (1078 + len + more));

//			byte[] bArr = new byte[256 * 4];
			Color[] entries = image.Palette.Entries;

			for (int i = 1; i < 256; i++)
			{
//			for (int i = 0; i < bArr.Length; i += 4)
//			{
//				bArr[i] = entries[i / 4].B;
//				bArr[i + 1] = entries[i / 4].G;
//				bArr[i + 2] = entries[i / 4].R;
//				bArr[i + 3] = 0;

				bw.Write(entries[i].B);
				bw.Write(entries[i].G);
				bw.Write(entries[i].R);
				bw.Write((byte)0);

//				bw.Write((byte)image.Palette.Entries[i].B);
//				bw.Write((byte)image.Palette.Entries[i].G);
//				bw.Write((byte)image.Palette.Entries[i].R);
//				bw.Write((byte)0);
			}
//			bw.Write(bArr);
			Console.WriteLine("Table");

			var table = new Dictionary<Color, byte>();

			int idx = 0;
			foreach(Color c in image.Palette.Entries)
				table[c] = (byte)idx++;

//			Console.WriteLine("Colors: "+idx);

			// the blank color between each individual image
			table[Color.FromArgb(0, 0, 0, 0)] = (byte)255;

			Console.WriteLine("image data");

			for (int i = image.Height - 1; i >= 0; i--)
			{
				for (int j = 0; j < image.Width; j++)
					bw.Write(table[image.GetPixel(j, i)]);

				for (int j = 0; j < more; j++)
					bw.Write((byte)0x00);
			}

			bw.Flush();
			bw.Close();
			Console.WriteLine("Done");
		}

		public static void Save24(string path, Bitmap image)
		{
			Save24(new FileStream(path, FileMode.Create), image);
		}

		public static void Save24(Stream str, Bitmap image)
		{
			var bw = new BinaryWriter(str);

			int more = 0;
			while ((image.Width * 3 + more) % 4 != 0)
				more++;

			int len = (image.Width * 3 + more) * image.Height;

			bw.Write('B');					// must always be set to 'BM' to declare that this is a .bmp-file.
			bw.Write('M');
			bw.Write(14 + 40 + len);		// specifies the size of the file in bytes.
			bw.Write((int)0);				// zero
			bw.Write((int)14 + 40);			// specifies the offset from the beginning of the file to the bitmap data.

			bw.Write((int)40);				// specifies the size of the BITMAPINFOHEADER structure, in bytes
			bw.Write((int)image.Width);
			bw.Write((int)image.Height);
			bw.Write((short)1);				// specifies the number of planes of the target device
			bw.Write((short)24);			// specifies the number of bits per pixel
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);

			for (int i = image.Height - 1; i >= 0; i--)
			{
				for (int j = 0; j < image.Width; j++)
				{
					var c = image.GetPixel(j, i);
					bw.Write((byte)c.B);
					bw.Write((byte)c.G);
					bw.Write((byte)c.R);
				}

				for (int j = 0; j < more; j++)
					bw.Write((byte)0x00);
			}

			bw.Flush();
			bw.Close();
		}

		/// <summary>
		/// Creates a TRUE 8-bit indexed bitmap from the specified byte array
		/// </summary>
		/// <param name="width">width of final bitmap</param>
		/// <param name="height">height of final bitmap</param>
		/// <param name="idx">image data</param>
		/// <param name="palette">Palette to color the image with</param>
		/// <returns></returns>
		public static Bitmap MakeBitmap8(
				int width,
				int height,
				byte[] idx,
				ColorPalette palette)
		{
			var image = new Bitmap(
								width, height,
								PixelFormat.Format8bppIndexed);
			var rect = new Rectangle(
								0, 0,
								width, height);

			var bitmapData = image.LockBits(
										rect,
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed);

			//Console.WriteLine("Bitmap created: {0},{1}", width, height);

			// Write to the temporary buffer that is provided by LockBits.
			// Copy the pixels from the source image in this loop.
			// Because you want an index, convert RGB to the appropriate
			// palette index here.
			var pixels = bitmapData.Scan0;

			unsafe
			{
				// Get the pointer to the image bits.
				// This is the unsafe operation.
				byte* pBits;

				if (bitmapData.Stride > 0)
				{
					pBits = (byte*)pixels.ToPointer();
				}
				else
				{
					// If the Stide is negative, Scan0 points to the last
					// scanline in the buffer. To normalize the loop, obtain
					// a pointer to the front of the buffer that is located
					// (Height-1) scanlines previous.
					pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (height - 1);
				}
				uint stride = (uint)Math.Abs(bitmapData.Stride);

				int ex = 0;
				for (uint row = 0; row < height; ++row )
					for (uint col = 0; col < width && ex < idx.Length; ++col)
					{
						// The destination pixel.
						// The pointer to the color index byte of the
						// destination; this real pointer causes this
						// code to be considered unsafe.
						byte* p8bppPixel = pBits + row * stride + col;
						*p8bppPixel = idx[ex++];
					}
			}
			image.UnlockBits(bitmapData);
			image.Palette = palette;
			return image;
		}
		
		public static Bitmap MakeBitmap(int width, int height, ColorPalette pal)
		{
			var image = new Bitmap(
								width, height,
								PixelFormat.Format8bppIndexed);
			var rect = new Rectangle(
								0, 0,
								width, height);

			var bitmapData = image.LockBits(
										rect,
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed);

			// Write to the temporary buffer that is provided by LockBits.
			// Copy the pixels from the source image in this loop.
			// Because you want an index, convert RGB to the appropriate
			// palette index here.
			var pixels = bitmapData.Scan0;

			unsafe
			{
				// Get the pointer to the image bits.
				// This is the unsafe operation.
				byte* pBits;
				if (bitmapData.Stride > 0)
				{
					pBits = (byte*)pixels.ToPointer();
				}
				else
				{
					// If the Stide is negative, Scan0 points to the last
					// scanline in the buffer. To normalize the loop, obtain
					// a pointer to the front of the buffer that is located
					// (Height-1) scanlines previous.
					pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (height - 1);
				}
				uint stride = (uint)Math.Abs(bitmapData.Stride);

				for (uint row = 0; row < height; ++row )
					for (uint col = 0; col < width; ++col )
					{
						// The destination pixel.
						// The pointer to the color index byte of the
						// destination; this real pointer causes this
						// code to be considered unsafe.
						byte* p8bppPixel = pBits + row*stride + col;
						*p8bppPixel = DefaultTransparentIndex;
					}
			}
			image.UnlockBits(bitmapData);
			image.Palette = pal;
			return image;
		}

		public static void Draw(
				Bitmap src,
				Bitmap dst,
				int x,
				int y)
		{
			var destRect = new Rectangle(
									0, 0,
									dst.Width, dst.Height);

			var destData = dst.LockBits(
									destRect,
									ImageLockMode.WriteOnly,
									PixelFormat.Format8bppIndexed);

			var srcRect = new Rectangle(
									0, 0,
									src.Width, src.Height);

			var srcData = src.LockBits(
									srcRect,
									ImageLockMode.ReadOnly,
									PixelFormat.Format8bppIndexed);

			var srcPixels = srcData.Scan0;
			var dstPixels = destData.Scan0;

			unsafe
			{
				byte* sBits;
				if (srcData.Stride > 0)
					sBits = (byte*)srcPixels.ToPointer();
				else
					sBits = (byte*)srcPixels.ToPointer() + srcData.Stride * (src.Height - 1);

				uint sStride = (uint)Math.Abs(srcData.Stride);

				byte* dBits;
				if (destData.Stride > 0)
					dBits = (byte*)dstPixels.ToPointer();
				else
					dBits = (byte*)dstPixels.ToPointer() + destData.Stride * (dst.Height - 1);

				uint dStride = (uint)Math.Abs(destData.Stride);

				for (uint row = 0; row < src.Height; row++ )
					for (uint col = 0; col < src.Width; col++ )
					{
						byte* d8bppPixel = dBits + (row + y) * dStride + (col + x);
//						byte* s8bppPixel = sBits + ((row / PckImage.Scale) * sStride + (col / PckImage.Scale));
						byte* s8bppPixel = sBits + row * sStride + col;

						if (*s8bppPixel != DefaultTransparentIndex && row + y < dst.Height)
							*d8bppPixel = *s8bppPixel;
					}
			}
			src.UnlockBits(srcData);
			dst.UnlockBits(destData);
		}

		public static Rectangle GetBoundsRect(Bitmap src, int transparent)
		{
			var srcRect = new Rectangle(
									0, 0,
									src.Width, src.Height);

			var srcData = src.LockBits(
									srcRect,
									ImageLockMode.ReadOnly,
									PixelFormat.Format8bppIndexed);

			var srcPixels = srcData.Scan0;

			int minR, minC, maxR, maxC;
			unsafe
			{
				byte* sBits;
				if (srcData.Stride > 0)
					sBits = (byte*)srcPixels.ToPointer();
				else
					sBits = (byte*)srcPixels.ToPointer() + srcData.Stride * (src.Height - 1);
				
				uint sStride = (uint)Math.Abs(srcData.Stride);

				for (minR = 0; minR < src.Height; minR++)
				{
					for (uint col = 0; col < src.Width; col++)
					{
						byte idx = *(sBits + minR * sStride + col);
						if (idx != transparent)
							goto outLoop1;
					}
				}

			outLoop1:
				for (minC = 0; minC < src.Width; minC++)
				{
					for (int r = minR; r < src.Height; r++)
					{
						byte idx = *(sBits + r * sStride + minC);
						if (idx != transparent)
							goto outLoop2;
					}
				}

			outLoop2:
				for (maxR = src.Height - 1; maxR > minR; maxR--)
				{
					for (int col = minC; col < src.Width; col++)
					{
						byte idx = *(sBits + maxR * sStride + col);
						if (idx != transparent)
							goto outLoop3;
					}
				}

			outLoop3:
				for (maxC = src.Width - 1; maxC > minC; maxC--)
				{
					for (int r = minR; r < maxR; r++)
					{
						byte idx = *(sBits + r * sStride + maxC);
						if ( idx != transparent)
							goto outLoop4;
					}
				}

			outLoop4:
				Console.Write("");
			}
			src.UnlockBits(srcData);

			return new Rectangle(
							minC - 1, minR - 1,
							maxC - minC + 3, maxR - minR + 3);
		}

		public static Bitmap Crop(Bitmap src, Rectangle bounds)
		{
			//Console.WriteLine(
			//				"Old size: {0},{1} New Size: {2},{3}",
			//				src.Width, src.Height,
			//				bounds.Width, bounds.Height);

			var dst = MakeBitmap(bounds.Width, bounds.Height, src.Palette);
			var destRect = new Rectangle(
									0, 0,
									dst.Width, dst.Height);

			var dstData = dst.LockBits(
									destRect,
									ImageLockMode.WriteOnly,
									PixelFormat.Format8bppIndexed);

			var srcRect = new Rectangle(
									0, 0,
									src.Width, src.Height);

			var srcData = src.LockBits(
									srcRect,
									ImageLockMode.ReadOnly,
									PixelFormat.Format8bppIndexed);

			var srcPixels = srcData.Scan0;
			var dstPixels = dstData.Scan0;

			unsafe
			{
				byte* sBits;
				if (srcData.Stride > 0)
					sBits = (byte*)srcPixels.ToPointer();
				else
					sBits = (byte*)srcPixels.ToPointer() + srcData.Stride * (src.Height - 1);

				uint sStride = (uint)Math.Abs(srcData.Stride);

				byte* dBits;
				if (dstData.Stride > 0)
					dBits = (byte*)dstPixels.ToPointer();
				else
					dBits = (byte*)dstPixels.ToPointer() + dstData.Stride * (dst.Height - 1);

				uint dStride = (uint)Math.Abs(dstData.Stride);

				for (uint row = 0; row < bounds.Height; row++)
					for (uint col = 0; col < bounds.Width; col++)
					{
						byte* s8bppPixel = sBits + (row + bounds.Y) * sStride + (col + bounds.X);
						byte* d8bppPixel = dBits + row * dStride + col;

//						if (*s8bppPixel != PckImage.TransparentIndex && row + y < dst.Height)
						*d8bppPixel = *s8bppPixel;
					}
			}
			src.UnlockBits(srcData);
			dst.UnlockBits(dstData);
			return dst;
		}

		public static void FireLoadingEvent(int curr, int total)
		{
			if (LoadingEvent != null)
				LoadingEvent(curr, total);
		}

		public static unsafe Bitmap Hq2x(Bitmap image)
		{
#if hq2xWorks
			CImage in24 = new CImage();
			in24.Init(image.Width, image.Height, 24);

			for (int row = 0; row < image.Height; row++)
				for (int col = 0; col < image.Width; col++)
				{
					Color c = image.GetPixel(col,row);
					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 0)) = c.B;
					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 1)) = c.G;
					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 2)) = c.R;
				}

			in24.ConvertTo16();

			CImage out32 = new CImage();
			out32.Init(in24.m_Xres * 2, in24.m_Yres * 2, 32);

			CImage.InitLUTs();
			CImage.hq2x_32(
						in24.m_pBitmap,
						out32.m_pBitmap,
						in24.m_Xres,
						in24.m_Yres,
						out32.m_Xres * 4);

			out32.ConvertTo24();

			Bitmap b = new Bitmap(
								out32.m_Xres, out32.m_Yres,
								PixelFormat.Format24bppRgb);

//			Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
			BitmapData bitmapData = b.LockBits(
											new Rectangle(
														0, 0,
														b.Width, b.Height),
											ImageLockMode.WriteOnly,
											b.PixelFormat);

			IntPtr pixels = bitmapData.Scan0;

			byte* pBits;
			if (bitmapData.Stride > 0)
				pBits = (byte*)pixels.ToPointer();
			else
				pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (b.Height - 1);

			byte* srcBits = out32.m_pBitmap;

			for (int i = 0; i < b.Width * b.Height; i++)
			{
				*(pBits++) = *(srcBits++);
				*(pBits++) = *(srcBits++);
				*(pBits++) = *(srcBits++);
			}

			b.UnlockBits(bitmapData);

			image.Dispose();
			in24.__dtor();
			out32.__dtor();

			return b;
#else
			return null;
#endif
		}

		/// <summary>
		/// Saves an image collection as a bmp sprite sheet
		/// </summary>
		/// <param name="file">output file name</param>
		/// <param name="collection">image collection</param>
		/// <param name="pal">Palette to color the images with</param>
		/// <param name="across">number of columns to use for images</param>
		/// <param name="space"></param>
		public static void SaveBMP(
				string file,
				XCImageCollection collection,
				Palette pal,
				int across,
				int space)
		{
			if (collection.Count == 1)
				across = 1;

			int mod = 1;
			if (collection.Count % across == 0)
				mod = 0;

			var b = MakeBitmap(
							across * (collection.IXCFile.ImageSize.Width + space) - space,
							(collection.Count / across + mod) * (collection.IXCFile.ImageSize.Height + space) - space,
							pal.Colors);

			for (int i = 0; i < collection.Count; i++)
			{
				int x = i % across * (collection.IXCFile.ImageSize.Width  + space);
				int y = i / across * (collection.IXCFile.ImageSize.Height + space);
				Draw(collection[i].Image, b, x, y);
			}
			Save(file, b);
		}

		/// <summary>
		/// Loads a previously saved sprite sheet as a generic collection to be saved later
		/// </summary>
		/// <param name="b">bitmap containing sprites</param>
		/// <param name="pal"></param>
		/// <param name="imgWid"></param>
		/// <param name="imgHei"></param>
		/// <param name="space"></param>
		/// <returns></returns>
		public static XCImageCollection Load(
				Bitmap b,
				Palette pal,
				int imgWid,
				int imgHei,
				int space)
		{
			var list = new XCImageCollection();

			int cols = (b.Width  + space) / (imgWid + space);
			int rows = (b.Height + space) / (imgHei + space);

			int num = 0;

			//Console.WriteLine("Image: {0},{1} -> {2},{3}", b.Width, b.Height, cols, rows);
			for (int i = 0; i < cols * rows; i++)
			{
				int x = (i % cols) * (imgWid + space);
				int y = (i / cols) * (imgHei + space);
				//Console.WriteLine("{0}: {1},{2} -> {3}", num, x, y, PckImage.Width);
				list.Add(LoadTile(
								b,
								num++,
								pal,
								x,y,
								imgWid,imgHei));
				FireLoadingEvent(num, rows * cols);
			}

			list.Pal = pal;

			return list;
		}

		public static XCImage LoadTile(
				Bitmap src,
				int imgNum,
				Palette p,
				int startX,
				int startY,
				int imgWid,
				int imgHei)
		{
			// image data in 8-bit form
			var data = new byte[imgWid * imgHei];

			var srcRect = new Rectangle(
									startX, startY,
									imgWid, imgHei);

			var srcData = src.LockBits(
									srcRect,
									ImageLockMode.ReadOnly,
									PixelFormat.Format8bppIndexed);

			var srcPixels = srcData.Scan0;

			unsafe
			{
				byte* sBits;
				if (srcData.Stride > 0)
					sBits = (byte*)srcPixels.ToPointer();
				else
					sBits = (byte*)srcPixels.ToPointer() + srcData.Stride * (src.Height - 1);
				
				uint sStride = (uint)Math.Abs(srcData.Stride);

				for (uint row = 0, i = 0; row < imgHei; row++)
					for (uint col = 0; col < imgWid; col++)
						data[i++] = *(sBits + row * sStride + col);
			}

			src.UnlockBits(srcData);

			return new XCImage(data, imgWid, imgHei, p, imgNum);
		}

/*		public static XCImageCollection Load(string file, Type collectionType)
		{
			Bitmap b = new Bitmap(file);

			MethodInfo mi = collectionType.GetMethod("FromBmp");
			if (mi == null)
				return null;
			else
				return (XCImageCollection)mi.Invoke(null, new object[]{ b });
		} */

/*		public static XCImage LoadSingle(Bitmap src, int num, Palette pal, Type collectionType)
		{
//			return PckFile.FromBmpSingle(src, num, pal);

			MethodInfo mi = collectionType.GetMethod("FromBmpSingle");
			if (mi == null)
				return null;
			else
				return (XCImage)mi.Invoke(null, new object[]{ src, num, pal });
		}*/
	}
}
