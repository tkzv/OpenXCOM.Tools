namespace XCom.Interfaces.Base
{
	internal sealed class MapTileList
	{
		private readonly MapTileBase[] _tileArray;
		private readonly MapPosition   _pos;

		public MapTileBase this[int row, int col, int lev]
		{
			get
			{
				if (   row <= _pos.MaxRows
					&& col <= _pos.MaxCols
					&& lev <= _pos.MaxLevs)
				{
					var id = _pos.GetPositionId(row, col, lev);
					if (id < _tileArray.Length)
						return _tileArray[id];
				}
				return null;
			}
			set
			{
				_tileArray[_pos.GetPositionId(row, col, lev)] = value;
			}
		}


		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		internal MapTileList(int rows, int cols, int levs)
		{
			_tileArray = new MapTileBase[rows * cols * levs];
			_pos       = new MapPosition(rows,  cols,  levs);
		}
	}
}
