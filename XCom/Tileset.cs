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
		internal string Type
		{ get; private set; }

		internal string Group
		{ get; set; }

		internal string Category
		{ get; set; }

		internal List<string> Terrains
		{ get; set; }
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
			Type     = type;
			Group    = gruop;
			Category = category;
			Terrains = terrains;
		}
		#endregion
	}
}
