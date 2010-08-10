//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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
			Guard.NotNull(() => underlyingCreateMocks, underlyingCreateMocks);

			this.Expression = Expression.Constant(this);
			this.underlyingCreateMocks = underlyingCreateMocks;
		}

		public MockQueryable(MethodCallExpression underlyingCreateMocks, Expression expression)
		{
			Guard.NotNull(() => underlyingCreateMocks, underlyingCreateMocks);
			Guard.NotNull(() => expression, expression);
			Guard.CanBeAssigned(() => expression, expression.Type, typeof(IQueryable<T>));

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
			return lambda.Compile().Invoke();
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