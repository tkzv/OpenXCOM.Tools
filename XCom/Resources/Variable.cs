using System;
using System.Collections;
using System.IO;


namespace XCom
{
	internal sealed class Variable
	{
		private readonly ArrayList _list = new ArrayList();

		private readonly string _name;
		private readonly string _value;

		private static int _id;


		internal Variable(string prefix, string post)
		{
			_name  = "${var" + (_id++) + "}";
			_value = post;
			_list.Add(prefix);
		}

		internal Variable(string baseVar, string prefix, string post)
		{
			_name  = "${var" + baseVar + (_id++) + "}";
			_value = post;
			_list.Add(prefix);
		}


		internal string Name
		{
			get { return _name; }
		}

//		internal string Value
//		{
//			get { return _value; }
//		}

		internal void Add(string prefix)
		{
			_list.Add(prefix);
		}


		private const string DefaultPrefix = ""; // NOTE: doesn't accept String.Empty.

		internal void Write(TextWriter sw, string prefix = DefaultPrefix)
		{
			if (_list.Count > 1)
			{
				sw.WriteLine(Environment.NewLine + prefix + _name + ":" + _value);
				foreach (string st in _list)
					sw.WriteLine(prefix + st + _name);
			}
			else
				sw.WriteLine(prefix + (string)_list[0] + _value);
		}
	}
}
