// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Moq.Async;

namespace Moq
{
	/// <summary>
	///   Represents a set (or the "shape") of invocations
	///   against which concrete <see cref="Invocation"/>s can be matched.
	/// </summary>
	internal abstract class Expectation : IEquatable<Expectation>
	{
		public abstract LambdaExpression Expression { get; }

		public virtual bool HasResultExpression(out IAwaitableFactory awaitableFactory)
		{
			awaitableFactory = null;
			return false;
		}

		public override bool Equals(object obj)
		{
			return obj is Expectation other && this.Equals(other);
		}

		public abstract bool Equals(Expectation other);

		public abstract override int GetHashCode();

		public abstract bool IsMatch(Invocation invocation);

		public virtual void SetupEvaluatedSuccessfully(Invocation invocation)
		{
		}

		public override string ToString()
		{
			return this.Expression.ToStringFixed();
		}
	}
}
