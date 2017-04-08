using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;

using YamlDotNet.RepresentationModel;	// read values (deserialization)
using YamlDotNet.Serialization;			// write values


namespace DSShared.Windows
{
	/// <summary>
	/// Delegate for use in the saving and loading events raised in the
	/// RegistryInfo class.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
//	public delegate void EventHandler<RegistryEventArgs>(object sender, RegistryEventArgs e);
	public delegate void RegistryEventHandler(object sender, RegistryEventArgs e);


	/// <summary>
	/// A class to help facilitate the saving and loading of values into the
	/// registry in a central location.
	/// </summary>
	public class RegistryInfo
	{
		public const string SoftwareRegistry = "Software";
		public const string MapViewRegistry  = "MapView";

		private readonly object _obj;
		private readonly string _regkey;

		private readonly Dictionary<string, PropertyInfo> _infoDictionary = new Dictionary<string, PropertyInfo>();

//		private bool _saveOnClose = true;

		/// <summary>
		/// Event fired when retrieving values from the registry. This happens
		/// after the values are read and set in the object.
		/// </summary>
//		public event EventHandler LoadingEvent;
		public event RegistryEventHandler RegistryLoadEvent;

		/// <summary>
		/// Event fired when saving values to the registry. This happens after
		/// the values are saved.
		/// </summary>
//		public event EventHandler SavingEvent;
		public event RegistryEventHandler RegistrySaveEvent;


		/// <summary>
		/// Main cTor. Uses the specified string as a registry key.
		/// </summary>
		/// <param name="obj">the object to save/load values into the registry</param>
		/// <param name="regkey">the name of the registry key to save/load</param>
		public RegistryInfo(object obj, string regkey)
		{
			_obj = obj;
			_regkey = regkey;

			var f = obj as Form;
			if (f != null)
			{
				f.StartPosition = FormStartPosition.Manual;
				f.Load    += OnLoad;
				f.Closing += OnClose;
				AddProperty("Width", "Height", "Left", "Top");
			}
		}
/*		/// <summary>
		/// Auxiliary cTor. Uses the ToString() return of the specified object
		/// as a registry key.
		/// </summary>
		/// <param name="obj"></param>
		public RegistryInfo(object obj)
			:
				this(obj, obj.GetType().ToString())
		{} */


		/// <summary>
		/// Adds properties to be saved/loaded.
		/// </summary>
		/// <param name="keys">the names of the properties to be saved/loaded</param>
		public void AddProperty(params string[] keys)
		{
			var type = _obj.GetType();

			PropertyInfo info;
			foreach (string key in keys)
			{
				if ((info = type.GetProperty(key)) != null)
					_infoDictionary[info.Name] = info;
			}
		}

		/// <summary>
		/// Loads values for the subsidiary viewers from "MapViewers.yml".
		/// - TopView      (size and position, and perhaps the Quadrant visibilities)
		/// - RouteView    (size and position)
		/// - TopRouteView (size and position)
		/// - TileView     (size and position)
		/// - Console      (size and position)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
		{
//			string file = Path.Combine(SharedSpace.Instance.GetString(SharedSpace.SettingsDirectory), PathInfo.YamlViewers);

			string path = AppDomain.CurrentDomain.BaseDirectory;	// NOTE: this is probably where 'DSShared.dll' resides.
			const string file = @"settings\MapViewers.yml";			// not necessarily where 'MapView.exe' is ....
			string pfe = Path.Combine(path, file);

			using (var sr = new StreamReader(File.OpenRead(pfe)))
			{
				var str = new YamlStream();
				str.Load(sr);

				var nodeRoot = (YamlMappingNode)str.Documents[0].RootNode; // TODO: Error handling. ->
				foreach (var node in nodeRoot.Children)
				{
					string viewer = ((YamlScalarNode)node.Key).Value;
					if (String.Equals(viewer, _regkey, StringComparison.OrdinalIgnoreCase))
					{
						var keyvals = (YamlMappingNode)nodeRoot.Children[new YamlScalarNode(viewer)];
						ImportValues(keyvals);
					}
				}
			}
		}

		/// <summary>
		/// Helper for OnLoad().
		/// </summary>
		/// <param name="keyvals">yaml-mapped keyval pairs</param>
		private void ImportValues(YamlMappingNode keyvals)
		{
			string key;
			object val;

			foreach (var keyval in keyvals)
			{
				key = keyval.Key.ToString();

				if (key.StartsWith("vis", StringComparison.Ordinal))	// NOTE: vis# are for TopView's visible-quadrant-type MenuItem toggles
				{														// ... these are currently handled by TopView.OnExtraRegistrySettingsLoad()
					if (RegistryLoadEvent != null)
					{
						val = Boolean.Parse(keyval.Value.ToString());
						RegistryLoadEvent(this, new RegistryEventArgs(key, Convert.ToBoolean(val)));
					}
				}
				else if (_infoDictionary.ContainsKey(key)) // it'll be there, i trust. yeah right
				{
					val = Int32.Parse(keyval.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
					_infoDictionary[key].SetValue(
												_obj,
												val,
												null);
				}
			}
		}

/*		private void OnLoad(object sender, EventArgs e)
		{
			using (RegistryKey regkeySoftware = Registry.CurrentUser.CreateSubKey(SoftwareRegistry))
			using (RegistryKey regkeyMapView  = regkeySoftware.CreateSubKey(MapViewRegistry))
			using (RegistryKey regkeyViewer   = regkeyMapView.CreateSubKey(_regkey))
			{
				foreach (string key in _infoDictionary.Keys)
					_infoDictionary[key].SetValue(
										_obj,
										regkeyViewer.GetValue(key, _infoDictionary[key].GetValue(_obj, null)),
										null);

				if (RegistryLoadEvent != null)
					RegistryLoadEvent(this, new RegistryEventArgs(regkeyViewer));

				regkeyViewer.Close();
				regkeyMapView.Close();
				regkeySoftware.Close();
			}
		} */

		/// <summary>
		/// Saves values to the registry on the Closing events of various forms.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClose(object sender, EventArgs e)
		{
			var f = _obj as Form;
			if (f != null)
			{
				f.WindowState = FormWindowState.Normal;

//				if (_saveOnClose)
//				{
				using (RegistryKey regkeySoftware = Registry.CurrentUser.CreateSubKey(SoftwareRegistry))
				using (RegistryKey regkeyMapView  = regkeySoftware.CreateSubKey(MapViewRegistry))
				using (RegistryKey regkeyViewer   = regkeyMapView.CreateSubKey(_regkey))
				{
					foreach (string key in _infoDictionary.Keys)
						regkeyViewer.SetValue(
											key,
											_infoDictionary[key].GetValue(_obj, null));

//					if (RegistrySaveEvent != null)
//						RegistrySaveEvent(this, new RegistryEventArgs(regkeyViewer));

					regkeyViewer.Close();
					regkeyMapView.Close();
					regkeySoftware.Close();
				}
//				}
			}
		}

		// TODO: Vet the PathsEditor button that clears registry keys.


/*		/// <summary>
		/// Deletes the key located at HKEY_CURRENT_USER\Software\'_regKey'\
		/// '_regKey' is a public static parameter of this class.
		/// </summary>
		/// <param name="saveOnClose">if false, a future call to Save() will
		/// have no effect</param>
		public void ClearKey(bool saveOnClose)
		{
			RegistryKey keySoftware = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey keyDSShared = keySoftware.CreateSubKey(_regKey);
			keyDSShared.DeleteSubKey(_name);
			_saveOnClose = saveOnClose;
		} */
/*		/// <summary>
		/// Calls ClearKey(false).
		/// </summary>
		public void ClearKey()
		{
			ClearKey(false);
		} */


/*		/// <summary>
		/// Opens the registry key and returns it for custom read/write.
		/// </summary>
		/// <returns>RegistryKey</returns>
		public RegistryKey OpenKey()
		{
			if (_keySoftware == null)
			{
				_keySoftware = Registry.CurrentUser.CreateSubKey("Software");
				_keyDSShared = _keySoftware.CreateSubKey(_regKey);
				_keyLabel    = _keyDSShared.CreateSubKey(_name);
			}
			return _keyLabel;
		} */
/*		/// <summary>
		/// Closes the registry key previously opened by OpenKey().
		/// </summary>
		public void CloseKey()
		{
			if (_keyLabel != null)
			{
				_keyLabel.Close();
				_keyDSShared.Close();
				_keySoftware.Close();
			}
			_keyLabel    =
			_keyDSShared =
			_keySoftware = null;
		} */


//		RegistryKey _keySoftware = null;
//		RegistryKey _keyDSShared = null;
//		RegistryKey _keyLabel    = null;
/*		/// <summary>
		/// Gets/Sets the global registry key everything will be saved under.
		/// </summary>
		private static string RegKey
		{
			get { return _regKey; }
			set { _regKey = value; }
		} */
	}


	/// <summary>
	/// EventArgs for saving and loading events. Used only to load/save TopView's
	/// visible-quadrant-types, to be specific.
	/// </summary>
	public class RegistryEventArgs
		:
			EventArgs
	{
		private readonly string _key;
		public string Key
		{
			get { return _key; }
		}

		private readonly bool _val;
		public bool Value
		{
			get { return _val; }
		}

		internal RegistryEventArgs(string key, bool val)
		{
			_key = key;
			_val = val;
		}



//		private readonly RegistryKey _regkey;
//		/// <summary>
//		/// The registry key that is now open for saving and loading to. Do not
//		/// close the key here.
//		/// </summary>
//		public RegistryKey OpenRegistryKey
//		{
//			get { return _regkey; }
//		}
//
//
//		/// <summary>
//		/// cTor
//		/// </summary>
//		/// <param name="regkey">registry key that has been opened for reading
//		/// and writing to</param>
//		internal RegistryEventArgs(RegistryKey regkey)
//		{
//			_regkey = regkey;
//		}
	}
}
