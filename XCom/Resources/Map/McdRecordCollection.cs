using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdRecordCollection
		:
			ReadOnlyCollection<Tilepart>
	{
		internal McdRecordCollection(Tilepart[] parts)
			:
				base(parts)
		{}
	}
}
