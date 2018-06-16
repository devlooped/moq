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
				Assert.Equal(MockExceptionReason.UnmatchedSetups, mex.Reason);
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
				Assert.Equal(MockExceptionReason.UnmatchedSetups, mex.Reason);
				Expression<Action<IFoo>> doExpr = foo => foo.Do();
				Assert.DoesNotContain(doExpr.ToString(), mex.Message);
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
				Assert.Contains(fooExpect.ToString(), mex.Message);

				Expression<Action<IBar>> barExpect = b => b.Redo();
				Assert.Contains(barExpect.ToString(), mex.Message);
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

		[Fact]
		public void DefaultValue_can_be_set_to_Empty()
		{
			var mockRepository = new MockRepository(MockBehavior.Default);

			mockRepository.DefaultValue = DefaultValue.Empty;

			Assert.Equal(DefaultValue.Empty, mockRepository.DefaultValue);
		}

		[Fact]
		public void DefaultValue_can_be_set_to_Mock()
		{
			var mockRepository = new MockRepository(MockBehavior.Default);

			mockRepository.DefaultValue = DefaultValue.Mock;

			Assert.Equal(DefaultValue.Mock, mockRepository.DefaultValue);
		}

		[Theory]
		[InlineData(DefaultValue.Custom)]
		[InlineData((DefaultValue)(-1))]
		public void DefaultValue_cannot_be_set_to_anything_other_than_Empty_or_Mock(DefaultValue defaultValue)
		{
			var mockRepository = new MockRepository(MockBehavior.Default);

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				mockRepository.DefaultValue = defaultValue;
			});
		}

		[Fact]
		public void DefaultValueProvider_cannot_be_set_to_null()
		{
			var mockRepository = new MockRepository(MockBehavior.Default);

			Assert.Throws<ArgumentNullException>(() =>
			{
				mockRepository.DefaultValueProvider = null;
			});
		}

		[Fact]
		public void Repository_initially_uses_default_switches()
		{
			var repository = new MockRepository(MockBehavior.Default);

			Assert.Equal(Switches.Default, actual: repository.Switches);
		}

		[Fact]
		public void Mock_inherits_switches_from_repository()
		{
			const Switches expectedSwitches = Switches.CollectDiagnosticFileInfoForSetups;

			var repository = new MockRepository(MockBehavior.Default) { Switches = expectedSwitches };
			var mock = repository.Create<IFoo>();

			Assert.Equal(expectedSwitches, actual: mock.Switches);
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
