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

using Moq.Language.Flow;

namespace Moq.Protected
{
	/// <summary>
	/// Allows setups to be specified for protected members by using their 
	/// name as a string, rather than strong-typing them which is not possible 
	/// due to their visibility.
	/// </summary>
	public interface IProtectedMock<T> : IHideObjectMembers
	{
		/// <summary>
		/// Specifies a setup for a void method invocation with the given 
		/// <paramref name="voidMethodName"/>, optionally specifying 
		/// arguments for the method call.
		/// </summary>
		/// <param name="voidMethodName">Name of the void method to be invoke.</param>
		/// <param name="args">Optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		ISetup<T> Setup(string voidMethodName, params object[] args);

		/// <summary>
		/// Specifies a setup for an invocation on a property or a non void method with the given 
		/// <paramref name="methodOrPropertyName"/>, optionally specifying 
		/// arguments for the method call.
		/// </summary>
		/// <param name="methodOrPropertyName">Name of the method or property to be invoke.</param>
		/// <param name="args">Optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		/// <typeparam name="TResult">Return type of the method or property.</typeparam>
		ISetup<T, TResult> Setup<TResult>(string methodOrPropertyName, params object[] args);

		/// <summary>
		/// Specifies a setup for an invocation on a property getter with the given 
		/// <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <typeparam name="TProperty">Type of the property.</typeparam>
		ISetupGetter<T, TProperty> SetupGet<TProperty>(string propertyName);

		/// <summary>
		/// Specifies a setup for an invocation on a property setter with the given 
		/// <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <typeparam name="TProperty">Type of the property.</typeparam>
		ISetupSetter<T, TProperty> SetupSet<TProperty>(string propertyName);
	}
}
