using System.Collections.Generic;
using System.IO;

using XCom.Resources.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	/// <summary>
	/// Loads a tileset. Called by XCMainWindow.LoadSelectedMap()
	/// </summary>
	public static class XCMapFileService
	{
		#region Methods
		public static MapFileBase LoadTileset(Descriptor descriptor)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("XCMapFileService.LoadTileset descriptor= " + descriptor);

			if (descriptor != null)
			{
				string pfeMap = Path.Combine(
										descriptor.BasePath + MapFileChild.MapsDir,
										descriptor.Label + MapFileChild.MapExt);
				LogFile.WriteLine(". pfeMap= " + pfeMap);
	
				if (File.Exists(pfeMap))
				{
					LogFile.WriteLine(". . Map file exists");
	
					var parts = new List<TilepartBase>();
	
					foreach (string terrain in descriptor.Terrains)
					{
						var infoTerrain = ResourceInfo.TerrainInfo[terrain];
						if (infoTerrain != null)
						{
							var MCD = infoTerrain.GetMcdRecords(descriptor.Pal);
							foreach (XCTilepart part in MCD)
								parts.Add(part);
						}
					}
	
					var RMP = new RouteNodeCollection(descriptor.Label, descriptor.BasePath);
					var MAP = new MapFileChild(
											descriptor.Label,
											descriptor.BasePath,
											parts,
											descriptor.Terrains,
											RMP);
					return MAP;
				}
			}

			LogFile.WriteLine(". descriptor NOT Valid or file does NOT exist");
			return null;
		}
		#endregion
	}
}
