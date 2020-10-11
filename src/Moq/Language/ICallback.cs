// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb and overloads.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface ICallback : IFluentInterface
	{
		/// <summary>
		///   Specifies a callback to invoke when the method is called that receives the original <see cref="IInvocation"/>.
		///   <para>
		///     This overload is intended to be used in scenarios involving generic type argument matchers
		///     (such as <see cref="It.IsAnyType"/>). The callback will receive the current <see cref="IInvocation"/>,
		///     which allows discovery of both arguments and type arguments.
		///   </para>
		///   <para>
		///     For all other use cases, you should prefer the other <c>Callback</c> overloads as they provide
		///     better static type safety.
		///   </para>
		/// </summary>
		/// <example>
		///   <code>
		///     Figure out the generic type argument used for a mocked method call:
		///     mock.Setup(m => m.DoSomethingWith&lt;It.IsAnyType&gt;(...))
		///         .Callback(new InvocationAction(invocation =>
		///                  {
		///                      var typeArgument = invocation.Method.GetGenericArguments()[0];
		///                      // do something interesting with the type argument
		///                  });
		///     mock.Object.DoSomethingWith&lt;Something&gt;();
		///   </code>
		/// </example>
		ICallbackResult Callback(InvocationAction action);

		/// <summary>
		/// Specifies a callback of any delegate type to invoke when the method is called.
		/// This overload specifically allows you to define callbacks for methods with by-ref parameters.
		/// By-ref parameters can be assigned to.
		/// </summary>
		/// <param name="callback">The callback method to invoke. Must have return type <c>void</c> (C#) or be a <c>Sub</c> (VB.NET).</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value. You can modify
		/// by-ref parameters inside the callback.
		/// <code>
		/// delegate void ExecuteAction(ref Command command);
		///
		/// Command c = ...;
		/// mock.Setup(x => x.Execute(ref c))
		///     .Callback(new ExecuteAction((ref Command command) => Console.WriteLine("Executing command...")));
		/// </code>
		/// </example>
		ICallbackResult Callback(Delegate callback);

		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="action">The callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean 
		/// value that can be used later:
		/// <code>
		/// var called = false;
		/// mock.Setup(x => x.Execute())
		///     .Callback(() => called = true);
		/// </code>
		/// </example>
		ICallbackResult Callback(Action action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T">The argument type of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value. 
		/// <para>
		/// Notice how the specific string argument is retrieved by simply declaring 
		/// it as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Callback((string command) => Console.WriteLine(command));
		/// </code>
		/// </example>
		ICallbackResult Callback<T>(Action<T> action);
	}

	/// <summary>
	/// Defines the <c>Callback</c> verb and overloads for callbacks on
	/// setups that return a value.
	/// </summary>
	/// <typeparam name="TMock">Mocked type.</typeparam>
	/// <typeparam name="TResult">Type of the return value of the setup.</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface ICallback<TMock, TResult> : IFluentInterface
		where TMock : class
	{
		/// <summary>
		///   Specifies a callback to invoke when the method is called that receives the original <see cref="IInvocation"/>.
		///   <para>
		///     This overload is intended to be used in scenarios involving generic type argument matchers
		///     (such as <see cref="It.IsAnyType"/>). The callback will receive the current <see cref="IInvocation"/>,
		///     which allows discovery of both arguments and type arguments.
		///   </para>
		///   <para>
		///     For all other use cases, you should prefer the other <c>Callback</c> overloads as they provide
		///     better static type safety.
		///   </para>
		/// </summary>
		/// <example>
		///     Figure out the generic type argument used for a mocked method call:
		///   <code>
		///     mock.Setup(m => m.DoSomethingWith&lt;It.IsAnyType&gt;(...))
		///         .Callback(new InvocationAction(invocation =>
		///                  {
		///                      var typeArgument = invocation.Method.GetGenericArguments()[0];
		///                      // do something interesting with the type argument
		///                  });
		///     mock.Object.DoSomethingWith&lt;Something&gt;();
		///   </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback(InvocationAction action);

		/// <summary>
		/// Specifies a callback of any delegate type to invoke when the method is called.
		/// This overload specifically allows you to define callbacks for methods with by-ref parameters.
		/// By-ref parameters can be assigned to.
		/// </summary>
		/// <param name="callback">The callback method to invoke. Must have return type <c>void</c> (C#) or be a <c>Sub</c> (VB.NET).</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value. You can modify
		/// by-ref parameters inside the callback.
		/// <code>
		/// delegate void ExecuteAction(ref Command command);
		///
		/// Command c = ...;
		/// mock.Setup(x => x.Execute(ref c))
		///     .Callback(new ExecuteAction((ref Command command) => Console.WriteLine("Executing command...")));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback(Delegate callback);

		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="action">The callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean value that can be used later:
		/// <code>
		/// var called = false;
		/// mock.Setup(x => x.Execute())
		///     .Callback(() => called = true)
		///     .Returns(true);
		/// </code>
		/// Note that in the case of value-returning methods, after the <c>Callback</c>
		/// call you can still specify the return value.
		/// </example>
		IReturnsThrows<TMock, TResult> Callback(Action action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T">The type of the argument of the invoked method.</typeparam>
		/// <param name="action">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value.
		/// <para>
		/// Notice how the specific string argument is retrieved by simply declaring
		/// it as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Callback(command => Console.WriteLine(command))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T>(Action<T> action);
	}
}