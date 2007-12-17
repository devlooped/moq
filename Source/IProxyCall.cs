using System.Runtime.Remoting.Messaging;

namespace Moq
{
	internal interface IProxyCall
	{
		bool Matches(IMethodCallMessage call);
		IMethodReturnMessage Execute(IMethodCallMessage call);
	}
}
