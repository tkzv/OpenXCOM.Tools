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

//		private readonly IXCImageFile _codec;


		public XCProfile(ImgProfile profile)
			:
			base(0, 0)
		{
			ImageSize = new System.Drawing.Size(profile.Width, profile.Height);

//			_codec = profile.ImgType;

			Brief         = profile.Description;
			Description   = profile.Description;
			FileExtension = profile.Extension;
			Author        = "Profile";

			if (profile.OpenSingle != Environment.NewLine)
				SingleFile = profile.OpenSingle;

			FileOptions.Init(false, true, true, false);

			xConsole.AddLine("Profile created: " + Description);

			try
			{
				DefaultPalette = SharedSpace.Instance.GetPaletteTable()[profile.Palette];
			}
			catch
			{
				DefaultPalette = Palette.UFOBattle;
			}
		}

/*		public XCProfile()
			:
			base(0, 0)
		{
			_fileOptions.Init(false, false, false, false);

			ext    = ProfileExt;
			desc   = "Provides profile support";
			author = "Ben Ratzlaff";
		} */


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
