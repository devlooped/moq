using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Moq.Tests
{
	[TestFixture]
	public class MockFactoryFixture
	{
		[Test]
		public void ShouldCreateFactoryWithMockBehaviorAndVerificationBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			Assert.IsNotNull(factory);
		}

		[Test]
		public void ShouldCreateMocksWithFactoryBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			var mock = factory.Create<IFormatProvider>();

			Assert.AreEqual(MockBehavior.Loose, mock.Behavior);
		}

		[Test]
		public void ShouldCreateMockWithConstructorArgs()
		{
			var factory = new MockFactory(MockBehavior.Loose);

			var mock = factory.Create<BaseClass>("foo");

			Assert.AreEqual("foo", mock.Object.Value);
		}

		[Test]
		public void ShouldVerifyAll()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Default);
				var mock = factory.Create<ICloneable>();

				mock.Expect(cloneable => cloneable.Clone());

				factory.VerifyAll();
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Test]
		public void ShouldVerifyVerifiables()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Default);
				var mock = factory.Create<IFoo>();

				mock.Expect(foo => foo.Do());
				mock.Expect(foo => foo.Undo()).Verifiable();

				factory.Verify();
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Expression<Action<IFoo>> doExpr = foo => foo.Do();
				Assert.IsFalse(mex.Message.Contains(doExpr.ToString()));
			}
		}

		[Test]
		public void ShouldAggregateFailures()
		{
			try
			{
				var factory = new MockFactory(MockBehavior.Loose);
				var foo = factory.Create<IFoo>();
				var bar = factory.Create<IBar>();

				foo.Expect(f => f.Do());
				bar.Expect(b => b.Redo());

				factory.VerifyAll();
			}
			catch (MockException mex)
			{
				Expression<Action<IFoo>> fooExpect = f => f.Do();
				Assert.IsTrue(mex.Message.Contains(fooExpect.ToString()));

				Expression<Action<IBar>> barExpect = b => b.Redo();
				Assert.IsTrue(mex.Message.Contains(barExpect.ToString()));
			}
		}

		[Test]
		public void ShouldOverrideDefaultBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose);
			var mock = factory.Create<IFoo>(MockBehavior.Strict);

			Assert.AreEqual(MockBehavior.Strict, mock.Behavior);
		}

		[Test]
		public void ShouldOverrideDefaultBehaviorWithCtorArgs()
		{
			var factory = new MockFactory(MockBehavior.Loose);
			var mock = factory.Create<BaseClass>(MockBehavior.Strict, "Foo");

			Assert.AreEqual(MockBehavior.Strict, mock.Behavior);
			Assert.AreEqual("Foo", mock.Object.Value);
		}

		public interface IFoo
		{
			void Do();
			void Undo();
		}

		public interface IBar { void Redo(); }

		public abstract class BaseClass
		{
			public BaseClass(string value)
			{
				this.Value = value;
			}

			public string Value { get; set; }
		}
	}
}
