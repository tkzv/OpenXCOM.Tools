using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdRecordCollection
		:
			ReadOnlyCollection<XCTile>
	{
		internal McdRecordCollection(XCTile[] tiles)
			:
				base(tiles)
		{}
	}
}
