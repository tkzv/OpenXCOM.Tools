using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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
										descriptor.Label    + MapFileChild.MapExt);
				LogFile.WriteLine(". pfeMap= " + pfeMap);
	
				if (File.Exists(pfeMap))
				{
					LogFile.WriteLine(". . Map file exists");
	
					var parts = new List<TilepartBase>();
	
					foreach (string terrain in descriptor.Terrains) // push together all allocated terrains
					{
						var MCD = descriptor.GetMcdRecords(terrain);
						foreach (Tilepart part in MCD)
							parts.Add(part);

//						var infoTerrain = ResourceInfo.TerrainInfo[terrain];
//						if (infoTerrain != null)
//						{
//							var MCD = infoTerrain.GetMcdRecords(descriptor.Pal);
//							foreach (Tilepart part in MCD)
//								parts.Add(part);
//						}
					}

					if (parts.Count == 0) // NOTE: safety. This should have been disallowed before things got here. Perhaps (cf, create new tileset)
						MessageBox.Show(
									"There are no terrains allocated or they contain no MCD records.",
									"Warning",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning,
									MessageBoxDefaultButton.Button1,
									0);

					var RMP = new RouteNodeCollection(descriptor.Label, descriptor.BasePath);
					var MAP = new MapFileChild(
											descriptor,
											parts,
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
