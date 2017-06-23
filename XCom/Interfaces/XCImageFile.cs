using System;
//using System.Drawing;


namespace XCom.Interfaces
{
	/// <summary>
	/// Basically the width/height template for any sprite. Also defines a
	/// default palette staticly. This class is nearly irrelevant ... unless
	/// perhaps PckView becomes a bigobs editor also. Even then such data can
	/// and should be stored concisely - ie, elsewhere, in the spriteset-object
	/// and/or the sprite-objects.
	/// </summary>
//	public sealed class XCImageFile
	public static class XCImageFile
	{
		#region Fields (static)
		public const int SpriteWidth  = 32;
		public const int SpriteHeight = 40;
		#endregion


//		#region Properties
//		/// <summary>
//		/// Image size that will be loaded.
//		/// </summary>
//		public Size ImageSize
//		{ get; private set; }
//		#endregion


//		#region cTor
//		/// <summary>
//		/// Contains width/height info for a generic image-type.
//		/// </summary>
//		/// <param name="width">width (default 32)</param>
//		/// <param name="height">height (default 40)</param>
//		public XCImageFile(
//				int width  = SpriteWidth,
//				int height = SpriteHeight)
//		{
//			ImageSize = new Size(width, height);
//		}
//		#endregion


		#region Methods (static)
		public static Palette GetDefaultPalette()
		{
			return Palette.UfoBattle;
		}
		#endregion
	}
}
