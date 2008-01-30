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
			var factory = new MockFactory(MockBehavior.Loose, MockVerification.All);

			Assert.IsNotNull(factory);
		}

		[Test]
		public void ShouldCreateMocksWithFactoryBehavior()
		{
			var factory = new MockFactory(MockBehavior.Loose, MockVerification.All);

			var mock = factory.Create<IFormatProvider>();

			Assert.AreEqual(MockBehavior.Loose, mock.Behavior);
		}

		[Test]
		public void ShouldCreateMockWithConstructorArgs()
		{
			var factory = new MockFactory(MockBehavior.Loose, MockVerification.All);

			var mock = factory.Create<BaseClass>("foo");

			Assert.AreEqual("foo", mock.Object.Value);
		}

		[Test]
		public void ShouldDisposeFactoryVerifyAll()
		{
			try
			{
				using (var factory = new MockFactory(MockBehavior.Default, MockVerification.All))
				{
					var mock = factory.Create<ICloneable>();

					mock.Expect(cloneable => cloneable.Clone());
				}

				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Test]
		public void ShouldDisposeFactoryVerifyVerifiables()
		{
			// Should not fail
			using (var factory = new MockFactory(MockBehavior.Default, MockVerification.Verifiable))
			{
				var mock = factory.Create<ICloneable>();
				// non verifiable
				mock.Expect(cloneable => cloneable.Clone());
			}

			try
			{
				using (var factory = new MockFactory(MockBehavior.Default, MockVerification.Verifiable))
				{
					var mock = factory.Create<IFoo>();

					mock.Expect(foo => foo.Do());
					mock.Expect(foo => foo.Undo()).Verifiable();
				}

				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Expression<Action<IFoo>> doExpr = foo => foo.Do();
				Assert.IsFalse(mex.Message.Contains(doExpr.ToString()));
			}
		}

		[Test]
		public void ShouldDisposeAggregateFailures()
		{
			try
			{
				using (var factory = new MockFactory(MockBehavior.Loose, MockVerification.All))
				{
					var foo = factory.Create<IFoo>();
					var bar = factory.Create<IBar>();

					foo.Expect(f => f.Do());
					bar.Expect(b => b.Redo());
				}
			}
			catch (MockException mex)
			{
				Expression<Action<IFoo>> fooExpect = f => f.Do();
				Assert.IsTrue(mex.Message.Contains(fooExpect.ToString()));

				Expression<Action<IBar>> barExpect = b => b.Redo();
				Assert.IsTrue(mex.Message.Contains(barExpect.ToString()));
			}
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
