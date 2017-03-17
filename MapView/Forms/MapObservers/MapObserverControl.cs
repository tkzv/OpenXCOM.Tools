using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	public class MapObserverControl
		:
		UserControl,
		IMap_Observer
	{
		private IMap_Base _baseMap;

		private RegistryInfo _regInfo;
		private readonly Dictionary<string, IMap_Observer> _moreObservers;


		public MapObserverControl()
		{
			_moreObservers = new Dictionary<string, IMap_Observer>();
			Settings = new Settings();
		}


		public virtual void LoadDefaultSettings()
		{}

		public Settings Settings
		{ get; set; }

		public Dictionary<string, IMap_Observer> MoreObservers
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
				value.Loading += (sender, e) => OnRISettingsLoad(e);
				value.Saving  += (sender, e) => OnRISettingsSave(e);
			}
		}

		protected virtual void OnRISettingsSave(RegistrySaveLoadEventArgs e)
		{}

		protected virtual void OnRISettingsLoad(RegistrySaveLoadEventArgs e)
		{}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IMap_Base Map
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

		public virtual void HeightChanged(IMap_Base sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}
	}
}
