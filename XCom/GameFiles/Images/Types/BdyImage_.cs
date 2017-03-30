/*
using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	/// <summary>
	/// Summary description for BdyImage.
	/// </summary>
	public class BdyImage
		:
		XCImage
	{
		public BdyImage(
				Palette pal,
				Stream str,
				int width,
				int height)
		{
			using (var data = new BinaryReader(str)) // NOTE: passing in the stream may obviate 'using'.
			{
				_id = new byte[width * height];
				for(int i = 0; i < _id.Length; i++)
					_id[i] = 254;

				int x = 0;

				while (data.BaseStream.Position < data.BaseStream.Length)
				{
					int space = data.ReadByte();
					byte c = data.ReadByte();

					if (space >= 129)
					{
						space = 256 - space + 1;
						for (int i = 0; i < space; i++)
							_id[x++] = c;
					}
					else
					{
						_id[x++] = c;
						for (int i = 0; i < space; i++)
						{
							c = data.ReadByte();
							_id[x++] = c;
						}
					}
				}
				_image = Bmp.MakeBitmap8(320, 200, _id, pal.Colors);
				Palette = pal;

				data.Close(); // TODO: Check if it's okay to pass the stream into 'using' above^
			}
		}

//		public override byte TransparentIndex
//		{
//			get { return 0; }
//		}

		public static void Save(byte[] image, Stream str)
		{
			using (var bw = new BinaryWriter(str)) // NOTE: passing in the stream may obviate 'using'.
			{
				var al = new List<BdyNode>();

				var last = new BdyNode(image[0]);
				al.Add(last);

				int count = 0;
				for (int i = 1; i < image.Length; ++i)
				{
					++count;
					var cur = new BdyNode(image[i]);
					
					if (count % 320 == 0)
					{
						last = cur;
						al.Add(last);
						count = 0;
						continue;
					}

					if (cur._data == last._data) // and we have a match!
					{
						if (last._count < 128)
							++last._count;
						else
						{
							last = cur;
							al.Add(last);
						}
					}
					else
					{
						last = cur;
						al.Add(last);
					}
				}

				count = 0;
				var tmp = new List<BdyNode>();
				foreach (BdyNode node in al)
				{
					if (node._count == 1)
						tmp.Add(node);
					else if (node._count == 2 && tmp.Count != 0)
					{
						tmp.Add(node);
						tmp.Add(node);
					}
					else // write out what's in the array list, write out our value, reset arraylist
					{
						if (tmp.Count > 0)
						{
							if (count + tmp.Count >= 320)
							{
								int left = 320-count;
								if (left > 0)
								{
									bw.Write((byte)(left - 1));
									for (int i = 0; i < left; i++)
										bw.Write((byte)tmp[i]._data);

									int left2 = tmp.Count - left;
									if (left2 > 0)
									{
										bw.Write((byte)(left2 - 1));
										for (int i = 0; i < left2; i++)
											bw.Write((byte)tmp[left + i]._data);
									}
									count = left2;
								}
								else
								{
									bw.Write((byte)(tmp.Count - 1));
									count += tmp.Count;
									for (int i = 0; i < tmp.Count; i++)
										bw.Write((byte)tmp[i]._data);
								}
							}
							else
							{
								bw.Write((byte)(tmp.Count - 1));
								count += tmp.Count;
								for (int i = 0; i < tmp.Count; i++)
									bw.Write((byte)tmp[i]._data);
							}
							tmp = new List<BdyNode>();
						}

						bw.Write((byte)(256 - node._count + 1));
						count += node._count;
						bw.Write(node._data);

						if (count > 320)
							count -= 320;
					}
				}

				if (tmp.Count > 0)
				{
					bw.Write((byte)(tmp.Count - 1));

					for (int i = 0; i < tmp.Count; i++)
						bw.Write(tmp[i]._data);

					tmp = new List<BdyNode>();
				}

				bw.Flush(); // TODO: Check if it's okay to pass the stream into 'using' above^
				bw.Close();
			}
		}

//		private enum BdyNodeType
//		{
//			DataOnly,
//			RunLength
//		};

		private class BdyNode
		{
//			public BdyNodeType myType;

			public byte _data;
			public byte _count;

			public BdyNode(byte data)
			{
				_data = data;
				_count = 1;

//				myType = BdyNodeType.DataOnly;
			}

			public BdyNode(byte count, byte data)
			{
				_data = data;
				_count = count;

//				myType = BdyNodeType.RunLength;
			}
		}
	}
}
*/
