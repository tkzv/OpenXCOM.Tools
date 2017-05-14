using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Objects of this class are drawn to the screen with the MainViewPanel.
	/// </summary>
	public abstract class MapTileBase
	{
		/// <summary>
		/// An array of TilepartBase[] in the correct draw order. This array
		/// should be iterated over when drawing to the screen.</summary>
		public abstract TilepartBase[] UsedTiles
		{ get; }

		/// <summary>
		/// Flag to optimize screen drawing.
		/// </summary>
		public bool DrawAbove
		{ get; set; }
	}
}
