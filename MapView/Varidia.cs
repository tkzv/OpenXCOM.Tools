using System;
using System.Collections;
using System.IO;


namespace MapView
{
	/// <summary>
	/// Varidia is [still] used to read/write/parse/interpret user-settings/Options.
	/// </summary>
	internal sealed class Varidia
	{
		#region Fields
		private readonly StreamReader _sr;
		#endregion


		#region cTors
		/// <summary>
		/// cTor. Called from OptionsManager.
		/// </summary>
		public Varidia(StreamReader sr)
		{
			_sr = sr;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Read a line from MVSettings.cfg
		/// </summary>
		/// <returns>KeyValPair</returns>
		internal KeyvalPair ReadLine()
		{
			string line = String.Empty;
			do // get a good line - not a comment or empty string
			{
				if (_sr.Peek() == -1) // zilch, exit.
					return null;

				line = _sr.ReadLine().Trim();
			}
			while (line.Length == 0 || line[0] == '#');

			if (line != null)
			{
				int pos = line.IndexOf(':');
				return (pos > 0) ? new KeyvalPair(line.Substring(0, pos), line.Substring(pos + 1))
								 : new KeyvalPair(line, String.Empty);
			}
			return null;
		}
		#endregion
	}


	/// <summary>
	/// KeyvalPair - helper class for Varidia.
	/// </summary>
	public class KeyvalPair
	{
		#region Properties
		private readonly string _key;
		internal string Key
		{
			get { return _key; }
		}

		private readonly string _val;
		internal string Value
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
		internal KeyvalPair(string key, string value)
		{
			_key = key;
			_val = value;
		}
		#endregion


		#region Methods (override)
		public override string ToString()
		{
			return _key + ":" + _val;
		}
		#endregion
	}
}
