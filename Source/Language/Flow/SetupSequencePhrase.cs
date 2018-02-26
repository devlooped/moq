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
using System.ComponentModel;
using System.Reflection;

using Moq.Properties;

namespace Moq.Language.Flow
{
	// keeping the fluent API separate from `SequenceMethodCall` saves us from having to
	// define a generic variant `SequenceMethodCallReturn<TResult>`, which would be much more
	// work that having a generic fluent API counterpart `SequenceMethodCall<TResult>`.
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class SetupSequencePhrase : ISetupSequentialAction
	{
		private SequenceMethodCall setup;

		public SetupSequencePhrase(SequenceMethodCall setup)
		{
			this.setup = setup;
		}

		public ISetupSequentialAction Pass()
		{
			this.setup.AddPass();
			return this;
		}

		public ISetupSequentialAction Throws<TException>()
			where TException : Exception, new()
			=> this.Throws(new TException());

		public ISetupSequentialAction Throws(Exception exception)
		{
			this.setup.AddThrows(exception);
			return this;
		}
	}

	internal sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
	{
		private SequenceMethodCall setup;

		public SetupSequencePhrase(SequenceMethodCall setup)
		{
			this.setup = setup;
		}

		public ISetupSequentialResult<TResult> CallBase()
		{
			this.setup.AddCallBase();
			return this;
		}

		public ISetupSequentialResult<TResult> Returns(TResult value)
		{
			this.setup.AddReturns(value);
			return this;
		}

		public ISetupSequentialResult<TResult> Returns(Func<TResult> valueFunction)
		{
			Guard.NotNull(valueFunction, nameof(valueFunction));

			// If `valueFunction` is `TResult`, that is someone is setting up the return value of a method
			// that returns a `TResult`, then we have arrived here because C# picked the wrong overload:
			// We don't want to invoke the passed delegate to get a return value; the passed delegate
			// already is the return value.
			if (valueFunction is TResult)
			{
				return this.Returns((TResult)(object)valueFunction);
			}

			this.setup.AddReturns(() => valueFunction());
			return this;
		}

		public ISetupSequentialResult<TResult> Throws(Exception exception)
		{
			this.setup.AddThrows(exception);
			return this;
		}

		public ISetupSequentialResult<TResult> Throws<TException>()
			where TException : Exception, new()
			=> this.Throws(new TException());
	}
}
