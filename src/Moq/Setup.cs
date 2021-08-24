// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Moq.Async;

namespace Moq
{
	internal abstract class Setup : ISetup
	{
		private readonly Expectation expectation;
		private readonly Expression originalExpression;
		private readonly Mock mock;
		private Flags flags;

		protected Setup(Expression originalExpression, Mock mock, Expectation expectation)
		{
			Debug.Assert(mock != null);
			Debug.Assert(expectation != null);

			this.originalExpression = originalExpression;
			this.expectation = expectation;
			this.mock = mock;
		}

		public virtual Condition Condition => null;

		public Expectation Expectation => this.expectation;

		public LambdaExpression Expression => this.expectation.Expression;

		Mock ISetup.InnerMock => this.InnerMocks.SingleOrDefault();

		public virtual IEnumerable<Mock> InnerMocks => Enumerable.Empty<Mock>();

		public bool IsConditional => this.Condition != null;

		public bool IsOverridden => (this.flags & Flags.Overridden) != 0;

		public bool IsVerifiable => (this.flags & Flags.Verifiable) != 0;

		public Mock Mock => this.mock;

		public Expression OriginalExpression => this.originalExpression;

		public bool IsMatched => (this.flags & Flags.Matched) != 0;

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

			if (this.expectation.HasResultExpression(out var awaitableFactory))
			{
				try
				{
					this.ExecuteCore(invocation);
				}
				catch (Exception exception)
				{
					invocation.Exception = exception;
				}
				finally
				{
					invocation.ConvertResultToAwaitable(awaitableFactory);
				}
			}
			else
			{
				this.ExecuteCore(invocation);
			}
		}

		protected abstract void ExecuteCore(Invocation invocation);

		public void MarkAsOverridden()
		{
			Debug.Assert(!this.IsOverridden);

			this.flags |= Flags.Overridden;
		}

		public void MarkAsVerifiable()
		{
			this.flags |= Flags.Verifiable;
		}

		public bool Matches(Invocation invocation)
		{
			return this.expectation.IsMatch(invocation) && (this.Condition == null || this.Condition.IsTrue);
		}

		public bool Matches(MethodExpectation expectation)
		{
			return this.expectation.Equals(expectation);
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
		/// <param name="recursive">
		///   Specifies whether recursive verification should be performed.
		/// </param>
		/// <param name="predicate">
		///   Specifies which setups should be verified.
		/// </param>
		/// <param name="verifiedMocks">
		///   The set of mocks that have already been verified.
		/// </param>
		/// <exception cref="MockException">
		///   This setup or any of its inner mock (if present and known) failed verification.
		/// </exception>
		internal void Verify(bool recursive, Func<ISetup, bool> predicate, HashSet<Mock> verifiedMocks)
		{
			// verify this setup:
			this.VerifySelf();

			// optionally verify setups of inner mock (if present and known):
			if (recursive)
			{
				try
				{
					foreach (var innerMock in this.InnerMocks)
					{
						innerMock.Verify(predicate, verifiedMocks);
					}
				}
				catch (MockException error) when (error.IsVerificationError)
				{
					throw MockException.FromInnerMockOf(this, error);
				}
			}
		}

		protected virtual void VerifySelf()
		{
			if (!this.IsMatched)
			{
				throw MockException.UnmatchedSetup(this);
			}
		}

		public void Reset()
		{
			this.flags &= ~Flags.Matched;

			this.ResetCore();
		}

		protected virtual void ResetCore()
		{
		}

		public void Verify(bool recursive = true)
		{
			this.Verify(recursive, setup => setup.IsVerifiable);
		}

		public void VerifyAll()
		{
			this.Verify(recursive: true, setup => true);
		}

		private void Verify(bool recursive, Func<ISetup, bool> predicate)
		{
			var verifiedMocks = new HashSet<Mock>();

			foreach (Invocation invocation in this.mock.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedBy(setup => setup == this);
			}

			this.Verify(recursive, predicate, verifiedMocks);
		}

		protected static Mock TryGetInnerMockFrom(object returnValue)
		{
			return (Awaitable.TryGetResultRecursive(returnValue) as IMocked)?.Mock;
		}

		[Flags]
		private enum Flags : byte
		{
			Matched = 1,
			Overridden = 2,
			Verifiable = 4,
		}
	}
}
