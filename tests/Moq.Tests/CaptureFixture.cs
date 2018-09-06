// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using Xunit;

namespace Moq.Tests
{
	public class CaptureFixture
	{
		[Fact]
		public void CanCaptureAnyParameterInCollection()
		{
			var items = new List<string>();
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(Capture.In(items)));

			mock.Object.DoSomething("Hello!");

			var expectedValues = new List<string> { "Hello!" };
			Assert.Equal(expectedValues, items);
		}

		[Fact]
		public void CanCaptureSpecificParameterInCollection()
		{
			var items = new List<string>();
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(Capture.In(items, p => p.StartsWith("W"))));

			mock.Object.DoSomething("Hello!");
			mock.Object.DoSomething("World!");

			var expectedValues = new List<string> { "World!" };
			Assert.Equal(expectedValues, items);
		}

		public interface IFoo
		{
			void DoSomething(string s);
		}
	}
}
