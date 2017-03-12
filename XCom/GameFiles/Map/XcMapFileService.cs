using System.Collections.Generic;
using System.IO;

using XCom.GameFiles.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XcMapFileService
	{
		private readonly XcTileFactory _xcTileFactory;


		public XcMapFileService(XcTileFactory xcTileFactory)
		{
			_xcTileFactory = xcTileFactory;
		}


		public IMap_Base Load(XCMapDesc imd)
		{
			if (imd != null && File.Exists(imd.FilePath))
			{
				var tiles = new List<TileBase>();

				var images = GameInfo.ImageInfo;
				foreach (string dep in imd.Dependencies)
				{
					var image = images[dep];
					if (image != null)
					{
						var mcd = image.GetMcdFile(imd.Palette, _xcTileFactory);
						foreach (XCTile tile in mcd)
							tiles.Add(tile);
					}
				}

				var rmp = new RmpFile(imd.BaseName, imd.RmpPath);
				var map = new XCMapFile(
									imd.BaseName,
									imd.BasePath,
									imd.BlankPath,
									tiles,
									imd.Dependencies, rmp);
				return map;
			}
			return null;
		}
	}
}
