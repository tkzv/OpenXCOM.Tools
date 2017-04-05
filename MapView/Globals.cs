using System;


namespace MapView
{
	public static class Globals
	{
		public const double MinPckImageScale = 0.3;
		public const double MaxPckImageScale = 2.0;

		private static double _pckImageScale = 1.0;
		public static double PckImageScale
		{
			get { return _pckImageScale; }
			set { _pckImageScale = value; }
		}

		private static bool _autoPckImageScale = true;
		public static bool AutoPckImageScale
		{
			get { return _autoPckImageScale; }
			set { _autoPckImageScale = value; }
		}

//		public static readonly string RegistryKey = "MapView";


		private static XCom.PckSpriteCollection _extraTiles;
		public static XCom.PckSpriteCollection ExtraTiles
		{
			get { return _extraTiles; }
		}

		public static void LoadExtras()
		{
			if (_extraTiles == null)
			{
				using (var strPck = System.Reflection.Assembly.GetExecutingAssembly()
									.GetManifestResourceStream("MapView._Embedded.Extra.PCK"))
				using (var strTab = System.Reflection.Assembly.GetExecutingAssembly()
									.GetManifestResourceStream("MapView._Embedded.Extra.TAB"))
				{
					_extraTiles = new XCom.PckSpriteCollection(
														strPck,
														strTab,
														2,
														XCom.Palette.UfoBattle);
				}
			}
		}
	}
}
