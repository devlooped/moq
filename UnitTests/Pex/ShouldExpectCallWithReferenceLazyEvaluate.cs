// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Pex.Framework.Suppression;
using Microsoft.Pex.Framework.Choices;

namespace Mock.Tests
{
    [TestClass]
    [PexClass(Suite = "checkin")]
    public partial class ShouldExpectCallWithReferenceLazyEvaluate
    {
        public interface IFoo
        {
            int Parse(string value);
        }

        [PexMethod(MaxBranches = int.MaxValue)]
        public void Moq()
        {
            int a = 25;
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Parse(a.ToString())).Returns(() => a);

            a = 10;
            Assert.AreEqual(10, mock.Object.Parse("10"));
        }
    }
}
