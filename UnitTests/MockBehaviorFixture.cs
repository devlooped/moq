using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;

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

		[Ignore("TODO: remove when Normal is deleted from API")]
		[Test]
		public void ShouldNotThrowIfNormalNoExpectationOnNonVirtual()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//int value = mock.Object.NonVirtualGet();
		}

		[Ignore("TODO: remove when Normal is deleted from API")]
		[Test]
		public void ShouldNotThrowIfNormalNoExpectationOnVirtual()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//int value = mock.Object.VirtualGet();
		}

		[Ignore("TODO: remove when Normal is deleted from API")]
		[Test]
		public void ShouldThrowIfNormalNoExpectationOnInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.Do();
			//   Assert.Fail("Should have thrown for interface call with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.AreEqual(MockException.ExceptionReason.InterfaceNoExpectation, mex.Reason);
			//}
		}

		[Ignore("TODO: remove when Normal is deleted from API")]
		[Test]
		public void ShouldThrowIfNormalNoExpectationOnAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.AbstractGet();
			//   Assert.Fail("Should have thrown for abstract call with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.AreEqual(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			//}
		}

		[Ignore("TODO: remove when Normal is deleted from API")]
		[Test]
		public void ShouldThrowIfNormalNoExpectationOnNonAbstractCallsAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.Get();
			//   Assert.Fail("Should have thrown for indirect call to abstract with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.AreEqual(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			//}
		}

		[Ignore("TODO: remove when Relaxed is deleted from API")]
		[Test]
		public void ShouldNotThrowIfRelaxedVoidInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			//mock.Object.Do();
		}

		[Ignore("TODO: remove when Relaxed is deleted from API")]
		[Test]
		public void ShouldNotThrowIfRelaxedVoidClass()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//mock.Object.DoNonVirtual();
			//mock.Object.DoVirtual();
		}

		[Ignore("TODO: remove when Relaxed is deleted from API")]
		[Test]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			//try
			//{
			//   mock.Object.Get();
			//   Assert.Fail("Should have thrown for interface with MockBehavior.Relaxed");
			//}
			//catch (MockException mex)
			//{
			//   Assert.AreEqual(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			//}
		}

		[Ignore("TODO: remove when Relaxed is deleted from API")]
		[Test]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//try
			//{
			//   mock.Object.AbstractGet();
			//   Assert.Fail("Should have thrown for abstract with MockBehavior.Relaxed");
			//}
			//catch (MockException mex)
			//{
			//   Assert.AreEqual(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			//}
		}

		[Ignore("TODO: remove when Relaxed is deleted from API")]
		[Test]
		public void ShouldNotThrowIfRelaxedNoExpectationOnImplementedClass()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//mock.Object.VirtualGet();
			//mock.Object.NonVirtualGet();
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

		[Test]
		public void ShouldReturnEmptyArrayOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNotNull(mock.Object.GetArray());
			Assert.AreEqual(0, mock.Object.GetArray().Length);
		}

		[Test]
		public void ShouldReturnEmptyArrayTwoDimensionsOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNotNull(mock.Object.GetArrayTwoDimensions());
			Assert.AreEqual(0, mock.Object.GetArrayTwoDimensions().Length);
		}

		[Test]
		public void ShouldReturnNullListOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNull(mock.Object.GetList());
		}

		[Test]
		public void ShouldReturnEmptyEnumerableStringOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNotNull(mock.Object.GetEnumerable());
			Assert.AreEqual(0, mock.Object.GetEnumerable().Count());
		}

		[Test]
		public void ShouldReturnEmptyEnumerableObjectsOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNotNull(mock.Object.GetEnumerableObjects());
			Assert.AreEqual(0, mock.Object.GetEnumerableObjects().Cast<object>().Count());
		}

		[Test]
		public void ShouldReturnDefaultGuidOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			Assert.AreEqual(default(Guid), mock.Object.GetGuid());
		}

		[Test]
		public void ShouldReturnNullStringOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.IsNull(mock.Object.DoReturnString());
		}

		[Test]
		public void ShouldReturnNullStringOnLooseWithExpect()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			mock.Expect(x => x.DoReturnString());

			Assert.IsNull(mock.Object.DoReturnString());
		}


		public interface IFoo
		{
			void Do();
			int Get();
			Guid GetGuid();
			object GetObject();
			string[] GetArray();
			string[][] GetArrayTwoDimensions();
			List<string> GetList();
			IEnumerable<string> GetEnumerable();
			IEnumerable GetEnumerableObjects();
			string DoReturnString();
		}

		public abstract class Foo : IFoo
		{
			public abstract void Do();
			public abstract object GetObject();
			public abstract string DoReturnString();

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

			public string[] GetArray()
			{
				return new string[0];
			}

			public string[][] GetArrayTwoDimensions()
			{
				return new string[0][];
			}

			public List<string> GetList()
			{
				return null;
			}

			public IEnumerable<string> GetEnumerable()
			{
				return new string[0];
			}

			public IEnumerable GetEnumerableObjects()
			{
				return new object[0];
			}


			public Guid GetGuid()
			{
				return default(Guid);
			}
		}
	}
}
