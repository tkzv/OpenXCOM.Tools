using System.Collections.Generic;
using System.IO;

using XCom.Resources.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public static class XCMapFileService
	{
		#region Methods
		public static MapFileBase LoadTileset(Descriptor descriptor)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("XCMapFileService.LoadTileset descriptor= " + descriptor);

			LogFile.WriteLine("file= " + descriptor.FullPath);

			if (descriptor != null && File.Exists(descriptor.FullPath))
			{
				LogFile.WriteLine(". descriptor VALID and file exists");

				var parts = new List<TilepartBase>();
				var info = ResourceInfo.TerrainInfo;

				foreach (string terrain in descriptor.Terrains)
				{
					var infoTerrain = info[terrain];
					if (infoTerrain != null)
					{
						var MCD = infoTerrain.GetMcdRecords(descriptor.Palette);
						foreach (XCTilepart part in MCD)
							parts.Add(part);
					}
				}

				var RMP = new RouteNodeCollection(descriptor.Label, descriptor.RoutesPath);
				var MAP = new MapFileChild(
										descriptor.Label,
										descriptor.MapsPath,
										descriptor.OccultsPath,
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
