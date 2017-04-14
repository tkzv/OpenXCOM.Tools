using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace MapView.SettingServices
{
	internal static class SettingsService
	{
		internal static void Save(IDictionary<string, Settings> settings)
		{
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.Instance[PathInfo.SettingsFile]).FullPath))
			{
				foreach (string key in settings.Keys)
					if (settings.ContainsKey(key))
						settings[key].Save(key, sw);
			}
		}
	}
}
