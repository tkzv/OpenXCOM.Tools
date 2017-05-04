using System;
using System.IO;
using System.Windows.Forms;


namespace ConfigConverter
{
	/// <summary>
	/// A converter for turning MapEdit.cfg/dat into a YAML file.
	/// </summary>
	public partial class MainForm
		:
			Form
	{
		#region cTor
		/// <summary>
		/// Instantiates the ConfigConverter.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
		}
		#endregion


		#region EventCalls
		/// <summary>
		/// Closes the converter when the Cancel button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Opens a file browser when the find button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFindInputClick(object sender, EventArgs e)
		{
			var input = new OpenFileDialog();
			input.Filter = "MapView DAT files(*.dat)|*.dat|MapView CFG files(*.cfg)|*.cfg|All files(*.*)|*.*";
			input.FilterIndex = 2;

			if (input.ShowDialog() == DialogResult.OK)
			{
				tbInput.Text = input.FileName;
				string file = Path.GetFileName(tbInput.Text);
				if (file == "MapEdit.dat" || file == "MapEdit.cfg")
				{
					btnConvert.Enabled = true;
				}
				else
					MessageBox.Show(
								"File is not recognized as a MapView config file.",
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Converts the MapEdit configuration file to YAML format.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnConvertClick(object sender, EventArgs e)
		{
			DSShared.DSLogFile.CreateLogFile();


			string dir = Path.GetDirectoryName(tbInput.Text);

			// get input file ->
			string ext = String.Empty;
			if (Path.GetExtension(tbInput.Text) == ".dat")
			{
				ext = ".dat";
			}
			else if (Path.GetExtension(tbInput.Text) == ".cfg")
			{
				ext = ".cfg";
			}
			string pfeMapEdit_in = Path.Combine(dir, "MapEdit" + ext);


			// set output file ->
			string pfeMapConfig_out = Path.Combine(dir, "MapConfig.cfg");


			// readfile + parselines + writefile ->
			using (var sr = new StreamReader(File.OpenRead(pfeMapEdit_in)))
			using (var sw = new StreamWriter(new FileStream(pfeMapConfig_out, FileMode.Create)))
			{
				string deps = String.Empty;

				string line = String.Empty;
				while ((line = sr.ReadLine()) != null)
				{
					line = line.Trim();

					DSShared.DSLogFile.WriteLine("line= " + line);

					if (line.StartsWith("Tileset", StringComparison.Ordinal))
					{
						sw.WriteLine(line.Substring(line.IndexOf(':') + 1)); // write user-defined group-label: eg, "UFO - Terrain", "UFO - Ships", "TFTD - Terrain" etc.

						while ((line = sr.ReadLine()) != null)
						{
							line = line.Trim();

							DSShared.DSLogFile.WriteLine(". line= " + line);

							if (line.Contains("end"))
								break;

							if (!String.IsNullOrEmpty(line))
							{
								if (line.Contains("files"))
								{
									sw.WriteLine("  " + line.Substring(line.IndexOf(':') + 1)); // write user-defined tileset-label: eg, "Cydonia", "Farm", "Xcom Craft", etc.

									while ((line = sr.ReadLine()) != null)
									{
										line = line.Trim();

										DSShared.DSLogFile.WriteLine(". . line= " + line);

										if (line.Contains("$")) // deps and blocks are using aliases
										{
											if (!line.Contains(":$"))
											{
												sw.WriteLine("    deps:"); // write deps header

												deps = line.Substring(line.IndexOf(':') + 1);

												string[] dep = deps.Split(' ');
												for (int i = 0; i != dep.Length; ++i)
													sw.WriteLine("      - " + dep[i]); // write dep-labels
											}
											else
											{
												sw.WriteLine("    blocks:"); // write blocks header

												while (true)
												{
													sw.WriteLine("      - " + line.Substring(0, line.IndexOf(':'))); // write block-labels

													line = sr.ReadLine().Trim();

													while (String.IsNullOrEmpty(line))
														line = sr.ReadLine().Trim();

													DSShared.DSLogFile.WriteLine(". . . line= " + line);

													if (line.Contains("end"))
														break;
												}
											}
										}
										else if (line.Contains(":")) // deps and blocks are literal
										{
											while (true)
											{
												sw.WriteLine("    deps:");

												deps = line.Substring(line.IndexOf(':') + 1);

												string[] dep = deps.Split(' ');
												for (int i = 0; i != dep.Length; ++i)
													sw.WriteLine("      - " + dep[i]); // write dep-labels

												sw.WriteLine("    blocks:"); // write blocks header

												sw.WriteLine("      - " + line.Substring(0, line.IndexOf(':'))); // write block-labels

												line = sr.ReadLine().Trim();

												while (String.IsNullOrEmpty(line))
													line = sr.ReadLine().Trim();

												DSShared.DSLogFile.WriteLine(". . . line= " + line);

												if (line.Contains("end"))
													break;
											}
										}

										if (line.Contains("end"))
											break;
									}
								}
							}
						}
					}
				}
			}

			lblResult.Text = "Finished";

			btnConvert.Enabled =
			btnInput.Enabled   = false;

			btnCancel.Select();
		}
		#endregion
	}
}
