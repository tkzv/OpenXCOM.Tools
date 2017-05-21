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
		{ get; private set; }

		internal string Category
		{ get; private set; }

		internal List<string> Terrains
		{ get; private set; }

		internal string BasePath
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="grup"></param>
		/// <param name="category"></param>
		/// <param name="terrains"></param>
		/// <param name="basepath"></param>
		internal Tileset(
				string type,
				string grup, // sic.
				string category,
				List<string> terrains,
				string basepath)
		{
			Type     = type;
			Group    = grup;
			Category = category;
			Terrains = terrains;
			BasePath = basepath;
		}
		#endregion
	}
}
