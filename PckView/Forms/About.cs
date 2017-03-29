using System;
using System.Diagnostics;
using System.Reflection;


namespace PckView
{
	internal sealed partial class About
		:
			System.Windows.Forms.Form
	{
		public About()
		{
			InitializeComponent();

			FileVersionInfo info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
#if DEBUG
			lblVer.Text = string.Format(
									System.Globalization.CultureInfo.InvariantCulture,
									"Debug version {0},{1},{2},{3}",
									info.FileMajorPart,
									info.FileMinorPart,
									info.FileBuildPart,
									info.FilePrivatePart);
#else
			lblVer.Text = string.Format(
									System.Globalization.CultureInfo.InvariantCulture,
									"Release version {0},{1},{2},{3}",
									info.FileMajorPart,
									info.FileMinorPart,
									info.FileBuildPart,
									info.FilePrivatePart);
#endif
		}
	}
}
