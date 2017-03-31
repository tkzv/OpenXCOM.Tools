using System;
using System.Collections.Generic;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// This is used with VarCollection_Structure for PckView.
	/// </summary>
	internal sealed class DSVaridia
	{
		private const char Separator = ':';


		private readonly Dictionary<string, DSVariable> _vars;
//		private readonly DSVaridia _other;

		private readonly StreamReader _sr;


		/// <summary>
		/// </summary>
		public DSVaridia(StreamReader sr)
		{
			_vars = new Dictionary<string, DSVariable>();
//			_other = null;
			_sr = sr;
		}

		/// <summary>
		/// </summary>
		private string this[string var]
		{
/*			get
			{
//				if (_other == null || _vars[var] != null)
				return (string)_vars[var].Value;

//				return _other[var];
			} */
			set
			{
				if (_vars[var] != null)
					_vars[var].Value = value;
				else
					_vars[var] = new DSVariable(value);
//					_vars[var] = new DSVariable(var, value);
			}
		}

		/// <summary>
		/// </summary>
		public bool ReadLine(out DSKeyvalPair output)
		{
			return (output = ReadLine()) != null;
		}

		private DSKeyvalPair ReadLine()
		{
			string line = ReadLine(_sr, this);
			if (line != null)
			{
				int idx = line.IndexOf(Separator);
				return (idx > 0) ? new DSKeyvalPair(line.Substring(0, idx), line.Substring(idx + 1))
								 : new DSKeyvalPair(line, String.Empty);
			}
			return null;
		}

		private static string ReadLine(TextReader sr, DSVaridia vars)
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
				line = vars.Parse(line);

			return line;
		}

		private string Parse(string line)
		{
			foreach (string st in _vars.Keys)
				line = line.Replace(st, _vars[st].Value);

//			return (_other != null) ? _other.Parse(line)
//									: line;
			return line;
		}


//		private string _baseVar;

/*		/// <summary>
		/// </summary>
		private DSVaridia()
		{
			_vars = new Dictionary<string, DSVariable>();
			_other = null;
			_baseVar = String.Empty;
		} */

/*		/// <summary>
		/// </summary>
		public DSVaridia(string baseVar)
			:
				this()
		{
			_baseVar = baseVar;
		} */

/*		/// <summary>
		/// </summary>
		public DSVaridia(DSVaridia other)
			:
				this()
		{
			_other = other;
		} */

/*		/// <summary>
		/// </summary>
		public void AddVar(string flag, string val)
		{
			if (_vars[val] == null)
				_vars[val] = new DSVariable(_baseVar, flag + Separator, val);
			else
				((DSVariable)_vars[val]).Add(flag + Separator);
		} */

/*		/// <summary>
		/// </summary>
		public Dictionary<string, DSVariable> Vars
		{
			get { return _vars; }
		} */

/*		/// <summary>
		/// </summary>
		public StreamReader BaseStream
		{
			get { return _sr; }
		} */

/*		/// <summary>
		/// </summary>
		public ICollection<string> Variables
		{
			get { return _vars.Keys; }
		} */

/*		/// <summary>
		/// </summary>
		public string ReadLine(StreamReader sr)
		{
			return ReadLine(_sr, this);
		} */
	}


	/// <summary>
	/// </summary>
	public sealed class DSKeyvalPair
	{
		private readonly string _key;
		private readonly string _val;


		/// <summary>
		/// </summary>
		internal DSKeyvalPair(string key, string value)
		{
			_key = key;
			_val = value;
		}


		/// <summary>
		/// </summary>
		internal string Key
		{
			get { return _key; }
		}

		/// <summary>
		/// </summary>
		public string Value
		{
			get { return _val; }
		}

		/// <summary>
		/// </summary>
		public Dictionary<string, DSKeyvalPair> SubHash
		{ get; set; }

		/// <summary>
		/// </summary>
		public override string ToString() // isUsed yes/no
		{
			return _key + ':' + _val;
		}
	}
}
