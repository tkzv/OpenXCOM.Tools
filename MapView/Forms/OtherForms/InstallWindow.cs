using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	internal sealed partial class InstallWindow
		:
			Form
	{
//		private string pathsFile = "Paths.cfg";
//		private string mapFile   = "MapEdit.cfg";
//		private string imageFile = "Images.cfg";
//		private string miscFile  = "Misc.cfg";

//		private string _runPath  = String.Empty;

		private Varidia _vars = new Varidia();


		public InstallWindow()
		{
//			_runPath = Directory.GetCurrentDirectory();

			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			var size = new System.Drawing.Size();
			size.Width  =
			size.Height = 0;
			MaximumSize = size; // fu.net
			DialogResult = DialogResult.Cancel;


			// NOTE: Add your own personal XCOM resources-dir here if desired:
			var dirsUfo = new List<string>();
			dirsUfo.Add(@"C:\0xC_kL\data");

//			dirsUfo.Add(@"C:\ufo");
//			dirsUfo.Add(@"C:\ufo enemy unknown");
//			dirsUfo.Add(@"C:\mps\ufo");
//			dirsUfo.Add(@"C:\mps\ufo enemy unknown");
//			dirsUfo.Add(@"d:\xcompro\mapedit\ufo");
//			dirsUfo.Add(@"c:\program files\ufo enemy unknown");
//			dirsUfo.Add(@"C:\Documents and Settings\Ben\Desktop\XCFiles\ufo");

			foreach (string path in dirsUfo)
				if (Directory.Exists(path))
				{
					tbUfo.Text = path;
					break;
				}


//			var dirsTftd = new List<string>();

//			dirsTftd.Add(@"C:\tftd");
//			dirsTftd.Add(@"C:\mps\tftd");
//			dirsTftd.Add(@"C:\terror");
//			dirsTftd.Add(@"C:\mps\tftd");
//			dirsTftd.Add(@"d:\xcompro\mapedit\tftd");
//			dirsTftd.Add(@"c:\program files\Terror From the Deep");
//			dirsTftd.Add(@"C:\Documents and Settings\Ben\Desktop\XCFiles\tftd");

//			foreach (string path in dirsTftd)
//				if (Directory.Exists(path))
//				{
//					tbTftd.Text = path;
//					break;
//				}
		}


//		public DSShared.PathInfo PathsPath
//		{
//			get { return new DSShared.PathInfo(pathsFile); }
//		}

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

			// Or install the dirs/configs anyway and allow the user to set things up in the PathsEditor.
			// Otherwise an exception will be thrown when XCMainWindow cTor tries to instantiate the CursorSprite.
			// what's the CursorSprite used for anyway -> the cursor looks like a windows cursor.
			const string Pck = @"\UFOGRAPH\CURSOR.PCK";
			const string Tab = @"\UFOGRAPH\CURSOR.TAB";
			if (   (File.Exists(tbUfo.Text  + Pck) && File.Exists(tbUfo.Text  + Tab))
				|| (File.Exists(tbTftd.Text + Pck) && File.Exists(tbTftd.Text + Tab)))
			{
				var info = (PathInfo)SharedSpace.Instance[PathInfo.PathsFile];
				info.CreateDirectory();			// create a dir for Paths.Cfg

				var infoTerrains = (PathInfo)SharedSpace.Instance[PathInfo.MapEditFile];
				var infoImages   = (PathInfo)SharedSpace.Instance[PathInfo.ImagesFile];

				infoTerrains.CreateDirectory();	// create a dir for MapEdit.Cfg
				infoImages.CreateDirectory();	// create a dir for Images.Cfg
												// psst. All three dirs are going to be the same: appdir\"settings"

				// 'pfe' = path+file+extension
				string pfeTerrains = infoTerrains.FullPath;
				string pfeImages   = infoImages.FullPath;

				using (var sw = new StreamWriter(new FileStream(info.FullPath, FileMode.Create))) // NOTE: will overwrite Path.cfg if it exists.
				{
					sw.WriteLine("${ufo}:"  + ((!String.IsNullOrEmpty(tbUfo.Text))  ? tbUfo.Text
																					: String.Empty));
					sw.WriteLine("${tftd}:" + ((!String.IsNullOrEmpty(tbTftd.Text)) ? tbTftd.Text
																					: String.Empty));

					sw.WriteLine("mapdata:" + pfeTerrains);
					sw.WriteLine("images:"  + pfeImages);

					sw.WriteLine("useBlanks:false");

					if (!String.IsNullOrEmpty(tbUfo.Text))
					{
						sw.WriteLine(@"cursor:${ufo}\UFOGRAPH");
					}
					else if (!String.IsNullOrEmpty(tbTftd.Text))
					{
						sw.WriteLine(@"cursor:${tftd}\UFOGRAPH");
					}
				}

/*				#region write misc.cfg
				StreamWriter sw2 = new StreamWriter(new FileStream(miscFile, FileMode.Create));
				if (txtTFTD.Text != "")
				{
					sw2.WriteLine(@"${ufoGraph}:${tftd}\UFOGRAPH\");
					sw2.WriteLine(@"${geoGraph}:${tftd}\GEOGRAPH\");
					sw2.WriteLine("cursor:${ufoGraph}cursor");
				}
				else
				{
					sw2.WriteLine(@"${ufoGraph}:${ufo}\UFOGRAPH\");
					sw2.WriteLine(@"${geoGraph}:${ufo}\GEOGRAPH\");
					sw2.WriteLine("cursor:${ufoGraph}cursor");
				}
				sw2.Flush();
				sw2.Close();
				#endregion */


//				_vars["##UFOPath##"]  = txtUFO.Text;
//				_vars["##TFTDPath##"] = txtTFTD.Text;

//				_vars["##imgUFO##"]  = txtUFO.Text  + @"\TERRAIN\";
//				_vars["##imgTFTD##"] = txtTFTD.Text + @"\TERRAIN\";

				_vars["##RunPath##"] = SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory);

				using (var fs = new FileStream(pfeTerrains, FileMode.Create))	// wipe/create MapEdit.Cfg
				{}

				using (var fs = new FileStream(pfeImages, FileMode.Create))		// wipe/create Images.Cfg
				{}

				if (!String.IsNullOrEmpty(tbUfo.Text))
				{
					using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													.GetManifestResourceStream("MapView._Embedded.MapEditUFO.cfg")))
					using (var fs = new FileStream(pfeTerrains, FileMode.Append))
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
					using (var fs = new FileStream(pfeTerrains, FileMode.Append))
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
				Close();
			}
			else
			{
				MessageBox.Show(
							this,
							"A valid UFO or TFTD resource directory must exist (with cursor-sprites in particular).",
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}

		private void Write(TextReader sr, TextWriter sw)
		{
			string line;
			while (sr.Peek() != -1)
			{
				line = sr.ReadLine();
				if (line.IndexOf('#') > 0)
					foreach (string val in _vars.Variables)
						line = line.Replace(val, _vars[val]);

				sw.WriteLine(line);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
