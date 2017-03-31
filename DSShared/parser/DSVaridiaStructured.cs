using System;
using System.Collections.Generic;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// Class to automatically parse out a Varidia file into a tree structure.
	/// </summary>
	public sealed class DSVaridiaStructured
	{
		private DSKeyvalPair _keyvalParent;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sr">file to read</param>
		public DSVaridiaStructured(StreamReader sr)
		{
			_keyvalParent = new DSKeyvalPair("parent", "parent");
			_keyvalParent.SubHash = new Dictionary<string, DSKeyvalPair>();
			
			var vars = new DSVaridia(sr);
			ParseBlock(vars, _keyvalParent);
		}


		/// <summary>
		/// The Varidia structure of the file.
		/// </summary>
		public Dictionary<string, DSKeyvalPair> KeyvalList
		{
			get { return _keyvalParent.SubHash; }
		}

		private void ParseBlock(DSVaridia vars, DSKeyvalPair keyValParent)
		{
			DSKeyvalPair keyVal;
			DSKeyvalPair keyVal0 = null;

			while (vars.ReadLine(out keyVal))
			{
				switch (keyVal.Key)
				{
					case "{":
						keyVal0.SubHash = new Dictionary<string, DSKeyvalPair>();
						ParseBlock(vars, keyVal0);
						break;

					case "}":
						return;

					default:
						keyValParent.SubHash[keyVal.Key] = keyVal;
						keyVal0 = keyVal;
						break;
				}
			}
		}
	}
}
