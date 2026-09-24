using System.ComponentModel;
using System.Linq;
using Avatars;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// A custom behavior to enable calls to the base member virtual implementation.
    /// See <see cref="CallBaseExtension"/> method calls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CallBaseBehavior : IAvatarBehavior
    {
        /// <inheritdoc />
        public bool AppliesTo(IMethodInvocation invocation) => true;

        /// <inheritdoc />
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            // Check if CallBase is configured at the Mock or Invocation level
            var shouldCallBase = invocation.Target.AsMoq().CallBase || invocation.Context.ContainsKey(nameof(IMoq.CallBase));

            if (shouldCallBase)
            {
                // Skip the default value to force the base target member is executed
                invocation.SkipBehaviors.Add(typeof(DefaultValueBehavior));

                // If there is a matching setup for the current invocation, skip the strict 
                // behavior because CallBase should be called instead
                if (invocation.Target.AsMock().Behaviors.OfType<IMockBehaviorPipeline>().Any(x => x.AppliesTo(invocation)))
                    invocation.SkipBehaviors.Add(typeof(StrictMockBehavior));
            }

            return next.Invoke(invocation, next);
        }
    }
}