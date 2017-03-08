using System.Collections.Generic;
using System.IO;

using MapView.SettingServices;


namespace MapView.Forms.MainWindow
{
	public class SettingsManager
	{
		private readonly Dictionary<string, Settings> _settingsHash;
		private readonly SettingsService _settingsService;


		public SettingsManager()
		{
			_settingsHash = new Dictionary<string, Settings>();
			_settingsService = new SettingsService();
		}


		public void Add(string registryKey, Settings settings)
		{
			_settingsHash.Add(registryKey, settings);
		}

		public void Save()
		{
			_settingsService.Save(_settingsHash);
		}

		public void Load(string file)
		{
			using (var sr = new StreamReader(file))
			{
				ReadMapViewSettings(sr);
			}
		}

		private void ReadMapViewSettings(StreamReader sr)
		{
			var vars = new XCom.VarCollection(sr);
			var l = vars.ReadLine();
			while (l != null)
			{
				Settings.ReadSettings(vars, l, _settingsHash[l.Keyword]);
				l = vars.ReadLine();
			}
			sr.Close();
		}

		public Settings this[string key]
		{
			get { return _settingsHash[key]; }
			set { _settingsHash[key] = value; }
		}
	}
}
