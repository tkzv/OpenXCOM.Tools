/*
using System;
using XCom.Interfaces;


namespace XCom
{
	public class UncompressedCollection
		:
			XCImageCollection
	{
		public UncompressedCollection(
				int width,
				int height,
				System.IO.Stream inFile,
				Palette pal)
		{
			using (var sr = new System.IO.BinaryReader(inFile))
			{
				int i = 0;
				while (sr.BaseStream.Position < sr.BaseStream.Length)
					Add(new XCImage(
								sr.ReadBytes(width * height),
								width,
								height,
								pal,
								i++));
			}
		}


		public static void Save(
				string dir,
				string file,
				string ext,
				XCImageCollection images)
		{
			using (var bw = new System.IO.BinaryWriter(System.IO.File.Create(dir + @"\" + file + ext)))
			{
				foreach (XCImage tile in images)
					bw.Write(tile.Offsets);
			}
		}
	}
}
*/
