//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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
		///     .Returns((string arg1, string arg2) => arg1 + arg2);
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
		///     .Returns((string arg1, string arg2, string arg3) => arg1 + arg2 + arg3);
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4) => arg1 + arg2 + arg3 + arg4);
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5) => arg1 + arg2 + arg3 + arg4 + arg5);
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6);
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7);
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15);
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
		/// <typeparam name="T9">The type of the nineth argument of the invoked method.</typeparam>
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
		///     .Returns((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15, string arg16) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction);
	}
}
