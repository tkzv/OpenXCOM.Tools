using System;


namespace MapView
{
	internal static class Globals
	{
		internal const double ScaleMinimum = 0.3;
		internal const double ScaleMaximum = 3.0;

		private static double _scale = 1.0;
		/// <summary>
		/// The scale-factor for sprites and clicks in MainView only.
		/// </summary>
		internal static double Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		private static bool _autoScale = true;
		internal static bool AutoScale
		{
			get { return _autoScale; }
			set { _autoScale = value; }
		}

//		public static readonly string RegistryKey = "MapView";


		private static XCom.PckSpriteCollection _extraTiles;
		internal static XCom.PckSpriteCollection ExtraTiles
		{
			get { return _extraTiles; }
		}

		internal static void LoadExtras()
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
		internal static T Clamp<T>(
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
