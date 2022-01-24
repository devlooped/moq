// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moq.Async
{
	/// <summary>
	/// </summary>
	public interface IAwaitableFactory
	{
		/// <summary>
		/// </summary>
		Type ResultType { get; }

		/// <summary>
		/// </summary>
		object CreateCompleted(object result = null);

		/// <summary>
		/// </summary>
		object CreateFaulted(Exception exception);

		/// <summary>
		/// </summary>
		object CreateFaulted(IEnumerable<Exception> exceptions);

		/// <summary>
		/// </summary>
		Expression CreateResultExpression(Expression awaitableExpression);

		/// <summary>
		/// </summary>
		bool TryGetResult(object awaitable, out object result);
	}
}
