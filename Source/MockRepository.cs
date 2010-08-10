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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Utility repository class to use to construct multiple 
	/// mocks when consistent verification is 
	/// desired for all of them.
	/// </summary>
	/// <remarks>
	/// If multiple mocks will be created during a test, passing 
	/// the desired <see cref="MockBehavior"/> (if different than the 
	/// <see cref="MockBehavior.Default"/> or the one 
	/// passed to the repository constructor) and later verifying each
	/// mock can become repetitive and tedious.
	/// <para>
	/// This repository class helps in that scenario by providing a 
	/// simplified creation of multiple mocks with a default 
	/// <see cref="MockBehavior"/> (unless overriden by calling 
	/// <see cref="MockFactory.Create{T}(MockBehavior)"/>) and posterior verification.
	/// </para>
	/// </remarks>
	/// <example group="repository">
	/// The following is a straightforward example on how to 
	/// create and automatically verify strict mocks using a <see cref="MockRepository"/>:
	/// <code>
	/// var repository = new MockRepository(MockBehavior.Strict);
	/// 
	/// var foo = repository.Create&lt;IFoo&gt;();
	/// var bar = repository.Create&lt;IBar&gt;();
	/// 
	///	// no need to call Verifiable() on the setup 
	///	// as we'll be validating all of them anyway.
	/// foo.Setup(f => f.Do());
	/// bar.Setup(b => b.Redo());
	/// 
	///	// exercise the mocks here
	/// 
	/// repository.VerifyAll(); 
	/// // At this point all setups are already checked 
	/// // and an optional MockException might be thrown. 
	/// // Note also that because the mocks are strict, any invocation 
	/// // that doesn't have a matching setup will also throw a MockException.
	/// </code>
	/// The following examples shows how to setup the repository 
	/// to create loose mocks and later verify only verifiable setups:
	/// <code>
	/// var repository = new MockRepository(MockBehavior.Loose);
	/// 
	/// var foo = repository.Create&lt;IFoo&gt;();
	/// var bar = repository.Create&lt;IBar&gt;();
	/// 
	/// // this setup will be verified when we verify the repository
	/// foo.Setup(f => f.Do()).Verifiable();
	/// 	
	/// // this setup will NOT be verified 
	/// foo.Setup(f => f.Calculate());
	/// 	
	/// // this setup will be verified when we verify the repository
	/// bar.Setup(b => b.Redo()).Verifiable();
	/// 
	///	// exercise the mocks here
	///	// note that because the mocks are Loose, members 
	///	// called in the interfaces for which no matching
	///	// setups exist will NOT throw exceptions, 
	///	// and will rather return default values.
	///	
	/// repository.Verify();
	/// // At this point verifiable setups are already checked 
	/// // and an optional MockException might be thrown.
	/// </code>
	/// The following examples shows how to setup the repository with a 
	/// default strict behavior, overriding that default for a 
	/// specific mock:
	/// <code>
	/// var repository = new MockRepository(MockBehavior.Strict);
	/// 
	/// // this particular one we want loose
	/// var foo = repository.Create&lt;IFoo&gt;(MockBehavior.Loose);
	/// var bar = repository.Create&lt;IBar&gt;();
	/// 
	/// // specify setups
	/// 
	///	// exercise the mocks here
	///	
	/// repository.Verify();
	/// </code>
	/// </example>
	/// <seealso cref="MockBehavior"/>
#pragma warning disable 618
	public partial class MockRepository : MockFactory
	{
		/// <summary>
		/// Initializes the repository with the given <paramref name="defaultBehavior"/> 
		/// for newly created mocks from the repository.
		/// </summary>
		/// <param name="defaultBehavior">The behavior to use for mocks created 
		/// using the <see cref="MockFactory.Create{T}()"/> repository method if not overriden 
		/// by using the <see cref="MockFactory.Create{T}(MockBehavior)"/> overload.</param>
		public MockRepository(MockBehavior defaultBehavior)
			: base(defaultBehavior)
		{
		}
#pragma warning restore 618
	}
}