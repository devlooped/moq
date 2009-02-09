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
using Microsoft.Pex.Framework.Instrumentation;
using System.Diagnostics;
using Microsoft.Pex.Framework.Choices;
using Microsoft.Pex.Framework.Validation;

[assembly: PexInstrumentAssembly("Moq")]
[assembly: PexInstrumentAssembly("DynamicProxyGenAssembly2")]

namespace Mock.Tests
{
	[TestClass]
	[PexClass(Suite = "checkin")]
	public partial class ShouldExpectCallWithArgument
	{
		public interface IFoo
		{
			int DoInt(int i);
		}

		[TestMethod]
		[PexMethod(MaxBranches = int.MaxValue)]
		public void Moq()
		{
			// ShouldExpectCallWithArgument
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoInt(1)).Returns(11);
			mock.Setup(x => x.DoInt(2)).Returns(22);

			Assert.AreEqual(11, mock.Object.DoInt(1));
			Assert.AreEqual(22, mock.Object.DoInt(2));
		}

		[PexMethod(MaxBranches = int.MaxValue)]
		[PexExpectedGoals]
		public void MoqWithParameterValue(int y)
		{
			// ShouldExpectCallWithArgument
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoInt(1)).Returns(y);

			if (mock.Object.DoInt(1) == 4242)
				throw new PexGoalException();
		}

		[PexMethod(MaxBranches = int.MaxValue)]
		[PexExpectedGoals]
		public void MoqWithParameterDelegate(int y)
		{
			// ShouldExpectCallWithArgument
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoInt(1)).Returns(() => y);

			if (mock.Object.DoInt(1) == 4242)
				throw new PexGoalException();
		}

		[PexMethod(MaxBranches = int.MaxValue)]
		[PexExpectedGoals]
		[PexIgnore("TODO")]
		public void MoqWithSymbolicArgumentParameterValue(int y, int z)
		{
			// ShouldExpectCallWithArgument
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoInt(y)).Returns(() => z);

			if (mock.Object.DoInt(2323) == 4242)
				throw new PexGoalException();
		}
	}
}