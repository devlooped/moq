// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class CSharpCompilerExpressionsFixture
	{
		/// <summary>
		///   Documents some cases that can lead to the C# compiler introducing / not introducing
		///   `<see cref="ExpressionType.Convert"/>` nodes in <see cref="Expression"/>s. These
		///   tests are here to guide us when deciding whether we should add/remove such nodes.
		/// </summary>
		public class Convert_nodes
		{
			[Fact]
			public void Null_literal_for_nullable_parameter()
			{
				AssertNoConvert(x => x.NullableInt(null));
			}

			[Fact]
			public void Non_null_literal_value_for_nullable_parameter()
			{
				AssertConvert(x => x.NullableInt(0));
			}

			[Fact]
			public void Null_value_in_nullable_variable_for_nullable_parameter()
			{
				int? arg = null;
				AssertNoConvert(x => x.NullableInt(arg));
			}

			[Fact]
			public void Non_nullable_variable_for_nullable_parameter()
			{
				int arg = 0;
				AssertConvert(x => x.NullableInt(arg));
			}

			[Fact]
			public void Non_null_literal_value_cast_to_nullable_type_for_nullable_parameter()
			{
				AssertConvert(x => x.NullableInt((int?)0));
			}

			[Fact]
			public void Non_null_but_nullable_variable_for_nullable_parameter()
			{
				int? arg = 0;
				AssertNoConvert(x => x.NullableInt(arg));
			}

			[Fact]
			public void Boxing_of_literal_value()
			{
				AssertConvert(x => x.Object(0));
			}

			[Fact]
			public void Boxing_of_variable()
			{
				int arg = 0;
				AssertConvert(x => x.Object(arg));
			}

			[Fact]
			public void Boxed_value()
			{
				object arg = 0;
				AssertNoConvert(x => x.Object(arg));
			}

			[Fact]
			public void Widening_of_literal_value_1()
			{
				AssertNoConvert(x => x.Long(0));
			}

			[Fact]
			public void Widening_of_literal_value_2()
			{
				AssertNoConvert(x => x.Int((short)0));
			}

			[Fact]
			public void Widening_of_variable_1()
			{
				int arg = 0;
				AssertConvert(x => x.Long(arg));
			}

			[Fact]
			public void Widening_of_variable_2()
			{
				short arg = 0;
				AssertConvert(x => x.Int(arg));
			}

			[Fact]
			public void Narrowing_of_literal_value()
			{
				AssertNoConvert(x => x.Short(0));
			}

			[Fact]
			public void Narrowing_of_variable()
			{
				int arg = 0;
				AssertConvert(x => x.Short(arg));
			}

			[Fact]
			public void Downcast_of_interface_variable()
			{
				AssertNoConvert(x => x.Object(x));
			}

			public interface IX
			{
				void Int(int arg);
				void Long(long arg);
				void NullableInt(int? arg);
				void Object(object arg);
				void Short(long arg);
			}

			private static void AssertConvert(Expression<Action<IX>> expression)
			{
				var visitor = new FilteringVisitor(e => e.NodeType == ExpressionType.Convert);
				visitor.Visit(expression.Body);
				Assert.True(visitor.Result.Any());
			}

			private static void AssertNoConvert(Expression<Action<IX>> expression)
			{
				var visitor = new FilteringVisitor(e => e.NodeType == ExpressionType.Convert);
				visitor.Visit(expression.Body);
				Assert.False(visitor.Result.Any());
			}
		}

		private sealed class FilteringVisitor : ExpressionVisitor
		{
			private readonly Func<Expression, bool> predicate;
			private readonly List<Expression> result;

			public FilteringVisitor(Func<Expression, bool> predicate)
			{
				this.predicate = predicate;
				this.result = new List<Expression>();
			}

			public IReadOnlyList<Expression> Result => this.result;

			public override Expression Visit(Expression node)
			{
				if (this.predicate(node))
				{
					this.result.Add(node);
				}

				return base.Visit(node);
			}
		}
	}
}
