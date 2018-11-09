// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	///   Abstract base class for default value providers that look up and delegate to value factory functions for the requested type(s).
	///   If a request cannot be satisfied by any registered factory, the default value gets produced by a fallback strategy.
	/// </summary>
	/// <remarks>
	///   <para>
	///     Derived classes can register and deregister factory functions with <see cref="Register"/> and <see cref="Deregister"/>,
	///     respectively.
	///   </para>
	///   <para>
	///     The fallback value generation strategy is implemented by the overridable <see cref="GetFallbackDefaultValue"/> method.
	///   </para>
	///   <para>
	///     This base class sets up factory functions for task types (<see cref="Task"/>, <see cref="Task{TResult}"/>,
	///     and <see cref="ValueTask{TResult}"/>) that produce completed tasks containing default values.
	///     If this behavior is not desired, derived classes may deregister those standard factory functions via <see cref="Deregister"/>.
	///   </para>
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class LookupOrFallbackDefaultValueProvider : DefaultValueProvider
	{
		private Dictionary<object, Func<Type, Mock, object>> factories;

		/// <summary>
		/// Initializes a new instance of the <see cref="LookupOrFallbackDefaultValueProvider"/> class.
		/// </summary>
		protected LookupOrFallbackDefaultValueProvider()
		{
			this.factories = new Dictionary<object, Func<Type, Mock, object>>()
			{
				[typeof(Task)] = CreateTask,
				[typeof(Task<>)] = CreateTaskOf,
				[typeof(ValueTask<>)] = CreateValueTaskOf,
				["System.ValueTuple`1"] = CreateValueTupleOf,
				["System.ValueTuple`2"] = CreateValueTupleOf,
				["System.ValueTuple`3"] = CreateValueTupleOf,
				["System.ValueTuple`4"] = CreateValueTupleOf,
				["System.ValueTuple`5"] = CreateValueTupleOf,
				["System.ValueTuple`6"] = CreateValueTupleOf,
				["System.ValueTuple`7"] = CreateValueTupleOf,
				["System.ValueTuple`8"] = CreateValueTupleOf,
			};
		}

		/// <summary>
		/// Deregisters any factory function that might currently be registered for the given type(s).
		/// Subsequent requests for values of the given type(s) will be handled by the fallback strategy.
		/// </summary>
		/// <param name="factoryKey">The type(s) for which to remove any registered factory function.</param>
		protected void Deregister(Type factoryKey)
		{
			Debug.Assert(factoryKey != null);

			this.factories.Remove(factoryKey);
			this.factories.Remove(factoryKey.FullName);
		}

		/// <summary>
		/// Registers a factory function for the given type(s).
		/// Subsequent requests for values of the given type(s) will be handled by the specified function.
		/// </summary>
		/// <param name="factoryKey">
		///   <para>
		///     The type(s) for which to register the given <paramref name="factory"/> function.
		///   </para>
		///   <para>
		///     All array types are represented by <c><see langword="typeof"/>(<see cref="Array"/>)</c>.
		///     Generic types are represented by their open generic type definition, e. g. <c><see langword="typeof"/>(<see cref="IEnumerable"/>&lt;&gt;)</c>.
		///   </para>
		/// </param>
		/// <param name="factory">The factory function responsible for producing values for the given type.</param>
		protected void Register(Type factoryKey, Func<Type, Mock, object> factory)
		{
			Debug.Assert(factoryKey != null);
			Debug.Assert(factory != null);

			this.factories[factoryKey] = factory;
		}

		/// <inheritdoc/>
		protected internal sealed override object GetDefaultParameterValue(ParameterInfo parameter, Mock mock)
		{
			Debug.Assert(parameter != null);
			Debug.Assert(parameter.ParameterType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValue(parameter.ParameterType, mock);
		}

		/// <inheritdoc/>
		protected internal sealed override object GetDefaultReturnValue(MethodInfo method, Mock mock)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValue(method.ReturnType, mock);
		}

		/// <inheritdoc/>
		protected internal sealed override object GetDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			var typeInfo = type.GetTypeInfo();

			var handlerKey = typeInfo.IsGenericType ? type.GetGenericTypeDefinition()
			               : type.IsArray ? typeof(Array)
			               : type;

			Func<Type, Mock, object> factory;
			return this.factories.TryGetValue(handlerKey         , out factory) ? factory.Invoke(type, mock)
			     : this.factories.TryGetValue(handlerKey.FullName, out factory) ? factory.Invoke(type, mock)
			     : this.GetFallbackDefaultValue(type, mock);
		}

		/// <summary>
		/// Determines the default value for the given <paramref name="type"/> when no suitable factory is registered for it.
		/// May be overridden in derived classes.
		/// </summary>
		/// <param name="type">The type of which to produce a value.</param>
		/// <param name="mock">The <see cref="Moq.Mock"/> on which an unexpected invocation has occurred.</param>
		protected virtual object GetFallbackDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			return type.GetDefaultValue();
		}

		private static object CreateTask(Type type, Mock mock)
		{
			return Task.FromResult(false);
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

		private object CreateValueTupleOf(Type type, Mock mock)
		{
			var itemTypes = type.GetGenericArguments();
			var items = new object[itemTypes.Length];
			for (int i = 0, n = itemTypes.Length; i < n; ++i)
			{
				items[i] = this.GetDefaultValue(itemTypes[i], mock);
			}
			return Activator.CreateInstance(type, items);
		}
	}
}
