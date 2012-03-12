using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Tests.ThreadSafe
{
    public interface IExpectToBeThreadSafe
    {
        void DoTheJob();
        void RaceCondition();
    }
}
