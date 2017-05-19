using System.Collections.Generic;
using System.IO;

using XCom.Resources.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapFileService
	{
		#region Fields
		private readonly XCTileFactory _tileFactory = new XCTileFactory();
		#endregion


//		#region cTor
//		public XCMapFileService()
//		{}
//		#endregion


		#region Methods
		public MapFileBase LoadTileset(Descriptor descriptor)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("XCMapFileService.LoadTileset descriptor= " + descriptor);

			LogFile.WriteLine("file= " + descriptor.FilePath);

			if (descriptor != null && File.Exists(descriptor.FilePath))
			{
				LogFile.WriteLine(". descriptor VALID and file exists");

				var parts = new List<TilepartBase>();
				var info = ResourceInfo.TerrainInfo;

				foreach (string terrain in descriptor.Terrains)
				{
					var infoTerrain = info[terrain];
					if (infoTerrain != null)
					{
						var MCD = infoTerrain.GetMcdRecords(descriptor.Palette, _tileFactory);
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

			LogFile.WriteLine(". descriptor NOT Valid or file does NOT exist");
			return null;
		}
		#endregion
	}
}
