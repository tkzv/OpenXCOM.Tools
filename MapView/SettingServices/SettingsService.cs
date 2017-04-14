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
			LogFile.WriteLine("\nSettingsService.Save");
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.Instance[PathInfo.SettingsFile]).FullPath))
			{
				foreach (string key in settings.Keys)
				{
					LogFile.WriteLine(". key= " + key);
					if (settings.ContainsKey(key))
					{
						LogFile.WriteLine(". . FOUND");
						LogFile.WriteLine(". . write settings[key]= " + settings[key]);
						settings[key].Save(key, sw);
					}
				}
			}
		}
	}
}
