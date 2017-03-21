using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	public partial class InstallWindow
		:
		Form
	{
//		private string pathsFile = "Paths.pth";
//		private string mapFile   = "MapEdit.dat";
//		private string imageFile = "Images.dat";
//		private string miscFile  = "Misc.dat";

//		private string _runPath  = String.Empty;

		private XCom.VarCollection _vars;


		public InstallWindow()
		{
//			_runPath = Directory.GetCurrentDirectory();

			InitializeComponent();

			DialogResult = DialogResult.Cancel;

			_vars = new XCom.VarCollection();

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

			var dirsTftd = new List<string>();
//			dirsTftd.Add(@"C:\tftd");
//			dirsTftd.Add(@"C:\mps\tftd");
//			dirsTftd.Add(@"C:\terror");
//			dirsTftd.Add(@"C:\mps\tftd");
//			dirsTftd.Add(@"d:\xcompro\mapedit\tftd");
//			dirsTftd.Add(@"c:\program files\Terror From the Deep");
//			dirsTftd.Add(@"C:\Documents and Settings\Ben\Desktop\XCFiles\tftd");

			foreach (string path in dirsUfo)
				if (Directory.Exists(path))
				{
					tbUfo.Text = path;
					break;
				}

			foreach (string path in dirsTftd)
				if (Directory.Exists(path))
				{
					tbTftd.Text = path;
					break;
				}
		}


//		public DSShared.PathInfo PathsPath
//		{
//			get { return new DSShared.PathInfo(pathsFile); }
//		}

		private void btnFindUfo_Click(object sender, EventArgs e)
		{
			folderBrowser.Description = "Select UFO directory";

			if (folderBrowser.ShowDialog(this) == DialogResult.OK)
			{
				tbUfo.Text = @folderBrowser.SelectedPath;

				if (folderBrowser.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
					tbUfo.Text = tbUfo.Text.Substring(0, tbUfo.Text.Length - 1);
			}
		}

		private void btnFindTftd_Click(object sender, EventArgs e)
		{
			folderBrowser.Description = "Select TFTD directory";

			if (folderBrowser.ShowDialog(this) == DialogResult.OK)
			{
				tbTftd.Text = @folderBrowser.SelectedPath;

				if (folderBrowser.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
					tbTftd.Text = tbTftd.Text.Substring(0, tbTftd.Text.Length - 1);
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			var info = (PathInfo)SharedSpace.Instance[PathInfo.PathsFile];
			info.CreateDirectory();

			((PathInfo)SharedSpace.Instance[PathInfo.MapEditFile]).CreateDirectory();
			((PathInfo)SharedSpace.Instance[PathInfo.ImagesFile]).CreateDirectory();

			// 'pfe' = path+file+extension
			string pfeMapEdit = ((PathInfo)SharedSpace.Instance[PathInfo.MapEditFile]).FullPath;
			string pfeImages  = ((PathInfo)SharedSpace.Instance[PathInfo.ImagesFile]).FullPath;

			using (var sw = new StreamWriter(new FileStream(info.FullPath, FileMode.Create))) // NOTE: will overwrite file if it exists.
			{
				sw.WriteLine("${ufo}:"  + ((!String.IsNullOrEmpty(tbUfo.Text))  ? tbUfo.Text
																				: String.Empty));
				sw.WriteLine("${tftd}:" + ((!String.IsNullOrEmpty(tbTftd.Text)) ? tbTftd.Text
																				: String.Empty));

				sw.WriteLine("mapdata:" + pfeMapEdit);
				sw.WriteLine("images:"  + pfeImages);

				sw.WriteLine("useBlanks:false");

				if (!String.IsNullOrEmpty(tbUfo.Text))
					sw.WriteLine(@"cursor:${ufo}\UFOGRAPH");
				else if (!String.IsNullOrEmpty(tbTftd.Text))
					sw.WriteLine(@"cursor:${tftd}\UFOGRAPH");

//				sw.Flush();
//				sw.Close();
			}

			#region write misc.dat
/*			StreamWriter sw2 = new StreamWriter(new FileStream(miscFile, FileMode.Create));
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
			sw2.Close();*/
			#endregion


//			_vars["##UFOPath##"]  = txtUFO.Text;
//			_vars["##TFTDPath##"] = txtTFTD.Text;

//			_vars["##imgUFO##"]  = txtUFO.Text  + @"\TERRAIN\";
//			_vars["##imgTFTD##"] = txtTFTD.Text + @"\TERRAIN\";

			_vars["##RunPath##"] = SharedSpace.Instance.GetString(SharedSpace.AppDir);

			// create files
			using (var fs = new FileStream(pfeMapEdit, FileMode.Create))
			{
//				fs.Close();
			}

			using (var fs = new FileStream(pfeImages, FileMode.Create))
			{
//				fs.Close();
			}

			if (!String.IsNullOrEmpty(tbUfo.Text)) // write UFO data
			{
				using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream("MapView._Embedded.MapEditUFO.dat")))
					using (var fs = new FileStream(pfeMapEdit, FileMode.Append))
						using (var sw = new StreamWriter(fs))
						{
							writeFile(sr, sw);
//							sw.Flush();
//							sw.Close();
//							sr.Close();
						}

				using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream("MapView._Embedded.ImagesUFO.dat")))
					using (var fs = new FileStream(pfeImages, FileMode.Append))
						using (var sw = new StreamWriter(fs))
						{
							writeFile(sr, sw);
//							sw.Flush();
//							sw.Close();
//							sr.Close();
						}
			}

			if (!String.IsNullOrEmpty(tbTftd.Text)) // write TFTD data
			{
				using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream("MapView._Embedded.MapEditTFTD.dat")))
					using (var fs = new FileStream(pfeMapEdit, FileMode.Append))
						using (var sw = new StreamWriter(fs))
						{
							writeFile(sr, sw);
							sw.WriteLine();
//							sw.Flush();
//							sw.Close();
//							sr.Close();
						}

				using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
												.GetManifestResourceStream("MapView._Embedded.ImagesTFTD.dat")))
					using (var fs = new FileStream(pfeImages, FileMode.Append))
						using (var sw = new StreamWriter(fs))
						{
							writeFile(sr, sw);
							sw.WriteLine();
//							sw.Flush();
//							sw.Close();
//							sr.Close();
						}
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void writeLine(string line, TextWriter sw)
		{
			if (line.IndexOf('#') > 0)
				foreach (string st in _vars.Variables)
					line = line.Replace(st, _vars[st]);

			sw.WriteLine(@line);
		}

		private void writeFile(TextReader sr, TextWriter sw)
		{
			while (sr.Peek() != -1)
				writeLine(sr.ReadLine(), sw);
		}
	}
}
