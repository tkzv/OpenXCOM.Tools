using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using XCom;


namespace MapView
{
	public delegate string ConvertObject(object obj);
	public delegate void ValueChangedDelegate(object sender, string keyword, object val);


	/// <summary>
	/// A wrapper around a Hashtable for Setting objects. Setting objects are
	/// intended to use with the CustomPropertyGrid.
	/// </summary>
	public class Settings
	{
		private Dictionary<string, Setting> _settings;
		private Dictionary<string, PropertyObject> _propertyObject;

		private static Dictionary<Type,ConvertObject> converters;

		public static void AddConverter(Type t, ConvertObject obj)
		{
			if (converters == null)
				converters = new Dictionary<Type, ConvertObject>();

			converters[t] = obj;
		}


		public Settings()
		{
			_settings = new Dictionary<string, Setting>();
			_propertyObject = new Dictionary<string, PropertyObject>();

			if (converters == null)
			{
				converters = new Dictionary<Type,ConvertObject>();
				converters[typeof(Color)] = new ConvertObject(ConvertColor);
			}
		}


		public static void ReadSettings(
				VarCollection vars,
				KeyVal keyVal,
				Settings curSettings)
		{
			while ((keyVal = vars.ReadLine()) != null)
			{
				switch (keyVal.Keyword)
				{
					case "{": // starting out
						break;

					case "}": // all done
						return;

					default:
						if (curSettings[keyVal.Keyword] != null)
						{
							curSettings[keyVal.Keyword].Value = keyVal.Rest;
							curSettings[keyVal.Keyword].FireUpdate(keyVal.Keyword);
						}
						break;
				}
			}
		}

		/// <summary>
		/// Gets the key collection for this Settings object. Every key is a string.
		/// </summary>
		public Dictionary<string,Setting>.KeyCollection Keys
		{
			get { return _settings.Keys; }
		}

		/// <summary>
		/// Gets/Sets the Setting object tied to the input string.
		/// </summary>
		public Setting this[string key]
		{
			get
			{
				key = key.Replace(" ", String.Empty);
				return (_settings.ContainsKey(key)) ? _settings[key]
													: null;
			}

			set
			{
				key = key.Replace(" ", String.Empty);
				if (!_settings.ContainsKey(key))
					_settings.Add(key, value);
				else
				{
					_settings[key] = value;
					value.Name = key;
				}
			}
		}

		/// <summary>
		/// Adds a setting to a specified object.
		/// </summary>
		/// <param name="name">property name</param>
		/// <param name="val">start value of the property</param>
		/// <param name="desc">property description</param>
		/// <param name="category">property category</param>
		/// <param name="update">event handler to recieve the PropertyValueChanged event</param>
		/// <param name="reflect">if true, an internal event handler will be created - the refObj
		/// must not be null and the name must be the name of a property of the type that refObj is</param>
		/// <param name="refObj">the object that will recieve the changed property values</param>
		public void AddSetting(
				string name,
				object val,
				string desc,
				string category,
				ValueChangedDelegate update,
				bool reflect,
				object refObj)
		{
			name = name.Replace(" ", String.Empty);

			Setting setting;
			if (!_settings.ContainsKey(name))
			{
				setting = new Setting(val, desc, category);
				_settings[name] = setting;
			}
			else
			{
				setting = _settings[name];
				setting.Value = val;
				setting.Description = desc;
			}

			if (update != null)
				setting.ValueChanged += update;

			if (reflect && refObj != null)
			{
				_propertyObject[name] = new PropertyObject(refObj, name);
				this[name].ValueChanged += ReflectEvent;
			}
		}

		/// <summary>
		/// Gets the Setting object tied to the string. If there is no Setting
		/// object, one will be created with the defaultValue.
		/// </summary>
		/// <param name="key">the name of the setting object</param>
		/// <param name="val">if there is no Setting object tied to the
		/// string, a Setting will be created with this as its Value</param>
		/// <returns>the Setting object tied to the string</returns>
		public Setting GetSetting(string key, object val)
		{
			if (!_settings.ContainsKey(key))
			{
				var item = new Setting(val, null, null);
				_settings.Add(key, item);
				item.Name = key;
			}
			return _settings[key];
		}

		private void ReflectEvent(object sender, string key, object val)
		{
//			System.Windows.Forms.PropertyValueChangedEventArgs pe = (System.Windows.Forms.PropertyValueChangedEventArgs)e;
			_propertyObject[key].SetValue(val);
		}

		public void Save(string line, System.IO.StreamWriter sw)
		{
			sw.WriteLine(line);
			sw.WriteLine("{");

			foreach (string st in _settings.Keys)
				sw.WriteLine("\t" + st + ":" + Convert(this[st].Value));

			sw.WriteLine("}");
		}

		private string Convert(object obj)
		{
			return (converters.ContainsKey(obj.GetType())) ? converters[obj.GetType()](obj)
														   : obj.ToString();
		}

		private static string ConvertColor(object obj)
		{
			var color = (Color)obj;
			if (color.IsKnownColor || color.IsNamedColor || color.IsSystemColor)
				return color.Name;

			return string.Format("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
		}
	}

	/// <summary>
	/// Stores information to be used in the CustomPropertyGrid.
	/// </summary>
	public class Setting
	{
		private object _val;

		private static Dictionary<Type,parseString> converters;

		public event ValueChangedDelegate ValueChanged;

		private delegate object parseString(string s);

		private static object parseBoolString(string s)
		{
			return bool.Parse(s);
		}

		private static object parseIntString(string s)
		{
			return int.Parse(s);
		}

		private static object parseColorString(string s)
		{
			string[] vals = s.Split(',');

			switch (vals.Length)
			{
				case 1:
					return Color.FromName(s);

				case 3:
					return Color.FromArgb(
									int.Parse(vals[0]),
									int.Parse(vals[1]),
									int.Parse(vals[2]));
			}

			return Color.FromArgb(
								int.Parse(vals[0]),
								int.Parse(vals[1]),
								int.Parse(vals[2]),
								int.Parse(vals[3]));
		}

		public Setting(object val, string desc, string category)
		{
			_val = val;
			Description = desc;
			Category = category;

			if (converters == null)
			{
				converters = new Dictionary<Type, parseString>();

				converters[typeof(int)]   = parseIntString;
				converters[typeof(Color)] = parseColorString;
				converters[typeof(bool)]  = parseBoolString;
			}
		}

		public bool ValueBool
		{
			get
			{
				if (Value is bool)
					return (bool)Value;

				return false;
			}
		}

		public object Value
		{
			get { return _val; }
			set
			{
				if (_val != null)
				{
					var type = _val.GetType();
					if (converters.ContainsKey(type) && value is string)
					{
						_val = converters[type]((string)value);
						return;
					}
				}
				_val = value;
			}
		}

		public string Description
		{ get; set; }

		public string Category
		{ get; set; }

		public string Name
		{ get; set; }

		public void FireUpdate(string key, object val)
		{
			if (ValueChanged != null)
				ValueChanged(this, key, val);
		}

		public void FireUpdate(string key)
		{
			if (ValueChanged != null)
				ValueChanged(this, key, _val);
		}
	}


	/// <summary>
	/// struct PropertyObject
	/// </summary>
	internal struct PropertyObject
	{
		public PropertyInfo _info;
		public object _obj;


		public PropertyObject(object obj, string property)
		{
			_obj  = obj;
			_info = obj.GetType().GetProperty(property);
		}


		public void SetValue(object obj)
		{
			_info.SetValue(_obj, obj, new object[]{});
		}
	}
}
