using System;

using DSShared;


namespace PckView
{
	internal sealed class OpenSaveFilter
		:
			IFilter<XCom.Interfaces.IXCImageFile>
	{
		private XCom.Interfaces.IXCImageFile.Filter _filter;


		public OpenSaveFilter()
		{
			_filter = XCom.Interfaces.IXCImageFile.Filter.Open;
		}


		public void SetFilter(XCom.Interfaces.IXCImageFile.Filter filter)
		{
			_filter = filter;
		}

		public bool FilterObj(XCom.Interfaces.IXCImageFile obj)
		{
			//Console.WriteLine("Filter: {0} -> {1}", filterBy, obj.FileOptions[filterBy]);
			return obj.FileOptions[_filter];
		}
	}
}
