using System;


namespace XCom.Interfaces.Base
{
	public class MapPosition
	{
		public int _rMax;
		public int _cMax;
		public int _hMax;
		public int _r;
		public int _c;
		public int _h;

		public int LocationId
		{
			get { return (_rMax * _cMax * _h) + (_cMax * _r) + _c; }
		}
	}
}
