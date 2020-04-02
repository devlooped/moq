// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
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
		private Response afterReturnCallbackResponse;
		private Response callbackResponse;
		private LimitInvocationCountResponse limitInvocationCountResponse;
		private Condition condition;
		private string failMessage;
		private Flags flags;
		private RaiseEventResponse raiseEventResponse;
		private Response returnOrThrowResponse;

		private string declarationSite;

		public MethodCall(FluentSetup fluentSetup, Mock mock, Condition condition, InvocationShape expectation)
			: base(fluentSetup, mock, expectation)
		{
			this.condition = condition;
			this.flags = expectation.Method.ReturnType != typeof(void) ? Flags.MethodIsNonVoid : 0;

			if ((mock.Switches & Switches.CollectDiagnosticFileInfoForSetups) != 0)
			{
				this.declarationSite = GetUserCodeCallSite();
			}
		}

		public string FailMessage
		{
			get => this.failMessage;
		}

		public override Condition Condition => this.condition;

		private static string GetUserCodeCallSite()
		{
			try
			{
				var thisMethod = MethodBase.GetCurrentMethod();
				var mockAssembly = Assembly.GetExecutingAssembly();
				var frame = new StackTrace(true)
					.GetFrames()
					.SkipWhile(f => f.GetMethod() != thisMethod)
					.SkipWhile(f => f.GetMethod().DeclaringType == null || f.GetMethod().DeclaringType.Assembly == mockAssembly)
					.FirstOrDefault();
				var member = frame?.GetMethod();
				if (member != null)
				{
					var declaredAt = new StringBuilder();
					declaredAt.AppendNameOf(member.DeclaringType).Append('.').AppendNameOf(member, false);
					var fileName = Path.GetFileName(frame.GetFileName());
					if (fileName != null)
					{
						declaredAt.Append(" in ").Append(fileName);
						var lineNumber = frame.GetFileLineNumber();
						if (lineNumber != 0)
						{
							declaredAt.Append(": line ").Append(lineNumber);
						}
					}
					return declaredAt.ToString();
				}
			}
			catch
			{
				// Must NEVER fail, as this is a nice-to-have feature only.
			}

			return null;
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			this.limitInvocationCountResponse?.RespondTo(invocation);

			this.callbackResponse?.RespondTo(invocation);

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
						// Instead of duplicating the entirety of `Return`'s implementation,
						// let's just call it here. This is permissible only if the inter-
						// ception pipeline will terminate right away (otherwise `Return`
						// might be executed a second time).
						Return.Handle(invocation, this.Mock);
					}
				}

				this.afterReturnCallbackResponse?.RespondTo(invocation);
			}
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			if (this.returnOrThrowResponse is ReturnEagerValueResponse revs)
			{
				returnValue = revs.Value;
				return true;
			}
			else
			{
				returnValue = default;
				return false;
			}
		}

		public void SetCallBaseResponse()
		{
			if (this.Mock.TargetType.IsDelegateType())
			{
				throw new NotSupportedException(Resources.CallBaseCannotBeUsedWithDelegateMocks);
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
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			ref Response response = ref this.returnOrThrowResponse == null ? ref this.callbackResponse
			                                                               : ref this.afterReturnCallbackResponse;

			if (callback is Action callbackWithoutArguments)
			{
				response = new CallbackResponse(args => callbackWithoutArguments());
			}
			else if (callback.GetType() == typeof(Action<IInvocation>))
			{
				// NOTE: Do NOT rewrite the above condition as `callback is Action<IInvocation>`,
				// because this will also yield true if `callback` is a `Action<object>` and thus
				// break existing uses of `(object arg) => ...` callbacks!
				response = new InvocationCallbackResponse((Action<IInvocation>)callback);
			}
			else
			{
				var expectedParamTypes = this.Method.GetParameterTypes();
				if (!callback.CompareParameterTypesTo(expectedParamTypes))
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.InvalidCallbackParameterMismatch,
							this.Method.GetParameterTypeList(),
							callback.GetMethodInfo().GetParameterTypeList()));
				}

				if (callback.GetMethodInfo().ReturnType != typeof(void))
				{
					throw new ArgumentException(Resources.InvalidCallbackNotADelegateWithReturnTypeVoid, nameof(callback));
				}

				response = new CallbackResponse(args => callback.InvokePreserveStack(args));
			}
		}

		public void SetFailMessage(string failMessage)
		{
			this.failMessage = failMessage;
		}

		public void SetRaiseEventResponse<TMock>(Action<TMock> eventExpression, Delegate func)
			where TMock : class
		{
			Guard.NotNull(eventExpression, nameof(eventExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(eventExpression, this.Mock.ConstructorArguments);

			// TODO: validate that expression is for event subscription or unsubscription

			this.raiseEventResponse = new RaiseEventResponse(this.Mock, expression, func, null);
		}

		public void SetRaiseEventResponse<TMock>(Action<TMock> eventExpression, params object[] args)
			where TMock : class
		{
			Guard.NotNull(eventExpression, nameof(eventExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(eventExpression, this.Mock.ConstructorArguments);

			// TODO: validate that expression is for event subscription or unsubscription

			this.raiseEventResponse = new RaiseEventResponse(this.Mock, expression, null, args);
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
			else if (valueFactory.GetType() == typeof(Func<IInvocation, object>))
			{
				// NOTE: Do NOT rewrite the above condition as `valueFactory is Func<IInvocation, object>`,
				// for similar reasons as noted above in `SetCallbackResponse`. We want to test for this
				// particular delegate type and no others that may happen to be implicitly convertible to it!
				this.returnOrThrowResponse = new ReturnInvocationLazyValueResponse((Func<IInvocation, object>)valueFactory);
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
					// TODO: If the return type is a matcher, does the callback's return type need to be matched against it?
					if (typeof(ITypeMatcher).IsAssignableFrom(expectedReturnType) == false)
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
		}

		public void SetThrowExceptionResponse(Exception exception)
		{
			this.returnOrThrowResponse = new ThrowExceptionResponse(exception);
		}

		protected override void ResetCore()
		{
			this.limitInvocationCountResponse?.Reset();
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

			if (this.declarationSite != null)
			{
				message.Append(" (").Append(this.declarationSite).Append(')');
			}

			return message.ToString().Trim();
		}

		[Flags]
		private enum Flags : byte
		{
			CallBase = 1,
			MethodIsNonVoid = 2,
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

			public void Reset()
			{
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

		private sealed class CallbackResponse : Response
		{
			private readonly Action<object[]> callback;

			public CallbackResponse(Action<object[]> callback)
			{
				Debug.Assert(callback != null);

				this.callback = callback;
			}

			public override void RespondTo(Invocation invocation)
			{
				this.callback.Invoke(invocation.Arguments);
			}
		}

		private sealed class InvocationCallbackResponse : Response
		{
			private readonly Action<IInvocation> callback;

			public InvocationCallbackResponse(Action<IInvocation> callback)
			{
				Debug.Assert(callback != null);

				this.callback = callback;
			}

			public override void RespondTo(Invocation invocation)
			{
				this.callback.Invoke(invocation);
			}
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
			public readonly object Value;

			public ReturnEagerValueResponse(object value)
			{
				this.Value = value;
			}

			public override void RespondTo(Invocation invocation)
			{
				invocation.Return(this.Value);
			}
		}

		private sealed class ReturnInvocationLazyValueResponse : Response
		{
			private readonly Func<IInvocation, object> valueFactory;

			public ReturnInvocationLazyValueResponse(Func<IInvocation, object> valueFactory)
			{
				Debug.Assert(valueFactory != null);

				this.valueFactory = valueFactory;
			}

			public override void RespondTo(Invocation invocation)
			{
				invocation.Return(this.valueFactory.Invoke(invocation));
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
			private LambdaExpression expression;
			private Delegate eventArgsFunc;
			private object[] eventArgsParams;

			public RaiseEventResponse(Mock mock, LambdaExpression expression, Delegate eventArgsFunc, object[] eventArgsParams)
			{
				Debug.Assert(mock != null);
				Debug.Assert(expression != null);
				Debug.Assert(eventArgsFunc != null ^ eventArgsParams != null);

				this.mock = mock;
				this.expression = expression;
				this.eventArgsFunc = eventArgsFunc;
				this.eventArgsParams = eventArgsParams;
			}

			public void RespondTo(Invocation invocation)
			{
				object[] args;

				if (this.eventArgsParams != null)
				{
					args = this.eventArgsParams;
				}
				else
				{
					var argsFuncType = this.eventArgsFunc.GetType();
					if (argsFuncType.IsGenericType && argsFuncType.GetGenericArguments().Length == 1)
					{
						args = new object[] { this.mock.Object, this.eventArgsFunc.InvokePreserveStack() };
					}
					else
					{
						args = new object[] { this.mock.Object, this.eventArgsFunc.InvokePreserveStack(invocation.Arguments) };
					}
				}

				Mock.RaiseEvent(this.mock, this.expression, this.expression.Split(), args);
			}
		}
	}
}
