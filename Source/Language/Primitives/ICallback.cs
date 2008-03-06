using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;

namespace Moq.Language.Primitives
{
	/// <summary>
	/// Defines the <c>Callback</c> verb.
	/// </summary>
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
		/// mock.Expect(x => x.Execute()).Callback(() => called = true);
		/// </code>
		/// </example>
		IThrowsOnceVerifies Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Argument type of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IThrowsOnceVerifies Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IThrowsOnceVerifies Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IThrowsOnceVerifies Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IThrowsOnceVerifies Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
	}

	/// <summary>
	/// Defines the <c>Callback</c> verb.
	/// </summary>
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
		/// mock.Expect(x => x.Execute()).Callback(() => called = true).Returns(true);
		/// </code>
		/// Note that in the case of value-returning methods, after the <c>Callback</c> 
		/// call you still can specify the return value.
		/// </example>
		IReturnsThrows<TResult> Callback(Action callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IReturnsThrows<TResult> Callback<T>(Action<T> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
		IReturnsThrows<TResult> Callback<T1, T2>(Action<T1, T2> callback);
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="callback">Callback method to invoke.</param>
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
		IReturnsThrows<TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
	}
}
