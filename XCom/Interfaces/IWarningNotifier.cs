using System;

namespace XCom.Interfaces
{
	internal interface IWarningNotifier
	{
		event Action<string> HandleWarning;
	}
}
