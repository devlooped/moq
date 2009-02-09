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

namespace Mock.Tests
{
    [TestClass]
    [PexClass(Suite = "checkin")]
    public partial class ShouldMatchPredicateArgument
    {
        public interface IFoo
        {
            int Duplicate(int i);
        }

        [TestMethod]
        [PexMethod(MaxBranches = int.MaxValue)]
        public void Moq()
        {
            var mock = new Mock<IFoo>();

            mock.Setup(x => x.Duplicate(It.Is<int>(value => value < 5 && value > 0)))
                .Returns(() => 1);

            Assert.AreEqual(1, mock.Object.Duplicate(3));
            Assert.AreEqual(0, mock.Object.Duplicate(0));
        }
    }
}
