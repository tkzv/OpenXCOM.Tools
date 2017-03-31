using System;
//using System.IO;
//using System.Collections.Generic;


namespace DSShared
{
	/// <summary>
	/// This is used with VaridiaStructured for PckView.
	/// </summary>
	internal sealed class DSVariable
	{
//		private static int Count = 0;

//		private readonly List<string> _list;
//		private readonly string _var;

		/// <summary>
		/// </summary>
		public string Value
		{ get; set; }


		// kL_note: Ah, briliant! An entire class holds a single variable.
		/// <summary>
		/// </summary>
//		public DSVariable(string prefix, string value)
		public DSVariable(string value)
		{
//			_var = "${var" + (Count++) + "}";
			Value = value;
//			_list = new List<string>();
//			_list.Add(prefix);
		}


/*		/// <summary>
		/// </summary>
		public string Name
		{
			get { return _var; }
		} */

/*		/// <summary>
		/// </summary>
		public DSVariable(string baseVar, string prefix, string post)
		{
//			_var = "${var" + baseVar + (Count++) + "}";
			Value = post;
			_list = new List<string>();
			_list.Add(prefix);
		} */

/*		/// <summary>
		/// </summary>
		public void Add(string prefix)
		{
			_list.Add(prefix);
		} */

/*		/// <summary>
		/// </summary>
		public void Write(StreamWriter sw)
		{
			Write(sw, String.Empty);
		} */

/*		/// <summary>
		/// </summary>
		public void Write(StreamWriter sw, string pref)
		{
			if (_list.Count > 1)
			{
				sw.WriteLine(pref + _var + DSVaridia.Separator + Value);
				foreach (string pre in _list)
					sw.WriteLine(pref + pre + _var);
			}
			else
				sw.WriteLine(pref + (string)_list[0] + Value);
		} */
	}
}
