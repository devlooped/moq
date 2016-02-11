namespace Moq.Tests.Linq
{
    using System;

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
        public void WithQuery_Strict()
        {
            var target = Mock.Of<IFoo>(x => x.BoolProperty1 == true, MockBehavior.Strict);
            Assert.True(target.BoolProperty1);
            Assert.Equal(MockBehavior.Strict, Mock.Get(target).Behavior);
            var mex = Assert.Throws<MockException>(() => target.BoolProperty2);
            Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);
        }

        [Fact]
        public void WithQuery_Loose()
        {
            var target = Mock.Of<IFoo>(x => x.BoolProperty1 == true, MockBehavior.Loose);
            Assert.True(target.BoolProperty1);
            Assert.Equal(MockBehavior.Loose, Mock.Get(target).Behavior);
            Assert.False(target.BoolProperty2);
        }

        [Fact]
        public void ShouldAllowFluentOnNonVirtualReadWriteProperty_Strict()
        {
            var dto = Mock.Of<QueryableMocksFixture.Dto>(x => x.Value == "foo", MockBehavior.Strict);
            Assert.Equal("foo", dto.Value);
        }

        [Fact]
        public void WithQuery_ThrowsWhenStrict()
        {
            var target = Mock.Of<IFoo>(x => x.BoolProperty1 == true, MockBehavior.Strict);
            Assert.True(target.BoolProperty1);

            var mex = Assert.Throws<MockException>(() => target.BoolProperty2);
            Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);

            mex = Assert.Throws<MockException>(() => target.BoolMethod());
            Assert.Equal(MockException.ExceptionReason.NoSetup, mex.Reason);
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
        public void WithQuery_DoesNotThrowWhenLoose()
        {
            var target = Mock.Of<IFoo>(x => x.BoolProperty1 == true, MockBehavior.Loose);
            Assert.True(target.BoolProperty1);

            Assert.False(target.BoolProperty2);

            Assert.False(target.BoolMethod());

            Assert.DoesNotThrow(() => target.VoidMethod());
        }

        [Fact]
        public void NoQuery_DoesNotThrowsWhenLoose()
        {
            var target = Mock.Of<IFoo>(MockBehavior.Loose);
            Assert.DoesNotThrow(() => { var temp = target.BoolProperty1; });
            Assert.DoesNotThrow(() => { var temp = target.BoolProperty2; });
            Assert.DoesNotThrow(() => target.BoolMethod());
            Assert.DoesNotThrow(() => target.VoidMethod());
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
