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
//		private string pathsFile	= "Paths.pth";
//		private string mapFile		= "MapEdit.dat";
//		private string imageFile	= "Images.dat";
//		private string miscFile		= "Misc.dat";

//		private string _runPath		= String.Empty;

		private XCom.VarCollection _vars;


		public InstallWindow()
		{
//			_runPath = Directory.GetCurrentDirectory();

			InitializeComponent();

			DialogResult = DialogResult.Cancel;

			_vars = new XCom.VarCollection();

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
					txtUFO.Text = path;
					break;
				}

			foreach (string path in dirsTftd)
				if (Directory.Exists(path))
				{
					txtTFTD.Text = path;
					break;
				}
		}


//		public DSShared.PathInfo PathsPath
//		{
//			get { return new DSShared.PathInfo(pathsFile); }
//		}

		private void btnFindUFO_Click(object sender, EventArgs e)
		{
			folderSelector.Description = "Select UFO directory";

			if (folderSelector.ShowDialog(this) == DialogResult.OK)
			{
				txtUFO.Text = @folderSelector.SelectedPath;

				if (folderSelector.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
					txtUFO.Text = txtUFO.Text.Substring(0, txtUFO.Text.Length - 1);
			}
		}

		private void btnFindTFTD_Click(object sender, EventArgs e)
		{
			folderSelector.Description = "Select TFTD directory";

			if (folderSelector.ShowDialog(this) == DialogResult.OK)
			{
				txtTFTD.Text = folderSelector.SelectedPath;

				if (folderSelector.SelectedPath.EndsWith(@"\", StringComparison.Ordinal))
					txtTFTD.Text = txtTFTD.Text.Substring(0, txtTFTD.Text.Length - 1);
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			var pathsFile = (PathInfo)SharedSpace.Instance["MV_PathsFile"];
			pathsFile.EnsureDirectoryExists();
			((PathInfo)SharedSpace.Instance["MV_MapEditFile"]).EnsureDirectoryExists();
			((PathInfo)SharedSpace.Instance["MV_ImagesFile"]).EnsureDirectoryExists();

			var sw = new StreamWriter(new FileStream(pathsFile.ToString(), FileMode.Create));

			if (!String.IsNullOrEmpty(txtUFO.Text))
				sw.WriteLine("${ufo}:" + txtUFO.Text);

			if (!String.IsNullOrEmpty(txtTFTD.Text))
				sw.WriteLine("${tftd}:" + txtTFTD.Text);

			string mapFile		= SharedSpace.Instance["MV_MapEditFile"].ToString();
			string imageFile	= SharedSpace.Instance["MV_ImagesFile"].ToString();
			string runPath		= SharedSpace.Instance.GetString("AppDir");

			sw.WriteLine("mapdata:" + @mapFile);
			sw.WriteLine("images:" + @imageFile);
//			sw.WriteLine("misc:" + @miscFile);

			sw.WriteLine("useBlanks:false");
			if (!String.IsNullOrEmpty(txtUFO.Text))
				sw.WriteLine(@"cursor:${ufo}\UFOGRAPH");
			else if (!String.IsNullOrEmpty(txtTFTD.Text))
				sw.WriteLine(@"cursor:${tftd}\UFOGRAPH");

			sw.Flush();
			sw.Close();

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

			_vars["##RunPath##"] = runPath;

			// create files
			var fs = new FileStream(@imageFile, FileMode.Create);
			fs.Close();

			fs = new FileStream(@mapFile, FileMode.Create);
			fs.Close();

			// write UFO
			if (!String.IsNullOrEmpty(txtUFO.Text))
			{
				var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.ImagesUFO.dat"));
				fs = new FileStream(@imageFile, FileMode.Append);
				sw = new StreamWriter(fs);

				writeFile(sr, sw);
				sw.Flush();
				sw.Close();
				sr.Close();

				sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.MapEditUFO.dat"));
				fs = new FileStream(@mapFile, FileMode.Append);
				sw = new StreamWriter(fs);

				writeFile(sr, sw);
				sw.Flush();
				sw.Close();
				sr.Close();
			}

			// write TFTD
			if (!String.IsNullOrEmpty(txtTFTD.Text))
			{
				var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.ImagesTFTD.dat"));
				fs = new FileStream(@imageFile, FileMode.Append);
				sw = new StreamWriter(fs);

				writeFile(sr, sw);
				sw.WriteLine();
				sw.Flush();
				sw.Close();
				sr.Close();

				sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MapView._Embedded.MapEditTFTD.dat"));
				fs = new FileStream(@mapFile, FileMode.Append);
				sw = new StreamWriter(fs);

				writeFile(sr, sw);
				sw.WriteLine();
				sw.Flush();
				sw.Close();
				sr.Close();
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
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
