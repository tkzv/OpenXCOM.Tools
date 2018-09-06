using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TopRouteViewForm
		:
			Form
	{
		internal TopRouteViewForm()
		{
			InitializeComponent();

			var regInfo = new RegistryInfo(RegistryInfo.TopRouteView, this); // subscribe to Load and Closing events.
			regInfo.RegisterProperties();
		}


		internal TopView ControlTop
		{
			get { return TopViewControl; }
		}

		internal RouteView ControlRoute
		{
			get { return RouteViewControl; }
		}
	}
}
