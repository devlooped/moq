// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class ActionObserverFixture
	{
		public class Reconstructibility
		{
			// NOTE: These tests might look pointless at first glance, until you notice
			// the signature of `AssertReconstructable`: delegates are being compared to
			// LINQ expression trees for equality.

			[Fact]
			public void Void_method_call()
			{
				AssertReconstructable(
					x => x.Void(),
					x => x.Void());
			}

			[Fact]
			public void Void_method_call_with_arg()
			{
				AssertReconstructable(
					x => x.VoidWithInt(42),
					x => x.VoidWithInt(42));
			}

			[Fact]
			public void Void_method_call_with_coerced_arg()
			{
				AssertReconstructable(
					x => x.VoidWithLong(42),
					x => x.VoidWithLong(42));
			}

			[Fact]
			public void Void_method_call_with_coerced_nullable_arg()
			{
				AssertReconstructable(
					x => x.VoidWithNullableInt(42),
					x => x.VoidWithNullableInt(42));
			}

			[Fact]
			public void Void_method_call_with_cast_arg()
			{
				AssertReconstructable(
					x => x.VoidWithInt((int)42L),
					x => x.VoidWithInt((int)42L));
			}

			[Fact]
			public void Void_method_call_with_arg_evaluated()
			{
				int arg = 42;
				AssertReconstructable(
					x => x.VoidWithInt(42),
					x => x.VoidWithInt(arg));
			}

			[Fact]
			public void Void_method_call_on_sub_object()
			{
				AssertReconstructable(
					x => x.GetY().Z.Void(),
					x => x.GetY().Z.Void());
			}

			[Fact]
			public void Void_method_call_on_sub_with_several_args()
			{
				AssertReconstructable(
					x => x.GetY(1).Z.VoidWithIntInt(2, 3),
					x => x.GetY(1).Z.VoidWithIntInt(2, 3));
			}

			private void AssertCanReconstruct(string expected, Action<IX> action)
			{
				var expression = ActionObserver.Instance.ReconstructExpression(action);
				Assert.Equal(expected, expression.ToStringFixed());
			}

			private void AssertReconstructable(Expression<Action<IX>> expected, Action<IX> action)
			{
				var actual = ActionObserver.Instance.ReconstructExpression(action);
				Assert.Equal(expected, actual, ExpressionComparer.Default);
			}

			public interface IX
			{
				IY GetY();
				IY GetY(int arg);
				void Void();
				void VoidWithInt(int arg);
				void VoidWithLong(long arg);
				void VoidWithNullableInt(int? arg);
			}

			public interface IY
			{
				IZ Z { get; }
			}

			public interface IZ
			{
				void Void();
				void VoidWithIntInt(int arg1, int arg2);
			}
		}

		public class Error_detection
		{
			[Fact]
			public void Stops_before_non_interceptable_method()
			{
				AssertFailsAfter<X>("x => x...", x => x.NonVirtual());
			}

			[Fact]
			public void Stops_after_non_interceptable_return_type()
			{
				AssertFailsAfter<IX>("x => x.SealedY...", x => x.SealedY.Method());
			}

			private void AssertFailsAfter<TRoot>(string expectedPartial, Action<TRoot> action)
			{
				var error = Assert.Throws<ArgumentException>(() => ActionObserver.Instance.ReconstructExpression(action));
				Assert.Contains($": {expectedPartial}", error.Message);
			}

			public interface IX
			{
				SealedY SealedY { get; }
			}

			public class X
			{
				public void NonVirtual() { }
			}

			public sealed class SealedY
			{
				public void Method() { }
			}
		}
	}
}
