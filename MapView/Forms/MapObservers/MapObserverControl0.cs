using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	internal class MapObserverControl0
		:
			UserControl,
			IMapObserver
	{
		private IMapBase _baseMap;

		private RegistryInfo _regInfo;
		private readonly Dictionary<string, IMapObserver> _moreObservers;


		public MapObserverControl0()
		{
			_moreObservers = new Dictionary<string, IMapObserver>();
			Settings = new Settings();
		}


		public virtual void LoadDefaultSettings()
		{}

		public Settings Settings
		{ get; set; }

		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return _moreObservers; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; }
			set
			{
				_regInfo = value;
				value.LoadingEvent += (sender, e) => OnRISettingsLoad(e);
				value.SavingEvent  += (sender, e) => OnRISettingsSave(e);
			}
		}

		protected virtual void OnRISettingsSave(RegistryEventArgs e)
		{}

		protected virtual void OnRISettingsLoad(RegistryEventArgs e)
		{}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IMapBase Map
		{
			get { return _baseMap; }
			set
			{
				_baseMap = value;
				Refresh();
			}
		}

		/// <summary>
		/// Scrolls the z-axis for TopView and RouteView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if		(e.Delta < 0) _baseMap.Up();
			else if (e.Delta > 0) _baseMap.Down();
		}

		public virtual void HeightChanged(IMapBase sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void SelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}
	}
}
