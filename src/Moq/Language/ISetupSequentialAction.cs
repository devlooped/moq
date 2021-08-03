// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Pass</c> and <c>Throws</c> verbs for sequence setups
	/// on <c>void</c> methods.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetupSequentialAction
	{
		/// <summary>
		/// Configures the next call in the sequence to do nothing.
		/// </summary>
		/// <example>
		/// The following code configures the first call to <c>Execute()</c>
		/// to do nothing, and the second call to throw an exception.
		/// <code>
		/// mock.SetupSequence(m => m.Execute())
		///    .Pass()
		///    .Throws&lt;InvalidOperationException&gt;();
		/// </code>
		/// </example>
		ISetupSequentialAction Pass();

		/// <summary>
		/// Configures the next call in the sequence to throw an exception.
		/// </summary>
		/// <example>
		/// The following code configures the first call to <c>Execute()</c>
		/// to do nothing, and the second call to throw an exception.
		/// <code>
		/// mock.SetupSequence(m => m.Execute())
		///    .Pass()
		///    .Throws&lt;InvalidOperationException&gt;();
		/// </code>
		/// </example>
		ISetupSequentialAction Throws<TException>() 
			where TException : Exception, new();

		/// <summary>
		/// Configures the next call in the sequence to throw an exception.
		/// </summary>
		/// <example>
		/// The following code configures the first call to <c>Execute()</c>
		/// to do nothing, and the second call to throw an exception.
		/// <code>
		/// mock.SetupSequence(m => m.Execute())
		///    .Pass()
		///    .Throws(new InvalidOperationException());
		/// </code>
		/// </example>
		ISetupSequentialAction Throws(Exception exception);

		/// <summary>
		/// Configures the next call in the sequence to throw a calculated exception.
		/// </summary>
		/// <example>
		/// The following code configures the first call to <c>Execute()</c>
		/// to do nothing, and the second call to throw a calculated exception.
		/// <code>
		/// mock.SetupSequence(m => m.Execute())
		///    .Pass()
		///    .Throws(() => new InvalidOperationException());
		/// </code>
		/// </example>
		ISetupSequentialAction Throws<TException>(Func<TException> exceptionFunction)
			where TException : Exception;
	}
}
