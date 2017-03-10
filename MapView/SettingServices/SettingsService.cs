using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using XCom;


namespace MapView.SettingServices
{
	public static class SettingsService
	{
		public const string FILE_NAME = "MV_SettingsFile";
		 
		public static void Save(Dictionary<string, Settings> settingsHash)
		{
			using (var sw = new StreamWriter(SharedSpace.Instance[FILE_NAME].ToString()))
			{
				foreach (string st in settingsHash.Keys)
					if (settingsHash.ContainsKey(st))
						settingsHash[st].Save(st, sw);

				sw.Flush();
				sw.Close();
			}
		}
	}
}
