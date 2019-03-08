// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal abstract class Setup
	{
		private readonly InvocationShape expectation;

		protected Setup(InvocationShape expectation)
		{
			this.expectation = expectation;
		}

		public virtual Condition Condition => null;

		public LambdaExpression Expression => this.expectation.Expression;

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
			var expression = this.expectation.Expression;
			var mockedType = expression.Parameters[0].Type;

			var builder = new StringBuilder();
			builder.AppendNameOf(mockedType)
			       .Append(' ')
			       .Append(expression.PartialMatcherAwareEval().ToStringFixed());

			return builder.ToString();
		}

		public virtual MockException TryVerify()
		{
			return this.IsVerifiable ? this.TryVerifyAll() : null;
		}

		public abstract MockException TryVerifyAll();
	}
}
