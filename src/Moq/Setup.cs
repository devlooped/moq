// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal abstract class Setup
	{
		private readonly InvocationShape expectation;
		private readonly LambdaExpression expression;

		protected Setup(InvocationShape expectation, LambdaExpression expression)
		{
			Debug.Assert(expression != null);

			this.expectation = expectation;
			this.expression = expression;
		}

		public virtual Condition Condition => null;

		public LambdaExpression Expression => this.expression;

		public virtual bool IsVerifiable => false;

		public MethodInfo Method => this.expectation.Method;

		public abstract void Execute(Invocation invocation);

		public bool Matches(Invocation invocation)
		{
			return this.expectation.IsMatch(invocation) && (this.Condition == null || this.Condition.IsTrue);
		}

		public virtual void SetOutParameters(Invocation invocation)
		{
		}

		public override string ToString()
		{
			var expression = this.expression.PartialMatcherAwareEval();
			var mockedType = this.expression.Parameters[0].Type;

			var builder = new StringBuilder();
			builder.AppendNameOf(mockedType)
			       .Append(' ')
			       .Append(expression.ToStringFixed());

			return builder.ToString();
		}

		public bool TryVerify()
		{
			return !this.IsVerifiable || this.TryVerifyAll();
		}

		public abstract bool TryVerifyAll();
	}
}
