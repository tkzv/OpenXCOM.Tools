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


		public MapFileBase Load(MapDescChild desc)
		{
			if (desc != null && File.Exists(desc.FilePath))
			{
				var parts = new List<TilepartBase>();
				var info = ResourceInfo.ImageInfo;

				foreach (string dep in desc.Terrains)
				{
					var tileInfo = info[dep];
					if (tileInfo != null)
					{
						var MCD = tileInfo.GetRecordsByPalette(desc.Palette, _tileFactory);
						foreach (XCTilepart part in MCD)
							parts.Add(part);
					}
				}

				var RMP = new RouteNodeCollection(desc.Label, desc.RoutePath);
				var MAP = new MapFileChild(
										desc.Label,
										desc.MapPath,
										desc.OccultPath,
										parts,
										desc.Terrains,
										RMP);
				return MAP;
			}
			return null;
		}
	}
}
