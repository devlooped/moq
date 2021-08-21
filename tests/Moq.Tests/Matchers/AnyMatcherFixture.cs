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

		[Fact]
		public void ParameterIncompatibleWithMatchedType()
		{
			//shorthand wrapper
			void AssertNotCompatible(Type parameterType, Type matchType)
			{
				Assert.True(MatcherFactory.ParameterIncompatibleWithMatchedType(parameterType: parameterType, matchType: matchType));
			}
			//shorthand wrapper
			void AssertCompatible(Type parameterType, Type matchType)
			{
				Assert.False(MatcherFactory.ParameterIncompatibleWithMatchedType(parameterType: parameterType, matchType: matchType));
			}

			// incompatible tests
			AssertNotCompatible(parameterType: typeof(DateTimeOffset), matchType: typeof(DateTime));
			AssertNotCompatible(parameterType: typeof(DateTime), matchType: typeof(DateTimeOffset));
			AssertNotCompatible(parameterType: typeof(DateTime), matchType: typeof(object));

			// if mocked method parameter type is an object, should allow `It.IsAny<string>()`
			AssertCompatible(parameterType: typeof(object), matchType: typeof(string));

			// if mocked method parameter type is an object, should allow `It.IsAny<DateTime>()`
			AssertCompatible(parameterType: typeof(object), matchType: typeof(DateTime));
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
