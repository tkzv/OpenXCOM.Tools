using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class Descriptor // *snap*
		:
			DescriptorBase
	{
		#region Properties
		internal string MapsPath
		{ get; private set; }

		internal string RoutesPath
		{ get; private set; }

		internal string OccultsPath
		{ get; private set; }

		internal string FullPath
		{ get; private set; }

		public List<string> Terrains
		{ get; set; }

		internal Palette Palette
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="tileset"></param>
		/// <param name="pathMaps"></param>
		/// <param name="pathRoutes"></param>
		/// <param name="pathOccults"></param>
		/// <param name="terrains"></param>
		/// <param name="pal"></param>
		public Descriptor(
				string tileset,
				string pathMaps,
				string pathRoutes,
				string pathOccults,
				List<string> terrains,
				Palette pal)
			:
				base(tileset)
		{
			LogFile.WriteLine("Descriptor cTor tileset= " + tileset);
			LogFile.WriteLine("");

			MapsPath    = pathMaps;
			RoutesPath  = pathRoutes;
			OccultsPath = pathOccults;
			Terrains    = terrains;
			Palette     = pal;

			FullPath = Path.Combine(MapsPath, Label + MapFileChild.MapExt);
		}
		#endregion
	}
}
