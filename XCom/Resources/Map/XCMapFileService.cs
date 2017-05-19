using System.Collections.Generic;
using System.IO;

using XCom.Resources.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapFileService
	{
		#region Fields
		private readonly XCTileFactory _tileFactory;
		#endregion


		#region cTor
		public XCMapFileService(XCTileFactory tileFactory)
		{
			_tileFactory = tileFactory;
		}
		#endregion


		#region Methods
		public MapFileBase Load(Descriptor descriptor)
		{
			if (descriptor != null && File.Exists(descriptor.FilePath))
			{
				var parts = new List<TilepartBase>();
				var info = ResourceInfo.TerrainInfo;

				foreach (string terrain in descriptor.Terrains)
				{
					var tileInfo = info[terrain];
					if (tileInfo != null)
					{
						var MCD = tileInfo.GetMcdRecords(descriptor.Palette, _tileFactory);
						foreach (XCTilepart part in MCD)
							parts.Add(part);
					}
				}

				var RMP = new RouteNodeCollection(descriptor.Label, descriptor.RoutePath);
				var MAP = new MapFileChild(
										descriptor.Label,
										descriptor.MapPath,
										descriptor.OccultPath,
										parts,
										descriptor.Terrains,
										RMP);
				return MAP;
			}
			return null;
		}
		#endregion
	}
}
