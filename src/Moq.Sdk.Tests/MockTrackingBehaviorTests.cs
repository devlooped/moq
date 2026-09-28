using System.Reflection;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockTrackingBehaviorTests
    {
        [Fact]
        public void SetsCurrentInvocationAndSetup()
        {
            var target = new TrackingMock();
            var invocation = new MethodInvocation(target, typeof(TrackingMock).GetMethod(nameof(TrackingMock.Do)));
            var tracking = new MockContextBehavior();

            Assert.NotNull(tracking.Execute(invocation, (m, n) => m.CreateReturn()));

            Assert.Same(invocation, MockContext.CurrentInvocation);
            Assert.NotNull(MockContext.CurrentSetup);
            Assert.True(MockContext.CurrentSetup.AppliesTo(invocation));
        }

        [Fact]
        public void RecordsInvocation()
        {
            var target = new TrackingMock();
            var invocation = new MethodInvocation(target, typeof(TrackingMock).GetMethod(nameof(TrackingMock.Do)));
            var recording = new MockRecordingBehavior();

            Assert.NotNull(recording.Execute(invocation, (m, n) => m.CreateReturn()));

            Assert.Single(target.Runtime.Invocations);
        }

        [Fact]
        public void SkipInvocationRecordingIfSetupScopeActive()
        {
            var target = new TrackingMock();
            var invocation = new MethodInvocation(target, typeof(TrackingMock).GetMethod(nameof(TrackingMock.Do)));
            var tracking = new MockContextBehavior();

            using (new SetupScope())
            {
                Assert.NotNull(tracking.Execute(invocation, (m, n) => m.CreateReturn()));
            }

            Assert.Empty(target.Runtime.Invocations);
        }

        class TrackingMock : FakeMock
        {
            public void Do() => Pipeline.Execute(new MethodInvocation(this, MethodBase.GetCurrentMethod()));
        }
    }
}
