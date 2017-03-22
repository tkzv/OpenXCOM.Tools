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
		private readonly Hashtable _vars;

		private string _baseVar;

		private readonly VarCollection _other;

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
		/// Adds a key-val pair to this collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void AddKeyVal(string key, string val)
		{
			if (_vars[val] == null)
				_vars[val] = new Variable(_baseVar, key + ":", val);
			else
				((Variable)_vars[val]).Add(key + ":");
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
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[2]VarCollection.ReadLine()");
			string line = ReadLine(_sr, this);
			//LogFile.WriteLine(". [2]line= " + (line ?? "null"));
			if (line != null)
			{
				int pos = line.IndexOf(':');
				//LogFile.WriteLine((pos > 0) ? ". . [2]pos':'>0 RET key= " + line.Substring(0, pos) + " val= " + line.Substring(pos + 1)
				//                            : ". . [2]pos':'<1 RET key= " + line + " val=");
				return (pos > 0) ? new KeyVal(line.Substring(0, pos), line.Substring(pos + 1))
								 : new KeyVal(line, String.Empty);
			}
			//LogFile.WriteLine("[2]. . Ret NULL");
			return null;
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <returns>string</returns>
		public string ReadLine(StreamReader sr)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[5]VarCollection.ReadLine(sr)");
			return ReadLine(sr, this);
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="vars"></param>
		/// <returns>string</returns>
		public static string ReadLine(TextReader sr, VarCollection vars)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[3,6]VarCollection.ReadLine(sr, vars)");
			string line = String.Empty;

			while (true)
			{
				//LogFile.WriteLine(". [3,6]iterate TRUE");
				do // get a good line - not a comment or empty string
				{
					if (sr.Peek() == -1) // zilch, exit.
					{
						//LogFile.WriteLine(". . . [3,6]end of file. Ret NULL");
						return null;
					}

					line = sr.ReadLine().Trim();
					//LogFile.WriteLine(". . [3,6](seeking) line= " + line);
				}
				while (line.Length == 0 || line[0] == '#');

				if (line[0] == '$') // cache the variable and get another line
				{
					int pos    = line.IndexOf(':');
					string key = line.Substring(0, pos);
					string val = vars.AliasSubstitution(line.Substring(pos + 1));

					//LogFile.WriteLine(". . [3,6]found $");
					//LogFile.WriteLine(". . [3,6]pos= " + pos);
					//LogFile.WriteLine(". . [3,6]key= " + key);
					//LogFile.WriteLine(". . [3,6]val= " + val);
					//LogFile.WriteLine(". . [3,6]cache it and look for another ...");

					vars[key] = val;
				}
				else // got a line
				{
					//LogFile.WriteLine(". . [3,6]found a line!");
					break;
				}
				//LogFile.WriteLine("");
			}

			if (line.IndexOf("$", StringComparison.Ordinal) > 0) // replace any variables the line might have
			{
				//LogFile.WriteLine(". . [3,6]$var found - do ParseVar()");
				line = vars.AliasSubstitution(line);
			}

			//LogFile.WriteLine(". . . [3,6]RET line= " + line);
			//LogFile.WriteLine("");
			return line;
		}

		/// <summary>
		/// ParseVar
		/// </summary>
		/// <param name="line"></param>
		/// <returns>string</returns>
		private string AliasSubstitution(string line)
		{
			foreach (string key in _vars.Keys)
				line = line.Replace(key, (string)_vars[key]);

			return (_other != null) ? _other.AliasSubstitution(line)
									: line;
		}
	}


	/// <summary>
	/// class KeyVal - helper class for VarCollection
	/// </summary>
	public class KeyVal
	{
		private readonly string _key;
		private readonly string _val;


		public KeyVal(string key, string val)
		{
			_key = key;
			_val = val;
		}


		public string Keyword
		{
			get { return _key; }
		}

		public string Value
		{
			get { return _val; }
		}

		public override string ToString()
		{
			return _key + ":" + _val;
		}
	}
}
