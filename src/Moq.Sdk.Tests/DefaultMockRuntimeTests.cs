using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class DefaultMockRuntimeTests
    {
        [Fact]
        public void ThrowsIfNullStunt()
            => Assert.Throws<ArgumentNullException>(() => new DefaultMockRuntime(null));

        [Fact]
        public void AddsMockContextBehavior()
        {
            var mock = new DefaultMockRuntime(new FakeStunt());

            Assert.Contains(mock.Behaviors, x => x is MockContextBehavior);
        }

        [Fact]
        public void AddsMockRecordingBehavior()
        {
            var mock = new DefaultMockRuntime(new FakeStunt());

            Assert.Contains(mock.Behaviors, x => x is MockRecordingBehavior);
        }

        [Fact]
        public void PreventsDuplicateMockContextBehavior()
        {
            var mock = new DefaultMockRuntime(new FakeStunt());

            Assert.Throws<InvalidOperationException>(() => mock.Behaviors.Add(new MockContextBehavior()));
        }

        [Fact]
        public void PreventsDuplicateMockRecordingBehavior()
        {
            var mock = new DefaultMockRuntime(new FakeStunt());

            Assert.Throws<InvalidOperationException>(() => mock.Behaviors.Add(new MockRecordingBehavior()));
        }

        [Fact]
        public void TrackMockBehaviors()
        {
            var stunt = new FakeStunt();
            // Forces initialization of the default mock.
            Assert.NotNull(stunt.Runtime);

            var setup = new MockSetup(
                MethodInvocation.Create(stunt, typeof(FakeStunt).GetMethod("Do")),
                Array.Empty<IArgumentMatcher>());

            var initialBehaviors = stunt.Behaviors.Count;
            var behavior = new MockBehaviorPipeline(setup);

            stunt.AddBehavior(behavior);
            stunt.AddBehavior((m, n) => n(m, n));
            Assert.Equal(initialBehaviors + 2, stunt.Behaviors.Count);

            Assert.Single(stunt.Runtime.Setups);
            Assert.Same(behavior, stunt.Runtime.GetPipeline(setup));

            stunt.Behaviors.Remove(behavior);

            Assert.Equal(initialBehaviors + 1, stunt.Behaviors.Count);
            Assert.Empty(stunt.Runtime.Setups);
        }

        [Fact]
        public void AddPipelineForSetupIfMissing()
        {
            var stunt = new FakeStunt();
            // Forces initialization of the default mock.
            Assert.NotNull(stunt.Runtime);

            var initialBehaviors = stunt.Behaviors.Count;
            var setup = new MockSetup(
                MethodInvocation.Create(stunt, typeof(FakeStunt).GetMethod("Do")),
                Array.Empty<IArgumentMatcher>());

            var behavior = stunt.Runtime.GetPipeline(setup);

            Assert.NotNull(behavior);
            Assert.Equal(initialBehaviors + 1, stunt.Behaviors.Count);
            Assert.Single(stunt.Runtime.Setups);
        }

        [Fact]
        public void TracksTargetObject()
        {
            var stunt = new FakeStunt();
            Assert.Same(stunt, stunt.Runtime.Object);
        }

        [Fact]
        public void InitializesState()
            => Assert.NotNull(new FakeStunt().Runtime.State);

        class FakeStunt : IStunt, IMocked
        {
            readonly BehaviorPipeline pipeline = new BehaviorPipeline();
            DefaultMockRuntime mock;

            public IList<IStuntBehavior> Behaviors => pipeline.Behaviors;

            public IMockRuntime Runtime => LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMockRuntime(this));

            public void Do() => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod()));
        }
    }
}
