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
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for invocations that do not have expectations or return values, with loose mocks.
	/// This is the default behavior for a mock.
	/// </summary>
	internal class EmptyDefaultValueProvider : IDefaultValueProvider
	{
		public virtual object ProvideDefault(MethodInfo member, object[] arguments)
		{
			var valueType = member.ReturnType;

			// Return default value.
			if (valueType.IsValueType)
			{
				if (valueType.IsAssignableFrom(typeof(int)))
					return 0;
				else
					return Activator.CreateInstance(valueType);
			}
			else
			{
				if (valueType.IsArray)
				{
					return Activator.CreateInstance(valueType, 0);
				}
				else if (valueType == typeof(System.Collections.IEnumerable))
				{
					return new object[0];
				}
				else if (valueType == typeof(IQueryable))
				{
					return new object[0].AsQueryable();
				}
				else if (valueType.IsGenericType &&
					valueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					var genericListType = typeof(List<>).MakeGenericType(
						valueType.GetGenericArguments()[0]);
					return Activator.CreateInstance(genericListType);
				}
				else if (valueType.IsGenericType &&
					valueType.GetGenericTypeDefinition() == typeof(IQueryable<>))
				{
					var genericType = valueType.GetGenericArguments()[0];
					var genericListType = typeof(List<>).MakeGenericType(genericType);
					var list = Activator.CreateInstance(genericListType);

					return typeof(Queryable).GetMethods()
						.Single(x => x.Name == "AsQueryable" && x.IsGenericMethod)
						.MakeGenericMethod(genericType)
						.Invoke(null, new[] { list });
				}
				else
				{
					return null;
				}
			}
		}
	}
}
