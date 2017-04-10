using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Parent for TopViewPanelBase, TopViewPanel, QuadrantPanel.
	/// </summary>
	internal class MapObserverControl1
		:
			DoubleBufferControl,
			IMapObserver
	{
		#region IMapObserver requirements

		private readonly Dictionary<string, IMapObserver> _observersDictionary = new Dictionary<string, IMapObserver>();
		[Browsable(false)]
		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return _observersDictionary; }
		}

		private IMapBase _baseMap;
		[Browsable(false), DefaultValue(null)]
		public virtual IMapBase Map
		{
			get { return _baseMap; }
			set
			{
				_baseMap = value;
				Refresh();
			}
		}

		public virtual void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

//		/// <summary>
//		/// This is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		public RegistryInfo RegistryInfo
//		{ get; set; }

//		/// <summary>
//		/// This is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		private RegistryInfo _regInfo;
//		/// <summary>
//		/// This stuff is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		[Browsable(false), DefaultValue(null)]
//		public RegistryInfo RegistryInfo
//		{
//			get { return _regInfo; }
//			set
//			{
//				_regInfo = value;
//				value.RegistryLoadEvent += (sender, e) => OnRegistrySettingsLoad(e);
//				value.RegistrySaveEvent += (sender, e) => OnRegistrySettingsSave(e);
//			}
//		}

		#endregion


//		protected virtual void OnRegistrySettingsLoad(RegistryEventArgs e) {}
//		protected virtual void OnRegistrySettingsSave(RegistryEventArgs e) {}
	}
}
