using System;
using System.IO;

using XCom.Interfaces;


namespace XCom.GameFiles.Images.XCFiles
{
	public class xcBdy
		:
		IXCImageFile
	{
		public xcBdy()
			:
			this(320, 200)
		{}

		public xcBdy(int wid, int hei)
			:
			base(wid, hei)
		{
			author	= "Ben Ratzlaff";
			ext		= ".bdy";
			desc	= "Bdy file codec";
			expDesc	= "BDY Image";

			defPal = Palette.UFOResearch;

			fileOptions.Init(true, false, true, true);
		}

		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			var collection = new XCImageCollection();
			XCImage image = new BdyImage(
										pal,
										File.OpenRead(directory + @"\" + file),
										imgWid,
										imgHei);
			collection.Add(image);

			return collection;
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCImageCollection images)
		{
			switch (images.Count)
			{
				case 1:
					BdyImage.Save(images[0].Bytes, File.OpenWrite(directory + @"\" + file + ext));
					break;

				default:
					for (int i = 0; i < images.Count; i++)
						BdyImage.Save(
									images[i].Bytes,
									File.OpenWrite(directory + @"\" + file + i.ToString() + ext));
					break;
			}
		}
	}
}
