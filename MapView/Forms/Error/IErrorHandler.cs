using System;


namespace MapView.Forms.Error
{
	public interface IErrorHandler
	{
		void HandleException(Exception exception);
	}
}
