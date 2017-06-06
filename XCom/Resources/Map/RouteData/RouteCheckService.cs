using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom.Resources.Map.RouteData
{
	public static class RouteCheckService
	{
		/// <summary>
		/// Checks for and if found gives user a choice to delete nodes that are
		/// outside of a Map's x/y/z bounds.
		/// </summary>
		/// <param name="mapFile"></param>
		public static void CheckNodeBounds(MapFileChild mapFile)
		{
			if (mapFile != null)
			{
				var invalid = new List<RouteNode>();

				foreach (RouteNode node in mapFile.Routes)
				{
					if (RouteNodeCollection.IsOutsideMap(
													node,
													mapFile.MapSize.Cols,
													mapFile.MapSize.Rows,
													mapFile.MapSize.Levs))
					{
						invalid.Add(node);
					}
				}

				if (invalid.Count != 0)
				{
					if (MessageBox.Show(
									"There are route nodes outside the bounds of"
										+ " this Map. Do you want them removed?",
									"Invalid Nodes",
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0) == DialogResult.Yes)
					{
						mapFile.RoutesChanged = true;

						foreach (var node in invalid)
							mapFile.Routes.DeleteNode(node);
					}
				}
			}
		}
	}
}
