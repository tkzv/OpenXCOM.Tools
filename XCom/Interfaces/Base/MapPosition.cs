using System;


namespace XCom.Interfaces.Base
{
	public class MapPosition // TODO: merge with MapLocation.
	{
		private readonly int _rMax;
		private readonly int _cMax;
		private readonly int _hMax;


		public MapPosition(int rows, int cols, int height)
		{
			_rMax = rows;
			_cMax = cols;
			_hMax = height;
		}


		public int MaxRows
		{
			get { return _rMax; }
		}

		public int MaxCols
		{
			get { return _cMax; }
		}

		public int MaxHeight
		{
			get { return _hMax; }
		}

		public int GetLocationId(int r, int c, int h)
		{
			return (_rMax * _cMax * h) + (_cMax * r) + c;
		}
	}
}
