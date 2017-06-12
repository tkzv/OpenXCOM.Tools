using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	public sealed class PckImage
		:
			XCImage
	{
		#region Fields (static)
		private const byte TransparentId = 0xFE;	// should that be '0x0' - there's something "funny"
													// going on with using id=254 as the transparent index.
		private static int _idCanonical;
		#endregion


		#region Fields
//		private int _mapId;

		private readonly SpriteCollection _spriteset;

		private readonly byte[] _expanded; // i suspect this should be '_scaled'
		private int _moveId = -1;
//		private byte _moveVal = 0;
		#endregion


		#region Properties
		/// <summary>
		/// Id is used only by MapInfoForm.
		/// </summary>
		public int Id
		{ get; private set; }
		#endregion


		#region cTors
//		internal PckImage(
//				int imageId,
//				byte[] id,
//				Palette pal,
//				SpriteCollection pckFile)
//			:
//			this(
//				imageId,
//				id,
//				pal,
//				pckFile)
//		{}
		internal PckImage(
				int fileId,
				byte[] bindata,
				Palette pal,
				SpriteCollection spriteset)
			:
				base(
					new byte[]{},
					0, 0,
					null,
					-1)
		{
			FileId = fileId;
			Pal    = pal;
			_spriteset = spriteset;

//			this.imageNum = imageNum;
//			this.idx = idx;

			Id = _idCanonical++;

//			image = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
			_expanded = new byte[XCImageFile.SpriteWidth * XCImageFile.SpriteHeight];

			for (int i = 0; i != _expanded.Length; ++i)
				_expanded[i] = TransparentId;

			int posStart    = 0;
			int posExpanded = 0;

			if (bindata[0] != 254)
				posExpanded = bindata[posStart++] * XCImageFile.SpriteWidth;

			for (int i = posStart; i < bindata.Length; ++i)
			{
				switch (bindata[i])
				{
					case 254: // skip required pixels
						if (_moveId == -1)
						{
							_moveId = i + 1;
//							_moveVal = id[i + 1];
						}
						posExpanded += bindata[i + 1];
						++i;
						break;

					case 255: // end of image
						break;

					default:
						_expanded[posExpanded++] = bindata[i];
						break;
				}
			}
			Bindata = _expanded;
		
			Image = XCBitmap.MakeBitmap8(
										XCImageFile.SpriteWidth,
										XCImageFile.SpriteHeight,
										_expanded,
										pal.Colors);
			SpriteGray = XCBitmap.MakeBitmap8(
										XCImageFile.SpriteWidth,
										XCImageFile.SpriteHeight,
										_expanded,
										pal.Grayscale.Colors);
		}
		#endregion


		#region Methods
		internal static int WritePckFile(BinaryWriter bw, XCImage image)
		{
			int pos = 0;
			bool flag = true;

			byte[] input = image.Bindata;

			var bindata = new List<byte>();

//			Color trans = pal.Transparent;
//			pal.SetTransparent(false);

			for (int id = 0; id != input.Length; ++id)
			{
				byte b = input[id];

				if (b == TransparentId)
					++pos;
				else
				{
					if (pos != 0)
					{
						if (flag)
						{
							flag = false;

							bindata.Add((byte)(pos / image.Image.Width));	// # of initial rows to skip
							pos =       (byte)(pos % image.Image.Width);	// current position in the transparent row
							//Console.WriteLine("count, lines: {0}, cells {1}", count/PckImage.IMAGE_WIDTH, count%PckImage.IMAGE_WIDTH);
						}

						while (pos >= 255)
						{
							bindata.Add(TransparentId);
							bindata.Add(255);
							pos -= 255;
						}

						if (pos != 0)
						{
							bindata.Add(TransparentId);
							bindata.Add((byte)pos);
						}
						pos = 0;
					}
					bindata.Add(b);
				}
			}

			bool throughLoop = false;
			while (pos >= 255)
			{
				bindata.Add(254);
				bindata.Add(255);
				pos -= 255;
				throughLoop = true;
			}

			if ((byte)bindata[bindata.Count - 1] != 255 || throughLoop)
				bindata.Add(255);

//			if (bytes.Count % 2 == 1 || throughLoop)
//				bytes.Add(255);

			bw.Write(bindata.ToArray());

			return bindata.Count;
		}

//		public override void Hq2x()
//		{
//			if (Width == 32) // hasn't been done yet
//				base.Hq2x();
//		}

//		public static Type GetCollectionType()
//		{
//			return typeof(SpriteCollection);
//		}

//		public void ReImage()
//		{
//			_image = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Colors);
//			_gray = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Grayscale.Colors);
//		}

//		public void MoveImage(byte offset)
//		{
//			_id[_moveId] = (byte)(_moveVal - offset);
//			int ex = 0;
//			int startIdx = 0;
//			for (int i = 0; i != _expanded.Length; ++i)
//				_expanded[i] = TransparentIndex;
//
//			if (_id[0] != 254)
//				ex = _id[startIdx++] * Width;
//
//			for (int i = startIdx; i < _id.Length; ++i)
//			{
//				switch (_id[i])
//				{
//					case 254: // skip required pixels
//						ex += _id[i + 1];
//						++i;
//						break;
//
//					case 255: // end of image
//						break;
//
//					default:
//						_expanded[ex++] = _id[i];
//						break;
//				}
//			}
//		
//			_image = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Colors);
//			_gray = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Grayscale.Colors);
//		}

//		public int MapId
//		{
//			get { return _mapId; }
//			set { _mapId = value; }
//		}
		#endregion


		#region Methods (override)
		public override bool Equals(object obj)
		{
			if (obj is PckImage)
				return ToString().Equals(obj.ToString());

			return false;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			string ret = String.Empty;

			if (_spriteset != null)
				ret += _spriteset.ToString();

			ret += FileId + Environment.NewLine;

			for (int i = 0; i != _expanded.Length; ++i)
			{
				ret += _expanded[i];

				switch (_expanded[i])
				{
					case 255:
						ret += Environment.NewLine;
						break;

					default:
						ret += " ";
						break;
				}
			}
			return ret;
		}
		#endregion
	}
}
