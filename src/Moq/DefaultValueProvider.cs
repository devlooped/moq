// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// <see cref="DefaultValueProvider"/> is the abstract base class for default value providers.
	/// These are responsible for producing e. g. return values when mock methods or properties get invoked unexpectedly.
	/// In other words, whenever there is no setup that would determine the return value for a particular invocation,
	/// Moq asks the mock's default value provider to produce a return value.
	/// </summary>
	public abstract class DefaultValueProvider
	{
		/// <summary>
		/// Gets the <see cref="DefaultValueProvider"/> corresponding to <see cref="DefaultValue.Empty"/>;
		/// that is, a default value provider returning "empty" values e. g. for collection types.
		/// </summary>
		public static DefaultValueProvider Empty { get; } = new EmptyDefaultValueProvider();

		/// <summary>
		/// Gets the <see cref="DefaultValueProvider"/> corresponding to <see cref="DefaultValue.Mock"/>;
		/// that is, a default value provider returning mocked objects or "empty" values for unmockable types.
		/// </summary>
		public static DefaultValueProvider Mock { get; } = new MockDefaultValueProvider();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultValueProvider"/> class.
		/// </summary>
		protected DefaultValueProvider()
		{
		}

		/// <summary>
		/// Gets the <see cref="DefaultValue"/> enumeration value that corresponds to this default value provider.
		/// Must be overridden by Moq's internal providers that have their own corresponding <see cref="DefaultValue"/>.
		/// </summary>
		internal virtual DefaultValue Kind => DefaultValue.Custom;

		/// <summary>
		/// Produces a default value of the specified type.
		/// Must be overridden in derived classes.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the requested default value.</param>
		/// <param name="mock">The <see cref="Moq.Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// Implementations may assume that all parameters have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal abstract object GetDefaultValue(Type type, Mock mock);

		/// <summary>
		///   <para>
		///     Produces a default argument value for the specified method parameter.
		///     May be overridden in derived classes.
		///   </para>
		///   <para>
		///     By default, this method will delegate to <see cref="GetDefaultValue"/>.
		///   </para>
		/// </summary>
		/// <param name="parameter">The <see cref="ParameterInfo"/> describing the method parameter for which a default argument value should be produced.</param>
		/// <param name="mock">The <see cref="Moq.Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// Implementations may assume that all parameters have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal virtual object GetDefaultParameterValue(ParameterInfo parameter, Mock mock)
		{
			Debug.Assert(parameter != null);
			Debug.Assert(parameter.ParameterType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValue(parameter.ParameterType, mock);
		}

		/// <summary>
		///   <para>
		///     Produces a default return value for the specified method.
		///     May be overridden in derived classes.
		///   </para>
		///   <para>
		///     By default, this method will delegate to <see cref="GetDefaultValue"/>.
		///   </para>
		/// </summary>
		/// <param name="method">The <see cref="MethodInfo"/> describing the method for which a default return value should be produced.</param>
		/// <param name="mock">The <see cref="Moq.Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// Implementations may assume that all parameters have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal virtual object GetDefaultReturnValue(MethodInfo method, Mock mock)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValue(method.ReturnType, mock);
		}
	}
}
