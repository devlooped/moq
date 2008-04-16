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
				.Raises(mock.CreateEventHandler(), null);			
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

		public interface IAdder<T>
		{
			event EventHandler Added;
			void Add(T value);
			int Insert(T value, int index);
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
			public string Value { get; set; }
		}

		public interface IFooView
		{
			event EventHandler<FooArgs> FooSelected;
			event EventHandler Canceled;
		}
	}
}
