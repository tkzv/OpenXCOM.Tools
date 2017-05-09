using System;
using System.Collections.Generic;

using XCom.Interfaces;


namespace XCom
{
	public sealed class PckImage // [xImage(32,40)]
		:
			XCImage
	{
//		private int _mapId;

		private const byte TransparentId = 0xFE;

		private readonly PckSpriteCollection _pckPack;
		private readonly byte[] _expanded; // i suspect this should be '_scaled'
		private int _moveId = -1;
//		private byte _moveVal = 0;

		private static int _idCanonical;
		private int _id;

		public static int Width  = 32;
		public static int Height = 40;


/*		internal PckImage(
				int imageId,
				byte[] id,
				Palette pal,
				PckSpriteCollection pckFile)
			:
			this(
				imageId,
				id,
				pal,
				pckFile,
				32, 40)
		{} */
		internal PckImage(
				int imageId,
				byte[] binData,
				Palette pal,
				PckSpriteCollection pckPack,
				int width,
				int height)
			:
				base(
					new byte[]{},
					0, 0,
					null,
					-1)
		{
			Palette = pal;
			_pckPack = pckPack;
			FileId = imageId;

//			this.imageNum = imageNum;
//			this.idx = idx;

			_id = _idCanonical++;

			Width  = width;
			Height = height;

//			image = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
			_expanded = new byte[Width * Height];

			for (int i = 0; i != _expanded.Length; ++i)
				_expanded[i] = TransparentId;

			int posStart = 0;
			int posExpanded = 0;

			if (binData[0] != 254)
				posExpanded = binData[posStart++] * Width;

			for (int i = posStart; i < binData.Length; ++i)
			{
				switch (binData[i])
				{
					case 254: // skip required pixels
						if (_moveId == -1)
						{
							_moveId = i + 1;
//							_moveVal = id[i + 1];
						}
						posExpanded += binData[i + 1];
						++i;
						break;

					case 255: // end of image
						break;

					default:
						_expanded[posExpanded++] = binData[i];
						break;
				}
			}
			Bindata = _expanded;
		
			Sprite = XCBitmap.MakeBitmap8(
										Width,
										Height,
										_expanded,
										pal.Colors);
			SpriteGray = XCBitmap.MakeBitmap8(
										Width,
										Height,
										_expanded,
										pal.Grayscale.Colors);
		}


		internal static int EncodePckData(System.IO.BinaryWriter output, XCImage image)
		{
			int count = 0;
			bool flag = true;

			byte[] input = image.Bindata;

			var binData = new List<byte>();

//			Color trans = pal.Transparent;
//			pal.SetTransparent(false);

			int totalCount = 0;
			for (int i = 0; i != input.Length; ++i)
			{
				byte id = input[i];
				++totalCount;

				if (id == TransparentId)
					++count;
				else
				{
					if (count != 0)
					{
						if (flag)
						{
							flag = false;

							binData.Add((byte)(count / image.Sprite.Width));	// # of initial rows to skip
							count     = (byte)(count % image.Sprite.Width);		// current position in the transparent row
							//Console.WriteLine("count, lines: {0}, cells {1}", count/PckImage.IMAGE_WIDTH, count%PckImage.IMAGE_WIDTH);
						}

						while (count >= 255)
						{
							binData.Add(TransparentId);
							binData.Add(255);
							count -= 255;
						}

						if (count != 0)
						{
							binData.Add(TransparentId);
							binData.Add((byte)count);
						}
						count = 0;
					}
					binData.Add(id);
				}
			}

			bool throughLoop = false;
			while (count >= 255)
			{
				binData.Add(254);
				binData.Add(255);
				count -= 255;
				throughLoop = true;
			}

			if ((byte)binData[binData.Count - 1] != 255 || throughLoop)
				binData.Add(255);

//			if (bytes.Count % 2 == 1 || throughLoop)
//				bytes.Add(255);

			output.Write(binData.ToArray());

			return binData.Count;
		}

//		public override void Hq2x()
//		{
//			if (Width == 32) // hasn't been done yet
//				base.Hq2x();
//		}

		public int StaticId
		{
			get { return _id; }
		}

/*		public static Type GetCollectionType()
		{
			return typeof(PckSpriteCollection);
		} */

/*		public void ReImage()
		{
			_image = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								Palette.Colors);
			_gray = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								Palette.Grayscale.Colors);
		} */

/*		public void MoveImage(byte offset)
		{
			_id[_moveId] = (byte)(_moveVal - offset);
			int ex = 0;
			int startIdx = 0;
			for (int i = 0; i != _expanded.Length; ++i)
				_expanded[i] = TransparentIndex;

			if (_id[0] != 254)
				ex = _id[startIdx++] * Width;

			for (int i = startIdx; i < _id.Length; ++i)
			{
				switch (_id[i])
				{
					case 254: // skip required pixels
						ex += _id[i + 1];
						++i;
						break;

					case 255: // end of image
						break;

					default:
						_expanded[ex++] = _id[i];
						break;
				}
			}
		
			_image = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								Palette.Colors);
			_gray = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								Palette.Grayscale.Colors);
		} */

/*		public int MapId
		{
			get { return _mapId; }
			set { _mapId = value; }
		} */

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

			if (_pckPack != null)
				ret += _pckPack.ToString();

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
	}
}
