using System;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Represents a generic event that has been mocked and can 
	/// be rised.
	/// </summary>
	public class MockedEvent
	{
		/// <summary>
		/// Event raised whenever the mocked event is rised.
		/// </summary>
		public event EventHandler Raised;

		Mock mock;

		internal MockedEvent(Mock mock)
		{
			this.mock = mock;
		}

		internal EventInfo Event { get; set; }

		/// <summary>
		/// Provided solely to allow the interceptor to determine when the attached 
		/// handler is coming from this mocked event so we can assign the 
		/// corresponding EventInfo for it.
		/// </summary>
		private void Handle(object sender, EventArgs args)
		{
		}

		/// <summary>
		/// Raises the associated event with the given 
		/// event argument data.
		/// </summary>
		internal void DoRaise(EventArgs args)
		{
			if (Event == null)
				throw new InvalidOperationException(Properties.Resources.RaisedUnassociatedEvent);

			if (Raised != null)
				Raised(this, EventArgs.Empty);

			foreach (var del in mock.GetInvocationList(Event))
			{
				del.InvokePreserveStack(mock.Object, args);
			}
		}

		/// <summary>
		/// Provides support for attaching a <see cref="MockedEvent"/> to 
		/// a generic <see cref="EventHandler"/> event.
		/// </summary>
		public static implicit operator EventHandler(MockedEvent mockEvent)
		{
			return mockEvent.Handle;
		}
	}
}
