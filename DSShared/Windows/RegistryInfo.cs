using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;


namespace DSShared.Windows
{
	/// <summary>
	/// Delegate for use in the saving and loading events raised in the RegistryInfo class.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void RegistrySaveLoadHandler(object sender, RegistrySaveLoadEventArgs e);


	/// <summary>
	/// A class to help facilitate the saving and loading of values into the registry in a central location.
	/// </summary>
	public class RegistryInfo
	{
		private const string _regKey = "DSShared"; // was static not const. Cf, property 'RegKey'.

		private object _obj;
		private string _label;

		private Dictionary<string, PropertyInfo> _properties;

//		private bool _saveOnClose = true;

		/// <summary>
		/// Event fired when retrieving values from the registry. This happens
		/// after the values are read and set in the object.
		/// </summary>
		public event RegistrySaveLoadHandler Loading;

		/// <summary>
		/// Event fired when saving values to the registry. This happens after
		/// the values are saved.
		/// </summary>
		public event RegistrySaveLoadHandler Saving;


		/// <summary>
		/// Constructor that uses the name parameter as the registry key to save
		/// values under.
		/// </summary>
		/// <param name="obj">the object to save/load values into the registry</param>
		/// <param name="label">the name of the registry key to save/load</param>
		public RegistryInfo(object obj, string label)
		{
			_obj = obj;
			_label = label;

			_properties = new Dictionary<string, PropertyInfo>();

			var f = (obj as Form);
			if (f != null)
			{
				f.StartPosition = FormStartPosition.Manual;
				f.Load += Load;
				f.Closing += Closing;
				AddProperty("Width", "Height", "Left", "Top");
			}
		}

		/// <summary>
		/// Constructor that uses the ToString() value of the object as the name
		/// of the constructor parameter.
		/// </summary>
		/// <param name="obj"></param>
		public RegistryInfo(object obj)
			:
			this(obj, obj.GetType().ToString())
		{}


		/// <summary>
		/// Loads the specified values from the registry. Parameters match those
		/// needed for a Form.Load event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Load(object sender, EventArgs e)
		{
			RegistryKey keySoftware = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey keyDSShared = keySoftware.CreateSubKey(_regKey);
			RegistryKey keyLabel    = keyDSShared.CreateSubKey(_label);

			foreach (string st in _properties.Keys)
				_properties[st].SetValue(
									_obj,
									keyLabel.GetValue(st, _properties[st].GetValue(_obj, null)),
									null);

			if (Loading != null)
				Loading(this, new RegistrySaveLoadEventArgs(keyLabel));

			keyLabel.Close();
			keyDSShared.Close();
			keySoftware.Close();
		}

		/// <summary>
		/// Adds properties to be saved/loaded.
		/// </summary>
		/// <param name="labels">the names of the properties to be saved/loaded</param>
		public void AddProperty(params string[] labels)
		{
			var type = _obj.GetType();

			PropertyInfo info;
			foreach (string st in labels)
			{
				info = type.GetProperty(st);
				_properties[info.Name] = info;
			}
		}

		/// <summary>
		/// Saves the specified values to the registry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Save(object sender, EventArgs e)
		{
			var f = _obj as Form;
			if (f != null)
				f.WindowState = FormWindowState.Normal;

//			if (_saveOnClose)
//			{
			RegistryKey keySoftware = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey keyDSShared = keySoftware.CreateSubKey(_regKey);
			RegistryKey keyLabel    = keyDSShared.CreateSubKey(_label);

			foreach (string st in _properties.Keys)
				keyLabel.SetValue(
							st,
							_properties[st].GetValue(_obj, null));

			if (Saving != null)
				Saving(this, new RegistrySaveLoadEventArgs(keyLabel));

			keyLabel.Close();
			keyDSShared.Close();
			keySoftware.Close();
//			}
		}

		/// <summary>
		/// Method intended for use with Form.Closing events - directly calls
		/// Save() to the registry for various windows.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Closing(object sender, EventArgs e)
		{
			Save(sender, e);
		}

		// TODO: Implement a btn to clear registry keys. See 'PathsEditor'.

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
	/// EventArgs for saving and loading events.
	/// </summary>
	public class RegistrySaveLoadEventArgs
		:
		EventArgs
	{
		private readonly RegistryKey _key;
		/// <summary>
		/// The registry key that is now open for saving and loading to. Do not
		/// close the key here.
		/// </summary>
		public RegistryKey OpenRegistryKey
		{
			get { return _key; }
		}


		/// <summary>
		/// cTor
		/// </summary>
		/// <param name="key">registry key that has been opened for reading
		/// and writing to</param>
		public RegistrySaveLoadEventArgs(RegistryKey key)
		{
			_key = key;
		}
	}
}
