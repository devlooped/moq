using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Moq
{
	internal class MethodCall : ICall, IProxyCall
	{
		MethodInfo method;
		Exception exception;
		Action callback;
		IMatcher[] argumentMatchers;

		public MethodCall(MethodInfo method, params Expression[] arguments)
		{
			this.method = method;
			argumentMatchers = (from expr in arguments
							   select MatcherFactory.CreateMatcher(expr)).ToArray();
		}

		public void Throws(Exception exception)
		{
			this.exception = exception;
		}

		public ICall Callback(Action callback)
		{
			this.callback = callback;
			return this;
		}

		public bool Matches(IMethodCallMessage call)
		{
			if (call.MethodBase == method &&
				argumentMatchers.Length == call.ArgCount)
			{
				for (int i = 0; i < argumentMatchers.Length; i++)
				{
					if (!argumentMatchers[i].Matches(call.Args[i]))
						return false;
				}

				return true;
			}

			return false;
		}

		public IMethodReturnMessage Execute(IMethodCallMessage call)
		{
			if (callback != null)
				callback();

			if (exception != null)
				return new ReturnMessage(exception, call);

			return GetReturnMessage(call);
		}

		protected virtual IMethodReturnMessage GetReturnMessage(IMethodCallMessage call)
		{
			return new ReturnMessage(null, null, 0, null, call);
		}
	}
}
