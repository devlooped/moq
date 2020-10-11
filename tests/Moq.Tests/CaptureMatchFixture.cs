// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class CaptureMatchFixture
	{
		[Fact]
		public void CanRunCaptureCallback()
		{
			var capturedValue = string.Empty;
			var captureMatch = new CaptureMatch<string>(s => capturedValue = s);

			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(Capture.With(captureMatch)));

			mock.Object.DoSomething("Hello!");

			Assert.Equal("Hello!", capturedValue);
		}

		[Fact]
		public void CanRunCaptureCallbackWithPredicate()
		{
			var capturedValue = string.Empty;
			var captureMatch = new CaptureMatch<string>(s => capturedValue += s, s => s.StartsWith("W"));

			var mock = new Mock<IFoo>();
			mock.Setup(x => x.DoSomething(Capture.With(captureMatch)));

			mock.Object.DoSomething("Hello!");
			mock.Object.DoSomething("World!");

			Assert.Equal("World!", capturedValue);
		}

		public interface IFoo
		{
			void DoSomething(string item);
		}
	}
}
