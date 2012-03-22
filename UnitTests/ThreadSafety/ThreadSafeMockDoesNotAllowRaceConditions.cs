using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;

namespace Moq.Tests.ThreadSafe
{
    public class ThreadSafeMockDoesNotAllowRaceConditions
    {
       
        [Fact]
        public void ExpectAvoidRaceConditions()
        {
            int concurrent = 8;
            var v = new Mock<ExpectToBeThreadSafe>();

            v.CallBase = true;
            for (int i = 0; i < concurrent; ++i)
            {
                ThreadPool.QueueUserWorkItem(
                    (k) =>
                    {
                        v.Object.DoTheJob();
                    }
                    );
            }

            Thread.Sleep(200);
            v.Verify(k => k.DoTheJob(), Times.Exactly(concurrent));
            v.Verify(k => k.RaceCondition(), Times.Never());
        }
    }
}
