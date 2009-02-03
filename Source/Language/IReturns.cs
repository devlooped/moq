//Copyright (c) 2007, Moq Team 
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

//    * Neither the name of the Moq Team nor the 
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
	/// <summary>
	/// Defines the <c>Returns</c> verb.
	/// </summary>
	/// <typeparam name="TResult">Type of the return value from the expression.</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturns<TMock, TResult> : IHideObjectMembers
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the method call:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns(TResult value);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value when the method is called:
		/// <code>
		/// mock.Setup(x => x.Execute("ping"))
		///     .Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the method 
		/// is executed and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IReturnsResult<TMock> Returns(Func<TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T">Type of the argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The lookup list can change between invocations and the setup 
		/// will return different values accordingly. Also, notice how the specific 
		/// string argument is retrieved by simply declaring it as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(It.IsAny&lt;string&gt;()))
		///     .Returns((string command) => returnValues[command]);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T>(Func<T, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                     It.IsAny&lt;string&gt;(), 
		///                     It.IsAny&lt;string&gt;()))
		///     .Returns((string arg1, string arg2) => arg1 + arg2);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;string&gt;(), 
		///                      It.IsAny&lt;int&gt;()))
		///     .Returns((string arg1, string arg2, int arg3) => arg1 + arg2 + arg3);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction);

		/// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		/// <typeparam name="T1">Type of the first argument of the invoked method.</typeparam>
		/// <typeparam name="T2">Type of the second argument of the invoked method.</typeparam>
		/// <typeparam name="T3">Type of the third argument of the invoked method.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument of the invoked method.</typeparam>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example group="returns">
		/// Return a calculated value which is evaluated lazily at the time of the invocation.
		/// <para>
		/// The return value is calculated from the value of the actual method invocation arguments. 
		/// Notice how the arguments are retrieved by simply declaring them as part of the lambda 
		/// expression:
		/// </para>
		/// <code>
		/// mock.Setup(x => x.Execute(
		///                       It.IsAny&lt;string&gt;(), 
		///                       It.IsAny&lt;string&gt;(), 
		///                       It.IsAny&lt;int&gt;(), 
		///                       It.IsAny&lt;bool&gt;()))
		///     .Returns((string arg1, string arg2, int arg3, bool arg4) => arg1 + arg2 + arg3 + arg4);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction);
	}
}
