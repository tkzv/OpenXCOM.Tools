using System;


namespace MapView.Forms.XCError
{
	internal sealed class ErrorWindowAdapter
		:
			IErrorHandler
	{
		public void HandleException(Exception exception)
		{
			using (var f = new ErrorWindow(exception))
				f.ShowDialog();
		}
	}
}
