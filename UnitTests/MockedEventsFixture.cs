using System;
using Xunit;

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

			mock.Expect(add => add.Add(It.IsAny<string>()))
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

			mock.Expect(add => add.Add(It.IsAny<string>()))
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

			mock.Expect(add => add.Insert(It.IsAny<string>(), 0))
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

			Assert.Throws<ArgumentNullException>(() => mock.Expect(add => add.Add(It.IsAny<string>()))
				.Raises(null, EventArgs.Empty));
		}

		[Fact]
		public void ShouldThrowIfRaisesNullArgs()
		{
			var mock = new Mock<IAdder<string>>();

			Assert.Throws<ArgumentNullException>(() => mock.Expect(add => add.Add(It.IsAny<string>()))
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

			mock.Expect(add => add.Add(It.IsAny<string>()))
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

			mock.Expect(add => add.Add(It.IsAny<string>()))
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

			mock.Expect(add => add.Do(It.IsAny<string>(), It.IsAny<int>()))
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

			mock.Expect(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
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

			mock.Expect(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
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

		public interface IParent
		{
			event EventHandler<EventArgs> Event;
		}

		public interface IDerived : IParent { }
	}
}
