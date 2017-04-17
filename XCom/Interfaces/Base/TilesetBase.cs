using System;
//using System.Collections;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public class TilesetBase
	{
		private readonly string _name;
		public string Name
		{
			get { return _name; }
		}

		private readonly Dictionary<string, MapDesc> _mapDescs;
		internal protected Dictionary<string, MapDesc> MapDescs
		{
			get { return _mapDescs; }
		}

		private readonly Dictionary<string, Dictionary<string, MapDesc>> _subsets;
		public Dictionary<string, Dictionary<string, MapDesc>> Subsets
		{
			get { return _subsets; }
		}


		internal protected TilesetBase(string name)
		{
			_name = name;
			_mapDescs = new Dictionary<string, MapDesc>();
			_subsets = new Dictionary<string, Dictionary<string, MapDesc>>();
		}


		public MapDesc this[string name]
		{
			get { return _mapDescs[name]; }
			set
			{
				if (!_mapDescs.ContainsKey(name)) // isNecessary(?)
					_mapDescs.Add(name, value);

				_mapDescs[name] = value;
			}
		}

/*		public ICollection MapList
		{
			get { return _mapDescs.Keys; }
		} */
	}
}
