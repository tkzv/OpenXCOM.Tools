using System;


namespace XCom
{
	public struct StrEnum
	{
		private readonly object _enumeration;
		private string _display;


		public StrEnum(string display, object enumeration)
		{
			_display = display;
			_enumeration = enumeration;
		}


		public override string ToString()
		{
			return _display;
		}

		public object Enum
		{
			get { return _enumeration; }
		}
	}
}
