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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Allows to create parameter captures in setup expressions.
	/// </summary>
	public static class Capture
	{
		/// <summary>
		/// Creates a parameter capture that will store values in a collection.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <param name="collection">The collection that will store captured parameter values</param>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var parameters = new List{string}();
		/// mock.Setup(x => x.DoSomething(Capture.In(parameters)));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", parameters.Single());
		/// </code>
		/// </example>
		public static T In<T>(ICollection<T> collection)
		{
			var match = new CaptureMatch<T>(collection.Add);
			return With(match);
		}

		/// <summary>
		/// Creates a parameter capture that will store specific values in a collection.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <param name="collection">The collection that will store captured parameter values</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var parameters = new List{string}();
		/// mock.Setup(x => x.DoSomething(Capture.In(parameters, p => p.StartsWith("W"))));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", parameters.Single());
		/// </code>
		/// </example>
		public static T In<T>(IList<T> collection, Expression<Func<T, bool>> predicate)
		{
			var match = new CaptureMatch<T>(collection.Add, predicate);
			return With(match);
		}

		/// <summary>
		/// Creates a parameter capture using specified <see cref="CaptureMatch{T}"/>.
		/// </summary>
		/// <typeparam name="T">The captured object type</typeparam>
		/// <example>
		/// Arrange code:
		/// <code>
		/// var capturedValue = string.Empty;
		/// var match = new CaptureMatch{string}(x => capturedValue = x);
		/// mock.Setup(x => x.DoSomething(Capture.With(match)));
		/// </code>
		/// Assert code:
		/// <code>
		/// Assert.Equal("Hello!", capturedValue);
		/// </code>
		/// </example>
		public static T With<T>(CaptureMatch<T> match)
		{
			return Match.Create(match);
		}
	}
}
