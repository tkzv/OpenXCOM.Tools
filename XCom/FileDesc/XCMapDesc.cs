using System;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class XCMapDesc
		:
			MapDesc
	{
		#region Properties
		internal string MapPath
		{ get; private set; }

		internal string BlankPath
		{ get; private set; }

		internal string RoutePath
		{ get; private set; }

		internal string FilePath
		{
			get { return MapPath
					   + Label
					   + XCMapFile.MapExt; }
		}

		public string[] Dependencies
		{ get; set; }

		internal Palette Palette
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="pathMaps"></param>
		/// <param name="pathBlanks"></param>
		/// <param name="pathRoutes"></param>
		/// <param name="deps"></param>
		/// <param name="pal"></param>
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
			MapPath      = pathMaps;
			BlankPath    = pathBlanks;
			RoutePath    = pathRoutes;
			Dependencies = deps;
			Palette      = pal;
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
