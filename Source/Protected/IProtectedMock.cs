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

using System.Diagnostics.CodeAnalysis;
using Moq.Language.Flow;

namespace Moq.Protected
{
	/// <summary>
	/// Allows setups to be specified for protected members by using their 
	/// name as a string, rather than strong-typing them which is not possible 
	/// due to their visibility.
	/// </summary>
	public interface IProtectedMock<TMock> : IHideObjectMembers
		where TMock : class
	{
		#region Setup

		/// <summary>
		/// Specifies a setup for a void method invocation with the given 
		/// <paramref name="voidMethodName"/>, optionally specifying arguments for the method call.
		/// </summary>
		/// <param name="voidMethodName">The name of the void method to be invoked.</param>
		/// <param name="args">The optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		ISetup<TMock> Setup(string voidMethodName, params object[] args);

		/// <summary>
		/// Specifies a setup for an invocation on a property or a non void method with the given 
		/// <paramref name="methodOrPropertyName"/>, optionally specifying arguments for the method call.
		/// </summary>
		/// <param name="methodOrPropertyName">The name of the method or property to be invoked.</param>
		/// <param name="args">The optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		/// <typeparam name="TResult">The return type of the method or property.</typeparam>
		ISetup<TMock, TResult> Setup<TResult>(string methodOrPropertyName, params object[] args);

		/// <summary>
		/// Specifies a setup for an invocation on a property getter with the given 
		/// <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		ISetupGetter<TMock, TProperty> SetupGet<TProperty>(string propertyName);

		/// <summary>
		/// Specifies a setup for an invocation on a property setter with the given 
		/// <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="value">The property value. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		ISetupSetter<TMock, TProperty> SetupSet<TProperty>(string propertyName, object value);

		#endregion

		#region Verify

		/// <summary>
		/// Specifies a verify for a void method with the given <paramref name="methodName"/>,
		/// optionally specifying arguments for the method call. Use in conjuntion with the default
		/// <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="methodName">The name of the void method to be verified.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="args">The optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		void Verify(string methodName, Times times, params object[] args);

		/// <summary>
		/// Specifies a verify for an invocation on a property or a non void method with the given 
		/// <paramref name="methodName"/>, optionally specifying arguments for the method call.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by 
		/// <paramref name="times"/>.</exception>
		/// <param name="methodName">The name of the method or property to be invoked.</param>
		/// <param name="args">The optional arguments for the invocation. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <typeparam name="TResult">The type of return value from the expression.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void Verify<TResult>(string methodName, Times times, params object[] args);

		/// <summary>
		/// Specifies a verify for an invocation on a property getter with the given 
		/// <paramref name="propertyName"/>.
		/// <exception cref="MockException">The invocation was not call the times specified by 
		/// <paramref name="times"/>.</exception>
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		// TODO should receive args to support indexers
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void VerifyGet<TProperty>(string propertyName, Times times);

		/// <summary>
		/// Specifies a setup for an invocation on a property setter with the given 
		/// <paramref name="propertyName"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by 
		/// <paramref name="times"/>.</exception>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="value">The property value.</param>
		/// <typeparam name="TProperty">The type of the property. If argument matchers are used, 
		/// remember to use <see cref="ItExpr"/> rather than <see cref="It"/>.</typeparam>
		// TODO should receive args to support indexers
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		void VerifySet<TProperty>(string propertyName, Times times, object value);

		#endregion
	}
}