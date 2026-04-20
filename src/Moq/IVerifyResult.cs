using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq
{
    /// <summary>
    /// A collection of invocations verified by a call to mock.Verify() or one of its variations
    /// </summary>
    /// <typeparam name="T">The type being mocked and verified</typeparam>
    public interface IVerifyResult<T> : IReadOnlyList<IInvocation> where T: class
    {
        /// <summary>
        /// The mock that was verified
        /// </summary>
        Mock<T> Mock { get; }
    }
}
