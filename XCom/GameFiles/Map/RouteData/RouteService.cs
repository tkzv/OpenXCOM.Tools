using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom.GameFiles.Map.RouteData
{
	public static class RouteService
	{
		public static void ReviewRouteEntries(IMap_Base baseMap)
		{
			var map = baseMap as XCMapFile;
			if (map != null)
			{
				var incorrectEntries = new List<RouteNode>();

				foreach (RouteNode node in map.RouteFile)
					if (RouteFile.IsOutsideMap(
										node,
										baseMap.MapSize.Cols,
										baseMap.MapSize.Rows,
										baseMap.MapSize.Height))
					{
						incorrectEntries.Add(node);
					}

				if (incorrectEntries.Count != 0)
				{
					var result = MessageBox.Show(
											"There are route entries outside the bounds of this Map. Do you want to remove them?",
											"Invalid Nodes",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button2,
											0);

					if (result == DialogResult.Yes)
					{
						foreach (var rmpEntry in incorrectEntries)
							map.RouteFile.RemoveEntry(rmpEntry);

						map.MapChanged = true;
					}
				}
			}
		}
	}
}
