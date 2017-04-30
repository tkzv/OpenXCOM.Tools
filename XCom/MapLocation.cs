using System;


namespace XCom
{
	/// <summary>
	/// A container for 3d Map coordinates.
	/// TODO: override Equals, ==, !=
	/// </summary>
	public class MapLocation // TODO: merge with MapPosition.
	{
		public int Row
		{ get; set; }

		public int Col
		{ get; set; }

		public int Lev
		{ get; set; }


		/// <summary>
		/// cTor. Constructs a MapLocation vector.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		public MapLocation(int row, int col, int lev) // TODO: Switch row & col so things are x/y instead of y/x.
		{
			Row = row;
			Col = col;
			Lev = lev;
		}
	}
}
