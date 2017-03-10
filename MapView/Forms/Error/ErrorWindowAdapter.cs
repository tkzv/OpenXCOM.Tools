using System;


namespace MapView.Forms.Error
{
	public class ErrorWindowAdapter
		:
		IErrorHandler
	{
		public void HandleException(Exception exception)
		{
			using (var errorWindow = new ErrorWindow(exception))
			{
				errorWindow.ShowDialog();
			}
		}
	}
}
