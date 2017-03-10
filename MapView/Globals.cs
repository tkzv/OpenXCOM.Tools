using System;


namespace MapView
{
	public static class Globals
	{
		public static double MinPckImageScale	= 0.3;
		public static double MaxPckImageScale	= 2.0;
		public static double PckImageScale		= 1.0;

		public static bool AutoPckImageScale = true;

		public static readonly string RegistryKey = "MapView";


		private static XCom.PckFile _extraTiles = null;

		public static XCom.PckFile ExtraTiles
		{
			get { return _extraTiles; }
		}

		public static void LoadExtras()
		{
			if (_extraTiles == null)
			{
				using (System.IO.Stream sPck = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.Extra.PCK"))
				using (System.IO.Stream sTab = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.Extra.TAB"))
				{
					_extraTiles = new XCom.PckFile(
												sPck,
												sTab,
												2,
												XCom.Palette.UFOBattle);
//												XCom.Palette.TFTDBattle);
				}
			}
		}
	}
}
