// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	partial interface IReturns<TMock, TResult>
	{ 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2) => arg1 + arg2);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3) => arg1 + arg2 + arg3);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4) => arg1 + arg2 + arg3 + arg4);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5) => arg1 + arg2 + arg3 + arg4 + arg5);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
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
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <return>Returns a calculated value which is evaluated lazily at the time of the invocation.</return>
		/// <example>
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15, int arg16) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction);
	}
}
