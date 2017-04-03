using System.Windows.Forms;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed partial class RouteViewForm
		:
			Form,
			IMapObserverFormProvider
	{
		public RouteViewForm()
		{
			InitializeComponent();
		}


		public RouteView Control
		{
			get { return RouteViewControl; }
		}

		public MapObserverControl0 MapObserver
		{
			get { return RouteViewControl; }
		}
	}
}
