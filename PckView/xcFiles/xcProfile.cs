using System;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public class XCProfile
		:
		IXCImageFile
	{
		public static readonly string ProfileExt = ".pvp";

		private readonly IXCImageFile _codec;


/*		public XCProfile()
			:
			base(0, 0)
		{
			_fileOptions.Init(false, false, false, false);

			author	= "Ben Ratzlaff";
			ext		= ProfileExt;
			desc	= "Provides profile support";
		} */

		public XCProfile(ImgProfile profile)
			:
			base(0, 0)
		{
			_imageSize = new System.Drawing.Size(profile.Width, profile.Height);

			_codec = profile.ImgType;

			author	= "Profile";
			ext		= profile.Extension;
			desc	= profile.Description;
			expDesc	= profile.Description;

			if (profile.OpenSingle != Environment.NewLine)
				_singleFile = profile.OpenSingle;

			_fileOptions.Init(false, true, true, false);

			xConsole.AddLine("Profile created: " + desc);

			try
			{
				_palDefault = XCom.SharedSpace.Instance.GetPaletteTable()[profile.Palette];
			}
			catch
			{
				_palDefault = XCom.Palette.UFOBattle;
			}
		}


/*		public IXCImageFile Codec
		{
			get { return _codec; }
			set { _codec = value; }
		} */

/*		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				Palette pal)
		{
			return _codec.LoadFile(
								directory,
								file,
								width,
								height,
								pal);
		} */

/*		public override void SaveCollection(
				string directory,
				string file,
				XCom.XCImageCollection images)
		{
			_codec.SaveCollection(
							directory,
							file,
							images);
		} */
	}
}
