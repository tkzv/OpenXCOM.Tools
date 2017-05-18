using System;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	public delegate void LoadingEventHandler(int cur, int total);


	/// <summary>
	/// A spriteset: a collection of images taken from PCK/TAB file data.
	/// </summary>
	public sealed class PckSpriteCollection
		:
			XCImageCollection
	{
		#region Constants
		public const string PckExt = ".PCK";
		public const string TabExt = ".TAB";
		#endregion


		#region Fields & Properties
		private int _bpp;
		public int Bpp
		{
			get { return _bpp; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Parses a PCK-file into the collection of its images according
		/// to its TAB-file.
		/// </summary>
		/// <param name="strPck"></param>
		/// <param name="strTab"></param>
		/// <param name="bpp"></param>
		/// <param name="pal"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public PckSpriteCollection(
				Stream strPck,
				Stream strTab,
				int bpp,
				Palette pal,
				int width  = 32,
				int height = 40)
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

			for (int id = 0; id != offsets.Length - 1; ++id)
			{
				var bindata = new byte[offsets[id + 1] - offsets[id]];
				for (int j = 0; j != bindata.Length; ++j)
					bindata[j] = info[offsets[id] + j];

				Add(new PckImage(
								id,
								bindata,
								pal,
								this,
								width,
								height));
			}
		}
		#endregion


		#region Methods
		public static void Save(
				string dir,
				string file,
				XCImageCollection images,
				int bpp)
		{
			string pfePck = Path.Combine(dir, file + PckExt);
			string pfeTab = Path.Combine(dir, file + TabExt);

			using (var bwPck = new BinaryWriter(File.Create(pfePck)))
			using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
			{
				switch (bpp)
				{
					case 2:
					{
						ushort count = 0;
						foreach (XCImage image in images)
						{
							bwTab.Write(count);
							ushort len = (ushort)PckImage.EncodePckData(bwPck, image);
							count += len;
						}
						break;
					}

					default:
					{
						uint count = 0;
						foreach (XCImage image in images)
						{
							bwTab.Write(count);
							uint len = (uint)PckImage.EncodePckData(bwPck, image);
							count += len;
						}
						break;
					}
				}
			}
		}
		#endregion
	}
}
