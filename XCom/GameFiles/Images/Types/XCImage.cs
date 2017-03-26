using System;
using System.Drawing;


namespace XCom.Interfaces
{
	public class XCImage
		:
		ICloneable
	{
		private byte[] _offsets;
		public byte[] Offsets
		{
			get { return _offsets; }
			protected set { _offsets = value; }
		}

		private int _fileId;
		public int FileId
		{
			get { return _fileId; }
			set { _fileId = value; }
		}

		private Bitmap _image;
		public Bitmap Image
		{
			get { return _image; }
			protected set { _image = value; }
		}

		public Bitmap Gray
		{ get; protected set; }

		protected const byte TransparentId = 0xFE;

		private Palette _palette;


		/// <summary>
		/// Entries must not be compressed.
		/// </summary>
		/// <param name="offsets"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal"></param>
		/// <param name="id"></param>
		public XCImage(
				byte[] offsets,
				int width,
				int height,
				Palette pal,
				int id)
		{
			_fileId = id;
			_offsets = offsets;
			_palette = pal;

			if (pal != null)
				_image = Bmp.MakeBitmap8(
									width,
									height,
									offsets,
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

		public XCImage(Bitmap image, int id)
		{
			_fileId = id;
			_image = image;
			_offsets = null;
			_palette = null;
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

		public object Clone()
		{
			if (_offsets != null)
			{
				var offsets = new byte[_offsets.Length];
				for (int i = 0; i != offsets.Length; ++i)
					offsets[i] = _offsets[i];

				return new XCImage(
								offsets,
								_image.Width,
								_image.Height,
								_palette,
								_fileId);
			}

			return (_image != null) ? new XCImage((Bitmap)_image.Clone(), _fileId)
									: null;

		}

		public void Hq2x()
		{
			_image = Bmp.Hq2x(/*_image*/);
		}
	}
}
