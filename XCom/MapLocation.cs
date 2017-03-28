using System;


namespace XCom
{
	/// <summary>
	/// A container for 3d Map coordinates.
	/// TODO: override Equals, ==, !=
	/// </summary>
	public struct MapLocation // TODO: merge with MapPosition.
	{
		private int _row;
		public int Row
		{
			get { return _row; }
			set { _row = value; }
		}

		private int _col;
		public int Col
		{
			get { return _col; }
			set { _col = value; }
		}

		private int _height;
		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}


		public MapLocation(int row, int col, int height) // TODO: Switch row & col so things are x/y instead of y/x.
		{
			_row    = row;
			_col    = col;
			_height = height;
		}
	}
}
