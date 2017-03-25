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

		public xcCustom(int width, int height)
			:
			base(width, height)
		{
			_ext        = ".*";
			_fileFilter = "Any File";
			_desc       = "Options for opening unknown files";
			_author     = "Ben Ratzlaff";

			_fileOptions.Init(false, false, true, false);

			_palDefault = XCom.Palette.UFOBattle;
		}

/*		public override int FilterIndex
		{
			get { return base.FilterIndex; }
			set { base.FilterIndex = value; FIDX = value; }
		} */

/*		protected override XCom.XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int width,
				int height,
				XCom.Palette pal)
		{
			var f = new OpenCustomForm(directory, file);
			f.TryClick += tryIt;
			f.Show();

			return null;
		} */

/*		private void tryIt(object sender, TryDecodeEventArgs tde)
		{
			var pvf = (PckViewForm)XCom.SharedSpace.Instance["PckView"];

			XCom.XCImageCollection ixc = tde.XCFile.LoadFile(
														tde.Directory,
														tde.File,
														tde.TryWidth,
														tde.TryHeight);
//			ixc.IXCFile = this;
			_imageSize = new System.Drawing.Size(tde.TryWidth, tde.TryHeight);

			pvf.SetImages(ixc);
		} */
	}
}
