using System;
using System.IO;


namespace XCom
{
	public class LogFile
	{
		private static readonly string DebugLogFile = "debug.log";
		private static LogFile _file;

		private static bool _on = false;

		private readonly StreamWriter _sw;


		private LogFile(string file)
		{
			if (_on)
				_sw = new StreamWriter(File.Open(file, FileMode.Create));
		}


		public static bool DebugOn
		{
			get { return _on; }
			set { _on = value; }
		}

		public static LogFile Instance
		{
			get { return Init(DebugLogFile); }
		}

		public static LogFile Init(string file)
		{
#if DEBUG
			_on = true;
#endif
			if (_file == null)
				_file = new LogFile(file);

			return _file;
		}

		public void Write(string text)
		{
//			if ((_on || _sw != null) && _sw != null)	// wft.
//			if (_on && _sw != null)						// kL
			if (_sw != null)							// your choice.
			{
				_sw.Write(text);
				_sw.Flush();
			}
		}

		public void WriteLine(string text)
		{
//			if ((_on || _sw != null) && _sw != null)	// wft.
//			if (_on && _sw != null)						// kL
			if (_sw != null)							// your choice.
			{
				_sw.WriteLine(text);
				_sw.Flush();
			}
		}

		public void Close()
		{
//			if ((_on || _sw != null) && _sw != null)	// wft.
//			if (_on && _sw != null)						// kL
			if (_sw != null)							// your choice.
			{
				_sw.Close();
			}
		}
	}
}
