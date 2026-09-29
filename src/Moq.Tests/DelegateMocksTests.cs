using System;
using System.ComponentModel;
using Moq.Sdk;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    public class DelegateMocksTests
    {
        [Fact]
        public void CanMockDelegate()
        {
            var mock = new Mock<EventHandler>();

            mock.Object.Invoke(this, EventArgs.Empty);

            Assert.Single(mock.Sdk.Invocations);
        }

        [Fact]
        public void CanVerifyLooseMockDelegateWithNoReturnValue()
        {
            var action = new Mock<Action<int>>(MockBehavior.Loose);

            Use(action.Object, 3);

            Verify.Called(action).Invoke(3);
            Verify.Called(() => action.Object(3));
        }

        [Fact]
        public void CanSetupStrictMockDelegateWithNoReturnValue()
        {
            var action = new Mock<Action<int>>(MockBehavior.Strict);

            action.Invoke(7);

            Use(action.Object, 7);
            Assert.Throws<StrictMockException>(() => Use(action.Object, 8));
        }

        [Fact]
        public void CanVerifyLooseMockDelegateWithReturnValue()
        {
            var func = new Mock<Func<int, string>>(MockBehavior.Loose);

            func.Invoke(Any<int>()).Returns("hello");

            var result = UseAndGetReturn(func.Object, 96);

            Verify.Called(func).Invoke(96);
            Assert.Equal("hello", result);
        }

        [Fact]
        public void CanSubscribeMockDelegateAsEventListener()
        {
            var notifyingObject = new NotifyingObject();
            var listener = new Mock<PropertyChangedEventHandler>();
            notifyingObject.PropertyChanged += listener.Object;

            notifyingObject.Value = 5;

            // That should have caused one event to have been fired.
            Verify.Called(listener)
                .Invoke(notifyingObject, Any<PropertyChangedEventArgs>(e => e?.PropertyName == "Value"))
                .Once();
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
            methMock.Invoke(out _).Callback((out int x) => x = out1);
            var dlgtMock = new Mock<DelegateOutAction<int>>();
            dlgtMock.Invoke(out _).Callback((out int x) => x = out1);

            methMock.Object.Invoke(out var methOut1);
            dlgtMock.Object(out var dlgtOut1);

            Assert.Equal(42, methOut1);
            Assert.Equal(methOut1, dlgtOut1);
        }

        [Fact]
        public void CanHandleRefParameterOfActionAsSameAsVoidMethod()
        {
            var ref1 = 42;
            var methMock = new Mock<TypeRefAction<int>>(MockBehavior.Strict);
            methMock.Invoke(ref ref1).Once();
            var dlgtMock = new Mock<DelegateRefAction<int>>(MockBehavior.Strict);
            dlgtMock.Invoke(ref ref1).Once();

            var methRef1 = 42;
            methMock.Object.Invoke(ref methRef1);
            var dlgtRef1 = 42;
            dlgtMock.Object(ref dlgtRef1);

            Verify.Calls(methMock);
            Verify.Calls(dlgtMock);
        }

        [Fact]
        public void CanHandleOutParameterOfFuncAsSameAsReturnableMethod()
        {
            var methMock = new Mock<TypeOutFunc<int, int>>();
            methMock.Invoke(out _).Returns((out int x) => (x = 42) + 114472);
            var dlgtMock = new Mock<DelegateOutFunc<int, int>>();
            dlgtMock.Invoke(out _).Returns((out int x) => (x = 42) + 114472);

            var methResult = methMock.Object.Invoke(out var methOut1);
            var dlgtResult = dlgtMock.Object(out var dlgtOut1);

            Assert.Equal(42, methOut1);
            Assert.Equal(methOut1, dlgtOut1);
            Assert.Equal(114514, methResult);
            Assert.Equal(methResult, dlgtResult);
        }

        [Fact]
        public void CanHandleRefParameterOfFuncAsSameAsReturnableMethod()
        {
            var ref1 = 42;
            var methMock = new Mock<TypeRefFunc<int, int>>(MockBehavior.Strict);
            methMock.Invoke(ref ref1).Returns(114514).Once();
            var dlgtMock = new Mock<DelegateRefFunc<int, int>>(MockBehavior.Strict);
            dlgtMock.Invoke(ref ref1).Returns(114514).Once();

            var methRef1 = 42;
            var methResult = methMock.Object.Invoke(ref methRef1);
            var dlgtRef1 = 42;
            var dlgtResult = dlgtMock.Object(ref dlgtRef1);

            Verify.Calls(methMock);
            Verify.Calls(dlgtMock);
            Assert.Equal(114514, methResult);
            Assert.Equal(methResult, dlgtResult);
        }

        static void Use(Action<int> action, int valueToPass) => action(valueToPass);

        static string UseAndGetReturn(Func<int, string> func, int valueToPass) => func(valueToPass);

        class NotifyingObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            int value;
            public int Value
            {
                get => value;
                set
                {
                    this.value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
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
