using System;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	/// <summary>
	/// A spriteset: a collection of images taken from PCK/TAB file data.
	/// </summary>
	public sealed class SpriteCollection
		:
			SpriteCollectionBase
	{
		#region Fields (static)
		public const string PckExt = ".PCK";
		public const string TabExt = ".TAB";
		#endregion


//		#region Properties
//		private int _lenTabOffset;
//		public int TabOffset
//		{
//			get { return _lenTabOffset; }
//		}
//		#endregion


		#region cTor
		/// <summary>
		/// cTor. Parses a PCK-file into the collection of its images according
		/// to its TAB-file.
		/// </summary>
		/// <param name="strPck"></param>
		/// <param name="strTab"></param>
		/// <param name="lenTabOffset"></param>
		/// <param name="pal"></param>
		public SpriteCollection(
				Stream strPck,
				Stream strTab,
				int lenTabOffset,
				Palette pal)
		{
//			_lenTabOffset = lenTabOffset;

			Pal = pal;

			uint[] offsets;
			
			if (strTab != null)
			{
				strTab.Position = 0;

				offsets = new uint[(strTab.Length / lenTabOffset) + 1];
				using (var br = new BinaryReader(strTab))
				{
					switch (lenTabOffset)
					{
						case 2:
							for (int i = 0; i != strTab.Length / lenTabOffset; ++i)
								offsets[i] = br.ReadUInt16();
							break;
	
						case 4:
							for (int i = 0; i != strTab.Length / lenTabOffset; ++i)
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
								bindata,
								Pal,
								id,
								this));
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Saves the current spriteset to PCK+TAB.
		/// </summary>
		/// <param name="dir">the directory to save to</param>
		/// <param name="file">the filename without extension</param>
		/// <param name="spriteset">pointer to the base spriteset</param>
		/// <param name="lenTabOffset">2 for UFO, 4 for TFTD (roughly..)</param>
		public static void SaveSpriteset(
				string dir,
				string file,
				SpriteCollectionBase spriteset,
				int lenTabOffset)
		{
			string pfePck = Path.Combine(dir, file + PckExt);
			string pfeTab = Path.Combine(dir, file + TabExt);

			using (var bwPck = new BinaryWriter(File.Create(pfePck)))
			using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
			{
				switch (lenTabOffset)
				{
					case 2:
					{
						ushort pos = 0;
						foreach (XCImage sprite in spriteset)
						{
							bwTab.Write(pos);
							pos += (ushort)PckImage.SaveSpritesetSprite(bwPck, sprite);
						}
						break;
					}

					case 4:
					{
						uint pos = 0;
						foreach (XCImage sprite in spriteset)
						{
							bwTab.Write(pos);
							pos += (uint)PckImage.SaveSpritesetSprite(bwPck, sprite);
						}
						break;
					}
				}
			}
		}
		#endregion
	}
}
