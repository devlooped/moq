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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
		/// <typeparam name="T9">The type of the nineth argument received by the expected invocation.</typeparam>
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
