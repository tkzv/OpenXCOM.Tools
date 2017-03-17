using System.Windows.Forms;

namespace MapView.Forms.MapObservers.TopViews
{
	public partial class TopViewForm
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

		public MapObserverControl MapObserver // TODO: Consolidate this w/ Control.
		{
			get { return TopViewControl; }
		}
	}
}
