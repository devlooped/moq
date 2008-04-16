using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb and overloads.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallback : IHideObjectMembers
	{
		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean 
		/// value that can be used later:
		/// <code>
		/// bool called = false;
		/// mock.Expect(x => x.Execute())
		///     .Callback(() => called = true);
		/// </code>
		/// </example>
		IThrowsOnceVerifiesRaise Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Argument type of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value. 
		/// <para>
		/// Notice how the specific string argument is retrieved by simply declaring 
		/// it as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Callback((string command) => Console.WriteLine(command));
		/// </code>
		/// </example>
		IThrowsOnceVerifiesRaise Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2) => Console.WriteLine(arg1 + arg2));
		/// </code>
		/// </example>
		IThrowsOnceVerifiesRaise Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Callback((string arg1, string arg2, int arg3) => Console.WriteLine(arg1 + arg2 + arg3));
		/// </code>
		/// </example>
		IThrowsOnceVerifiesRaise Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;(),
		///                      It.IsAny&lt;bool&gt;()))
		///     .Callback((string arg1, string arg2, int arg3, bool arg4) => Console.WriteLine(arg1 + arg2 + arg3 + arg4));
		/// </code>
		/// </example>
		IThrowsOnceVerifiesRaise Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
	}

	/// <summary>
	/// Defines the <c>Callback</c> verb and overloads for callbacks on 
	/// expectations that return a value.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallback<TResult> : IHideObjectMembers
	{
		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// The following example specifies a callback to set a boolean 
		/// value that can be used later:
		/// <code>
		/// bool called = false;
		/// mock.Expect(x => x.Execute())
		///     .Callback(() => called = true)
		///     .Returns(true);
		/// </code>
		/// Note that in the case of value-returning methods, after the <c>Callback</c> 
		/// call you can still specify the return value.
		/// </example>
		IReturnsThrows<TResult> Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation argument value. 
		/// <para>
		/// Notice how the specific string argument is retrieved by simply declaring 
		/// it as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Callback((string command) => Console.WriteLine(command))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrows<TResult> Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2) => Console.WriteLine(arg1 + arg2))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrows<TResult> Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Callback((string arg1, string arg2, int arg3) => Console.WriteLine(arg1 + arg2 + arg3))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrows<TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Expect(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;(),
		///                      It.IsAny&lt;bool&gt;()))
		///     .Callback((string arg1, string arg2, int arg3, bool arg4) => Console.WriteLine(arg1 + arg2 + arg3 + arg4))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsThrows<TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
	}
}
