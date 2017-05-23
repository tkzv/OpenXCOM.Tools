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
		/// Descriptors is a dictionary of descriptor-labels (.MAP/.RMP
		/// filenames w/out extension) mapped to Descriptors.
		/// </summary>
		private readonly Dictionary<string, Descriptor> _descriptors = new Dictionary<string, Descriptor>();
		internal protected Dictionary<string, Descriptor> Descriptors
		{
			get { return _descriptors; }
		}

		/// <summary>
		/// Categories is a dictionary of category-labels mapped to a
		/// subdictionary of descriptor-labels (.MAP/.RMP filenames w/out
		/// extension) mapped to the Descriptors themselves.
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
				if (!_descriptors.ContainsKey(label)) // TODO: this needs to work through Categories *only*
					_descriptors[label] = value;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="labelGroup"></param>
		internal protected TileGroupBase(string labelGroup)
		{
			Label = labelGroup;
		}
		#endregion
	}
}
