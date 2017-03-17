using System;
using System.Windows.Forms;


namespace MapView.Forms.MapObservers.RouteViews
{
	public partial class RouteViewForm
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

		public MapObserverControl MapObserver
		{
			get { return controlRouteView; }
		}
	}
}
