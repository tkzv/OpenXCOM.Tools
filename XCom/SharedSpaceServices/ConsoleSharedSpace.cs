namespace XCom
{
	public class ConsoleSharedSpace
	{
		private const string xConsole = "xConsole";

		private readonly SharedSpace _share;


		public ConsoleSharedSpace(SharedSpace share)
		{
			_share = share;
		}


		public ConsoleForm GetConsole()
		{
			return _share.AllocateObject(xConsole) as ConsoleForm;
		}

		public ConsoleForm GetNewConsole()
		{
			var console = GetConsole();
			return console ?? (ConsoleForm)_share.AllocateObject(xConsole, new ConsoleForm());
		}
	}
}
