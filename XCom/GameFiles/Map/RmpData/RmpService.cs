using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom.GameFiles.Map.RmpData
{
	public static class RmpService
	{
		public static void ReviewRouteEntries(IMap_Base baseMap)
		{
			var map = baseMap as XCMapFile;
			if (map != null)
			{
				var incorrectEntries = new List<RmpEntry>();

				foreach (RmpEntry entry in map.Rmp)
					if (RmpFile.IsOutsideMap(
										entry,
										baseMap.MapSize.Cols,
										baseMap.MapSize.Rows,
										baseMap.MapSize.Height))
					{
						incorrectEntries.Add(entry);
					}

				if (incorrectEntries.Count != 0)
				{
					var result = MessageBox.Show(
											"There are route entries outside the bounds of this Map. Do you want to remove them?",
											"Incorrect Routes",
											MessageBoxButtons.YesNo);

					if (result == DialogResult.Yes)
					{
						foreach (var rmpEntry in incorrectEntries)
							map.Rmp.RemoveEntry(rmpEntry);

						map.MapChanged = true;
					}
				}
			}
		}
	}
}
