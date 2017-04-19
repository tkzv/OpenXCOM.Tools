using System;

using XCom.Interfaces;


namespace PckView.Panels // NOTE: this isn't a panel.
{
	internal sealed class SpriteSelected
	{
		internal int X
		{ get; set; }

		internal int Y
		{ get; set; }

		internal int Id
		{ get; set; }

		internal XCImage Image
		{ get; set; }
	}
}
