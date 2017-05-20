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
		private readonly Dictionary<string, DescriptorBase> _descriptors = new Dictionary<string, DescriptorBase>();
		internal protected Dictionary<string, DescriptorBase> Descriptors
		{
			get { return _descriptors; }
		}

		/// <summary>
		/// 
		/// </summary>
		private readonly Dictionary<string, Dictionary<string, DescriptorBase>> _categories = new Dictionary<string, Dictionary<string, DescriptorBase>>();
		public Dictionary<string, Dictionary<string, DescriptorBase>> Categories
		{
			get { return _categories; }
		}

		/// <summary>
		/// 
		/// </summary>
		public DescriptorBase this[string label]
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
