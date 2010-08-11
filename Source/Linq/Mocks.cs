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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Linq;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Allows querying the universe of mocks for those that behave 
	/// according to the LINQ query specification.
	/// </summary>
	/// <devdoc>
	/// This entry-point into Linq to Mocks is the only one in the root Moq 
	/// namespace to ease discovery. But to get all the mocking extension 
	/// methods on Object, a using of Moq.Linq must be done, so that the 
	/// polluting of the intellisense for all objects is an explicit opt-in.
	/// </devdoc>
	public static class Mocks
	{
		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those 
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>() where T : class
		{
			return CreateMockQuery<T>();
		}

		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those 
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <param name="specification">The predicate with the setup expressions.</param>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public static IQueryable<T> Of<T>(Expression<Func<T, bool>> specification) where T : class
		{
			return CreateMockQuery<T>().Where(specification);
		}

		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Moved to Mock.Of<T>, as it's a single one, so no reason to be on Mocks.", true)]
		public static T OneOf<T>() where T : class
		{
			return CreateMockQuery<T>().First<T>();
		}

		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <param name="specification">The predicate with the setup expressions.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Moved to Mock.Of<T>, as it's a single one, so no reason to be on Mocks.", true)]
		public static T OneOf<T>(Expression<Func<T, bool>> specification) where T : class
		{
			return CreateMockQuery<T>().First<T>(specification);
		}

		/// <summary>
		/// Creates the mock query with the underlying queriable implementation.
		/// </summary>
		internal static IQueryable<T> CreateMockQuery<T>() where T : class
		{
			return new MockQueryable<T>(Expression.Call(null,
				((Func<IQueryable<T>>)CreateQueryable<T>).Method));
		}

		/// <summary>
		/// Wraps the enumerator inside a queryable.
		/// </summary>
		internal static IQueryable<T> CreateQueryable<T>() where T : class
		{
			return CreateMocks<T>().AsQueryable();
		}

		/// <summary>
		/// Method that is turned into the actual call from .Query{T}, to 
		/// transform the queryable query into a normal enumerable query.
		/// This method is never used directly by consumers.
		/// </summary>
		private static IEnumerable<T> CreateMocks<T>() where T : class
		{
			do
			{
				var mock = new Mock<T>();
				mock.SetupAllProperties();

				yield return mock.Object;
			}
			while (true);
		}

		/// <summary>
		/// Extension method used to support Linq-like setup properties that are not virtual but do have 
		/// a getter and a setter, thereby allowing the use of Linq to Mocks to quickly initialize Dtos too :)
		/// </summary>
		internal static bool SetPropery<T, TResult>(Mock<T> target, Expression<Func<T, TResult>> propertyReference, TResult value)
			where T : class
		{
			var memberExpr = (MemberExpression)propertyReference.Body;
			var member = (PropertyInfo)memberExpr.Member;

			member.SetValue(target.Object, value, null);

			return true;
		}
	}

	/// <summary>
	/// Helper extensions that are used by the query translator.
	/// </summary>
	internal static class QueryableMockExtensions
	{
		/// <summary>
		/// Retrieves a fluent mock from the given setup expression.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static Mock<TResult> FluentMock<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> setup)
			where T : class
			where TResult : class
		{
			Guard.NotNull(() => mock, mock);
			Guard.NotNull(() => setup, setup);
			typeof(TResult).ThrowIfNotMockeable();

			MethodInfo info;
			if (setup.Body.NodeType == ExpressionType.MemberAccess)
			{
				var memberExpr = ((MemberExpression)setup.Body);
				memberExpr.ThrowIfNotMockeable();

				info = ((PropertyInfo)memberExpr.Member).GetGetMethod();
			}
			else if (setup.Body.NodeType == ExpressionType.Call)
			{
				info = ((MethodCallExpression)setup.Body).Method;
			}
			else
			{
				throw new NotSupportedException("Unsupported expression: " + setup.ToStringFixed());
			}

			info.ReturnType.ThrowIfNotMockeable();

			Mock fluentMock;
			if (!mock.InnerMocks.TryGetValue(info, out fluentMock))
			{
				fluentMock = ((IMocked)new MockDefaultValueProvider(mock).ProvideDefault(info)).Mock;
				Mock.SetupAllProperties(fluentMock);
			}

			var result = (TResult)fluentMock.Object;

			mock.Setup(setup).Returns(result);

			return (Mock<TResult>)fluentMock;
		}
	}
}