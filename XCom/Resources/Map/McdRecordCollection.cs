using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdRecordCollection
		:
			ReadOnlyCollection<Tilepart>
	{
		#region cTor
		/// <summary>
		/// Instantiates a read-only collection of MCD records.
		/// </summary>
		/// <param name="parts"></param>
		internal McdRecordCollection(Tilepart[] parts)
			:
				base(parts)
		{}
		#endregion
	}
}
