// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Raises</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface IRaise<T> : IFluentInterface
	{
		/// <summary>
		/// Specifies the event that will be raised 
		/// when the setup is met.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="args">The event arguments to pass for the raised event.</param>
		/// <example>
		/// The following example shows how to raise an event when 
		/// the setup is met:
		/// <code>
		/// var mock = new Mock&lt;IContainer&gt;();
		/// 
		/// mock.Setup(add => add.Add(It.IsAny&lt;string&gt;(), It.IsAny&lt;object&gt;()))
		///     .Raises(add => add.Added += null, EventArgs.Empty);
		/// </code>
		/// </example>
		IVerifies Raises(Action<T> eventExpression, EventArgs args);

		/// <summary>
		/// Specifies the event that will be raised 
		/// when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises(Action<T> eventExpression, Func<EventArgs> func);

		/// <summary>
		/// Specifies the custom event that will be raised 
		/// when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="args">The arguments to pass to the custom delegate (non EventHandler-compatible).</param>
		IVerifies Raises(Action<T> eventExpression, params object[] args);
	}
}
