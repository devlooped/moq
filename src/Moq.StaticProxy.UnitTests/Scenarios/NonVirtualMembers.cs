#pragma warning disable CS0436
using System;
using Xunit;

namespace Moq.Scenarios.NonVirtualMembers
{
    /// <summary>
    /// Accessing members that can't be intercepted does not 
    /// generate recursive mocks for their return types.
    /// </summary>
    public class Test : IRunnable
    {
        public void Run()
        {
            var mock = Mock.Of<IServiceProvider>();

            Assert.NotNull(mock.GetType());
        }
    }
}
