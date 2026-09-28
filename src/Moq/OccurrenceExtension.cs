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
        /// Forwards to <see cref="AtLeastOnce{TSetup}(TSetup)"/>.
        /// </summary>
        public static TSetup Verifiable<TSetup>(this TSetup setup) where TSetup : ISetup => setup.Occurs(Times.AtLeastOnce);

        /// <summary>
        /// Specifies that the setup is expected to be called at least once.
        /// </summary>
        public static TSetup AtLeastOnce<TSetup>(this TSetup setup) where TSetup : ISetup => setup.Occurs(Times.AtLeastOnce);

        /// <summary>
        /// Specifies that the setup is expected to be called exactly once.
        /// </summary>
        public static TSetup Once<TSetup>(this TSetup setup) where TSetup : ISetup => setup.Occurs(Times.Once);

        /// <summary>
        /// Specifies that the setup is expected to never be called.
        /// </summary>
        public static TSetup Never<TSetup>(this TSetup setup) where TSetup : ISetup => setup.Occurs(Times.Never);

        /// <summary>
        /// Specifies that the setup is expected to be called exactly the 
        /// given <paramref name="callCount"/> number of times.
        /// </summary>
        public static TSetup Exactly<TSetup>(this TSetup setup, int callCount) where TSetup : ISetup => setup.Occurs(Times.Exactly(callCount));

        static TSetup Occurs<TSetup>(this TSetup setup, Times times) where TSetup : ISetup
        {
            var sdk = setup.Sdk;
            sdk.Occurrence = times;

            var runtime = setup.GetRuntime();
            if (!Verify.IsVerifying(runtime))
                // Ensures the setup is registered with the mock so it's verified later.
                runtime.GetPipeline(sdk);
            else if (!times.Validate(runtime.Invocations.Count(sdk.AppliesTo)))
                throw new VerifyException(runtime, sdk);

            return setup;
        }
    }
}
