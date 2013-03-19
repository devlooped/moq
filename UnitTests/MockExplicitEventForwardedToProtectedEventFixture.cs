using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Xunit;

namespace Moq.Tests
{
    public class MockExplicitEventForwardedToProtectedEventFixture
    {

        public interface IInterfaceWithEvent
        {
            event EventHandler<EventArgs> JustAnEvent;
        }

        public class Implementation : IInterfaceWithEvent
        {
            private EventHandler<EventArgs> _justAnEvent;

            event EventHandler<EventArgs> IInterfaceWithEvent.JustAnEvent
            {
                add { this.JustAnEvent += value; }
                remove { this.JustAnEvent -= value; }
            }

            protected virtual event EventHandler<EventArgs> JustAnEvent
            {
                add
                {
                    EventHandler<EventArgs> changedEventHandler = this._justAnEvent;
                    EventHandler<EventArgs> comparand;
                    do
                    {
                        comparand = changedEventHandler;
                        changedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this._justAnEvent, comparand + value, comparand);
                    }
                    while (changedEventHandler != comparand);
                }
                remove
                {
                    EventHandler<EventArgs> changedEventHandler = this._justAnEvent;
                    EventHandler<EventArgs> comparand;
                    do
                    {
                        comparand = changedEventHandler;
                        changedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this._justAnEvent, comparand - value, comparand);
                    }
                    while (changedEventHandler != comparand);
                }
            }
        }

        [Fact]
        public void ReproduceBug()
        {
            var mock = new Mock<Implementation>();
            IInterfaceWithEvent observable = mock.Object;

            observable.JustAnEvent += (sender, args) => { };


        }


        [Fact]
        public void ReproduceBugWithObservableCollection()
        {
            var mock = new Mock<ObservableCollection<int>>();
            INotifyPropertyChanged observable = mock.Object;
            observable.PropertyChanged += (sender, args) => { };
        }
    }
}