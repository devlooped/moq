//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Text;
using Moq.Language;
using Moq.Language.Flow;
using Moq.Proxy;

namespace Moq
{
	internal partial class MethodCall<TMock> : MethodCall, ISetup<TMock>
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

		public IVerifies Raises(Action<TMock> eventExpression, params object[] args)
		{
			return RaisesImpl(eventExpression, args);
		}
	}

	internal partial class MethodCall : IProxyCall, ICallbackResult, IVerifies, IThrowsResult
	{
		// Internal for AsMockExtensions
		protected internal Mock mock;
		protected MethodInfo method;
		private Expression originalExpression;
		private Exception exception;
		private Action<object[]> callback;
		private List<IMatcher> argumentMatchers = new List<IMatcher>();
		private bool isOnce;
		private EventInfo mockEvent;
		private Delegate mockEventArgsFunc;
		private object[] mockEventArgsParams;
		private int? expectedCallCount = null;
		private List<KeyValuePair<int, object>> outValues = new List<KeyValuePair<int, object>>();

		// Where the setup was performed.
		public string FileName { get; private set; }
		public int FileLine { get; private set; }
		public MethodBase TestMethod { get; private set; }

		public string FailMessage { get; set; }
		public bool IsVerifiable { get; set; }
		public bool Invoked { get; set; }

		public Expression SetupExpression
		{
			get { return originalExpression; }
		}

		public int CallCount { get; private set; }

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
					var value = argument.PartialEval();
					if (value.NodeType != ExpressionType.Constant)
					{
						throw new NotSupportedException("Out expression must evaluate to a constant value.");
					}

					outValues.Add(new KeyValuePair<int, object>(i, ((ConstantExpression)value).Value));
				}
				else if (parameter.ParameterType.IsByRef)
				{
					var value = argument.PartialEval();
					if (value.NodeType != ExpressionType.Constant)
					{
						throw new NotSupportedException("Ref expression must evaluate to a constant value.");
					}

					argumentMatchers.Add(new RefMatcher(((ConstantExpression)value).Value));
				}
				else
				{
					var isParamArray = parameter.GetCustomAttribute<ParamArrayAttribute>(true) != null;
					argumentMatchers.Add(MatcherFactory.CreateMatcher(argument, isParamArray));
				}
			}

			this.SetFileInfo();
		}

		[Conditional("DESKTOP")]
		private void SetFileInfo()
		{
#if !SILVERLIGHT
			try
			{
				var thisMethod = MethodBase.GetCurrentMethod();
				var stack = new StackTrace(true);
				var index = 0;

				// Move 'till our own frame first
				while (stack.GetFrame(index).GetMethod() != thisMethod && index <= stack.FrameCount)
				{
					index++;
				}

				// Move 'till we're at the entry point 
				// into Moq API
				var mockAssembly = Assembly.GetExecutingAssembly();
				while (stack.GetFrame(index).GetMethod().DeclaringType.Assembly == mockAssembly &&
					index <= stack.FrameCount)
				{
					index++;
				}

				if (index < stack.FrameCount)
				{
					var frame = stack.GetFrame(index);
					this.FileLine = frame.GetFileLineNumber();
					this.FileName = Path.GetFileName(frame.GetFileName());
					TestMethod = frame.GetMethod();
				}

			}
			catch
			{
				// Must NEVER fail, as this is a nice-to-have feature only.
			}
#endif
		}

		public void SetOutParameters(ICallContext call)
		{
			foreach (var item in outValues)
			{
				// it's already evaluated here
				// TODO: refactor so that we 
				call.SetArgumentValue(item.Key, item.Value);
			}
		}

		public virtual bool Matches(ICallContext call)
		{
			var parameters = call.Method.GetParameters();
			var args = new List<object>();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].IsOut)
				{
					args.Add(call.Arguments[i]);
				}
			}

			if (argumentMatchers.Count == args.Count && this.IsEqualMethodOrOverride(call))
			{
				for (int i = 0; i < argumentMatchers.Count; i++)
				{
					if (!argumentMatchers[i].Matches(args[i]))
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public virtual void Execute(ICallContext call)
		{
			this.Invoked = true;

			if (callback != null)
				callback(call.Arguments);

			if (exception != null)
				throw exception;

			this.CallCount++;

			if (this.isOnce && this.CallCount > 1)
			{
				throw new MockException(
					MockException.ExceptionReason.MoreThanOneCall,
					Times.Once().GetExceptionMessage(FailMessage, SetupExpression.ToStringFixed(), this.CallCount));
			}

			if (expectedCallCount.HasValue && this.CallCount > expectedCallCount)
			{
				throw new MockException(
					MockException.ExceptionReason.MoreThanNCalls,
					Times.AtMost(expectedCallCount.Value).GetExceptionMessage(FailMessage, SetupExpression.ToStringFixed(), CallCount));
			}

			if (this.mockEvent != null)
			{
				if (mockEventArgsParams != null)
				{
					this.mock.DoRaise(this.mockEvent, mockEventArgsParams);
				}
				else
				{
					var argsFuncType = mockEventArgsFunc.GetType();
					if (argsFuncType.IsGenericType && argsFuncType.GetGenericArguments().Length == 1)
					{
						this.mock.DoRaise(this.mockEvent, (EventArgs)mockEventArgsFunc.InvokePreserveStack());
					}
					else
					{
						this.mock.DoRaise(this.mockEvent, (EventArgs)mockEventArgsFunc.InvokePreserveStack(call.Arguments));
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
			throw new ArgumentException(string.Format(
				CultureInfo.CurrentCulture,
				"Invalid callback. Setup on method with parameters ({0}) cannot invoke callback with parameters ({1}).",
				string.Join(",", expected.Select(p => p.ParameterType.Name).ToArray()),
				string.Join(",", actual.Select(p => p.ParameterType.Name).ToArray())
			));
		}

		public void Verifiable()
		{
			this.IsVerifiable = true;
		}

		public void Verifiable(string failMessage)
		{
			this.IsVerifiable = true;
			this.FailMessage = failMessage;
		}

		private bool IsEqualMethodOrOverride(ICallContext call)
		{
			if (call.Method == this.method)
			{
				return true;
			}

			if (this.method.DeclaringType.IsAssignableFrom(call.Method.DeclaringType))
			{
				if (!this.method.Name.Equals(call.Method.Name, StringComparison.Ordinal) ||
					this.method.ReturnType != call.Method.ReturnType ||
					!call.Method.GetParameterTypes().SequenceEqual(this.method.GetParameterTypes()))
				{
					return false;
				}

				if (method.IsGenericMethod && !call.Method.GetGenericArguments().SequenceEqual(method.GetGenericArguments()))
				{
					return false;
				}

				return true;
			}

			return false;
		}

		public IVerifies AtMostOnce()
		{
			this.isOnce = true;
			return this;
		}

		public IVerifies AtMost(int callCount)
		{
			this.expectedCallCount = callCount;
			return this;
		}

		protected IVerifies RaisesImpl<TMock>(Action<TMock> eventExpression, Delegate func)
			where TMock : class
		{
			this.mockEvent = eventExpression.GetEvent((TMock)mock.Object);
			this.mockEventArgsFunc = func;
			return this;
		}

		protected IVerifies RaisesImpl<TMock>(Action<TMock> eventExpression, params object[] args)
			where TMock : class
		{
			this.mockEvent = eventExpression.GetEvent((TMock)mock.Object);
			this.mockEventArgsParams = args;
			return this;
		}

		public override string ToString()
		{
			var message = new StringBuilder();

			if (this.FailMessage != null)
			{
				message.Append(this.FailMessage).Append(": ");
			}

			var lambda = SetupExpression.PartialMatcherAwareEval().ToLambda();
			var targetTypeName = lambda.Parameters[0].Type.Name;

			message.Append(targetTypeName).Append(" ").Append(lambda.ToStringFixed());

			if (TestMethod != null && FileName != null && FileLine != 0)
			{
				message.AppendFormat(
					" ({0}() in {1}: line {2})",
					TestMethod.Name,
					FileName,
					FileLine);
			}

			return message.ToString().Trim();
		}
	}
}
