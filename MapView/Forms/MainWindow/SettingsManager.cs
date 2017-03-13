using System.Collections.Generic;
using System.IO;

using MapView.SettingServices;


namespace MapView.Forms.MainWindow
{
	public class SettingsManager
	{
		private readonly Dictionary<string, Settings> _settingsHash;


		public SettingsManager()
		{
			_settingsHash = new Dictionary<string, Settings>();
		}


		public void Add(string registryKey, Settings settings)
		{
			_settingsHash.Add(registryKey, settings);
		}

		public void Save()
		{
			SettingsService.Save(_settingsHash);
		}

		public void Load(string file)
		{
			using (var sr = new StreamReader(file))
			{
				var vars = new XCom.VarCollection(sr);

				XCom.KeyVal line = null;
				while ((line = vars.ReadLine()) != null)
				{
					Settings.ReadSettings(vars, line, _settingsHash[line.Keyword]);
				}
//				sr.Close(); // NOTE: the 'using' block closes the stream.
			}
		}

		public Settings this[string key]
		{
			get { return _settingsHash[key]; }
			set { _settingsHash[key] = value; }
		}
	}
}
