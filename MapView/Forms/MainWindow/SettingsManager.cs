using System.Collections.Generic;
using System.IO;

using MapView.SettingServices;

using XCom;


namespace MapView.Forms.MainWindow
{
	public class SettingsManager
	{
		private readonly Dictionary<string, Settings> _dictSettings;


		public SettingsManager()
		{
			_dictSettings = new Dictionary<string, Settings>();
		}


		public void Add(string registryKey, Settings settings)
		{
			_dictSettings.Add(registryKey, settings);
		}

		public void Save()
		{
			SettingsService.Save(_dictSettings);
		}

		public void Load(string file)
		{
			using (var sr = new StreamReader(file))
			{
				var vars = new VarCollection(sr);

				KeyVal line;
				while ((line = vars.ReadLine()) != null)
					Settings.ReadSettings(vars, line, _dictSettings[line.Keyword]);
			}
		}

		public Settings this[string key]
		{
			get { return _dictSettings[key]; }
			set { _dictSettings[key] = value; }
		}
	}
}
