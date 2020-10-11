// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	partial interface ICallback
	{ 
		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2) => Console.WriteLine(arg1 + arg2));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2>(Action<T1, T2> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3) => Console.WriteLine(arg1 + arg2 + arg3));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4) => Console.WriteLine(arg1 + arg2 + arg3 + arg4));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T16">The type of the sixteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="ICallbackResult"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15, string arg16) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16));
		/// </code>
		/// </example>
		ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action);
	}

	partial interface ICallback<TMock, TResult>
		where TMock : class
	{

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2) => Console.WriteLine(arg1 + arg2));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2>(Action<T1, T2> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3) => Console.WriteLine(arg1 + arg2 + arg3));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4) => Console.WriteLine(arg1 + arg2 + arg3 + arg4));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action);

		/// <summary>
		/// Specifies a callback to invoke when the method is called that receives the original
		/// arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument of the invoked method.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument of the invoked method.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument of the invoked method.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument of the invoked method.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument of the invoked method.</typeparam>
		/// <typeparam name="T16">The type of the sixteenth argument of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		/// <returns>A reference to <see cref="IReturnsThrows{TMock, TResult}"/> interface.</returns>
		/// <example>
		/// Invokes the given callback with the concrete invocation arguments values. 
		/// <para>
		/// Notice how the specific arguments are retrieved by simply declaring 
		/// them as part of the lambda expression for the callback:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;(),
		///                      It.IsAny&lt;string&gt;()))
		///     .Callback((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16) => Console.WriteLine(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16));
		/// </code>
		/// </example>
		IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action);
	}
}
