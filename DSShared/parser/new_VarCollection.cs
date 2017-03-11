using System;
using System.Collections.Generic;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// </summary>
	public class VarCollection
	{
		private Dictionary<string, Variable> _vars;

		private VarCollection _other;

		private string _baseVar;

		private StreamReader _sr;


		/// <summary>
		/// </summary>
		public static readonly char Separator = ':';


		/// <summary>
		/// </summary>
		public VarCollection()
		{
			_vars = new Dictionary<string, Variable>();
			_other = null;
			_baseVar = String.Empty;
		}

		/// <summary>
		/// </summary>
		public VarCollection(StreamReader sr)
		{
			_sr = sr;
			_vars = new Dictionary<string, Variable>();
			_other = null;
		}

		/// <summary>
		/// </summary>
		public VarCollection(string baseVar)
			:
			this()
		{
			_baseVar = baseVar;
		}

		/// <summary>
		/// </summary>
		public VarCollection(VarCollection other)
			:
			this()
		{
			_other = other;
		}


		/// <summary>
		/// </summary>
		public void AddVar(string flag, string val)
		{
			if (_vars[val] == null)
				_vars[val] = new Variable(_baseVar, flag + Separator, val);
			else
				((Variable)_vars[val]).Inc(flag + Separator);
		}

		/// <summary>
		/// </summary>
		public Dictionary<string, Variable> Vars
		{
			get { return _vars; }
		}

		/// <summary>
		/// </summary>
		public StreamReader BaseStream
		{
			get { return _sr; }
		}

		/// <summary>
		/// </summary>
		public string ParseVar(string line)
		{
			foreach (string st in _vars.Keys)
				line = line.Replace(st, _vars[st].Value);

			return (_other != null) ? _other.ParseVar(line)
									: line;
		}

		/// <summary>
		/// </summary>
		public ICollection<string> Variables
		{
			get { return _vars.Keys; }
		}

		/// <summary>
		/// </summary>
		public string this[string var]
		{
			get
			{
				if (_other == null || _vars[var] != null)
					return (string)_vars[var].Value;

				return _other[var];
			}
			set
			{
				if (_vars[var] != null)
					_vars[var].Value = value;
				else
					_vars[var] = new Variable(var, value);
			}
		}

		/// <summary>
		/// </summary>
		public bool ReadLine(out KeyVal output)
		{
			return (output = ReadLine()) != null;
		}

		/// <summary>
		/// </summary>
		public KeyVal ReadLine()
		{
			string line = ReadLine(_sr, this);
			if (line != null)
			{
				int idx = line.IndexOf(Separator);
				return (idx > 0) ? new KeyVal(line.Substring(0, idx), line.Substring(idx + 1))
								 : new KeyVal(line, String.Empty);
			}
			return null;
		}

		/// <summary>
		/// </summary>
		public string ReadLine(StreamReader sr)
		{
			return ReadLine(_sr, this);
		}

		/// <summary>
		/// </summary>
		public static string ReadLine(StreamReader sr, VarCollection vars)
		{
			string line = String.Empty;

			while (true)
			{
				do // get a good line - not a comment or empty string
				{
					if (sr.Peek() != -1)
						line = sr.ReadLine().Trim();
					else
						return null;
				}
				while (line.Length == 0 || line[0] == '#');

				if (line[0] == '$') // cache variable, get another line
				{
					int idx = line.IndexOf(Separator);
					string var = line.Substring(0, idx);
					string val = line.Substring(idx + 1);
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
	/// </summary>
	public class KeyVal
	{
		private readonly string _keyword;
		private readonly string _rest;

		/// <summary>
		/// </summary>
		public KeyVal(string keyword, string rest)
		{
			_keyword = keyword;
			_rest = rest;
		}

		/// <summary>
		/// </summary>
		public string Keyword
		{
			get { return _keyword; }
		}

		/// <summary>
		/// </summary>
		public string Rest
		{
			get { return _rest; }
		}

		/// <summary>
		/// </summary>
		public Dictionary<string, KeyVal> SubHash
		{ get; set; }

		/// <summary>
		/// </summary>
		public override string ToString()
		{
			return _keyword + ':' + _rest;
		}
	}
}
