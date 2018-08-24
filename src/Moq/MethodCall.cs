//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Moq.Properties;

namespace Moq
{
	internal partial class MethodCall<TMock> : MethodCall
		where TMock : class
	{
		public MethodCall(Mock mock, Condition condition, LambdaExpression originalExpression, MethodInfo method,
			IReadOnlyList<Expression> arguments)
			: base(mock, condition, originalExpression, method, arguments)
		{
		}
	}

	internal partial class MethodCall
	{
		private Action<object[]> callbackResponse;
		private int callCount;
		private Condition condition;
		private InvocationShape expectation;
		private int? expectedMaxCallCount;
		private string failMessage;
		private Mock mock;
#if !NETCORE
		private string originalCallerFilePath;
		private int originalCallerLineNumber;
		private MethodBase originalCallerMember;
#endif
		private LambdaExpression originalExpression;
		private List<KeyValuePair<int, object>> outValues;
		private RaiseEventResponse raiseEventResponse;
		private Exception throwExceptionResponse;
		private bool verifiable;

		/// <remarks>
		///   Only use this constructor when you know that the specified <paramref name="method"/> has no `out` parameters,
		///   and when you want to avoid the <see cref="MatcherFactory"/>-related overhead of the other constructor overload.
		/// </remarks>
		public MethodCall(Mock mock, Condition condition, LambdaExpression originalExpression, MethodInfo method, IMatcher[] argumentMatchers)
		{
			this.condition = condition;
			this.expectation = new InvocationShape(method, argumentMatchers);
			this.mock = mock;
			this.originalExpression = originalExpression;
			this.outValues = null;
		}

		public MethodCall(Mock mock, Condition condition, LambdaExpression originalExpression, MethodInfo method, IReadOnlyList<Expression> arguments)
		{
			this.condition = condition;
			this.expectation = new InvocationShape(method, arguments);
			this.mock = mock;
			this.originalExpression = originalExpression;
			this.outValues = GetOutValues(arguments, method.GetParameters());

			this.SetFileInfo();
		}

		private static List<KeyValuePair<int, object>> GetOutValues(IReadOnlyList<Expression> arguments, ParameterInfo[] parameters)
		{
			List<KeyValuePair<int, object>> outValues = null;
			for (int i = 0, n = parameters.Length; i < n; ++i)
			{
				var parameter = parameters[i];
				if (parameter.ParameterType.IsByRef)
				{
					if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out)
					{
						var constant = arguments[i].PartialEval() as ConstantExpression;
						if (constant == null)
						{
							throw new NotSupportedException(Resources.OutExpressionMustBeConstantValue);
						}

						if (outValues == null)
						{
							outValues = new List<KeyValuePair<int, object>>();
						}

						outValues.Add(new KeyValuePair<int, object>(i, constant.Value));
					}
				}
			}
			return outValues;
		}

		public string FailMessage
		{
			get => this.failMessage;
			set => this.failMessage = value;
		}

		public MethodInfo Method => this.expectation.Method;

		public Mock Mock => this.mock;

		public bool IsConditional => this.condition != null;

		public bool IsVerifiable => this.verifiable;

		public bool Invoked => this.callCount > 0;

		public LambdaExpression SetupExpression => this.originalExpression;

		[Conditional("DESKTOP")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void SetFileInfo()
		{
#if !NETCORE
			if ((this.mock.Switches & Switches.CollectDiagnosticFileInfoForSetups) == 0)
			{
				return;
			}

			try
			{
				var thisMethod = MethodBase.GetCurrentMethod();
				var mockAssembly = Assembly.GetExecutingAssembly();
				// Move 'till we're at the entry point into Moq API
				var frame = new StackTrace(true)
					.GetFrames()
					.SkipWhile(f => f.GetMethod() != thisMethod)
					.SkipWhile(f => f.GetMethod().DeclaringType == null || f.GetMethod().DeclaringType.Assembly == mockAssembly)
					.FirstOrDefault();

				if (frame != null)
				{
					this.originalCallerLineNumber = frame.GetFileLineNumber();
					this.originalCallerFilePath = Path.GetFileName(frame.GetFileName());
					this.originalCallerMember = frame.GetMethod();
				}
			}
			catch
			{
				// Must NEVER fail, as this is a nice-to-have feature only.
			}
#endif
		}

		public void SetOutParameters(Invocation invocation)
		{
			if (this.outValues == null)
			{
				return;
			}

			foreach (var item in this.outValues)
			{
				invocation.Arguments[item.Key] = item.Value;
			}
		}

		public bool Matches(Invocation invocation)
		{
			return this.expectation.IsMatch(invocation) && (condition == null || condition.IsTrue);
		}

		public void EvaluatedSuccessfully()
		{
			if (condition != null)
				condition.EvaluatedSuccessfully();
		}

		public virtual void Execute(Invocation invocation)
		{
			++this.callCount;

			if (expectedMaxCallCount.HasValue && this.callCount > expectedMaxCallCount)
			{
				if (expectedMaxCallCount == 1)
				{
					throw MockException.MoreThanOneCall(this, this.callCount);
				}
				else
				{
					throw MockException.MoreThanNCalls(this, this.expectedMaxCallCount.Value, this.callCount);
				}
			}

			this.callbackResponse?.Invoke(invocation.Arguments);

			this.raiseEventResponse?.RespondTo(invocation);

			if (this.throwExceptionResponse != null)
			{
				throw this.throwExceptionResponse;
			}
		}

		public virtual void SetCallbackResponse(Delegate callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			if (callback is Action callbackWithoutArguments)
			{
				this.callbackResponse = (object[] args) => callbackWithoutArguments();
			}
			else
			{
				var expectedParams = this.Method.GetParameters();
				var actualParams = callback.GetMethodInfo().GetParameters();

				if (!callback.HasCompatibleParameterList(expectedParams))
				{
					ThrowParameterMismatch(expectedParams, actualParams);
				}

				if (callback.GetMethodInfo().ReturnType != typeof(void))
				{
					throw new ArgumentException(Resources.InvalidCallbackNotADelegateWithReturnTypeVoid, nameof(callback));
				}

				this.callbackResponse = (object[] args) => callback.InvokePreserveStack(args);
			}

			void ThrowParameterMismatch(ParameterInfo[] expected, ParameterInfo[] actual)
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.InvalidCallbackParameterMismatch,
						string.Join(",", expected.Select(p => p.ParameterType.Name).ToArray()),
						string.Join(",", actual.Select(p => p.ParameterType.Name).ToArray())));
			}
		}

		public void SetRaiseEventResponse<TMock>(Action<TMock> eventExpression, Delegate func)
			where TMock : class
		{
			var (ev, _) = eventExpression.GetEventWithTarget((TMock)mock.Object);
			if (ev != null)
			{
				this.raiseEventResponse = new RaiseEventResponse(this.mock, ev, func, null);
			}
			else
			{
				this.raiseEventResponse = null;
			}
		}

		public void SetRaiseEventResponse<TMock>(Action<TMock> eventExpression, params object[] args)
			where TMock : class
		{
			var (ev, _) = eventExpression.GetEventWithTarget((TMock)mock.Object);
			if (ev != null)
			{
				this.raiseEventResponse = new RaiseEventResponse(this.mock, ev, null, args);
			}
			else
			{
				this.raiseEventResponse = null;
			}
		}

		public void SetThrowExceptionResponse(Exception exception)
		{
			this.throwExceptionResponse = exception;
		}

		public void Verifiable()
		{
			this.verifiable = true;
		}

		public void Verifiable(string failMessage)
		{
			this.verifiable = true;
			this.failMessage = failMessage;
		}

		public void AtMostOnce() => this.AtMost(1);

		public void AtMost(int callCount)
		{
			this.expectedMaxCallCount = callCount;
		}

		public override string ToString()
		{
			var message = new StringBuilder();

			if (this.failMessage != null)
			{
				message.Append(this.failMessage).Append(": ");
			}

			var lambda = this.originalExpression.PartialMatcherAwareEval();
			var targetTypeName = lambda.Parameters[0].Type.Name;

			message.Append(targetTypeName).Append(" ").Append(lambda.ToStringFixed());

#if !NETCORE
			if (this.originalCallerMember != null && this.originalCallerFilePath != null && this.originalCallerLineNumber != 0)
			{
				message.AppendFormat(
					" ({0}() in {1}: line {2})",
					this.originalCallerMember.Name,
					this.originalCallerFilePath,
					this.originalCallerLineNumber);
			}
#endif

			return message.ToString().Trim();
		}

		public string Format()
		{
			var builder = new StringBuilder();
			builder.Append(this.originalExpression.PartialMatcherAwareEval().ToStringFixed());

			if (this.expectedMaxCallCount != null)
			{
				if (this.expectedMaxCallCount == 1)
				{
					builder.Append(", Times.AtMostOnce()");
				}
				else
				{
					builder.Append(", Times.AtMost(");
					builder.Append(this.expectedMaxCallCount.Value);
					builder.Append(")");
				}
			}

			return builder.ToString();
		}

		private sealed class RaiseEventResponse
		{
			private Mock mock;
			private EventInfo @event;
			private Delegate eventArgsFunc;
			private object[] eventArgsParams;

			public RaiseEventResponse(Mock mock, EventInfo @event, Delegate eventArgsFunc, object[] eventArgsParams)
			{
				Debug.Assert(mock != null);
				Debug.Assert(@event != null);
				Debug.Assert(eventArgsFunc != null ^ eventArgsParams != null);

				this.mock = mock;
				this.@event = @event;
				this.eventArgsFunc = eventArgsFunc;
				this.eventArgsParams = eventArgsParams;
			}

			public void RespondTo(Invocation invocation)
			{
				if (this.eventArgsParams != null)
				{
					this.mock.DoRaise(this.@event, this.eventArgsParams);
				}
				else
				{
					var argsFuncType = this.eventArgsFunc.GetType();
					if (argsFuncType.GetTypeInfo().IsGenericType && argsFuncType.GetGenericArguments().Length == 1)
					{
						this.mock.DoRaise(this.@event, (EventArgs)this.eventArgsFunc.InvokePreserveStack());
					}
					else
					{
						this.mock.DoRaise(this.@event, (EventArgs)this.eventArgsFunc.InvokePreserveStack(invocation.Arguments));
					}
				}
			}
		}
	}
}
