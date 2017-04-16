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
			var console = _share.SetShare(xConsole);
			return (console != null) ? console as ConsoleForm
									 : (ConsoleForm)_share.SetShare(xConsole, new ConsoleForm());
		}
	}
}
