using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using XCom;


namespace MapView
{
	internal delegate void ValueChangedEventHandler(object sender, string key, object value); // TODO: FxCop CA1009.

	internal delegate string ConvertObjectHandler(object value);


	/// <summary>
	/// A wrapper around a Hashtable for Setting objects. Setting objects are
	/// intended to use with the OptionsPropertyGrid.
	/// </summary>
	public sealed class Settings
	{
		private Dictionary<string, Setting> _settingsDictionary;
		private Dictionary<string, Property> _propertiesDictionary;

		private static Dictionary<Type, ConvertObjectHandler> _converters;

/*		public static void AddConverter(Type type, ConvertObjectHandler obj)
		{
			if (_converters == null)
				_converters = new Dictionary<Type, ConvertObjectHandler>();

			_converters[type] = obj;
		} */


		internal Settings()
		{
			_settingsDictionary   = new Dictionary<string, Setting>();
			_propertiesDictionary = new Dictionary<string, Property>();

			if (_converters == null)
			{
				_converters = new Dictionary<Type, ConvertObjectHandler>();
				_converters[typeof(Color)] = new ConvertObjectHandler(ConvertColor);
			}
		}


		internal static void ReadSettings(
				Varidia vars,
				KeyvalPair keyval,
				Settings settings)
		{
			while ((keyval = vars.ReadLine()) != null)
			{
				switch (keyval.Keyword)
				{
					case "{": // starting out
						break;

					case "}": // all done
						return;

					default:
						if (settings[keyval.Keyword] != null)
						{
							LogFile.WriteLine("\nReadSettings keyval= " + keyval);
							LogFile.WriteLine(". settings[keval.Keyword]= " + settings[keyval.Keyword].Value);
							settings[keyval.Keyword].Value = keyval.Value;
							settings[keyval.Keyword].doUpdate(keyval.Keyword);
						}
						break;
				}
			}
		}

		/// <summary>
		/// Gets the dictionary for this Settings.
		/// </summary>
		internal Dictionary<string, Setting>.KeyCollection Keys
		{
			get { return _settingsDictionary.Keys; }
		}

		/// <summary>
		/// Gets/Sets the Setting tied to the input string.
		/// </summary>
		internal Setting this[string key]
		{
			get
			{
				key = key.Replace(" ", String.Empty);
				return (_settingsDictionary.ContainsKey(key)) ? _settingsDictionary[key]
															  : null;
			}
/*			set
			{
				key = key.Replace(" ", String.Empty);
				if (!_dictSettings.ContainsKey(key))
					_dictSettings.Add(key, value);
				else
				{
					_dictSettings[key] = value;
//					value.Key = key;
				}
			} */
		}

		/// <summary>
		/// Adds a Setting to a specified target.
		/// </summary>
		/// <param name="key">property key - any spaces will be removed</param>
		/// <param name="value">start value of the property</param>
		/// <param name="desc">property description</param>
		/// <param name="category">property category</param>
		/// <param name="valueChangedEvent">event handler to receive the
		/// PropertyValueChanged event</param>
		/// <param name="target">the object that will receive the changed
		/// property values: an internal event handler will be created and the
		/// name must be the name of a property of the type that the target is
		/// whatever that meant</param>
		internal void AddSetting(
				string key,
				object value,
				string desc,
				string category,
				ValueChangedEventHandler valueChangedEvent = null,
				object target = null)
		{
			key = key.Replace(" ", String.Empty);

			Setting setting;
			if (!_settingsDictionary.ContainsKey(key))
			{
				setting = new Setting(value, desc, category);
				_settingsDictionary[key] = setting;
			}
			else
			{
				setting = _settingsDictionary[key];
				setting.Value = value;
				setting.Description = desc;
			}

			if (valueChangedEvent != null)
			{
				setting.ValueChangedEvent += valueChangedEvent;
			}
			else if (target != null)
			{
				_propertiesDictionary[key] = new Property(target, key);
				this[key].ValueChangedEvent += OnValueChanged;
			}
		}

		/// <summary>
		/// Gets the object tied to the string. If there is no object one will
		/// be created with the value specified.
		/// </summary>
		/// <param name="key">the name of the setting object</param>
		/// <param name="value">if there is no Setting object tied to the
		/// string, a Setting will be created with this as its Value</param>
		/// <returns>the Setting object tied to the string</returns>
		internal Setting GetSetting(string key, object value)
		{
			if (!_settingsDictionary.ContainsKey(key))
			{
				var setting = new Setting(value, null, null);
				_settingsDictionary.Add(key, setting);
//				setting.Key = key;
			}
			return _settingsDictionary[key];
		}

		private void OnValueChanged(object sender, string key, object val)
		{
//			System.Windows.Forms.PropertyValueChangedEventArgs pe = (System.Windows.Forms.PropertyValueChangedEventArgs)e;
			LogFile.WriteLine("\nOnValueChanged key= " + key + " val= " + val);
			_propertiesDictionary[key].SetValue(val);
		}

		internal void Save(string line, System.IO.TextWriter sw)
		{
			sw.WriteLine(line);
			sw.WriteLine("{");

			foreach (string key in _settingsDictionary.Keys)
				sw.WriteLine("\t" + key + ":" + Convert(this[key].Value));

			sw.WriteLine("}");
		}

		private static string Convert(object obj)
		{
			return (_converters.ContainsKey(obj.GetType())) ? _converters[obj.GetType()](obj)
															: obj.ToString();
		}

		private static string ConvertColor(object obj)
		{
			var color = (Color)obj;
			if (!color.IsKnownColor && !color.IsNamedColor && !color.IsSystemColor)
				return string.Format(
								System.Globalization.CultureInfo.InvariantCulture,
								"{0},{1},{2},{3}",
								color.A, color.R, color.G, color.B);

			return color.Name;
		}
	}


	/// <summary>
	/// Stores information to be used in the OptionsPropertyGrid.
	/// </summary>
	public sealed class Setting
	{
		internal event ValueChangedEventHandler ValueChangedEvent;


		private static Dictionary<Type, ParseString> _converters;

		private delegate object ParseString(string st);

		private static object ParseStringBool(string st)
		{
			return bool.Parse(st);
		}

		private static object ParseStringInt(string st)
		{
			return int.Parse(st, System.Globalization.CultureInfo.InvariantCulture);
		}

		private static object ParseStringColor(string st)
		{
			string[] vals = st.Split(',');

			switch (vals.Length)
			{
				case 1:
					return Color.FromName(st);

				case 3:
					return Color.FromArgb(
									int.Parse(vals[0], System.Globalization.CultureInfo.InvariantCulture),
									int.Parse(vals[1], System.Globalization.CultureInfo.InvariantCulture),
									int.Parse(vals[2], System.Globalization.CultureInfo.InvariantCulture));
			}

			return Color.FromArgb(
								int.Parse(vals[0], System.Globalization.CultureInfo.InvariantCulture),
								int.Parse(vals[1], System.Globalization.CultureInfo.InvariantCulture),
								int.Parse(vals[2], System.Globalization.CultureInfo.InvariantCulture),
								int.Parse(vals[3], System.Globalization.CultureInfo.InvariantCulture));
		}


		private object _value;
		internal object Value
		{
			get { return _value; }
			set
			{
				if (_value != null)
				{
					var type = _value.GetType();
					if (_converters.ContainsKey(type))
					{
						string val = value as String;
						if (val != null)
						{
							_value = _converters[type](val);
							return;
						}
					}
				}
				_value = value;
			}
		}


		internal Setting(object value, string desc, string category)
		{
			_value = value;
			Description = desc;
			Category = category;

			if (_converters == null)
			{
				_converters = new Dictionary<Type, ParseString>();

				_converters[typeof(int)]   = ParseStringInt;
				_converters[typeof(Color)] = ParseStringColor;
				_converters[typeof(bool)]  = ParseStringBool;
			}
		}


		internal bool IsTrue
		{
			get
			{
				if (Value is bool)
					return (bool)Value;

				return false;
			}
		}

		internal string Description
		{ get; set; }

		internal string Category
		{ get; set; }

//		internal string Key
//		{ get; set; }

		internal void doUpdate(string key, object value) // FxCop CA1030:UseEventsWhereAppropriate
		{
			if (ValueChangedEvent != null)
				ValueChangedEvent(this, key, value);
		}

		internal void doUpdate(string key) // FxCop CA1030:UseEventsWhereAppropriate
		{
			LogFile.WriteLine("\ndoUpdate key= " + key);
			if (ValueChangedEvent != null)
				ValueChangedEvent(this, key, _value);
		}
	}


	/// <summary>
	/// Property struct.
	/// </summary>
	internal struct Property
	{
		private readonly PropertyInfo _info;
		private readonly object _obj;


		public Property(object obj, string property)
		{
			LogFile.WriteLine("\nProperty cTor _obj= " + obj);
			LogFile.WriteLine(". property= " + property);
			LogFile.WriteLine(". obj.GetType()= " + obj.GetType());
			LogFile.WriteLine(". obj.GetType().GetProperty(property)= " + obj.GetType().GetProperty(property));
			_obj  = obj;
			_info = obj.GetType().GetProperty(property);
			LogFile.WriteLine(". _info= " + _info);
		}


		public void SetValue(object obj)
		{
			LogFile.WriteLine("\nSetValue obj= " + obj);
			LogFile.WriteLine(". _obj= " + _obj);
			_info.SetValue(_obj, obj, new object[]{});
		}
	}
}
