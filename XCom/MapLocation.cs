using System;


namespace XCom
{
	/// <summary>
	/// A container for map positions.
	/// </summary>
	public struct MapLocation // TODO: merge with MapPosition.
	{
		public int Row, Col, Height;

		public MapLocation(int row, int col, int height) // TODO: Switch row & col so things are x/y instead of y/x.
		{
			Row    = row;
			Col    = col;
			Height = height;
		}
	}
}
