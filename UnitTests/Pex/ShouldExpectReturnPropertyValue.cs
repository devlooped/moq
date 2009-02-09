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
    public partial class ShouldExpectReturnPropertyValue
    {
        public interface IFoo
        {
            int ValueProperty { get; }
        }

        [PexMethod(MaxBranches = int.MaxValue)]
        public void Moq()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.ValueProperty).Returns(25);
            Assert.AreEqual(25, mock.Object.ValueProperty);
        }
    }
}
