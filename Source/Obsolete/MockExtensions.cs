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
using System.Linq.Expressions;
using Moq.Language.Flow;

namespace Moq
{
	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	/// <devdoc>
	/// Provided as extension methods as they confuse the compiler 
	/// with the overloads taking Action.
	/// </devdoc>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockExtensions
	{
		/// <summary>
		/// Specifies a setup on the mocked type for a call to 
		/// to a property setter, regardless of its value.
		/// </summary>
		/// <remarks>
		/// If more than one setup is set for the same property setter, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="mock">The target mock for the setup.</param>
		/// <param name="expression">Lambda expression that specifies the property setter.</param>
		/// <example group="setups">
		/// <code>
		/// mock.SetupSet(x =&gt; x.Suspended);
		/// </code>
		/// </example>
		/// <devdoc>
		/// This method is not legacy, but must be on an extension method to avoid 
		/// confusing the compiler with the new Action syntax.
		/// </devdoc>
		[Obsolete("Replaced by SetupSet(Action)")]
		public static ISetupSetter<T, TProperty> SetupSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			return Mock.SetupSet<T, TProperty>(mock, expression);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regarless of its value.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by VerifySet(Action)")]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
			where T : class
		{
			Mock.VerifySet(mock, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, specifying a failure  
		/// error message. 
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, string)")]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, string failMessage)
			where T : class
		{
			Mock.VerifySet(mock, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regardless 
		/// of the value but only the specified number of times.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, Times)")]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, Times times)
			where T : class
		{
			Mock.VerifySet(mock, expression, times, null);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock, regardless 
		/// of the value but only the specified number of times, and specifying a failure  
		/// error message. 
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="mock">The mock instance.</param>
		/// <typeparam name="T">Mocked type.</typeparam>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		[Obsolete("Replaced by  VerifySet(Action, Times, string)")]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, Times times, string failMessage)
			where T : class
		{
			Mock.VerifySet(mock, expression, times, failMessage);
		}
	}
}