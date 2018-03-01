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
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;
using Moq.Linq;

namespace Moq
{
	public partial class Mock
	{
		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		public static T Of<T>() where T : class
		{
			// This method was originally implemented as follows:
			//
			// return Mocks.CreateMockQuery<T>().First<T>();
			//
			// which involved a lot of avoidable `IQueryable` query provider overhead and lambda compilation.
			// What it really boils down to is this (much faster) code:
			var mock = new Mock<T>();
			mock.SetupAllProperties();
			return mock.Object;
		}

		/// <summary>
		/// Creates an mock object of the indicated type.
		/// </summary>
		/// <param name="predicate">The predicate with the specification of how the mocked object should behave.</param>
		/// <typeparam name="T">The type of the mocked object.</typeparam>
		/// <returns>The mocked object created.</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public static T Of<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			var mocked = Mocks.CreateMockQuery<T>().First<T>(predicate);

			// The current implementation of LINQ to Mocks creates mocks that already have recorded invocations.
			// Because this interferes with `VerifyNoOtherCalls`, we recursively clear all invocations before
			// anyone ever gets to see the new created mock.
			//
			// TODO: Make LINQ to Mocks set up mocks without causing invocations of its own, then remove this hack.
			var mock = Mock.Get(mocked);
			mock.ResetCalls();
			foreach (var inner in mock.InnerMocks.Values)
			{
				inner.Mock.ResetCalls();
			}

			return mocked;
		}
	}
}
