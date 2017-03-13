using System;

using XCom.Interfaces;


namespace PckView
{
	public class xcCustom
		:
		IXCImageFile
	{
		public xcCustom()
			:
			this(0, 0)
		{}

		public xcCustom(int wid, int hei)
			:
			base(wid, hei)
		{
			author	= "Ben Ratzlaff";
			ext		= ".*";
			desc	= "Options for opening unknown files";
			expDesc	= "Any File";

			fileOptions.Init(false, false, true, false);

			defPal = XCom.Palette.UFOBattle;
		}

//		public override int FilterIndex
//		{
//			get { return base.FilterIndex; }
//			set { base.FilterIndex = value; FIDX = value; }
//		}

		protected override XCom.XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				XCom.Palette pal)
		{
			var ocf = new OpenCustomForm(directory, file);
			ocf.TryClick += tryIt;
			ocf.Show();

			return null;
		}

		private void tryIt(object sender, TryDecodeEventArgs tde)
		{
			var pvf = (PckViewForm)XCom.SharedSpace.Instance["PckView"];

			XCom.XCImageCollection ixc = tde.XCFile.LoadFile(
														tde.Directory,
														tde.File,
														tde.TryWidth,
														tde.TryHeight);
//			ixc.IXCFile = this;
			imageSize = new System.Drawing.Size(tde.TryWidth, tde.TryHeight);

			pvf.SetImages(ixc);
		}
	}
}
