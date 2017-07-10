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
using Moq.Proxy;

namespace Moq
{
	/// <summary>
	/// Tracks the current mock and interception context.
	/// </summary>
	internal class FluentMockContext : IDisposable
	{
		[ThreadStatic]
		private static FluentMockContext current;

		private List<MockInvocation> invocations = new List<MockInvocation>();

		public static FluentMockContext Current
		{
			get { return current; }
		}

		/// <summary>
		/// Having an active fluent mock context means that the invocation 
		/// is being performed in "trial" mode, just to gather the 
		/// target method and arguments that need to be matched later 
		/// when the actual invocation is made.
		/// </summary>
		public static bool IsActive
		{
			get { return current != null; }
		}

		public FluentMockContext()
		{
			current = this;
		}

		public void Add(Mock mock, ICallContext invocation)
		{
			invocations.Add(new MockInvocation(mock, invocation, LastMatch));
		}

		public MockInvocation LastInvocation
		{
			get { return invocations.LastOrDefault(); }
		}

		public Match LastMatch { get; set; }

		public void Dispose()
		{
			invocations.Reverse();
			foreach (var invocation in invocations)
			{
				invocation.Dispose();
			}

			current = null;
		}

		internal class MockInvocation : IDisposable
		{
			private DefaultValue defaultValue;

			public MockInvocation(Mock mock, ICallContext invocation, Match matcher)
			{
				this.Mock = mock;
				this.Invocation = invocation;
				this.Match = matcher;
				defaultValue = mock.DefaultValue;
				// Temporarily set mock default value to Mock so that recursion works.
				mock.DefaultValue = DefaultValue.Mock;
			}

			public Mock Mock { get; private set; }

			public ICallContext Invocation { get; private set; }

			public Match Match { get; private set; }

			public void Dispose()
			{
				Mock.DefaultValue = defaultValue;
			}
		}
	}
}
