using System;
using Xunit;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Moq.Tests
{
	public class MockedEventsFixture
	{
		[Fact]
		public void ShouldExpectAddHandler()
		{
			var view = new Mock<IFooView>();

			var handler = view.CreateEventHandler<FooArgs>();

			view.Object.FooSelected += handler;

			var presenter = new FooPresenter(view.Object);
			bool fired = false;

			presenter.Fired += (sender, args) =>
				{
					fired = true;
					Assert.True(args is FooArgs);
					Assert.Equal("foo", ((FooArgs)args).Value);
				};

			handler.Raise(new FooArgs { Value = "foo" });

			Assert.True(fired);
		}

		[Fact]
		public void ShouldRaiseEventIfAttachedAfterUse()
		{
			var view = new Mock<IFooView>();
			var presenter = new FooPresenter(view.Object);

			Assert.False(presenter.Canceled);

			var handler = view.CreateEventHandler();
			view.Object.Canceled += handler;
			handler.Raise(EventArgs.Empty);

			Assert.True(presenter.Canceled);
		}

		[Fact]
		public void ShouldExpectAddGenericHandler()
		{
			var view = new Mock<IFooView>();
			var handler = view.CreateEventHandler();
			view.Object.Canceled += handler;
			var presenter = new FooPresenter(view.Object);

			Assert.False(presenter.Canceled);

			handler.Raise(EventArgs.Empty);

			Assert.True(presenter.Canceled);
		}

		[Fact]
		public void ShouldNotThrowIfEventIsNotMocked()
		{
			var view = new Mock<IFooView>();

			// Presenter class attaches to the event and nothing happens.
			var presenter = new FooPresenter(view.Object);
		}

		[Fact]
		public void ShouldRaiseEventWhenExpectationMet()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();
			var raised = false;
			handler.Raised += delegate { raised = true; };

			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(handler, EventArgs.Empty);

			mock.Object.Added += handler;

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithArgWhenExpectationMet()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();
			var raised = false;
			handler.Raised += delegate { raised = true; };

			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(handler, EventArgs.Empty);

			mock.Object.Added += handler;

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWhenExpectationMetReturn()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();
			var raised = false;
			handler.Raised += delegate { raised = true; };
			mock.Object.Added += handler;

			mock.Setup(add => add.Insert(It.IsAny<string>(), 0))
				.Returns(1)
				.Raises(handler, EventArgs.Empty);

			int value = mock.Object.Insert("foo", 0);

			Assert.Equal(1, value);
			Assert.True(raised);
		}

		[Fact]
		public void ShouldThrowIfRaisesNullMockedEvent()
		{
			var mock = new Mock<IAdder<string>>();

			Assert.Throws<ArgumentNullException>(() => mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(null, EventArgs.Empty));
		}

		[Fact]
		public void ShouldThrowIfRaisesNullArgs()
		{
			var mock = new Mock<IAdder<string>>();

			Assert.Throws<ArgumentNullException>(() => mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(mock.CreateEventHandler(), (EventArgs)null));
		}

		[Fact]
		public void ShouldPreserveStackTraceWhenRaisingEvent()
		{
			var mock = new Mock<IAdder<string>>();
			var handler = mock.CreateEventHandler();

			mock.Object.Added += handler;
			mock.Object.Added += delegate { throw new InvalidOperationException(); };

			Assert.Throws<InvalidOperationException>(() => handler.Raise(EventArgs.Empty));
		}

		[Fact]
		public void ShouldThrowIfRaisingNonAttachedEvent()
		{
			var mock = new Mock<IFooView>();
			var handler = mock.CreateEventHandler();

			Assert.Throws<InvalidOperationException>(() => handler.Raise(EventArgs.Empty));
		}

		[Fact]
		public void ShouldRaiseEventWithFunc()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();
			var raised = false;
			handler.Raised += delegate { raised = true; };

			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(handler, () => EventArgs.Empty);

			mock.Object.Added += handler;

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithFuncOneArg()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();

			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(handler, (string s) => new FooArgs { Value = s });

			mock.Object.Added += handler;

			var raised = false;
			handler.Raised += (sender, args) => raised = true;

			mock.Object.Added += (sender, args) =>
			{
				Assert.True(args is FooArgs);
				Assert.Equal("foo", ((FooArgs)args).Value);
			};

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithFuncTwoArgs()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>()))
				.Raises(handler, (string s, int i) => new FooArgs { Args = new object[] { s, i } });

			mock.Object.Added += handler;

			var raised = false;
			handler.Raised += (sender, args) => raised = true;

			mock.Object.Added += (sender, args) =>
			{
				Assert.True(args is FooArgs);
				Assert.Equal("foo", ((FooArgs)args).Args[0]);
				Assert.Equal(5, ((FooArgs)args).Args[1]);
			};

			mock.Object.Do("foo", 5);

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithFuncThreeArgs()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
				.Raises(handler, (string s, int i, bool b) => new FooArgs { Args = new object[] { s, i, b } });

			mock.Object.Added += handler;

			var raised = false;
			handler.Raised += (sender, args) => raised = true;

			mock.Object.Added += (sender, args) =>
			{
				Assert.True(args is FooArgs);
				Assert.Equal("foo", ((FooArgs)args).Args[0]);
				Assert.Equal(5, ((FooArgs)args).Args[1]);
				Assert.Equal(true, ((FooArgs)args).Args[2]);
			};

			mock.Object.Do("foo", 5, true);

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithFuncFourArgs()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Raises(handler, (string s, int i, bool b, string v) => new FooArgs { Args = new object[] { s, i, b, v } });

			mock.Object.Added += handler;

			var raised = false;
			handler.Raised += (sender, args) => raised = true;

			mock.Object.Added += (sender, args) =>
			{
				Assert.True(args is FooArgs);
				Assert.Equal("foo", ((FooArgs)args).Args[0]);
				Assert.Equal(5, ((FooArgs)args).Args[1]);
				Assert.Equal(true, ((FooArgs)args).Args[2]);
				Assert.Equal("bar", ((FooArgs)args).Args[3]);
			};

			mock.Object.Do("foo", 5, true, "bar");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldAttachToInheritedEvent()
		{
			var bar = new Mock<IDerived>(MockBehavior.Strict);
			bar.Object.Event += (o, e) => { ;}; // Exception Fired here
		}

		[Fact]
		public void ShouldThrowIfRaisingDettachedEvent()
		{
			var mock = new Mock<IFooView>();
			var handler = mock.CreateEventHandler();

			mock.Object.Canceled += handler;

			handler.Raise(EventArgs.Empty);

			mock.Object.Canceled -= handler;

			Assert.Throws<InvalidOperationException>(() => handler.Raise(EventArgs.Empty));
		}

		[Fact]
		public void ShouldAttachAndDetachListener()
		{
			var parent = new Mock<IParent>(MockBehavior.Strict);
			var raised = false;
			EventHandler<EventArgs> listener = (sender, args) => raised = true;

			var handler = parent.CreateEventHandler();
			parent.Object.Event += handler;

			parent.Object.Event += listener;

			handler.Raise(EventArgs.Empty);

			Assert.True(raised);

			raised = false;

			parent.Object.Event -= listener;

			handler.Raise(EventArgs.Empty);

			Assert.False(raised);
		}

		bool raisedField = false;

		[Fact]
		public void ShouldAttachAndDetachListenerMethod()
		{
			var parent = new Mock<IParent>(MockBehavior.Strict);
			raisedField = false;

			var handler = parent.CreateEventHandler();
			parent.Object.Event += handler;

			parent.Object.Event += OnRaised;

			handler.Raise(EventArgs.Empty);

			Assert.True(raisedField);

			raisedField = false;

			parent.Object.Event -= OnRaised;

			handler.Raise(EventArgs.Empty);

			Assert.False(raisedField);
		}

		[Fact]
		public void RaisesEvent()
		{
			var mock = new Mock<IAdder<string>>();

			bool raised = false;
			mock.Object.Added += (sender, args) => raised = true;

			mock.Raise(a => a.Added -= null, EventArgs.Empty);

			Assert.True(raised);
		}

		[Fact]
		public void RaisesPropertyChanged()
		{
			var mock = new Mock<IParent>();

			var prop = "";
			mock.Object.PropertyChanged += (sender, args) => prop = args.PropertyName;

			mock.Raise(x => x.PropertyChanged -= null, new PropertyChangedEventArgs("foo"));

			Assert.Equal("foo", prop);
		}

		[Fact]
		public void FailsIfArgumentException()
		{
			var mock = new Mock<IParent>();

			var prop = "";
			mock.Object.PropertyChanged += (sender, args) => prop = args.PropertyName;

			Assert.Throws<ArgumentException>(() => mock.Raise(x => x.PropertyChanged -= null, EventArgs.Empty));
		}

		[Fact]
		public void RaisesEventOnSubObject()
		{
			var mock = new Mock<IParent> { DefaultValue = DefaultValue.Mock };

			bool raised = false;
			mock.Object.Adder.Added += (sender, args) => raised = true;

			Assert.Same(mock.Object.Adder, mock.Object.Adder);

			mock.Raise(p => p.Adder.Added += null, EventArgs.Empty);

			Assert.True(raised);

		}

		[Fact(Skip="Events on non-virtual events not supported yet")]
		public void EventRaisingFailsOnNonVirtualEvent()
		{
			var mock = new Mock<WithEvent>();

			var raised = false;
			mock.Object.Event += delegate { raised = true; };

			// TODO: fix!!! We should go the GetInvocationList route here...
			mock.Raise(x => x.Event += null, EventArgs.Empty);

			Assert.True(raised);
		}

		[Fact]
		public void EventRaisingSucceedsOnVirtualEvent()
		{
			var mock = new Mock<WithEvent>();

			var raised = false;
			mock.Object.VirtualEvent += delegate { raised = true; };

			// TODO: fix!!! We should go the GetInvocationList route here...
			mock.Raise(x => x.VirtualEvent += null, EventArgs.Empty);

			Assert.True(raised);
		}

		public class WithEvent
		{
			public event EventHandler Event;
			public virtual event EventHandler VirtualEvent;
			public object Value { get; set; }
		}

		private void OnRaised(object sender, EventArgs e)
		{
			raisedField = true;
		}

		public interface IAdder<T>
		{
			event EventHandler Added;
			void Add(T value);
			int Insert(T value, int index);
			void Do(string s, int i);
			void Do(string s, int i, bool b);
			void Do(string s, int i, bool b, string v);
		}

		public class FooPresenter
		{
			public event EventHandler Fired;

			public FooPresenter(IFooView view)
			{
				view.FooSelected += (sender, args) => Fired(sender, args);
				view.Canceled += delegate { Canceled = true; };
			}

			public bool Canceled { get; set; }
		}

		public class FooArgs : EventArgs
		{
			public object Value { get; set; }
			public object[] Args { get; set; }
		}

		public interface IFooView
		{
			event EventHandler<FooArgs> FooSelected;
			event EventHandler Canceled;
		}

		public interface IParent : INotifyPropertyChanged
		{
			event EventHandler<EventArgs> Event;
			event PropertyChangedEventHandler PropertyChanged;
			IAdder<int> Adder { get; set; }
		}

		public interface IDerived : IParent { }
	}
}
