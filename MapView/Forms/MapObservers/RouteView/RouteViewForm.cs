using System.Windows.Forms;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed partial class RouteViewForm
		:
			Form,
			IMapObserverProvider
	{
		internal RouteViewForm()
		{
			InitializeComponent();
		}


		internal RouteView Control
		{
			get { return RouteViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return RouteViewControl; }
		}
	}
}
