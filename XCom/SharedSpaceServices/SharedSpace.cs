using System;
using System.Collections.Generic;
using XCom.Interfaces;


namespace XCom
{
	public class SharedSpace
	{
		private static SharedSpace _instance;

		private readonly Dictionary<string, object> _share;


		public SharedSpace()
		{
			_share = new Dictionary<string, object>();
		}

		public static SharedSpace Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SharedSpace();

				return _instance;
			}
		}


		public object GetObj(string key)
		{
			return GetObj(key, null);
		}

		/// <summary>
		/// Gets a value in the SharedSpace.
		/// </summary>
		/// <param name="key">the key to look for</param>
		/// <param name="val">the object to add and return if the
		/// current value doesn't exist or is null</param>
		/// <returns>the value associated with the key as an object</returns>
		public object GetObj(string key, object val)
		{
			if (!_share.ContainsKey(key))
			{
				_share.Add(key, val);
			}
			else if (_share[key] == null)
			{
				_share[key] = val;
			}

			return _share[key];
		}

		public object this[string key]
		{
			get { return _share[key]; }
			set { _share[key] = value; }
		}

		public int GetInt(string key)
		{
			return (int)_share[key];
		}

		public string GetString(string key)
		{
			return (string)_share[key];
		}

		public double GetDouble(string key)
		{
			return (double)_share[key];
		}

		public List<IXCImageFile> GetImageModList()
		{
			return (List<IXCImageFile>)_share["ImageMods"];
		}

		public Dictionary<string, Palette> GetPaletteTable()
		{
			return (Dictionary<string, Palette>)_share["Palettes"];
		}
	}
}
