// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class ExpressionSplitFixture
	{
		[Fact]
		public void Split_parameter_fails()
		{
			// a => a
			var expression = E((IA a) => a);

			AssertSplitFails(expression);
		}

		[Fact]
		public void Split_parameter_assignment_fails()
		{
			// a => a = ...
			var expression = A((IA a) => a, D<IA>());

			AssertSplitFails(expression);
		}

		[Fact]
		public void Split_delegate_invocation()
		{
			// a => a()
			var expression = E((ADelegate a) => a());

			AssertSplitYields(expression,
				E((ADelegate a) => a()));
		}

		[Fact]
		public void Split_one_property_get_access()
		{
			// a => a.B
			var expression = E((IA a) => a.B);

			AssertSplitYields(expression,
				E((IA a) => a.B));
		}

		[Fact]
		public void Split_one_property_set_access()
		{
			// a => a.B = ...
			var expression = A((IA a) => a.B, D<IB>());

			AssertSplitYields(expression,
				A((IA a) => a.B, D<IB>()));
		}

		[Fact]
		public void Split_two_property_get_accesses()
		{
			// a => a.B.C
			var expression = E((IA a) => a.B.C);

			AssertSplitYields(expression,
				E((IA a) => a.B),
				E((IB b) => b.C));
		}

		[Fact]
		public void Split_one_method_call()
		{
			// a => a.GetB()
			var expression = E((IA a) => a.GetB());

			AssertSplitYields(expression,
				E((IA a) => a.GetB()));
		}

		[Fact]
		public void Split_one_method_call_with_arg()
		{
			// a => a.GetB(...)
			var expression = E((IA a) => a.GetB(1));

			AssertSplitYields(expression,
				E((IA a) => a.GetB(1)));
		}

		[Fact]
		public void Split_two_method_calls()
		{
			// a => a.GetB().GetC()
			var expression = E((IA a) => a.GetB().GetC());

			AssertSplitYields(expression,
				E((IA a) => a.GetB()),
				E((IB b) => b.GetC()));
		}

		[Fact]
		public void Split_two_method_calls_with_args()
		{
			// a => a.GetB(...).GetC(...)
			var expression = E((IA a) => a.GetB(1).GetC(false, true));

			AssertSplitYields(expression,
				E((IA a) => a.GetB(1)),
				E((IB b) => b.GetC(false, true)));
		}

		[Fact]
		public void Split_one_property_get_access_and_one_method_calls()
		{
			// a => a.B.GetC()
			var expression = E((IA a) => a.B.GetC());

			AssertSplitYields(expression,
				E((IA a) => a.B),
				E((IB b) => b.GetC()));
		}

		[Fact]
		public void Split_one_method_call_and_one_property_get_access()
		{
			// a => GetB().C
			var expression = E((IA a) => a.GetB().C);

			AssertSplitYields(expression,
				E((IA a) => a.GetB()),
				E((IB b) => b.C));
		}

		[Fact]
		public void Split_one_method_call_and_one_property_set_access()
		{
			// a => a.GetB().C = ...
			var expression = A((IA a) => a.GetB().C, D<IC>());

			AssertSplitYields(expression,
				E((IA a) => a.GetB()),
				A((IB b) => b.C, D<IC>()));
		}

		[Fact]
		public void Split_one_delegate_invocation_and_one_property_get_access()
		{
			// a => a().C
			var expression = E((ADelegate a) => a().C);

			AssertSplitYields(expression,
				E((ADelegate a) => a()),
				E((IB b) => b.C));
		}

		[Fact]
		public void Split_one_property_getter_and_one_delegate_invocation_and_one_property_get_access()
		{
			// a => a.DelegateB().C
			var expression = E((IA a) => a.DelegateB().C);

			AssertSplitYields(expression,
				E((IA a) => a.DelegateB),
				E((ADelegate a) => a()),
				E((IB b) => b.C));
		}

		[Fact]
		public void Split_one_indexer_get_access()
		{
			// a => a[...]
			var expression = E((IA a) => a[1]);

			AssertSplitYields(expression,
				E((IA a) => a[1]));
		}

		[Fact]
		public void Split_two_indexer_get_accesses()
		{
			// a => a[...][...]
			var expression = E((IA a) => a[1][false, true]);

			AssertSplitYields(expression,
				E((IA a) => a[1]),
				E((IB b) => b[false, true]));
		}

		[Fact]
		public void Split_one_property_get_access_and_one_indexer_access()
		{
			// a => a.B[...]
			var expression = E((IA a) => a.B[true, false]);

			AssertSplitYields(expression,
				E((IA a) => a.B),
				E((IB b) => b[true, false]));
		}

		[Fact]
		public void Split_one_indexer_get_access_and_one_property_get_access()
		{
			// a => a[...].C
			var expression = E((IA a) => a[1].C);

			AssertSplitYields(expression,
				E((IA a) => a[1]),
				E((IB b) => b.C));
		}

		[Fact]
		public void Split_one_method_call_and_one_indexer_get_access()
		{
			// a => a.GetB()[...]
			var expression = E((IA a) => a.GetB()[false, false]);

			AssertSplitYields(expression,
				E((IA a) => a.GetB()),
				E((IB b) => b[false, false]));
		}

		[Fact]
		public void Split_one_indexer_get_access_and_one_method_call()
		{
			// a => a[...].GetC()
			var expression = E((IA a) => a[1].GetC());

			AssertSplitYields(expression,
				E((IA a) => a[1]),
				E((IB b) => b.GetC()));
		}

		[Fact]
		public void Split_one_indexer_set_access()
		{
			// a => a[...] = ...
			var expression = A((IA a) => a[1], D<IB>());

			AssertSplitYields(expression,
				A((IA a) => a[1], D<IB>()));
		}

		[Fact]
		public void Split_one_property_get_access_and_one_indexer_set_access()
		{
			// a => a.B[...] = ...
			var expression = A((IA a) => a.B[true, true], D<IC>());

			AssertSplitYields(expression,
				E((IA a) => a.B),
				A((IB b) => b[true, true], D<IC>()));
		}

		[Fact]
		public void Split_one_indexer_get_access_and_one_property_set_access()
		{
			// a => a[...].C = ...
			var expression = A((IA a) => a[1].C, D<IC>());

			AssertSplitYields(expression,
				E((IA a) => a[1]),
				A((IB b) => b.C, D<IC>()));
		}

		[Fact]
		public void Non_lenient_split_fails_if_rightmost_part_is_non_overridable_property()
		{
			var expression = E((U u) => u.V.SealedW);

			AssertSplitFails(expression, allowNonOverridableLastProperty: false);
		}

		[Fact]
		public void Lenient_split_succeeds_if_rightmost_part_is_non_overridable_property()
		{
			var expression = E((U u) => u.V.SealedW);

			AssertSplitYields(expression, allowNonOverridableLastProperty: true,
				E((U u) => u.V),
				E((V v) => v.SealedW));
		}

		[Fact]
		public void Lenient_split_fails_if_any_part_other_than_the_rightmost_one_is_non_overridable_property()
		{
			var expression = E((U u) => u.SealedV.W);

			AssertSplitFails(expression, allowNonOverridableLastProperty: true);
		}

		private void AssertSplitFails(LambdaExpression expression, params LambdaExpression[] expected)
		{
			Assert.Throws<ArgumentException>(() => expression.Split());
		}

		private void AssertSplitFails(LambdaExpression expression, bool allowNonOverridableLastProperty)
		{
			Assert.ThrowsAny<Exception>(() => expression.Split(allowNonOverridableLastProperty));
		}

		private void AssertSplitYields(LambdaExpression expression, params LambdaExpression[] expected)
		{
			Assert.Equal(expected, expression.Split().Select(e => e.Expression), ExpressionComparer.Default);
		}

		private void AssertSplitYields(LambdaExpression expression, bool allowNonOverridableLastProperty, params LambdaExpression[] expected)
		{
			Assert.Equal(expected, expression.Split(allowNonOverridableLastProperty).Select(e => e.Expression), ExpressionComparer.Default);
		}

		/// <summary>
		///   Helper method producing an assignment <see cref="BinaryExpression"/>.
		///   Useful because C# does not allow assignments in literal expressions.
		/// </summary>
		private static LambdaExpression A<T, TResult>(Expression<Func<T, TResult>> left, Expression right)
		{
			return Expression.Lambda(
				Expression.Assign(
					IndexerReplacer.Instance.Visit(left.Body),
					IndexerReplacer.Instance.Visit(right)),
				left.Parameters);
		}

		/// <summary>
		///   Helper method producing a <see cref="ConstantExpression"/> for the default value of <typeparamref name="T"/>.
		/// </summary>
		private static ConstantExpression D<T>()
		{
			return Expression.Constant(default(T), typeof(T));
		}

		/// <summary>
		///   Helper method producing a <see cref="LambdaExpression"/>.
		/// </summary>
		private static LambdaExpression E<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return (LambdaExpression)IndexerReplacer.Instance.Visit(expression);
		}

		/// <summary>
		/// Expression visitor that compensates the Roslyn C# compiler's encoding
		/// indexer accesses as plain method calls. The visitor "lifts" such method
		/// calls into indexer access representations so we can use them in our unit
		/// tests without having to construct expression trees manually.
		/// </summary>
		private sealed class IndexerReplacer : ExpressionVisitor
		{
			public static readonly IndexerReplacer Instance = new IndexerReplacer();

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Method.IsSpecialName && node.Method.Name == "get_Item")
				{
					var indexer = node.Method.DeclaringType.GetProperty("Item");
					return Expression.MakeIndex(
						this.Visit(node.Object),
						indexer,
						node.Arguments.Select(a => this.Visit(a)));
				}
				else
				{
					return Expression.Call(
						this.Visit(node.Object),
						node.Method,
						node.Arguments.Select(a => this.Visit(a)));
				}
			}
		}

		public interface IA
		{
			IB B { get; set; }
			IB GetB();
			IB GetB(int arg);
			IB this[int index] { get; set; }
			ADelegate DelegateB { get; }
		}

		public interface IB
		{
			IC C { get; set; }
			IC GetC();
			IC GetC(bool arg1, bool arg2);
			IC this[bool arg1, bool arg2] { get; set; }
		}

		public interface IC
		{
		}

		public delegate IB ADelegate();

		public abstract class U
		{
			public abstract V V { get; }
			public V SealedV { get => throw new NotImplementedException(); }
		}

		public abstract class V
		{
			public abstract W W { get; }
			public W SealedW { get => throw new NotImplementedException(); }
		}

		public abstract class W
		{
		}
	}
}
