// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using Moq.Language.Flow;

namespace Moq.Language
{
	partial interface IThrows
	{ 
		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Throws((string arg1, string arg2) => new Exception(arg1 + arg2));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, TException>(Func<T1, T2, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Throws((string arg1, string arg2, string arg3) => new Exception(arg1 + arg2 + arg3));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, TException>(Func<T1, T2, T3, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Throws((string arg1, string arg2, string arg3, string arg4) => new Exception(arg1 + arg2 + arg3 + arg4));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, TException>(Func<T1, T2, T3, T4, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, TException>(Func<T1, T2, T3, T4, T5, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;()))
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, TException>(Func<T1, T2, T3, T4, T5, T6, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">The type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">The type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument of the invoked method.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument of the invoked method.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument of the invoked method.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument of the invoked method.</typeparam>
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, TException>(Func<T1, T2, T3, T4, T5, T6, T7, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TException> exceptionFunction) where TException : Exception;

		/// <summary>
		/// Specifies a function that will calculate the exception to throw when the method is invoked, 
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
		/// <typeparam name="TException">Type of exception that will be calculated and thrown when the setup is matched.</typeparam>
		/// <param name="exceptionFunction">The function that will calculate the exception to be thrown.</param>
		/// <example>
		/// <para>
		/// The thrown exception is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
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
		///     .Throws((string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, string arg12, string arg13, string arg14, string arg15, string arg16) => new Exception(arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16));
		/// </code>
		/// </example>
		IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TException> exceptionFunction) where TException : Exception;
	}
}
