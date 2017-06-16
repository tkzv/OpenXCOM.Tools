using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	// kL_note: It appears that RLE-encoded .Pck images cannot use color-indices
	// of 0xFE or 0xFF. Because, in the Pck-packed file, 0xFF is used to denote
	// the end of each image's bindata, and 0xFE is used to flag a quantity of
	// pixels as transparent by using 2 bytes: 0xFE itself, and the next byte is
	// the quantity of pixels that are transparent and hence are not written.

	public sealed class PckImage
		:
			XCImage
	{
		#region Fields (static)
		/// <summary>
		/// A flag that is inserted into the file-data that indicates that an
		/// image's data has ended.
		/// </summary>
		private const byte FileStopByte = 0xFF;
		/// <summary>
		/// A flag that is inserted into the file-data that indicates that the
		/// following pixels are transparent.
		/// </summary>
		private const byte FileTransparencyByte = 0xFE;	// the PCK-file uses 0xFE to flag a succeeding quantity of pixels
														// as transparent. That is, it is *not* a color-id entry; it's
														// just a flag in the Pck-file. Stop using it as a color-id entry.
		/// <summary>
		/// Tracks the id of an image across all loaded terrainsets. Used only
		/// by 'MapInfoOutputBox'.
		/// </summary>
		private static int _idCanonical;
		#endregion


		#region Fields
		private readonly SpriteCollection _spriteset; // currently used only for ToString() override.
		#endregion


		#region Properties
		/// <summary>
		/// MapId is used only by 'MapInfoOutputBox'.
		/// </summary>
		public int MapId
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// Instantiates a PckImage, based on an XCImage.
		/// </summary>
		/// <param name="bindata">the compressed source data</param>
		/// <param name="pal"></param>
		/// <param name="terrainId"></param>
		/// <param name="spriteset"></param>
		internal PckImage(
				byte[] bindata,
				Palette pal,
				int terrainId,
				SpriteCollection spriteset)
			:
				base(
					new byte[XCImageFile.SpriteWidth * XCImageFile.SpriteHeight],
					XCImageFile.SpriteWidth,
					XCImageFile.SpriteHeight,
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					terrainId)
		{
			_spriteset = spriteset; // for ToString() only.
			MapId = _idCanonical++; // for 'MapInfoOutputBox' only.

			Pal = pal;

			for (int id = 0; id != Bindata.Length; ++id)
				Bindata[id] = Palette.TransparentId;	// good effing Lord. yeh set transparent pixels to #254 instead of #0, sure.
														// as if that's not going to confuse anyone who tries to paint with color #0
			int posSrc = 0;								// only to find it's not actually transparent. Oh yeah, they're supposed to
			int posDst = 0;								// be clever and figure out that the transparency-marker in the PCK-file has
														// the value of 0xFE .... NOBODY CARES.
			if (bindata[0] != FileTransparencyByte)
				posDst = bindata[posSrc++] * XCImageFile.SpriteWidth;

			for (int id = posSrc; id != bindata.Length; ++id)
			{
				switch (bindata[id])
				{
					default:
						Bindata[posDst++] = bindata[id];
						break;

					case FileTransparencyByte: // skip quantity of pixels
						posDst += bindata[++id];
						break;

					case FileStopByte: // end of image
						break;
				}
			}

			Image = BitmapService.MakeBitmapTrue(
												XCImageFile.SpriteWidth,
												XCImageFile.SpriteHeight,
												Bindata,
												Pal.ColorTable);
			SpriteGray = BitmapService.MakeBitmapTrue(
												XCImageFile.SpriteWidth,
												XCImageFile.SpriteHeight,
												Bindata,
												Pal.Grayscale.ColorTable);
		}
		#endregion


		#region Methods
		internal static int SaveSpritesetSprite(BinaryWriter bw, XCImage sprite)
		{
			var binlist = new List<byte>();

			int pos = 0;
			bool first = true;

			for (int id = 0; id != sprite.Bindata.Length; ++id)
			{
				byte b = sprite.Bindata[id];

				if (b == Palette.TransparentId)
					++pos;
				else
				{
					if (pos != 0)
					{
						if (first)
						{
							first = false;

							binlist.Add((byte)(pos / sprite.Image.Width));	// # of initial rows to skip
							pos =       (byte)(pos % sprite.Image.Width);	// current position in the next row
						}

						while (pos >= FileStopByte)
						{
							pos -= FileStopByte;

							binlist.Add(FileTransparencyByte);
							binlist.Add(FileStopByte);
						}

						if (pos != 0)
						{
							binlist.Add(FileTransparencyByte);
							binlist.Add((byte)pos);
						}
						pos = 0;
					}
					binlist.Add(b);
				}
			}

			bool hasloopedthroughthethingdob = false;
			while (pos >= FileStopByte)
			{
				pos -= FileStopByte;

				binlist.Add(FileTransparencyByte);
				binlist.Add(FileStopByte);

				hasloopedthroughthethingdob = true;
			}

			if (hasloopedthroughthethingdob
				|| (byte)binlist[binlist.Count - 1] != FileStopByte)
			{
				binlist.Add(FileStopByte);
			}

			bw.Write(binlist.ToArray());

			return binlist.Count;
		}
		#endregion


		#region Methods (override)
		public override string ToString()
		{
			string ret = String.Empty;

			if (_spriteset != null)
				ret += _spriteset.ToString();

			ret += TerrainId + Environment.NewLine;

			for (int i = 0; i != Bindata.Length; ++i)
			{
				ret += Bindata[i];

				switch (Bindata[i])
				{
					case FileStopByte:
						ret += Environment.NewLine;
						break;

					default:
						ret += " ";
						break;
				}
			}
			return ret;
		}

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
		#endregion
	}
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
//			if (_id[0] != FileTransparencyByte)
//				ex = _id[startIdx++] * Width;
//
//			for (int i = startIdx; i < _id.Length; ++i)
//			{
//				switch (_id[i])
//				{
//					case FileTransparencyByte: // skip quantity of pixels
//						ex += _id[i + 1];
//						++i;
//						break;
//
//					case FileStopByte: // end of image
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
