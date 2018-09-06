// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{
	internal class MatchExpression : Expression
	{
		public MatchExpression(Match match)
		{
			this.Match = match;
		}

		public override ExpressionType NodeType
		{
			get { return ExpressionType.Call; }
		}

		public override Type Type
		{
			get { return typeof(Match); }
		}

		public Match Match { get; private set; }
	}
}
