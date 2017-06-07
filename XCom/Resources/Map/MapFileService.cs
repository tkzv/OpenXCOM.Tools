using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom
{
	/// <summary>
	/// Loads a tileset. Called by XCMainWindow.LoadSelectedMap()
	/// </summary>
	public static class MapFileService
	{
		#region Methods
		public static MapFileBase LoadTileset(Descriptor descriptor)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("MapFileService.LoadTileset descriptor= " + descriptor);

			if (descriptor != null)
			{
				string dirMap = Path.Combine(descriptor.BasePath, MapFileChild.MapsDir);
				string pfeMap = Path.Combine(
										dirMap,
										descriptor.Label + MapFileChild.MapExt);
				//LogFile.WriteLine(". pfeMap= " + pfeMap);

				if (File.Exists(pfeMap))
				{
					//LogFile.WriteLine(". . Map file exists");

					var parts = new List<TilepartBase>();

					foreach (string terrain in descriptor.Terrains) // push together the tileparts of all allocated terrains
					{
						var MCD = descriptor.GetTerrainRecords(terrain);
						foreach (Tilepart part in MCD)
							parts.Add(part);
					}

					if (parts.Count != 0)
					{
						var RMP = new RouteNodeCollection(descriptor.Label, descriptor.BasePath);
						var MAP = new MapFileChild(
												descriptor,
												parts,
												RMP);
						return MAP;
					}

					//LogFile.WriteLine(". . . descriptor has no terrains");
					MessageBox.Show(
								"There are no terrains allocated or they do not contain MCD records.",
								"Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
				{
					//LogFile.WriteLine(". . Mapfile does NOT exist");
					MessageBox.Show(
								"The Mapfile does not exist.",
								"Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button1,
								0);
				}
			}
			else
			{
				//LogFile.WriteLine(". descriptor NOT Valid");
				MessageBox.Show(
							"The tileset is not valid.",
							"Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0);
			}

			return null;
		}
		#endregion
	}
}
