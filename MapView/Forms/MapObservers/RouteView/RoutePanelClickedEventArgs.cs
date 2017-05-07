using System;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed class RoutePanelClickedEventArgs
		:
			EventArgs
	{
		internal MapLocation ClickedLocation
		{ get; set; }

		internal MapTileBase ClickedTile
		{ get; set; }

		internal MouseEventArgs MouseEventArgs
		{ get; set; }
	}
}
