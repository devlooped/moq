// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq
{
	internal static class Wrap
	{
		public static object AsFaultedTask(Type type, Exception exception)
		{
			var resultType = type.GetGenericArguments()[0];

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetException", new Type[] { typeof(Exception) }).Invoke(tcs, new[] { exception });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		public static object AsTask(Type type, object result)
		{
			var resultType = type.GetGenericArguments()[0];

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetResult").Invoke(tcs, new[] { result });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		public static object AsFaultedValueTask(Type type, Exception exception)
		{
			var resultType = type.GetGenericArguments()[0];

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var valueTaskCtor = type.GetConstructor(new[] { typeof(Task<>).MakeGenericType(resultType) });
			return valueTaskCtor.Invoke(new object[] { Wrap.AsFaultedTask(type, exception) });
		}

		public static object AsValueTask(Type type, object result)
		{
			var resultType = type.GetGenericArguments()[0];

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var valueTaskCtor = type.GetConstructor(new[] { resultType });
			return valueTaskCtor.Invoke(new object[] { result });
		}
	}
}
