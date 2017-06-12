using System;


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
		private readonly Palette _palDefault = Palette.UfoBattle;
		/// <summary>
		/// The initial palette for the sprites.
		/// </summary>
		public Palette DefaultPalette
		{
			get { return _palDefault; }
		}

		/// <summary>
		/// Image size that will be loaded.
		/// </summary>
		public System.Drawing.Size ImageSize
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
			ImageSize = new System.Drawing.Size(width, height);
		}
		#endregion
	}
}
