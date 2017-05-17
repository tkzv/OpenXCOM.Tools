using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdRecordCollection
		:
			ReadOnlyCollection<XCTilepart>
	{
		internal McdRecordCollection(XCTilepart[] parts)
			:
				base(parts)
		{}
	}
}
