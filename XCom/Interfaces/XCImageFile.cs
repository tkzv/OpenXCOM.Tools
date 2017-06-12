using System;
using System.Collections.Generic;


namespace XCom.Interfaces
{
	/// <summary>
	/// Class that contains all information needed to read/save image files and
	/// collections. This class should not be instantiated directly. Objects
	/// from derived classes will be created and tracked on startup.
	/// </summary>
	public sealed class XCImageFile
	{
		private readonly Palette _palDefault = Palette.UfoBattle;
		/// <summary>
		/// Defines the initial palette for the sprites.
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

		/// <summary>
		/// It is not recommended to instantiate objects of this type directly.
		/// See PckView.XCProfile for a generic implementation that does not
		/// throw runtime exceptions
		/// </summary>
		/// <param name="width">default width</param>
		/// <param name="height">default height</param>
		public XCImageFile(int width, int height)
		{
			ImageSize = new System.Drawing.Size(width, height);
		}
	}
}
