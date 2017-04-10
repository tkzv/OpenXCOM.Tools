using System.Windows.Forms;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TileViewForm
		:
			Form,
			IMapObserverProvider
	{
		internal TileViewForm()
		{
			InitializeComponent();
		}


		internal TileView Control
		{
			get { return TileViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return TileViewControl; }
		}
	}
}
