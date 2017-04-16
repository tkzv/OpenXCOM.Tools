using System;

using XCom;


namespace MapView.Forms.XCError.WarningConsole
{
	internal sealed class ConsoleWarningHandler
	{
		private readonly ConsoleSharedSpace _consoleShare;

		internal ConsoleWarningHandler(ConsoleSharedSpace consoleShare)
		{
			_consoleShare = consoleShare;
		}

		internal void HandleWarning(string st)
		{
			var console = _consoleShare.GetConsole();
			if (console != null)
			{
				console.Show();
				XConsole.AdZerg("WARNING: " + st);
			}
		}
	}
}
