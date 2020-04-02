// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Moq
{
	internal sealed class FluentSetup : IFluentSetup
	{
		private readonly LambdaExpression expression;
		private List<ISetup> parts;
#if DEBUG
		private bool complete;
#endif

		public FluentSetup(LambdaExpression expression)
		{
			Debug.Assert(expression != null);

			this.expression = expression;
		}

		public LambdaExpression Expression => this.expression;

		public bool IsConditional => this.parts.First().IsConditional;

		public bool IsOverridden => this.parts.Any(p => p.IsOverridden);

		public bool IsVerifiable => this.parts.Last().IsVerifiable;

		public Mock Mock => this.parts.First().Mock;

		public IReadOnlyList<ISetup> Parts => this.parts;

		public bool WasMatched => this.parts.All(p => p.WasMatched);

		public void AddPart(ISetup part)
		{
#if DEBUG
			// NOTE: This operation is not thread-safe, and it probably doesn't need to be.
			// This `FluentSetup` instance shouldn't be accessible until `Mock.SetupRecursive` is done
			// and the first part is added to the root mock's `Setup` collection. At that time,
			// all setup parts should have been added to this instance. Let's verify this assumption
			// with a debug-only 'complete' flag:
			Debug.Assert(!this.complete);
#endif

			if (this.parts == null)
			{
				this.parts = new List<ISetup>();
			}

			this.parts.Add(part);
		}

#if DEBUG
		public void MarkAsComplete()
		{
			this.complete = true;
		}
#endif

		public bool IsPartOfFluentSetup(out IFluentSetup fluentSetup)
		{
			fluentSetup = null;
			return false;
		}

		public bool? ReturnsMock(out Mock innerMock)
		{
			return this.parts.Last().ReturnsMock(out innerMock);
		}

		public override string ToString()
		{
			var mockedType = this.expression.Parameters[0].Type;

			var builder = new StringBuilder();
			builder.AppendNameOf(mockedType)
			       .Append(' ')
			       .Append(expression.PartialMatcherAwareEval().ToStringFixed());

			return builder.ToString();
		}

		public void Verify(bool recursive = true)
		{
			this.Verify(lastPart => lastPart.Verify(recursive));
		}

		public void VerifyAll()
		{
			this.Verify(lastPart => lastPart.VerifyAll());
		}

		private void Verify(Action<ISetup> verifyLast)
		{
			Debug.Assert(verifyLast != null);
			Debug.Assert(this.parts.Count > 1);

			try
			{
				foreach (var part in this.parts.Take(this.parts.Count - 1))
				{
					part.Verify(recursive: false);
				}
				verifyLast(this.parts.Last());
			}
			catch (MockException error) when (error.IsVerificationError)
			{
				throw MockException.FromInnerMockOf(this, error);
			}
		}
	}
}
