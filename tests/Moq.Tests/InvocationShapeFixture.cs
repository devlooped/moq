// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class InvocationShapeFixture
	{
		// This test is unspectacular but sets the stage for the one following it. See comment below.
		[Fact]
		public void Regular_parameters_are_compared_using_equality()
		{
			var fst = ToInvocationShape<A>(a => a.Method(1, 2, 3));
			var snd = ToInvocationShape<A>(a => a.Method(1, 2, 3));

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
			var fst = ToInvocationShape<B>(b => b.Method(1, 2, 3));
			var snd = ToInvocationShape<B>(b => b.Method(1, 2, 3));

			Assert.NotSame(fst, snd);
			Assert.Equal(fst, snd);
		}

		private static InvocationShape ToInvocationShape<T>(Expression<Action<T>> expression)
		{
			Debug.Assert(expression != null);
			Debug.Assert(expression.Body is MethodCallExpression);

			var methodCall = (MethodCallExpression)expression.Body;
			return new InvocationShape(expression, methodCall.Method, methodCall.Arguments);
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
