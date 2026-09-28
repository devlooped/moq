using System.ComponentModel;
using System.Linq;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Extensions for specifying occurrence for behavior specification 
    /// or verification.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OccurrenceExtension
    {
        /// <summary>
        /// Supports legacy API and forwards to <see cref="AtLeastOnce{TResult}(TResult)"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TResult Verifiable<TResult>(this TResult target) => AtLeastOnce(target);

        /// <summary>
        /// Specifies that the current fluent invocation is expected to be 
        /// called at least once.
        /// </summary>
        public static TResult AtLeastOnce<TResult>(this TResult target)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                setup.Occurrence = Sdk.Times.AtLeastOnce;
                var mock = MockRuntime.Get(setup.Invocation.Target);
                if (Verify.IsVerifying(mock))
                {
                    var calls = mock.Invocations.Where(call => setup.AppliesTo(call));
                    if (!calls.Any())
                        throw new VerifyException(mock, setup);
                }
            }
            else
            {
                // TODO: throw if no setup?
            }

            return target;
        }

        /// <summary>
        /// Specifies that the current fluent invocation is expected to be 
        /// called exactly once.
        /// </summary>
        public static TResult Once<TResult>(this TResult target)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                setup.Occurrence = Sdk.Times.Once;
                var mock = MockRuntime.Get(setup.Invocation.Target);
                if (Verify.IsVerifying(mock))
                {
                    var calls = mock.Invocations.Where(call => setup.AppliesTo(call)).Take(2).ToArray();
                    if (calls.Length != 1)
                        throw new VerifyException(mock, setup);
                }
            }
            else
            {
                // TODO: throw if no setup?
            }

            return target;
        }

        /// <summary>
        /// Specifies that the current fluent invocation is expected to never 
        /// be called.
        /// </summary>
        public static TResult Never<TResult>(this TResult target)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                setup.Occurrence = Sdk.Times.Never;
                var mock = MockRuntime.Get(setup.Invocation.Target);
                if (Verify.IsVerifying(mock))
                {
                    if (mock.Invocations.Where(call => setup.AppliesTo(call)).Any())
                        throw new VerifyException(mock, setup);
                }
            }
            else
            {
                // TODO: throw if no setup?
            }

            return target;
        }

        /// <summary>
        /// Specifies that the current fluent invocation is expected to be 
        /// called exactly the given <paramref name="callCount"/> number of times.
        /// </summary>
        public static TResult Exactly<TResult>(this TResult target, int callCount)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                setup.Occurrence = Sdk.Times.Exactly(callCount);
                var mock = MockRuntime.Get(setup.Invocation.Target);
                if (Verify.IsVerifying(mock))
                {
                    if (mock.Invocations.Where(call => setup.AppliesTo(call)).Count() != callCount)
                        throw new VerifyException(mock, setup);
                }
            }
            else
            {
                // TODO: throw if no setup?
            }

            return target;
        }
    }
}