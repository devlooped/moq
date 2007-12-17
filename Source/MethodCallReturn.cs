using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Moq
{
	internal class MethodCallReturn<TResult> : MethodCall, ICall<TResult>, IProxyCall
	{
		TResult value;
		Func<TResult> valueFunc;

		public MethodCallReturn(MethodInfo method, params Expression[] arguments)
			: base(method, arguments)
		{
		}

		public void Returns(Func<TResult> valueExpression)
		{
			this.valueFunc = valueExpression;
		}

		public void Returns(TResult value)
		{
			this.value = value;
		}

		public new ICall<TResult> Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		protected override IMethodReturnMessage GetReturnMessage(IMethodCallMessage call)
		{
			if (valueFunc != null)
				return new ReturnMessage(valueFunc(), null, 0, null, call);
			else
				return new ReturnMessage(value, null, 0, null, call);
		}
	}
}
