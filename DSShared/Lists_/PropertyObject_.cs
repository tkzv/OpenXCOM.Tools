/*
using System;
using System.Reflection;
//using DSShared.Exceptions;


namespace DSShared.Lists
{
	/// <summary>
	/// Class that a CustomList uses to figure out the value of a particular
	/// row and column.
	/// </summary>
	public sealed class PropertyObject
	{
		private readonly PropertyInfo _info;
		private object[] _id;

		private PropertyObject _property;

		private EditStrType _editType = EditStrType.None;
		/// <summary>
		/// Gets or sets the type of the edit.
		/// </summary>
		/// <value>the type of the edit</value>
		internal EditStrType EditType
		{
			get { return _editType; }
			set { _editType = value; }
		}

		/// <summary>
		/// Gets or sets the key function. This is called when a key is pressed
		/// on a selected row.
		/// </summary>
		/// <value>the key function</value>
		internal EditStrDelegate KeyFunction
		{ get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.PropertyObject"/> class.
		/// </summary>
		/// <param name="info">the PropertyInfo object that will reflect on
		/// objects later on to display information with</param>
		/// <param name="id">an array of index parameters if the property
		/// parameter represents an indexed property</param>
		/// <param name="property">if the information required resides in a
		/// property's property, this parameter represents that information</param>
		private PropertyObject(PropertyInfo info, object[] id, PropertyObject property)
		{
			_info     = info;
			_id       = id;
			_property = property;

			if (info != null)
			{
				object[] attr = info.GetCustomAttributes(typeof(EditStrAttribute), true);

				if (attr != null && attr.Length > 0)
					_editType = ((EditStrAttribute)attr[0]).EditType;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.PropertyObject"/> class.
		/// </summary>
		/// <param name="info">the PropertyInfo object that will reflect on
		/// objects later on to display information with</param>
		internal PropertyObject(PropertyInfo info)
			:
				this(info, null, null)
		{}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:DSShared.Lists.PropertyObject"/> class.
//		/// </summary>
//		/// <param name="info">the PropertyInfo object that will reflect on
//		/// objects later on to display information with</param>
//		/// <param name="property">if the information required resides in a
//		/// property's property, this parameter represents that information</param>
//		public PropertyObject(PropertyInfo info, PropertyObject property)
//			:
//				this(info, null, property)
//		{}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:DSShared.Lists.PropertyObject"/> class.
//		/// </summary>
//		/// <param name="info">the PropertyInfo object that will reflect on
//		/// objects later on to display information with</param>
//		/// <param name="id">an array of index parameters if the property
//		/// parameter represents an indexex property</param>
//		public PropertyObject(PropertyInfo info, object[] id)
//			:
//				this(info, id, null)
//		{}


		/// <summary>
		/// Sets the value of the provided object's property to the provided value.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="val"></param>
		internal void SetValue(object obj, object val)
		{
			if (_property != null)
				_property.SetValue(_info.GetValue(obj, _id), val);
			else
				_info.SetValue(obj, val, _id);
		}

		/// <summary>
		/// Gets the value of the provided object's property.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		internal object GetValue(object obj)
		{
			if (_info == null)
				return "<no property>";

			if (obj != null)
			{
				object val = _info.GetValue(obj, _id);
				if (val != null)
					return (_property != null) ? _property.GetValue(val)
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
			if (obj is PropertyObject)
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
			return (_info != null) ? _info.GetHashCode()
								   : 0;
		}
	}
}
*/
