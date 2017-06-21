using System;

using XCom.Interfaces;


namespace PckView
{
	internal sealed class SelectedSprite
	{
		internal int TerrainId
		{ get; set; }

		internal XCImage Sprite
		{ get; set; }
	}
}
