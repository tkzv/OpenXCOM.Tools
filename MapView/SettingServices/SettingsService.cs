using System;
using System.Collections.Generic;
using System.IO;

using XCom;


namespace MapView.SettingServices
{
	public static class SettingsService
	{
		public const string MV_SETTINGS_FILE = "MV_SettingsFile";


		public static void Save(Dictionary<string, Settings> settingsHash)
		{
			using (var sw = new StreamWriter(SharedSpace.Instance[MV_SETTINGS_FILE].ToString()))
			{
				foreach (string st in settingsHash.Keys)
					if (settingsHash.ContainsKey(st))
						settingsHash[st].Save(st, sw);

//				sw.Flush();
//				sw.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
		}
	}
}
