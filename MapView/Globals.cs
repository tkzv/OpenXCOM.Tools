using System;


namespace MapView
{
	internal static class Globals
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

		/// <summary>
		/// Clamps a value between min and max inclusively. Note that no check
		/// is done to ensure that min is less than max.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns>min if val is less than min; max if value is greater than
		/// max; else the value itself</returns>
		public static T Clamp<T>(
				this T val,
				T min,
				T max) where T
			:
				IComparable<T>
		{
			if (val.CompareTo(min) < 0)
				return min;

			if (val.CompareTo(max) > 0)
				return max;

			return val;
		}
	}
}
