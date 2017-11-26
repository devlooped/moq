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
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	/// A <see cref="DefaultValueProvider"/> that returns an empty default value 
	/// for non-mockeable types, and mocks for all other types (interfaces and 
	/// non-sealed classes) that can be mocked.
	/// </summary>
	internal sealed class MockDefaultValueProvider : DefaultValueProvider
	{
		public static MockDefaultValueProvider Instance { get; } = new MockDefaultValueProvider();

		private Dictionary<Type, Func<Type, Mock, object>> factories;

		private MockDefaultValueProvider()
		{
			this.factories = new Dictionary<Type, Func<Type, Mock, object>>()
			{
				[typeof(Task<>)] = CreateTaskOf,
				[typeof(ValueTask<>)] = CreateValueTaskOf,
			};
		}

		internal override DefaultValue Kind => DefaultValue.Mock;

		protected internal override object GetDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			Type factoryKey = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition()
			                : type;

			if (factories.TryGetValue(factoryKey, out Func<Type, Mock, object> factory))
			{
				return factory.Invoke(type, mock);
			}

			var emptyValue = EmptyDefaultValueProvider.Instance.GetDefaultValue(type, mock);
			if (emptyValue != null)
			{
				return emptyValue;
			}
			else if (type.IsMockeable())
			{
				// Create a new mock to be placed to InnerMocks dictionary if it's missing there
				var mockType = typeof(Mock<>).MakeGenericType(type);
				Mock newMock = (Mock)Activator.CreateInstance(mockType, mock.Behavior);
				newMock.DefaultValueProvider = mock.DefaultValueProvider;
				newMock.CallBase = mock.CallBase;
				newMock.Switches = mock.Switches;
				return newMock.Object;
			}
			else
			{
				return null;
			}
		}

		private object CreateTaskOf(Type type, Mock mock)
		{
			var resultType = type.GetGenericArguments()[0];
			var result = this.GetDefaultValue(resultType, mock);

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetResult").Invoke(tcs, new[] { result });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		private object CreateValueTaskOf(Type type, Mock mock)
		{
			var resultType = type.GetGenericArguments()[0];
			var result = this.GetDefaultValue(resultType, mock);

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var valueTaskCtor = type.GetConstructor(new[] { resultType });
			return valueTaskCtor.Invoke(new object[] { result });
		}
	}
}
