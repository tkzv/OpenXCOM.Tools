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
			return _share.SetShare(xConsole) as ConsoleForm;
		}

		public ConsoleForm GetNewConsole()
		{
			var console = GetConsole();
			return console ?? (ConsoleForm)_share.SetShare(xConsole, new ConsoleForm());
		}
	}
}
