using System;

using XCom;
using XCom.Interfaces;


namespace MapView.Forms.XCError.WarningConsole
{
	public class ConsoleWarningHandler
		:
		IWarningHandler
	{
		private readonly ConsoleSharedSpace _consoleShare;

		public ConsoleWarningHandler(ConsoleSharedSpace consoleShare)
		{
			_consoleShare = consoleShare;
		}

		public void HandleWarning(string message)
		{
			var console = _consoleShare.GetConsole();
			if (console == null)
			{
				console = _consoleShare.GetNewConsole();
				console.Show();
			}
			xConsole.AddLine("WARNING: " + message);
		}
	}
}
