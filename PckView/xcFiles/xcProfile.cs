using System;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class XCProfile
		:
			IXCImageFile
	{
		public const string ProfileExt = ".pvp";

//		private readonly IXCImageFile _codec;


		public XCProfile(ImageProfile profile)
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

			XConsole.AdZerg("Profile created: " + Description);

			try
			{
				DefaultPalette = SharedSpace.Instance.GetPaletteTable()[profile.Palette];
			}
			catch
			{
				DefaultPalette = Palette.UfoBattle;
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
				string dir,
				string file,
				int width,
				int height,
				Palette pal)
		{
			return _codec.LoadFile(
								dir,
								file,
								width,
								height,
								pal);
		} */

/*		public override void SaveCollection(
				string dir,
				string file,
				XCom.XCImageCollection images)
		{
			_codec.SaveCollection(
							dir,
							file,
							images);
		} */
	}
}
