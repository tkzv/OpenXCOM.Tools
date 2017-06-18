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


		#region Properties
		public int TabOffset
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Parses a PCK-file into the collection of its images according
		/// to its TAB-file.
		/// </summary>
		/// <param name="fsPck"></param>
		/// <param name="fsTab"></param>
		/// <param name="tabOffset"></param>
		/// <param name="pal"></param>
		public SpriteCollection(
				Stream fsPck,
				Stream fsTab,
				int tabOffset,
				Palette pal)
		{
			TabOffset = tabOffset;

			Pal = pal;

			uint[] offsets;

			if (fsTab != null)
			{
				int sprites = (int)fsTab.Length / tabOffset;

				fsTab.Position = 0;

				offsets = new uint[sprites + 1]; // NOTE: the last entry will be set to the total length of the input-bindata.
				using (var br = new BinaryReader(fsTab))
				{
					switch (tabOffset)
					{
						case 2:
							for (int i = 0; i != sprites; ++i)
								offsets[i] = br.ReadUInt16();
							break;

						case 4:
							for (int i = 0; i != sprites; ++i)
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


			fsPck.Position = 0;

			var bindata = new byte[(int)fsPck.Length];
			fsPck.Read(
					bindata,			// buffer
					0,					// offset
					bindata.Length);	// count

			offsets[offsets.Length - 1] = (uint)bindata.Length;

			for (int i = 0; i != offsets.Length - 1; ++i)
			{
				var bindataSprite = new byte[offsets[i + 1] - offsets[i]];

				for (int j = 0; j != bindataSprite.Length; ++j)
					bindataSprite[j] = bindata[offsets[i] + j];

				Add(new PckImage(
								bindataSprite,
								Pal,
								i,
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
		/// <param name="tabOffset">2 for UFO, 4 for TFTD (roughly..)</param>
		public static void SaveSpriteset(
				string dir,
				string file,
				SpriteCollectionBase spriteset,
				int tabOffset)
		{
			string pfePck = Path.Combine(dir, file + PckExt);
			string pfeTab = Path.Combine(dir, file + TabExt);

			using (var bwPck = new BinaryWriter(File.Create(pfePck)))
			using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
			{
				switch (tabOffset)
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
