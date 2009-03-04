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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal class MethodCall<TMock> : MethodCall, ISetup<TMock>
		where TMock : class
	{
		public MethodCall(Mock mock, Expression originalExpression, MethodInfo method, 
			params Expression[] arguments)
			: base(mock, originalExpression, method, arguments)
		{
		}

		public IVerifies Raises(Action<TMock> eventExpression, EventArgs args)
		{
			return Raises(eventExpression, () => args);
		}

		public IVerifies Raises(Action<TMock> eventExpression, Func<EventArgs> func)
		{
			return RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises<T1>(Action<TMock> eventExpression, Func<T1, EventArgs> func)
		{
			return RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises<T1, T2>(Action<TMock> eventExpression, Func<T1, T2, EventArgs> func)
		{
			return RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises<T1, T2, T3>(Action<TMock> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			return RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises<T1, T2, T3, T4>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			return RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises(Action<TMock> eventExpression, params object[] args)
		{
			return RaisesImpl(eventExpression, args);
		}
	}

	internal partial class MethodCall : IProxyCall, ICallbackResult, IVerifies, IThrowsResult
	{
		protected Mock mock;
		protected MethodInfo method;
		Expression originalExpression;
		Exception exception;
		Action<object[]> callback;
		List<IMatcher> argumentMatchers = new List<IMatcher>();
		int callCount;
		bool isOnce;
		MockedEvent mockEvent;
		Delegate mockEventArgsFunc;
		object[] mockEventArgsParams;
		int? expectedCallCount = null;
		List<KeyValuePair<int, object>> outValues = new List<KeyValuePair<int, object>>();

		// Where the setup was performed.
		public string FileName { get; private set; }
		public int FileLine { get; private set; }
		public MethodBase TestMethod { get; private set; }

		public string FailMessage { get; set; }
		public bool IsVerifiable { get; set; }
		public bool IsNever { get; set; }
		public bool Invoked { get; set; }
		public Expression SetupExpression { get { return originalExpression; } }

		public MethodCall(Mock mock, Expression originalExpression, MethodInfo method, params Expression[] arguments)
		{
			this.mock = mock;
			this.originalExpression = originalExpression;
			this.method = method;

			var parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i];
				var argument = arguments[i];
				if (parameter.IsOut)
				{
					//changed to eager-evaluate.
					//outValues.Add(new KeyValuePair<int, Expression>(i, argument));
					var value = argument.PartialEval();
					if (value.NodeType == ExpressionType.Constant)
						outValues.Add(new KeyValuePair<int, object>(i, ((ConstantExpression)value).Value));
					else
						throw new NotSupportedException("Out expression must evaluate to a constant value.");
				}
				else if (parameter.ParameterType.IsByRef)
				{
					var value = argument.PartialEval();
					if (value.NodeType == ExpressionType.Constant)
						argumentMatchers.Add(new RefMatcher(((ConstantExpression)value).Value));
					else
						throw new NotSupportedException("Ref expression must evaluate to a constant value.");
				}
				else if (parameter.GetCustomAttribute<ParamArrayAttribute>(true) != null)
				{
					IMatcher matcher = new ParamArrayMatcher();
					matcher.Initialize(argument);
					argumentMatchers.Add(matcher);
				}
				else
				{
					argumentMatchers.Add(MatcherFactory.CreateMatcher(argument));
				}
			}

			SetFileInfo();
		}

		[Conditional("DESKTOP")]
		private void SetFileInfo()
		{
#if !SILVERLIGHT
			var thisMethod = MethodBase.GetCurrentMethod();
			var stack = new StackTrace(true);
			var index = 0;

			// Move 'till our own frame first
			while (stack.GetFrame(index).GetMethod() != thisMethod 
				&& index <= stack.FrameCount)
			{
				index++;
			}

			// Move 'till we're at the entry point 
			// into Moq API
			// Move 'till our own frame first
			while (stack.GetFrame(index).GetMethod().DeclaringType.Namespace.StartsWith("Moq")
				&& index <= stack.FrameCount)
			{
				index++;
			}

			if (index < stack.FrameCount)
			{
				var frame = stack.GetFrame(index);
				FileLine = frame.GetFileLineNumber();
				FileName = Path.GetFileName(frame.GetFileName());
				TestMethod = frame.GetMethod();
			}
#endif
		}

		public void SetOutParameters(IInvocation call)
		{
			foreach (var item in outValues)
			{
				// it's already evaluated here
				// TODO: refactor so that we 
				call.SetArgumentValue(item.Key, item.Value);
			}
		}

		public virtual bool Matches(IInvocation call)
		{
			var args = new List<object>();
			var parameters = call.Method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].IsOut)
					args.Add(call.Arguments[i]);
			}

			if (IsEqualMethodOrOverride(call) &&
				argumentMatchers.Count == args.Count)
			{
				for (int i = 0; i < argumentMatchers.Count; i++)
				{
					if (!argumentMatchers[i].Matches(args[i]))
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
					String.Format(CultureInfo.CurrentCulture, 
					Properties.Resources.MoreThanOneCall,
					call.Format()));


			if (IsNever)
				throw new MockException(MockException.ExceptionReason.SetupNever,
					String.Format(CultureInfo.CurrentCulture, 
					Properties.Resources.SetupNever,
					call.Format()));


			if (expectedCallCount.HasValue && callCount > expectedCallCount)
				throw new MockException(MockException.ExceptionReason.MoreThanNCalls,
					String.Format(CultureInfo.CurrentCulture, 
					Properties.Resources.MoreThanNCalls, expectedCallCount,
					call.Format()));


			if (mockEvent != null)
			{
				if (mockEventArgsParams != null)
				{
					mockEvent.DoRaise(mockEventArgsParams);
				}
				else
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
		}

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
			var expectedParams = this.method.GetParameters();
			var actualParams = callback.Method.GetParameters();

			if (expectedParams.Length == actualParams.Length)
			{
				for (int i = 0; i < expectedParams.Length; i++)
				{
					if (!actualParams[i].ParameterType.IsAssignableFrom(expectedParams[i].ParameterType))
						ThrowParameterMismatch(expectedParams, actualParams);
				}
			}
			else
			{
				ThrowParameterMismatch(expectedParams, actualParams);
			}

			this.callback = delegate(object[] args) { callback.InvokePreserveStack(args); };
		}

		private static void ThrowParameterMismatch(ParameterInfo[] expected, ParameterInfo[] actual)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
				"Invalid callback. Setup on method with parameters ({0}) cannot invoke callback with parameters ({1}).",
				String.Join(",", expected.Select(p => p.ParameterType.Name).ToArray()),
				String.Join(",", actual.Select(p => p.ParameterType.Name).ToArray())
			));
		}

		public void Verifiable()
		{
			IsVerifiable = true;
		}

		public void Verifiable(string failMessage)
		{
			IsVerifiable = true;
			FailMessage = failMessage;
		}

		private bool IsEqualMethodOrOverride(IInvocation call)
		{
			if (call.Method == method)
				return true;

			if ((call.Method == method) ||
				(call.Method.DeclaringType.IsClass &&
				call.Method.IsVirtual &&
				method.DeclaringType.IsClass &&
				method.IsVirtual &&
				(call.Method.GetBaseDefinition() == method.GetBaseDefinition())))
			{
				if (method.IsGenericMethod)
				{
					var callMethodGenericArguments = call.Method.GetGenericArguments();
					var methodGenericArguments = method.GetGenericArguments();

					for (int argumentIndex = 0; argumentIndex < methodGenericArguments.Length; argumentIndex++)
						if (!methodGenericArguments[argumentIndex].Equals(
							callMethodGenericArguments[argumentIndex]))
							return false;
				}

				return true;
			}

			return false;
		}

		public IVerifies AtMostOnce()
		{
			isOnce = true;

			return this;
		}

		public void Never()
		{
			IsNever = true;
		}

		public IVerifies AtMost(int callCount)
		{
			expectedCallCount = callCount;

			return this;
		}

		protected IVerifies RaisesImpl<TMock>(Action<TMock> eventExpression, Delegate func)
			where TMock : class
		{
			var ev = eventExpression.GetEvent((TMock)mock.Object);

			mockEvent = new MockedEvent(mock);
			mockEvent.Event = ev;
			mockEventArgsFunc = func;

			return this;
		}

		protected IVerifies RaisesImpl<TMock>(Action<TMock> eventExpression, params object[] args)
			where TMock : class
		{
			var ev = eventExpression.GetEvent((TMock)mock.Object);

			mockEvent = new MockedEvent(mock);
			mockEvent.Event = ev;
			mockEventArgsParams = args;

			return this;
		}
	}
}
