using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	public class MapObserverControl1
		:
		DoubleBufferControl,
		IMap_Observer
	{
		private IMap_Base _baseMap;

		private RegistryInfo _regInfo;

		private readonly Dictionary<string, IMap_Observer> moreObservers;


		public MapObserverControl1()
		{
			moreObservers = new Dictionary<string, IMap_Observer>();
		}


		#region IMap_Observer Members

		[Browsable(false), DefaultValue(null)] // DefaultValue *cough
		public virtual IMap_Base Map
		{
			get { return _baseMap; }
			set
			{
				_baseMap = value;
				Refresh();
			}
		}

		public virtual void HeightChanged(IMap_Base sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
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
