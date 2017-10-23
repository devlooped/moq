//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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
using System.Linq.Expressions;

using Moq.Language;
using Moq.Language.Flow;

namespace Moq.Protected
{
	/// <summary>
	/// Allows setups to be specified for protected members (methods and properties)
	/// seen through another type with corresponding members (that is, members
	/// having identical signatures as the ones to be set up).
	/// </summary>
	/// <typeparam name="T">Type of the mocked object.</typeparam>
	/// <typeparam name="TDuck">
	/// Any type with members whose signatures are identical to the mock's protected members (except for their accessibility level).
	/// </typeparam>
	public interface IProtectedAsMock<T, TDuck> : IFluentInterface
		where T : class
		where TDuck : class
    {
		/// <summary>
		/// Specifies a setup on the mocked type for a call to a <see langword="void"/> method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <seealso cref="Mock{T}.Setup(Expression{Action{T}})"/>
		ISetup<T> Setup(Expression<Action<TDuck>> expression);

		/// <summary>
		/// Specifies a setup on the mocked type for a call to a value-returning method.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <seealso cref="Mock{T}.Setup{TResult}(Expression{Func{T, TResult}})"/>
		ISetup<T, TResult> Setup<TResult>(Expression<Func<TDuck, TResult>> expression);

		/// <summary>
		/// Specifies a setup on the mocked type for a call to a property getter.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the property getter.</param>
		ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<TDuck, TProperty>> expression);

		/// <summary>
		/// Specifies that the given property should have "property behavior",
		/// meaning that setting its value will cause it to be saved and later returned when the property is requested.
		/// (This is also known as "stubbing".)
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the property.</param>
		/// <param name="initialValue">Initial value for the property.</param>
		Mock<T> SetupProperty<TProperty>(Expression<Func<TDuck, TProperty>> expression, TProperty initialValue = default(TProperty));

		/// <summary>
		/// Return a sequence of values, once per call.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		ISetupSequentialResult<TResult> SetupSequence<TResult>(Expression<Func<TDuck, TResult>> expression);

		/// <summary>
		/// Performs a sequence of actions, one per call.
		/// </summary>
		/// <param name="expression">Lambda expression that specifeid the expected method invocation.</param>
		ISetupSequentialAction SetupSequence(Expression<Action<TDuck>> expression);
	}
}
