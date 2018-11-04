// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Programmable setup used by <see cref="Mock.SetupVoidSequence(Mock, LambdaExpression)"/> and <see cref="Mock.SetupNonVoidSequence(Mock, LambdaExpression)"/>.
	/// </summary>
	internal sealed class SequenceSetup : SetupWithOutParameterSupport
	{
		// contains the responses set up with the `CallBase`, `Pass`, `Returns`, and `Throws` verbs
		private ConcurrentQueue<(ResponseKind, object)> responses;
		private bool invoked;

		public SequenceSetup(LambdaExpression originalExpression, MethodInfo method, IReadOnlyList<Expression> arguments)
			: base(method, arguments, originalExpression)
		{
			this.responses = new ConcurrentQueue<(ResponseKind, object)>();
		}

		public void AddCallBase()
		{
			this.responses.Enqueue((ResponseKind.CallBase, (object)null));
		}

		public void AddPass()
		{
			this.responses.Enqueue((ResponseKind.Pass, (object)null));
		}

		public void AddReturns(object value)
		{
			this.responses.Enqueue((ResponseKind.Returns, value));
		}

		public void AddReturns(Func<object> valueFunction)
		{
			this.responses.Enqueue((ResponseKind.InvokeFunc, valueFunction));
		}

		public void AddThrows(Exception exception)
		{
			this.responses.Enqueue((ResponseKind.Throws, (object)exception));
		}

		public override void Execute(Invocation invocation)
		{
			this.invoked = true;

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
						invocation.Return(arg);
						break;

					case ResponseKind.Throws:
						throw (Exception)arg;

					case ResponseKind.InvokeFunc:
						invocation.Return(((Func<object>)arg)());
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

		public override bool TryVerifyAll()
		{
			return this.invoked;
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
