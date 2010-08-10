using System;
using System.Linq.Expressions;
using Xunit;

namespace Moq.Tests
{
	public class MockRepositoryFixture
	{
		[Fact]
		public void ShouldCreateFactoryWithMockBehaviorAndVerificationBehavior()
		{
			var repository = new MockRepository(MockBehavior.Loose);

			Assert.NotNull(repository);
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryBehavior()
		{
			var repository = new MockRepository(MockBehavior.Loose);

			var mock = repository.Create<IFormatProvider>();

			Assert.Equal(MockBehavior.Loose, mock.Behavior);
		}

		[Fact]
		public void ShouldCreateMockWithConstructorArgs()
		{
			var repository = new MockRepository(MockBehavior.Loose);

			var mock = repository.Create<BaseClass>("foo");

			Assert.Equal("foo", mock.Object.Value);
		}

		[Fact]
		public void ShouldVerifyAll()
		{
			try
			{
				var repository = new MockRepository(MockBehavior.Default);
				var mock = repository.Create<IFoo>();

				mock.Setup(foo => foo.Do());

				repository.VerifyAll();
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
				var repository = new MockRepository(MockBehavior.Default);
				var mock = repository.Create<IFoo>();

				mock.Setup(foo => foo.Do());
				mock.Setup(foo => foo.Undo()).Verifiable();

				repository.Verify();
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
				var repository = new MockRepository(MockBehavior.Loose);
				var foo = repository.Create<IFoo>();
				var bar = repository.Create<IBar>();

				foo.Setup(f => f.Do());
				bar.Setup(b => b.Redo());

				repository.VerifyAll();
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
			var repository = new MockRepository(MockBehavior.Loose);
			var mock = repository.Create<IFoo>(MockBehavior.Strict);

			Assert.Equal(MockBehavior.Strict, mock.Behavior);
		}

		[Fact]
		public void ShouldOverrideDefaultBehaviorWithCtorArgs()
		{
			var repository = new MockRepository(MockBehavior.Loose);
			var mock = repository.Create<BaseClass>(MockBehavior.Strict, "Foo");

			Assert.Equal(MockBehavior.Strict, mock.Behavior);
			Assert.Equal("Foo", mock.Object.Value);
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryDefaultValue()
		{
			var repository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			var mock = repository.Create<IFoo>();

			Assert.NotNull(mock.Object.Bar());
		}

		[Fact]
		public void ShouldCreateMocksWithFactoryCallBase()
		{
			var repository = new MockRepository(MockBehavior.Loose);

			var mock = repository.Create<BaseClass>();

			mock.Object.BaseMethod();

			Assert.False(mock.Object.BaseCalled);

			repository.CallBase = true;

			mock = repository.Create<BaseClass>();

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
