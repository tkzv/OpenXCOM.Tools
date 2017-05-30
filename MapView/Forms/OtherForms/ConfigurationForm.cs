using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using XCom;

using YamlDotNet.Serialization;


namespace MapView
{
	internal sealed partial class ConfigurationForm
		:
			Form
	{
		#region Properties
		private string Ufo
		{
			get { return tbUfo.Text; }
			set { tbUfo.Text = value; }
		}

		private string Tftd
		{
			get { return tbTftd.Text; }
			set { tbTftd.Text = value; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ConfigurationForm()
		{
			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			// NOTE: Add your own personal XCOM resources-dir here if desired:
			var dirsUfo = new List<string>();
//			dirsUfo.Add(@"C:\MapView_test");
			dirsUfo.Add(@"C:\0xC_kL\data");

			foreach (string dir in dirsUfo)
			{
				if (Directory.Exists(dir))
				{
					Ufo = dir;
					break;
				}
			}
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindUfoClick(object sender, EventArgs e)
		{
			using (var f = folderBrowser)
			{
				f.Description = "Select UFO directory";

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					if (f.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
						Ufo = f.SelectedPath.Substring(0, f.SelectedPath.Length - 1);
					else
						Ufo = f.SelectedPath;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindTftdClick(object sender, EventArgs e)
		{
			using (var f = folderBrowser)
			{
				f.Description = "Select TFTD directory";

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					if (f.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
						Tftd = f.SelectedPath.Substring(0, f.SelectedPath.Length - 1);
					else
						Tftd = f.SelectedPath;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAcceptClick(object sender, EventArgs e)
		{
			Ufo  = Ufo.Trim();
			Tftd = Tftd.Trim();

			if (Ufo.EndsWith(@"\", StringComparison.Ordinal))
				Ufo = Ufo.Substring(0, Ufo.Length - 1);

			if (Tftd.EndsWith(@"\", StringComparison.Ordinal))
				Tftd = Tftd.Substring(0, Tftd.Length - 1);

			if (String.IsNullOrEmpty(Ufo) && String.IsNullOrEmpty(Tftd))
			{
				ShowErrorDialog("Both folders cannot be blank.");
			}
			else if (!String.IsNullOrEmpty(Ufo) && !Directory.Exists(Ufo))
			{
				ShowErrorDialog("The UFO folder does not exist.");
			}
			else if (!String.IsNullOrEmpty(Tftd) && !Directory.Exists(Tftd))
			{
				ShowErrorDialog("The TFTD folder does not exist.");
			}
			else if (!cbResources.Checked && !cbTilesets.Checked)
			{
				ShowErrorDialog("Choose at least one option, or Cancel.");
			}
			else
			{
				// Or install the dirs/configs anyway and allow the user to set things up in the PathsEditor.
				// Otherwise an exception will be thrown when XCMainWindow cTor tries to instantiate the CuboidSprite.
				// what's the CuboidSprite used for anyway -> the cursor looks like a windows cursor.
				// note: It's used to indicate the dragStart and dragEnd tiles.

				const string CursorPck = SharedSpace.CursorFilePrefix + SpriteCollection.PckExt;
				const string CursorTab = SharedSpace.CursorFilePrefix + SpriteCollection.TabExt;

				if (   (File.Exists(Path.Combine(Ufo,  CursorPck)) && File.Exists(Path.Combine(Ufo,  CursorTab)))
					|| (File.Exists(Path.Combine(Tftd, CursorPck)) && File.Exists(Path.Combine(Tftd, CursorTab))))
				{
					var pathConfig = SharedSpace.Instance[PathInfo.MapTilesets] as PathInfo;
					pathConfig.CreateDirectory(); // create a dir for MapConfig.yml and MapDirectory.yml


					if (cbResources.Checked)
					{
						string pfeMapDirectory = Path.Combine(pathConfig.DirectoryPath, PathInfo.ConfigResources);

						using (var fs = new FileStream(pfeMapDirectory, FileMode.Create)) // wipe/create MapDirectory.yml
						using (var sw = new StreamWriter(fs))
						{
							object node = new
							{
								ufo  = (!String.IsNullOrEmpty(Ufo)  ? Ufo
																	: "placeholder"),
								tftd = (!String.IsNullOrEmpty(Tftd) ? Tftd
																	: "placeholder")
							};

							var ser = new Serializer();
							ser.Serialize(sw, node);
						}
					}

					if (cbTilesets.Checked)
					{
						string pfeMapConfig = pathConfig.FullPath;

						if (!String.IsNullOrEmpty(Ufo))
						{
							using (var fs = new FileStream(pfeMapConfig, FileMode.Create)) // wipe/create MapConfig.yml
							{}

							using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
															.GetManifestResourceStream("MapView._Embedded.MapConfig.yml")))
							using (var fs = new FileStream(pfeMapConfig, FileMode.Append))
							using (var sw = new StreamWriter(fs))
								while (sr.Peek() != -1) // transfer embedded textfile to MapConfig.yml
									sw.WriteLine(sr.ReadLine());
						}

						// TODO: TFTD dir
					}

					DialogResult = DialogResult.OK;
				}
				else
					ShowErrorDialog("A valid UFO or TFTD resource directory must exist with"
									+ Environment.NewLine + Environment.NewLine
									+ CursorPck + Environment.NewLine
									+ CursorTab);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Wrapper for MessageBox.Show()
		/// </summary>
		/// <param name="error">the error-string to show</param>
		private void ShowErrorDialog(string error)
		{
			MessageBox.Show(
						this,
						error,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion
	}
}
