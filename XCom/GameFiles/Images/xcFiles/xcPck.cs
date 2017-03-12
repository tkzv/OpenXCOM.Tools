using System;
using System.Windows.Forms;

using XCom.Interfaces;


namespace XCom.GameFiles.Images.xcFiles
{
	public class xcPck
		:
		IXCImageFile
	{
		private const string TAB_EXT = ".tab";
		private Panel savePanel;
		private RadioButton radio2, radio4;


		public xcPck()
			:
			base(32,40)
		{
			ext = ".pck";
			author = "Ben Ratzlaff";
			desc = "Standard pck file codec";

			expDesc = "Pck File";
		}


		protected override XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			string tabBase = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));
			var tabFilePath = directory + @"\" + tabBase + TAB_EXT;

			System.IO.Stream tabStream = null;
			if (System.IO.File.Exists(tabFilePath))
				tabStream = System.IO.File.OpenRead(tabFilePath);

			using (tabStream)
			using (var pckStream = System.IO.File.OpenRead(directory + @"\" + file))
			{
				try
				{
					return new PckFile(
									pckStream,
									tabStream,
									2,
									pal,
									imgHei,
									imgWid);
				}
				catch (Exception)
				{
					return new PckFile(
									pckStream,
									tabStream,
									4,
									pal,
									imgHei,
									imgWid);
				}
			}
		}

		private Panel SavingOptions
		{
			get
			{
				if (savePanel == null)
				{
					savePanel = new Panel();

					var gb = new GroupBox();
					gb.Text = "Bpp Options";

					var top = new Panel();
					top.Dock = DockStyle.Top;
					top.Height = 50;

					radio2 = new RadioButton();
					radio2.Text = "2";
					radio2.Dock = DockStyle.Left;
					radio2.Height = 50;
					radio2.Width = 40;
					radio2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
					radio2.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
					radio2.Checked = true;
					radio2.CheckedChanged += checkChange;

					var l = new Label();
					l.Text = "UFO/TFTD Terrain\nUFO Units";
					l.Dock = DockStyle.Fill;

					top.Controls.AddRange(new Control[]{ l, radio2 });

					var mid = new Panel();
					mid.Dock = DockStyle.Fill;

					radio4 = new RadioButton();
					radio4.Text = "4";
					radio4.Dock = DockStyle.Left;
					radio4.Width = radio2.Width;
					radio4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
					radio4.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
					radio4.CheckedChanged += checkChange;

					l = new Label();
					l.Text = "TFTD Units";
					l.Dock = DockStyle.Fill;

					mid.Controls.AddRange(new Control[]{ l, radio4 });

					gb.Controls.AddRange(new Control[]{ mid, top });
					gb.Dock = DockStyle.Left;

					var left = new Panel();
					left.Dock = DockStyle.Top;
					left.Height = 100;

					left.Controls.Add(gb);

					l = new Label();
					l.Text = "If you are unsure about the correct bpp option,"
						+ " open up the original .pck file and see what the number"
						+ " is in the lower right of the main screen.";
					l.Dock = DockStyle.Fill;

					savePanel.Controls.AddRange(new Control[]{ l, left });
				}

				return savePanel;
			}

			set { savePanel = value; }
		}

		private void checkChange(object sender, EventArgs e)
		{
			radio2.Checked = (sender == radio2);
			radio4.Checked = (sender == radio4);
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
				PckFile.Save(
						directory,
						file,
						images,
						(radio2.Checked) ? 2 : 4);
			}
		}
	}


	/// <summary>
	///  class xcPckTab
	/// </summary>
	public class xcPckTab
		:
		xcPck
	{
		public xcPckTab()
		{
			author	= "Ben Ratzlaff";
			desc	= "Opens tab files as pck";
			ext		= ".tab";
			expDesc	= "Tab File";

			fileOptions.Init(false, false, false, false);
		}

		protected override XCImageCollection LoadFileOverride(
														string directory,
														string file,
														int imgWid,
														int imgHei,
														Palette pal)
		{
			string fileBase = file.Substring(0, file.IndexOf(".", StringComparison.Ordinal));
			return base.LoadFileOverride(
									directory,
									fileBase + ".pck",
									imgWid,
									imgHei,
									pal);
		}
	}
}
