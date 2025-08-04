using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq
{
    class VerifyResult<T> : List<IInvocation>, IVerifyResult<T> where T : class
    {
        public Mock<T> Mock { get; }

        public VerifyResult(Mock<T> mock, IEnumerable<IInvocation> invocations) : base(invocations)
        {
            this.Mock = mock;
        }
    }
}
