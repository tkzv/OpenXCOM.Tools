using System;
using System.IO;


namespace XCom
{
	public static class BlankFile
	{
		public static readonly string BlankExt = ".BLK";

		public static void LoadBlanks(
				string baseName,
				string blankPath,
				XCom.Interfaces.Base.IMap_Base file)
//				XCMapFile file)
		{
			using (var br = new BinaryReader(File.OpenRead(blankPath + baseName + BlankExt)))
			{
				bool flip = true;
				int cur = 0;

				while (br.BaseStream.Length > br.BaseStream.Position)
				{
					UInt16 v = br.ReadUInt16();

					if (flip)
					{
						for (int i = cur; i < cur + v; i++)
						{
							int h = i / (file.MapSize.Rows * file.MapSize.Cols);
							int c = i % file.MapSize.Cols;
							int r = (i / file.MapSize.Cols) - h * file.MapSize.Rows;

							((XCMapTile)file[r, c, h]).DrawAbove = false;
						}
					}

					cur += v;
					flip = !flip;
				}

//				br.Close(); // NOTE: the 'using' block closes the stream.
			}
		}

		public static void SaveBlanks(
				string baseName,
				string blankPath,
				XCom.Interfaces.Base.IMap_Base file)
//				XCMapFile file)
		{
			if (!Directory.Exists(blankPath))
				Directory.CreateDirectory(blankPath);

			using (var bw = new BinaryWriter(new FileStream(blankPath + baseName + BlankExt, FileMode.Create)))
			{
				UInt16 cur = 0;
				bool flip = true;

				for (int h = 0; h < file.MapSize.Height; h++)
					for (int r = 0; r < file.MapSize.Rows; r++)
						for (int c = 0; c < file.MapSize.Cols; c++)
						{
							if (flip)
							{
								if (((XCMapTile)file[r, c, h]).DrawAbove)
								{
									flip = !flip;
									bw.Write(cur);
									cur = 1;
								}
								else
									cur++;
							}
							else
							{
								if (((XCMapTile)file[r, c, h]).DrawAbove)
									cur++;
								else
								{
									flip = !flip;
									bw.Write(cur);
									cur = 1;
								}
							}
						}

//				bw.Flush();
//				bw.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
		}
	}
}
