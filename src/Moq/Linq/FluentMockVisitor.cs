// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Moq.Linq
{
	internal class FluentMockVisitor : FluentMockVisitorBase
	{
		public FluentMockVisitor()
			: base(setupFirst: true)
		{
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// the actual first object being used in a fluent expression.
			return Expression.Call(null, Mock.GetMethod.MakeGenericMethod(node.Type), node);
		}
	}
}
