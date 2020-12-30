// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Async
{
	/// <summary>
	///   Converts return values and exceptions to and from instances of a particular awaitable type.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public interface IAwaitableHandler
	{
		/// <summary>
		///   Gets the type of result value represented by instances of this handler's awaitable type.
		/// </summary>
		/// <remarks>
		///   If this awaitable type does not have any result values, this property should return
		///   <see langword="typeof"/>(<see langword="void"/>).
		/// </remarks>
		Type ResultType { get; }

		/// <summary>
		///   Converts the given result value to a successfully completed awaitable.
		/// </summary>
		/// <remarks>
		///   If this awaitable types does not have any result values, <paramref name="result"/> may be ignored.
		/// </remarks>
		object CreateCompleted(object result);

		/// <summary>
		///   Converts the given exception to a faulted awaitable.
		/// </summary>
		object CreateFaulted(Exception exception);

		/// <summary>
		///   Attempts to extract the result value from the given awaitable.
		///   This will succeed only for a successfully completed awaitable that has a result value.
		/// </summary>
		/// <param name="awaitable">The awaitable from which a result value should be extracted.</param>
		/// <param name="result">
		///   If successful, this <see langword="out"/> parameter is set to the extracted result value.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if extraction of a result value succeeded;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		bool TryGetResult(object awaitable, out object result);
	}
}
