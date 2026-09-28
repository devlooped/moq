using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Provides configuration information for a mock.
    /// </summary>
    public interface IMoq : IMock
    {
        /// <summary>
        /// Gets the <see cref="MockBehavior"/> of the mock.
        /// </summary>
        MockBehavior Behavior { get; set; }

        /// <summary>
        /// Gets the default value behavior of the mock. 
        /// Only available for <see cref="MockBehavior.Loose"/> mocks.
        /// </summary>
        DefaultValueProvider DefaultValue { get; set; }

        /// <summary>
		/// Whether the base member virtual implementation will be called for mocked classes if no setup is matched.
		/// Defaults to <see langword="false"/>.
        /// </summary>
        bool CallBase { get; set; }
    }
}