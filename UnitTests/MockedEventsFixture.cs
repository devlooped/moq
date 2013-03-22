using System;
using System.ComponentModel;
using System.Threading;
using Xunit;

namespace Moq.Tests
{
	public class MockedEventsFixture
	{
		[Fact]
		public void ShouldExpectAddHandler()
		{
			var view = new Mock<IFooView>();

			var presenter = new FooPresenter(view.Object);
			bool fired = false;

			presenter.Fired += (sender, args) =>
			{
				fired = true;
				Assert.True(args is FooArgs);
				Assert.Equal("foo", ((FooArgs)args).Value);
			};

			view.Raise(v => v.FooSelected += null, new FooArgs { Value = "foo" });

			Assert.True(fired);
		}

		[Fact]
		public void ShouldRaiseEventIfAttachedAfterUse()
		{
			var view = new Mock<IFooView>();
			var presenter = new FooPresenter(view.Object);

			Assert.False(presenter.Canceled);

			view.Raise(v => v.Canceled += null, EventArgs.Empty);

			Assert.True(presenter.Canceled);
		}

		[Fact]
		public void ShouldExpectAddGenericHandler()
		{
			var view = new Mock<IFooView>();
			var presenter = new FooPresenter(view.Object);

			Assert.False(presenter.Canceled);

			view.Raise(v => v.Canceled += null, EventArgs.Empty);

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

			var raised = false;
			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(m => m.Added += null, EventArgs.Empty);

			mock.Object.Added += (s, e) => raised = true;

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithArgWhenExpectationMet()
		{
			var mock = new Mock<IAdder<string>>();

			var raised = false;
			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(m => m.Added += null, EventArgs.Empty);

			mock.Object.Added += (s, e) => raised = true;

			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWhenExpectationMetReturn()
		{
			var mock = new Mock<IAdder<string>>();

			var raised = false;
			mock.Object.Added += (s, e) => raised = true;

			mock.Setup(add => add.Insert(It.IsAny<string>(), 0))
				.Returns(1)
				.Raises(m => m.Added += null, EventArgs.Empty);

			int value = mock.Object.Insert("foo", 0);

			Assert.Equal(1, value);
			Assert.True(raised);
		}

		[Fact]
		public void ShouldPreserveStackTraceWhenRaisingEvent()
		{
			var mock = new Mock<IAdder<string>>();
			mock.Object.Added += (s, e) => { throw new InvalidOperationException(); };

			Assert.Throws<InvalidOperationException>(() => mock.Raise(m => m.Added += null, EventArgs.Empty));
		}

		[Fact]
		public void ShouldRaiseEventWithFunc()
		{
			var mock = new Mock<IAdder<string>>();

			var raised = false;
			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(m => m.Added += null, () => EventArgs.Empty);

			mock.Object.Added += (s, e) => raised = true;
			mock.Object.Add("foo");

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventWithFuncOneArg()
		{
			var mock = new Mock<IAdder<string>>();

			mock.Setup(add => add.Add(It.IsAny<string>()))
				.Raises(m => m.Added += null, (string s) => new FooArgs { Value = s });

			var raised = false;

			mock.Object.Added += (sender, args) =>
			{
				raised = true;
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

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>()))
				.Raises(m => m.Added += null, (string s, int i) => new FooArgs { Args = new object[] { s, i } });

			var raised = false;

			mock.Object.Added += (sender, args) =>
			{
				raised = true;
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

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
				.Raises(m => m.Added += null, (string s, int i, bool b) => new FooArgs { Args = new object[] { s, i, b } });

			var raised = false;

			mock.Object.Added += (sender, args) =>
			{
				raised = true;
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

			mock.Setup(add => add.Do(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Raises(m => m.Added += null, (string s, int i, bool b, string v) => new FooArgs { Args = new object[] { s, i, b, v } });

			var raised = false;

			mock.Object.Added += (sender, args) =>
			{
				raised = true;
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
		public void ShouldAttachAndDetachListener()
		{
			var parent = new Mock<IParent>(MockBehavior.Strict);
			var raised = false;
			EventHandler<EventArgs> listener = (sender, args) => raised = true;

			parent.Object.Event += listener;

			parent.Raise(p => p.Event += null, EventArgs.Empty);

			Assert.True(raised);

			raised = false;

			parent.Object.Event -= listener;

			parent.Raise(p => p.Event += null, EventArgs.Empty);

			Assert.False(raised);
		}

		bool raisedField = false;

		[Fact]
		public void ShouldAttachAndDetachListenerMethod()
		{
			var parent = new Mock<IParent>(MockBehavior.Strict);
			raisedField = false;

			parent.Object.Event += this.OnRaised;

			parent.Raise(p => p.Event += null, EventArgs.Empty);

			Assert.True(raisedField);

			raisedField = false;

			parent.Object.Event -= OnRaised;

			parent.Raise(p => p.Event += null, EventArgs.Empty);

			Assert.False(raisedField);
		}

		[Fact]
		public void ShouldAllowListenerListToBeModifiedDuringEventHandling()
		{
			var parent = new Mock<IParent>(MockBehavior.Strict);

			parent.Object.Event += delegate
			{
				parent.Object.Event += delegate { raisedField = true; };
			};

			parent.Raise(p => p.Event += null, EventArgs.Empty);

			// we don't expect the inner event to be raised the first time
			Assert.False(raisedField);

			// the second time around, the event handler added the first time
			// should kick in
			parent.Raise(p => p.Event += null, EventArgs.Empty);

			Assert.True(raisedField);
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

			mock.Raise(x => x.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>(), new PropertyChangedEventArgs("foo"));

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
		public void DoesNotRaiseEventOnSubObject()
		{
			var mock = new Mock<IParent> { DefaultValue = DefaultValue.Mock };

			bool raised = false;
			mock.Object.Adder.Added += (sender, args) => raised = true;

			Assert.Same(mock.Object.Adder, mock.Object.Adder);

			mock.Raise(p => p.Adder.Added += null, EventArgs.Empty);

			Assert.False(raised);
		}

		[Fact]
		public void RaisesEventWithActionLambda()
		{
			var mock = new Mock<IWithEvent>();

			mock.SetupSet(m => m.Value = It.IsAny<int>()).Raises(m => m.InterfaceEvent += null, EventArgs.Empty);

			var raised = false;
			mock.Object.InterfaceEvent += (sender, args) => raised = true;

			mock.Object.Value = 5;

			Assert.True(raised);
		}

		[Fact]
		public void RaisesEventWithActionLambdaOnClass()
		{
			var mock = new Mock<WithEvent>();

			mock.SetupSet(m => m.Value = It.IsAny<int>()).Raises(m => m.VirtualEvent += null, EventArgs.Empty);

			var raised = false;
			mock.Object.VirtualEvent += (sender, args) => raised = true;

			mock.Object.Value = 5;

			Assert.True(raised);
		}

		[Fact]
		public void RaisesThrowsIfEventNonVirtual()
		{
			var mock = new Mock<WithEvent>();

			Assert.Throws<ArgumentException>(
				() => mock.SetupSet(m => m.Value = It.IsAny<int>()).Raises(m => m.ClassEvent += null, EventArgs.Empty));
		}

		[Fact(Skip = "Events on non-virtual events not supported yet")]
		public void EventRaisingFailsOnNonVirtualEvent()
		{
			var mock = new Mock<WithEvent>();

			var raised = false;
			mock.Object.ClassEvent += delegate { raised = true; };

			// TODO: fix!!! We should go the GetInvocationList route here...
			mock.Raise(x => x.ClassEvent += null, EventArgs.Empty);

			Assert.True(raised);
		}

		[Fact]
		public void EventRaisingSucceedsOnVirtualEvent()
		{
			var mock = new Mock<WithEvent>();

			var raised = false;
			mock.Object.VirtualEvent += (s, e) => raised = true;

			// TODO: fix!!! We should go the GetInvocationList route here...
			mock.Raise(x => x.VirtualEvent += null, EventArgs.Empty);

			Assert.True(raised);
		}

		[Fact]
		public void RaisesEventWithActionLambdaOneArg()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo")).Raises<string>(m => m.Done += null, s => new DoneArgs { Value = s });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo");

			Assert.NotNull(args);
			Assert.Equal("foo", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaTwoArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5))
				.Raises(m => m.Done += null, (string s, int i) => new DoneArgs { Value = s + i });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5);

			Assert.NotNull(args);
			Assert.Equal("foo5", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaThreeArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true))
				.Raises(m => m.Done += null, (string s, int i, bool b) => new DoneArgs { Value = s + i + b });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true);

			Assert.NotNull(args);
			Assert.Equal("foo5True", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaFourArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true, "bar"))
				.Raises(m => m.Done += null, (string s, int i, bool b, string s1) => new DoneArgs { Value = s + i + b + s1 });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true, "bar");

			Assert.NotNull(args);
			Assert.Equal("foo5Truebar", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaFiveArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true, "bar", 5))
				.Raises(m => m.Done += null, (string s, int i, bool b, string s1, int arg5) => new DoneArgs { Value = s + i + b + s1 + arg5 });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true, "bar", 5);

			Assert.NotNull(args);
			Assert.Equal("foo5Truebar5", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaSixArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true, "bar", 5, 6))
				.Raises(m => m.Done += null, (string s, int i, bool b, string s1, int arg5, int arg6) => new DoneArgs { Value = s + i + b + s1 + arg5 + arg6 });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true, "bar", 5, 6);

			Assert.NotNull(args);
			Assert.Equal("foo5Truebar56", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaSevenArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true, "bar", 5, 6, 7))
				.Raises(m => m.Done += null, (string s, int i, bool b, string s1, int arg5, int arg6, int arg7) => new DoneArgs { Value = s + i + b + s1 + arg5 + arg6 + arg7 });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true, "bar", 5, 6, 7);

			Assert.NotNull(args);
			Assert.Equal("foo5Truebar567", args.Value);
		}

		[Fact]
		public void RaisesEventWithActionLambdaEightArgs()
		{
			var mock = new Mock<IAdder<int>>();

			mock.Setup(m => m.Do("foo", 5, true, "bar", 5, 6, 7, 8))
				.Raises(m => m.Done += null, (string s, int i, bool b, string s1, int arg5, int arg6, int arg7, int arg8) => new DoneArgs { Value = s + i + b + s1 + arg5 + arg6 + arg7 + arg8 });

			DoneArgs args = null;
			mock.Object.Done += (sender, e) => args = e;

			mock.Object.Do("foo", 5, true, "bar", 5, 6, 7, 8);

			Assert.NotNull(args);
			Assert.Equal("foo5Truebar5678", args.Value);
		}

		[Fact]
		public void RaisesCustomEventWithLambda()
		{
			var mock = new Mock<IWithEvent>();
			string message = null;
			int? value = null;

			mock.Object.CustomEvent += (s, i) => { message = s; value = i; };

			mock.Raise(x => x.CustomEvent += null, "foo", 5);

			Assert.Equal("foo", message);
			Assert.Equal(5, value);
		}

		[Fact]
		public void RaisesCustomEventWithLambdaOnPropertySet()
		{
			var mock = new Mock<IWithEvent>();
			string message = null;
			int? value = null;

			mock.Object.CustomEvent += (s, i) => { message = s; value = i; };
			mock.SetupSet(w => w.Value = 5).Raises(x => x.CustomEvent += null, "foo", 5);

			mock.Object.Value = 5;

			Assert.Equal("foo", message);
			Assert.Equal(5, value);
		}

		[Fact]
		public void ShouldSuccessfullyAttachToEventForwaredToAProtectedEvent()
		{
			var mock = new Mock<FordawrdEventDoProtectedImplementation>();
			INotifyPropertyChanged observable = mock.Object;
			
			Assert.DoesNotThrow(() => observable.PropertyChanged += (sender, args) => { });
		}

		public delegate void CustomEvent(string message, int value);

		public interface IWithEvent
		{
			event EventHandler InterfaceEvent;
			event CustomEvent CustomEvent;
			object Value { get; set; }
		}

		public class WithEvent : IWithEvent
		{
			public event EventHandler InterfaceEvent = (s, e) => { };
			public event EventHandler ClassEvent = (s, e) => { };
			public event CustomEvent CustomEvent = (s, e) => { };
			public virtual event EventHandler VirtualEvent = (s, e) => { };
			public virtual object Value { get; set; }
		}

		private void OnRaised(object sender, EventArgs e)
		{
			raisedField = true;
		}

		public class DoneArgs : EventArgs
		{
			public string Value { get; set; }
		}

		public interface IAdder<T>
		{
			event EventHandler<DoneArgs> Done;
			event EventHandler Added;
			void Add(T value);
			int Insert(T value, int index);
			void Do(string s);
			void Do(string s, int i);
			void Do(string s, int i, bool b);
			void Do(string s, int i, bool b, string v);
			void Do(string s, int i, bool b, string v, int arg5);
			void Do(string s, int i, bool b, string v, int arg5, int arg6);
			void Do(string s, int i, bool b, string v, int arg5, int arg6, int arg7);
			void Do(string s, int i, bool b, string v, int arg5, int arg6, int arg7, int arg8);
		}

		public class FooPresenter
		{
			public event EventHandler Fired;

			public FooPresenter(IFooView view)
			{
				view.FooSelected += (s, e) => Fired(s, e);
				view.Canceled += (s, e) => Canceled = true;
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
			IAdder<int> Adder { get; set; }
		}

		public interface IDerived : IParent
		{
		}


		public interface IInterfaceWithEvent
		{
			event EventHandler<EventArgs> JustAnEvent;
		}

		public class FordawrdEventDoProtectedImplementation : INotifyPropertyChanged
		{
			private PropertyChangedEventHandler eventHandler;

			event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
			{
				add { this.PropertyChanged += value; }
				remove { this.PropertyChanged -= value; }
			}

			protected virtual event PropertyChangedEventHandler PropertyChanged
			{
				add
				{
					this.eventHandler += value;
				}
				remove
				{
					this.eventHandler -= value;
				}
			}
		}

	}
}
