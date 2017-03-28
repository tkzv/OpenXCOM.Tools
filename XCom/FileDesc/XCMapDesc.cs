using System;

using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapDesc
		:
		IMapDesc
	{
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
			IsStatic     = false;
		}

		public Palette Palette
		{ get; protected set; }

		public string MapPath
		{ get; protected set; }

		public string RoutePath
		{ get; protected set; }

		public string BlankPath
		{ get; protected set; }

		public string FilePath
		{
			get { return MapPath
					   + Label
					   + XCMapFile.MapExt; }
		}

		public string[] Dependencies
		{ get; set; }

		public bool IsStatic
		{ get; set; }

		public int CompareTo(object other)
		{
			var desc = other as XCMapDesc;
			return (desc != null) ? String.CompareOrdinal(Label, desc.Label)
								  : 1;
		}
	}
}
