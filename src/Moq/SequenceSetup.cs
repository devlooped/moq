// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	///   Programmable setup used by <see cref="Mock.SetupSequence(Mock, LambdaExpression)"/>.
	/// </summary>
	internal sealed class SequenceSetup : SetupWithOutParameterSupport
	{
		// contains the responses set up with the `CallBase`, `Pass`, `Returns`, and `Throws` verbs
		private ConcurrentQueue<Response> responses;
		private readonly Func<object, object> wrapper;
		private readonly Func<Exception, object> exceptionWrapper;

		public SequenceSetup(Expression originalExpression, Mock mock, InvocationShape expectation)
			: base(originalExpression, mock, expectation)
		{
			this.responses = new ConcurrentQueue<Response>();

			this.wrapper = expectation.Await ? Wrap.GetResultWrapper(expectation.Method.ReturnType) : null;
			this.exceptionWrapper = expectation.Await ? Wrap.GetExceptionWrapper(expectation.Method.ReturnType) : null;
		}

		public void AddCallBase()
		{
			this.responses.Enqueue(new Response(ResponseKind.CallBase, null));
		}

		public void AddPass()
		{
			this.responses.Enqueue(new Response(ResponseKind.Pass, null));
		}

		public void AddReturns(object value)
		{
			this.responses.Enqueue(new Response(ResponseKind.Returns, value));
		}

		public void AddReturns(Func<object> valueFunction)
		{
			this.responses.Enqueue(new Response(ResponseKind.InvokeFunc, valueFunction));
		}

		public void AddThrows(Exception exception)
		{
			this.responses.Enqueue(new Response(ResponseKind.Throws, exception));
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			if (this.responses.TryDequeue(out var response))
			{
				var (kind, arg) = response;
				switch (kind)
				{
					case ResponseKind.Pass:
						invocation.Return();
						break;

					case ResponseKind.CallBase:
						invocation.ReturnBase();
						break;

					case ResponseKind.Returns:
						if (this.wrapper != null) arg = this.wrapper(arg);
						invocation.Return(arg);
						break;

					case ResponseKind.Throws:
						if (this.exceptionWrapper != null)
						{
							invocation.Return(this.exceptionWrapper((Exception)arg));
							break;
						}
						else
						{
							throw (Exception)arg;
						}

					case ResponseKind.InvokeFunc:
						arg = ((Func<object>)arg)();
						if (this.wrapper != null) arg = this.wrapper(arg);
						invocation.Return(arg);
						break;
				}
			}
			else
			{
				// we get here if there are more invocations than configured responses.
				// if the setup method does not have a return value, we don't need to do anything;
				// if it does have a return value, we produce the default value.

				var returnType = invocation.Method.ReturnType;
				if (returnType == typeof(void))
				{
				}
				else
				{
					invocation.Return(returnType.GetDefaultValue());
				}
			}
		}

		private readonly struct Response
		{
			private readonly ResponseKind kind;
			private readonly object arg;

			public Response(ResponseKind kind, object arg)
			{
				this.kind = kind;
				this.arg = arg;
			}

			public void Deconstruct(out ResponseKind kind, out object arg)
			{
				kind = this.kind;
				arg = this.arg;
			}
		}

		private enum ResponseKind
		{
			Pass,
			CallBase,
			Returns,
			Throws,
			InvokeFunc
		}
	}
}
