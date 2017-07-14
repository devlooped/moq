namespace Moq.Tests.Linq
{
	using Xunit;

	public class BehaviorFixture
	{
		[Fact]
		public void NoQuery_Default()
		{
			var target = Mock.Of<IFoo>();
			Assert.Equal(MockBehavior.Default, Mock.Get(target).Behavior);
			Assert.False(target.BoolProperty1);
		}

		[Fact]
		public void WithQuery_Default()
		{
			var target = Mock.Of<IFoo>(x => x.BoolProperty1 == true);
			Assert.Equal(MockBehavior.Default, Mock.Get(target).Behavior);
			Assert.True(target.BoolProperty1);
		}

		[Fact]
		public void NoQuery_Strict()
		{
			var target = Mock.Of<IFoo>(MockBehavior.Strict);
			Assert.Equal(MockBehavior.Strict, Mock.Get(target).Behavior);
			var mex = Assert.Throws<MockException>(() => target.BoolProperty1);
			Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);
		}

		[Fact]
		public void NoQuery_Loose()
		{
			var target = Mock.Of<IFoo>(MockBehavior.Loose);
			Assert.Equal(MockBehavior.Loose, Mock.Get(target).Behavior);
			Assert.False(target.BoolProperty1);
		}

		[Fact]
		public void NoQuery_ThrowsWhenStrict()
		{
			var target = Mock.Of<IFoo>(MockBehavior.Strict);
			var mex = Assert.Throws<MockException>(() => target.BoolProperty1);
			Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);

			mex = Assert.Throws<MockException>(() => target.BoolMethod());
			Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);
		}

		[Fact]
		public void NoQuery_DoesNotThrowWhenLoose()
		{
			var target = Mock.Of<IFoo>(MockBehavior.Loose);
			Assert.Equal(false, target.BoolProperty1);
			Assert.Equal(false, target.BoolProperty2);
			Assert.Equal(false, target.BoolMethod());

			// Just checking that calling the void method does not throw.
			target.VoidMethod();
		}

		public interface IFoo
		{
			bool BoolProperty1 { get; set; }
			bool BoolProperty2 { get; set; }
			bool BoolMethod();
			void VoidMethod();
		}
	}
}
