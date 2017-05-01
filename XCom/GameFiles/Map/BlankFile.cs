using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	public static class BlankFile
	{
		public static readonly string BlankExt = ".BLK";


		public static void LoadBlank(
				string file,
				string path,
				XCMapBase mapBase)
		{
			using (var br = new BinaryReader(File.OpenRead(path + file + BlankExt)))
			{
				bool flip = true;
				int i = 0;

				while (br.BaseStream.Length > br.BaseStream.Position)
				{
					int inconspicuousVariable = (int)br.ReadUInt16();

					if (flip)
					{
						for (int j = i; j < i + inconspicuousVariable; ++j)
						{
							int l =  j / (mapBase.MapSize.Rows  * mapBase.MapSize.Cols);
							int c =  j %  mapBase.MapSize.Cols;
							int r = (j /  mapBase.MapSize.Cols) - mapBase.MapSize.Rows * l;

							((XCMapTile)mapBase[r, c, l]).DrawAbove = false;
						}
					}

					i += inconspicuousVariable;
					flip = !flip;
				}
			}
		}

		public static void SaveBlank(
				string file,
				string path,
				XCMapBase mapBase)
		{
			Directory.CreateDirectory(path);

			using (var bw = new BinaryWriter(new FileStream(path + file + BlankExt, FileMode.Create)))
			{
				bool flip = true;
				UInt16 i = 0;

				for (int l = 0; l != mapBase.MapSize.Levs; ++l)
					for (int r = 0; r != mapBase.MapSize.Rows; ++r)
						for (int c = 0; c != mapBase.MapSize.Cols; ++c)
						{
							if (   ( flip &&  ((XCMapTile)mapBase[r, c, l]).DrawAbove)
								|| (!flip && !((XCMapTile)mapBase[r, c, l]).DrawAbove))
							{
								bw.Write(i);

								flip = !flip;
								i = 1;
							}
							else
								++i;
						}
			}
		}
	}
}
