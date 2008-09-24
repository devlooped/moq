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
using System.Linq.Expressions;

namespace Moq.Stub
{
	/// <summary>
	/// Adds <c>Stub</c> extension method to a mock so that you can 
	/// stub properties.
	/// </summary>
	public static class StubExtensions
	{
		/// <summary>
		/// Specifies that the given property should have stub behavior, 
		/// meaning that setting its value will cause it to be saved and 
		/// later returned when the property is requested.
		/// </summary>
		/// <typeparam name="T">Mocked type, inferred from the object 
		/// where this method is being applied (does not need to be specified).</typeparam>
		/// <typeparam name="TProperty">Type of the property, inferred from the property 
		/// expression (does not need to be specified).</typeparam>
		/// <param name="mock">The instance to stub.</param>
		/// <param name="property">Property expression to stub.</param>
		/// <example>
		/// If you have an interface with an int property <c>Value</c>, you might 
		/// stub it using the following straightforward call:
		/// <code>
		/// var mock = new Mock&lt;IHaveValue&gt;();
		/// mock.Stub(v => v.Value);
		/// </code>
		/// After the <c>Stub</c> call has been issued, setting and 
		/// retrieving the object value will behave as expected:
		/// <code>
		/// IHaveValue v = mock.Object;
		/// 
		/// v.Value = 5;
		/// Assert.Equal(5, v.Value);
		/// </code>
		/// </example>
		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property)
			 where T : class
		{
			mock.Stub(property, default(TProperty));
		}

		/// <summary>
		/// Specifies that the given property should have stub behavior, 
		/// meaning that setting its value will cause it to be saved and 
		/// later returned when the property is requested. This overload 
		/// allows setting the initial value for the property.
		/// </summary>
		/// <typeparam name="T">Mocked type, inferred from the object 
		/// where this method is being applied (does not need to be specified).</typeparam>
		/// <typeparam name="TProperty">Type of the property, inferred from the property 
		/// expression (does not need to be specified).</typeparam>
		/// <param name="mock">The instance to stub.</param>
		/// <param name="property">Property expression to stub.</param>
		/// <param name="initialValue">Initial value for the property.</param>
		/// <example>
		/// If you have an interface with an int property <c>Value</c>, you might 
		/// stub it using the following straightforward call:
		/// <code>
		/// var mock = new Mock&lt;IHaveValue&gt;();
		/// mock.Stub(v => v.Value, 5);
		/// </code>
		/// After the <c>Stub</c> call has been issued, setting and 
		/// retrieving the object value will behave as expected:
		/// <code>
		/// IHaveValue v = mock.Object;
		/// // Initial value was stored
		/// Assert.Equal(5, v.Value);
		/// 
		/// // New value set which changes the initial value
		/// v.Value = 6;
		/// Assert.Equal(6, v.Value);
		/// </code>
		/// </example>
		public static void Stub<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> property, TProperty initialValue)
			 where T : class
		{
			TProperty value = initialValue;
			mock.ExpectGet(property).Returns(() => value);
			mock.ExpectSet(property).Callback(p => value = p);
		}
	}
}
