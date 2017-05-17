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

		private readonly Dictionary<string, MapDescBase> _mapDescDictionary;
		internal protected Dictionary<string, MapDescBase> MapDescDictionary
		{
			get { return _mapDescDictionary; }
		}

		private readonly Dictionary<string, Dictionary<string, MapDescBase>> _categories;
		public Dictionary<string, Dictionary<string, MapDescBase>> Categories
		{
			get { return _categories; }
		}

		public MapDescBase this[string label]
		{
			get { return _mapDescDictionary[label]; }
			set
			{
				if (!_mapDescDictionary.ContainsKey(label))	// isNecessary=TRUE/FALSE
					_mapDescDictionary.Add(label, value);	// no, none of this inheritance of inheritance of interfaces that aren't bullshit is "necessary".

				_mapDescDictionary[label] = value;
			}
		}

//		public ICollection MapList
//		{
//			get { return _mapDescs.Keys; }
//		}
		#endregion


		#region cTor
		internal protected TileGroupBase(string label)
		{
			_label = label;
			_mapDescDictionary = new Dictionary<string, MapDescBase>();
			_categories = new Dictionary<string, Dictionary<string, MapDescBase>>();
		}
		#endregion
	}
}
