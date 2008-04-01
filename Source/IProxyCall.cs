using System.Linq.Expressions;
using Castle.Core.Interceptor;

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
