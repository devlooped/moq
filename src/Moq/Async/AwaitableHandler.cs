// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Moq.Async
{
	/// <summary>
	///   Provides methods for registering custom awaitable type handlers (<see cref="IAwaitableHandler"/>).
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class AwaitableHandler
	{
		private static readonly Dictionary<Type, Func<Type, IAwaitableHandler>> factories;

		static AwaitableHandler()
		{
			AwaitableHandler.factories = new Dictionary<Type, Func<Type, IAwaitableHandler>>()
			{
				[typeof(Task)] = type => TaskHandler.Instance,
				[typeof(Task<>)] = type => new TaskOfHandler(type.GetGenericArguments()[0]),
				[typeof(ValueTask)] = type => ValueTaskHandler.Instance,
				[typeof(ValueTask<>)] = type => new ValueTaskOfHandler(type, type.GetGenericArguments()[0]),
			};
		}

		/// <summary>
		/// Registers an <see cref="IAwaitableHandler"/> factory function for the given awaitable type.
		/// This allows Moq to properly recognize the awaitable type.
		/// </summary>
		/// <remarks>
		/// As an example, given an <see langword="await"/>-able type <c>TaskLike&lt;TResult&gt;</c>
		/// and a corresponding <see cref="IAwaitableHandler"/> implementation <c>TaskLikeHandler</c>,
		/// call this method as follows:
		/// <code>
		///   AwaitableHandler.Register(typeof(TaskLike&lt;&gt;), type => new TaskLikeHandler(resultType: type.GetGenericArguments().Single()));
		/// </code>
		/// </remarks>
		/// <param name="typeDefinition">
		///   The awaitable type for which to register the given <paramref name="factory"/> function.
		///   You can specify an open generic type definition, e. g. <c>typeof(TaskLike&lt;&gt;)</c>,
		///   to cover all concrete type instantiations.
		/// </param>
		/// <param name="factory">
		///   The factory function that, given the <see cref="Type"/> of a concrete awaitable type,
		///   will produce a suitable <see cref="IAwaitableHandler"/> for it.</param>
		public static void Register(Type typeDefinition, Func<Type, IAwaitableHandler> factory)
		{
			AwaitableHandler.factories[typeDefinition] = factory;
		}

		internal static IAwaitableHandler TryGet(Type type)
		{
			var typeDefinition = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;
			return AwaitableHandler.factories.TryGetValue(typeDefinition, out var factory) ? factory.Invoke(type) : null;
		}
	}
}
