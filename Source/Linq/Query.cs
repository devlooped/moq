// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Moq.Linq
{
	internal interface IQueryText
	{
		string GetQueryText(Expression expression);
	}

	/// <summary>
	/// A default implementation of IQueryable for use with QueryProvider
	/// </summary>
	internal class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
	{
		public Query(IQueryProvider provider)
		{
			Guard.NotNull(() => provider, provider);

			this.Provider = provider;
			this.Expression = Expression.Constant(this);
		}

		public Query(QueryProvider provider, Expression expression)
		{
			Guard.NotNull(() => provider, provider);
			Guard.NotNull(() => expression, expression);
			Guard.CanBeAssigned(() => expression, expression.Type, typeof(IQueryable<T>));

			this.Provider = provider;
			this.Expression = expression;
		}

		public Expression Expression { get; private set; }

		public Type ElementType
		{
			get { return typeof(T); }
		}

		public IQueryProvider Provider { get; private set; }

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.Provider.Execute(this.Expression)).GetEnumerator();
		}

		public override string ToString()
		{
			if (this.Expression.NodeType == ExpressionType.Constant &&
				((ConstantExpression)this.Expression).Value == this)
			{
				return "Query(" + typeof(T) + ")";
			}

			return this.Expression.ToString();
		}

		public string QueryText
		{
			get
			{
				var queryText = this.Provider as IQueryText;
				if (queryText != null)
				{
					return queryText.GetQueryText(this.Expression);
				}

				return string.Empty;
			}
		}
	}
}