using System;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public class xcProfile
		:
		IXCImageFile
	{
		public static readonly string PROFILE_EXT = ".pvp";

		private IXCImageFile _codec;


		public xcProfile()
			:
			base(0, 0)
		{
			fileOptions.Init(false, false, false, false);

			author	= "Ben Ratzlaff";
			desc	= "Provides profile support";

			ext = PROFILE_EXT;
		}

		public xcProfile(ImgProfile profile)
			:
			base(0, 0)
		{
			imageSize = new System.Drawing.Size(profile.ImgWid, profile.ImgHei);

			_codec = profile.ImgType;

			desc	= profile.Description;
			expDesc	= profile.Description;
			ext		= profile.Extension;
			author	= "Profile";

			if (profile.OpenSingle != "")
				singleFile = profile.OpenSingle;

			fileOptions.Init(false, true, true, false);

			xConsole.AddLine("Profile created: " + desc);

			try
			{
				defPal = XCom.SharedSpace.Instance.GetPaletteTable()[profile.Palette];
			}
			catch
			{
				defPal = XCom.Palette.UFOBattle;
//				defPal = XCom.Palette.TFTDBattle;
			}
		}


		public IXCImageFile Codec
		{
			get { return _codec; }
			set { _codec = value; }
		}

		protected override XCom.XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			return _codec.LoadFile(
								directory,
								file,
								imgWid,
								imgHei,
								pal);
		}

		public override void SaveCollection(
				string directory,
				string file,
				XCom.XCImageCollection images)
		{
			_codec.SaveCollection(
							directory,
							file,
							images);
		}
	}
}
