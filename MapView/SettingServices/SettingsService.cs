using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace MapView.SettingServices
{
	internal static class SettingsService
	{
		public static void Save(Dictionary<string, Settings> dictSettings)
		{
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.Instance[PathInfo.SettingsFile]).FullPath))
			{
				foreach (string key in dictSettings.Keys)
					if (dictSettings.ContainsKey(key))
						dictSettings[key].Save(key, sw);
			}
		}
	}
}
