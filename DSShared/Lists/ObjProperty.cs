using System;
using System.Reflection;
//using DSShared.Exceptions;


namespace DSShared.Lists
{
	/// <summary>
	/// Class that a CustomList uses to figure out the value of a particular row+column
	/// </summary>
	public class ObjProperty
	{
		private readonly PropertyInfo _property;
		private object[] _propertyId;

		private ObjProperty _nested;

		private EditStrType _editType = EditStrType.None;
		/// <summary>
		/// Gets or sets the type of the edit.
		/// </summary>
		/// <value>the type of the edit</value>
		public EditStrType EditType
		{
			get { return _editType; }
			set { _editType = value; }
		}

		private EditStrDelegate _editFunc;
		/// <summary>
		/// Gets or sets the key function. This is called when a key is pressed.
		/// on a selected row
		/// </summary>
		/// <value>the key function</value>
		public EditStrDelegate KeyFunction
		{
			get { return _editFunc; }
			set { _editFunc = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.ObjProperty"/> class.
		/// </summary>
		/// <param name="property">the propertyInfo object that will reflect on
		/// objects later on to display information with</param>
		public ObjProperty(PropertyInfo property)
			:
				this(property, null, null)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.ObjProperty"/> class.
		/// </summary>
		/// <param name="property">the propertyInfo object that will reflect on
		/// objects later on to display information with</param>
		/// <param name="nested">if the information required resides in a
		/// property's property, this parameter represents that information</param>
		public ObjProperty(PropertyInfo property, ObjProperty nested)
			:
				this(property, null, nested)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.ObjProperty"/> class.
		/// </summary>
		/// <param name="property">the propertyInfo object that will reflect on
		/// objects later on to display information with</param>
		/// <param name="propertyIndex">an array of index parameters if the
		/// property parameter represents an indexex property</param>
		public ObjProperty(PropertyInfo property, object[] propertyIndex)
			:
				this(property, propertyIndex, null)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.ObjProperty"/> class.
		/// </summary>
		/// <param name="property">the propertyInfo object that will reflect on
		/// objects later on to display information with</param>
		/// <param name="propertyIndex">an array of index parameters if the
		/// property parameter represents an indexex property</param>
		/// <param name="nested">if the information required resides in a
		/// property's property, this parameter represents that information</param>
		public ObjProperty(PropertyInfo property, object[] propertyIndex, ObjProperty nested)
		{
			_property = property;
			_nested = nested;
			_propertyId = propertyIndex;

			if (property != null)
			{
				object[] attr = property.GetCustomAttributes(typeof(EditStrAttribute), true);

				if (attr != null && attr.Length > 0)
				{
					_editType = ((EditStrAttribute)attr[0]).EditType;
				}
			}
		}


		/// <summary>
		/// Sets the value of the provided object's property to the provided value.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="val"></param>
		public void SetValue(object obj, object val)
		{
			if (_nested == null)
				_property.SetValue(obj, val, _propertyId);
			else
				_nested.SetValue(_property.GetValue(obj, _propertyId), val);
		}

		/// <summary>
		/// Gets the value of the provided object's property.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Value(object obj)
		{
			if (_property == null)
				return "<no property>";

			if (obj != null)
			{
				object val = _property.GetValue(obj, _propertyId);
				if (val != null)
					return (_nested != null) ? _nested.Value(val)
											 : val;

				return String.Empty;
			}

			throw new Exception("value is null");
//			throw new ObjPropertyNullValueException();
		}

		/// <summary>
		/// Test for equality between two objects. Test is based on the
		/// property's hashcode despite the fact that hashcodes are not an
		/// absolute equality test.
		/// </summary>
		/// <param name="obj">the other object to test against</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is ObjProperty)
				return (obj.GetHashCode() == GetHashCode());

			return false;
		}

		/// <summary>
		/// Gets the constructor parameter: property.GetHashCode() or 0 if null.
		/// </summary>
		/// <returns>a hash code for the current <see cref="T:System.Object"></see>
		/// </returns>
		public override int GetHashCode()
		{
			return (_property != null) ? _property.GetHashCode()
									  : 0;
		}
	}
}
