using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TileViewForm
		:
			Form,
			IMapObserverProvider
	{
		public TileViewForm()
		{
			InitializeComponent();
		}


		public TileView Control
		{
			get { return TileViewControl; }
		}

		public MapObserverControl0 MapObserver
		{
			get { return TileViewControl; }
		}
	}
}
