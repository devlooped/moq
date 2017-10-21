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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Proxy;

namespace Moq
{
	// this corresponds to a setup created by `mock.SetupSequence`
	internal sealed class SequenceMethodCall : MethodCall
	{
		// contains the responses set up with the `CallBase`, `Pass`, `Returns`, and `Throws` verbs
		private ConcurrentQueue<Tuple<ResponseKind, object>> responses;

		public SequenceMethodCall(Mock mock, Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(mock, null, originalExpression, method, arguments)
		{
			this.responses = new ConcurrentQueue<Tuple<ResponseKind, object>>();
		}

		public void AddCallBase()
		{
			this.responses.Enqueue(Tuple.Create(ResponseKind.CallBase, (object)null));
		}

		public void AddPass()
		{
			this.responses.Enqueue(Tuple.Create(ResponseKind.Pass, (object)null));
		}

		public void AddReturns(object value)
		{
			this.responses.Enqueue(Tuple.Create(ResponseKind.Returns, value));
		}

		public void AddThrows(Exception exception)
		{
			this.responses.Enqueue(Tuple.Create(ResponseKind.Throws, (object)exception));
		}

		public override void Execute(ICallContext call)
		{
			base.Execute(call);

			if (this.responses.TryDequeue(out Tuple<ResponseKind, object> response))
			{
				switch (response.Item1)
				{
					case ResponseKind.Pass:
						break;

					case ResponseKind.CallBase:
						call.InvokeBase();
						break;

					case ResponseKind.Returns:
						call.ReturnValue = response.Item2;
						break;

					case ResponseKind.Throws:
						throw (Exception)response.Item2;
				}
			}
			else
			{
				// we get here if there are more invocations than configured responses.
				// if the setup method does not have a return value, we don't need to do anything;
				// if it does have a return value, we produce the default value.

				var returnType = call.Method.ReturnType;
				if (returnType == typeof(void))
				{
				}
				else
				{
					call.ReturnValue = returnType.GetTypeInfo().IsValueType ? Activator.CreateInstance(returnType)
					                                                        : null;
				}
			}
		}

		private enum ResponseKind
		{
			Pass,
			CallBase,
			Returns,
			Throws,
		}
	}
}
