using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;


namespace Moq.Tests.ThreadSafety
{
    public interface IFoo
    {
        int OneMethod(string a);
        double AProperty { get; set; }
    }
    public class SingleThreadExpectations
    {
        
        [Fact]
        public void SinglethreadMethodCallFailsWhenOtherThreadCalls()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(k => k.OneMethod(It.IsAny<string>()))
                .SingleThread()
                .Returns(1);
            Assert.Equal(1, mock.Object.OneMethod(string.Empty));
            ManualResetEvent mre = new ManualResetEvent(false);
            bool signaled = false;
            ThreadPool.QueueUserWorkItem(c => 
            {
                try
                {
                    mock.Object.OneMethod(string.Empty);
                }
                catch (MockException me)
                {
                    if (me.Reason == MockException.ExceptionReason.SingleThread)
                    {
                        signaled = true;
                    }
                }
                mre.Set();
            }
            );
            mre.WaitOne();
            Assert.True(signaled);
        }
        [Fact]
        public void SinglethreadPropertyGetFailsWhenOtherThreadCalls()
        {
            var mock = new Mock<IFoo>();
            mock.SetupGet(k => k.AProperty)
                .SingleThread()
                .Returns(1);
            Assert.Equal(1, mock.Object.AProperty);
            ManualResetEvent mre = new ManualResetEvent(false);
            bool signaled = false;
            ThreadPool.QueueUserWorkItem(c =>
            {
                try
                {
                    var x = mock.Object.AProperty;
                }
                catch (MockException me)
                {
                    if (me.Reason == MockException.ExceptionReason.SingleThread)
                    {
                        signaled = true;
                    }
                }
                mre.Set();
            }
            );
            mre.WaitOne();
            Assert.True(signaled);
        }
        [Fact]
        public void SinglethreadPropertySetFailsWhenOtherThreadCalls()
        {
            var mock = new Mock<IFoo>();
            mock.SetupSet((k) => k.AProperty=It.IsAny<double>())
                .SingleThread();
                
            
            ManualResetEvent mre = new ManualResetEvent(false);
            bool signaled = false;
            ThreadPool.QueueUserWorkItem(c =>
            {
                try
                {
                    mock.Object.AProperty=15;
                }
                catch (MockException me)
                {
                    if (me.Reason == MockException.ExceptionReason.SingleThread)
                    {
                        signaled = true;
                    }
                }
                mre.Set();
            }
            );
            mre.WaitOne();
            mock.VerifySet(k => k.AProperty=15);
            Assert.True(signaled);
        }
    }
}
