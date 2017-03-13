namespace XCom
{
	public class ConsoleSharedSpace
	{
		private const string X_CONSOLE = "xConsole";

		private readonly SharedSpace _share;


		public ConsoleSharedSpace(SharedSpace share)
		{
			_share = share;
		}


		public ConsoleForm GetConsole()
		{
			return _share.AllocateObject(X_CONSOLE) as ConsoleForm;
		}

		public ConsoleForm GetNewConsole()
		{
			var console = GetConsole();
			return console ?? (ConsoleForm)_share.AllocateObject(X_CONSOLE, new ConsoleForm());
		}
	}
}
