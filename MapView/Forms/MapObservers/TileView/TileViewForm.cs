using System;
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
			TileViewControl = new TileView();
			InitializeComponent();

			Activated += OnActivated;
		}

		/// <summary>
		/// Fires when the form is activated.
		/// </summary>
		private void OnActivated(object sender, EventArgs e)
		{
			TileViewControl.GetSelectedPanel().Focus();
		}

		/// <summary>
		/// Gets TileView as a child of MapObserverControl0.
		/// </summary>
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
