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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Linq;
using Moq.Properties;
using System.Globalization;

namespace Moq
{
	/// <summary>
	/// Allows querying the universe of mocks for those that behave 
	/// according to the LINQ query specification.
	/// </summary>
	public static class Mocks
	{
		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those 
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <typeparam name="T">The type of mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>() where T : class
		{
			return new MockQueryable<T>();
		}

		/// <summary>
		/// Wraps the enumerator inside a queryable.
		/// </summary>
		internal static IQueryable<T> CreateQueryable<T>() where T : class
		{
			return CreateEnumerable<T>().AsQueryable();
		}

		/// <summary>
		/// Method that is turned into the actual call from .Query{T}, to 
		/// transform the queryable query into a normal enumerable query.
		/// This method should not be used by consumers.
		/// </summary>
		private static IEnumerable<T> CreateEnumerable<T>() where T : class
		{
			do
			{
				var mock = new Mock<T>();
				mock.SetupAllProperties();
				yield return mock.Object;
			}
			while (true);
		}
	}

	/// <summary>
	/// Helper extensions that are used by the query translator.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class QueryableMockExtensions
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
				var property = ((MemberExpression)setup.Body).Member as PropertyInfo;
				if (property == null)
				{
					throw new NotSupportedException(string.Format(
						Resources.FieldsNotSupported,
						CultureInfo.CurrentCulture,
						setup.Body.ToStringFixed()));
				}

				info = property.GetGetMethod();
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
				((Mock<TResult>)fluentMock).SetupAllProperties();
			}

			var result = (TResult)fluentMock.Object;

			mock.Setup(setup).Returns(result);

			return (Mock<TResult>)fluentMock;
		}
	}
}