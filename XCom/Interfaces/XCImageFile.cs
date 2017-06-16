using System;
using System.Drawing;


namespace XCom.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class XCImageFile
	{
		#region Fields (static)
		public const int SpriteWidth  = 32;
		public const int SpriteHeight = 40;
		#endregion


		#region Properties
		/// <summary>
		/// Image size that will be loaded.
		/// </summary>
		public Size ImageSize
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width">width (default 32)</param>
		/// <param name="height">height (default 40)</param>
		public XCImageFile(
				int width  = SpriteWidth,
				int height = SpriteHeight)
		{
			ImageSize = new Size(width, height);
		}
		#endregion


		#region Methods (static)
		public static Palette GetDefaultPalette()
		{
			return Palette.UfoBattle;
		}
		#endregion
	}
}
