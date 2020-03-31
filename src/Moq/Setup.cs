// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal abstract class Setup : ISetup
	{
		private readonly InvocationShape expectation;
		private Flags flags;

		protected Setup(InvocationShape expectation)
		{
			Debug.Assert(expectation != null);

			this.expectation = expectation;
		}

		public virtual Condition Condition => null;

		public InvocationShape Expectation => this.expectation;

		public LambdaExpression Expression => this.expectation.Expression;

		public bool IsConditional => this.Condition != null;

		public bool IsOverridden => (this.flags & Flags.Overridden) != 0;

		public virtual bool IsVerifiable => false;

		public MethodInfo Method => this.expectation.Method;

		public bool WasMatched => (this.flags & Flags.Matched) != 0;

		public void Execute(Invocation invocation)
		{
			// update this setup:
			this.flags |= Flags.Matched;

			// update invocation:
			invocation.MarkAsMatchedBy(this);
			this.SetOutParameters(invocation);

			// update condition (important for `MockSequence`) and matchers (important for `Capture`):
			this.Condition?.SetupEvaluatedSuccessfully();
			this.expectation.SetupEvaluatedSuccessfully(invocation);

			this.ExecuteCore(invocation);
		}

		protected abstract void ExecuteCore(Invocation invocation);

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

		public void MarkAsOverridden()
		{
			Debug.Assert(!this.IsOverridden);

			this.flags |= Flags.Overridden;
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

		/// <summary>
		///   Verifies this setup and those of its inner mock (if present and known).
		/// </summary>
		/// <param name="predicate">
		///   Specifies which setups should be verified.
		/// </param>
		/// <param name="error">
		///   If this setup or any of its inner mock (if present and known) failed verification,
		///   this <see langword="out"/> parameter will receive a <see cref="MockException"/> describing the verification error(s).
		/// </param>
		/// <returns>
		///   <see langword="true"/> if verification succeeded without any errors;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryVerify(Func<Setup, bool> predicate, out MockException error)
		{
			if (predicate(this))
			{
				MockException e;

				// verify this setup:
				if (!this.TryVerifySelf(predicate, out e) && e.IsVerificationError)
				{
					error = e;
					return false;
				}

				// verify setups of inner mock (if present and known):
				if (this.ReturnsInnerMock(out var innerMock) && !innerMock.TryVerify(predicate, out e) && e.IsVerificationError)
				{
					error = MockException.FromInnerMockOf(this, e);
					return false;
				}
			}

			error = null;
			return true;
		}

		protected virtual bool TryVerifySelf(Func<Setup, bool> predicate, out MockException error)
		{
			error = this.WasMatched ? null : MockException.UnmatchedSetup(this);
			return error == null;
		}

		public void Reset()
		{
			this.flags &= ~Flags.Matched;

			this.ResetCore();
		}

		protected virtual void ResetCore()
		{
		}

		[Flags]
		private enum Flags : byte
		{
			Matched = 1,
			Overridden = 2,
		}
	}
}
