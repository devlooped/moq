// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal static class AwaitableHandler
	{
		private static readonly Dictionary<Type, Func<Type, IAwaitableHandler>> factories;

		static AwaitableHandler()
		{
			AwaitableHandler.factories = new Dictionary<Type, Func<Type, IAwaitableHandler>>()
			{
				[typeof(Task<>)] = type => new TaskOfHandler(type.GetGenericArguments()[0]),
				[typeof(ValueTask<>)] = type => new ValueTaskOfHandler(type, type.GetGenericArguments()[0]),
			};
		}

		public static IAwaitableHandler TryGet(Type type)
		{
			var typeDefinition = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;
			return AwaitableHandler.factories.TryGetValue(typeDefinition, out var factory) ? factory.Invoke(type) : null;
		}
	}
}
