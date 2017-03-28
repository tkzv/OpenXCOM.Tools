using System;
using System.Collections.Generic;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// Class to automatically parse out a VC file into a tree structure.
	/// </summary>
	public class VarCollection_Structure
	{
		private KeyVal _keyValParent;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sr">file to read</param>
		public VarCollection_Structure(StreamReader sr)
		{
			_keyValParent = new KeyVal("parent", "parent");
			_keyValParent.SubHash = new Dictionary<string, KeyVal>();
			
			var vars = new Varidia(sr);
			parse_block(vars, _keyValParent);
		}


		/// <summary>
		/// VC structure of the file.
		/// </summary>
		public Dictionary<string, KeyVal> KeyValList
		{
			get { return _keyValParent.SubHash; }
		}

		private void parse_block(Varidia vars, KeyVal keyValParent)
		{
			KeyVal keyVal;
			KeyVal keyVal0 = null;

			while (vars.ReadLine(out keyVal))
			{
				switch (keyVal.Keyword)
				{
					case "{":
						keyVal0.SubHash = new Dictionary<string, KeyVal>();
						parse_block(vars, keyVal0);
						break;

					case "}":
						return;

					default:
						keyValParent.SubHash[keyVal.Keyword] = keyVal;
						keyVal0 = keyVal;
						break;
				}
			}
		}
	}
}
