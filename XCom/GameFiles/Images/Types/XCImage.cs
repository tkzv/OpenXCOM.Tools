using System;
using System.Drawing;

namespace XCom.Interfaces
{
	public class XCImage
		:
		ICloneable
	{
		protected byte[] idx;
		protected int fileNum;
		protected Bitmap _image;
		protected Bitmap _gray;

		private const byte transparent = 0xFE;

		private Palette palette;


		/// <summary>
		/// Entries must not be compressed.
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal"></param>
		/// <param name="idx"></param>
		public XCImage(
				byte[] entries,
				int width,
				int height,
				Palette pal,
				int idx)
		{
			fileNum = idx;
			this.idx = entries;
			palette = pal;

			if (pal != null)
				_image = Bmp.MakeBitmap8(
									width,
									height,
									entries,
									pal.Colors);
		}

		public XCImage()
			:
			this(
				new byte[]{},
				0,
				0,
				null,
				-1)
		{}

		public XCImage(Bitmap bmp, int idx)
		{
			fileNum = idx;
			_image = bmp;
			this.idx = null;
			palette = null;
		}


		public byte[] Bytes
		{
			get { return idx; }
		}

		public int FileNum
		{
			get { return fileNum; }
			set { fileNum = value; }
		}

		public Bitmap Image
		{
			get { return _image; }
		}

		public Palette Palette
		{
			get { return palette; }
			set
			{
				palette = value;

				if (_image != null)
					_image.Palette = palette.Colors;
			}
		}

		public virtual byte TransparentIndex
		{
			get { return transparent; }
		}

		public Bitmap Gray
		{
			get { return _gray; }
		}

		public object Clone()
		{
			if (idx != null)
			{
				var bites = new byte[idx.Length];
				for (int i = 0; i != bites.Length; ++i)
					bites[i] = idx[i];

				return new XCImage(
								bites,
								_image.Width,
								_image.Height,
								palette,
								fileNum);
			}

			return (_image != null) ? new XCImage((Bitmap)_image.Clone(), fileNum)
									: null;

		}

		public virtual void Hq2x()
		{
			_image = Bmp.Hq2x(/*_image*/);
		}
	}
}
