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
		IVerifies Raises(MockedEvent eventHandler, EventArgs args);
	}
}
