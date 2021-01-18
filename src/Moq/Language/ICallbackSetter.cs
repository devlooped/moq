// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb for property setter setups.
	/// </summary>
	/// <typeparam name="TProperty">Type of the property.</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallbackSetter<TProperty> : IFluentInterface
	{ 
		/// <summary>
		/// Specifies a callback to invoke when the property is set that receives the 
		/// property value being set.
		/// </summary>
		/// <param name="action">Callback method to invoke.</param>
		/// <example>
		/// Invokes the given callback with the property value being set. 
		/// <code>
		/// mock.SetupSet(x => x.Suspended)
		///     .Callback((bool state) => Console.WriteLine(state));
		/// </code>
		/// </example>
		ICallbackResult Callback(Action<TProperty> action);
	}
}
