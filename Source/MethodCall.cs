using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;
using Moq.Language.Flow;
using Moq.Language;

namespace Moq
{
	internal class MethodCall : IProxyCall, IExpect
	{
		Expression originalExpression;
		MethodInfo method;
		Exception exception;
		Action<object[]> callback;
		IMatcher[] argumentMatchers;
		int callCount;
		bool isOnce;

		public MethodCall(Expression originalExpression, MethodInfo method, params Expression[] arguments)
		{
			this.originalExpression = originalExpression;
			this.method = method;
			this.argumentMatchers = arguments.Select(expr => MatcherFactory.CreateMatcher(expr)).ToArray();
		}

		public bool IsVerifiable { get; set; }
		public bool Invoked { get; set; }
		public Expression ExpectExpression { get { return originalExpression; } }

		public IOnceVerifies Throws(Exception exception)
		{
			this.exception = exception;
			return this;
		}

		public IThrowsOnceVerifies Callback(Action callback)
		{
			this.callback = delegate { callback(); };
			return this;
		}

		public IThrowsOnceVerifies Callback<T>(Action<T> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifies Callback<T1, T2>(Action<T1, T2> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifies Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifies Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		private void SetCallbackWithArguments(Delegate callback)
		{
			this.callback = delegate(object[] args) { callback.DynamicInvoke(args); };
		}

		public void Verifiable()
		{
			IsVerifiable = true;
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

			callCount++;

			if (isOnce && callCount > 1)
				throw new MockException(MockException.ExceptionReason.MoreThanOneCall,
					String.Format(Properties.Resources.MoreThanOneCall,
					call.Format()));
		}


		public IVerifies AtMostOnce()
		{
			isOnce = true;

			return this;
		}
	}
}
