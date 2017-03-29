using System;


namespace MapView.Forms.XCError
{
	internal sealed class ErrorWindowAdapter
		:
			IErrorHandler
	{
		public void HandleException(Exception exception)
		{
			using (var errorWindow = new ErrorWindow(exception)) // wtf. 'using' ... for what.
			{
				errorWindow.ShowDialog();
			}
		}
	}
}
