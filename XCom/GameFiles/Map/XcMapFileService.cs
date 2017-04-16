using System.Collections.Generic;
using System.IO;

using XCom.GameFiles.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapFileService
	{
		private readonly XCTileFactory _tileFactory;


		public XCMapFileService(XCTileFactory tileFactory)
		{
			_tileFactory = tileFactory;
		}


		public IMapBase Load(XCMapDesc desc)
		{
			if (desc != null && File.Exists(desc.FilePath))
			{
				var tiles = new List<TileBase>();
				var info = GameInfo.ImageInfo;

				foreach (string dep in desc.Dependencies)
				{
					var tileInfo = info[dep];
					if (tileInfo != null)
					{
						var MCD = tileInfo.GetRecordsByPalette(desc.Palette, _tileFactory);
						foreach (XCTile tile in MCD)
							tiles.Add(tile);
					}
				}

				var RMP = new RouteNodeCollection(desc.Label, desc.RoutePath);
				var MAP = new XCMapFile(
									desc.Label,
									desc.MapPath,
									desc.BlankPath,
									tiles,
									desc.Dependencies,
									RMP);
				return MAP;
			}
			return null;
		}
	}
}
