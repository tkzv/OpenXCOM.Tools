/*
using System;
using System.Windows.Forms;

using XCom.Interfaces;


namespace XCom.GameFiles.Images.XCFiles
{
	public class xcPck
		:
		IXCImageFile
	{
		private const string TabExt = ".tab";

		private Panel _savePanel;
		private RadioButton _radio2;
		private RadioButton _radio4;


		public xcPck()
			:
			base(32, 40)
		{
			author	= "Ben Ratzlaff";
			ext		= ".pck";
			desc	= "Standard pck file codec";
			expDesc	= "Pck File";
		}


		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				Palette pal)
		{
			string tabBase = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));
			var tabFilePath = directory + @"\" + tabBase + TabExt;

			System.IO.Stream tabStream = null;
			if (System.IO.File.Exists(tabFilePath))
				tabStream = System.IO.File.OpenRead(tabFilePath);

			using (tabStream)
			using (var pckStream = System.IO.File.OpenRead(directory + @"\" + file))
			{
				try
				{
					return new PckSpriteCollection(
									pckStream,
									tabStream,
									2,
									pal,
									width,
									height);
				}
				catch (Exception)
				{
					return new PckSpriteCollection(
									pckStream,
									tabStream,
									4,
									pal,
									width,
									height);
				}
			}
		}

		private Panel SavingOptions
		{
			get
			{
				if (_savePanel == null)
				{
					_savePanel = new Panel();

					var gb = new GroupBox();
					gb.Text = "Bpp Options";

					var top = new Panel();
					top.Dock = DockStyle.Top;
					top.Height = 50;

					_radio2 = new RadioButton();
					_radio2.Text = "2";
					_radio2.Dock = DockStyle.Left;
					_radio2.Height = 50;
					_radio2.Width = 40;
					_radio2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
					_radio2.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
					_radio2.Checked = true;
					_radio2.CheckedChanged += checkChange;

					var label = new Label();
					label.Text = "UFO/TFTD Terrain\nUFO Units";
					label.Dock = DockStyle.Fill;

					top.Controls.AddRange(new Control[]{ label, _radio2 });

					var mid = new Panel();
					mid.Dock = DockStyle.Fill;

					_radio4 = new RadioButton();
					_radio4.Text = "4";
					_radio4.Dock = DockStyle.Left;
					_radio4.Width = _radio2.Width;
					_radio4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
					_radio4.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
					_radio4.CheckedChanged += checkChange;

					label = new Label();
					label.Text = "TFTD Units";
					label.Dock = DockStyle.Fill;

					mid.Controls.AddRange(new Control[]{ label, _radio4 });

					gb.Controls.AddRange(new Control[]{ mid, top });
					gb.Dock = DockStyle.Left;

					var left = new Panel();
					left.Dock = DockStyle.Top;
					left.Height = 100;

					left.Controls.Add(gb);

					label = new Label();
					label.Text = "If you are unsure about the correct bpp option,"
						+ " open up the original .pck file and see what the number"
						+ " is in the lower right of the main screen.";
					label.Dock = DockStyle.Fill;

					_savePanel.Controls.AddRange(new Control[]{ label, left });
				}

				return _savePanel;
			}

//			set { _savePanel = value; }
		}

		private void checkChange(object sender, EventArgs e)
		{
			_radio2.Checked = (sender == _radio2);
			_radio4.Checked = (sender == _radio4);
		}

		/// <summary>
		/// Method to save a collection in its original format.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="file"></param>
		/// <param name="images"></param>
		public override void SaveCollection(
				string directory,
				string file,
				XCImageCollection images)
		{
			var ib = new DSShared.Windows.InputBox("Enter Pck Options", SavingOptions);
			if (ib.ShowDialog() == DialogResult.OK)
			{
				PckSpriteCollection.Save(
						directory,
						file,
						images,
						(_radio2.Checked) ? 2 : 4);
			}
		}
	}


	/// <summary>
	/// class xcPckTab
	/// </summary>
	public class xcPckTab
		:
		xcPck
	{
		public xcPckTab()
		{
			author	= "Ben Ratzlaff";
			ext		= ".tab";
			desc	= "Opens tab files as pck";
			expDesc	= "Tab File";

			_fileOptions.Init(false, false, false, false);
		}

		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				Palette pal)
		{
			string fileBase = file.Substring(0, file.IndexOf(".", StringComparison.Ordinal));
			return base.LoadFileOverride(
									directory,
									fileBase + ".pck",
									width,
									height,
									pal);
		}
	}
}
*/
