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

		private RegistryInfo _regInfo;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; }
			set
			{
				_regInfo = value;
				value.RegistryLoadEvent += (sender, e) => OnRegistrySettingsLoad(e);
				value.RegistrySaveEvent += (sender, e) => OnRegistrySettingsSave(e);
			}
		}

		private readonly Dictionary<string, IMapObserver> _moreObservers;
		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return _moreObservers; }
		}

		public Settings Settings
		{ get; set; }


		public MapObserverControl0()
		{
			_moreObservers = new Dictionary<string, IMapObserver>();
			Settings = new Settings();
		}


		public virtual void LoadDefaultSettings()
		{}

		protected virtual void OnRegistrySettingsSave(RegistryEventArgs e)
		{}

		protected virtual void OnRegistrySettingsLoad(RegistryEventArgs e)
		{}

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

		public virtual void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}
	}
}
