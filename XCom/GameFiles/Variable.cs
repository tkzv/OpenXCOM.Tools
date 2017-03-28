using System;
using System.Collections;
using System.IO;


namespace XCom
{
	internal sealed class Variable
	{
		private readonly string _name;
		private readonly string _value;

		private static int _count = 0;

		private readonly ArrayList _list;


		public Variable(string prefix, string post)
		{
			_name = "${var" + (_count++) + "}";
			_value = post;
			_list = new ArrayList();
			_list.Add(prefix);
		}

		public Variable(string baseVar, string prefix, string post)
		{
			_name = "${var" + baseVar + (_count++) + "}";
			_value = post;
			_list = new ArrayList();
			_list.Add(prefix);
		}


		public string Name
		{
			get { return _name; }
		}

		public string Value
		{
			get { return _value; }
		}

		public void Add(string prefix)
		{
			_list.Add(prefix);
		}

		public void Write(StreamWriter sw)
		{
			Write(sw, String.Empty);
		}

		public void Write(TextWriter sw, string prefix)
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
