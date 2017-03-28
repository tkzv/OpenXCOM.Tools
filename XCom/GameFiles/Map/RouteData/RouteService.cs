using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom.GameFiles.Map.RouteData
{
	public static class RouteService
	{
		/// <summary>
		/// Checks for and if necessary deletes nodes that are outside of a
		/// Map's x/y/z bounds. See also RouteFile.CheckNodeBounds().
		/// </summary>
		/// <param name="baseMap"></param>
		public static void CheckNodeBounds(IMapBase baseMap)
		{
			var mapFile = baseMap as XCMapFile;
			if (mapFile != null)
			{
				var invalid = new List<RouteNode>();

				foreach (RouteNode node in mapFile.RouteFile)
					if (RouteNodeCollection.IsOutsideMap(
													node,
													baseMap.MapSize.Cols,
													baseMap.MapSize.Rows,
													baseMap.MapSize.Height))
					{
						invalid.Add(node);
					}

				if (invalid.Count != 0)
				{
					var result = MessageBox.Show(
											"There are route nodes outside the bounds of this Map. Do you want to remove them?",
											"Invalid Nodes",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button2,
											0);

					if (result == DialogResult.Yes)
					{
						foreach (var node in invalid)
							mapFile.RouteFile.Delete(node);

						mapFile.MapChanged = true;
					}
				}
			}
		}
	}
}
