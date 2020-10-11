// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{
	internal sealed class MatchExpression : Expression
	{
		public readonly Match Match;

		public MatchExpression(Match match)
		{
			this.Match = match;
		}

		public override ExpressionType NodeType => ExpressionType.Extension;

		public override Type Type => this.Match.RenderExpression.Type;

		// This node type is irreducible in order to prevent compilation.
		// The best possible reduction would involve `RenderExpression`,
		// which isn't intended to be used for that purpose.
		public override bool CanReduce => false;

		protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

		public override string ToString() => this.Match.RenderExpression.ToString();
	}
}
