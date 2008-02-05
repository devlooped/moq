using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;

namespace Moq
{
	internal class MethodCall : ICall, IProxyCall
	{
		Expression originalExpression;
		MethodInfo method;
		Exception exception;
		Action<object[]> callback;
		IMatcher[] argumentMatchers;

		public MethodCall(Expression originalExpression, MethodInfo method, params Expression[] arguments)
		{
			this.originalExpression = originalExpression;
			this.method = method;
			this.argumentMatchers = arguments.Select(expr => MatcherFactory.CreateMatcher(expr)).ToArray();
		}

		public bool IsVerifiable { get; set; }
		public bool Invoked { get; set; }
		public Expression ExpectExpression { get { return originalExpression; } }

		public void Throws(Exception exception)
		{
			this.exception = exception;
		}

		public ICall Callback(Action callback)
		{
			this.callback = delegate { callback(); };
			return this;
		}

		public ICall Callback<T>(Action<T> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICall Callback<T1, T2>(Action<T1, T2> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICall Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICall Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		private void SetCallbackWithArguments(Delegate callback)
		{
			this.callback = delegate(object[] args) { callback.DynamicInvoke(args); };
		}

		public ICall Verifiable()
		{
			IsVerifiable = true;

			return this;
		}

		public bool Matches(IInvocation call)
		{
			if (call.Method == method &&
				argumentMatchers.Length == call.Arguments.Length)
			{
				for (int i = 0; i < argumentMatchers.Length; i++)
				{
					if (!argumentMatchers[i].Matches(call.Arguments[i]))
						return false;
				}

				return true;
			}

			return false;
		}

		public virtual void Execute(IInvocation call)
		{
			Invoked = true;

			if (callback != null)
				callback(call.Arguments);

			if (exception != null)
				throw exception;
		}
	}
}
