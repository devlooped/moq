// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests.Matchers
{
	public class AnyMatcherFixture
	{
		[Fact]
		public void MatchesNull()
		{
			var expr = ToExpression<object>(() => It.IsAny<object>()).Body;

			var (matcher, _) = MatcherFactory.CreateMatcher(expr);

			Assert.True(matcher.Matches(null, typeof(object)));
		}

		[Fact]
		public void MatchesIfAssignableType()
		{
			var expr = ToExpression<object>(() => It.IsAny<object>()).Body;

			var (matcher, _) = MatcherFactory.CreateMatcher(expr);

			Assert.True(matcher.Matches("foo", typeof(object)));
		}

		[Fact]
		public void MatchesIfAssignableInterface()
		{
			var expr = ToExpression<IDisposable>(() => It.IsAny<IDisposable>()).Body;

			var (matcher, _) = MatcherFactory.CreateMatcher(expr);

			Assert.True(matcher.Matches(new Disposable(), typeof(IDisposable)));
		}

		[Fact]
		public void DoesntMatchIfNotAssignableType()
		{
			var expr = ToExpression<IFormatProvider>(() => It.IsAny<IFormatProvider>()).Body;

			var (matcher, _) = MatcherFactory.CreateMatcher(expr);

			Assert.False(matcher.Matches("foo", typeof(IFormatProvider)));
		}

		private LambdaExpression ToExpression<TResult>(Expression<Func<TResult>> expr)
		{
			return expr;
		}

		class Disposable : IDisposable
		{
			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}
	}
}
