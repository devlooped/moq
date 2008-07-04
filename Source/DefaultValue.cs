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

//    * Neither the name of the <ORGANIZATION> nor the 
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

namespace Moq
{
	/// <summary>
	/// Represents the default value returned from invocations that 
	/// do not have expectations or return values, with loose mocks.
	/// </summary>
	internal class DefaultValue
	{
		public DefaultValue(Type valueType)
		{
			// Return default value.
			if (valueType.IsValueType)
			{
				if (valueType.IsAssignableFrom(typeof(int)))
					this.Value = 0;
				else
					this.Value = Activator.CreateInstance(valueType);
			}
			else
			{
				if (valueType.IsArray)
				{
					this.Value = Activator.CreateInstance(valueType, 0);
				}
				else if (valueType == typeof(System.Collections.IEnumerable))
				{
					this.Value = new object[0];
				}
				else if (valueType.IsGenericType &&
					valueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					var genericListType = typeof(List<>).MakeGenericType(
						valueType.GetGenericArguments()[0]);
					this.Value = Activator.CreateInstance(genericListType);
				}
				else
				{
					this.Value = null;
				}
			}
		}

		public object Value { get; private set; }
	}
}
