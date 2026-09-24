using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Avatars;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class DefaultMockTests
    {
        [Fact]
        public void ThrowsIfNullStunt()
            => Assert.Throws<ArgumentNullException>(() => new DefaultMock(null));

        [Fact]
        public void AddsMockContextBehavior()
        {
            var mock = new DefaultMock(new FakeStunt());

            Assert.Contains(mock.Behaviors, x => x is MockContextBehavior);
        }

        [Fact]
        public void AddsMockRecordingBehavior()
        {
            var mock = new DefaultMock(new FakeStunt());

            Assert.Contains(mock.Behaviors, x => x is MockRecordingBehavior);
        }

        [Fact]
        public void PreventsDuplicateMockContextBehavior()
        {
            var mock = new DefaultMock(new FakeStunt());

            Assert.Throws<InvalidOperationException>(() => mock.Behaviors.Add(new MockContextBehavior()));
        }

        [Fact]
        public void PreventsDuplicateMockRecordingBehavior()
        {
            var mock = new DefaultMock(new FakeStunt());

            Assert.Throws<InvalidOperationException>(() => mock.Behaviors.Add(new MockRecordingBehavior()));
        }

        [Fact]
        public void TrackMockBehaviors()
        {
            var stunt = new FakeStunt();
            // Forces initialization of the default mock.
            Assert.NotNull(stunt.Mock);

            var setup = new MockSetup(
                MethodInvocation.Create(stunt, typeof(FakeStunt).GetMethod("Do")),
                Array.Empty<IArgumentMatcher>());

            var initialBehaviors = stunt.Behaviors.Count;
            var behavior = new MockBehaviorPipeline(setup);

            stunt.AddBehavior(behavior);
            stunt.AddBehavior((m, n) => n(m, n));
            Assert.Equal(initialBehaviors + 2, stunt.Behaviors.Count);

            Assert.Single(stunt.Mock.Setups);
            Assert.Same(behavior, stunt.Mock.GetPipeline(setup));

            stunt.Behaviors.Remove(behavior);

            Assert.Equal(initialBehaviors + 1, stunt.Behaviors.Count);
            Assert.Empty(stunt.Mock.Setups);
        }

        [Fact]
        public void AddPipelineForSetupIfMissing()
        {
            var stunt = new FakeStunt();
            // Forces initialization of the default mock.
            Assert.NotNull(stunt.Mock);

            var initialBehaviors = stunt.Behaviors.Count;
            var setup = new MockSetup(
                MethodInvocation.Create(stunt, typeof(FakeStunt).GetMethod("Do")),
                Array.Empty<IArgumentMatcher>());

            var behavior = stunt.Mock.GetPipeline(setup);

            Assert.NotNull(behavior);
            Assert.Equal(initialBehaviors + 1, stunt.Behaviors.Count);
            Assert.Single(stunt.Mock.Setups);
        }

        [Fact]
        public void TracksTargetObject()
        {
            var stunt = new FakeStunt();
            Assert.Same(stunt, stunt.Mock.Object);
        }

        [Fact]
        public void InitializesState()
            => Assert.NotNull(new FakeStunt().Mock.State);

        class FakeStunt : IAvatar, IMocked
        {
            readonly BehaviorPipeline pipeline = new BehaviorPipeline();
            DefaultMock mock;

            public IList<IAvatarBehavior> Behaviors => pipeline.Behaviors;

            public IMock Mock => LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMock(this));

            public void Do() => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod()));
        }
    }
}
