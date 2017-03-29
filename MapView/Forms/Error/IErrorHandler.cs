using System;


namespace MapView.Forms.XCError
{
	internal interface IErrorHandler
	{
		void HandleException(Exception exception);
	}
}
