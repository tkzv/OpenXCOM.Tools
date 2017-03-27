using System;
using System.Windows.Forms;


namespace PckView
{
	public delegate void TryDecodeEventHandler(object sender, TryDecodeEventArgs e);


	public partial class OpenCustomForm
	{
		private string _file;
		private string _dir;

		public event TryDecodeEventHandler TryClick;


		public OpenCustomForm(string dir, string file)
		{
			_dir  = dir;
			_file = file;

			InitializeComponent();

			var ri = new DSShared.Windows.RegistryInfo(this);
			ri.AddProperty("WidVal");
			ri.AddProperty("HeiVal");

			var sharedSpace = XCom.SharedSpace.Instance;

			foreach (XCom.Interfaces.IXCImageFile imageFile in sharedSpace.GetImageModList())
				if (imageFile.FileOptions[XCom.Interfaces.IXCImageFile.Filter.Custom])
					cbTypes.Items.Add(new BmpForm.CbxItem(imageFile, imageFile.Brief));

			if (cbTypes.Items.Count > 0)
				cbTypes.SelectedIndex = 0;
		}


		public string ErrorString
		{
			get { return txtErr.Text; }
			set { txtErr.Text = value; }
		}

		public int WidVal
		{
			get { return scrollWid.Value; }
			set
			{
				scrollWid.Value = value;
				wid_Scroll(null, null);
			}
		}

		public int HeiVal
		{
			get { return scrollHei.Value; }
			set
			{
				scrollHei.Value = value;
				hei_Scroll(null, null);
			}
		}

		private void wid_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			txtWid.Text = scrollWid.Value.ToString();
		}

		private void hei_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			txtHei.Text = scrollHei.Value.ToString();
		}

		private void btnTry_Click(object sender, System.EventArgs e)
		{
			if (TryClick != null)
			{
				try
				{
					TryClick(this, new TryDecodeEventArgs(
													scrollWid.Value,
													scrollHei.Value,
													_dir,
													_file,
													((BmpForm.CbxItem)cbTypes.SelectedItem)._imageFile));
					txtErr.Text = String.Empty;
					Height = 184;
				}
				catch (Exception ex)
				{
					txtErr.Text = ex.Message + System.Environment.NewLine + ex.StackTrace;
					if (Height <= 184)
						Height = 184 + 200;
				}
			}
		}

		private void btnProfile_Click(object sender, System.EventArgs e)
		{
			var spf = new SaveProfileForm();
			spf.ImgHei = scrollHei.Value;
			spf.ImgWid = scrollWid.Value;
			spf.ImgType = ((BmpForm.CbxItem)cbTypes.SelectedItem)._imageFile;
			spf.FileString = _file;

			if (spf.ShowDialog(this) == DialogResult.OK)
				Close();
		}

		private void txtWid_TextChanged(object sender, EventArgs e)
		{
			int val = scrollWid.Value;
			int.TryParse(txtWid.Text, out val); // TODO: Parse() / InvariantCulture.
			if (val >= scrollWid.Minimum && val <= scrollWid.Maximum)
				scrollWid.Value = val;
		}

		private void txtHei_TextChanged(object sender, EventArgs e)
		{
			int val = scrollHei.Value;
			int.TryParse(txtHei.Text, out val); // TODO: Parse() / InvariantCulture.
			if (val >= scrollHei.Minimum && val <= scrollHei.Maximum)
				scrollHei.Value = val;
		}
	}


	public class TryDecodeEventArgs
		:
		EventArgs
	{
		private readonly int _width;
		public int TryWidth
		{
			get { return _width; }
		}

		private readonly int _height;
		public int TryHeight
		{		
			get { return _height; }
		}

		private readonly string _file;
		public string File
		{
			get { return _file; }
		}

		private readonly string _directory;
		public string Directory
		{
			get { return _directory; }
		}

		private readonly XCom.Interfaces.IXCImageFile _imageFile;
		public XCom.Interfaces.IXCImageFile XCFile
		{
			get { return _imageFile; }
		}


		public TryDecodeEventArgs(
				int width,
				int height,
				string directory,
				string file,
				XCom.Interfaces.IXCImageFile imageFile)
		{
			_imageFile = imageFile;
			_file      = file;
			_width     = width;
			_height    = height;
			_directory = directory;
		}
	}
}
