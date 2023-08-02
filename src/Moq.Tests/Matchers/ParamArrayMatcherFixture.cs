// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests.Matchers
{
	public class ParamArrayMatcherFixture
	{
		[Theory]
		[InlineData(42, "", true)]
		[InlineData(42, null, true)]
		[InlineData(3.141f, "", false)]
		[InlineData(null, "", false)]
		public void Matches_several_matchers_from_params_array(object first, object second, bool shouldMatch)
		{
			var seconds = new List<string>();
			var methodCallExpr = (MethodCallExpression)ToExpression<IX>(x => x.Method(It.IsAny<int>(), Capture.In(seconds))).Body;
			var expr = methodCallExpr.Arguments.Single();
			var parameter = typeof(IX).GetMethod("Method").GetParameters().Single();

			var (matcher, _) = MatcherFactory.CreateMatcher(expr, parameter);

			Assert.Equal(shouldMatch, matcher.Matches(new object[] { first, second }, typeof(object[])));
		}

		[Fact]
		public void SetupEvaluatedSuccessfully_succeeds_for_matching_values()
		{
			var seconds = new List<string>();
			var methodCallExpr = (MethodCallExpression)ToExpression<IX>(x => x.Method(It.IsAny<int>(), Capture.In(seconds))).Body;
			var expr = methodCallExpr.Arguments.Single();
			var parameter = typeof(IX).GetMethod("Method").GetParameters().Single();

			var (matcher, _) = MatcherFactory.CreateMatcher(expr, parameter);

			matcher.SetupEvaluatedSuccessfully(new object[] { 42, "" }, typeof(object[]));
		}

		private LambdaExpression ToExpression<T>(Expression<Action<T>> expr)
		{
			return expr;
		}

		public interface IX
		{
			void Method(params object[] args);
		}
	}
}
