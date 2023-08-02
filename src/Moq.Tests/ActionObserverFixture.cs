// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
					"x => x.VoidWithNullableInt(42)",
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

			[Fact]
			public void Void_method_call_with_matcher()
			{
				AssertReconstructable(
					x => x.VoidWithInt(It.IsAny<int>()),
					x => x.VoidWithInt(It.IsAny<int>()));
			}

			[Fact]
			public void Void_method_call_with_matcher_of_assignment_compatible_type()
			{
				AssertReconstructable(
					x => x.VoidWithObject(It.IsAny<int>()),
					x => x.VoidWithObject(It.IsAny<int>()));
			}

			[Fact]
			public void Void_method_call_with_matcher_in_first_of_three_invocations()
			{
				AssertReconstructable(
					x => x.GetY(It.IsAny<int>()).Z.VoidWithIntInt(0, 0),
					x => x.GetY(It.IsAny<int>()).Z.VoidWithIntInt(0, 0));
			}

			[Fact]
			public void Void_method_call_with_matcher_in_third_of_three_invocations_1()
			{
				AssertReconstructable(
					x => x.GetY(0).Z.VoidWithIntInt(1, It.IsAny<int>()),
					x => x.GetY(0).Z.VoidWithIntInt(1, It.IsAny<int>()));
			}

			[Fact]
			public void Void_method_call_with_matcher_in_third_of_three_invocations_2()
			{
				AssertReconstructable(
					x => x.GetY(0).Z.VoidWithIntInt(It.IsAny<int>(), 2),
					x => x.GetY(0).Z.VoidWithIntInt(It.IsAny<int>(), 2));
			}

			[Fact]
			public void Void_method_call_with_matcher_in_first_and_third_of_three_invocations()
			{
				AssertReconstructable(
					"x => x.GetY(It.Is<int>(i => i % 2 == 0)).Z.VoidWithIntInt(It.IsAny<int>(), 2)",
					 x => x.GetY(It.Is<int>(i => i % 2 == 0)).Z.VoidWithIntInt(It.IsAny<int>(), 2));
			}

			[Fact]
			public void Method_with_matchers_after_default_arg()
			{
				// This demonstrates that even though the first argument has a default value,
				// the matcher isn't placed there, because it has a type (string) that won't fit (int).

				AssertReconstructable(
					x => x.VoidWithIntString(0, It.IsAny<string>()),
					x => x.VoidWithIntString(0, It.IsAny<string>()));
			}

			[Fact]
			public void Assignment()
			{
				AssertReconstructable(
					"x => x.GetY().Z.Property = \"value\"",
					 x => x.GetY().Z.Property =  "value" );
			}

			[Fact]
			public void Assignment_with_captured_var_on_rhs()
			{
				var arg = "value";
				AssertReconstructable(
					"x => x.GetY().Z.Property = \"value\"",
					 x => x.GetY().Z.Property = arg);
			}

			[Fact]
			public void Assignment_with_matcher_on_rhs()
			{
				AssertReconstructable(
					"x => x.GetY().Z.Property = It.IsAny<string>()",
					 x => x.GetY().Z.Property = It.IsAny<string>());
			}

			[Fact]
			public void Indexer_assignment_with_arg()
			{
				AssertReconstructable(
					"x => x[1] = null",
					 x => x[1] = null);
			}

			[Fact]
			public void Indexer_assignment_with_matcher_on_lhs_1()
			{
				AssertReconstructable(
					"x => x[It.IsAny<int>()] = null",
					 x => x[It.IsAny<int>()] = null);
			}

			[Fact]
			public void Indexer_assignment_with_matcher_on_lhs_2()
			{
				AssertReconstructable(
					"x => x[1, It.IsAny<int>()] = 0",
					 x => x[1, It.IsAny<int>()] = 0);
			}

			[Fact]
			public void Indexer_assignment_with_matcher_on_rhs()
			{
				AssertReconstructable(
					"x => x[1] = It.IsAny<ActionObserverFixture.Reconstructibility.IY>()",
					 x => x[1] = It.IsAny<ActionObserverFixture.Reconstructibility.IY>());
			}

			[Fact]
			public void Indexer_assignment_with_matchers_everywhere()
			{
				AssertReconstructable(
					"x => x[It.Is<int>(i => i == 0), It.Is<int>(i => i == 2)] = It.Is<int>(i => i == 3)",
					 x => x[It.Is<int>(i => i == 0), It.Is<int>(i => i == 2)] = It.Is<int>(i => i == 3));
			}

			[Fact]
			public void Widening_and_narrowing_and_enum_convertions()
			{
				ushort arg = 123;
				AssertReconstructable(
					"x => x.VoidWithShort(123)",
					 x => x.VoidWithShort((short)arg));
				AssertReconstructable(
					"x => x.VoidWithInt(123)",
					 x => x.VoidWithInt(arg));
				AssertReconstructable(
					"x => x.VoidWithLong(123)",
					 x => x.VoidWithLong(arg));

				long longArg = 654L;
				AssertReconstructable(
					"x => x.VoidWithShort(654)",
					 x => x.VoidWithShort((short)longArg));

				AssertReconstructable(
					"x => x.VoidWithObject(ActionObserverFixture.Reconstructibility.Color.Green)",
					 x => x.VoidWithObject(Color.Green));
				AssertReconstructable(
					"x => x.VoidWithEnum(ActionObserverFixture.Reconstructibility.Color.Green)",
					 x => x.VoidWithEnum(Color.Green));
				AssertReconstructable(
					"x => x.VoidWithNullableEnum(ActionObserverFixture.Reconstructibility.Color.Green)",
					 x => x.VoidWithNullableEnum(Color.Green));
			}

			[Fact]
			public void It_IsAny_enum_converted_and_assigned_to_int_parameter()
			{
				AssertReconstructable(
					x => x.VoidWithInt((int)It.IsAny<Color>()),
					x => x.VoidWithInt((int)It.IsAny<Color>()));
			}

			[Fact]
			public void It_IsAny_short_widened_to_int_parameter()
			{
				AssertReconstructable(
					x => x.VoidWithInt(It.IsAny<short>()),
					x => x.VoidWithInt(It.IsAny<short>()));
			}

			private void AssertReconstructable(string expected, Action<IX> action)
			{
				Expression actual = ActionObserver.Instance.ReconstructExpression(action);
				actual = PrepareForComparison.Instance.Visit(actual);
				Assert.Equal(expected, actual.ToStringFixed());
			}

			private void AssertReconstructable(Expression<Action<IX>> expected, Action<IX> action)
			{
				Expression actual = ActionObserver.Instance.ReconstructExpression(action);
				expected = (Expression<Action<IX>>)PrepareForComparison.Instance.Visit(expected);
				actual = PrepareForComparison.Instance.Visit(actual);
				Assert.Equal(expected, actual, ExpressionComparer.Default);
			}

			public interface IX
			{
				IY this[int index] { get; set; }
				int this[int index1, int index2] { get; set; }
				IY GetY();
				IY GetY(int arg);
				void Void();
				void VoidWithShort(short arg);
				void VoidWithInt(int arg);
				void VoidWithLong(long arg);
				void VoidWithNullableInt(int? arg);
				void VoidWithIntString(int arg1, string arg2);
				void VoidWithObject(object arg);
				void VoidWithEnum(Color arg);
				void VoidWithNullableEnum(Color? arg);
			}

			public interface IY
			{
				IZ Z { get; }
			}

			public interface IZ
			{
				object Property { get; set; }
				void Void();
				void VoidWithIntInt(int arg1, int arg2);
			}

			public enum Color
			{
				Red,
				Green,
				Blue,
				Yellow,
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
			public void Stops_before_non_interceptable_property()
			{
				AssertFailsAfter<X>("x => x...", x => x.NonVirtualProperty = It.IsAny<IY>());
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
				IY Y { get; }
				SealedY SealedY { get; }
			}

			public class X
			{
				public IY NonVirtualProperty { get; set; }
				public void NonVirtual() { }
			}

			public interface IY
			{
				void Method(int arg1, int arg2);
			}

			public sealed class SealedY
			{
				public void Method() { }
			}
		}

		// These tests document limitations of the current implementation.
		public class Limitations
		{
			[Fact]
			public void Method_with_matchers_after_default_arg()
			{
				// This is because parameters with default values are filled from left to right.

				AssertIncorrectlyReconstructsAs(
					x => x.Method(It.IsAny<int>(), 0              ),
					x => x.Method(0              , It.IsAny<int>()));
			}

			[Fact]
			public void Indexer_with_default_value_on_lfs_and_matcher_on_rhs_both_having_same_types()
			{
				// Same as above, since LHS and RHS are actually both part of a single parameter list of a method call `get_Item(...lhs, rhs).
				AssertIncorrectlyReconstructsAs(
					"x => x[It.IsAny<int>()] = 0",
					 x => x[0              ] = It.IsAny<int>());
			}

			private void AssertIncorrectlyReconstructsAs(string expected, Action<IX> action)
			{
				Expression actual = ActionObserver.Instance.ReconstructExpression(action);
				actual = PrepareForComparison.Instance.Visit(actual);
				Assert.Equal(expected, actual.ToStringFixed());
			}

			private void AssertIncorrectlyReconstructsAs(Expression<Action<IX>> expected, Action<IX> action)
			{
				Expression actual = ActionObserver.Instance.ReconstructExpression(action);
				expected = (Expression<Action<IX>>)PrepareForComparison.Instance.Visit(expected);
				actual = PrepareForComparison.Instance.Visit(actual);
				Assert.Equal(expected, actual, ExpressionComparer.Default);
			}

			public interface IX
			{
				int this[int index] { get; set; }
				void Method(int arg1, int arg2);
			}
		}

		// The expression trees reconstructed by `ActionObserver` may differ from those
		// produced by the Roslyn compilers in some minor regards that shouldn't actually
		// matter to program execution; however `ExpressionComparer` will notice the
		// differences, making above tests fail. Because of this, we try to "equalize"
		// expressions created by the Roslyn compilers (`expected`) and those produced
		// by `ActionObserver` (`actual`) using this expression visitor:
		private sealed class PrepareForComparison : ExpressionVisitor
		{
			public static readonly PrepareForComparison Instance = new PrepareForComparison();

			protected override Expression VisitExtension(Expression node)
			{
				if (node is MatchExpression me)
				{
					// Resolve `MatchExpression`s to their matcher's `RenderExpression`:
					return me.Match.RenderExpression;
				}
				else
				{
					return base.VisitExtension(node);
				}
			}
		}
	}
}
