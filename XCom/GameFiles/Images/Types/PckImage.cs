using System;
using System.Collections.Generic;

using XCom.Interfaces;


namespace XCom
{
	public class PckImage // [xImage(32,40)]
		:
		XCImage
	{
//		private int _mapId;

		private readonly PckFile _pckFile;
		private readonly byte[] _expanded;
		private int _moveId = -1;
		private byte _moveVal = 0;

		private static int _globalStaticId = 0;
		private int _staticId;

		public static int Width  = 32;
		public static int Height = 40;

		private const byte TransId = 0xFE;


/*		internal PckImage(
				int imageId,
				byte[] id,
				Palette pal,
				PckFile pckFile)
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
				byte[] id,
				Palette pal,
				PckFile pckFile,
				int width,
				int height)
		{
			Palette = pal;
			_pckFile = pckFile;
			_fileId = imageId;

//			this.imageNum = imageNum;
//			this.idx = idx;

			_staticId = _globalStaticId++;

			Width  = width;
			Height = height;

//			image = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
			_expanded = new byte[Width * Height];

			for (int i = 0; i < _expanded.Length; i++)
				_expanded[i] = TransparentIndex;

			int startId = 0;
			int expandedId = 0;

			if (id[0] != 254)
				expandedId = id[startId++] * Width;

			for (int i = startId; i < id.Length; i++)
			{
				switch (id[i])
				{
					case 254: // skip required pixels
						if (_moveId == -1)
						{
							_moveId = i + 1;
							_moveVal = id[i + 1];
						}
						expandedId += id[i + 1];
						++i;
						break;

					case 255: // end of image
						break;

					default:
						_expanded[expandedId++] = id[i];
						break;
				}
			}
			_id = _expanded;
		
			_image = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								pal.Colors);
			_gray = Bmp.MakeBitmap8(
								Width,
								Height,
								_expanded,
								pal.Grayscale.Colors);
		}


		public static int EncodePck(System.IO.BinaryWriter output, XCImage tile)
		{
			int count = 0;
			bool flag = true;

			byte[] input = tile.Bytes;

			var bytes = new List<byte>();

//			Color trans = pal.Transparent;
//			pal.SetTransparent(false);

			int totalCount = 0;
			for (int i = 0; i < input.Length; i++)
			{
				byte idx = input[i];
				totalCount++;

				if (idx == TransId)
					count++;
				else
				{
					if (count != 0)
					{
						if (flag)
						{
							flag = false;

							bytes.Add((byte)(count / tile.Image.Width));	// # of initial rows to skip
							count   = (byte)(count % tile.Image.Width);		// where we currently are in the transparent row
							//Console.WriteLine("count, lines: {0}, cells {1}", count/PckImage.IMAGE_WIDTH, count%PckImage.IMAGE_WIDTH);
						}

						while (count >= 255)
						{
							bytes.Add(TransId);
							bytes.Add(255);
							count -= 255;
						}

						if (count != 0)
						{
							bytes.Add(TransId);
							bytes.Add((byte)count);
						}
						count = 0;
					}
					bytes.Add(idx);
				}
			}

			bool throughLoop = false;
			while (count >= 255)
			{
				bytes.Add(254);
				bytes.Add(255);
				count -= 255;
				throughLoop = true;
			}

			if ((byte)bytes[bytes.Count - 1] != 255 || throughLoop)
				bytes.Add(255);

//			if (bytes.Count % 2 == 1 || throughLoop)
//				bytes.Add(255);

			output.Write(bytes.ToArray());

			return bytes.Count;
		}

//		public override void Hq2x()
//		{
//			if (Width == 32) // hasn't been done yet
//				base.Hq2x();
//		}

		public int StaticId
		{
			get { return _staticId; }
		}

/*		public static Type GetCollectionType()
		{
			return typeof(PckFile);
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

			if (_pckFile != null)
				ret += _pckFile.ToString();

			ret += _fileId + Environment.NewLine;

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
