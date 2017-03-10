using XCom.Interfaces.Base;


namespace XCom.Services
{
	public static class MapResizeService
	{
		public static MapTileList ResizeMap(
				int newR,
				int newC,
				int newH,
				MapSize mapSize,
				MapTileList oldMapTileList,
				bool wrtCeiling)
		{
			if (   newR != 0
				&& newC != 0
				&& newH != 0)
			{
				var newMap = new MapTileList(newR, newC, newH);

				FillNewMap(newR, newC, newH, newMap);

				for (int h = 0; h < newH && h < mapSize.Height; h++)
					for (int r = 0; r < newR && r < mapSize.Rows; r++)
						for (int c = 0; c < newC && c < mapSize.Cols; c++)
						{
							int hCopy = h;
							int hCurrent = h;
							if (wrtCeiling)
							{
								hCopy = mapSize.Height - h - 1;
								hCurrent = newH - h - 1;
							}
							newMap[r, c, hCurrent] = oldMapTileList[r, c, hCopy];
						}

				return newMap;
			}
			return null;
		}

		private static void FillNewMap(
				int newR,
				int newC,
				int newH,
				MapTileList newMap)
		{
			for (int h = 0; h < newH; h++)
				for (int r = 0; r < newR; r++)
					for (int c = 0; c < newC; c++)
						newMap[r, c, h] = XCMapTile.BlankTile;
		}
	}
}
