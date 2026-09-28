using System;
using System.ComponentModel;
using System.Reflection;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class EventBehaviorTests
    {
        [Fact]
        public void AppliesToAdd()
        {
            var behavior = new EventBehavior();
            Assert.True(behavior.AppliesTo(MethodInvocation.Create(
                new EventfulMock(),
                typeof(EventfulMock).GetEvent(nameof(EventfulMock.Empty)).GetAddMethod(),
                new EventHandler((s, a) => { }))));
        }

        [Fact]
        public void AppliesToRemove()
        {
            var behavior = new EventBehavior();
            Assert.True(behavior.AppliesTo(MethodInvocation.Create(
                new EventfulMock(),
                typeof(EventfulMock).GetEvent(nameof(EventfulMock.Empty)).GetRemoveMethod(),
                new EventHandler((s, a) => { }))));
        }

        [Fact]
        public void AddsHandler()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            EventHandler handler = (_, __) => { };
            mock.Empty += handler;

            Assert.True(mock.AsMock().State.TryGetValue<Delegate>(nameof(IEventful.Empty), out var e));
            Assert.Contains(handler, e.GetInvocationList());
        }

        [Fact]
        public void RemovesHandler()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            EventHandler handler = (_, __) => { };
            mock.Empty += handler;
            mock.Empty -= handler;

            Assert.True(mock.AsMock().State.TryGetValue<Delegate>(nameof(IEventful.Empty), out var e));
            Assert.Null(e);

            mock.Empty += handler;
            Assert.True(mock.AsMock().State.TryGetValue<Delegate>(nameof(IEventful.Empty), out e));
            Assert.Contains(handler, e.GetInvocationList());
        }


        [Fact]
        public void RaisesEventHandlerIfRaiserInContextOnAdd()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            var called = false;
            EventHandler handler = (_, __) => called = true;
            mock.Empty += handler;

            CallContext<EventRaiser>.SetData(EventRaiser.Empty);
            mock.Empty += null;

            Assert.True(called);
        }

        [Fact]
        public void RaisesEventArgsIfRaiserInContextOnAdd()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            var expected = new Args();
            var actual = default(Args);
            EventHandler<Args> handler = (sender, args) => actual = args;
            mock.WithArgs += handler;

            CallContext<EventRaiser>.SetData(new EventArgsEventRaiser(expected));
            mock.WithArgs += null;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RaisesPropertyChangedIfRaiserInContextOnAdd()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            var expected = new PropertyChangedEventArgs("Foo");
            var actual = default(PropertyChangedEventArgs);
            PropertyChangedEventHandler handler = (sender, args) => actual = args;
            mock.PropertyChanged += handler;

            CallContext<EventRaiser>.SetData(new EventArgsEventRaiser(expected));
            mock.PropertyChanged += null;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RaisesActionIfRaiserInContextOnAdd()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            var expected = 5;
            var actual = 0;
            Action<int> handler = i => actual = i;
            mock.Action += handler;

            CallContext<EventRaiser>.SetData(new CustomEventRaiser(new object[] { expected }));
            mock.Action += null;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RaisesCustomDelegateIfRaiserInContextOnAdd()
        {
            var mock = new EventfulMock();
            mock.Behaviors.Add(new EventBehavior());
            mock.AddBehavior((m, n) => m.CreateReturn());

            var (id, name) = (5, "foo");
            var (id2, name2) = (0, "");

            CustomDelegate handler = (i, n) => { id2 = i; name2 = n; };
            mock.Custom += handler;

            CallContext<EventRaiser>.SetData(new CustomEventRaiser(new object[] { id, name }));
            mock.Custom += null;

            Assert.Equal(id, id2);
            Assert.Equal(name, name2);
        }

        public class EventfulMock : FakeMock, IEventful
        {
            public event EventHandler Empty
            {
                add => Pipeline.Execute<EventHandler>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
                remove => Pipeline.Execute<EventHandler>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
            }

            public event EventHandler<Args> WithArgs
            {
                add => Pipeline.Execute<EventHandler<Args>>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
                remove => Pipeline.Execute<EventHandler<Args>>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
            }

            public event PropertyChangedEventHandler PropertyChanged
            {
                add => Pipeline.Execute<PropertyChangedEventHandler>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
                remove => Pipeline.Execute<PropertyChangedEventHandler>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
            }

            public event CustomDelegate Custom
            {
                add => Pipeline.Execute<CustomDelegate>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
                remove => Pipeline.Execute<CustomDelegate>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
            }

            public event Action<int> Action
            {
                add => Pipeline.Execute<Action<int>>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
                remove => Pipeline.Execute<Action<int>>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), value));
            }
        }

        public interface IEventful
        {
            event EventHandler Empty;
            event EventHandler<Args> WithArgs;
            event PropertyChangedEventHandler PropertyChanged;
            event CustomDelegate Custom;
            event Action<int> Action;
        }

        public delegate void CustomDelegate(int id, string name);

        public class Args : EventArgs { }
    }
}
