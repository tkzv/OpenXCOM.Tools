using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	public class Map_Observer_Control
		:
		DoubleBufferControl,
		IMap_Observer
	{
		protected IMap_Base map;

		private RegistryInfo _regInfo;

		private readonly Dictionary<string, IMap_Observer> moreObservers;


		public Map_Observer_Control()
		{
			moreObservers = new Dictionary<string, IMap_Observer>();
		}


		#region IMap_Observer Members

		[Browsable(false), DefaultValue(null)]
		public virtual IMap_Base Map
		{
			get { return map; }
			set { map = value; Refresh(); }
		}

		public virtual void HeightChanged(IMap_Base sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}

		[Browsable(false)]
		[DefaultValue(null)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; }
			set
			{
				_regInfo = value;

				value.Loading += delegate(object sender, RegistrySaveLoadEventArgs e)
				{
					OnRISettingsLoad(e);
				};

				value.Saving += delegate(object sender, RegistrySaveLoadEventArgs e)
				{
					OnRISettingsSave(e);
				};
			}
		}

		protected virtual void OnRISettingsSave(RegistrySaveLoadEventArgs e)
		{}

		protected virtual void OnRISettingsLoad(RegistrySaveLoadEventArgs e)
		{}

		[Browsable(false)]
		public Dictionary<string, IMap_Observer> MoreObservers
		{
			get { return moreObservers; }
		}

		#endregion
	}
}
