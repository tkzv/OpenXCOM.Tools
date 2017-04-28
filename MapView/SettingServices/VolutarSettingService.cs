using System;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;


namespace MapView.SettingServices
{
	/// <summary>
	/// Deals with Volutar's MCD Editor app. Or any app/file really.
	/// </summary>
	internal sealed class VolutarSettingService
	{
		private const string VolutarMcdEditorPath = "VolutarMcdEditorPath";

		private readonly Settings _settings;

		private string _fullpath;
		internal string FullPath
		{
			get
			{
				var setting = _settings.GetSetting(VolutarMcdEditorPath, String.Empty);

				_fullpath = setting.Value as String;
				if (!File.Exists(_fullpath))
				{
					using (var f = new InputBox("Enter the Volutar MCD Editor Path in full"))
					{
						if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							if (File.Exists(f.InputString))
							{
								_fullpath = f.InputString;
								setting.Value = (object)f.InputString;
							}
							else
								MessageBox.Show(
											f,
											"File not found.",
											"Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error,
											MessageBoxDefaultButton.Button1,
											0);
						}
					}
				}
				return _fullpath;
			}
		}


		internal VolutarSettingService(Settings settings)
		{
			_settings = settings;
		}


		public static void LoadSettings(Settings settings)
		{
			settings.AddSetting(
							VolutarMcdEditorPath,
							String.Empty,
							"Path to Volutar MCD Editor" + Environment.NewLine
								+ "note: The path specified can actually be "
								+ "used to start any valid program or to open "
								+ "a specific file with its associated viewer.",
							"McdViewer");
		}
	}
}
