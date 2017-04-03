using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	internal class MapObserverControl1
		:
			DoubleBufferControl,
			IMapObserver
	{
		private IMapBase _baseMap;

		private RegistryInfo _regInfo;

		private readonly Dictionary<string, IMapObserver> moreObservers;


		public MapObserverControl1()
		{
			moreObservers = new Dictionary<string, IMapObserver>();
		}


		#region IMapObserver Members

		[Browsable(false), DefaultValue(null)] // DefaultValue *cough
		public virtual IMapBase Map
		{
			get { return _baseMap; }
			set
			{
				_baseMap = value;
				Refresh();
			}
		}

		public virtual void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}

		[Browsable(false), DefaultValue(null)]	// "A DefaultValueAttribute will not cause a member to be automatically initialized
		public RegistryInfo RegistryInfo		// with the attribute's value. You must set the initial value in your code."
		{
			get { return _regInfo; }
			set
			{
				_regInfo = value;
				value.RegistryLoadEvent += (sender, e) => OnRegistrySettingsLoad(e);
				value.RegistrySaveEvent  += (sender, e) => OnRegistrySettingsSave(e);
			}
		}

		protected virtual void OnRegistrySettingsSave(RegistryEventArgs e)
		{}

		protected virtual void OnRegistrySettingsLoad(RegistryEventArgs e)
		{}

		[Browsable(false)]
		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return moreObservers; }
		}

		#endregion
	}
}
