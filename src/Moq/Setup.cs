// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
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
			Debug.Assert(expectation != null);

			this.expectation = expectation;
		}

		public virtual Condition Condition => null;

		public InvocationShape Expectation => this.expectation;

		public LambdaExpression Expression => this.expectation.Expression;

		public virtual bool IsVerifiable => false;

		public MethodInfo Method => this.expectation.Method;

		public abstract void Execute(Invocation invocation);

		public Mock GetInnerMock()
		{
			return this.ReturnsInnerMock(out var innerMock) ? innerMock : throw new InvalidOperationException();
		}

		/// <summary>
		///   Attempts to get this setup's return value without invoking user code
		///   (which could have side effects beyond Moq's understanding and control).
		/// </summary>
		public virtual bool TryGetReturnValue(out object returnValue)
		{
			returnValue = default;
			return false;
		}

		public bool Matches(Invocation invocation)
		{
			return this.expectation.IsMatch(invocation) && (this.Condition == null || this.Condition.IsTrue);
		}

		public bool ReturnsInnerMock(out Mock mock)
		{
			if (this.TryGetReturnValue(out var returnValue) && Unwrap.ResultIfCompletedTask(returnValue) is IMocked mocked)
			{
				mock = mocked.Mock;
				return true;
			}
			else
			{
				mock = null;
				return false;
			}
		}

		public void EvaluatedSuccessfully(Invocation invocation)
		{
			this.Condition?.SetupEvaluatedSuccessfully();
			this.expectation.SetupEvaluatedSuccessfully(invocation);
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

		public bool TryVerify(Func<Setup, bool> predicate, out MockException error)
		{
			if (predicate(this))
			{
				if (!this.TryVerifySelf(out var e) && e.IsVerificationError)
				{
					error = e;
					return false;
				}
			}

			return this.TryVerifyInnerMock(predicate, out error);
		}

		protected virtual bool TryVerifyInnerMock(Func<Setup, bool> predicate, out MockException error)
		{
			if (this.ReturnsInnerMock(out var innerMock))
			{
				if (!innerMock.TryVerify(predicate, out var e) && e.IsVerificationError)
				{
					error = MockException.FromInnerMockOf(this, e);
					return false;
				}
			}

			error = null;
			return true;
		}

		protected virtual bool TryVerifySelf(out MockException error)
		{
			error = null;
			return true;
		}

		public virtual void Uninvoke()
		{
		}
	}
}
