using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom.Resources.Map.RouteData
{
	public static class RouteCheckService
	{
		/// <summary>
		/// Checks for and if necessary deletes nodes that are outside of a
		/// Map's x/y/z bounds. See also RouteNodeCollectionFile.CheckNodeBounds().
		/// </summary>
		/// <param name="mapBase"></param>
		public static void CheckNodeBounds(MapFileBase mapBase)
		{
			var mapFile = mapBase as MapFileChild;
			if (mapFile != null)
			{
				var invalid = new List<RouteNode>();

				foreach (RouteNode node in mapFile.RouteFile)
					if (RouteNodeCollection.IsOutsideMap(
													node,
													mapBase.MapSize.Cols,
													mapBase.MapSize.Rows,
													mapBase.MapSize.Levs))
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
						mapFile.MapChanged = true;

						foreach (var node in invalid)
							mapFile.RouteFile.DeleteNode(node);
					}
				}
			}
		}
	}
}
