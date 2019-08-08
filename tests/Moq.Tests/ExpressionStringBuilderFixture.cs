// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class ExpressionStringBuilderFixture
	{
		[Fact]
		public void Formats_call_to_indexer_setter_method_using_indexer_syntax()
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

			Assert.Equal(@"foo => foo[""index""] = ""value""", Format(expression));
		}

		private static string Format(Expression expression)
		{
			return new ExpressionStringBuilder().Append(expression).ToString();
		}

		public interface IFoo
		{
			object this[object index] { get; set; }
		}
	}
}
