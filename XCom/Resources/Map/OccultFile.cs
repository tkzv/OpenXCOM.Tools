using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	internal static class OccultFile
	{
		internal const string OccultExt = ".OTD";


		internal static void LoadOccult(
				string file,
				string path,
				MapFileBase mapBase)
		{
			using (var br = new BinaryReader(File.OpenRead(path + file + OccultExt)))
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
							int lev = j / (mapBase.MapSize.Rows * mapBase.MapSize.Cols);
							int col = j %  mapBase.MapSize.Cols;
							int row = j /  mapBase.MapSize.Cols - mapBase.MapSize.Rows * lev;

							((XCMapTile)mapBase[row, col, lev]).Occulted = true;
						}
					}

					i += inconspicuousVariable;
					flip = !flip;
				}
			}
		}

		internal static void SaveOccult(
				string file,
				string path,
				MapFileBase mapBase)
		{
			Directory.CreateDirectory(path);

			using (var bw = new BinaryWriter(new FileStream(path + file + OccultExt, FileMode.Create)))
			{
				bool flip = true;
				UInt16 i = 0;

				for (int lev = 0; lev != mapBase.MapSize.Levs; ++lev)
				for (int row = 0; row != mapBase.MapSize.Rows; ++row)
				for (int col = 0; col != mapBase.MapSize.Cols; ++col)
				{
					if (   ( flip && !((XCMapTile)mapBase[row, col, lev]).Occulted)
						|| (!flip &&  ((XCMapTile)mapBase[row, col, lev]).Occulted))
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
