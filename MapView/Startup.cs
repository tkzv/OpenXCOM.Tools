using System;
using System.Threading;
using System.Windows.Forms;

using MapView.Forms.XCError;


namespace MapView
{
	/// <summary>
	/// Class that starts program execution.
	/// </summary>
	public class Startup
//		: MarshalByRefObject
	{
		private readonly IErrorHandler _errorHandler;


		public Startup()
		{
			_errorHandler = new ErrorWindowAdapter();
		}


		public void RunProgram()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += Application_ThreadException; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			try
			{
				var mainWindow = new XCMainWindow();

//				mainWindow.SendMessage += new StringDelegate(mw_SendMessage);

				Application.Run(mainWindow);

				// https://msdn.microsoft.com/en-us/library/system.appdomain.aspx
				// Get this AppDomain's settings and display some of them.
//				var ads = AppDomain.CurrentDomain.SetupInformation;
//				Console.WriteLine(
//								"AppName={0}, AppBase={1}, ConfigFile={2}",
//								ads.ApplicationName,
//								ads.ApplicationBase,
//								ads.ConfigurationFile);
			}
			catch (Exception ex)
			{
				_errorHandler.HandleException(ex);
//				throw;
			}
		}

		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			_errorHandler.HandleException(e.Exception);
		}

//		void mw_SendMessage(object sender, string args)
//		{
//			Console.WriteLine("External command: " + args);
//		}
	}
}
