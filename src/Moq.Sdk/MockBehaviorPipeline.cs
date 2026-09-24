using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avatars;

namespace Moq.Sdk
{
    /// <summary>
    /// Default implementation of <see cref="IMockBehaviorPipeline"/>, which 
    /// provides a sub-pipeline of behaviors within the <see cref="IAvatar"/>'s 
    /// <see cref="BehaviorPipeline"/>, which is only run if the current invocation 
    /// matches the <see cref="Setup"/>.
    /// </summary>
    [DebuggerDisplay("{Setup}")]
    public class MockBehaviorPipeline : IMockBehaviorPipeline
    {
        /// <inheritdoc />
        public MockBehaviorPipeline(IMockSetup setup) => Setup = setup;

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public ObservableCollection<IMockBehavior> Behaviors { get; } = new ObservableCollection<IMockBehavior>();

        /// <inheritdoc />
        public IMockSetup Setup { get; }

        /// <summary>
        /// Delegates to the <see cref="IMockSetup.AppliesTo(IMethodInvocation)"/> to determine 
        /// if the current behavior applies to the given invocation.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation) => Setup.AppliesTo(invocation);

        /// <summary>
        /// Executes the effective sub-pipeline that a <see cref="IMockBehavior"/> provides, 
        /// where all sub-pipeline behaviors automatically apply to the invocation, since they 
        /// are filtered as a whole according to the <see cref="Setup"/>.
        /// </summary>
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            var mock = (invocation.Target as IMocked)?.Mock ?? throw new ArgumentException(ThisAssembly.Strings.TargetNotMock);

            // NOTE: the mock behavior is like a sub-pipeline within the overall stunt 
            // behavior pipeline, where all the behaviors added automatically apply 
            // since they all share the same AppliesTo implementation, which is the setup 
            // itself.

            // Regardless of the configured mock behaviors or subsequent pipeline behaviors, 
            // we have matched a call, meaning the strict mode behavior, if configured at all, 
            // should always be skipped, since we consider this a successfull match.
            invocation.SkipBehavior<StrictMockBehavior>();

            if (Behaviors.Count == 0)
                return next.Invoke(invocation, next);

            var index = 0;
            var result = Behaviors[0].Execute(mock, invocation, () =>
            {
                ++index;
                return (index < Behaviors.Count) ?
                    Behaviors[index].Execute :
                    // Adapt the GetNextBehavior to our mock version
                    new ExecuteMockDelegate((m, i, n) => next.Invoke(i, next));
            });

            return result;
        }
    }
}
