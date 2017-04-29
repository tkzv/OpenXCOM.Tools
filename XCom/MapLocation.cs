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

		public int Height
		{ get; set; }


		public MapLocation(int row, int col, int height) // TODO: Switch row & col so things are x/y instead of y/x.
		{
			Row    = row;
			Col    = col;
			Height = height;
		}
	}
}
