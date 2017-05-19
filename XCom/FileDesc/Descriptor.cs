using System;
using System.Collections.Generic;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class Descriptor // *snap*
		:
			DescriptorBase
	{
		#region Properties
		internal string MapPath
		{ get; private set; }

		internal string RoutePath
		{ get; private set; }

		internal string FilePath
		{
			get { return MapPath
					   + Label
					   + MapFileChild.MapExt; }
		}

		internal string OccultPath
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
			LogFile.WriteLine("");
			LogFile.WriteLine("Descriptor cTor tileset= " + tileset);

			MapPath    = pathMaps;
			RoutePath  = pathRoutes;
			OccultPath = pathOccults;
			Terrains   = terrains;
			Palette    = pal;
		}
		#endregion
	}
}
