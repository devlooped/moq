using System;
using System.ComponentModel;
using Xunit;

namespace Moq.Tests
{
	public class MockedDelegatesFixture
	{
		[Fact]
		public void CanMockDelegate()
		{
			Assert.DoesNotThrow(() => new Mock<EventHandler>());
		}

		[Fact]
		public void CannotPassParametersToMockDelegate()
		{
			// Pass in parameters that match the delegate signature,
			// but this still makes absolutely no sense.
			Assert.Throws<ArgumentException>(() =>
				new Mock<EventHandler>(this, EventArgs.Empty));
		}

		[Fact]
		public void CanVerifyLooseMockDelegateWithNoReturnValue()
		{
			var mockIntAcceptingAction = new Mock<Action<int>>(MockBehavior.Loose);

			Use(mockIntAcceptingAction.Object, 3);

			mockIntAcceptingAction.Verify(act => act(3));
		}

		[Fact]
		public void CanSetupStrictMockDelegateWithNoReturnValue()
		{
			var mockIntAcceptingAction = new Mock<Action<int>>(MockBehavior.Strict);

			mockIntAcceptingAction.Setup(act => act(7));

			Use(mockIntAcceptingAction.Object, 7);
		}

		[Fact]
		public void CanVerifyLooseMockDelegateWithReturnValue()
		{
			var mockIntAcceptingStringReturningAction = new Mock<Func<int, string>>(MockBehavior.Loose);

			mockIntAcceptingStringReturningAction
				.Setup(f => f(It.IsAny<int>()))
				.Returns("hello");

			var result = UseAndGetReturn(mockIntAcceptingStringReturningAction.Object, 96);

			mockIntAcceptingStringReturningAction
				.Verify(f => f(96));
			Assert.Equal("hello", result);
		}

		[Fact]
		public void CanSubscribeMockDelegateAsEventListener()
		{
			var notifyingObject = new NotifyingObject();
			var mockListener = new Mock<PropertyChangedEventHandler>();
			notifyingObject.PropertyChanged += mockListener.Object;

			notifyingObject.Value = 5;

			// That should have caused one event to have been fired.
			mockListener
				.Verify(l => l(notifyingObject,
							   It.Is<PropertyChangedEventArgs>(e => e.PropertyName == "Value")),
						Times.Once());
		}

		[Fact]
		public void DelegateInterfacesAreReused()
		{
			// White box test: we want to ensure that we're not creating new proxy interfaces
			// every time we come across a particular delegate type.  Specifically,
			// it's good if multiple mocks for the same delegate (interface) both
			// consider themselves to be proxying for the same method.
			var mock1 = new Mock<PropertyChangedEventHandler>();
			var mock2 = new Mock<PropertyChangedEventHandler>();

			Assert.Same(mock1.DelegateInterfaceMethod, mock2.DelegateInterfaceMethod);
		}

		private static void Use(Action<int> action, int valueToPass)
		{
			action(valueToPass);
		}

		private static string UseAndGetReturn(Func<int, string> func, int valueToPass)
		{
			return func(valueToPass);
		}

		private class NotifyingObject : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private int value;
			public int Value
			{
				get { return value; }
				set
				{
					this.value = value;
					var listeners = PropertyChanged;
					if (listeners != null)
					{
						listeners(this, new PropertyChangedEventArgs("Value"));
					}
				}
			}
		}
	}
}
