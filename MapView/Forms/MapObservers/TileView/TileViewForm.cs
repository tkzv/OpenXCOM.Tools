using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TileViews
{
	public partial class TileViewForm
		:
		Form,
		IMapObserverFormProvider
	{
		public TileView TileViewControl
		{
			get { return controlTileView; }
		}

		public MapObserverControl MapObserver
		{
			get { return controlTileView; }
		}


		public TileViewForm()
		{
			InitializeComponent();
		}
	}
}
