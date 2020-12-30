// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq.Async
{
	/// <summary>
	///   Represents an <see langword="await"/> expression.
	/// </summary>
	/// <remarks>
	///   When reconstructing an expression tree from a delegate, <see cref="ActionObserver"/> will insert
	///   nodes of this kind as a substitute for inferred calls to <c>Await</c> methods. (Being static methods,
	///   they are not recorded / seen by <see cref="ActionObserver"/> and therefore their identity gets "lost".)
	/// </remarks>
	internal sealed class AwaitExpression : Expression
	{
		public readonly AwaitableHandler AwaitableHandler;
		public readonly Expression Operand;

		public AwaitExpression(Expression operand, AwaitableHandler awaitableHandler)
		{
			this.AwaitableHandler = awaitableHandler;
			this.Operand = operand;
		}

		public override bool CanReduce => false;

		public override ExpressionType NodeType => ExpressionType.Extension;

		public override Type Type => this.AwaitableHandler.ResultType;

		public override string ToString()
		{
			return $"(await {this.Operand})";
		}

		protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
	}
}
