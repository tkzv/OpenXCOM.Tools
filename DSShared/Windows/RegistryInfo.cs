using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel;	// read values (deserialization)
using YamlDotNet.Serialization;			// write values


namespace DSShared.Windows
{
/*	/// <summary>
	/// Delegate for use in the saving and loading events raised in the
	/// RegistryInfo class.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
//	public delegate void EventHandler<RegistryEventArgs>(object sender, RegistryEventArgs e);
	public delegate void RegistryEventHandler(object sender, RegistryEventArgs e);
*/

	/// <summary>
	/// A class to help facilitate the saving and loading of values into the
	/// registry in a central location.
	/// </summary>
	public class RegistryInfo
	{
		#region Fields (static)
		// viewer labels (keys)
		public const string MainView     = "MainView";
		public const string TopView      = "TopView";
		public const string RouteView    = "RouteView";
		public const string TopRouteView = "TopRouteView";
		public const string TileView     = "TileView";

		public const string Console      = "Console";
		public const string Options      = "Options";

		public const string PckView      = "PckView";

		// viewer property metrics
		private const string PropLeft   = "Left";
		private const string PropTop    = "Top";
		private const string PropWidth  = "Width";
		private const string PropHeight = "Height";

		// obsolete ->
//		public const string SoftwareRegistry = "Software";
//		public const string MapViewRegistry  = "MapView";
		#endregion


		#region Fields
		private readonly string _viewer;
		private readonly Form _f;

		private readonly Dictionary<string, PropertyInfo> _infoDictionary = new Dictionary<string, PropertyInfo>();

//		private bool _saveOnClose = true;
		#endregion


/*		/// <summary>
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
		public event RegistryEventHandler RegistrySaveEvent; */


		#region cTor
		/// <summary>
		/// Main cTor. Uses the specified string as a registry key.
		/// </summary>
		/// <param name="viewer">the label of the viewer to save/load</param>
		/// <param name="f">the form-object corresponding to the label</param>
		public RegistryInfo(string viewer, Form f)
		{
			_viewer = viewer;
			_f      = f;

			RegisterProperties(
							PropLeft,
							PropTop,
							PropWidth,
							PropHeight);

			_f.StartPosition = FormStartPosition.Manual;
			_f.Load         += OnLoad;
			_f.FormClosing  += OnFormClosing;
		}
//		/// <summary>
//		/// Auxiliary cTor. Uses the ToString() return of the specified object
//		/// as a registry key.
//		/// </summary>
//		/// <param name="obj"></param>
//		public RegistryInfo(object obj)
//			:
//				this(obj, obj.GetType().ToString())
//		{}
		#endregion


		#region Methods
		/// <summary>
		/// Adds properties to be saved/loaded.
		/// </summary>
		/// <param name="keys">the keys of the properties to be saved/loaded</param>
		private void RegisterProperties(params string[] keys)
		{
			//DSLogFile.WriteLine("RegisterProperties");
			var type = _f.GetType();
			//DSLogFile.WriteLine(". type= " + type);

			PropertyInfo info;
			foreach (string key in keys)
			{
				//DSLogFile.WriteLine(". . key= " + key);
				if ((info = type.GetProperty(key)) != null)
				{
					//DSLogFile.WriteLine(". . . info= " + info.Name);
					_infoDictionary[info.Name] = info;
				}
			}
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// Loads values for the subsidiary viewers from "MapViewers.yml".
		/// - TopView      (size and position, but not Quadrant visibilities)
		/// - RouteView    (size and position)
		/// - TopRouteView (size and position)
		/// - TileView     (size and position)
		/// - Console      (size and position)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string pfeViewers  = Path.Combine(
											dirSettings,
											PathInfo.ConfigViewers);
			if (File.Exists(pfeViewers))
			{
				using (var sr = new StreamReader(File.OpenRead(pfeViewers)))
				{
					var fileViewers = new YamlStream();
					fileViewers.Load(sr);

					var nodeRoot = (YamlMappingNode)fileViewers.Documents[0].RootNode; // TODO: Error handling. ->
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, _viewer, StringComparison.Ordinal))
							ImportMetrics((YamlMappingNode)nodeRoot.Children[new YamlScalarNode(viewer)]);
					}
				}
			}
		}

		/// <summary>
		/// Helper for OnLoad().
		/// </summary>
		/// <param name="keyvals">yaml-mapped keyval pairs</param>
		private void ImportMetrics(YamlMappingNode keyvals)
		{
			//DSLogFile.WriteLine("ImportValues");
			string key;
			int val;

			foreach (var keyval in keyvals)
			{
				key = keyval.Key.ToString();
				//DSLogFile.WriteLine(". key= " + key);

				var cultureInfo = CultureInfo.InvariantCulture;
				key = cultureInfo.TextInfo.ToTitleCase(key);
				//DSLogFile.WriteLine(". Key= " + key);

//				if (key.StartsWith("vis", StringComparison.Ordinal))	// NOTE: vis# are for TopView's visible-quadrant-type MenuItem toggles;
//				{														// these were handled by TopView.OnExtraRegistrySettingsLoad()
//					if (RegistryLoadEvent != null)
//					{
//						val = Boolean.Parse(keyval.Value.ToString());
//						RegistryLoadEvent(this, new RegistryEventArgs(key, Convert.ToBoolean(val)));
//					}
//				}
//				else
//				if (_infoDictionary.ContainsKey(key)) // it'll be there, i trust. yeah right
//				{
				val = Int32.Parse(keyval.Value.ToString(), cultureInfo);
				//DSLogFile.WriteLine(". val= " + val);
				//if (!_infoDictionary.ContainsKey(key)) DSLogFile.WriteLine(". . infoDictionary does NOT contain " + key);

				switch (key) // check to ensure that viewer is at least partly onscreen.
				{
					case PropLeft:
					{
						var rectScreen = Screen.GetWorkingArea(new System.Drawing.Point(val, 0));
						if (!rectScreen.Contains(val + 200, 0))
							val = 100;
						break;
					}

					case PropTop:
					{
						var rectScreen = Screen.GetWorkingArea(new System.Drawing.Point(0, val));
						if (!rectScreen.Contains(0, val + 100))
							val = 50;
						break;
					}
				}

				_infoDictionary[key].SetValue(_f, val, null);

//				}
			}
		}

//		private void OnLoad(object sender, EventArgs e)
//		{
//			using (RegistryKey regkeySoftware = Registry.CurrentUser.CreateSubKey(SoftwareRegistry))
//			using (RegistryKey regkeyMapView  = regkeySoftware.CreateSubKey(MapViewRegistry))
//			using (RegistryKey regkeyViewer   = regkeyMapView.CreateSubKey(_regkey))
//			{
//				foreach (string key in _infoDictionary.Keys)
//					_infoDictionary[key].SetValue(
//										_obj,
//										regkeyViewer.GetValue(key, _infoDictionary[key].GetValue(_obj, null)),
//										null);
//
//				if (RegistryLoadEvent != null)
//					RegistryLoadEvent(this, new RegistryEventArgs(regkeyViewer));
//
//				regkeyViewer.Close();
//				regkeyMapView.Close();
//				regkeySoftware.Close();
//			}
//		}

		/// <summary>
		/// Saves values for the subsidiary viewers to "MapViewers.yml".
		/// - TopView      (size and position, but does not save Quadrant visibilities)
		/// - RouteView    (size and position)
		/// - TopRouteView (size and position)
		/// - TileView     (size and position)
		/// - Console      (size and position)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormClosing(object sender, EventArgs e)
		{
			//DSLogFile.WriteLine("OnClose _viewer= " + _viewer);

//			if (_f != null)
			{
				_f.WindowState = FormWindowState.Normal;

//				if (_saveOnClose)
//				{
				string dirSettings   = Path.Combine(
												Path.GetDirectoryName(Application.ExecutablePath),
												PathInfo.SettingsDirectory);
				string pfeViewers    = Path.Combine(
												dirSettings,
												PathInfo.ConfigViewers);
				string pfeViewersOld = Path.Combine(
												dirSettings,
												PathInfo.ConfigViewersOld);

				if (File.Exists(pfeViewers))
				{
					File.Copy(pfeViewers, pfeViewersOld, true);

					using (var sr = new StreamReader(File.OpenRead(pfeViewersOld))) // but now use dst as src ->
					using (var fs = new FileStream(pfeViewers, FileMode.Create)) // overwrite previous config.
					using (var sw = new StreamWriter(fs))
					{
						while (sr.Peek() != -1)
						{
							string line = sr.ReadLine().TrimEnd();
							//DSLogFile.WriteLine(". line= " + line);

							// At present, MainView is the only viewer that rolls its own metrics.
							// - see the XCMainWindow load/close eventcalls.

							if (String.Equals(line, _viewer + ":", StringComparison.Ordinal))
							{
								//DSLogFile.WriteLine(". . write= " + line);
								sw.WriteLine(line);

								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine(); // heh

								sw.WriteLine("  left: "   + Math.Max(0, _f.Location.X)); // =Left
								sw.WriteLine("  top: "    + Math.Max(0, _f.Location.Y)); // =Top
								sw.WriteLine("  width: "  + _f.Width);
								sw.WriteLine("  height: " + _f.Height);
							}
/*							if (String.Equals(line, _viewer + ":", StringComparison.Ordinal))
							{
								//DSLogFile.WriteLine(". line IS _regkey");
								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine();
								line = sr.ReadLine(); // heh

//								if (String.Equals(_regkey, "TopView", StringComparison.Ordinal))
//								{
//									DSLogFile.WriteLine(". _regkey IS TopView"); // these were to leave space for visible quadrants
//									line = sr.ReadLine();
//									line = sr.ReadLine();
//									line = sr.ReadLine();
//									line = sr.ReadLine(); // heheh
//								}

								object node = null;

								switch (_viewer) // NOTE: MainView is handled by XCMainWindow load/closing events
								{
									case TopView:
									{
										//DSLogFile.WriteLine(". . _regkey IS TopView");
										node = new
										{
											TopView = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case RouteView:
									{
										//DSLogFile.WriteLine(". . _regkey IS RouteView");
										node = new
										{
											RouteView = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case TopRouteView:
									{
										//DSLogFile.WriteLine(". . _regkey IS TopRouteView");
										node = new
										{
											TopRouteView = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case TileView:
									{
										//DSLogFile.WriteLine(". . _regkey IS TileView");
										node = new
										{
											TileView = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case Console:
									{
										//DSLogFile.WriteLine(". . _regkey IS Console");
										node = new
										{
											Console = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case Options:
									{
										//DSLogFile.WriteLine(". . _regkey IS Options");
										node = new
										{
											Options = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
									case PckView:
									{
										//DSLogFile.WriteLine(". . _regkey IS PckView");
										node = new
										{
											PckView = new
											{
												left   = _infoDictionary[PropLeft]  .GetValue(_f, null),
												top    = _infoDictionary[PropTop]   .GetValue(_f, null),
												width  = _infoDictionary[PropWidth] .GetValue(_f, null),
												height = _infoDictionary[PropHeight].GetValue(_f, null)
											},
										};
										break;
									}
								}

								if (node != null)
								{
									//DSLogFile.WriteLine(". node VALID -> serialize");
									var ser = new Serializer();
									ser.Serialize(sw, node);
								}
							} */
							else
								sw.WriteLine(line);
						}
					}
					File.Delete(pfeViewersOld);
				}
//			}
			}
		}
		#endregion
	}
}

/*		/// <summary>
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
		} */


//		/// <summary>
//		/// Deletes the key located at HKEY_CURRENT_USER\Software\'_regKey'\
//		/// '_regKey' is a public static parameter of this class.
//		/// </summary>
//		/// <param name="saveOnClose">if false, a future call to Save() will
//		/// have no effect</param>
//		public void ClearKey(bool saveOnClose)
//		{
//			RegistryKey keySoftware = Registry.CurrentUser.CreateSubKey("Software");
//			RegistryKey keyDSShared = keySoftware.CreateSubKey(_regKey);
//			keyDSShared.DeleteSubKey(_name);
//			_saveOnClose = saveOnClose;
//		}
//		/// <summary>
//		/// Calls ClearKey(false).
//		/// </summary>
//		public void ClearKey()
//		{
//			ClearKey(false);
//		}


//		/// <summary>
//		/// Opens the registry key and returns it for custom read/write.
//		/// </summary>
//		/// <returns>RegistryKey</returns>
//		public RegistryKey OpenKey()
//		{
//			if (_keySoftware == null)
//			{
//				_keySoftware = Registry.CurrentUser.CreateSubKey("Software");
//				_keyDSShared = _keySoftware.CreateSubKey(_regKey);
//				_keyLabel    = _keyDSShared.CreateSubKey(_name);
//			}
//			return _keyLabel;
//		}
//		/// <summary>
//		/// Closes the registry key previously opened by OpenKey().
//		/// </summary>
//		public void CloseKey()
//		{
//			if (_keyLabel != null)
//			{
//				_keyLabel.Close();
//				_keyDSShared.Close();
//				_keySoftware.Close();
//			}
//			_keyLabel    =
//			_keyDSShared =
//			_keySoftware = null;
//		}

//		RegistryKey _keySoftware = null;
//		RegistryKey _keyDSShared = null;
//		RegistryKey _keyLabel    = null;
//		/// <summary>
//		/// Gets/Sets the global registry key everything will be saved under.
//		/// </summary>
//		private static string RegKey
//		{
//			get { return _regKey; }
//			set { _regKey = value; }
//		}

//	/// <summary>
//	/// EventArgs for saving and loading events. Used only to load/save TopView's
//	/// visible-quadrant-types, to be specific.
//	/// </summary>
//	public class RegistryEventArgs
//		:
//			EventArgs
//	{
//		private readonly string _key;
//		public string Key
//		{
//			get { return _key; }
//		}
//
//		private readonly bool _val;
//		public bool Value
//		{
//			get { return _val; }
//		}
//
//		internal RegistryEventArgs(string key, bool val)
//		{
//			_key = key;
//			_val = val;
//		}

//		/// <summary>
//		/// cTor
//		/// </summary>
//		/// <param name="regkey">registry key that has been opened for reading
//		/// and writing to</param>
//		internal RegistryEventArgs(RegistryKey regkey)
//		{
//			_regkey = regkey;
//		}

//		private readonly RegistryKey _regkey;
//		/// <summary>
//		/// The registry key that is now open for saving and loading to. Do not
//		/// close the key here.
//		/// </summary>
//		public RegistryKey OpenRegistryKey
//		{
//			get { return _regkey; }
//		}
