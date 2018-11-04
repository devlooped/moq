// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Moq.Matchers;

namespace Moq
{
	// Keeps legacy implementations.
	public partial class Mock
	{
		[Obsolete]
		internal static void VerifySet(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			var method = expression.ToPropertyInfo().SetMethod;
			ThrowIfVerifyExpressionInvolvesUnsupportedMember(expression, method);

			var expectation = new InvocationShape(method, new IMatcher[] { AnyMatcher.Instance });
			VerifyCalls(GetTargetMock(((MemberExpression)expression.Body).Expression, mock), expectation, expression, times, failMessage);
		}
	}
}
