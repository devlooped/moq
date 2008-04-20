using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Raises</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IRaise : IHideObjectMembers
	{
		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="args">The event args to pass when raising the event.</param>
		/// <example>
		/// The following example shows how to set an expectation that will 
		/// raise an event when it's met:
		/// <code>
		/// var mock = new Mock&lt;IContainer&gt;();
		/// // create handler to associate with the event to raise
		/// var handler = mock.CreateEventHandler();
		/// // associate the handler with the event to raise
		/// mock.Object.Added += handler;
		/// // set the expectation and the handler to raise
		/// mock.Expect(add => add.Add(It.IsAny&lt;string&gt;(), It.IsAny&lt;object&gt;()))
		///     .Raises(handler, EventArgs.Empty);
		/// </code>
		/// </example>
		IVerifies Raises(MockedEvent eventHandler, EventArgs args);

		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <seealso cref="Raises(MockedEvent, EventArgs)"/>
		IVerifies Raises(MockedEvent eventHandler, Func<EventArgs> func);

		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T">Type of the argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(MockedEvent, EventArgs)"/>
		IVerifies Raises<T>(MockedEvent eventHandler, Func<T, EventArgs> func);

		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">Type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">Type of the second argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(MockedEvent, EventArgs)"/>
		IVerifies Raises<T1, T2>(MockedEvent eventHandler, Func<T1, T2, EventArgs> func);

		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">Type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">Type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">Type of the third argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(MockedEvent, EventArgs)"/>
		IVerifies Raises<T1, T2, T3>(MockedEvent eventHandler, Func<T1, T2, T3, EventArgs> func);

		/// <summary>
		/// Specifies the mocked event that will be raised 
		/// when the expectation is met.
		/// </summary>
		/// <param name="eventHandler">The mocked event, retrieved from 
		/// <see cref="Mock.CreateEventHandler"/> or <see cref="Mock.CreateEventHandler{TEventArgs}"/>.
		/// </param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <typeparam name="T1">Type of the first argument received by the expected invocation.</typeparam>
		/// <typeparam name="T2">Type of the second argument received by the expected invocation.</typeparam>
		/// <typeparam name="T3">Type of the third argument received by the expected invocation.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument received by the expected invocation.</typeparam>
		/// <seealso cref="Raises(MockedEvent, EventArgs)"/>
		IVerifies Raises<T1, T2, T3, T4>(MockedEvent eventHandler, Func<T1, T2, T3, T4, EventArgs> func);
	}
}
