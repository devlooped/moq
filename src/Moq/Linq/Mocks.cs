// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
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
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
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
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
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

		/// <summary>
		/// Extension method used to support Linq-like setup properties that are not virtual but do have 
		/// a getter and a setter, thereby allowing the use of Linq to Mocks to quickly initialize DTOs too :)
		/// </summary>
		private static bool SetProperty(Mock target, PropertyInfo property, object value)
		{
			// For strict mocks, we haven't called `SetupAllProperties` on the mock being set up.
			// Therefore, whenever a property is being initialized, we quickly need to enable auto-stubbing.
			//
			// (One would think that it would be simpler to perform `SetupAllProperties` at the beginning
			// and leave it enabled until the initialized mock is returned to the user. However, transforming
			// the LINQ query such that a final disable of auto-stubbing happens is much more difficult!)

			var temporaryAutoSetupProperties = target.AutoSetupPropertiesDefaultValueProvider == null;

			if (temporaryAutoSetupProperties)
			{
				target.AutoSetupPropertiesDefaultValueProvider = target.DefaultValueProvider;
			}
			try
			{
				property.SetValue(target.Object, value, null);
			}
			finally
			{
				if (temporaryAutoSetupProperties)
				{
					target.AutoSetupPropertiesDefaultValueProvider = null;
				}
			}

			return true;
		}

		internal static readonly MethodInfo SetupReturnsMethod =
			typeof(Mocks).GetMethod(nameof(SetupReturns), BindingFlags.NonPublic | BindingFlags.Static);

		internal static bool SetupReturns(Mock mock, LambdaExpression expression, object value)
		{
			PropertyInfo propertyToSet = null;

			if (expression.Body is MemberExpression me
				&& me.Member is PropertyInfo pi
				&& !(pi.CanRead(out var getter) && getter.CanOverride() && ProxyFactory.Instance.IsMethodVisible(getter, out _))
				&& pi.CanWrite(out _))
			{
				// LINQ to Mocks allows setting non-interceptable properties, which is handy e.g. when initializing DTOs.
				// Generally, we prefer having a setup for a property's return value, but if that isn't possible (e.g.
				// with a non-virtual or sealed property), we will have to fall back to simply setting that property
				// through reflection.
				//
				// The above condition detects exactly those cases.
				propertyToSet = pi;

				// The property access (which is one that cannot be setup) must be subtracted from the setup expression:
				expression = Expression.Lambda(me.Expression, expression.Parameters);
				if (!expression.CanSplit())
				{
					Mocks.SetProperty(mock, propertyToSet, value);
					return true;
				}
			}

			var setup = Mock.Setup(mock, expression, condition: null);

			if (propertyToSet == null)
			{
				setup.SetEagerReturnsResponse(value);
			}
			else
			{
				Mocks.SetProperty(setup.Mock, propertyToSet, value);
			}

			return true;
		}
	}
}
