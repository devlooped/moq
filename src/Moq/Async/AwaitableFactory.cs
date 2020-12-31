// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal static class AwaitableFactory
	{
		private static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;

		static AwaitableFactory()
		{
			AwaitableFactory.Providers = new Dictionary<Type, Func<Type, IAwaitableFactory>>
			{
				[typeof(Task)] = awaitableType => TaskFactory.Instance,
				[typeof(ValueTask)] = awaitableType => ValueTaskFactory.Instance,
				[typeof(Task<>)] = awaitableType => AwaitableFactory.Create(typeof(TaskFactory<>), awaitableType),
				[typeof(ValueTask<>)] = awaitableType => AwaitableFactory.Create(typeof(ValueTaskFactory<>), awaitableType),
			};
		}

		private static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
		{
			return (IAwaitableFactory)Activator.CreateInstance(
				awaitableFactoryType.MakeGenericType(
					awaitableType.GetGenericArguments()));
		}

		public static IAwaitableFactory TryGet(Type type)
		{
			Debug.Assert(type != null);

			var key = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;

			if (AwaitableFactory.Providers.TryGetValue(key, out var provider))
			{
				return provider.Invoke(type);
			}

			return null;
		}
	}
}
