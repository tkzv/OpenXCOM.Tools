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
		private int _width  = 0; // NOTE: class-vars are default-initialized.
		private int _height = 0;

		private IXCImageFile _imageType;

		private string desc   = String.Empty;
		private string defPal = String.Empty;
		private string single = String.Empty;
		private string ext    = String.Empty;


		public static List<XCProfile> LoadFile(string inFile)
		{
			var sr = new StreamReader(inFile);
			var vars = new VarCollection_Structure(sr);
			sr.Close();

			var profiles = new List<XCProfile>();

			foreach (string st in vars.KeyValList.Keys)
			{
				var profile = new ImgProfile();

				Dictionary<string, DSShared.KeyVal> info = vars.KeyValList[st].SubHash;

				profile.desc	= st;
				profile.ext		= info["open"].Rest;
				profile._width	= int.Parse(info["width"].Rest);
				profile._height	= int.Parse(info["height"].Rest);
				profile.defPal	= info["palette"].Rest;

				if (info.ContainsKey("openSingle") && info["openSingle"] != null)
					profile.single = info["openSingle"].Rest + info["open"].Rest;

				foreach (IXCImageFile ixc in SharedSpace.Instance.GetImageModList())
					if (ixc.ExplorerDescription == info["codec"].Rest)
					{
						profile._imageType = ixc;
						break;
					}

				profiles.Add(new XCProfile(profile));
			}

			return profiles;
		}

		public string OpenSingle
		{
			get { return single; }
			set { single = value; }
		}

		public string Palette
		{
			get { return defPal; }
			set { defPal = value; }
		}

		public string Description
		{
			get { return desc; }
			set { desc = value; }
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
			get { return ext; }
		}

		public string FileString
		{
			get { return ext; }
			set { ext = value; }
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

			sw.WriteLine("\tcodec:" + ImgType.ExplorerDescription);
			sw.WriteLine("\topen:." + ext.Substring(ext.LastIndexOf(".", StringComparison.Ordinal) + 1));
			sw.WriteLine("\twidth:" + Width);
			sw.WriteLine("\theight:" + Height);
			sw.WriteLine("\tpalette:" + Palette);

			if (single != String.Empty)
				sw.WriteLine("\topenSingle:" + single);

			// here would be the place to put decoder-specific settings

			sw.WriteLine("}" + Environment.NewLine);
			sw.Close();
		}
	}
}
