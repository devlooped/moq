using System;
using NUnit.Framework;

namespace Moq.Tests
{
	[TestFixture]
	public class MockedEventsFixture
	{
		[Test]
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
					Assert.IsTrue(args is FooArgs);
					Assert.AreEqual("foo", ((FooArgs)args).Value);
				};

			handler.Raise(new FooArgs { Value = "foo" });

			Assert.IsTrue(fired);
		}

		[Test]
		public void ShouldRaiseEventIfAttachedAfterUse()
		{
			var view = new Mock<IFooView>();
			var presenter = new FooPresenter(view.Object);

			Assert.IsFalse(presenter.Canceled);

			var handler = view.CreateEventHandler();
			view.Object.Canceled += handler;
			handler.Raise(EventArgs.Empty);

			Assert.IsTrue(presenter.Canceled);
		}

		[Test]
		public void ShouldExpectAddGenericHandler()
		{
			var view = new Mock<IFooView>();
			var handler = view.CreateEventHandler();
			view.Object.Canceled += handler;
			var presenter = new FooPresenter(view.Object);

			Assert.IsFalse(presenter.Canceled);

			handler.Raise(EventArgs.Empty);

			Assert.IsTrue(presenter.Canceled);
		}

		[Test]
		public void ShouldNotThrowIfEventIsNotMocked()
		{
			var view = new Mock<IFooView>();

			// Presenter class attaches to the event and nothing happens.
			var presenter = new FooPresenter(view.Object);
		}

		[Test]
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

			Assert.IsTrue(raised);
		}

		[Test]
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

			Assert.IsTrue(raised);
		}

		[Test]
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

			Assert.AreEqual(1, value);
			Assert.IsTrue(raised);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfRaisesNullMockedEvent()
		{
			var mock = new Mock<IAdder<string>>();

			mock.Expect(add => add.Add(It.IsAny<string>()))
				.Raises(null, EventArgs.Empty);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfRaisesNullArgs()
		{
			var mock = new Mock<IAdder<string>>();

			mock.Expect(add => add.Add(It.IsAny<string>()))
				.Raises(mock.CreateEventHandler(), (EventArgs)null);
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldPreserveStackTraceWhenRaisingEvent()
		{
			var mock = new Mock<IAdder<string>>();
			var handler = mock.CreateEventHandler();

			mock.Object.Added += handler;
			mock.Object.Added += delegate { throw new InvalidOperationException(); };

			handler.Raise(EventArgs.Empty);
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldThrowIfRaisingNonAttachedEvent()
		{
			var mock = new Mock<IFooView>();
			var handler = mock.CreateEventHandler();

			handler.Raise(EventArgs.Empty);
		}

		[Test]
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

			Assert.IsTrue(raised);
		}

		[Test]
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
				Assert.That(args is FooArgs);
				Assert.AreEqual("foo", ((FooArgs)args).Value);
			};

			mock.Object.Add("foo");

			Assert.IsTrue(raised);
		}

		[Test]
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
				Assert.That(args is FooArgs);
				Assert.AreEqual("foo", ((FooArgs)args).Args[0]);
				Assert.AreEqual(5, ((FooArgs)args).Args[1]);
			};

			mock.Object.Do("foo", 5);

			Assert.IsTrue(raised);
		}

		[Test]
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
				Assert.That(args is FooArgs);
				Assert.AreEqual("foo", ((FooArgs)args).Args[0]);
				Assert.AreEqual(5, ((FooArgs)args).Args[1]);
				Assert.AreEqual(true, ((FooArgs)args).Args[2]);
			};

			mock.Object.Do("foo", 5, true);

			Assert.IsTrue(raised);
		}

		[Test]
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
				Assert.That(args is FooArgs);
				Assert.AreEqual("foo", ((FooArgs)args).Args[0]);
				Assert.AreEqual(5, ((FooArgs)args).Args[1]);
				Assert.AreEqual(true, ((FooArgs)args).Args[2]);
				Assert.AreEqual("bar", ((FooArgs)args).Args[3]);
			};

			mock.Object.Do("foo", 5, true, "bar");

			Assert.IsTrue(raised);
		}

		[Test]
		public void ShouldRaiseEventWithFuncArgs()
		{
			var mock = new Mock<IAdder<string>>();

			var handler = mock.CreateEventHandler();

			mock.Expect(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Raises(handler, (object[] args) => new FooArgs { Args = args });

			mock.Object.Added += handler;

			var raised = false;
			handler.Raised += (sender, args) => raised = true;

			mock.Object.Added += (sender, args) =>
			{
				Assert.That(args is FooArgs);
				Assert.AreEqual("foo", ((FooArgs)args).Args[0]);
				Assert.AreEqual(5, ((FooArgs)args).Args[1]);
				Assert.AreEqual(true, ((FooArgs)args).Args[2]);
				Assert.AreEqual("bar", ((FooArgs)args).Args[3]);
			};

			mock.Object.Do("foo", 5, true, "bar");

			Assert.IsTrue(raised);
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
	}
}
