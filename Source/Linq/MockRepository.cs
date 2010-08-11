using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Linq.Expressions;
using Moq.Linq;
using System.Reflection;

namespace Moq
{
#pragma warning disable 618
	public partial class MockRepository
	{
#pragma warning restore 618

		/// <summary>
		/// Access the universe of mocks of the given type, to retrieve those 
		/// that behave according to the LINQ query specification.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object to query.</typeparam>
		public IQueryable<T> Of<T>() where T : class
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
		public IQueryable<T> Of<T>(Expression<Func<T, bool>> specification) where T : class
		{
			return CreateMockQuery<T>().Where(specification);
		}

		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public T OneOf<T>() where T : class
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
		public T OneOf<T>(Expression<Func<T, bool>> specification) where T : class
		{
			return CreateMockQuery<T>().First<T>(specification);
		}

		/// <summary>
		/// Creates the mock query with the underlying queriable implementation.
		/// </summary>
		internal IQueryable<T> CreateMockQuery<T>() where T : class
		{
			return new MockQueryable<T>(Expression.Call(
				Expression.Constant(this),
				((Func<IQueryable<T>>)CreateQueryable<T>).Method));
		}

		/// <summary>
		/// Wraps the enumerator inside a queryable.
		/// </summary>
		internal IQueryable<T> CreateQueryable<T>() where T : class
		{
			return CreateMocks<T>().AsQueryable();
		}

		/// <summary>
		/// Method that is turned into the actual call from .Query{T}, to 
		/// transform the queryable query into a normal enumerable query.
		/// This method is never used directly by consumers.
		/// </summary>
		private IEnumerable<T> CreateMocks<T>() where T : class
		{
			do
			{
				var mock = this.Create<T>();
				mock.SetupAllProperties();

				yield return mock.Object;
			}
			while (true);
		}
	}
}
