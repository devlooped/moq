using System.Collections.Generic;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Provides introspection information about a mock.
    /// </summary>
    public interface IMock : IStunt
    {
        /// <summary>
        /// Returns a <see cref="IMockBehaviorPipeline"/> for the given <see cref="IMockSetup"/>.
        /// </summary>
        /// <param name="setup">The setup that equals the returned <see cref="IMockBehaviorPipeline.Setup"/>.</param>
        IMockBehaviorPipeline GetPipeline(IMockSetup setup);

        /// <summary>
        /// Invocations performed on the mock so far.
        /// </summary>
        ICollection<IMethodInvocation> Invocations { get; }

        /// <summary>
        /// The mock object this introspection data belongs to.
        /// </summary>
        object Object { get; }

        /// <summary>
        /// Arbitrary state associated with a mock instance.
        /// </summary>
        StateBag State { get; set; }

        /// <summary>
        /// The list of mock behavior pipelines configured for this mock.
        /// </summary>
        IEnumerable<IMockBehaviorPipeline> Setups { get; }
    }
}
