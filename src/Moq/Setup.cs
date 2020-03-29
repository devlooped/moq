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

		/// <summary>
		///   Verifies this setup and/or those of its inner mock (if present and known).
		/// </summary>
		/// <param name="predicate">
		///   Specifies which setups should be verified.
		/// </param>
		/// <param name="error">
		///   If this setup and/or any of its inner mock (if present and known) failed verification,
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
				if (!this.TryVerifySelf(out var e) && e.IsVerificationError)
				{
					error = e;
					return false;
				}
				else
				{
					return this.TryVerifyInnerMock(predicate, out error);
				}
			}
			else
			{
				error = null;
				return true;
			}
		}

		/// <summary>
		///   Verifies all setups of this setup's inner mock (if present and known).
		///   Multiple verification errors are aggregated into a single <see cref="MockException"/>.
		/// </summary>
		/// <param name="predicate">
		///   Specifies which setups should be verified.
		/// </param>
		/// <param name="error">
		///   If one or more setups of this setup's inner mock (if present and known) failed verification,
		///   this <see langword="out"/> parameter will receive a <see cref="MockException"/> describing the verification error(s).
		/// </param>
		/// <returns>
		///   <see langword="true"/> if verification succeeded without any errors;
		///   otherwise, <see langword="false"/>.
		/// </returns>
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

		/// <summary>
		///   Verifies only this setup, excluding those of its inner mock (if present and known).
		/// </summary>
		/// <param name="error">
		///   If this setup failed verification,
		///   this <see langword="out"/> parameter will receive a <see cref="MockException"/> describing the verification error.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if verification succeeded without any errors;
		///   otherwise, <see langword="false"/>.
		/// </returns>
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
