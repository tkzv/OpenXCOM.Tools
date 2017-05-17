using System;
using System.Collections;
using System.IO;


namespace XCom
{
	/// <summary>
	/// class Varidia
	/// </summary>
	public sealed class Varidia
	{
		#region Fields & Properties
		private string _baseVar;

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
		/// property Variables
		/// </summary>
		public ICollection Variables
		{
			get { return _vars.Keys; }
		}

		private readonly Hashtable _vars;
		/// <summary>
		/// property Vars
		/// </summary>
		public Hashtable Vars
		{
			get { return _vars; }
		}

		private readonly Varidia _other;
		/// <summary>
		/// property Other
		/// </summary>
		public Varidia Other
		{
			get { return _other; }
		}

		private StreamReader _sr;
		/// <summary>
		/// property BaseStream
		/// </summary>
		public StreamReader BaseStream
		{
			get { return _sr; }
		}
		#endregion


		// TODO: Document the following 5 contructors. Figure out why not all
		// class variables are initialized. Figure out why this class exists.
		// oh yeah, to parse the Apollo trajectory to the moon. And back ....

		#region cTors
		/// <summary>
		/// cTor
		/// </summary>
		public Varidia()
		{
			_vars = new Hashtable();
			_other = null;
			_baseVar = String.Empty;
		}
		/// <summary>
		/// cTor
		/// </summary>
		public Varidia(StreamReader sr)
		{
			_sr = sr;

			_vars = new Hashtable();
			_other = null;
		}
		/// <summary>
		/// cTor
		/// </summary>
		public Varidia(string baseVar)
			:
				this()
		{
			_baseVar = baseVar;
		}
		/// <summary>
		/// cTor
		/// </summary>
		public Varidia(Varidia other)
			:
				this()
		{
			_other = other;
		}
		/// <summary>
		/// cTor
		/// </summary>
		public Varidia(StreamReader sr, Varidia vars)
		{
			_vars = new Hashtable();
			_sr = sr;
			_other = vars;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds a key-val pair to this collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddKeyvalPair(string key, string value)
		{
			if (_vars[value] == null)
				_vars[value] = new Variable(_baseVar, key + ":", value);
			else
				((Variable)_vars[value]).Add(key + ":");
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <returns>KeyVal</returns>
		public KeyvalPair ReadLine()
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[2]Varidia.ReadLine()");
			string line = ReadLine(_sr, this);
			//LogFile.WriteLine(". [2]line= " + (line ?? "null"));
			if (line != null)
			{
				int pos = line.IndexOf(':');
				//LogFile.WriteLine((pos > 0) ? ". . [2]pos':'>0 RET key= " + line.Substring(0, pos) + " val= " + line.Substring(pos + 1)
				//                            : ". . [2]pos':'<1 RET key= " + line + " val=");
				return (pos > 0) ? new KeyvalPair(line.Substring(0, pos), line.Substring(pos + 1))
								 : new KeyvalPair(line, String.Empty);
			}
			//LogFile.WriteLine("[2]. . Ret NULL");
			return null;
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <returns>string</returns>
		public string ReadLine(TextReader sr)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[5]Varidia.ReadLine(sr)");
			return ReadLine(sr, this);
		}

		/// <summary>
		/// ReadLine
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="vars"></param>
		/// <returns>string</returns>
		public static string ReadLine(TextReader sr, Varidia vars)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("[3,6]Varidia.ReadLine(sr, vars)");
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
		#endregion
	}


	/// <summary>
	/// KeyvalPair - helper class for Varidia.
	/// </summary>
	public class KeyvalPair
	{
		#region Fields & Properties
		private readonly string _key;
		public string Keyword
		{
			get { return _key; }
		}

		private readonly string _val;
		public string Value
		{
			get { return _val; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public KeyvalPair(string key, string value)
		{
			_key = key;
			_val = value;
		}
		#endregion


		#region Methods
		public override string ToString()
		{
			return _key + ":" + _val;
		}
		#endregion
	}
}
