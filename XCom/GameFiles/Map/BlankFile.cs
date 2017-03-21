using System;
using System.IO;


namespace XCom
{
	public static class BlankFile
	{
		public static readonly string BlankExt = ".BLK";


		public static void LoadBlank(
				string baseName,
				string blankPath,
				XCom.Interfaces.Base.IMap_Base file)
//				XCMapFile file)
		{
			using (var br = new BinaryReader(File.OpenRead(blankPath + baseName + BlankExt)))
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
							int h =  j / (file.MapSize.Rows  * file.MapSize.Cols);
							int c =  j %  file.MapSize.Cols;
							int r = (j /  file.MapSize.Cols) - file.MapSize.Rows * h;

							((XCMapTile)file[r, c, h]).DrawAbove = false;
						}
					}

					i += inconspicuousVariable;
					flip = !flip;
				}

//				br.Close(); // NOTE: the 'using' block closes the stream.
			}
		}

		public static void SaveBlank(
				string baseName,
				string blankPath,
				XCom.Interfaces.Base.IMap_Base file)
//				XCMapFile file)
		{
			Directory.CreateDirectory(blankPath);

			using (var bw = new BinaryWriter(new FileStream(blankPath + baseName + BlankExt, FileMode.Create)))
			{
				bool flip = true;
				UInt16 i = 0;

				for (int h = 0; h != file.MapSize.Height; ++h)
					for (int r = 0; r != file.MapSize.Rows; ++r)
						for (int c = 0; c != file.MapSize.Cols; ++c)
						{
							if (   ( flip &&  ((XCMapTile)file[r, c, h]).DrawAbove)
								|| (!flip && !((XCMapTile)file[r, c, h]).DrawAbove))
							{
								bw.Write(i);

								flip = !flip;
								i = 1;
							}
							else
								++i;
						}

//				bw.Flush();
//				bw.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
		}
	}
}
