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
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language.Flow;
using Moq.Protected;

namespace Moq
{
	internal class SetterMethodCall<TMock, TProperty> : MethodCall<TMock>, ISetupSetter<TMock, TProperty>
		where TMock : class
	{
		public SetterMethodCall(Mock mock, Expression originalExpression, MethodInfo method)
			: base(mock, null, originalExpression, method, new[] { ItExpr.IsAny<TProperty>() })
		{
		}

		public SetterMethodCall(Mock mock, Expression originalExpression, MethodInfo method, TProperty value)
			: base(mock, null, originalExpression, method, new[] { ItExpr.Is<TProperty>(arg => Object.Equals(arg, value)) })
		{
		}

		public SetterMethodCall(Mock mock, Func<bool> condition, Expression originalExpression, MethodInfo method, Expression value)
			: base(mock, condition, originalExpression, method, new[] { value })
		{
		}

		public ICallbackResult Callback(Action<TProperty> callback)
		{
			this.SetCallbackWithArguments(callback);
			return this;
		}
	}
}