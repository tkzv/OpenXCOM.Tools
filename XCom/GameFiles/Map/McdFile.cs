using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdFile
		:
		ReadOnlyCollection<XCTile>
	{
		internal McdFile(XCTile[] tiles)
			:
			base(tiles)
		{}
	}
}
