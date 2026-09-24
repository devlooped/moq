using System;
using Avatars;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockBehaviorPipelineTests
    {
        [Fact]
        public void AppliesToSetup()
        {
            var invocation = new MethodInvocation(new FakeMock(), typeof(object).GetMethod(nameof(object.ToString)));
            var setup = new MockSetup(invocation, Array.Empty<IArgumentMatcher>());
            var pipeline = new MockBehaviorPipeline(setup);

            Assert.True(pipeline.AppliesTo(invocation));
        }

        [Fact]
        public void ExecutesNextIfNoBehaviors()
        {
            var invocation = new MethodInvocation(new FakeMock(), typeof(object).GetMethod(nameof(object.ToString)));
            var pipeline = new MockBehaviorPipeline(new MockSetup(invocation, Array.Empty<IArgumentMatcher>()));

            Assert.NotNull(pipeline.Execute(invocation, (m, n) => m.CreateReturn()));
        }

        [Fact]
        public void ExecutesBehavior()
        {
            var invocation = new MethodInvocation(new FakeMock(), typeof(object).GetMethod(nameof(object.ToString)));
            var pipeline = new MockBehaviorPipeline(new MockSetup(invocation, Array.Empty<IArgumentMatcher>()));

            pipeline.Behaviors.Add(new AnonymousMockBehavior((m, i, n) => i.CreateReturn(), "test"));

            Assert.NotNull(pipeline.Execute(invocation, (m, n) => throw new NotImplementedException()));
        }

        [Fact]
        public void ExecutesBehaviorAndNext()
        {
            var invocation = new MethodInvocation(new FakeMock(), typeof(object).GetMethod(nameof(object.ToString)));
            var pipeline = new MockBehaviorPipeline(new MockSetup(invocation, Array.Empty<IArgumentMatcher>()));

            pipeline.Behaviors.Add(new AnonymousMockBehavior((m, i, n) => n().Invoke(m, i, n), "test"));

            Assert.NotNull(pipeline.Execute(invocation, (m, n) => m.CreateReturn()));
        }

        [Fact]
        public void ThrowsIfTargetNotIMocked()
        {
            var invocation = new MethodInvocation(new object(), typeof(object).GetMethod(nameof(object.ToString)));
            var pipeline = new MockBehaviorPipeline(new MockSetup(invocation, Array.Empty<IArgumentMatcher>()));

            pipeline.Behaviors.Add(new AnonymousMockBehavior((m, i, n) => i.CreateReturn(), "test"));

            Assert.Throws<ArgumentException>(() => pipeline.Execute(invocation, (m, n) => throw new NotImplementedException()));
        }
    }
}
