namespace XCom.Interfaces.Base
{
	public class MapTileList
	{
		private readonly MapTileBase[] _mapData;
		private readonly MapPosition _mapPos;


		public MapTileList(int rows, int cols, int height)
		{
			_mapData = new MapTileBase[rows * cols * height];

			_mapPos = new MapPosition();
			_mapPos._rMax = rows;
			_mapPos._cMax = cols;
			_mapPos._hMax = height;
		}


		public MapTileBase this[int row, int col, int height]
		{
			get
			{
				if (   row    <= _mapPos._rMax
					&& col    <= _mapPos._cMax
					&& height <= _mapPos._hMax)
				{
					var i = GetIndex(row, col, height);
					if (i < _mapData.Length)
						return _mapData[i];
				}
				return null;
			}

			set
			{
				var i = GetIndex(row, col, height);
				_mapData[i] = value;
			}
		}

		private int GetIndex(int row, int col, int height)
		{
			_mapPos._r = row;
			_mapPos._c = col;
			_mapPos._h = height;

			return _mapPos.LocationId;
		}
	}
}
