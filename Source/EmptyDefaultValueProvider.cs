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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for invocations that do not have setups or return values, with loose mocks.
	/// This is the default behavior for a mock.
	/// </summary>
	internal sealed class EmptyDefaultValueProvider : IDefaultValueProvider
	{
		private static Dictionary<Type, Func<Type, object>> factories = new Dictionary<Type, Func<Type, object>>()
		{
			[typeof(Array)] = CreateArray,
			[typeof(IEnumerable)] = CreateEnumerable,
			[typeof(IEnumerable<>)] = CreateEnumerableOf,
			[typeof(IQueryable)] = CreateQueryable,
			[typeof(IQueryable<>)] = CreateQueryableOf,
			[typeof(Task)] = CreateTask,
			[typeof(Task<>)] = CreateTaskOf,
			[typeof(ValueTask<>)] = CreateValueTaskOf,
		};

		DefaultValue IDefaultValueProvider.Kind => DefaultValue.Empty;

		public static EmptyDefaultValueProvider Instance { get; } = new EmptyDefaultValueProvider();

		private EmptyDefaultValueProvider()
		{
		}

		public object ProvideDefault(Type type, Mock mock)
		{
			return GetDefaultValue(type);
		}

		private static object GetDefaultValue(Type type)
		{
			return type.GetTypeInfo().IsValueType ? GetValueTypeDefault(type) : GetReferenceTypeDefault(type);
		}

		private static object GetReferenceTypeDefault(Type type)
		{
			Type factoryKey;

			if (type.IsArray)
			{
				factoryKey = typeof(Array);
			}
			else if (type.GetTypeInfo().IsGenericType)
			{
				factoryKey = type.GetGenericTypeDefinition();
			}
			else
			{
				factoryKey = type;
			}

			return factories.TryGetValue(factoryKey, out Func<Type, object> factory) ? factory.Invoke(type) : null;
		}

		private static object CreateArray(Type type)
		{
			var elementType = type.GetElementType();
			var lengths = new int[type.GetArrayRank()];
			return Array.CreateInstance(elementType, lengths);
		}

		private static object CreateEnumerable(Type type)
		{
			return new object[0];
		}

		private static object CreateEnumerableOf(Type type)
		{
			var elementType = type.GetGenericArguments()[0];
			return Array.CreateInstance(elementType, 0);
		}

		private static object CreateQueryable(Type type)
		{
			return new object[0].AsQueryable();
		}

		private static object CreateQueryableOf(Type type)
		{
			var elementType = type.GetGenericArguments()[0];
			var array = Array.CreateInstance(elementType, 0);

			return typeof(Queryable).GetMethods("AsQueryable")
				.Single(x => x.IsGenericMethod)
				.MakeGenericMethod(elementType)
				.Invoke(null, new[] { array });
		}

		private static object CreateTask(Type type)
		{
			return Task.FromResult(false);
		}

		private static object CreateTaskOf(Type type)
		{
			var resultType = type.GetGenericArguments()[0];
			var result = GetDefaultValue(resultType);

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetResult").Invoke(tcs, new[] { result });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		private static object CreateValueTaskOf(Type type)
		{
			var resultType = type.GetGenericArguments()[0];
			var result = GetDefaultValue(resultType);

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var valueTaskCtor = type.GetConstructor(new[] { resultType });
			return valueTaskCtor.Invoke(new object[] { result });
		}

		private static object GetValueTypeDefault(Type type)
		{
			Type factoryKey;

			if (type.GetTypeInfo().IsGenericType)
			{
				factoryKey = type.GetGenericTypeDefinition();
			}
			else
			{
				factoryKey = type;
			}

			return factories.TryGetValue(factoryKey, out Func<Type, object> factory) ? factory.Invoke(type) : Activator.CreateInstance(type);
		}
	}
}
