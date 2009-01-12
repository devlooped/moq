using System;
using System.Linq.Expressions;
using Xunit;

namespace Moq.Tests
{
	public class MockFactoryFixture
	{
		[Fact]
		public void ShouldCreateFactoryWithMockBehaviorAndVerificationBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			Assert.NotNull(factory);
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			var mock = factory.Create<IFormatProvider>();

			Assert.Equal(MockBehavior.Loose, mock.Behavior);
		}

		[Fact]
		public void ShouldCreateMockWithConstructorArgs()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			var mock = factory.Create<BaseClass>("foo");

			Assert.Equal("foo", mock.Object.Value);
		}

		[Fact]
		public void ShouldVerifyAll()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Default);
				var mock = factory.Create<ICloneable>();

				mock.Setup(cloneable => cloneable.Clone());

				factory.VerifyAll();
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ShouldVerifyVerifiables()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Default);
				var mock = factory.Create<IFoo>();

				mock.Setup(foo => foo.Do());
				mock.Setup(foo => foo.Undo()).Verifiable();

				factory.Verify();
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Expression<Action<IFoo>> doExpr = foo => foo.Do();
				Assert.False(mex.Message.Contains(doExpr.ToString()));
			}
		}

		[Fact]
		public void ShouldAggregateFailures()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Loose);
				var foo = factory.Create<IFoo>();
				var bar = factory.Create<IBar>();

				foo.Setup(f => f.Do());
				bar.Setup(b => b.Redo());

				factory.VerifyAll();
			}
			catch (MockException mex)
			{
				Expression<Action<IFoo>> fooExpect = f => f.Do();
				Assert.True(mex.Message.Contains(fooExpect.ToString()));

				Expression<Action<IBar>> barExpect = b => b.Redo();
				Assert.True(mex.Message.Contains(barExpect.ToString()));
			}
		}

		[Fact]
		public void ShouldOverrideDefaultBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);
			var mock = factory.Create<IFoo>(MockBehavior.Strict);

			Assert.Equal(MockBehavior.Strict, mock.Behavior);
		}

		[Fact]
		public void ShouldOverrideDefaultBehaviorWithCtorArgs()
		{
			var factory = new MockFactory(MockBehavior.Loose);
			var mock = factory.Create<BaseClass>(MockBehavior.Strict, "Foo");

			Assert.Equal(MockBehavior.Strict, mock.Behavior);
			Assert.Equal("Foo", mock.Object.Value);
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryDefaultValue()
		{
			var factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			var mock = factory.Create<IFoo>();

			Assert.NotNull(mock.Object.Bar());
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryCallBase()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			var mock = factory.Create<BaseClass>();

			mock.Object.BaseMethod();

			Assert.False(mock.Object.BaseCalled);

			factory.CallBase = true;

			mock = factory.Create<BaseClass>();

			mock.Object.BaseMethod();

			Assert.True(mock.Object.BaseCalled);
		}

		public interface IFoo
		{
			void Do();
			void Undo();
			IBar Bar();
		}

		public interface IBar { void Redo(); }

		public abstract class BaseClass
		{
			public bool BaseCalled;

			public BaseClass()
			{
			}

			public BaseClass(string value)
			{
				this.Value = value;
			}

			public string Value { get; set; }

			public virtual void BaseMethod()
			{
				BaseCalled = true;
			}
		}
	}
}
