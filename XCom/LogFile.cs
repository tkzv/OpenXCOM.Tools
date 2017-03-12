using System;

#if DEBUG
using System.IO;
#endif


namespace XCom
{
	public static class LogFile
	{
#if DEBUG
		private static readonly string DebugLogFile = "debug.log";
#endif

		/// <summary>
		/// Creates a logfile or Cleans the old one.
		/// </summary>
		public static void CreateLogFile()
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open( // clean the old logfile
													DebugLogFile,
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
													DebugLogFile,
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
