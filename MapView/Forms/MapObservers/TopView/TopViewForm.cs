using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopViewForm
		:
			Form,
			IMapObserverProvider
	{
		internal TopViewForm()
		{
			TopViewControl = new TopView();
			InitializeComponent();
		}


		internal TopView Control
		{
			get { return TopViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return TopViewControl; }
		}
	}
}
