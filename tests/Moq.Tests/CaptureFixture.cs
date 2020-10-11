// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
		public void ShouldOnlyCaptureParameterForSpecificArgumentBeforeCollection()
		{
			var items = new List<string>();
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(1, Capture.In(items)));

			mock.Object.DoSomething(1, "Hello!");
			mock.Object.DoSomething(2, "World");

			var expectedValues = new List<string> { "Hello!" };
			Assert.Equal(expectedValues, items);
		}

		[Fact]
		public void ShouldOnlyCaptureParameterForSpecificArgumentAfterCollection()
		{
			var items = new List<string>();
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(Capture.In(items), 1));

			mock.Object.DoSomething("Hello!", 1);
			mock.Object.DoSomething("World", 2);

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

		[Fact]
		public void ShouldNotCaptureParameterWhenConditionalSetupIsFalse()
		{
			var captures = new List<string>();
			var mock = new Mock<IFoo>();
			mock.When(() => false).Setup(m => m.DoSomething(Capture.In(captures)));

			mock.Object.DoSomething("X");

			Assert.Empty(captures);
		}

		[Fact]
		public void Can_be_used_with_Verify_to_replay_arguments()
		{
			var mock = new Mock<IFoo>();
			mock.Object.DoSomething("1");
			mock.Object.DoSomething("2");
			mock.Object.DoSomething("3");

			var args = new List<string>();
			mock.Verify(m => m.DoSomething(Capture.In(args)), Times.Exactly(3));

			Assert.Equal(new[] { "1", "2", "3" }, args);
		}

		public interface IFoo
		{
			void DoSomething(string s);
			void DoSomething(int i, string s);
			void DoSomething(string s, int i);
		}
	}
}
