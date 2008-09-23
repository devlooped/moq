//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the Moq Team nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal class SetterMethodCall<TProperty> : MethodCall, IExpectSetter<TProperty>
	{
		bool checkValue = false;
		TProperty value;

		public SetterMethodCall(Expression originalExpression, MethodInfo method)
			: base(originalExpression, method, new Expression[0])
		{
		}

		public SetterMethodCall(Expression originalExpression, MethodInfo method, TProperty value)
			: base(originalExpression, method, new Expression[0])
		{
			checkValue = true;
			this.value = value;
		}

		public override bool Matches(IInvocation call)
		{
			// Need to override default behavior as the arguments will be zero 
			// whereas the call arguments will be one: the property 
			// value to set.

			if (call.Method != method)
				return false;

			if (checkValue)
			{
				// If the ctor that received a value was used, 
				// we'll use it for comparison.
				return Object.Equals(value, call.Arguments[0]);
			}

			return true;
		}

		public ICallbackResult Callback(Action<TProperty> callback)
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
		bool isNever;
		MockedEvent mockEvent;
		Delegate mockEventArgsFunc;
		private int? expectedCallCount = null;

		public MethodCall(Expression originalExpression, MethodInfo method, params Expression[] arguments)
		{
			this.originalExpression = originalExpression;
			this.method = method;
			this.argumentMatchers = arguments.Select(expr => MatcherFactory.CreateMatcher(expr)).ToArray();
		}

		public bool IsVerifiable { get; set; }
		public bool Invoked { get; set; }
		public Expression ExpectExpression { get { return originalExpression; } }

		public IThrowsResult Throws(Exception exception)
		{
			this.exception = exception;
			return this;
		}

		public IThrowsResult Throws<TException>() 
			where TException : Exception, new()
		{
			this.exception = new TException();
			return this;
		}

		public ICallbackResult Callback(Action callback)
		{
			SetCallbackWithoutArguments(callback);
			return this;
		}

		public ICallbackResult Callback<T>(Action<T> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2>(Action<T1, T2> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			SetCallbackWithArguments(callback);
			return this;
		}

		protected virtual void SetCallbackWithoutArguments(Action callback)
		{
			this.callback = delegate { callback(); };
		}

		protected virtual void SetCallbackWithArguments(Delegate callback)
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


			if (isNever)
				throw new MockException(MockException.ExceptionReason.ExpectedNever,
					String.Format(Properties.Resources.ExpectedNever,
					call.Format()));


			if (expectedCallCount.HasValue && callCount > expectedCallCount)
				throw new MockException(MockException.ExceptionReason.MoreThanNCalls,
					String.Format(Properties.Resources.MoreThanNCalls, expectedCallCount,
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

		public void Never()
		{
			isNever = true;
		}



		public IVerifies AtMost( int callCount )
		{
			expectedCallCount = callCount;

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
