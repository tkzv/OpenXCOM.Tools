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
		private Dictionary<string, PropertyInfo> _properties;

		private object _obj;
		private string _name;

//		private bool _saveOnClose = true;

		private const string _regKey = "DSShared"; // was static not const. Cf, property 'RegKey'.

		/// <summary>
		/// Event fired when retrieving values from the registry. This happens after the values are read and set in the object.
		/// </summary>
		public event RegistrySaveLoadHandler Loading;

		/// <summary>
		/// Event fired when saving values to the registry. This happens after the values are saved.
		/// </summary>
		public event RegistrySaveLoadHandler Saving;

//		RegistryKey _swKey = null;
//		RegistryKey _riKey = null;
//		RegistryKey _ppKey = null;

/*		/// <summary>
		/// Gets/Sets the global registry key everything will be saved under.
		/// </summary>
		private static string RegKey
		{
			get { return _regKey; }
			set { _regKey = value; }
		} */


		/// <summary>
		/// Constructor that uses the name parameter as the registry key to save values under.
		/// </summary>
		/// <param name="obj">the object to save/load values into the registry</param>
		/// <param name="name">the name of the registry key to save/load</param>
		public RegistryInfo(object obj, string name)
		{
			_obj = obj;
			_name = name;

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
		/// Constructor that uses the ToString() value of the object as the name of the constructor parameter.
		/// </summary>
		/// <param name="obj"></param>
		public RegistryInfo(object obj)
			:
			this(obj, obj.GetType().ToString())
		{}


		// TODO: Implement a btn to clear registry keys. See 'PathsEditor'.

/*		/// <summary>
		/// Deletes the key located at HKEY_CURRENT_USER\Software\RegKey\
		/// RegKey is the public static parameter of this class.
		/// </summary>
		/// <param name="saveOnClose">if false, a future call to Save() will have no effect</param>
		public void ClearKey(bool saveOnClose)
		{
			RegistryKey swKey = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey riKey = swKey.CreateSubKey(_regKey);
			riKey.DeleteSubKey(_name);
			_saveOnClose = saveOnClose;
		} */

/*		/// <summary>
		/// Calls ClearKey(false).
		/// </summary>
		public void ClearKey()
		{
			ClearKey(false);
		} */

		/// <summary>
		/// Loads the specified values from the registry. Parameters match those needed for a Form.Load event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Load(object sender, EventArgs e)
		{
			RegistryKey swKey = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey riKey = swKey.CreateSubKey(_regKey);
			RegistryKey ppKey = riKey.CreateSubKey(_name);

			foreach (string st in _properties.Keys)
				_properties[st].SetValue(
									_obj,
									ppKey.GetValue(st, _properties[st].GetValue(_obj, null)),
									null);

			if (Loading != null)
				Loading(this, new RegistrySaveLoadEventArgs(ppKey));

			ppKey.Close();
			riKey.Close();
			swKey.Close();
		}

/*		/// <summary>
		/// Opens the registry key and returns it for custom read/write.
		/// </summary>
		/// <returns>RegistryKey</returns>
		public RegistryKey OpenKey()
		{
			if (_swKey == null)
			{
				_swKey = Registry.CurrentUser.CreateSubKey("Software");
				_riKey = _swKey.CreateSubKey(_regKey);
				_ppKey = _riKey.CreateSubKey(_name);
			}
			return _ppKey;
		} */

/*		/// <summary>
		/// Closes the registry key previously opened by OpenKey().
		/// </summary>
		public void CloseKey()
		{
			if (_ppKey != null)
			{
				_ppKey.Close();
				_riKey.Close();
				_swKey.Close();
			}
			_ppKey =
			_riKey =
			_swKey = null;
		} */

		/// <summary>
		/// Adds properties to be saved/loaded.
		/// </summary>
		/// <param name="names">the names of the properties to be saved/loaded</param>
		public void AddProperty(params string[] names)
		{
			var type = _obj.GetType();
			foreach (string st in names)
				AddProperty(type.GetProperty(st));
		}

		/// <summary>
		/// Adds a property to be saved/loaded.
		/// </summary>
		/// <param name="property"></param>
		private void AddProperty(PropertyInfo property)
		{
			_properties[property.Name] = property;
		}

		/// <summary>
		/// Saves the specified values into the registry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Save(object sender, EventArgs e)
		{
			var form = _obj as Form;
			if (form != null)
				form.WindowState = FormWindowState.Normal;

//			if (_saveOnClose)
//			{
			RegistryKey swKey = Registry.CurrentUser.CreateSubKey("Software");
			RegistryKey riKey = swKey.CreateSubKey(_regKey);
			RegistryKey ppKey = riKey.CreateSubKey(_name);

			foreach (string st in _properties.Keys)
				ppKey.SetValue(
							st,
							_properties[st].GetValue(_obj, null));

			if (Saving != null)
				Saving(this, new RegistrySaveLoadEventArgs(ppKey));

			ppKey.Close();
			riKey.Close();
			swKey.Close();
//			}
		}

		/// <summary>
		/// Method intended for use with Form.Closing events - directly calls Save().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Closing(object sender, EventArgs e)
		{
			Save(sender, e);
		}
	}


	/// <summary>
	/// EventArgs for saving and loading events
	/// </summary>
	public class RegistrySaveLoadEventArgs
		:
		EventArgs
	{
		private readonly RegistryKey _key;

		/// <summary>
		/// cTor
		/// </summary>
		/// <param name="openKey">registry key that has been opened for reading and writing to</param>
		public RegistrySaveLoadEventArgs(RegistryKey openKey)
		{
			_key = openKey;
		}

		/// <summary>
		/// The registry key that is now open for saving and loading to. Do not close the key when finished.
		/// </summary>
		public RegistryKey OpenKey
		{
			get { return _key; }
		}
	}
}
