using Moq.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Moq
{
    using Moq.Proxy;

    /// <summary>
    /// Provides additional methods on mocks.
    /// </summary>
    /// <remarks>
    /// Those methods are useful for Testeroids support. 
    /// </remarks>
    public static class MockExtensionsTesteroids
    {
        /// <summary>
        /// Resets the current count of calls for the specified method.
        /// </summary>
        /// <param name="mock">The mock whose call need to be reset.</param>
        public static void ResetAllCallCounts(this Mock mock) 
        {
            var calls = (IList<ICallContext>)mock.Interceptor.ActualCalls;
            calls.Clear();
        } 
    }
}
