using System;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class StrictMockBehaviorTests
    {
        [Fact]
        public void AppliesToAllInvocations()
            => Assert.True(new StrictMockBehavior().AppliesTo(new FakeInvocation()));

        [Fact]
        public void ThrowsStrictMockException()
            => Assert.Throws<StrictMockException>(() =>
                new StrictMockBehavior().Execute(new FakeInvocation(), (m, n) => throw new NotImplementedException()));
    }
}
