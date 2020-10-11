// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Moq.Linq
{
	/// <summary>
	/// A default implementation of IQueryable for use with QueryProvider
	/// </summary>
	internal class MockQueryable<T> : IQueryable<T>, IQueryProvider
	{
		private readonly Expression expression;

		public MockQueryable(Expression expression)
		{
			Debug.Assert(expression != null);

			Guard.ImplementsInterface(typeof(IQueryable<T>), expression.Type, nameof(expression));

			this.expression = expression;
		}

		public Type ElementType
		{
			get { return typeof(T); }
		}

		public Expression Expression => this.expression;

		public IQueryProvider Provider
		{
			get { return this; }
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return this.CreateQuery<T>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new MockQueryable<TElement>(expression);
		}

		public object Execute(Expression expression)
		{
			return this.Execute<IQueryable<T>>(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			var replaced = new MockSetupsBuilder().Visit(expression);

			var lambda = Expression.Lambda<Func<TResult>>(replaced);
			return lambda.CompileUsingExpressionCompiler().Invoke();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.Provider.Execute<IQueryable<T>>(this.Expression).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			if (this.Expression.NodeType == ExpressionType.Constant && ((ConstantExpression)this.Expression).Value == this)
			{
				return "Query(" + typeof(T) + ")";
			}

			return this.Expression.ToString();
		}
	}
}
