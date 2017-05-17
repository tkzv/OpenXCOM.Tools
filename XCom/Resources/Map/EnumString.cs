using System;


namespace XCom
{
	public struct EnumString
	{
		private readonly object _enumeration;
		public object Enum
		{
			get { return _enumeration; }
		}

		private readonly string _display;


		internal EnumString(string display, object enumeration)
		{
			_display     = display;
			_enumeration = enumeration;
		}


		public override string ToString()
		{
			return _display;
		}
	}
}
