using System;


namespace XCom.Interfaces.Base
{
	internal sealed class MapPosition // TODO: merge with MapLocation.
	{
		/// <summary>
		/// Gets the total rows.
		/// </summary>
		private readonly int _rows;
		internal int MaxRows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the total columns.
		/// </summary>
		private readonly int _cols;
		internal int MaxCols
		{
			get { return _cols; }
		}

		/// <summary>
		/// Gets the total levels.
		/// </summary>
		private readonly int _levs;
		internal int MaxLevs
		{
			get { return _levs; }
		}


		/// <summary>
		/// cTor. Constructs a MapPosition object.
		/// </summary>
		/// <param name="rows">the maximum rows of a Map</param>
		/// <param name="cols">the maximum columns of a Map</param>
		/// <param name="levs">the maximum levels of a Map</param>
		internal MapPosition(int rows, int cols, int levs)
		{
			_rows = rows;
			_cols = cols;
			_levs = levs;
		}


		/// <summary>
		/// Gets the Id of a specified position.
		/// </summary>
		/// <param name="row">the current row</param>
		/// <param name="col">the current column</param>
		/// <param name="lev">the current level</param>
		/// <returns></returns>
		internal int GetPositionId(int row, int col, int lev)
		{
			return (_rows * _cols * lev) + (_cols * row) + col;
		}
	}
}
