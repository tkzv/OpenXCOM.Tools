using System;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	public delegate void LoadingDelegate(int cur, int total);


	public class PckFile
		:
		XCImageCollection
	{
		public static readonly string PckExt = ".pck";
		public static readonly string TabExt = ".tab";

		private int _bpp;
		public int Bpp
		{
			get { return _bpp; }
		}


		public PckFile(
					Stream strPck,
					Stream strTab,
					int bpp,
					Palette pal,
					int width,
					int height)
		{
			_bpp = bpp;
			Pal = pal;

			uint[] offsets;
			
			if (strTab != null)
			{
				strTab.Position = 0;

				offsets = new uint[(strTab.Length / bpp) + 1];
				using (var br = new BinaryReader(strTab))
				{
					switch (bpp)
					{
						case 2:
							for (int i = 0; i != strTab.Length / bpp; ++i)
								offsets[i] = br.ReadUInt16();
							break;
	
						default:
							for (int i = 0; i != strTab.Length / bpp; ++i)
								offsets[i] = br.ReadUInt32();
							break;
					}
				}
			}
			else
			{
				offsets = new uint[2];
				offsets[0] = 0;
			}


			strPck.Position = 0;

			var info = new byte[strPck.Length];
			strPck.Read(info, 0, info.Length);

			offsets[offsets.Length - 1] = (uint)info.Length;

			for (int i = 0; i != offsets.Length - 1; ++i)
			{
				var binData = new byte[offsets[i + 1] - offsets[i]];
				for (int j = 0; j != binData.Length; ++j)
					binData[j] = info[offsets[i] + j];

				Add(new PckImage(
							i,
							binData,
							pal,
							this,
							width,
							height));
			}
		}

		public PckFile(
					Stream strPck,
					Stream strTab,
					int bpp,
					Palette pal)
			:
			this(
				strPck,
				strTab,
				bpp,
				pal,
				32, 40)
		{}


		public static void Save(
				string directory,
				string file,
				XCImageCollection images,
				int bpp)
		{
			using (var pck = new BinaryWriter(File.Create(directory + @"\" + file + PckExt)))
			using (var tab = new BinaryWriter(File.Create(directory + @"\" + file + TabExt)))
			{
				switch (bpp)
				{
					case 2:
					{
						ushort count = 0;
						foreach (XCImage image in images)
						{
							tab.Write((ushort)count);
							ushort encLen = (ushort)PckImage.EncodePck(pck, image);
							count += encLen;
						}
						break;
					}

					default:
					{
						uint count = 0;
						foreach(XCImage image in images)
						{
							tab.Write((uint)count);
							uint encLen = (uint)PckImage.EncodePck(pck, image);
							count += encLen;
						}
						break;
					}
				}

//				pck.Close(); // NOTE: the 'using' block flushes and closes the stream.
//				tab.Close();
			}
		}
	}
}
