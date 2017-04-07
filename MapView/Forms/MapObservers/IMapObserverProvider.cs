using System;


namespace MapView.Forms.MapObservers
{
	/// <summary>
	/// Interface for TopViewForm, TileViewForm, RouteViewForm.
	/// </summary>
	internal interface IMapObserverProvider
	{
		MapObserverControl0 MapObserver
		{ get; }
	}
}
