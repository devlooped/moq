using System;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// The configuration used to set up a <see cref="IMockBehaviorPipeline" />.
    /// </summary>
    public interface IMockSetup : IEquatable<IMockSetup>, IFluentInterface
    {
        /// <summary>
        /// The mock invocation that was intercepted for this setup.
        /// </summary>
		IMethodInvocation Invocation { get; }

        /// <summary>
        /// The <see cref="IArgumentMatcher"/>s used to set up the mock and that 
        /// will be used to evaluate the <see cref="AppliesTo(IMethodInvocation)"/> 
        /// method together with the original setup <see cref="Invocation"/>.
        /// </summary>
        IArgumentMatcher[] Matchers { get; }

        /// <summary>
        /// Gets or sets an occurrence constraint placed on the setup, for matching 
        /// or verification purposes.
        /// </summary>
        Times? Occurrence { get; set; }

        /// <summary>
        /// Arbitrary state associated with a setup.
        /// </summary>
        StateBag State { get; }

        /// <summary>
        /// Tests whether the setup applies to an actual invocation.
        /// </summary>
        /// <param name="actualInvocation">An actual invocation performed on the mock.
        /// </param>
        bool AppliesTo(IMethodInvocation actualInvocation);
    }
}