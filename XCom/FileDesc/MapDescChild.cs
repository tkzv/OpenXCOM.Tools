using System;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class MapDescChild
		:
			MapDescBase
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

		public string[] Terrains
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
		/// <param name="pathRoutes"></param>
		/// <param name="pathOccults"></param>
		/// <param name="terrains"></param>
		/// <param name="pal"></param>
		public MapDescChild(
				string label,
				string pathMaps,
				string pathRoutes,
				string pathOccults,
				string[] terrains,
				Palette pal)
			:
				base(label)
		{
			MapPath    = pathMaps;
			RoutePath  = pathRoutes;
			OccultPath = pathOccults;
			Terrains   = terrains;
			Palette    = pal;
//			IsStatic   = false;
		}
		#endregion
	}
}

//		public bool IsStatic
//		{ get; set; }

//		public int CompareTo(object other)
//		{
//			var desc = other as MapDescChild;
//			return (desc != null) ? String.CompareOrdinal(Label, desc.Label)
//								  : 1;
//		}
