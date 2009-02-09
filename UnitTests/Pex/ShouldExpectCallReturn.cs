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
    public partial class ShouldExpectCallReturn
    {
        [TestMethod]
        [PexMethod(MaxBranches = int.MaxValue)]
        public void Moq()
        {
            // ShouldExpectCallReturn
            var mock = new Mock<ICloneable>();
            var clone = new object();

            mock.Setup(x => x.Clone()).Returns(clone);

            Assert.AreSame(clone, mock.Object.Clone());
        }
    }
}
