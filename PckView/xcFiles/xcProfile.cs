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

			ext    = ProfileExt;
			desc   = "Provides profile support";
			author = "Ben Ratzlaff";
		} */

		public XCProfile(ImgProfile profile)
			:
			base(0, 0)
		{
			_imageSize = new System.Drawing.Size(profile.Width, profile.Height);

			_codec = profile.ImgType;

			_ext        = profile.Extension;
			_fileFilter = profile.Description;
			_desc       = profile.Description;
			_author     = "Profile";

			if (profile.OpenSingle != Environment.NewLine)
				_singleFile = profile.OpenSingle;

			_fileOptions.Init(false, true, true, false);

			xConsole.AddLine("Profile created: " + _desc);

			try
			{
				_palDefault = SharedSpace.Instance.GetPaletteTable()[profile.Palette];
			}
			catch
			{
				_palDefault = Palette.UFOBattle;
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
