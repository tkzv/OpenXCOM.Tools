using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public class TileGroupBase
	{
		#region Fields & Properties
		public string Label
		{ get; private set; }

		/// <summary>
		/// 
		/// </summary>
		private readonly Dictionary<string, Descriptor> _descriptors = new Dictionary<string, Descriptor>();
		internal protected Dictionary<string, Descriptor> Descriptors
		{
			get { return _descriptors; }
		}

		/// <summary>
		/// 
		/// </summary>
		private readonly Dictionary<string, Dictionary<string, Descriptor>> _categories = new Dictionary<string, Dictionary<string, Descriptor>>();
		public Dictionary<string, Dictionary<string, Descriptor>> Categories
		{
			get { return _categories; }
		}

		/// <summary>
		/// Used by PathsEditor to add/delete treenodes.
		/// </summary>
		public Descriptor this[string label]
		{
			get { return _descriptors[label]; }
			set
			{
				if (!_descriptors.ContainsKey(label))
					_descriptors[label] = value;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		internal protected TileGroupBase(string label)
		{
			Label = label;
		}
		#endregion
	}
}
