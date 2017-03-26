namespace XCom.Interfaces.Base
{
	public class MapTileList
	{
		private readonly MapTileBase[] _mapData;
		private readonly MapPosition _mapPos;


		public MapTileList(int rows, int cols, int height)
		{
			_mapData = new MapTileBase[rows * cols * height];
			_mapPos  = new MapPosition(rows,  cols,  height);
		}


		public MapTileBase this[int row, int col, int height]
		{
			get
			{
				if (   row    <= _mapPos.MaxRows
					&& col    <= _mapPos.MaxCols
					&& height <= _mapPos.MaxHeight)
				{
					var id = _mapPos.GetLocationId(row, col, height);
					if (id < _mapData.Length)
						return _mapData[id];
				}
				return null;
			}

			set
			{
				var id = _mapPos.GetLocationId(row, col, height);
				_mapData[id] = value;
			}
		}
	}
}
