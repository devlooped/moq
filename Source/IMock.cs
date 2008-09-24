using System;
namespace Moq
{
	/// <summary>
	/// Base mock interface exposing non-generic members.
	/// </summary>
	public interface IMock
	{
		/// <summary>
		/// Whether the base member virtual implementation will be called 
		/// for mocked classes if no expectation is met. Defaults to <see langword="true"/>.
		/// </summary>
		bool CallBase { get; set; }

		/// <summary>
		/// Determines how to generate default values for loose mocks on 
		/// unexpected invocations.
		/// </summary>
		DefaultValue DefaultValue { get; set; }

		/// <summary>
		/// Creates a handler that can be associated to an event receiving 
		/// the given <typeparamref name="TEventArgs"/> and can be used 
		/// to raise the event.
		/// </summary>
		/// <typeparam name="TEventArgs">Type of <see cref="EventArgs"/> 
		/// data passed in to the event.</typeparam>
		/// <example>
		/// This example shows how to invoke an event with a custom event arguments 
		/// class in a view that will cause its corresponding presenter to 
		/// react by changing its state:
		/// <code>
		/// var mockView = new Mock&lt;IOrdersView&gt;();
		/// var mockedEvent = mockView.CreateEventHandler&lt;OrderEventArgs&gt;();
		/// 
		/// var presenter = new OrdersPresenter(mockView.Object);
		/// 
		/// // Check that the presenter has no selection by default
		/// Assert.Null(presenter.SelectedOrder);
		/// 
		/// // Create a mock event handler of the appropriate type
		/// var handler = mockView.CreateEventHandler&lt;OrderEventArgs&gt;();
		/// // Associate it with the event we want to raise
		/// mockView.Object.Cancel += handler;
		/// // Finally raise the event with a specific arguments data
		/// handler.Raise(new OrderEventArgs { Order = new Order("moq", 500) });
		/// 
		/// // Now the presenter reacted to the event, and we have a selected order
		/// Assert.NotNull(presenter.SelectedOrder);
		/// Assert.Equal("moq", presenter.SelectedOrder.ProductName);
		/// </code>
		/// </example>
		MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>() where TEventArgs : EventArgs;

		/// <summary>
		/// Creates a handler that can be associated to an event receiving 
		/// a generic <see cref="EventArgs"/> and can be used 
		/// to raise the event.
		/// </summary>
		/// <example>
		/// This example shows how to invoke a generic event in a view that will 
		/// cause its corresponding presenter to react by changing its state:
		/// <code>
		/// var mockView = new Mock&lt;IOrdersView&gt;();
		/// var mockedEvent = mockView.CreateEventHandler();
		/// 
		/// var presenter = new OrdersPresenter(mockView.Object);
		/// 
		/// // Check that the presenter is not in the "Canceled" state
		/// Assert.False(presenter.IsCanceled);
		/// 
		/// // Create a mock event handler of the appropriate type
		/// var handler = mockView.CreateEventHandler();
		/// // Associate it with the event we want to raise
		/// mockView.Object.Cancel += handler;
		/// // Finally raise the event
		/// handler.Raise(EventArgs.Empty);
		/// 
		/// // Now the presenter reacted to the event, and changed its state
		/// Assert.True(presenter.IsCanceled);
		/// </code>
		/// </example>
		MockedEvent<EventArgs> CreateEventHandler();

		/// <summary>
		/// The mocked object instance.
		/// </summary>
		object Object { get; }

		/// <summary>
		/// Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation and marks it as verifiable. After 
		/// the mock is used, a <see cref="Verify()"/> call is issued on the mock 
		/// to ensure the method in the expectation was invoked:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Expect(x =&gt; x.HasInventory(TALISKER, 50)).Verifiable().Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory.
		/// mock.Verify();
		/// </code>
		/// </example>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		void Verify();

		/// <summary>
		/// Verifies all expectations regardless of whether they have 
		/// been flagged as verifiable.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation without marking it as verifiable. After 
		/// the mock is used, a <see cref="VerifyAll"/> call is issued on the mock 
		/// to ensure that all expectations are met:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Expect(x =&gt; x.HasInventory(TALISKER, 50)).Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory, even 
		/// // that expectation was not marked as verifiable.
		/// mock.VerifyAll();
		/// </code>
		/// </example>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		void VerifyAll();
	}
}
