using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal class MethodCall<TProperty> : MethodCall, IExpectSetter<TProperty>
	{
		public MethodCall(Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(originalExpression, method, arguments)
		{
		}

		public override bool Matches(IInvocation call)
		{
			// Need to override default behavior as the arguments will be zero 
			// whereas the call arguments will be one: the property 
			// value to set.
			return call.Method == method;
		}

		public IThrowsOnceVerifiesRaise Callback(Action<TProperty> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}
	}

	internal class MethodCall : IProxyCall, IExpect
	{
		protected MethodInfo method;
		Expression originalExpression;
		Exception exception;
		Action<object[]> callback;
		IMatcher[] argumentMatchers;
		int callCount;
		bool isOnce;
		MockedEvent mockEvent;
		Delegate mockEventArgsFunc;

		public MethodCall(Expression originalExpression, MethodInfo method, params Expression[] arguments)
		{
			this.originalExpression = originalExpression;
			this.method = method;
			this.argumentMatchers = arguments.Select(expr => MatcherFactory.CreateMatcher(expr)).ToArray();
		}

		public bool IsVerifiable { get; set; }
		public bool Invoked { get; set; }
		public Expression ExpectExpression { get { return originalExpression; } }

		public IOnceVerifiesRaise Throws(Exception exception)
		{
			this.exception = exception;
			return this;
		}

		public IThrowsOnceVerifiesRaise Callback(Action callback)
		{
			this.callback = delegate { callback(); };
			return this;
		}

		public IThrowsOnceVerifiesRaise Callback<T>(Action<T> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifiesRaise Callback<T1, T2>(Action<T1, T2> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifiesRaise Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public IThrowsOnceVerifiesRaise Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		protected void SetCallbackWithArguments(Delegate callback)
		{
			this.callback = delegate(object[] args) { callback.InvokePreserveStack(args); };
		}

		public void Verifiable()
		{
			IsVerifiable = true;
		}

		public virtual bool Matches(IInvocation call)
		{
			if (IsEqualMethodOrOverride(call) &&
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

		private bool IsEqualMethodOrOverride(IInvocation call)
		{
			return call.Method == method ||
				(call.Method.DeclaringType.IsClass &&
				call.Method.IsVirtual &&
				call.Method.GetBaseDefinition() == method);
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

			if (mockEvent != null)
			{
				var argsFuncType = mockEventArgsFunc.GetType();

				if (argsFuncType.IsGenericType &&
					argsFuncType.GetGenericArguments().Length == 1)
				{
					mockEvent.DoRaise((EventArgs)mockEventArgsFunc.InvokePreserveStack());
				}
				else
				{
					mockEvent.DoRaise((EventArgs)mockEventArgsFunc.InvokePreserveStack(call.Arguments));
				}
			}
		}

		public IVerifies AtMostOnce()
		{
			isOnce = true;

			return this;
		}

		public IVerifies Raises(MockedEvent eventHandler, EventArgs args)
		{
			Guard.ArgumentNotNull(args, "args");

			return RaisesImpl(eventHandler, (Func<EventArgs>)(() => args));
		}

		public IVerifies Raises(MockedEvent eventHandler, Func<EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T>(MockedEvent eventHandler, Func<T, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2>(MockedEvent eventHandler, Func<T1, T2, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2, T3>(MockedEvent eventHandler, Func<T1, T2, T3, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2, T3, T4>(MockedEvent eventHandler, Func<T1, T2, T3, T4, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises(MockedEvent eventHandler, Func<object[], EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		private IVerifies RaisesImpl(MockedEvent eventHandler, Delegate func)
		{
			Guard.ArgumentNotNull(eventHandler, "eventHandler");
			Guard.ArgumentNotNull(func, "func");

			mockEvent = eventHandler;
			mockEventArgsFunc = func;

			return this;
		}
	}
}
