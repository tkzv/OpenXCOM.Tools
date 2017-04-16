/*
using System;

using DSShared;


namespace PckView
{
	internal sealed class OpenSaveFilter
		:
			IFilter<XCom.Interfaces.XCImageFile>
	{
		private XCom.Interfaces.XCImageFile.Filter _filter;


		public OpenSaveFilter()
		{
			_filter = XCom.Interfaces.XCImageFile.Filter.Open;
		}


		public void SetFilter(XCom.Interfaces.XCImageFile.Filter filter)
		{
			_filter = filter;
		}

		public bool FilterObj(XCom.Interfaces.XCImageFile obj)
		{
			//Console.WriteLine("Filter: {0} -> {1}", filterBy, obj.FileOptions[filterBy]);
			return obj.FileOptions[_filter];
		}
	}
}
*/
