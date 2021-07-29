// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Language for ReturnSequence
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetupSequentialResult<TResult>
	{
		// would be nice to Mixin somehow the IReturn and IThrows with
		// another ReturnType

		/// <summary>
		/// Returns value
		/// </summary>
		ISetupSequentialResult<TResult> Returns(TResult value);

		/// <summary>
		/// Uses delegate to get return value
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param> 
		ISetupSequentialResult<TResult> Returns(Func<TResult> valueFunction);

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialResult<TResult> Throws(Exception exception);

		/// <summary>
		/// Throws an exception
		/// </summary>
		ISetupSequentialResult<TResult> Throws<TException>() where TException : Exception, new();

		/// <summary>
		/// Uses delegate to throws an exception
		/// </summary>
		ISetupSequentialResult<TResult> Throws<TException>(Func<TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Calls original method
		/// </summary>
		ISetupSequentialResult<TResult> CallBase();
	}
}
