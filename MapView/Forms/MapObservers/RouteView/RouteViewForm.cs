using System;
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


		public RouteView RouteViewControl
		{
			get { return controlRouteView; }
		}

		public MapObserverControl0 MapObserver
		{
			get { return controlRouteView; }
		}
	}
}
