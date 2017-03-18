using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TileViews
{
	public partial class TileViewForm
		:
		Form,
		IMapObserverFormProvider
	{
		public TileViewForm()
		{
			InitializeComponent();
		}


		public TileView TileViewControl
		{
			get { return controlTileView; }
		}

		public MapObserverControl0 MapObserver
		{
			get { return controlTileView; }
		}
	}
}
