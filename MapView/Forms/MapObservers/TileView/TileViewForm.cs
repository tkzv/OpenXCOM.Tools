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
