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

namespace Moq
{
	internal sealed class SetupSequentialActionContext<TMock> : ISetupSequentialAction
		where TMock : class
	{
		private int currentStep;
		private int expectationsCount;
		private Mock<TMock> mock;
		private Expression<Action<TMock>> expression;
		private readonly Action callbackAction;

		public SetupSequentialActionContext(
			Mock<TMock> mock,
			Expression<Action<TMock>> expression)
		{
			this.mock = mock;
			this.expression = expression;
			this.callbackAction = () => currentStep++;
		}

		public ISetupSequentialAction Pass()
		{
			var setup = this.GetSetup();
			setup.Callback(DoNothing);
			this.EndSetup(setup);
			return this;
		}

		private static void DoNothing() { }

		public ISetupSequentialAction Throws<TException>() where TException : Exception, new()
		{
			var setup = this.GetSetup();
			setup.Throws<TException>();
			this.EndSetup(setup);
			return this;
		}

		public ISetupSequentialAction Throws(Exception exception)
		{
			var setup = this.GetSetup();
			setup.Throws(exception);
			this.EndSetup(setup);
			return this;
		}

		private void EndSetup(ICallback callback)
		{
			callback.Callback(callbackAction);
		}

		private ISetup<TMock> GetSetup()
		{
			var expectationStep = this.expectationsCount;
			this.expectationsCount++;

			return this.mock
				.When(() => currentStep == expectationStep)
				.Setup(expression);
		}
	}
}
