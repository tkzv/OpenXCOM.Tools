using System;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed class RoutePanelClickEventArgs
		:
			EventArgs
	{
		internal MapLocation ClickLocation
		{ get; set; }

		internal MapTileBase ClickTile
		{ get; set; }

		internal MouseEventArgs MouseEventArgs
		{ get; set; }
	}
}
