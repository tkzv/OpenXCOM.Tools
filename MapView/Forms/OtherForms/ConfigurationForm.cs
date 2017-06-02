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
		#region Fields
		private PathInfo _pathResources = SharedSpace.Instance[PathInfo.ShareResources] as PathInfo;
		private PathInfo _pathTilesets  = SharedSpace.Instance[PathInfo.ShareTilesets]  as PathInfo;
		#endregion


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
		/// <param name="restart">true if MapView needs to restart to affect
		/// changes (default false)</param>
		internal ConfigurationForm(bool restart = false)
		{
			InitializeComponent();

			if (restart)
			{
				toolTip1.SetToolTip(cbResources, "auto restart! Create paths to"
											+ " stock UFO/TFTD installations");
				toolTip1.SetToolTip(rbTilesets, "auto restart! WARNING : This will"
									+ " replace any custom tileset configuration");
			}

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net

			if (!_pathResources.FileExists())
			{
				cbResources.Enabled = false;
			}
			else
				cbResources.Checked = false;

			if (!_pathTilesets.FileExists())
			{
				cbTilesets.Enabled    =
				rbTilesets.Enabled    =
				rbTilesetsTpl.Enabled = false;
			}
			else
			{
				cbTilesets.Checked = false;
				rbTilesetsTpl.Select();
			}


			// NOTE: Add your own personal XCOM resources-dir here if desired:
			var dirsUfo = new List<string>();
			dirsUfo.Add(@"C:\0xC_kL\data");
//			dirsUfo.Add(@"C:\MapView_test");

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
		private void OnResourcesCheckedChanged(object sender, EventArgs e)
		{
			gbResources.Visible = cbResources.Checked;

			btnOk.Enabled = cbResources.Checked
						 || cbTilesets.Checked;
		}

		private void OnTilesetsCheckedChanged(object sender, EventArgs e)
		{
			rbTilesets.Visible    =
			rbTilesetsTpl.Visible = cbTilesets.Checked;

			btnOk.Enabled = cbTilesets.Checked
						 || cbResources.Checked;
		}

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
			bool bork = false;

			if (cbResources.Checked) // handle XCOM resource path(s) configuration ->
			{
				Ufo  = Ufo.Trim();
				Tftd = Tftd.Trim();

				if (Ufo.EndsWith(@"\", StringComparison.Ordinal))
					Ufo = Ufo.Substring(0, Ufo.Length - 1);

				if (Tftd.EndsWith(@"\", StringComparison.Ordinal))
					Tftd = Tftd.Substring(0, Tftd.Length - 1);

				if (String.IsNullOrEmpty(Ufo) && String.IsNullOrEmpty(Tftd))
				{
					bork = true;
					ShowErrorDialog("Both folders cannot be blank.");
				}
				else if (!String.IsNullOrEmpty(Ufo) && !Directory.Exists(Ufo))
				{
					bork = true;
					ShowErrorDialog("The UFO folder does not exist.");
				}
				else if (!String.IsNullOrEmpty(Tftd) && !Directory.Exists(Tftd))
				{
					bork = true;
					ShowErrorDialog("The TFTD folder does not exist.");
				}
				else // check for a valid XCOM CursorSprite and create MapResources.yml ->
				{
					const string CursorPck = SharedSpace.CursorFilePrefix + SpriteCollection.PckExt;
					const string CursorTab = SharedSpace.CursorFilePrefix + SpriteCollection.TabExt;

					if (   (!File.Exists(Path.Combine(Ufo,  CursorPck)) || !File.Exists(Path.Combine(Ufo,  CursorTab)))
						&& (!File.Exists(Path.Combine(Tftd, CursorPck)) || !File.Exists(Path.Combine(Tftd, CursorTab))))
					{
						bork = true;
						ShowErrorDialog("A valid UFO or TFTD resource directory must exist with"
											+ Environment.NewLine + Environment.NewLine
											+ CursorPck + Environment.NewLine
											+ CursorTab);
					}
					else
					{
						var pathResources = SharedSpace.Instance[PathInfo.ShareResources] as PathInfo;
						pathResources.CreateDirectory();

						string pfeResources = pathResources.FullPath;

						using (var fs = new FileStream(pfeResources, FileMode.Create))
						using (var sw = new StreamWriter(fs))
						{
							object node = new
							{
								ufo  = (!String.IsNullOrEmpty(Ufo)  ? Ufo
																	: PathInfo.NotConfigured),
								tftd = (!String.IsNullOrEmpty(Tftd) ? Tftd
																	: PathInfo.NotConfigured)
							};

							var ser = new Serializer();
							ser.Serialize(sw, node);
						}
					}
				}
			}

			if (!bork)
			{
				if (cbTilesets.Checked) // deal with MapTilesets.yml/.tpl ->
				{
					var pathTilesets = SharedSpace.Instance[PathInfo.ShareTilesets] as PathInfo;
					pathTilesets.CreateDirectory();
	
					string pfeTilesets = rbTilesets.Checked ? pathTilesets.FullPath
															: Path.Combine(pathTilesets.DirectoryPath, PathInfo.ConfigTilesetsTpl);
	
					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.MapTilesets.yml")))
					using (var fs = new FileStream(pfeTilesets, FileMode.Create))
					using (var sw = new StreamWriter(fs))
						while (sr.Peek() != -1)
							sw.WriteLine(sr.ReadLine());
				}

				if (cbResources.Checked || (cbTilesets.Checked && rbTilesets.Checked)) // NOTE: 'cbResources' and 'rbTilesets' have priority over 'rbTilesetsTpl'
				{
					DialogResult = DialogResult.OK;
				}
				else if (cbTilesets.Checked) // && rbTilesetsTpl.Checked
				{
					ShowInfoDialog("Tileset template has been created in the settings subfolder.");
					Close();
				}
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

		/// <summary>
		/// Wrapper for MessageBox.Show()
		/// </summary>
		/// <param name="info">the info-string to show</param>
		private void ShowInfoDialog(string info)
		{
			MessageBox.Show(
						this,
						info,
						"Notice",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion
	}
}
