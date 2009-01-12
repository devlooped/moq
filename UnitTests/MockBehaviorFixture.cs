using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
				Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);
			}
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

			mock.Setup(x => x.DoReturnString());

			Assert.Null(mock.Object.DoReturnString());
		}

		[Fact]
		public void ReturnsMockDefaultValueForLooseBehaviorOnInterface()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			var value = mock.Object.GetObject();

			Assert.True(value is IMocked);
		}

		[Fact]
		public void ReturnsMockDefaultValueForLooseBehaviorOnAbstract()
		{
			var mock = new Mock<Foo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			var value = mock.Object.Bar;

			Assert.True(value is IMocked);

			value = mock.Object.GetBar();

			Assert.True(value is IMocked);
		}

		[Fact]
		public void ReturnsEmptyArrayOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetArray());
			Assert.Equal(0, mock.Object.GetArray().Length);
		}

		[Fact]
		public void ReturnsEmptyArrayTwoDimensionsOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetArrayTwoDimensions());
			Assert.Equal(0, mock.Object.GetArrayTwoDimensions().Length);
		}

		[Fact]
		public void ReturnsMockListOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetList());

			var list = mock.Object.GetList();

			list.Add("foo");

			Assert.Equal("foo", list[0]);
		}

		[Fact]
		public void ReturnsEmptyEnumerableStringOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetEnumerable());
			Assert.Equal(0, mock.Object.GetEnumerable().Count());
		}

		[Fact]
		public void ReturnsEmptyQueryableStringOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetQueryable());
			Assert.Equal(0, mock.Object.GetQueryable().Count());
		}

		[Fact]
		public void ReturnsEmptyEnumerableObjectsOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetEnumerableObjects());
			Assert.Equal(0, mock.Object.GetEnumerableObjects().Cast<object>().Count());
		}

		[Fact]
		public void ReturnsEmptyQueryableObjectsOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

			Assert.NotNull(mock.Object.GetQueryableObjects());
			Assert.Equal(0, mock.Object.GetQueryableObjects().Cast<object>().Count());
		}

		[Fact]
		public void ReturnsDefaultGuidOnLooseWithMockDefaultValueWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
			Assert.Equal(default(Guid), mock.Object.GetGuid());
		}

		[Fact]
		public void ReturnsNullStringOnLooseWithMockDefaultValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

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
			IQueryable<string> GetQueryable();
			IQueryable GetQueryableObjects();
		}

		public interface IBar { }

		public abstract class Foo : IFoo
		{
			public abstract IBar Bar { get; set; }
			public abstract IBar GetBar();

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

			public IQueryable<string> GetQueryable()
			{
				return new string[0].AsQueryable();
			}

			public IQueryable GetQueryableObjects()
			{
				return new object[0].AsQueryable();
			}
		}
	}
}
