// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;

namespace Moq
{
	internal abstract class FluentMockVisitorBase : ExpressionVisitor
	{
		protected FluentMockVisitorBase()
		{
		}

		protected override abstract Expression VisitMember(MemberExpression node);

		protected override abstract Expression VisitMethodCall(MethodCallExpression node);

		protected override abstract Expression VisitParameter(ParameterExpression node);
	}
}
