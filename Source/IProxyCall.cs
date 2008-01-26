using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;
using System.Linq.Expressions;

namespace Moq
{
	internal interface IProxyCall
	{
		bool Matches(IInvocation call);
		void Execute(IInvocation call);
		bool IsVerifiable { get; set; }
		bool Invoked { get; set; }
		Expression ExpectExpression { get; }
	}
}
