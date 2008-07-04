using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq
{
	/// <summary>
	/// Provides a typed <see cref="MockedEvent"/> for a 
	/// specific type of <see cref="EventArgs"/>.
	/// </summary>
	/// <typeparam name="TEventArgs">The type of event arguments required by the event.</typeparam>
	/// <remarks>
	/// The mocked event can either be a <see cref="EventHandler{TEventArgs}"/> or custom 
	/// event handler which follows .NET practice of providing <c>object sender, EventArgs args</c> 
	/// kind of signature.
	/// </remarks>
	public class MockedEvent<TEventArgs> : MockedEvent
		where TEventArgs : EventArgs
	{
		internal MockedEvent(Mock mock)
			: base(mock)
		{
		}

		/// <summary>
		/// Raises the associated event with the given 
		/// event argument data.
		/// </summary>
		public void Raise(TEventArgs args)
		{
			base.DoRaise(args);
		}

		/// <summary>
		/// Provides support for attaching a <see cref="MockedEvent{TEventArgs}"/> to 
		/// a generic <see cref="EventHandler{TEventArgs}"/> event.
		/// </summary>
		public static implicit operator EventHandler<TEventArgs>(MockedEvent<TEventArgs> mockEvent)
		{
			return mockEvent.Handle;
		}

		/// <summary>
		/// Provided solely to allow the interceptor to determine when the attached 
		/// handler is coming from this mocked event so we can assign the 
		/// corresponding EventInfo for it.
		/// </summary>
		private void Handle(object sender, TEventArgs args)
		{
		}
	}
}
