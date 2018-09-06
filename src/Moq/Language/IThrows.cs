// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Throws</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IThrows : IFluentInterface
	{
		/// <summary>
		/// Specifies the exception to throw when the method is invoked.
		/// </summary>
		/// <param name="exception">Exception instance to throw.</param>
		/// <example>
		/// This example shows how to throw an exception when the method is 
		/// invoked with an empty string argument:
		/// <code>
		/// mock.Setup(x =&gt; x.Execute(""))
		///     .Throws(new ArgumentException());
		/// </code>
		/// </example>
		IThrowsResult Throws(Exception exception);

		/// <summary>
		/// Specifies the type of exception to throw when the method is invoked.
		/// </summary>
		/// <typeparam name="TException">Type of exception to instantiate and throw when the setup is matched.</typeparam>
		/// <example>
		/// This example shows how to throw an exception when the method is 
		/// invoked with an empty string argument:
		/// <code>
		/// mock.Setup(x =&gt; x.Execute(""))
		///     .Throws&lt;ArgumentException&gt;();
		/// </code>
		/// </example>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		IThrowsResult Throws<TException>() where TException : Exception, new();
	}
}
