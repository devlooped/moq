using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Represents a unit of behavior (such as returning a value, 
    /// invoking a callback or throwing an exception) that applies 
    /// to a mock when a given setup is matched (such as a particular 
    /// method being called with specific arguments).
    /// </summary>
    public interface IMockBehavior
    {
        /// <summary>
        /// Executes the behavior for the given mock and method invocation.
        /// </summary>
        /// <param name="mock">The mock the invocation is being performed on.</param>
        /// <param name="invocation">The current method invocation.</param>
        /// <param name="next">Delegate to invoke the next behavior in the pipeline.</param>
        /// <returns>The result of the method invocation.</returns>
        IMethodReturn Execute(IMock mock, IMethodInvocation invocation, GetNextMockBehavior next);
    }

    /// <summary>
    /// Method signature for getting the next behavior in a pipeline.
    /// </summary>
    /// <returns>The delegate to invoke the next behavior in a pipeline.</returns>
	public delegate ExecuteMockDelegate GetNextMockBehavior();

    /// <summary>
    /// Method signature for invoking the next behavior in a pipeline.
    /// </summary>
    /// <param name="mock">The mock the invocation is performed on.</param>
    /// <param name="invocation">The current method invocation.</param>
    /// <param name="next">Delegate to invoke the next behavior in the pipeline.</param>
    /// <returns>The result of the method invocation.</returns>
    public delegate IMethodReturn ExecuteMockDelegate(IMock mock, IMethodInvocation invocation, GetNextMockBehavior next);

}