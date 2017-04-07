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

			var regInfo = new RegistryInfo(this, "TopRouteView"); // TODO: what's this doing if anything.
		}


		internal TopView ControlTop
		{
			get { return controlTopView; }
		}

		internal RouteView ControlRoute
		{
			get { return controlRouteView; }
		}
	}
}
