using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Moq.Tests.ThreadSafe
{
    public class ExpectToBeThreadSafe:IExpectToBeThreadSafe
    {
        #region IExpectToBeThreadSafe Members
        int someVariable = 0;
        public virtual void DoTheJob()
        {
            if (someVariable > 0)
                RaceCondition();
            someVariable++;
            Thread.Sleep(2);
            someVariable--;
        }

        #endregion

        #region IExpectToBeThreadSafe Members


        public virtual void RaceCondition()
        {
            
        }

        #endregion
    }
}
