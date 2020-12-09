// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class MockExtensions
	{
		/// <summary>
		/// Resets this mock's state. This includes its setups, configured default return values, registered event handlers, and all recorded invocations.
		/// </summary>
		/// <param name="mock">The mock whose state should be reset.</param>
		public static void Reset(this Mock mock)
		{
			mock.ConfiguredDefaultValues.Clear();
			mock.MutableSetups.Clear();
			mock.EventHandlers.Clear();
			mock.Invocations.Clear();
		}

		/// <summary>
		/// Gets the invocations on this mock of the specified method. The arguments of the specified method will be ignored.
		/// </summary>
		/// <typeparam name="T">The mocked type.</typeparam>
		/// <param name="mock">The mock which should be queried for invocations.</param>
		/// <param name="methodCall">Lambda expression that specifies the method invocation for which to return the invocations.</param>
		/// <returns>A list of all invocations of the specified method on this mock.</returns>
		/// <example>
		///   <code>
		///     var mock = new Mock&lt;IProcessor&gt;();
		///     
		///     ... // exercise mock
		///     
		///     var invocationsOnExecute = mock.GetInvocationsOf(x => x.Execute(default(string)));
		///   </code>
		/// </example>
		public static IEnumerable<IInvocation> GetInvocationsOf<T>(this Mock<T> mock, Expression<Action<T>> methodCall) where T : class
		{
			var methodCallExpression = (MethodCallExpression)methodCall.Body;
			var calledMethod = methodCallExpression.Method;

			var invocationsOfMethod = from invocation in mock.Invocations
									   let method = invocation.Method
									   where method == calledMethod
									   select invocation;

			return invocationsOfMethod;
		}
	}
}
