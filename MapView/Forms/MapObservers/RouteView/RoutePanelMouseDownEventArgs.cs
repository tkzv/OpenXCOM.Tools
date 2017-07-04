using System;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed class RoutePanelMouseDownEventArgs
		:
			EventArgs
	{
		internal MapLocation Location
		{ get; set; }

		internal MapTileBase Tile
		{ get; set; }

		internal MouseButtons MouseButton
		{ get; set; }
	}
}
