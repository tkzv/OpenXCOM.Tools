using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	/// <summary>
	/// Descriptors describe a tileset: a Map, its route-nodes, and terrain. It
	/// also holds the path to its files' parent directory.
	/// A descriptor is accessed *only* through a Group and Category, and is
	/// identified by its tileset-label. This allows multiple tilesets (ie. with
	/// the same label) to be configured differently according to Category and
	/// Group.
	/// </summary>
	public sealed class Descriptor // *snap*
	{
		#region Properties
		public string Label
		{ get; private set; }

		internal string BasePath
		{ get; private set; }

		public List<string> Terrains
		{ get; set; }

		internal Palette Pal
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="tileset"></param>
		/// <param name="terrains"></param>
		/// <param name="basepath"></param>
		/// <param name="palette"></param>
		public Descriptor(
				string tileset,
				List<string> terrains,
				string basepath,
				Palette palette)
		{
			LogFile.WriteLine("Descriptor cTor tileset= " + tileset);
			LogFile.WriteLine("");

			Label    = tileset;
			Terrains = terrains;
			BasePath = basepath;
			Pal      = palette;
		}
		#endregion


		#region Methods
		public override string ToString()
		{
			return Label;
		}
		#endregion
	}
}
