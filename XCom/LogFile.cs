using System;

#if DEBUG
using System.IO;
#endif


namespace XCom
{
	public static class LogFile
	{
#if DEBUG
		private const string DebugLogFile = "debug.log";

		private static string _pathdir;
		public static string PathDir
		{
			private get { return _pathdir; }
			set
			{
				_pathdir = Path.Combine(value, DebugLogFile);
				CreateLog();
			}
		}
#endif

		/// <summary>
		/// Creates a logfile or cleans the old one. The logfile should be
		/// created by setting 'PathDir' only.
		/// </summary>
		private static void CreateLog()
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													PathDir,
													FileMode.Create,
													FileAccess.Write,
													FileShare.None)))
			{}
#endif
		}

		/// <summary>
		/// Writes a line to the logfile.
		/// </summary>
		/// <param name="line">the line to write</param>
		public static void WriteLine(string line)
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													PathDir,
													FileMode.Append,
													FileAccess.Write,
													FileShare.None)))
			{
				sw.WriteLine(line);
			}
#endif
		}
	}
}
