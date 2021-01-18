// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Moq
{
	public partial class Mock
	{
		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>() where T : class
		{
			return Mock.Of<T>(MockBehavior.Default);
		}

		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>(MockBehavior behavior) where T : class
		{
			// This method was originally implemented as follows:
			//
			// return Mocks.CreateMockQuery<T>().First<T>();
			//
			// which involved a lot of avoidable `IQueryable` query provider overhead and lambda compilation.
			// What it really boils down to is this (much faster) code:
			var mock = new Mock<T>(behavior);
			if (behavior != MockBehavior.Strict)
			{
				mock.SetupAllProperties();
			}
			return mock.Object;
		}

		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <param name="predicate">The predicate with the specification of how the mocked object should behave.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			return Mock.Of<T>(predicate, MockBehavior.Default);
		}

		/// <summary>
		/// Creates a mock object of the indicated type.
		/// </summary>
		/// <param name="predicate">The predicate with the specification of how the mocked object should behave.</param>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>(Expression<Func<T, bool>> predicate, MockBehavior behavior) where T : class
		{
			return Mocks.CreateMockQuery<T>(behavior).First(predicate);
		}
	}
}
