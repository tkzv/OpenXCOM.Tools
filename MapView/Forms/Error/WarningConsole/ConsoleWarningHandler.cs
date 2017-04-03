using System;

using XCom;
using XCom.Interfaces;


namespace MapView.Forms.XCError.WarningConsole
{
	internal sealed class ConsoleWarningHandler
	{
		private readonly ConsoleSharedSpace _consoleShare;

		public ConsoleWarningHandler(ConsoleSharedSpace consoleShare)
		{
			_consoleShare = consoleShare;
		}

		public void HandleWarning(string st)
		{
			var console = _consoleShare.GetConsole();
			if (console == null)
			{
				console = _consoleShare.GetNewConsole();
				console.Show();
			}
			XConsole.AdZerg("WARNING: " + st);
		}
	}
}
