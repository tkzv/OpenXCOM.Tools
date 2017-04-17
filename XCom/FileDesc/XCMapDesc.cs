using System;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class XCMapDesc
		:
			MapDesc
	{
		#region Properties

		internal Palette Palette
		{ get; set; }

		internal string MapPath
		{ get; set; }

		internal string RoutePath
		{ get; set; }

		internal string BlankPath
		{ get; set; }

		internal string FilePath
		{
			get { return MapPath
					   + Label
					   + XCMapFile.MapExt; }
		}

		public string[] Dependencies
		{ get; set; }

		#endregion


		#region cTor

		public XCMapDesc(
				string label,
				string pathMaps,
				string pathBlanks,
				string pathRoutes,
				string[] deps,
				Palette pal)
			:
				base(label)
		{
			Palette      = pal;
			MapPath      = pathMaps;
			RoutePath    = pathRoutes;
			BlankPath    = pathBlanks;
			Dependencies = deps;
//			IsStatic     = false;
		}

		#endregion


/*		public bool IsStatic
		{ get; set; } */

/*		public int CompareTo(object other)
		{
			var desc = other as XCMapDesc;
			return (desc != null) ? String.CompareOrdinal(Label, desc.Label)
								  : 1;
		} */
	}
}
