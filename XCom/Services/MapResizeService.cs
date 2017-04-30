using XCom.Interfaces.Base;


namespace XCom.Services
{
	internal static class MapResizeService
	{
		internal static MapTileList ResizeMap(
				int rPost,
				int cPost,
				int hPost,
				MapSize sizePre,
				MapTileList tileListPre,
				bool toCeiling)
		{
			if (   rPost > 0
				&& cPost > 0
				&& hPost > 0)
			{
				var tileListPost = new MapTileList(rPost, cPost, hPost);

				for (int h = 0; h != hPost; ++h)
					for (int r = 0; r != rPost; ++r)
						for (int c = 0; c != cPost; ++c)
							tileListPost[r, c, h] = XCMapTile.BlankTile;

				int levelPost;
				int levelPre;

				for (int
						h = 0;
						h != hPost && h < sizePre.Levs;
						++h)
					for (int
							r = 0;
							r != rPost && r < sizePre.Rows;
							++r)
						for (int
								c = 0;
								c != cPost && c < sizePre.Cols;
								++c)
						{
							if (toCeiling)
							{
								levelPost = hPost        - h - 1;
								levelPre  = sizePre.Levs - h - 1;
							}
							else
							{
								levelPost = h;
								levelPre  = h;
							}
							tileListPost[r, c, levelPost] = tileListPre[r, c, levelPre];
						}

				return tileListPost;
			}
			return null;
		}
	}
}
