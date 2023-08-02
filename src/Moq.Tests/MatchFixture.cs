// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class MatchFixture
	{
		public class RenderExpression
		{
			[Fact]
			public void RenderExpression_of_It_Is_includes_formatted_predicate_expression()
			{
				var match = GetMatch(() => It.Is<int>(i => i == 123));
				Assert.Equal("It.Is<int>(i => i == 123)", match.RenderExpression.ToStringFixed());
			}

			[Fact]
			public void Two_custom_matchers_with_render_expression_have_equal_render_expression()
			{
				var first  = GetMatch(() => Order.IsSmallRE());
				var second = GetMatch(() => Order.IsSmallRE());
				Assert.Equal(first.RenderExpression, second.RenderExpression, ExpressionComparer.Default);
			}

			[Fact]
			public void Two_parameterized_custom_matchers_with_render_expression_have_equal_render_expressions()
			{
				var first  = GetMatch(() => Order.IsSmallerThanRE(123M));
				var second = GetMatch(() => Order.IsSmallerThanRE(123M));
				Assert.Equal(first.RenderExpression, second.RenderExpression, ExpressionComparer.Default);
			}
		}

		public class Equality
		{
			[Fact]
			public void It_IsAny_with_equal_generic_arguments()
			{
				var first  = GetMatch(() => It.IsAny<int>());
				var second = GetMatch(() => It.IsAny<int>());
				Assert.Equal(first, second);
			}

			[Fact]
			public void It_Is_with_equal_arguments_1()
			{
				var first  = GetMatch(() => It.Is<int>(i => i == 123));
				var second = GetMatch(() => It.Is<int>(i => i == 123));
				Assert.Equal(first, second);
			}

			[Fact]
			public void It_Is_with_equal_arguments_2()
			{
				Expression<Func<int, bool>> comparison = i => i == 123;
				var first  = GetMatch(() => It.Is<int>(comparison));
				var second = GetMatch(() => It.Is<int>(comparison));
				Assert.Equal(first, second);
			}

			[Fact]
			public void It_IsRegex_with_equal_arguments()
			{
				var first  = GetMatch(() => It.IsRegex("^.*$"));
				var second = GetMatch(() => It.IsRegex("^.*$"));
				Assert.Equal(first, second);
			}

			[Fact]
			public void Custom_matchers_with_render_expression()
			{
				var first  = GetMatch(() => Order.IsSmallRE());
				var second = GetMatch(() => Order.IsSmallRE());
				Assert.Equal(first, second);
			}

			[Fact]
			public void Custom_matcher_without_render_expressions()
			{
				var first  = GetMatch(() => Order.IsSmall());
				var second = GetMatch(() => Order.IsSmall());
				Assert.Equal(first, second);
			}

			[Fact]
			public void Parameterized_custom_matchers_with_render_expression_and_equal_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThanRE(123M));
				var second = GetMatch(() => Order.IsSmallerThanRE(123M));
				Assert.Equal(first, second);
			}
		}

		public class Inequality
		{
			[Fact]
			public void Different_custom_matchers_without_render_expressions()
			{
				var first  = GetMatch(() => Order.IsSmall());
				var second = GetMatch(() => Order.IsLarge());
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_custom_matchers_with_render_expressions()
			{
				var first  = GetMatch(() => Order.IsSmallRE());
				var second = GetMatch(() => Order.IsLargeRE());
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_parameterized_custom_matchers_without_render_expressions_but_equal_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThan(123M));
				var second = GetMatch(() => Order.IsLargerThan(123M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_parameterized_custom_matchers_with_render_expressions_and_equal_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThanRE(123M));
				var second = GetMatch(() => Order.IsLargerThanRE(123M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_parameterized_custom_matchers_without_render_expressions_and_different_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThan(123M));
				var second = GetMatch(() => Order.IsLargerThan(456M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_parameterized_custom_matchers_with_render_expressions_but_different_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThanRE(123M));
				var second = GetMatch(() => Order.IsLargerThanRE(456M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Parameterized_custom_matcher_without_render_expressions_and_different_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThan(123M));
				var second = GetMatch(() => Order.IsSmallerThan(456M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Parameterized_custom_matcher_with_render_expressions_but_different_arguments()
			{
				var first  = GetMatch(() => Order.IsSmallerThanRE(123M));
				var second = GetMatch(() => Order.IsSmallerThanRE(456M));
				Assert.NotEqual(first, second);
			}

			[Fact]
			public void Different_matchers_with_same_render_expression()
			{
				var first  = GetMatch(() => It.IsAny<Order>());
				var second = GetMatch(() => Order.PretendIsAny());
				Assert.NotEqual(first, second);
			}
		}

		public class Equality_ambiguity
		{
			[Fact]
			public void Parameterized_custom_matcher_without_render_expression_but_equal_arguments()
			{
				// This is almost guaranteed to trip someone up sooner or later, as the reason why
				// below two matchers aren't equal is not obvious at first glance.
				//
				// If a parameterized custom matcher only provides a predicate (delegate) but no
				// render expression, how do we get at the parameters in order to compare them?
				// Short answer: They typically sit on the delegate's target object, which we'd have
				// to compare for structural equality. Which is is kind of expensive.
				//
				// So for the moment, this ambiguity is left unresolved. If you run into this, the
				// easiest way to work around it is to provide a render expression to `Match.Create`.

				var first  = GetMatch(() => Order.IsSmallerThan(123M));
				var second = GetMatch(() => Order.IsSmallerThan(123M));
				Assert.NotEqual(first, second);
			}
		}

		private static Match GetMatch<T>(Func<T> func)
		{
			using (var observer = MatcherObserver.Activate())
			{
				_ = func();
				return observer.TryGetLastMatch(out var match) ? match as Match<T> : null;
			}
		}

		private abstract class Order
		{
			public abstract decimal MetricTons { get; }

			public static Order IsSmall()
			{
				return Match.Create<Order>(order => order.MetricTons < 1000M);
			}

			public static Order IsSmallRE()
			{
				return Match.Create<Order>(order => order.MetricTons < 1000M, () => Order.IsSmallRE());
			}

			public static Order IsLarge()
			{
				return Match.Create<Order>(order => order.MetricTons >= 1000M);
			}

			public static Order IsLargeRE()
			{
				return Match.Create<Order>(order => order.MetricTons >= 1000M, () => Order.IsLargeRE());
			}

			public static Order IsSmallerThan(decimal threshold)
			{
				return Match.Create<Order>(order => order.MetricTons < threshold);
			}

			public static Order IsSmallerThanRE(decimal threshold)
			{
				return Match.Create<Order>(order => order.MetricTons < threshold, () => Order.IsSmallerThanRE(threshold));
			}

			public static Order IsLargerThan(decimal threshold)
			{
				return Match.Create<Order>(order => order.MetricTons > threshold);
			}

			public static Order IsLargerThanRE(decimal threshold)
			{
				return Match.Create<Order>(order => order.MetricTons < threshold, () => Order.IsLargerThanRE(threshold));
			}

			public static Order PretendIsAny()
			{
				return Match.Create<Order>(order => false, () => It.IsAny<Order>());
			}
		}
	}
}
