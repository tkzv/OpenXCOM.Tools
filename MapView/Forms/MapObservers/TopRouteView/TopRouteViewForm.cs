using System.Windows.Forms;

//using DSShared.Windows;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MapObservers.TileViews
{
	public partial class TopRouteViewForm
		:
		Form
	{
//		public RegistryInfo RegistryInfo;

		public TopView TopViewControl
		{
			get { return controlTopView; }
			set { controlTopView = value; }
		}

		public RouteView RouteViewControl
		{
			get { return controlRouteView; }
			set { controlRouteView = value; }
		}


		public TopRouteViewForm()
		{
			InitializeComponent();

//			RegistryInfo = new RegistryInfo(this, "TopRmpViewForm");
		}
	}
}
