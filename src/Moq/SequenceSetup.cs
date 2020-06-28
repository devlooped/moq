// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
		// contains the behaviors set up with the `CallBase`, `Pass`, `Returns`, and `Throws` verbs
		private ConcurrentQueue<Behavior> behaviors;

		public SequenceSetup(Expression originalExpression, Mock mock, InvocationShape expectation)
			: base(originalExpression, mock, expectation)
		{
			this.behaviors = new ConcurrentQueue<Behavior>();
		}

		public void AddCallBase()
		{
			this.behaviors.Enqueue(new Behavior(BehaviorKind.CallBase, null));
		}

		public void AddPass()
		{
			this.behaviors.Enqueue(new Behavior(BehaviorKind.Pass, null));
		}

		public void AddReturns(object value)
		{
			this.behaviors.Enqueue(new Behavior(BehaviorKind.Returns, value));
		}

		public void AddReturns(Func<object> valueFunction)
		{
			this.behaviors.Enqueue(new Behavior(BehaviorKind.InvokeFunc, valueFunction));
		}

		public void AddThrows(Exception exception)
		{
			this.behaviors.Enqueue(new Behavior(BehaviorKind.Throws, exception));
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			if (this.behaviors.TryDequeue(out var behavior))
			{
				var (kind, arg) = behavior;
				switch (kind)
				{
					case BehaviorKind.CallBase:
						invocation.ReturnValue = invocation.CallBase();
						break;

					case BehaviorKind.Returns:
						invocation.ReturnValue = arg;
						break;

					case BehaviorKind.Throws:
						throw (Exception)arg;

					case BehaviorKind.InvokeFunc:
						invocation.ReturnValue = ((Func<object>)arg)();
						break;
				}
			}
			else
			{
				// we get here if there are more invocations than configured behaviors.
				// if the setup method does not have a return value, we don't need to do anything;
				// if it does have a return value, we produce the default value.

				var returnType = invocation.Method.ReturnType;
				if (returnType == typeof(void))
				{
				}
				else
				{
					invocation.ReturnValue = returnType.GetDefaultValue();
				}
			}
		}

		private readonly struct Behavior
		{
			private readonly BehaviorKind kind;
			private readonly object arg;

			public Behavior(BehaviorKind kind, object arg)
			{
				this.kind = kind;
				this.arg = arg;
			}

			public void Deconstruct(out BehaviorKind kind, out object arg)
			{
				kind = this.kind;
				arg = this.arg;
			}
		}

		private enum BehaviorKind
		{
			Pass,
			CallBase,
			Returns,
			Throws,
			InvokeFunc
		}
	}
}
