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
			var expr = ToExpression<object>(() => It.IsAny<object>()).ToLambda().Body;

			var matcher = MatcherFactory.CreateMatcher(expr, false);
			matcher.Initialize(expr);

			Assert.True(matcher.Matches(null));
		}

		[Fact]
		public void MatchesIfAssignableType()
		{
			var expr = ToExpression<object>(() => It.IsAny<object>()).ToLambda().Body;

			var matcher = MatcherFactory.CreateMatcher(expr, false);
			matcher.Initialize(expr);

			Assert.True(matcher.Matches("foo"));
		}

		[Fact]
		public void MatchesIfAssignableInterface()
		{
			var expr = ToExpression<IDisposable>(() => It.IsAny<IDisposable>()).ToLambda().Body;

			var matcher = MatcherFactory.CreateMatcher(expr, false);
			matcher.Initialize(expr);

			Assert.True(matcher.Matches(new Disposable()));
		}

		[Fact]
		public void DoesntMatchIfNotAssignableType()
		{
			var expr = ToExpression<IFormatProvider>(() => It.IsAny<IFormatProvider>()).ToLambda().Body;

			var matcher = MatcherFactory.CreateMatcher(expr, false);
			matcher.Initialize(expr);

			Assert.False(matcher.Matches("foo"));
		}

		private Expression ToExpression<TResult>(Expression<Func<TResult>> expr)
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
