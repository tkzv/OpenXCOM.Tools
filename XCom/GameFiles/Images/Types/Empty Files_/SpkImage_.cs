/*
using System;
using System.Collections;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	public class SpkImage
		:
		XCImage
	{
		public SpkImage(
					Palette pal,
					Stream str,
					int width,
					int height)
		{
			_id = new byte[width * height];
			for (int i = 0; i < _id.Length; i++)
				_id[i] = 254;

			long pix = 0;

			var data = new BinaryReader(str);

			try
			{
				while(data.BaseStream.Position < data.BaseStream.Length)
				{
					int cas = data.ReadUInt16();
					switch (cas)
					{
						case 0xFFFF:
						{
							long val = data.ReadUInt16() * 2;
							pix += val;
							break;
						}

						case 0xFFFE:
						{
							long val = data.ReadUInt16() * 2;
							while (val-- > 0)
								_id[pix++] = data.ReadByte();
							break;
						}

						case 0xFFFD:
						{
							_image = Bmp.MakeBitmap8(
												width,
												height,
												_id,
												pal.Colors);
							Palette = pal;
							data.Close();
							return;
						}
					}
				}
			}
			catch {} // TODO: that.

			_image = Bmp.MakeBitmap8(
								width,
								height,
								_id,
								pal.Colors);
			Palette = pal;
			data.Close();
		}

		public static void Save(byte[] img, Stream file)
		{
			const byte transparent = 254;

			//Console.WriteLine("length: " + img.Length);

			var data = new BinaryWriter(file);
			var toWrite = new ArrayList();

			int count = 0;

			for (int i = 0; i < img.Length; i++)
			{
				if (img[i] == transparent)
				{
					if (toWrite.Count != 0)
					{
						if (toWrite.Count % 2 == 1) // odd number of items in the list
						{
							toWrite.Add(img[i]);	// add transparent index to make it even
							count--;				// don't want this index to count
						}

						data.Write((ushort)0xFFFE);
						data.Write((ushort)(toWrite.Count / 2));

						foreach (byte b in toWrite)
							data.Write((byte)b);

						toWrite = new ArrayList();
					}
					count++;
				}
				else
				{
					if (count != 0)
					{
						if (count % 2 != 0)
						{
							toWrite.Add(transparent); // add to list to make the count even
							count--; // don't need to do this, but owell
						}
						data.Write((ushort)0xFFFF);
						data.Write((ushort)(count / 2));
						count = 0;
					}
					toWrite.Add(img[i]);
				}
			}

			data.Write((ushort)0xFFFE);
			data.Write((ushort)(toWrite.Count / 2));

			foreach(byte b in toWrite)
				data.Write((byte)b);

			data.Write((ushort)0xFFFD);

			data.Flush();
			data.Close();
		}
	}
}
*/
