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
		/// <summary>
		/// cTor.
		/// </summary>
		internal ConfigurationForm()
		{
			InitializeComponent();
			DialogResult = DialogResult.Cancel;

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net


			// NOTE: Add your own personal XCOM resources-dir here if desired:
			var dirsUfo = new List<string>();
			dirsUfo.Add(@"C:\MapView_test");
//			dirsUfo.Add(@"C:\0xC_kL\data");

			foreach (string dir in dirsUfo)
				if (Directory.Exists(dir))
				{
					tbUfo.Text = dir;
					break;
				}
		}

		private void btnFindUfo_Click(object sender, EventArgs e)
		{
			using (var f = folderBrowser)
			{
				f.Description = "Select UFO directory";

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					if (f.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
						tbUfo.Text = f.SelectedPath.Substring(0, f.SelectedPath.Length - 1);
					else
						tbUfo.Text = f.SelectedPath;
				}
			}
		}

		private void btnFindTftd_Click(object sender, EventArgs e)
		{
			using (var f = folderBrowser)
			{
				f.Description = "Select TFTD directory";

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					if (f.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
						tbTftd.Text = f.SelectedPath.Substring(0, f.SelectedPath.Length - 1);
					else
						tbTftd.Text = f.SelectedPath;
				}
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (tbUfo.Text.EndsWith(@"\", StringComparison.Ordinal))
				tbUfo.Text = tbUfo.Text.Substring(0, tbUfo.Text.Length - 1);

			if (tbTftd.Text.EndsWith(@"\", StringComparison.Ordinal))
				tbTftd.Text = tbTftd.Text.Substring(0, tbTftd.Text.Length - 1);

			// TODO: check that either directory given is valid if not blank.

			// Or install the dirs/configs anyway and allow the user to set things up in the PathsEditor.
			// Otherwise an exception will be thrown when XCMainWindow cTor tries to instantiate the CuboidSprite.
			// what's the CuboidSprite used for anyway -> the cursor looks like a windows cursor.
			// note: It's used to indicate the dragStart and dragEnd tiles.

			const string CursorPck = SharedSpace.CursorFilePrefix + PckSpriteCollection.PckExt;
			const string CursorTab = SharedSpace.CursorFilePrefix + PckSpriteCollection.TabExt;

			if (   (File.Exists(tbUfo.Text  + CursorPck) && File.Exists(tbUfo.Text  + CursorTab))
				|| (File.Exists(tbTftd.Text + CursorPck) && File.Exists(tbTftd.Text + CursorTab)))
			{
				var pathConfig = SharedSpace.Instance[PathInfo.MapConfig] as PathInfo;
				pathConfig.CreateDirectory(); // create a dir for MapConfig.yml and MapDirectory.yml


				string pfeMapDirectory = Path.Combine(pathConfig.Path, PathInfo.YamlResources);

				using (var fs = new FileStream(pfeMapDirectory, FileMode.Create)) // wipe/create MapDirectory.yml
				using (var sw = new StreamWriter(fs))
				{
					object node = new
					{
						ufo  = (!String.IsNullOrEmpty(tbUfo.Text)  ? tbUfo.Text
																   : "placeholder"),
						tftd = (!String.IsNullOrEmpty(tbTftd.Text) ? tbTftd.Text
																   : "placeholder")
					};

					var ser = new Serializer();
					ser.Serialize(sw, node);
				}


				string pfeMapConfig = pathConfig.FullPath;

				if (!String.IsNullOrEmpty(tbUfo.Text))
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

				DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				MessageBox.Show(
							this,
							"A valid UFO or TFTD resource directory must exist with"
								+ Environment.NewLine + Environment.NewLine
								+ CursorPck + Environment.NewLine
								+ CursorTab,
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}

//				var pathPaths = (PathInfo)SharedSpace.Instance[PathInfo.PathsFile];
//				pathPaths.CreateDirectory();	// create a dir for Paths.Cfg
//
//				var pathMapEdit = (PathInfo)SharedSpace.Instance[PathInfo.MapEditFile];
//				pathMapEdit.CreateDirectory();	// create a dir for MapEdit.Cfg
//
//				var pathImages  = (PathInfo)SharedSpace.Instance[PathInfo.ImagesFile];
//				pathImages.CreateDirectory();	// create a dir for Images.Cfg
//												// psst. All three dirs are going to be the same: appdir\"settings"

//				// 'pfe' = path+file+extension
//				string pfeMapEdit = pathMapEdit.FullPath;
//				string pfeImages  = pathImages.FullPath;

//				using (var sw = new StreamWriter(new FileStream(pathPaths.FullPath, FileMode.Create))) // NOTE: will overwrite Path.cfg if it exists.
//				{
//					sw.WriteLine("${ufo}:"  + ((!String.IsNullOrEmpty(tbUfo.Text))  ? tbUfo.Text
//																					: String.Empty));
//					sw.WriteLine("${tftd}:" + ((!String.IsNullOrEmpty(tbTftd.Text)) ? tbTftd.Text
//																					: String.Empty));
//
//					sw.WriteLine("mapdata:" + pfeMapEdit);
//					sw.WriteLine("images:"  + pfeImages);
//
//					sw.WriteLine("useBlanks:false");
//
//					if (!String.IsNullOrEmpty(tbUfo.Text))
//					{
//						sw.WriteLine(@"cursor:${ufo}\UFOGRAPH");
//					}
//					else if (!String.IsNullOrEmpty(tbTftd.Text))
//					{
//						sw.WriteLine(@"cursor:${tftd}\UFOGRAPH");
//					}
//				}

//				#region write misc.cfg
//				StreamWriter sw2 = new StreamWriter(new FileStream(miscFile, FileMode.Create));
//				if (txtTFTD.Text != "")
//				{
//					sw2.WriteLine(@"${ufoGraph}:${tftd}\UFOGRAPH\");
//					sw2.WriteLine(@"${geoGraph}:${tftd}\GEOGRAPH\");
//					sw2.WriteLine("cursor:${ufoGraph}cursor");
//				}
//				else
//				{
//					sw2.WriteLine(@"${ufoGraph}:${ufo}\UFOGRAPH\");
//					sw2.WriteLine(@"${geoGraph}:${ufo}\GEOGRAPH\");
//					sw2.WriteLine("cursor:${ufoGraph}cursor");
//				}
//				sw2.Flush();
//				sw2.Close();
//				#endregion


//				_vars["##UFOPath##"]  = txtUFO.Text;
//				_vars["##TFTDPath##"] = txtTFTD.Text;

//				_vars["##imgUFO##"]  = txtUFO.Text  + @"\TERRAIN\";
//				_vars["##imgTFTD##"] = txtTFTD.Text + @"\TERRAIN\";

/*				_vars["##RunPath##"] = SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory);

				using (var fs = new FileStream(pfeMapEdit, FileMode.Create))	// wipe/create MapEdit.Cfg
				{}

				using (var fs = new FileStream(pfeImages, FileMode.Create))		// wipe/create Images.Cfg
				{}

				if (!String.IsNullOrEmpty(tbUfo.Text))
				{
					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.MapEditUFO.cfg")))
					using (var fs = new FileStream(pfeMapEdit, FileMode.Append))
					using (var sw = new StreamWriter(fs))
						Write(sr, sw); // write UFO data to MapEdit.Cfg

					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.ImagesUFO.cfg")))
					using (var fs = new FileStream(pfeImages, FileMode.Append))
					using (var sw = new StreamWriter(fs))
						Write(sr, sw); // write UFO data to Images.Cfg
				}

				if (!String.IsNullOrEmpty(tbTftd.Text)) // write TFTD data
				{
					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.MapEditTFTD.cfg")))
					using (var fs = new FileStream(pfeMapEdit, FileMode.Append))
					using (var sw = new StreamWriter(fs))
					{
						Write(sr, sw); // write TFTD data to MapEdit.Cfg
						sw.WriteLine();
					}

					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.ImagesTFTD.cfg")))
					using (var fs = new FileStream(pfeImages, FileMode.Append))
					using (var sw = new StreamWriter(fs))
					{
						Write(sr, sw); // write TFTD data to Images.Cfg
						sw.WriteLine();
					}
				}

				DialogResult = DialogResult.OK;
				Close(); */
