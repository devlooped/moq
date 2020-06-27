// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
	partial interface IRaise<T>
	{ 
		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1>(Action<T> eventExpression, Func<T1, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2>(Action<T> eventExpression, Func<T1, T2, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3>(Action<T> eventExpression, Func<T1, T2, T3, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4>(Action<T> eventExpression, Func<T1, T2, T3, T4, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func);

		/// <summary>
		/// Specifies the event that will be raised when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">The expression that represents an event attach or detach action.</param>
		/// <param name="func">The function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">The type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">The type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">The type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">The type of the fourth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T5">The type of the fifth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T6">The type of the sixth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T7">The type of the seventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T8">The type of the eighth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T9">The type of the ninth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T10">The type of the tenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T11">The type of the eleventh argument received by the expected invocation.</typeparam>
		/// <typeparam name="T12">The type of the twelfth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T13">The type of the thirteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T14">The type of the fourteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T15">The type of the fifteenth argument received by the expected invocation.</typeparam>
		/// <typeparam name="T16">The type of the sixteenth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func);
	}
}
