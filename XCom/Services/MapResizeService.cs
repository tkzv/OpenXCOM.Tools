using XCom.Interfaces.Base;


namespace XCom.Services
{
	internal static class MapResizeService
	{
		internal static MapTileList ResizeMapDimensions(
				int rowPost,
				int colPost,
				int levPost,
				MapSize sizePre,
				MapTileList tileListPre,
				bool toCeiling)
		{
			if (   rowPost > 0
				&& colPost > 0
				&& levPost > 0)
			{
				var tileListPost = new MapTileList(rowPost, colPost, levPost);

				for (int lev = 0; lev != levPost; ++lev)
				for (int row = 0; row != rowPost; ++row)
				for (int col = 0; col != colPost; ++col)
					tileListPost[row, col, lev] = XCMapTile.BlankTile;

				int levelPre;
				int levelPost;

				for (int
						lev = 0;
						lev != levPost && lev < sizePre.Levs;
						++lev)
					for (int
							row = 0;
							row != rowPost && row < sizePre.Rows;
							++row)
						for (int
								col = 0;
								col != colPost && col < sizePre.Cols;
								++col)
						{
							if (toCeiling)
							{
								levelPost = levPost      - lev - 1;
								levelPre  = sizePre.Levs - lev - 1;
							}
							else
							{
								levelPost = lev;
								levelPre  = lev;
							}
							tileListPost[row, col, levelPost] = tileListPre[row, col, levelPre];
						}

				return tileListPost;
			}
			return null;
		}
	}
}
