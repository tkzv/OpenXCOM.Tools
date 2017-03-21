using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace MapView.SettingServices
{
	public static class SettingsService
	{
		public const string SettingsFile = "MV_SettingsFile";


		public static void Save(Dictionary<string, Settings> dictSettings)
		{
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.Instance[SettingsFile]).FullPath))
			{
				foreach (string st in dictSettings.Keys)
					if (dictSettings.ContainsKey(st))
						dictSettings[st].Save(st, sw);

//				sw.Flush();
//				sw.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
		}
	}
}
