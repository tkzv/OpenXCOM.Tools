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

		private readonly Dictionary<string, IMapDesc> _mapDescs;
		internal protected Dictionary<string, IMapDesc> MapDescs
		{
			get { return _mapDescs; }
		}

		private readonly Dictionary<string, Dictionary<string, IMapDesc>> _subsets;
		public Dictionary<string, Dictionary<string, IMapDesc>> Subsets
		{
			get { return _subsets; }
		}


		internal protected TilesetBase(string name)
		{
			_name = name;
			_mapDescs = new Dictionary<string, IMapDesc>();
			_subsets = new Dictionary<string, Dictionary<string, IMapDesc>>();
		}


		public IMapDesc this[string name]
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
