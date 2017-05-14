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


		public XCMapBase Load(XCMapDesc desc)
		{
			if (desc != null && File.Exists(desc.FilePath))
			{
				var parts = new List<TilepartBase>();
				var info = GameInfo.ImageInfo;

				foreach (string dep in desc.Dependencies)
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
				var MAP = new XCMapFile(
									desc.Label,
									desc.MapPath,
									desc.BlankPath,
									parts,
									desc.Dependencies,
									RMP);
				return MAP;
			}
			return null;
		}
	}
}
