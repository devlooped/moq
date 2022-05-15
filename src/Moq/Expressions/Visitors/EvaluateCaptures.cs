// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Moq.Expressions.Visitors
{
	/// <summary>
	///   Evaluates variables that have been closed over by a lambda function.
	/// </summary>
	internal sealed class EvaluateCaptures : ExpressionVisitor
	{
		public static readonly ExpressionVisitor Rewriter = new EvaluateCaptures();

		private EvaluateCaptures()
		{
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Member is FieldInfo fi
				&& node.Expression is ConstantExpression ce
				&& node.Member.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute)))
			{
				return Expression.Constant(fi.GetValue(ce.Value), node.Type);
			}
			else
			{
				return base.VisitMember(node);
			}
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			return node.NodeType == ExpressionType.Quote ? node : base.VisitUnary(node);
		}
	}
}
