using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public class ImgProfile
	{
		private int _width;
		private int _height;

		private IXCImageFile _imageType;

		private string _desc   = String.Empty;
		private string _pal    = String.Empty;
		private string _single = String.Empty;
		private string _ext    = String.Empty;


		public static List<XCProfile> LoadFile(string inFile)
		{
			using (var sr = new StreamReader(inFile))
			{
				var vars = new VarCollection_Structure(sr);
				var profiles = new List<XCProfile>();

				foreach (string key in vars.KeyValList.Keys)
				{
					var profile = new ImgProfile();

					Dictionary<string, DSShared.KeyVal> info = vars.KeyValList[key].SubHash;

					profile._desc   = key;
					profile._ext    = info["open"].Rest;
					profile._width  = int.Parse(info["width"].Rest);
					profile._height = int.Parse(info["height"].Rest);
					profile._pal    = info["palette"].Rest;

					if (info.ContainsKey("openSingle") && info["openSingle"] != null)
						profile._single = info["openSingle"].Rest + info["open"].Rest;

					foreach (IXCImageFile file in SharedSpace.Instance.GetImageModList())
						if (file.Brief == info["codec"].Rest)
						{
							profile._imageType = file;
							break;
						}

					profiles.Add(new XCProfile(profile));
				}

				return profiles;
			}
		}

		public string OpenSingle
		{
			get { return _single; }
			set { _single = value; }
		}

		public string Palette
		{
			get { return _pal; }
			set { _pal = value; }
		}

		public string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}

		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public IXCImageFile ImgType
		{
			get { return _imageType; }
			set { _imageType = value; }
		}

		public string Extension
		{
			get { return _ext; }
		}

		public string FileString
		{
			get { return _ext; }
			set { _ext = value; }
		}

		public void SaveProfile(string outFile)
		{
			bool append = false;

			if (File.Exists(outFile))
				append = MessageBox.Show(
									"File exists, append new profile? Clicking 'No' will overwrite the file",
									"File exists",
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Warning) == DialogResult.Yes;
				
			var sw = new StreamWriter(outFile, append);
			sw.WriteLine(Description);
			sw.WriteLine("{");

			sw.WriteLine("\tcodec:" + ImgType.Brief);
			sw.WriteLine("\topen:." + _ext.Substring(_ext.LastIndexOf(".", StringComparison.Ordinal) + 1));
			sw.WriteLine("\twidth:" + Width);
			sw.WriteLine("\theight:" + Height);
			sw.WriteLine("\tpalette:" + Palette);

			if (_single != String.Empty)
				sw.WriteLine("\topenSingle:" + _single);

			// here would be the place to put decoder-specific settings

			sw.WriteLine("}" + Environment.NewLine);
			sw.Close();
		}
	}
}
