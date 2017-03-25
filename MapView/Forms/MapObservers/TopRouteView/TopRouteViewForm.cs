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
		public RegistryInfo RegistryInfo; // FxCop CA1823.


		public TopRouteViewForm()
		{
			InitializeComponent();

			RegistryInfo = new RegistryInfo(this, "TopRouteViewForm"); // TODO: what's this doing if anything.
		}


		public TopView TopViewControl
		{
			get { return controlTopView; }
		}

		public RouteView RouteViewControl
		{
			get { return controlRouteView; }
		}
	}
}
