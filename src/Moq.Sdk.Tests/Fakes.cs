using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Stunts;

namespace Moq.Sdk.Tests
{
    public class FakeMock : IStunt, IMocked
    {
        readonly DefaultMockRuntime mock;

        protected BehaviorPipeline Pipeline = new BehaviorPipeline();

        public FakeMock() => mock = new DefaultMockRuntime(this);

        public IList<IStuntBehavior> Behaviors => Pipeline.Behaviors;

        public IMockRuntime Runtime => mock;
    }

    public class FakeSetup : IMockSetup
    {
        public Func<IMethodInvocation, bool> AppliesTo { get; set; } = m => true;

        public FakeInvocation Invocation { get; set; } = new FakeInvocation();

        public IArgumentMatcher[] Matchers { get; set; } = new IArgumentMatcher[0];

        public Times? Occurrence { get; set; }

        public StateBag State { get; } = new StateBag();

        IMethodInvocation IMockSetup.Invocation => Invocation;

        public bool Equals(IMockSetup other) => base.Equals(other);

        bool IMockSetup.AppliesTo(IMethodInvocation actualInvocation) => AppliesTo(actualInvocation);
    }

    public class FakeInvocation : IMethodInvocation
    {
        public FakeInvocation() => Target = new Mocked();

        public IArgumentCollection Arguments { get; set; }

        public IDictionary<string, object> Context { get; set; }

        public MethodBase MethodBase { get; set; }

        public object Target { get; set; }

        public HashSet<Type> SkipBehaviors { get; } = new HashSet<Type>();

        public bool HasImplementation => false;

        public IMethodReturn CreateInvokeReturn(IArgumentCollection? arguments = null) => throw new NotImplementedException();

        public IMethodReturn CreateExceptionReturn(Exception exception) => new FakeReturn { Exception = exception };

        public IMethodReturn CreateValueReturn(object? returnValue, IArgumentCollection arguments) => new FakeReturn { ReturnValue = returnValue, Outputs = arguments };

        public bool Equals(IMethodInvocation other) => base.Equals(other);

        public bool Equals(object other, IEqualityComparer comparer) => base.Equals(other);

        public int GetHashCode(IEqualityComparer comparer) => base.GetHashCode();
    }

    public class FakeReturn : IMethodReturn
    {
        public IDictionary<string, object> Context { get; set; }

        public Exception? Exception { get; set; }

        public IArgumentCollection Outputs { get; set; }

        public object? ReturnValue { get; set; }
    }
}
