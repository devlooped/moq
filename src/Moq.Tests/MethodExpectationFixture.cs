// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class MethodExpectationFixture
	{
		// This test is unspectacular but sets the stage for the one following it. See comment below.
		[Fact]
		public void Regular_parameters_are_compared_using_equality()
		{
			var fst = ToMethodExpectation<A>(a => a.Method(1, 2, 3));
			var snd = ToMethodExpectation<A>(a => a.Method(1, 2, 3));

			Assert.NotSame(fst, snd);
			Assert.Equal(fst, snd);
		}

		// If you look at just this test code, and not at the definition for `B.Method`,
		// you'd rightly expect the same outcome as for the above test, which looks almost the same.
		// What we're testing here is that when the compiler silently transforms literal values to
		// `params` arrays, we care about their structural equality, not about their differing identities.
		[Fact]
		public void Param_array_args_are_compared_using_structural_equality_not_reference_equality()
		{
			var fst = ToMethodExpectation<B>(b => b.Method(1, 2, 3));
			var snd = ToMethodExpectation<B>(b => b.Method(1, 2, 3));

			Assert.NotSame(fst, snd);
			Assert.Equal(fst, snd);
		}

		[Fact]
		public void Param_array_args_are_compared_partially_evaluated()
		{
			int x = 1;

			var fst = ToMethodExpectation<B>(b => b.Method(1, 2, 3));
			var snd = ToMethodExpectation<B>(b => b.Method(x, 2, 3));
			//                                           ^
			// `x` will be captured and represented in the expression tree as a display class field access:
			var xExpr = ((snd.Expression.Body as MethodCallExpression).Arguments.Last() as NewArrayExpression).Expressions.First();

			Assert.False(xExpr is ConstantExpression);
			Assert.NotSame(fst, snd);
			Assert.Equal(fst, snd);
		}

		private static MethodExpectation ToMethodExpectation<T>(Expression<Action<T>> expression)
		{
			Debug.Assert(expression != null);
			Debug.Assert(expression.Body is MethodCallExpression);

			var methodCall = (MethodCallExpression)expression.Body;
			return new MethodExpectation(expression, methodCall.Method, methodCall.Arguments);
		}

		public interface A
		{
			void Method(int arg1, int arg2, int arg3);
		}

		public interface B
		{
			void Method(params int[] args);
		}
	}
}
