using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;


namespace MapView
{
	internal delegate void OptionChangedEventHandler(string key, object value);

	internal delegate string ConvertObjectHandler(object value);


	/// <summary>
	/// A wrapper around a Hashtable for Options objects. Options objects are
	/// for use with the OptionsPropertyGrid.
	/// </summary>
	public sealed class Options
	{
		#region Fields (static)
		private static Dictionary<Type, ConvertObjectHandler> _converters;
		#endregion


		#region Fields
		private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();
		private Dictionary<string, ViewerOption>      _options    = new Dictionary<string, ViewerOption>();
		#endregion


		#region Properties
		/// <summary>
		/// Gets the dictionary for this Options.
		/// </summary>
		internal Dictionary<string, ViewerOption>.KeyCollection Keys
		{
			get { return _options.Keys; }
		}

		/// <summary>
		/// Gets/Sets the ViewerOption keyed by a given key.
		/// </summary>
		internal ViewerOption this[string key]
		{
			get
			{
				key = key.Replace(" ", String.Empty);
				return (_options.ContainsKey(key)) ? _options[key]
												   : null;
			}
		}
		#endregion


		#region cTor
		internal Options()
		{
			if (_converters == null)
			{
				_converters = new Dictionary<Type, ConvertObjectHandler>();
				_converters[typeof(Color)] = new ConvertObjectHandler(ConvertColor);
			}
		}
		#endregion


		#region Methods (static)
		internal static void ReadOptions(
				Varidia vars,
				KeyvalPair keyval,
				Options options)
		{
			while ((keyval = vars.ReadLine()) != null)
			{
				switch (keyval.Key)
				{
					case "{": // starting out
						break;

					case "}": // all done
						return;

					default:
						if (options[keyval.Key] != null)
						{
							options[keyval.Key].Value = keyval.Value;
							options[keyval.Key].doUpdate(keyval.Key);
						}
						break;
				}
			}
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

//		public static void AddConverter(Type type, ConvertObjectHandler obj)
//		{
//			if (_converters == null)
//				_converters = new Dictionary<Type, ConvertObjectHandler>();
//
//			_converters[type] = obj;
//		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds an Option to a specified target.
		/// </summary>
		/// <param name="key">property key - any spaces will be removed</param>
		/// <param name="value">start value of the property</param>
		/// <param name="desc">property description</param>
		/// <param name="category">property category</param>
		/// <param name="optionChangedEvent">event handler to receive the
		/// PropertyValueChanged event</param>
		/// <param name="target">the object that will receive the changed
		/// property values: an internal event handler will be created and the
		/// name must be the name of a property of the type that the target is
		/// whatever that meant</param>
		internal void AddOption(
				string key,
				object value,
				string desc,
				string category,
				OptionChangedEventHandler optionChangedEvent = null,
				object target = null)
		{
//			key = key.Replace(" ", String.Empty); // nobody be stupid ...

			ViewerOption option;
			if (!_options.ContainsKey(key))
			{
				option = new ViewerOption(value, desc, category);
				_options[key] = option;
			}
			else
			{
				option = _options[key];
				option.Value = value;
				option.Description = desc;
			}

			if (optionChangedEvent != null)
			{
				option.OptionChangedEvent += optionChangedEvent;
			}
			else if (target != null)
			{
				_properties[key] = new Property(target, key);
				this[key].OptionChangedEvent += OnOptionChanged;
			}
		}

		/// <summary>
		/// Gets the object tied to the key. If there is no object one will be
		/// created with the value specified.
		/// </summary>
		/// <param name="key">the name of the Option object</param>
		/// <param name="value">if there is no Option object tied to the
		/// string, an Option will be created with this as its Value</param>
		/// <returns>the Option object tied to the key</returns>
		internal ViewerOption GetOption(string key, object value)
		{
			if (!_options.ContainsKey(key))
			{
				var option = new ViewerOption(value, null, null);
				_options.Add(key, option);
			}
			return _options[key];
		}

		internal void SaveOptions(string line, TextWriter sw)
		{
			sw.WriteLine(line);
			sw.WriteLine("{");

			foreach (string key in _options.Keys)
				sw.WriteLine("\t" + key + ":" + Convert(this[key].Value));

			sw.WriteLine("}");
		}
		#endregion


		#region Eventcalls
		private void OnOptionChanged(string key, object val)
		{
			_properties[key].SetValue(val);
		}
		#endregion
	}


	/// <summary>
	/// Stores information to be used in the OptionsPropertyGrid.
	/// </summary>
	public sealed class ViewerOption
	{
		#region Delegates
		private delegate object ParseString(string st);
		#endregion


		#region Events
		internal event OptionChangedEventHandler OptionChangedEvent;
		#endregion


		#region Fields (static)
		private static Dictionary<Type, ParseString> _converters;
		#endregion


		#region Properties
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

		/// <summary>
		/// Checks if 'Value' is a boolean and TRUE. else false.
		/// </summary>
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
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="desc"></param>
		/// <param name="category"></param>
		internal ViewerOption(
				object value,
				string desc,
				string category)
		{
			_value      = value;
			Description = desc;
			Category    = category;

			if (_converters == null)
			{
				_converters = new Dictionary<Type, ParseString>();

				_converters[typeof(int)]   = ParseStringInt;
				_converters[typeof(Color)] = ParseStringColor;
				_converters[typeof(bool)]  = ParseStringBool;
			}
		}
		#endregion


		#region Methods (static)
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
				{
					var invariant = System.Globalization.CultureInfo.InvariantCulture;
					return Color.FromArgb(
									int.Parse(vals[0], invariant),
									int.Parse(vals[1], invariant),
									int.Parse(vals[2], invariant));
				}

				case 4:
				{
					var invariant = System.Globalization.CultureInfo.InvariantCulture;
					return Color.FromArgb(
										int.Parse(vals[0], invariant),
										int.Parse(vals[1], invariant),
										int.Parse(vals[2], invariant),
										int.Parse(vals[3], invariant));
				}
			}
			return null;
		}
		#endregion


		#region Methods
		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		internal void doUpdate(string key, object value)
		{
			if (OptionChangedEvent != null)
				OptionChangedEvent(key, value);
		}

		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		internal void doUpdate(string key)
		{
			if (OptionChangedEvent != null)
				OptionChangedEvent(key, _value);
		}
		#endregion
	}


	/// <summary>
	/// Property struct.
	/// </summary>
	internal struct Property
	{
		#region Fields
		private readonly PropertyInfo _info;
		private readonly object _obj;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="property"></param>
		internal Property(object obj, string property)
		{
			_obj  = obj;
			_info = obj.GetType().GetProperty(property);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Sets the value of this Property to a specified object.
		/// </summary>
		/// <param name="obj"></param>
		internal void SetValue(object obj)
		{
			_info.SetValue(_obj, obj, new object[]{});
		}
		#endregion
	}
}
