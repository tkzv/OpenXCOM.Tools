using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Parent for TopView, TileView, RouteView.
	/// </summary>
	internal class MapObserverControl0
		:
			UserControl,
			IMapObserver
	{
		private readonly Dictionary<string, IMapObserver> _observersDictionary = new Dictionary<string, IMapObserver>();
		public Dictionary<string, IMapObserver> MoreObservers
		{
			get { return _observersDictionary; }
		}

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
				value.RegistryLoadEvent += (sender, e) => OnExtraRegistrySettingsLoad(e);
				value.RegistrySaveEvent += (sender, e) => OnExtraRegistrySettingsSave(e);
			}
		}

		internal Settings Settings
		{ get; set; }


		/// <summary>
		/// Invoked by TopView, TileView, RouteView.
		/// </summary>
		public MapObserverControl0()
		{
			Settings = new Settings();
		}


		public virtual void LoadDefaultSettings()
		{}

		/// <summary>
		/// Currently implemented only to load TopView's visible quadrants menu.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnExtraRegistrySettingsLoad(RegistryEventArgs e)
		{}

		/// <summary>
		/// Currently implemented only to save TopView's visible quadrants menu.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnExtraRegistrySettingsSave(RegistryEventArgs e)
		{}

		public virtual void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			Refresh();
		}

		public virtual void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e)
		{
			Refresh();
		}

		/// <summary>
		/// Scrolls the z-axis for TopView and RouteView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) _baseMap.Up();
			else if (e.Delta > 0) _baseMap.Down();
		}
	}
}
