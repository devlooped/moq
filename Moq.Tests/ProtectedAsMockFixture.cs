using Moq.Protected;
using System;
using Xunit;

namespace Moq.Tests
{
    public class ProtectedAsMockFixture
    {
		private Mock<Foo> mock;
		private IProtectedAsMock<Foo, Fooish> protectedMock;

		public ProtectedAsMockFixture()
		{
			this.mock = new Mock<Foo>();
			this.protectedMock = this.mock.Protected().As<Fooish>();
		}

		[Fact]
		public void Setup_throws_when_expression_null()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(null);
			});

			Assert.IsType<ArgumentNullException>(actual);
		}

		[Fact]
		public void Setup_throws_ArgumentException_when_expression_contains_nonexistent_method()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(m => m.NonExistentMethod());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Contains("does not have matching protected member", actual.Message);
		}

		[Fact]
		public void Setup_throws_ArgumentException_when_expression_contains_nonexistent_property()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(m => m.NonExistentProperty);
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Contains("does not have matching protected member", actual.Message);
		}

		[Fact]
		public void Setup_TResult_throws_when_expression_null()
		{
			var actual = Record.Exception(() => this.protectedMock.Setup<object>(null));

			Assert.IsType<ArgumentNullException>(actual);
		}

		[Fact]
		public void Setup_can_setup_simple_method()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl()).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething();

			Assert.True(doSomethingImplInvoked);
		}

		[Fact]
		public void Setup_TResult_can_setup_simple_method_with_return_value()
		{
			this.protectedMock.Setup(m => m.GetSomethingImpl()).Returns(() => 42);

			var actual = this.mock.Object.GetSomething();

			Assert.Equal(42, actual);
		}

		[Fact]
		public void Setup_can_match_exact_arguments()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl(1)).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething(0);
			Assert.False(doSomethingImplInvoked);

			this.mock.Object.DoSomething(1);
			Assert.True(doSomethingImplInvoked);
		}

		[Fact]
		public void Setup_can_involve_matchers()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl(It.Is<int>(i => i == 1))).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething(0);
			Assert.False(doSomethingImplInvoked);

			this.mock.Object.DoSomething(1);
			Assert.True(doSomethingImplInvoked);
		}

		public abstract class Foo
		{
			protected Foo()
			{
			}

			public void DoSomething()
			{
				this.DoSomethingImpl();
			}

			public void DoSomething(int arg)
			{
				this.DoSomethingImpl(arg);
			}

			public int GetSomething()
			{
				return this.GetSomethingImpl();
			}

			protected abstract void DoSomethingImpl();

			protected abstract void DoSomethingImpl(int arg);

			protected abstract int GetSomethingImpl();
		}

		public interface Fooish
		{
			int NonExistentProperty { get; }
			void DoSomethingImpl();
			void DoSomethingImpl(int arg);
			int GetSomethingImpl();
			void NonExistentMethod();
		}
    }
}
