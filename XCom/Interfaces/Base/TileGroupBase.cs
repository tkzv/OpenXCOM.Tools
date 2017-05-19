using System;
//using System.Collections;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public class TileGroupBase
	{
		#region Fields & Properties
		private readonly string _label;
		public string Label
		{
			get { return _label; }
		}

		private readonly Dictionary<string, DescriptorBase> _descriptionDictionary = new Dictionary<string, DescriptorBase>();
		internal protected Dictionary<string, DescriptorBase> TilesetDescriptors
		{
			get { return _descriptionDictionary; }
		}

		private readonly Dictionary<string, Dictionary<string, DescriptorBase>> _categories = new Dictionary<string, Dictionary<string, DescriptorBase>>();
		public Dictionary<string, Dictionary<string, DescriptorBase>> TilesetCategories
		{
			get { return _categories; }
		}

		public DescriptorBase this[string label]
		{
			get { return _descriptionDictionary[label]; }
			set
			{
				if (!_descriptionDictionary.ContainsKey(label))	// isNecessary=TRUE/FALSE
					_descriptionDictionary.Add(label, value);	// no, none of this inheritance of inheritance of interfaces that aren't bullshit is "necessary".

				_descriptionDictionary[label] = value;
			}
		}

//		public ICollection MapList
//		{
//			get { return _descriptionDictionary.Keys; }
//		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		internal protected TileGroupBase(string label)
		{
			_label = label;
		}
		#endregion
	}
}
