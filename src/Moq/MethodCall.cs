// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
	internal sealed partial class MethodCall : SetupWithOutParameterSupport
	{
		private Action<object[]> afterReturnCallback;
		private Action<object[]> callbackResponse;
		private LimitInvocationCountResponse limitInvocationCountResponse;
		private Condition condition;
		private string failMessage;
		private Flags flags;
		private Mock mock;
#if !NETCORE
		private string originalCallerFilePath;
		private int originalCallerLineNumber;
		private MethodBase originalCallerMember;
#endif
		private RaiseEventResponse raiseEventResponse;
		private Response returnOrThrowResponse;

		public MethodCall(Mock mock, Condition condition, LambdaExpression originalExpression, MethodInfo method, IReadOnlyList<Expression> arguments)
			: base(method, arguments, originalExpression)
		{
			this.condition = condition;
			this.flags = method.ReturnType != typeof(void) ? Flags.MethodIsNonVoid : 0;
			this.mock = mock;

			this.SetFileInfo();
		}

		public string FailMessage
		{
			get => this.failMessage;
		}

		public Mock Mock => this.mock;

		public override Condition Condition => this.condition;

		public override bool IsVerifiable => (this.flags & Flags.Verifiable) != 0;

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

		public override void Execute(Invocation invocation)
		{
			this.flags |= Flags.Invoked;

			this.limitInvocationCountResponse?.RespondTo(invocation);

			this.callbackResponse?.Invoke(invocation.Arguments);

			if ((this.flags & Flags.CallBase) != 0)
			{
				invocation.ReturnBase();
			}

			this.raiseEventResponse?.RespondTo(invocation);

			this.returnOrThrowResponse?.RespondTo(invocation);

			if ((this.flags & Flags.MethodIsNonVoid) != 0)
			{
				if (this.returnOrThrowResponse == null)
				{
					if (this.Mock.Behavior == MockBehavior.Strict)
					{
						throw MockException.ReturnValueRequired(invocation);
					}
					else
					{
						invocation.Return(this.Method.ReturnType.GetDefaultValue());
					}
				}

				this.afterReturnCallback?.Invoke(invocation.Arguments);
			}
		}

		public void SetCallBaseResponse()
		{
			if (this.Mock.TargetType.IsDelegate())
			{
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.CallBaseCannotBeUsedWithDelegateMocks));
			}

			if ((this.flags & Flags.MethodIsNonVoid) != 0)
			{
				this.returnOrThrowResponse = ReturnBaseResponse.Instance;
			}
			else
			{
				this.flags |= Flags.CallBase;
			}
		}

		public void SetCallbackResponse(Delegate callback)
		{
			if (this.returnOrThrowResponse != null)
			{
				if (callback is Action afterReturnCallbackWithoutArguments)
				{
					this.afterReturnCallback = delegate { afterReturnCallbackWithoutArguments(); };
				}
				else
				{
					this.afterReturnCallback = delegate (object[] args) { callback.InvokePreserveStack(args); };
				}
				return;
			}

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
				var expectedParamTypes = this.Method.GetParameterTypes();
				if (!callback.CompareParameterTypesTo(expectedParamTypes))
				{
					var actualParams = callback.GetMethodInfo().GetParameters();
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.InvalidCallbackParameterMismatch,
							string.Join(",", expectedParamTypes.Select(p => p.Name).ToArray()),
							string.Join(",", actualParams.Select(p => p.ParameterType.Name).ToArray())));
				}

				if (callback.GetMethodInfo().ReturnType != typeof(void))
				{
					throw new ArgumentException(Resources.InvalidCallbackNotADelegateWithReturnTypeVoid, nameof(callback));
				}

				this.callbackResponse = (object[] args) => callback.InvokePreserveStack(args);
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

		public void SetEagerReturnsResponse(object value)
		{
			Debug.Assert((this.flags & Flags.MethodIsNonVoid) != 0);
			Debug.Assert(this.returnOrThrowResponse == null);

			this.returnOrThrowResponse = new ReturnEagerValueResponse(value);
		}

		public void SetReturnsResponse(Delegate valueFactory)
		{
			Debug.Assert((this.flags & Flags.MethodIsNonVoid) != 0);
			Debug.Assert(this.returnOrThrowResponse == null);

			if (valueFactory == null)
			{
				// A `null` reference (instead of a valid delegate) is interpreted as the actual return value.
				// This is necessary because the compiler might have picked the unexpected overload for calls
				// like `Returns(null)`, or the user might have picked an overload like `Returns<T>(null)`,
				// and instead of in `Returns(TResult)`, we ended up in `Returns(Delegate)` or `Returns(Func)`,
				// which likely isn't what the user intended.
				// So here we do what we would've done in `Returns(TResult)`:
				this.returnOrThrowResponse = new ReturnEagerValueResponse(this.Method.ReturnType.GetDefaultValue());
			}
			else if (this.Method.ReturnType == typeof(Delegate))
			{
				// If `TResult` is `Delegate`, that is someone is setting up the return value of a method
				// that returns a `Delegate`, then we have arrived here because C# picked the wrong overload:
				// We don't want to invoke the passed delegate to get a return value; the passed delegate
				// already is the return value.
				this.returnOrThrowResponse = new ReturnEagerValueResponse(valueFactory);
			}
			else
			{
				ValidateCallback(valueFactory);
				this.returnOrThrowResponse = new ReturnLazyValueResponse(valueFactory);
			}

			void ValidateCallback(Delegate callback)
			{
				var callbackMethod = callback.GetMethodInfo();

				// validate number of parameters:

				var numberOfActualParameters = callbackMethod.GetParameters().Length;
				if (callbackMethod.IsStatic)
				{
					if (callbackMethod.IsExtensionMethod() || callback.Target != null)
					{
						numberOfActualParameters--;
					}
				}

				if (numberOfActualParameters > 0)
				{
					var numberOfExpectedParameters = this.Method.GetParameters().Length;
					if (numberOfActualParameters != numberOfExpectedParameters)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.CurrentCulture,
								Resources.InvalidCallbackParameterCountMismatch,
								numberOfExpectedParameters,
								numberOfActualParameters));
					}
				}

				// validate return type:

				var actualReturnType = callbackMethod.ReturnType;

				if (actualReturnType == typeof(void))
				{
					throw new ArgumentException(Resources.InvalidReturnsCallbackNotADelegateWithReturnType);
				}

				var expectedReturnType = this.Method.ReturnType;

				if (!expectedReturnType.IsAssignableFrom(actualReturnType))
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.InvalidCallbackReturnTypeMismatch,
							expectedReturnType,
							actualReturnType));
				}
			}
		}

		public void SetThrowExceptionResponse(Exception exception)
		{
			this.returnOrThrowResponse = new ThrowExceptionResponse(exception);
		}

		public override bool TryVerifyAll()
		{
			return (this.flags & Flags.Invoked) != 0;
		}

		public void Verifiable()
		{
			this.flags |= Flags.Verifiable;
		}

		public void Verifiable(string failMessage)
		{
			this.flags |= Flags.Verifiable;
			this.failMessage = failMessage;
		}

		public void AtMost(int count)
		{
			this.limitInvocationCountResponse = new LimitInvocationCountResponse(this, count);
		}

		public override string ToString()
		{
			var message = new StringBuilder();

			if (this.failMessage != null)
			{
				message.Append(this.failMessage).Append(": ");
			}

			message.Append(base.ToString());

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

		[Flags]
		private enum Flags : byte
		{
			CallBase = 1,
			Invoked = 2,
			MethodIsNonVoid = 4,
			Verifiable = 8,
		}

		private sealed class LimitInvocationCountResponse
		{
			private readonly MethodCall setup;
			private readonly int maxCount;
			private int count;

			public LimitInvocationCountResponse(MethodCall setup, int maxCount)
			{
				this.setup = setup;
				this.maxCount = maxCount;
				this.count = 0;
			}

			public void RespondTo(Invocation invocation)
			{
				++this.count;

				if (this.count > this.maxCount)
				{
					if (this.maxCount == 1)
					{
						throw MockException.MoreThanOneCall(this.setup, this.count);
					}
					else
					{
						throw MockException.MoreThanNCalls(this.setup, this.maxCount, this.count);
					}
				}
			}
		}

		private abstract class Response
		{
			protected Response()
			{
			}

			public abstract void RespondTo(Invocation invocation);
		}

		private sealed class ReturnBaseResponse : Response
		{
			public static readonly ReturnBaseResponse Instance = new ReturnBaseResponse();

			private ReturnBaseResponse()
			{
			}

			public override void RespondTo(Invocation invocation)
			{
				invocation.ReturnBase();
			}
		}

		private sealed class ReturnEagerValueResponse : Response
		{
			private readonly object value;

			public ReturnEagerValueResponse(object value)
			{
				this.value = value;
			}

			public override void RespondTo(Invocation invocation)
			{
				invocation.Return(this.value);
			}
		}

		private sealed class ReturnLazyValueResponse : Response
		{
			private readonly Delegate valueFactory;

			public ReturnLazyValueResponse(Delegate valueFactory)
			{
				this.valueFactory = valueFactory;
			}

			public override void RespondTo(Invocation invocation)
			{
				invocation.Return(this.valueFactory.CompareParameterTypesTo(Type.EmptyTypes)
					? valueFactory.InvokePreserveStack()                //we need this, for the user to be able to use parameterless methods
					: valueFactory.InvokePreserveStack(invocation.Arguments)); //will throw if parameters mismatch
			}
		}

		private sealed class ThrowExceptionResponse : Response
		{
			private readonly Exception exception;

			public ThrowExceptionResponse(Exception exception)
			{
				this.exception = exception;
			}

			public override void RespondTo(Invocation invocation)
			{
				throw this.exception;
			}
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
