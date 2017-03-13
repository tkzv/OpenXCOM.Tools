using System;

using XCom.Interfaces;


namespace XCom.GameFiles.Images.XCFiles
{
	public class xcUncompressed
		:
		IXCImageFile
	{
		public xcUncompressed()
			:
			this(0, 0)
		{}

		public xcUncompressed(int wid, int hei)
			:
			base(wid, hei)
		{
			author	= "Ben Ratzlaff";
			ext		= ".unused";
			desc	= "Base class for opening uncompressed image files";
			expDesc	= "Uncompressed Images";

			fileOptions.Init(false, false, false, true);

//			fileOptions[Filter.Bmp]		= false;
//			fileOptions[Filter.Save]	= false;
//			fileOptions[Filter.Open]	= false;
//			fileOptions[Filter.Custom]	= true;
		}

		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			return new UncompressedCollection(
										imgWid,
										imgHei,
										System.IO.File.OpenRead(directory + "\\" + file),
										pal);
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCImageCollection images)
		{
			UncompressedCollection.Save(
									directory,
									file,
									FileExtension,
									images);
		}
	}
}
