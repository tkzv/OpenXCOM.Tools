using System;
using System.IO;

using DSShared.Windows;


namespace MapView.SettingServices
{
	public class VolutarSettingService
	{
		private const string VOLUTAR_MCD_EDITOR_PATH = "VolutarMcdEditorPath";

		private readonly Settings _settings;
		private string _McdEditorFullPath;


		public VolutarSettingService(Settings settings)
		{
			_settings = settings;
		}


		public string FullPath
		{
			get
			{
				var pathSetting = _settings.GetSetting(VOLUTAR_MCD_EDITOR_PATH, String.Empty);
				_McdEditorFullPath = pathSetting.Value as string ;

				if (string.IsNullOrEmpty(_McdEditorFullPath) || !File.Exists(_McdEditorFullPath))
				{
					var input = new InputBox("Enter the Volutar MCD Editor Path"); // NOTE: 'input' was formerly instantiated with a 'using' directive.
					input.ShowDialog();

					_McdEditorFullPath = input.InputValue;
					if (!string.IsNullOrEmpty(_McdEditorFullPath) && File.Exists(_McdEditorFullPath))
					{
						pathSetting.Value = _McdEditorFullPath;
					}
					else
						return null;
				}
				return _McdEditorFullPath;
			}
		}

		public static void LoadDefaultSettings(Settings settings)
		{
			settings.AddSetting(
							VOLUTAR_MCD_EDITOR_PATH,
							String.Empty,
							"Path to volutar MCD Editor",
							"TileView",
							null,
							false,
							null);
		}
	}
}
