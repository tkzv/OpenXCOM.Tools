using System;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	public delegate void LoadingDelegate(int curr, int total);


	public class PckFile
		:
		XCImageCollection
	{
		private int _bpp;

		public static readonly string TAB_EXT = ".tab";

		public PckFile(
					Stream pckFile,
					Stream tabFile,
					int bpp,
					Palette pal,
					int imgHeight,
					int imgWidth)
		{
			if (tabFile != null)
				tabFile.Position = 0;

			pckFile.Position = 0;

			var info = new byte[pckFile.Length];
			pckFile.Read(info, 0, info.Length);

			_bpp = bpp;

			Pal = pal;

			uint[] offsets;
			
			if (tabFile != null)
			{
				offsets = new uint[(tabFile.Length / bpp) + 1];
				var br = new BinaryReader(tabFile);

				if (bpp == 2)
					for (int i = 0; i < tabFile.Length / bpp; i++)
						offsets[i] = br.ReadUInt16();
				else
					for (int i = 0; i < tabFile.Length / bpp; i++)
						offsets[i] = br.ReadUInt32();

				br.Close();
			}
			else
			{
				offsets = new uint[2];
				offsets[0] = 0;
			}

			offsets[offsets.Length - 1] = (uint)info.Length;

			for (int i = 0; i < offsets.Length - 1; i++)
			{
				var imgDat = new byte[offsets[i + 1] - offsets[i]];
				for (int j = 0; j < imgDat.Length; j++)
					imgDat[j] = info[offsets[i] + j];

				Add(new PckImage(
							i,
							imgDat,
							pal,
							this,
							imgHeight,
							imgWidth));
			}
		}

		public PckFile(
					Stream pckFile,
					Stream tabFile,
					int bpp,
					Palette pal)
			:
			this(
				pckFile,
				tabFile,
				bpp,
				pal,
				40,
				32)
		{}

		public int Bpp
		{
			get { return _bpp; }
		}

		public static void Save(
							string directory,
							string file,
							XCImageCollection images,
							int bpp)
		{
			var pck = new BinaryWriter(File.Create(directory + @"\" + file + ".pck"));
			var tab = new BinaryWriter(File.Create(directory + @"\" + file + TAB_EXT));

			if (bpp == 2)
			{
				ushort count = 0;
				foreach (XCImage img in images)
				{
					tab.Write((ushort)count);
					ushort encLen = (ushort)PckImage.EncodePck(pck, img);
					count += encLen;
				}
			}
			else
			{
				uint count = 0;
				foreach(XCImage img in images)
				{
					tab.Write((uint)count);
					uint encLen = (uint)PckImage.EncodePck(pck, img);
					count += encLen;
				}
			}

			pck.Close();
			tab.Close();
		}
	}
}
