// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Text;

using Xunit;

namespace Moq.Tests
{
	public class StringBuilderExtensionsFixture
	{
		[Fact]
		public void AppendExpression_formats_call_to_indexer_setter_method_using_indexer_syntax()
		{
			// foo => foo.set_Item("index", "value")
			var foo = Expression.Parameter(typeof(IFoo), "foo");
			var expression =
				Expression.Lambda<Action<IFoo>>(
					Expression.Call(
						foo,
						typeof(IFoo).GetProperty("Item").SetMethod,
						Expression.Constant("index"),
						Expression.Constant("value")),
					foo);

			Assert.Equal(@"foo => foo[""index""] = ""value""", GetAppendExpressionResult(expression));
		}

		[Fact]
		public void AppendExpression_formats_ternary_conditional_expression_correctly()
		{
			// 1 == 2 ? 3 : 4
			var expression = Expression.Condition(
				Expression.Equal(Expression.Constant(1), Expression.Constant(2)),
				Expression.Constant(3),
				Expression.Constant(4));

			Assert.Equal(@"1 == 2 ? 3 : 4", GetAppendExpressionResult(expression));
		}

		[Fact]
		public void AppendExpression_formats_is_operator_correctly()
		{
			// 1 is string
			var expression = Expression.TypeIs(Expression.Constant(1), typeof(string));

			Assert.Equal(@"1 is string", GetAppendExpressionResult(expression));
		}

		private string GetAppendExpressionResult(Expression expression)
		{
			return new StringBuilder().AppendExpression(expression).ToString();
		}

		public interface IFoo
		{
			object this[object index] { get; set; }
		}
	}
}
