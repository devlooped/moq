using System.ComponentModel;
using System.Linq;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// A custom behavior to enable calls to the base member virtual implementation.
    /// See <see cref="CallBaseExtension"/> method calls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CallBaseBehavior : IStuntBehavior
    {
        /// <inheritdoc />
        public bool AppliesTo(IMethodInvocation invocation) => true;

        /// <inheritdoc />
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            // Check if CallBase is configured at the Mock or Invocation level
            var shouldCallBase = MockRuntimeExtensions.GetCallBase(MockRuntime.Get(invocation.Target)) || invocation.Context.ContainsKey(nameof(IMock.CallBase));

            if (shouldCallBase)
            {
                // Skip the default value to force the base target member is executed
                invocation.SkipBehaviors.Add(typeof(DefaultValueBehavior));

                // If there is a matching setup for the current invocation, skip the strict 
                // behavior because CallBase should be called instead
                if (MockRuntime.Get(invocation.Target).Behaviors.OfType<IMockBehaviorPipeline>().Any(x => x.AppliesTo(invocation)))
                    invocation.SkipBehaviors.Add(typeof(StrictMockBehavior));
            }

            return next.Invoke(invocation, next);
        }
    }
}