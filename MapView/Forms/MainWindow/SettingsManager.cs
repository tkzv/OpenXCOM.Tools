using System.Collections.Generic;
using System.IO;

using MapView.SettingServices;

using XCom;


namespace MapView.Forms.MainWindow
{
	internal sealed class SettingsManager
	{
		private readonly Dictionary<string, Settings> _settingsDictionary = new Dictionary<string, Settings>();


		internal SettingsManager()
		{}


		internal void Add(string registryKey, Settings settings)
		{
			_settingsDictionary.Add(registryKey, settings);
		}

		internal void Save()
		{
			SettingsService.Save(_settingsDictionary);
		}

		internal void Load(string file)
		{
			using (var sr = new StreamReader(file))
			{
				var vars = new Varidia(sr);

				KeyvalPair line;
				while ((line = vars.ReadLine()) != null)
					Settings.ReadSettings(vars, line, _settingsDictionary[line.Keyword]);
			}
		}

		internal Settings this[string key]
		{
			get { return _settingsDictionary[key]; }
			set { _settingsDictionary[key] = value; }
		}
	}
}
