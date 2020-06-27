// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class AsInterfaceFixture
	{
		[Fact]
		public void ShouldThrowIfAsIsInvokedAfterInstanceIsRetrieved()
		{
			var mock = new Mock<IBag>();

			var instance = mock.Object;

			Assert.Throws<InvalidOperationException>(() => mock.As<IFoo>());
		}

		[Fact]
		public void ShouldThrowIfAsIsInvokedWithANonInterfaceTypeParameter()
		{
			var mock = new Mock<IBag>();

			Assert.Throws<ArgumentException>(() => mock.As<object>());
		}

		[Fact]
		public void ShouldExpectGetOnANewInterface()
		{
			var mock = new Mock<IBag>();

			bool called = false;

			mock.As<IFoo>().SetupGet(x => x.Value)
				.Callback(() => called = true)
				.Returns(25);

			Assert.Equal(25, ((IFoo)mock.Object).Value);
			Assert.True(called);
		}

		[Fact]
		public void ShouldExpectCallWithArgumentOnNewInterface()
		{
			var mock = new Mock<IBag>();
			mock.As<IFoo>().Setup(x => x.Execute("ping")).Returns("ack");

			Assert.Equal("ack", ((IFoo)mock.Object).Execute("ping"));
		}

		[Fact]
		public void ShouldExpectPropertySetterOnNewInterface()
		{
			bool called = false;
			int value = 0;
			var mock = new Mock<IBag>();
			mock.As<IFoo>().SetupSet(x => x.Value = 100).Callback<int>(i => { value = i; called = true; });

			((IFoo)mock.Object).Value = 100;

			Assert.Equal(100, value);
			Assert.True(called);
		}

		[Fact]
		public void MockAsExistingInterfaceAfterObjectSucceedsIfNotNew()
		{
			var mock = new Mock<IBag>();

			mock.As<IFoo>().SetupGet(x => x.Value).Returns(25);

			Assert.Equal(25, ((IFoo)mock.Object).Value);

			var fm = mock.As<IFoo>();

			fm.Setup(f => f.Execute());
		}

		[Fact]
		public void ThrowsWithMockedTypeName()
		{
			var bag = new Mock<IBag>();
			var foo = bag.As<IFoo>();

			bag.Setup(b => b.Add("foo", "bar")).Verifiable();
			foo.Setup(f => f.Execute()).Verifiable();

			var me = Assert.Throws<MockException>(() => bag.Verify());
			Assert.True(me.IsVerificationError);
			Assert.Contains(typeof(IFoo).Name, me.Message);
			Assert.Contains(typeof(IBag).Name, me.Message);
		}

		[Fact]
		public void GetMockFromAddedInterfaceWorks()
		{
			var bag = new Mock<IBag>();
			var foo = bag.As<IFoo>();

			foo.SetupGet(x => x.Value).Returns(25);

			IFoo f = bag.Object as IFoo;

			var foomock = Mock.Get(f);

			Assert.NotNull(foomock);
		}

		[Fact]
		public void GetMockFromNonAddedInterfaceThrows()
		{
			var bag = new Mock<IBag>();
			bag.As<IFoo>();
			bag.As<IComparable>();
			object b = bag.Object;

			Assert.Throws<ArgumentException>(() => Mock.Get(b));
		}

		[Fact]
		public void VerifiesExpectationOnAddedInterface()
		{
			var bag = new Mock<IBag>();
			var foo = bag.As<IFoo>();

			foo.Setup(f => f.Execute()).Verifiable();

			var ex = Assert.Throws<MockException>(() => foo.Verify());
			Assert.True(ex.IsVerificationError);

			ex = Assert.Throws<MockException>(() => foo.VerifyAll());
			Assert.True(ex.IsVerificationError);

			Assert.Throws<MockException>(() => foo.Verify(f => f.Execute()));

			foo.Object.Execute();

			foo.Verify();
			foo.VerifyAll();
			foo.Verify(f => f.Execute());
		}

		[Fact]
		public void VerifiesExpectationOnAddedInterfaceCastedDynamically()
		{
			var bag = new Mock<IBag>();
			bag.As<IFoo>();

			((IFoo)bag.Object).Execute();

			bag.As<IFoo>().Verify(f => f.Execute());
		}

		[Fact]
		public void ShouldBeAbleToCastToImplementedInterface()
		{
			var fooBar = new Mock<FooBar>();
			var obj = fooBar.Object;
			fooBar.As<IFoo>();
		}

		[Fact]
		public void ShouldNotThrowIfCallExplicitlyImplementedInterfacesMethodWhenCallBaseIsTrue()
		{
			var fooBar = new Mock<FooBar>();
			fooBar.CallBase = true;
			var bag = (IBag)fooBar.Object;
			bag.Get("test");
		}

		[Fact]
		public void Setup_targets_method_implementing_interface_not_other_method_with_same_signature()
		{
			var mock = new Mock<Service>() { CallBase = true };
			mock.As<IService>().Setup(m => m.GetValue()).Returns(3);

			// The public method should have been left alone since it's not the one implementing `IService`:
			var valueOfOtherMethod = mock.Object.GetValue();
			Assert.Equal(1, valueOfOtherMethod);

			// The method implementing the interface method should have be mocked:
			var valueOfSetupMethod = ((IService)mock.Object).GetValue();
			Assert.Equal(3, valueOfSetupMethod);
		}

		[Fact]
		public void As_mocked_type_returns_original_mock()
		{
			Mock<A> mock = new Mock<A>();
			Assert.Same(mock, mock.As<A>());
		}

		[Fact]
		public void Can_roundtrip_to_original_interface_mock_via_Mock_Get_and_As_original_interface()
		{
			Mock<B> bMockOriginal = new Mock<B>();
			A a = bMockOriginal.Object;
			Mock<A> aMock = Mock.Get(a);
			Mock<B> bMockRoundtripped = aMock.As<B>();
			Assert.Same(bMockOriginal, bMockRoundtripped);
		}

		public interface A { }
		public interface B : A { }

		public class Service : IService
		{
			public virtual int GetValue() => 1;

			int IService.GetValue() => 2;
		}

		public interface IService
		{
			int GetValue();
		}

		// see also test fixture `Issue458` in `IssueReportsFixture`

		public interface IFoo
		{
			void Execute();
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);

			int Value { get; set; }
		}

		public interface IBag
		{
			void Add(string key, object o);
			object Get(string key);
		}

		internal interface IBar
		{
			void Test();
		}

		public abstract class FooBar : IFoo, IBag, IBar
		{
			public abstract void Execute();

			public abstract string Execute(string command);

			public abstract string Execute(string arg1, string arg2);

			public abstract string Execute(string arg1, string arg2, string arg3);

			public abstract string Execute(string arg1, string arg2, string arg3, string arg4);

			public abstract int Value { get; set; }

			void IBag.Add(string key, object o)
			{
			}

			object IBag.Get(string key)
			{
				return null;
			}

			public abstract void Test();
		}
	}
}
