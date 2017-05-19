using System;
using System.Collections.Generic;

//using XCom.Interfaces;


namespace XCom
{
	public sealed class SharedSpace
	{
		#region Fields & Properties
		private static SharedSpace _instance;
		public static SharedSpace Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SharedSpace();

				return _instance;
			}
		}

		private readonly Dictionary<string, object> _share = new Dictionary<string, object>();
		public object this[string key]
		{
			get { return _share[key]; }
			set { _share[key] = value; }
		}

		public const string ApplicationDirectory   = "ApplicationDirectory";
		public const string SettingsDirectory      = "SettingsDirectory"; // TODO: just put the Settings aka. Configuration files in the appdir.
		public const string ResourcesDirectoryUfo  = "ResourcesDirectoryUfo";
		public const string ResourcesDirectoryTftd = "ResourcesDirectoryTftd";

		public const string Palettes        = "Palettes"; // for PckView ->
//		public const string CustomDirectory = "CustomDirectory";
//		public const string ImageTypes      = "ImageTypes";

//		public const string CursorFileUfo    = "cursorFileUfo";		// the cursors are determined in XCMainWindow.cTor
//		public const string CursorFileTftd   = "cursorFileTftd";	// <- not currently implemented, per se.
		public const string CursorFilePrefix = @"UFOGRAPH\CURSOR";
		#endregion


//		public SharedSpace()
//		{}

		// TODO: Since SharedSpace holds only string-values factor away the
		// boxing and just use strings. Actually, Palettes is a dictionary ...
		// but it should be changed into a variable that's local to PckView
		// anyway.
		//
		// NOTE: which means that SharedSpace and PathInfo have very similar
		// usages and ought be merged.


		#region Methods
		/// <summary>
		/// Allocates a key-val pair in the SharedSpace and returns the value
		/// that is assigned. This does not change the value of an existing key
		/// unless its value is null.
		/// </summary>
		/// <param name="key">the key to look for</param>
		/// <param name="value">the object to add if the current value doesn't
		/// exist or is null (default null)</param>
		/// <returns>the value associated with the key as an object</returns>
		public object SetShare(string key, object value = null)
		{
			if (!_share.ContainsKey(key))
			{
				_share.Add(key, value);
			}
			else if (_share[key] == null)
			{
				_share[key] = value;
			}

			return _share[key];
		}

		public string GetShare(string key)
		{
			return _share[key] as String;
		}
		#endregion
	}
}

//		public int GetIntegralValue(string key) // not used.
//		{
//			return (int)_share[key];
//		}
//		public double GetDouble(string key) // not used.
//		{
//			return (double)_share[key];
//		}
//		public List<XCImageFile> GetImageModList()
//		{
//			return (List<XCImageFile>)_share[ImageTypes];
//		}
//		public Dictionary<string, Palette> GetPaletteTable()
//		{
//			return (Dictionary<string, Palette>)_share[Palettes];
//		}
