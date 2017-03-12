using System;
using System.Collections;
using System.IO;


namespace XCom
{
	/// <summary>
	/// class VarCollection
	/// </summary>
	public class VarCollection
	{
		private Hashtable _vars;

		private string _baseVar;

		private VarCollection _other;

		private StreamReader _sr;


		// TODO: Document the following 5 contructors. Figure out why not all
		// class variables are initialized.

		/// <summary>
		/// cTor
		/// </summary>
		public VarCollection()
		{
			_vars = new Hashtable();
			_other = null;
			_baseVar = String.Empty;
		}

		/// <summary>
		/// cTor
		/// </summary>
		public VarCollection(StreamReader sr)
		{
			_sr = sr;

			_vars = new Hashtable();
			_other = null;
		}

		/// <summary>
		/// cTor
		/// </summary>
		public VarCollection(string baseVar)
			:
			this()
		{
			_baseVar = baseVar;
		}

		/// <summary>
		/// cTor
		/// </summary>
		public VarCollection(VarCollection other)
			:
			this()
		{
			_other = other;
		}

		/// <summary>
		/// cTor
		/// </summary>
		public VarCollection(StreamReader sr, VarCollection vars)
		{
			_vars = new Hashtable();
			_sr = sr;
			_other = vars;
		}


		/// <summary>
		/// property BaseStream
		/// </summary>
		public StreamReader BaseStream
		{
			get { return _sr; }
		}

		/// <summary>
		/// property Other
		/// </summary>
		public VarCollection Other
		{
			get { return _other; }
		}

		/// <summary>
		/// property Vars
		/// </summary>
		public Hashtable Vars
		{
			get { return _vars; }
		}

		/// <summary>
		/// property Variables
		/// </summary>
		public ICollection Variables
		{
			get { return _vars.Keys; }
		}


		/// <summary>
		/// AddVar
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="val"></param>
		public void AddVar(string flag, string val)
		{
			if (_vars[val] == null)
				_vars[val] = new Variable(_baseVar, flag + ":", val);
			else
				((Variable)_vars[val]).Inc(flag + ":");
		}

		/// <summary>
		/// ParseVar
		/// </summary>
		/// <param name="line"></param>
		/// <returns>string</returns>
		public string ParseVar(string line)
		{
			foreach (string st in _vars.Keys)
				line = line.Replace(st, (string)_vars[st]);

			return (_other != null) ? _other.ParseVar(line)
									: line;
		}

		/// <summary>
		/// property this[var]
		/// </summary>
		public string this[string var]
		{
			get
			{
				if (_other == null || _vars[var] != null)
					return (string)_vars[var];

				return _other[var];
			}
			set { _vars[var] = value; }
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <returns>KeyVal</returns>
		public KeyVal ReadLine()
		{
			string line = ReadLine(_sr, this);
			if (line != null)
			{
				int idx = line.IndexOf(':');
				return (idx > 0) ? new KeyVal(line.Substring(0, idx), line.Substring(idx + 1))
								 : new KeyVal(line, String.Empty);
			}
			return null;
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <returns>string</returns>
		public string ReadLine(StreamReader sr)
		{
			return ReadLine(sr, this);
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="vars"></param>
		/// <returns>string</returns>
		public static string ReadLine(StreamReader sr, VarCollection vars)
		{
			string line = String.Empty;

			while (true)
			{
				do // get a good line - not a comment or empty string
				{
					if (sr.Peek() == -1) // zilch, exit.
						return null;

					line = sr.ReadLine().Trim();
				}
				while (line.Length == 0 || line[0] == '#');

				if (line[0] == '$') // cache variable, get another line
				{
					int idx = line.IndexOf(':');
					string var = line.Substring(0, idx);
					string val = vars.ParseVar(line.Substring(idx + 1));
					vars[var] = val;
				}
				else // got a line
					break;
			}

			if (line.IndexOf("$", StringComparison.Ordinal) > 0) // replace any variables the line might have
				line = vars.ParseVar(line);
			
			return line;
		}
	}


	/// <summary>
	/// class KeyVal - helper class for VarCollection
	/// </summary>
	public class KeyVal
	{
		private readonly string _keyword;
		private readonly string _rest;


		public KeyVal(string keyword, string rest)
		{
			_keyword = keyword;
			_rest = rest;
		}


		public string Keyword
		{
			get { return _keyword; }
		}

		public string Rest
		{
			get { return _rest; }
		}

		public override string ToString()
		{
			return _keyword + ":" + _rest;
		}
	}
}
