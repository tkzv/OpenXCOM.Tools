using System;


namespace XCom
{
	/// <summary>
	/// A container for map positions.
	/// </summary>
	public struct MapLocation
	{
		public int Row, Col, Height;

		public MapLocation(int row, int col, int height) // TODO: Switch row & col so things are x/y instead of y/x.
		{
			this.Row = row;
			this.Col = col;
			this.Height = height;
		}
	}
}
