// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Linq
{
	/// <summary>
	/// A default implementation of IQueryable for use with QueryProvider
	/// </summary>
	internal class MockQueryable<T> : IQueryable<T>, IQueryProvider
	{
		private MethodCallExpression underlyingCreateMocks;

		/// <summary>
		/// The <paramref name="underlyingCreateMocks"/> is a 
		/// static method that returns an IQueryable of Mocks of T which is used to 
		/// apply the linq specification to.
		/// </summary>
		public MockQueryable(MethodCallExpression underlyingCreateMocks)
		{
			Guard.NotNull(underlyingCreateMocks, nameof(underlyingCreateMocks));

			this.Expression = Expression.Constant(this);
			this.underlyingCreateMocks = underlyingCreateMocks;
		}

		public MockQueryable(MethodCallExpression underlyingCreateMocks, Expression expression)
		{
			Guard.NotNull(underlyingCreateMocks, nameof(underlyingCreateMocks));
			Guard.NotNull(expression, nameof(expression));
			Guard.CanBeAssigned(expression.Type, typeof(IQueryable<T>), nameof(expression));

			this.underlyingCreateMocks = underlyingCreateMocks;
			this.Expression = expression;
		}

		public Type ElementType
		{
			get { return typeof(T); }
		}

		public Expression Expression { get; private set; }

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
			return new MockQueryable<TElement>(this.underlyingCreateMocks, expression);
		}

		public object Execute(Expression expression)
		{
			return this.Execute<IQueryable<T>>(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			var replaced = new MockSetupsBuilder(this.underlyingCreateMocks).Visit(expression);

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
