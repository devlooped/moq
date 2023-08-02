// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
			new Mock<EventHandler>();
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
			// it's good if multiple mocks for the same delegate (interface) both
			// consider themselves to be proxying for the same method.
			var mock1 = new Mock<PropertyChangedEventHandler>();
			var mock2 = new Mock<PropertyChangedEventHandler>();
			Assert.Same(mock1.Object.Method, mock2.Object.Method);
		}

		[Fact]
		public void CanHandleOutParameterOfActionAsSameAsVoidMethod()
		{
			var out1 = 42;
			var methMock = new Mock<TypeOutAction<int>>();
			methMock.Setup(t => t.Invoke(out out1));
			var dlgtMock = new Mock<DelegateOutAction<int>>();
			dlgtMock.Setup(f => f(out out1));

			var methOut1 = default(int);
			methMock.Object.Invoke(out methOut1);
			var dlgtOut1 = default(int);
			dlgtMock.Object(out dlgtOut1);

			Assert.Equal(methOut1, dlgtOut1);
		}

		[Fact]
		public void CanHandleRefParameterOfActionAsSameAsVoidMethod()
		{
			var ref1 = 42;
			var methMock = new Mock<TypeRefAction<int>>(MockBehavior.Strict);
			methMock.Setup(t => t.Invoke(ref ref1));
			var dlgtMock = new Mock<DelegateRefAction<int>>(MockBehavior.Strict);
			dlgtMock.Setup(f => f(ref ref1));

			var methRef1 = 42;
			methMock.Object.Invoke(ref methRef1);
			var dlgtRef1 = 42;
			dlgtMock.Object(ref dlgtRef1);

			methMock.VerifyAll();
			dlgtMock.VerifyAll();
		}

		[Fact]
		public void CanHandleOutParameterOfFuncAsSameAsReturnableMethod()
		{
			var out1 = 42;
			var methMock = new Mock<TypeOutFunc<int, int>>();
			methMock.Setup(t => t.Invoke(out out1)).Returns(114514);
			var dlgtMock = new Mock<DelegateOutFunc<int, int>>();
			dlgtMock.Setup(f => f(out out1)).Returns(114514);

			var methOut1 = default(int);
			var methResult = methMock.Object.Invoke(out methOut1);
			var dlgtOut1 = default(int);
			var dlgtResult = dlgtMock.Object(out dlgtOut1);

			Assert.Equal(methOut1, dlgtOut1);
			Assert.Equal(methResult, dlgtResult);
		}

		[Fact]
		public void CanHandleRefParameterOfFuncAsSameAsReturnableMethod()
		{
			var ref1 = 42;
			var methMock = new Mock<TypeRefFunc<int, int>>(MockBehavior.Strict);
			methMock.Setup(t => t.Invoke(ref ref1)).Returns(114514);
			var dlgtMock = new Mock<DelegateRefFunc<int, int>>(MockBehavior.Strict);
			dlgtMock.Setup(f => f(ref ref1)).Returns(114514);

			var methRef1 = 42;
			var methResult = methMock.Object.Invoke(ref methRef1);
			var dlgtRef1 = 42;
			var dlgtResult = dlgtMock.Object(ref dlgtRef1);

			methMock.VerifyAll();
			dlgtMock.VerifyAll();
			Assert.Equal(methResult, dlgtResult);
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

		public interface TypeOutAction<TOut1> { void Invoke(out TOut1 out1); }
		public delegate void DelegateOutAction<TOut1>(out TOut1 out1);
		public interface TypeRefAction<TRef1> { void Invoke(ref TRef1 ref1); }
		public delegate void DelegateRefAction<TRef1>(ref TRef1 ref1);
		public interface TypeOutFunc<TOut1, TResult> { TResult Invoke(out TOut1 out1); }
		public delegate TResult DelegateOutFunc<TOut1, TResult>(out TOut1 out1);
		public interface TypeRefFunc<TRef1, TResult> { TResult Invoke(ref TRef1 ref1); }
		public delegate TResult DelegateRefFunc<TRef1, TResult>(ref TRef1 ref1);
	}
}
