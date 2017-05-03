using XCom.Interfaces.Base;


namespace XCom.Services
{
	internal static class MapResizeService
	{
		internal static MapTileList ResizeMapDimensions(
				int rPost,
				int cPost,
				int lPost,
				MapSize sizePre,
				MapTileList tileListPre,
				bool toCeiling)
		{
			if (   rPost > 0
				&& cPost > 0
				&& lPost > 0)
			{
				var tileListPost = new MapTileList(rPost, cPost, lPost);

				for (int l = 0; l != lPost; ++l)
					for (int r = 0; r != rPost; ++r)
						for (int c = 0; c != cPost; ++c)
							tileListPost[r, c, l] = XCMapTile.BlankTile;

				int levelPre;
				int levelPost;

				for (int
						l = 0;
						l != lPost && l < sizePre.Levs;
						++l)
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
								levelPost = lPost        - l - 1;
								levelPre  = sizePre.Levs - l - 1;
							}
							else
							{
								levelPost = l;
								levelPre  = l;
							}
							tileListPost[r, c, levelPost] = tileListPre[r, c, levelPre];
						}

				return tileListPost;
			}
			return null;
		}
	}
}
