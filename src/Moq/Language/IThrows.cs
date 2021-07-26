// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Throws</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface IThrows : IFluentInterface
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
		IThrowsResult Throws<TException>() where TException : Exception, new();

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked.
		/// </summary>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// Throw a calculated exception when the method is called:
		/// <code>
		/// delegate Exception ExecuteFunc(string name);
		///
		/// String s = ...;
		/// mock.Setup(x => x.Execute(s))
		///     .Throws(new ExecuteFunc((string s) => new Exception(s)));
		/// </code>
		/// </example>
		IThrowsResult Throws(Delegate exceptionFunction);

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked.
		/// </summary>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// Throw a calculated exception when the method is called:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Throws(() => new Exception($"bad {returnValues[0]}"));
		/// </code>
		/// The lambda expression to retrieve the exception is lazy-executed, 
		/// meaning that its exception may change depending on the moment the method 
		/// is executed and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IThrowsResult Throws<TException>(Func<TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T">The type of the argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// Throw a calculated exception which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The lookup list can change between invocations and the setup 
		/// will return different values accordingly. Also, notice how the specific 
		/// string argument is retrieved by simply declaring it as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Throws((string command) => new Exception(command));
		/// </code>
		/// </example>
		IThrowsResult Throws<T, TException>(Func<T, TException> exceptionFunction) where TException : Exception;
	}
}
