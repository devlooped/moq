using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moq.Tests
{
	[TestFixture]
	public class MockBehaviorFixture
	{
		[Test]
		public void ShouldThrowIfStrictNoExpectation()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			try
			{
				mock.Object.Do();
				Assert.Fail("Should have thrown for unexpected call with MockBehavior.Strict");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.NoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldNotThrowIfNormalNoExpectationOnNonVirtual()
		{
			var mock = new Mock<Foo>(MockBehavior.Normal);
			int value = mock.Object.NonVirtualGet();
		}

		[Test]
		public void ShouldNotThrowIfNormalNoExpectationOnVirtual()
		{
			var mock = new Mock<Foo>(MockBehavior.Normal);
			int value = mock.Object.VirtualGet();
		}

		[Test]
		public void ShouldThrowIfNormalNoExpectationOnInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Normal);
			try
			{
				mock.Object.Do();
				Assert.Fail("Should have thrown for interface call with MockBehavior.Normal");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.InterfaceNoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldThrowIfNormalNoExpectationOnAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Normal);
			try
			{
				mock.Object.AbstractGet();
				Assert.Fail("Should have thrown for abstract call with MockBehavior.Normal");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldThrowIfNormalNoExpectationOnNonAbstractCallsAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Normal);
			try
			{
				mock.Object.Get();
				Assert.Fail("Should have thrown for indirect call to abstract with MockBehavior.Normal");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldNotThrowIfRelaxedVoidInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			mock.Object.Do();
		}

		[Test]
		public void ShouldNotThrowIfRelaxedVoidClass()
		{
			var mock = new Mock<Foo>(MockBehavior.Relaxed);
			mock.Object.DoNonVirtual();
			mock.Object.DoVirtual();
		}

		[Test]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			try
			{
				mock.Object.Get();
				Assert.Fail("Should have thrown for interface with MockBehavior.Relaxed");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.ReturnValueNoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Relaxed);
			try
			{
				mock.Object.AbstractGet();
				Assert.Fail("Should have thrown for abstract with MockBehavior.Relaxed");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.ReturnValueNoExpectation, mex.Reason);
			}
		}

		[Test]
		public void ShouldNotThrowIfRelaxedNoExpectationOnImplementedClass()
		{
			var mock = new Mock<Foo>(MockBehavior.Relaxed);
			mock.Object.VirtualGet();
			mock.Object.NonVirtualGet();
		}

		[Test]
		public void ShouldReturnDefaultForLooseBehaviorOnInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.AreEqual(0, mock.Object.Get());
			Assert.IsNull(mock.Object.GetObject());
		}

		[Test]
		public void ShouldReturnDefaultForLooseBehaviorOnAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Loose);

			Assert.AreEqual(0, mock.Object.AbstractGet());
			Assert.IsNull(mock.Object.GetObject());
		}

		public interface IFoo
		{
			void Do();
			int Get();
			object GetObject();
		}

		public abstract class Foo : IFoo
		{
			public abstract void Do();
			public abstract object GetObject();

			public void DoNonVirtual() { }
			public virtual void DoVirtual() { }

			public int NonVirtualGet()
			{
				return 0;
			}

			public int VirtualGet()
			{
				return 0;
			}

			public virtual int Get()
			{
				return AbstractGet();
			}

			public abstract int AbstractGet();
		}
	}
}
