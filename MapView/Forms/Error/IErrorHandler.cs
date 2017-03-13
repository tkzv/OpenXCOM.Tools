using System;


namespace MapView.Forms.XCError
{
	public interface IErrorHandler
	{
		void HandleException(Exception exception);
	}
}
