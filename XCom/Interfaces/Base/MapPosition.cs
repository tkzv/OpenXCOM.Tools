using System;


namespace XCom.Interfaces.Base
{
	public class MapPosition // TODO: merge with MapLocation.
	{
		private readonly int _rMax;
		private readonly int _cMax;
		private readonly int _hMax;

		private int _r;
		private int _c;
		private int _h;


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

		public int Rows
		{
			set { _r = value; }
		}

		public int Cols
		{
			set { _c = value; }
		}

		public int Height
		{
			set { _h = value; }
		}

		public int GetLocationId()
		{
			return (_rMax * _cMax * _h) + (_cMax * _r) + _c;
		}
	}
}
