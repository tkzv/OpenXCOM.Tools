/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed partial class SaveProfileForm
		:
			Form
	{
		private ImageProfile _profile;


		public SaveProfileForm()
		{
			InitializeComponent();

			_profile = new ImageProfile();

			DialogResult = DialogResult.Cancel; // TODO: why 2 dialogresultCancel's

			var ri = new DSShared.Windows.RegistryInfo(this);

			string dirCustom = SharedSpace.Instance[SharedSpace.CustomDir].ToString();	// TODO: I don't trust that since changing SharedSpace.
			if (!Directory.Exists(dirCustom))											// it may well need an explicit cast to (PathInfo)
				Directory.CreateDirectory(dirCustom);

			saveFile.InitialDirectory = dirCustom;
			saveFile.FileName = "profile.pvp";

			txtOutDir.Text = saveFile.InitialDirectory + @"\" + saveFile.FileName;

			saveFile.Filter = "Image Profiles|*" + XCProfile.ProfileExt;

			foreach (string key in ((Dictionary<string, Palette>)SharedSpace.Instance[SharedSpace.Palettes]).Keys)
				cbPalette.Items.Add(key);

			if (cbPalette.Items.Count > 0)
				cbPalette.SelectedIndex = 0;

			restring();

			DialogResult = DialogResult.Cancel; // TODO: why 2 dialogresultCancel's
		}


		private void restring()
		{
			txtInfo.Text = String.Empty;
			
			if (ImageType != null)
				txtInfo.Text += "Type: " + ImageType.Brief + Environment.NewLine;
			
			txtInfo.Text += "Width: " + ImgWid + Environment.NewLine + "Height: " + ImgHei;
		}

		public int ImgWid
		{
			get { return _profile.Width; }
			set { _profile.Width = value; restring(); }
		}

		public int ImgHei
		{
			get { return _profile.Height; }
			set { _profile.Height = value; restring(); }
		}

		public IXCImageFile ImageType
		{
			get { return _profile.ImageType; }
			set { _profile.ImageType = value; restring(); }
		}

		public string FileString
		{
//			get { return _profile.FileString; }
			set 
			{
				_profile.FileString = value;

				saveFile.FileName = value.Substring(0,value.LastIndexOf(".", StringComparison.Ordinal)) + XCProfile.ProfileExt;
				txtOutDir.Text = saveFile.InitialDirectory + @"\" + saveFile.FileName;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK; // TODO: why 2 dialogresultOK's

			_profile.Description = txtDesc.Text;
			if (radioSingle.Checked)
			{
				string file = txtOutDir.Text.Substring(txtOutDir.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
				file = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));
				_profile.OpenSingle = file;
			}
			_profile.Palette = cbPalette.SelectedItem.ToString();
			_profile.SaveProfile(txtOutDir.Text);

			((PckViewForm)SharedSpace.Instance["PckView"]).LoadProfile(txtOutDir.Text);

			DialogResult = DialogResult.OK; // TODO: why 2 dialogresultOK's
			Close();
		}

		private void btnFindDir_Click(object sender, EventArgs e)
		{
			if (saveFile.ShowDialog() == DialogResult.OK)
				txtOutDir.Text = saveFile.FileName;
		}

		private void cbPalette_SelectedIndexChanged(object sender, EventArgs e)
		{
			_profile.Palette = cbPalette.SelectedText;
		}
	}
}
*/
