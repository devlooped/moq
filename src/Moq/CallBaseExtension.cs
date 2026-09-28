using System.ComponentModel;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Extensions for calling base member virtual implementation.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CallBaseExtension
    {
        /// <summary>
        /// Specifies to call the base member virtual implementations by default.
        /// </summary>
        public static T CallBase<T>(this T target)
        {
            if (target is IMocked mocked && mocked != null)
            {
                // Configure CallBase at the Mock level
                MockRuntimeExtensions.SetCallBase(MockRuntime.Get(mocked), true);
            }
            else if (MockContext.CurrentInvocation != null)
            {
                // Configure CallBase at the invocation level
                MockRuntime.Get(MockContext.CurrentInvocation.Target)
                    .GetPipeline(MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>())
                    .Behaviors.Add(new AnonymousMockBehavior(
                         (m, i, next) =>
                         {
                             // set CallBase
                             i.Context[nameof(IMock.CallBase)] = true;
                             return next().Invoke(MockRuntime.Get(i.Target), i, next);
                         },
                         nameof(IMock.CallBase)));
            }
            // TODO: else throw?

            return target;
        }
    }
}