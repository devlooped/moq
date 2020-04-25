// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq
{
	internal static class Wrap
	{
		public static Func<object, object> GetResultWrapper(Type resultType)
		{
			if (resultType.IsGenericType)
			{
				var resultTypeDef = resultType.GetGenericTypeDefinition();
				if (resultTypeDef == typeof(ValueTask<>))
				{
					return (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), resultType, AsValueTaskMethod);
				}
				else if (resultTypeDef == typeof(Task<>))
				{
					return (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), resultType, AsTaskMethod);
				}
			}

			return null;
		}

		public static Func<Exception, object> GetExceptionWrapper(Type resultType)
		{
			if (resultType.IsGenericType)
			{
				var resultTypeDef = resultType.GetGenericTypeDefinition();
				if (resultTypeDef == typeof(ValueTask<>))
				{
					return (Func<Exception, object>)Delegate.CreateDelegate(typeof(Func<Exception, object>), resultType, AsFaultedValueTaskMethod);
				}
				else if (resultTypeDef == typeof(Task<>))
				{
					return (Func<Exception, object>)Delegate.CreateDelegate(typeof(Func<Exception, object>), resultType, AsFaultedTaskMethod);
				}
			}

			return null;
		}

		private static readonly MethodInfo AsFaultedTaskMethod =
			typeof(Wrap).GetMethod(nameof(AsFaultedTask), BindingFlags.Public | BindingFlags.Static);

		public static object AsFaultedTask(Type type, Exception exception)
		{
			var resultType = type.GetGenericArguments()[0];

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetException", new Type[] { typeof(Exception) }).Invoke(tcs, new[] { exception });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		private static readonly MethodInfo AsTaskMethod =
			typeof(Wrap).GetMethod(nameof(AsTask), BindingFlags.Public | BindingFlags.Static);

		public static object AsTask(Type type, object result)
		{
			var resultType = type.GetGenericArguments()[0];

			var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
			var tcs = Activator.CreateInstance(tcsType);
			tcsType.GetMethod("SetResult").Invoke(tcs, new[] { result });
			return tcsType.GetProperty("Task").GetValue(tcs, null);
		}

		private static readonly MethodInfo AsFaultedValueTaskMethod =
			typeof(Wrap).GetMethod(nameof(AsFaultedValueTask), BindingFlags.Public | BindingFlags.Static);

		public static object AsFaultedValueTask(Type type, Exception exception)
		{
			var resultType = type.GetGenericArguments()[0];

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var valueTaskCtor = type.GetConstructor(new[] { typeof(Task<>).MakeGenericType(resultType) });
			return valueTaskCtor.Invoke(new object[] { Wrap.AsFaultedTask(type, exception) });
		}

		private static readonly MethodInfo AsValueTaskMethod =
			typeof(Wrap).GetMethod(nameof(AsValueTask), BindingFlags.Public | BindingFlags.Static);

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
