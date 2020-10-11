// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Allows to create parameter captures in setup expressions.
	/// </summary>
	public static class Capture
	{
		/// <summary>
		/// Creates a parameter capture that will store values in a collection.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <param name="collection">The collection that will store captured parameter values</param>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var parameters = new List{string}();
		/// mock.Setup(x => x.DoSomething(Capture.In(parameters)));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", parameters.Single());
		/// </code>
		/// </example>
		public static T In<T>(ICollection<T> collection)
		{
			var match = new CaptureMatch<T>(collection.Add);
			return With(match);
		}

		/// <summary>
		/// Creates a parameter capture that will store specific values in a collection.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <param name="collection">The collection that will store captured parameter values</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var parameters = new List{string}();
		/// mock.Setup(x => x.DoSomething(Capture.In(parameters, p => p.StartsWith("W"))));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", parameters.Single());
		/// </code>
		/// </example>
		public static T In<T>(IList<T> collection, Expression<Func<T, bool>> predicate)
		{
			var match = new CaptureMatch<T>(collection.Add, predicate);
			return With(match);
		}

		/// <summary>
		/// Creates a parameter capture using specified <see cref="CaptureMatch{T}"/>.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var capturedValue = string.Empty;
		/// var match = new CaptureMatch{string}(x => capturedValue = x);
		/// mock.Setup(x => x.DoSomething(Capture.With(match)));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", capturedValue);
		/// </code>
		/// </example>
		public static T With<T>(CaptureMatch<T> match)
		{
			Match.Register(match);
			return default(T);
		}
	}
}
