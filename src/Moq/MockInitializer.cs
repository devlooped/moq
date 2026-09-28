using System.ComponentModel;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Provides the <see cref="Initialize"/> method for configuring the initial 
    /// set of behaviors for a given <see cref="MockBehavior"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MockInitializer
    {
        /// <summary>
        /// Clears any existing behavior (including any setups) and 
        /// adds the necessary behaviors to the <paramref name="mocked"/> so 
        /// that it behaves as specified by the <paramref name="behavior"/> 
        /// enumeration.
        /// </summary>
        /// <remarks>
        /// This method can be used by custom mocks to ensure they have the 
        /// same default behaviors as a mock created using <c>Mock.Of{T}</c>.
        /// </remarks>
        public static void Initialize(this IMocked mocked, MockBehavior behavior = MockBehavior.Loose)
        {
            var mock = mocked.Runtime;
            var behaviors = mock.Behaviors;
            (behaviors as ISupportInitialize)?.BeginInit();

            try
            {
                behaviors.Clear();

                mock.Behavior = behavior;

                behaviors.Add(new SetupScopeBehavior());
                behaviors.Add(new MockContextBehavior());
                behaviors.Add(new ConfigurePipelineBehavior());
                behaviors.Add(new MockRecordingBehavior());
                behaviors.Add(new EventBehavior());
                behaviors.Add(new PropertyBehavior { SetterRequiresSetup = behavior == MockBehavior.Strict });
                behaviors.Add(new DefaultEqualityBehavior());
                behaviors.Add(new RecursiveMockBehavior());
                behaviors.Add(new CallBaseBehavior());

                // Dynamically configured by the ConfigurePipelineBehavior.
                behaviors.Add(new StrictMockBehavior());

                var defaultValue = mock.State.GetOrAdd(() => new DefaultValueProvider());
                behaviors.Add(new DefaultValueBehavior(defaultValue));
                mock.State.Set(defaultValue);
            }
            finally
            {
                (behaviors as ISupportInitialize)?.EndInit();
            }
        }
    }
}
