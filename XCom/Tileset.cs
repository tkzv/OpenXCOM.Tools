using System;
using System.Collections.Generic;


namespace XCom
{
	/// <summary>
	/// A Tileset is a tileset. It's comprised of references to loaded MAP/RMP
	/// data as well as required terrain-data loaded from PCK/TAB/MCD files.
	/// </summary>
	internal sealed class Tileset
	{
		#region Fields
		private readonly string _type;
		internal string Type
		{
			get { return _type; }
		}

		private readonly string _group;

		private readonly string _category;
		internal string Category
		{
			get { return _category; }
		}

		private readonly List<string> _terrains;
		internal List<string> Terrains
		{
			get { return _terrains; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="gruop"></param>
		/// <param name="category"></param>
		/// <param name="terrains"></param>
		internal Tileset(
				string type,
				string gruop, // sic.
				string category,
				List<string> terrains)
		{
			_type     = type;
			_group    = gruop;
			_category = category;
			_terrains = terrains;
		}
		#endregion
	}
}
