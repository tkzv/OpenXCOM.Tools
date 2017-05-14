using System;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	internal static class BlankFile
	{
		internal static readonly string BlankExt = ".BLK";


		internal static void LoadBlank(
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
							int lev =  j / (mapBase.MapSize.Rows  * mapBase.MapSize.Cols);
							int col =  j %  mapBase.MapSize.Cols;
							int row = (j /  mapBase.MapSize.Cols) - mapBase.MapSize.Rows * lev;

							((XCMapTile)mapBase[row, col, lev]).DrawAbove = false;
						}
					}

					i += inconspicuousVariable;
					flip = !flip;
				}
			}
		}

		internal static void SaveBlank(
				string file,
				string path,
				XCMapBase mapBase)
		{
			Directory.CreateDirectory(path);

			using (var bw = new BinaryWriter(new FileStream(path + file + BlankExt, FileMode.Create)))
			{
				bool flip = true;
				UInt16 i = 0;

				for (int lev = 0; lev != mapBase.MapSize.Levs; ++lev)
					for (int row = 0; row != mapBase.MapSize.Rows; ++row)
						for (int col = 0; col != mapBase.MapSize.Cols; ++col)
						{
							if (   ( flip &&  ((XCMapTile)mapBase[row, col, lev]).DrawAbove)
								|| (!flip && !((XCMapTile)mapBase[row, col, lev]).DrawAbove))
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
