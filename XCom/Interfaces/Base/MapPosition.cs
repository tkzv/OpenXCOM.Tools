using System;


namespace XCom.Interfaces.Base
{
	internal sealed class MapPosition // TODO: merge with MapLocation.
	{
		private readonly int _rowMax;
		internal int MaxRows
		{
			get { return _rowMax; }
		}

		private readonly int _colMax;
		internal int MaxCols
		{
			get { return _colMax; }
		}

		private readonly int _levMax;
		internal int MaxLevs
		{
			get { return _levMax; }
		}


		/// <summary>
		/// cTor. Constructs a MapPosition object.
		/// </summary>
		/// <param name="rows">the maximum rows of a Map</param>
		/// <param name="cols">the maximum columns of a Map</param>
		/// <param name="levs">the maximum levels of a Map</param>
		internal MapPosition(int rows, int cols, int levs)
		{
			_rowMax = rows;
			_colMax = cols;
			_levMax = levs;
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
			return (_rowMax * _colMax * lev) + (_colMax * row) + col;
		}
	}
}
