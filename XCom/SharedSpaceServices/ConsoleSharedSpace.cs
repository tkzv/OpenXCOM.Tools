namespace XCom
{
	/// <summary>
	/// Yet another class because we can apparently.
	/// </summary>
	public class ConsoleSharedSpace
	{
		#region Fields (static)
		private const string xConsole = "xConsole";
		#endregion

		#region Fields
		private readonly SharedSpace _share;
		#endregion


		#region Properties
		private ConsoleForm _console;
		public ConsoleForm Console
		{
			get
			{
				if (_console == null)
				{
					_console = new ConsoleForm();
					_share.SetShare(xConsole, _console);
				}
				return _console;
			}
		}
		#endregion


		#region cTor
		public ConsoleSharedSpace(SharedSpace share)
		{
			_share = share;
		}
		#endregion
	}
}
