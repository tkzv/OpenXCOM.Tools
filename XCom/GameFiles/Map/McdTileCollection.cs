using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdTileCollection
		:
			ReadOnlyCollection<XCTile>
	{
		internal McdTileCollection(XCTile[] tiles)
			:
				base(tiles)
		{}
	}
}
