using System;


namespace PckView
{
	public class xcForceCustom
		:
		xcCustom
	{
		public xcForceCustom()
			:
			base(0, 0)
		{
			_fileOptions.Init(false, false, true, false);

//			fileOptions[Filter.Bmp] = false;
//			fileOptions[Filter.Save] = false;
//			fileOptions[Filter.Open] = true;
//			fileOptions[Filter.Custom] = false;

			author	= "Ben Ratzlaff";
			ext		= ".*";
			desc	= "Forces the Custom File Dialog box to be shown";
			expDesc	= "Custom File";
		}

		public override string SingleFileName
		{
			get { return "*.*"; }
		}
	}
}
