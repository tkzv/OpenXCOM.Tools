using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopViewForm
		:
			Form,
			IMapObserverFormProvider
	{
		public TopViewForm()
		{
			InitializeComponent();
		}


		public TopView Control // TODO: Consolidate this w/ MapObserver.
		{
			get { return TopViewControl; }
		}

		public MapObserverControl0 MapObserver // TODO: Consolidate this w/ Control.
		{
			get { return TopViewControl; }
		}
	}
}
