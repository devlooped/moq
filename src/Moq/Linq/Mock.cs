// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;
using Moq.Linq;

namespace Moq
{
	public partial class Mock
	{
		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>() where T : class
		{
			// This method was originally implemented as follows:
			//
			// return Mocks.CreateMockQuery<T>().First<T>();
			//
			// which involved a lot of avoidable `IQueryable` query provider overhead and lambda compilation.
			// What it really boils down to is this (much faster) code:
			var mock = new Mock<T>();
			mock.SetupAllProperties();
			return mock.Object;
		}

		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <param name="predicate">The predicate with the specification of how the mocked object should behave.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public static T Of<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			var mocked = Mocks.CreateMockQuery<T>().First<T>(predicate);

			// The current implementation of LINQ to Mocks creates mocks that already have recorded invocations.
			// Because this interferes with `VerifyNoOtherCalls`, we recursively clear all invocations before
			// anyone ever gets to see the new created mock.
			//
			// TODO: Make LINQ to Mocks set up mocks without causing invocations of its own, then remove this hack.
			var mock = Mock.Get(mocked);
			mock.Invocations.Clear();
			foreach (var inner in mock.InnerMocks.Values)
			{
				inner.Mock.Invocations.Clear();
			}

			return mocked;
		}
	}
}
