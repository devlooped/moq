using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;

namespace Moq
{
	internal interface IProxyCall
	{
		bool Matches(IInvocation call);
		void Execute(IInvocation call);
	}
}
