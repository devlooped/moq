// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Linq;

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
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>() where T : class
		{
			return Mocks.Of<T>(MockBehavior.Default);
		}

		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <param name="behavior">Behavior of the mocks.</param>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>(MockBehavior behavior) where T : class
		{
			return Mocks.CreateMockQuery<T>(behavior);
		}

		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those 
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <param name="specification">The predicate with the setup expressions.</param>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>(Expression<Func<T, bool>> specification) where T : class
		{
			return Mocks.Of<T>(specification, MockBehavior.Default);
		}

		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <param name="specification">The predicate with the setup expressions.</param>
		/// <param name="behavior">Behavior of the mocks.</param>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public static IQueryable<T> Of<T>(Expression<Func<T, bool>> specification, MockBehavior behavior) where T : class
		{
			return Mocks.CreateMockQuery<T>(behavior).Where(specification);
		}

		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Moved to Mock.Of<T>, as it's a single one, so no reason to be on Mocks.", true)]
		public static T OneOf<T>() where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <param name="specification">The predicate with the setup expressions.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Moved to Mock.Of<T>, as it's a single one, so no reason to be on Mocks.", true)]
		public static T OneOf<T>(Expression<Func<T, bool>> specification) where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates the mock query with the underlying queryable implementation.
		/// </summary>
		internal static IQueryable<T> CreateMockQuery<T>(MockBehavior behavior) where T : class
		{
			var method = ((Func<MockBehavior, IQueryable<T>>)CreateQueryable<T>).GetMethodInfo();
			return new MockQueryable<T>(Expression.Call(method, Expression.Constant(behavior)));
		}

		/// <summary>
		/// Wraps the enumerator inside a queryable.
		/// </summary>
		internal static IQueryable<T> CreateQueryable<T>(MockBehavior behavior) where T : class
		{
			return Mocks.CreateMocks<T>(behavior).AsQueryable();
		}

		/// <summary>
		/// Method that is turned into the actual call from .Query{T}, to 
		/// transform the queryable query into a normal enumerable query.
		/// This method is never used directly by consumers.
		/// </summary>
		private static IEnumerable<T> CreateMocks<T>(MockBehavior behavior) where T : class
		{
			do
			{
				var mock = new Mock<T>(behavior);
				if (behavior != MockBehavior.Strict)
				{
					mock.SetupAllProperties();
				}

				yield return mock.Object;
			}
			while (true);
		}
	}
}
