// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Moq.Async
{
	/// <summary>
	///   Converts return values and exceptions to and from instances of a particular awaitable type.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class AwaitableHandler
	{
		private static readonly Dictionary<Type, Func<Type, AwaitableHandler>> factories;

		static AwaitableHandler()
		{
			AwaitableHandler.factories = new Dictionary<Type, Func<Type, AwaitableHandler>>()
			{
				[typeof(Task)] = type => TaskHandler.Instance,
				[typeof(Task<>)] = type => new TaskOfHandler(type.GetGenericArguments()[0]),
				[typeof(ValueTask)] = type => ValueTaskHandler.Instance,
				[typeof(ValueTask<>)] = type => new ValueTaskOfHandler(type, type.GetGenericArguments()[0]),
			};
		}

		/// <summary>
		/// Registers an <see cref="AwaitableHandler"/> factory function for the given awaitable type.
		/// This allows Moq to properly recognize the awaitable type.
		/// </summary>
		/// <remarks>
		/// As an example, given an <see langword="await"/>-able type <c>TaskLike&lt;TResult&gt;</c>
		/// and a corresponding <see cref="AwaitableHandler"/> implementation <c>TaskLikeHandler</c>,
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
		///   will produce a suitable <see cref="AwaitableHandler"/> for it.</param>
		public static void Register(Type typeDefinition, Func<Type, AwaitableHandler> factory)
		{
			AwaitableHandler.factories[typeDefinition] = factory;
		}

		internal static AwaitableHandler TryGet(Type type)
		{
			var typeDefinition = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;
			return AwaitableHandler.factories.TryGetValue(typeDefinition, out var factory) ? factory.Invoke(type) : null;
		}

		/// <summary>
		///   Gets the type of result value represented by instances of this handler's awaitable type.
		/// </summary>
		/// <remarks>
		///   If this awaitable type does not have any result values, this property should return
		///   <see langword="typeof"/>(<see langword="void"/>).
		/// </remarks>
		public abstract Type ResultType { get; }

		/// <summary>
		///   Converts the given result value to a successfully completed awaitable.
		/// </summary>
		/// <remarks>
		///   If this awaitable types does not have any result values, <paramref name="result"/> may be ignored.
		/// </remarks>
		public abstract object CreateCompleted(object result);

		/// <summary>
		///   Converts the given exception to a faulted awaitable.
		/// </summary>
		public abstract object CreateFaulted(Exception exception);

		/// <summary>
		///   Attempts to extract the result value from the given awaitable.
		///   This will succeed only for a successfully completed awaitable that has a result value.
		/// </summary>
		/// <param name="awaitable">The awaitable from which a result value should be extracted.</param>
		/// <param name="result">
		///   If successful, this <see langword="out"/> parameter is set to the extracted result value.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if extraction of a result value succeeded;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public virtual bool TryGetResult(object awaitable, out object result)
		{
			result = null;

			if (awaitable != null)
			{
				var awaitableType = awaitable.GetType();
				var awaiter = awaitableType.GetMethod("GetAwaiter")?.Invoke(awaitable, null);
				if (awaiter != null)
				{
					var awaiterType = awaiter.GetType();
					var isCompleted = awaiterType.GetProperty("IsCompleted")?.GetValue(awaiter);
					if (object.Equals(isCompleted, true))
					{
						try
						{
							result = awaiterType.GetMethod("GetResult")?.Invoke(awaiter, null);
						}
						catch
						{
						}
					}
				}
			}

			return result != null;
		}
	}
}
