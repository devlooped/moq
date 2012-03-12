using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;

namespace Moq.Tests.ThreadSafe
{
    /// <summary>
    /// This is a test showing moq fails in multithread
    /// </summary>
    public class ThreadSafeExpectFailsWithThreadUnsafe
    {
        
        /*
        [Fact]
        public void ExpectFailWithStandardMock()
        {
            int concurrent = 8;
            var v = new Mock<ExpectToBeThreadSafe>();
            
            v.CallBase = true;
            Exception failedWithException = null;
            try
            {
                for (int i = 0; i < concurrent; ++i)
                {
                    ThreadPool.QueueUserWorkItem(
                        (k) =>
                        {
                            try
                            {
                                v.Object.DoTheJob();
                            }
                            catch (Exception inner)
                            {
                                failedWithException = inner;
                            }
                        }
                        );
                }
                Thread.Sleep(200);
                if (null == failedWithException)
                {
                    v.Verify(k => k.DoTheJob(), Times.Exactly(concurrent));
                    v.Verify(k => k.RaceCondition(), Times.AtLeastOnce());
                }
            }
            catch (Exception e)
            {
                //sometimes thread unsafe fatally fails 
                //so the test is obviously passed
                failedWithException = e;
            }
        }
          */
    }
}
