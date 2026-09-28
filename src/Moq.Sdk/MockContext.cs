using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Provides information about the current mock call 
    /// context.
    /// </summary>
    public static class MockContext
    {
        /// <summary>
        /// The most recent invocation performed on the mock, tracked 
        /// by the <see cref="MockContextBehavior"/>.
        /// </summary>
        public static IMethodInvocation? CurrentInvocation
        {
            get => CallContext<IMethodInvocation>.GetData();
            set => CallContext<IMethodInvocation>.SetData(value);
        }

        /// <summary>
        /// The last invocation on the mock, turned into an <see cref="IMockSetup"/> 
        /// ready for use together with the <see cref="IMock.GetPipeline(IMockSetup)"/> 
        /// method to locate the matching <see cref="IMockBehaviorPipeline"/> to add new behaviors
        /// when a matching invocation is performed.
        /// </summary>
        /// <remarks>
        /// This property is also tracked and populated by the 
        /// <see cref="MockContextBehavior"/>.
        /// </remarks>
        public static IMockSetup? CurrentSetup
        {
            get => CallContext<IMockSetup>.GetData();
            set => CallContext<IMockSetup>.SetData(value);
        }
    }
}
