using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace XCom.Resources.Map.RouteData
{
	public static class RouteCheckService
	{
		/// <summary>
		/// Checks for and if found gives user a choice to delete nodes that are
		/// outside of a Map's x/y/z bounds.
		/// </summary>
		/// <param name="child"></param>
		public static void CheckNodeBounds(MapFileChild child)
		{
			if (child != null)
			{
				var invalids = new List<RouteNode>();

				foreach (RouteNode node in child.Routes)
				{
					if (RouteNodeCollection.IsOutsideMap(
													node,
													child.MapSize.Cols,
													child.MapSize.Rows,
													child.MapSize.Levs))
					{
						invalids.Add(node);
					}
				}

				if (invalids.Count != 0)
				{
					string info  = String.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"There {0} " + invalids.Count + " route-node{1} outside"
												+ " the bounds of this Map. Do you want {2} deleted ?{3}",
											(invalids.Count == 1) ? "is" : "are",
											(invalids.Count == 1) ? ""   : "s",
											(invalids.Count == 1) ? "it" : "them",
											Environment.NewLine);

					foreach (var node in invalids)
						info += Environment.NewLine
							  + "id " + node.Index
							  + " : " + node.GetLocationString(child.MapSize.Levs);

					if (MessageBox.Show(
									info,
									"Invalid Nodes",
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0) == DialogResult.Yes)
					{
						child.RoutesChanged = true;

						foreach (var node in invalids)
							child.Routes.DeleteNode(node);
					}
				}
			}
		}

		/// <summary>
		/// Checks for and if found gives user a choice to delete nodes that are
		/// outside of a Map's x/y/z bounds.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>true if node(s) are deleted</returns>
		public static bool CheckNodeBoundsMenuitem(MapFileChild child)
		{
			if (child != null)
			{
				var invalids = new List<RouteNode>();

				foreach (RouteNode node in child.Routes)
				{
					if (RouteNodeCollection.IsOutsideMap(
													node,
													child.MapSize.Cols,
													child.MapSize.Rows,
													child.MapSize.Levs))
					{
						invalids.Add(node);
					}
				}

				string info, title;
				MessageBoxIcon icon;
				MessageBoxButtons btns;

				if (invalids.Count != 0)
				{
					icon  = MessageBoxIcon.Warning;
					btns  = MessageBoxButtons.YesNo;
					title = "Warning";
					info  = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"There {0} " + invalids.Count + " route-node{1} outside"
											+ " the bounds of this Map. Do you want {2} deleted ?{3}",
										(invalids.Count == 1) ? "is" : "are",
										(invalids.Count == 1) ? ""   : "s",
										(invalids.Count == 1) ? "it" : "them",
										Environment.NewLine);

					foreach (var node in invalids)
						info += Environment.NewLine
							  + "id " + node.Index
							  + " : " + node.GetLocationString(child.MapSize.Levs);
				}
				else
				{
					icon  = MessageBoxIcon.Information;
					btns  = MessageBoxButtons.OK;
					title = "Good stuff, Magister Ludi";
					info  = "There are no Out of Bounds nodes detected.";
				}

				if (MessageBox.Show(
							info,
							title,
							btns,
							icon,
							MessageBoxDefaultButton.Button1,
							0) == DialogResult.Yes)
				{
					child.RoutesChanged = true;

					foreach (var node in invalids)
						child.Routes.DeleteNode(node);

					return true;
				}
			}
			return false;
		}
	}
}
