using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Collections;

namespace Moq.Tests
{
	public class MockBehaviorFixture
	{
		[Fact]
		public void ShouldThrowIfStrictNoExpectation()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			try
			{
				mock.Object.Do();
				Assert.True(false, "Should have thrown for unexpected call with MockBehavior.Strict");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.NoExpectation, mex.Reason);
			}
		}

		[Fact(Skip="TODO: remove when Normal is deleted from API")]
		public void ShouldNotThrowIfNormalNoExpectationOnNonVirtual()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//int value = mock.Object.NonVirtualGet();
		}

		[Fact(Skip = "TODO: remove when Normal is deleted from API")]
		public void ShouldNotThrowIfNormalNoExpectationOnVirtual()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//int value = mock.Object.VirtualGet();
		}

		[Fact(Skip = "TODO: remove when Normal is deleted from API")]
		public void ShouldThrowIfNormalNoExpectationOnInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.Do();
			//   Assert.True(false, "Should have thrown for interface call with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.Equal(MockException.ExceptionReason.InterfaceNoExpectation, mex.Reason);
			//}
		}

		[Fact(Skip = "TODO: remove when Normal is deleted from API")]
		public void ShouldThrowIfNormalNoExpectationOnAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.AbstractGet();
			//   Assert.True(false, "Should have thrown for abstract call with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.Equal(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			//}
		}

		[Fact(Skip = "TODO: remove when Normal is deleted from API")]
		public void ShouldThrowIfNormalNoExpectationOnNonAbstractCallsAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Normal);
			//try
			//{
			//   mock.Object.Get();
			//   Assert.True(false, "Should have thrown for indirect call to abstract with MockBehavior.Normal");
			//}
			//catch (MockException mex)
			//{
			//   Assert.Equal(MockException.ExceptionReason.AbstractNoExpectation, mex.Reason);
			//}
		}

		[Fact(Skip="TODO: remove when Relaxed is deleted from API")]
		public void ShouldNotThrowIfRelaxedVoidInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			//mock.Object.Do();
		}

		[Fact(Skip = "TODO: remove when Relaxed is deleted from API")]
		public void ShouldNotThrowIfRelaxedVoidClass()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//mock.Object.DoNonVirtual();
			//mock.Object.DoVirtual();
		}

		[Fact(Skip = "TODO: remove when Relaxed is deleted from API")]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidInterface()
		{
			//var mock = new Mock<IFoo>(MockBehavior.Relaxed);
			//try
			//{
			//   mock.Object.Get();
			//   Assert.True(false, "Should have thrown for interface with MockBehavior.Relaxed");
			//}
			//catch (MockException mex)
			//{
			//   Assert.Equal(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			//}
		}

		[Fact(Skip = "TODO: remove when Relaxed is deleted from API")]
		public void ShouldThrowIfRelaxedNoExpectationOnNonVoidAbstract()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//try
			//{
			//   mock.Object.AbstractGet();
			//   Assert.True(false, "Should have thrown for abstract with MockBehavior.Relaxed");
			//}
			//catch (MockException mex)
			//{
			//   Assert.Equal(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			//}
		}

		[Fact(Skip = "TODO: remove when Relaxed is deleted from API")]
		public void ShouldNotThrowIfRelaxedNoExpectationOnImplementedClass()
		{
			//var mock = new Mock<Foo>(MockBehavior.Relaxed);
			//mock.Object.VirtualGet();
			//mock.Object.NonVirtualGet();
		}

		[Fact]
		public void ShouldReturnDefaultForLooseBehaviorOnInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.Equal(0, mock.Object.Get());
			Assert.Null(mock.Object.GetObject());
		}

		[Fact]
		public void ShouldReturnDefaultForLooseBehaviorOnAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Loose);

			Assert.Equal(0, mock.Object.AbstractGet());
			Assert.Null(mock.Object.GetObject());
		}

		[Fact]
		public void ShouldReturnEmptyArrayOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.NotNull(mock.Object.GetArray());
			Assert.Equal(0, mock.Object.GetArray().Length);
		}

		[Fact]
		public void ShouldReturnEmptyArrayTwoDimensionsOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.NotNull(mock.Object.GetArrayTwoDimensions());
			Assert.Equal(0, mock.Object.GetArrayTwoDimensions().Length);
		}

		[Fact]
		public void ShouldReturnNullListOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.Null(mock.Object.GetList());
		}

		[Fact]
		public void ShouldReturnEmptyEnumerableStringOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.NotNull(mock.Object.GetEnumerable());
			Assert.Equal(0, mock.Object.GetEnumerable().Count());
		}

		[Fact]
		public void ShouldReturnEmptyEnumerableObjectsOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.NotNull(mock.Object.GetEnumerableObjects());
			Assert.Equal(0, mock.Object.GetEnumerableObjects().Cast<object>().Count());
		}

		[Fact]
		public void ShouldReturnDefaultGuidOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			Assert.Equal(default(Guid), mock.Object.GetGuid());
		}

		[Fact]
		public void ShouldReturnNullStringOnLoose()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			Assert.Null(mock.Object.DoReturnString());
		}

		[Fact]
		public void ShouldReturnNullStringOnLooseWithExpect()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);

			mock.Expect(x => x.DoReturnString());

			Assert.Null(mock.Object.DoReturnString());
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
