using System;
using System.IO;

using XCom.Interfaces;


namespace XCom.GameFiles.Images.XCFiles
{
	public class xcSpk
		:
		IXCImageFile
	{
		public xcSpk()
			:
			this(320, 200)
		{}

		public xcSpk(int wid, int hei)
			:
			base(wid, hei)
		{
			author	= "Ben Ratzlaff";
			ext		= ".spk";
			desc	= "Spk file codec";
			expDesc	= "SPK Image";

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
			var collect = new XCImageCollection();
			XCImage img = new SPKImage(
									pal,
									File.OpenRead(directory + @"\" + file),
									imgWid,
									imgHei);
			collect.Add(img);

			return collect;
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCImageCollection images)
		{
			switch (images.Count)
			{
				case 1:
					SPKImage.Save(
								images[0].Bytes,
								File.OpenWrite(directory + @"\" + file + ext));
					break;

				default:
					for (int i = 0; i < images.Count; i++)
						SPKImage.Save(
									images[i].Bytes,
									File.OpenWrite(directory + @"\" + file + i.ToString() + ext));
					break;
			}
		}
	}
}
