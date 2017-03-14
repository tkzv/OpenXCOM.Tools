using System;
using System.Drawing;


namespace XCom.Interfaces
{
	public class XCImage
		:
		ICloneable
	{
		protected byte[] _id;
		protected int _fileId;
		protected Bitmap _image;
		protected Bitmap _gray;

		private const byte _transparent = 0xFE;

		private Palette _palette;


		/// <summary>
		/// Entries must not be compressed.
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal"></param>
		/// <param name="id"></param>
		public XCImage(
				byte[] entries,
				int width,
				int height,
				Palette pal,
				int id)
		{
			_fileId = id;
			_id = entries;
			_palette = pal;

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
				0, 0,
				null,
				-1)
		{}

		public XCImage(Bitmap bmp, int idx)
		{
			_fileId = idx;
			_image = bmp;
			_id = null;
			_palette = null;
		}


		public byte[] Bytes
		{
			get { return _id; }
		}

		public int FileId
		{
			get { return _fileId; }
			set { _fileId = value; }
		}

		public Bitmap Image
		{
			get { return _image; }
		}

		public Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;

				if (_image != null)
					_image.Palette = _palette.Colors;
			}
		}

		public virtual byte TransparentIndex
		{
			get { return _transparent; }
		}

		public Bitmap Gray
		{
			get { return _gray; }
		}

		public object Clone()
		{
			if (_id != null)
			{
				var bites = new byte[_id.Length];
				for (int i = 0; i != bites.Length; ++i)
					bites[i] = _id[i];

				return new XCImage(
								bites,
								_image.Width,
								_image.Height,
								_palette,
								_fileId);
			}

			return (_image != null) ? new XCImage((Bitmap)_image.Clone(), _fileId)
									: null;

		}

		public virtual void Hq2x()
		{
			_image = Bmp.Hq2x(/*_image*/);
		}
	}
}
