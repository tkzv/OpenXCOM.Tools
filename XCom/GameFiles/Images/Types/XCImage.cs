using System;
using System.Drawing;


namespace XCom.Interfaces
{
	public class XCImage
		:
			ICloneable
	{
		public byte[] Offsets
		{ get; protected set; }

		public int FileId
		{ get; set; }

		public Bitmap Image
		{ get; protected set; }

		public Bitmap Gray
		{ get; protected set; }

		private Palette _palette;
		public Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;

				if (Image != null)
					Image.Palette = _palette.Colors;
			}
		}


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
			FileId   = id;
			Offsets  = offsets;
			_palette = pal;

			if (pal != null)
				Image = Bmp.MakeBitmap8(
									width,
									height,
									offsets,
									pal.Colors);
		}
/*		public XCImage()
			:
				this(
					new byte[]{},
					0, 0,
					null,
					-1)
		{} */
		public XCImage(Bitmap image, int id)
		{
			Image  = image;
			FileId = id;

//			Offsets  = null;
//			_palette = null;
		}


		public object Clone()
		{
			if (Offsets != null)
			{
				var offsets = new byte[Offsets.Length];
				for (int i = 0; i != offsets.Length; ++i)
					offsets[i] = Offsets[i];

				return new XCImage(
								offsets,
								Image.Width,
								Image.Height,
								_palette,
								FileId);
			}

			return (Image != null) ? new XCImage((Bitmap)Image.Clone(), FileId)
								   : null;

		}

		public void HQ2X()
		{
			Image = Bmp.HQ2X(/*Image*/);
		}
	}
}
